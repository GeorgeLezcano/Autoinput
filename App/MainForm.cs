using App.Constants;
using App.Models;
using App.Utils;

namespace App;

/// <summary>
/// Main application form – primarily UI event handlers.
/// </summary>
public partial class MainForm : Form
{
    #region Lifecycle

    private UserGuideForm? _manual;

    private List<Sequence> _sequences = [
        new Sequence()
    ];

    private int _currentSequenceIndex = AppDefault.SequenceIndex;
    private bool _isSyncingSequenceName = false;
    private bool _suppressSequencePickerSync = false;
    private int _sequenceStepIndex = 0;

    /// <summary>
    /// Constructor wires up the Designer.
    /// </summary>
    public MainForm()
    {
        InitializeComponent();
        SetStyle(ControlStyles.OptimizedDoubleBuffer | ControlStyles.AllPaintingInWmPaint | ControlStyles.UserPaint, true);
        UpdateStyles();
    }

    /// <summary>
    /// First-load: set defaults and register hotkey.
    /// </summary>
    private void MainForm_Load(object sender, EventArgs e)
    {
        // Tries to oad default config file first 
        string exeDir = AppDomain.CurrentDomain.BaseDirectory;
        string defaultConfigPath = Path.Combine(exeDir, AppDefault.DefaultConfigFileName);

        if (File.Exists(defaultConfigPath))
        {
            LoadConfigurationFromFile(defaultConfigPath);
            return;
        }

        // Status labels
        inputCountLabel.Text = LabelFormatter.SetInputCountLabel(inputCount, sequenceModeCheck.Checked);
        timerLabel.Text = LabelFormatter.SetTimeLabel(activeTimerSeconds);
        startStopButton.BackColor = UiColors.StartGreen;

        // Schedule defaults
        scheduleStartPicker.Value = DateTime.Now.AddMinutes(1);
        scheduleEnableStopCheck.Checked = false;
        scheduleStopPicker.Enabled = false;
        scheduleStopPicker.Value = DateTime.Now.AddMinutes(10);

        // Run mode defaults
        runUntilStoppedRadio.Checked = true;
        runForCountRadio.Checked = false;

        // Config defaults
        configPathText.Text = AppDefault.ConfigPathText;

        // Ensure the hotkey works minimized/unfocused.
        RegisterGlobalHotkey(hotKey);

        // Sequence defaults
        PopulateKeyDropdown();
        RefreshSequencePicker();
        sequencePicker.SelectedIndex = 0;
        _currentSequenceIndex = 0;
        SyncSequenceUiFromSelection();
    }

    /// <summary>
    /// Occurs when the user attempts to close the application.
    /// Prompts to save unsaved configuration changes and cancels
    /// closing if the user chooses Cancel or aborts saving.
    /// </summary>
    private void MainForm_FormClosing(object sender, FormClosingEventArgs e)
    {
        if (isRunning || isScheduled)
        {
            runTimer.Stop();
            inputCountTimer.Stop();
            isRunning = false;
            isScheduled = false;
        }

        var result = MessageBox.Show(
            "All unsaved changes will be lost after closing.\n\n" +
            "Do you want to save the current configuration before exiting?",
            "Confirm Exit",
            MessageBoxButtons.YesNoCancel,
            MessageBoxIcon.Warning,
            MessageBoxDefaultButton.Button3
        );

        if (result == DialogResult.Cancel)
        {
            e.Cancel = true;
            return;
        }

        if (result == DialogResult.Yes)
        {
            if (!TryPromptAndSaveConfig())
            {
                e.Cancel = true;
                return;
            }
        }
    }

    /// <summary>
    /// Ensure timers and global hotkey are stopped/unregistered.
    /// </summary>
    protected override void OnHandleDestroyed(EventArgs e)
    {
        try { scheduleTimer?.Stop(); scheduleTimer = null; } catch { }
        try { UnregisterHotKey(Handle, Win32Hotkey.HOTKEY_ID); } catch { }
        base.OnHandleDestroyed(e);
    }

    /// <summary>
    /// Handle WM_HOTKEY to toggle Start/Stop regardless of focus/minimized state.
    /// </summary>
    protected override void WndProc(ref Message m)
    {
        if (m.Msg == Win32Hotkey.WM_HOTKEY && m.WParam == Win32Hotkey.HOTKEY_ID)
        {
            StartStopButton_Click(startStopButton, EventArgs.Empty);
            return;
        }
        base.WndProc(ref m);
    }

    #endregion

    #region General Tab: Run / Timers

    /// <summary>
    /// Toggle running/scheduled and start/stop timers accordingly.
    /// </summary>
    private void StartStopButton_Click(object sender, EventArgs e)
    {
        if (isScheduled)
        {
            ExitScheduledMode();
            return;
        }

        bool togglingOn = !isRunning;
        if (togglingOn)
        {
            SnapshotScheduleSelections();

            if (scheduleEnableStartCheck.Checked && startDate.HasValue && startDate.Value <= DateTime.Now)
            {
                scheduleEnableStartCheck.Checked = false;
                startDate = null;
            }

            if (scheduleEnableStopCheck.Checked && stopTime.HasValue && stopTime.Value <= DateTime.Now)
            {
                scheduleEnableStopCheck.Checked = false;
                stopTime = null;
            }

            NormalizeStopTime();

            if (startDate.HasValue && startDate.Value > DateTime.Now)
            {
                EnterScheduledMode();
                return;
            }
            inputCountTimer.Interval = TimeUtils.ToMilliseconds(intervalInput.Value);
            _sequenceStepIndex = 0;
        }

        isRunning = !isRunning;
        ApplyRunningUiState(isRunning);

        //if (isRunning) WindowState = FormWindowState.Minimized;
    }

    /// <summary>
    /// Promote from “Scheduled” to active when start time is reached.
    /// </summary>
    private void ScheduleTimer_Tick(object? sender, EventArgs e)
    {
        if (!isScheduled || !startDate.HasValue) return;

        if (DateTime.Now >= startDate.Value)
        {
            StopScheduleTimer();
            isScheduled = false;

            _sequenceStepIndex = 0;

            inputCountTimer.Interval = TimeUtils.ToMilliseconds(intervalInput.Value);
            inputCountTimer.Start();

            //WindowState = FormWindowState.Minimized;

            startStopButton.Text = AppDefault.StopBtnLabel;
            startStopButton.BackColor = UiColors.StopRed;
            SetStartButtonScheduledVisuals(false);

            scheduleEnableStartCheck.Checked = false;
            startDate = null;
        }
    }

    /// <summary>
    /// Tick: elapsed timer + scheduled stop enforcement.
    /// </summary>
    private void RunTimer_Tick(object sender, EventArgs e)
    {
        if (ShouldStopNow())
        {
            StartStopButton_Click(sender, EventArgs.Empty);
            scheduleEnableStopCheck.Checked = false;
            return;
        }

        activeTimerSeconds++;
        timerLabel.Text = LabelFormatter.SetTimeLabel(activeTimerSeconds);
    }

    /// <summary>
    /// Tick: send input based on mode + counts + run-for-count enforcement.
    /// - Regular mode: same behavior as before (send target key every interval).
    /// - Sequence mode: walk steps; delay between steps is per-row Delay, and
    ///   after the last step we wait the General interval before the next sequence.
    ///   Input Count increments once per whole sequence.
    /// </summary>
    private void InputCount_Tick(object sender, EventArgs e)
    {
        if (EnforceScheduledStopIfDue(sender, e)) return;

        if (!sequenceModeCheck.Checked)
        {
            DoSingleTargetTick(sender, e);
            return;
        }

        DoSequenceTick(sender, e);
    }

    /// <summary>
    /// Toggle “run for count” input enablement.
    /// </summary>
    private void RunForInputsSelected_Changed(object sender, EventArgs e)
    {
        runCountInput.Enabled = runForCountRadio.Checked;
    }

    /// <summary>
    /// Reset timers, counts, key bindings, and schedule—back to defaults.
    /// </summary>
    private void ResetButton_Click(object sender, EventArgs e)
    {
        var result = MessageBox.Show(
            "This will reset all settings to their default values.\n\n" +
            "Any unsaved changes will be lost. Do you want to proceed?",
            "Confirm Reset",
            MessageBoxButtons.YesNo,
            MessageBoxIcon.Warning,
            MessageBoxDefaultButton.Button2
        );

        if (result == DialogResult.Yes)
        {
            // General tab
            activeTimerSeconds = AppDefault.ActiveTimerSeconds;
            inputCount = AppDefault.InputCount;
            forcedInputCount = AppDefault.ForcedInputCount;
            timerLabel.Text = LabelFormatter.SetTimeLabel(activeTimerSeconds);
            inputCountLabel.Text = LabelFormatter.SetInputCountLabel(inputCount, sequenceModeCheck.Checked);
            intervalInput.Value = TimeUtils.ToSeconds(AppDefault.InputInterval);
            runCountInput.Value = AppDefault.RunCountInput;
            runUntilStoppedRadio.Checked = true;
            _sequenceStepIndex = 0;
            
            holdTargetCheck.Checked = false;
            HoldTargetCheck_CheckedChanged(null, EventArgs.Empty);
    
            // Binding state/UI
            isHotKeyBinding = false;
            isTargetKeyBinding = false;
            hotKey = AppDefault.HotKey;
            keybindButton.Text = hotKey.ToString();
            targetKey = AppDefault.TargetKey;
            targetKeyButton.Text = targetKey.ToString();

            // Schedule fields
            scheduleEnableStartCheck.Checked = false;
            scheduleEnableStopCheck.Checked = false;
            startDate = null;
            stopTime = null;
            isScheduled = false;
            StopScheduleTimer();
            SetStartButtonScheduledVisuals(false);

            // Top bar visuals
            startStopButton.Text = AppDefault.StartBtnLabel;
            startStopButton.BackColor = UiColors.StartGreen;

            // Re-apply default global hotkey
            RegisterGlobalHotkey(hotKey);

            configPathText.Text = AppDefault.ConfigPathText;

            // Sequence tab
            ResetSequences();
            sequenceModeCheck.Checked = false;
        }
    }

    /// <summary>
    /// Opens the internal User Guide window (modeless, themed).
    /// </summary>
    private void UserGuideButton_Click(object? sender, EventArgs e)
    {
        if (_manual is null || _manual.IsDisposed)
        {
            _manual = new UserGuideForm
            {
                StartPosition = FormStartPosition.CenterParent
            };
            _manual.Show(this);
        }
        else
        {
            _manual.BringToFront();
            _manual.Focus();
        }
    }

    #endregion

    #region General Tab: Key Bindings

    /// <summary>
    /// Enter/exit hotkey binding mode with visual feedback.
    /// </summary>
    private void KeybindButton_Click(object sender, EventArgs e)
    {
        isHotKeyBinding = !isHotKeyBinding;

        if (isHotKeyBinding)
        {
            keybindButton.Text = "Press Any Key (Esc to cancel)...";
            SetBindingStyle(keybindButton, true);
            try { UnregisterHotKey(Handle, Win32Hotkey.HOTKEY_ID); } catch { }
        }
        else
        {
            keybindButton.Text = hotKey.ToString();
            SetBindingStyle(keybindButton, false);
            RegisterGlobalHotkey(hotKey);
        }
    }

    /// <summary>
    /// Enter/exit target binding mode (keyboard/mouse) with visual feedback.
    /// </summary>
    private void TargetInputKeyButton_Click(object sender, EventArgs e)
    {
        isTargetKeyBinding = !isTargetKeyBinding;

        if (isTargetKeyBinding)
        {
            targetKeyButton.Text = "Press Any Key (Esc to cancel)...";
            SetBindingStyle(targetKeyButton, true);
            EnableMouseBinding();
        }
        else
        {
            targetKeyButton.Text = targetKey.ToString();
            SetBindingStyle(targetKeyButton, false);
            DisableMouseBinding();
        }
    }

    /// <summary>
    /// Capture keys for binding; Esc cancels. Hotkey toggles start/stop.
    /// </summary>
    private void MainForm_KeyDown(object? sender, KeyEventArgs e)
    {
        if (isHotKeyBinding && e.KeyCode == Keys.Escape) { CancelHotkeyBinding(); e.Handled = true; return; }
        if (isTargetKeyBinding && e.KeyCode == Keys.Escape) { CancelTargetBinding(); e.Handled = true; return; }

        if (isHotKeyBinding)
        {
            hotKey = e.KeyCode;
            keybindButton.Text = hotKey.ToString();
            isHotKeyBinding = false;
            SetBindingStyle(keybindButton, false);
            RegisterGlobalHotkey(hotKey);
            e.Handled = true;
            return;
        }

        if (isTargetKeyBinding)
        {
            if (e.KeyCode == hotKey)
            {
                MessageBox.Show(
                    "That key is already used as the Start/Stop hotkey. Pick a different target key.",
                    "Key conflict", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                e.Handled = true;
                return;
            }

            targetKey = e.KeyCode;
            targetKeyButton.Text = targetKey.ToString();
            isTargetKeyBinding = false;
            DisableMouseBinding();
            SetBindingStyle(targetKeyButton, false);
            e.Handled = true;
            return;
        }

        if (e.KeyCode == hotKey)
        {
            StartStopButton_Click(startStopButton, EventArgs.Empty);
            e.Handled = true;
        }
    }

    /// <summary>
    /// Capture mouse button as target key during binding.
    /// </summary>
    private void MainForm_MouseDown(object? sender, MouseEventArgs e)
    {
        if (!isTargetKeyBinding) return;

        targetKey = e.Button switch
        {
            MouseButtons.Left => Keys.LButton,
            MouseButtons.Right => Keys.RButton,
            MouseButtons.Middle => Keys.MButton,
            _ => targetKey
        };

        targetKeyButton.Text = targetKey.ToString();
        isTargetKeyBinding = false;
        DisableMouseBinding();
        SetBindingStyle(targetKeyButton, false);
    }

    /// <summary>
    /// Handler for the hold key checkbox.
    /// </summary>
    private void HoldTargetCheck_CheckedChanged(object? sender, EventArgs e)
    {
        var hold = holdTargetCheck.Checked;

        intervalInput.Enabled = !hold;
        labelIntervalHint.Enabled = !hold;

        runCountLabel.Enabled = !hold;

        if (hold)
        {
            runUntilStoppedRadio.Checked = true;

            runForCountRadio.Enabled = false;
            runCountInput.Enabled = false;
        }
        else
        {
            runForCountRadio.Enabled = true;
            runCountInput.Enabled = runForCountRadio.Checked;
        }
    }

    #endregion

    #region Schedule Tab

    /// <summary>
    /// Enables or disables the stop time picker based on the checkbox state.
    /// </summary>
    private void ScheduleEnableStopCheck_CheckedChanged(object? sender, EventArgs e)
    {
        scheduleStopPicker.Enabled = scheduleEnableStopCheck.Checked;
    }

    /// <summary>
    /// Enables or disables the start time picker based on the checkbox state.
    /// </summary>
    private void ScheduleEnableStartCheck_CheckedChanged(object? sender, EventArgs e)
    {
        scheduleStartPicker.Enabled = scheduleEnableStartCheck.Checked;
    }

    #endregion

    #region Sequence Tab

    /// <summary>
    /// On click button to add a new key sequence.
    /// </summary>
    private void SequenceAddButton_Click(object? sender, EventArgs e)
    {
        var seq = GetSelectedSequence();
        if (seq is null) return;

        seq.Steps.Add(new SequenceStep { Key = Keys.LButton, DelayMS = AppDefault.InputInterval });

        RefreshSequenceGridFrom(seq);

        if (sequenceGrid.Rows.Count > 0)
        {
            var last = sequenceGrid.Rows[^1];
            sequenceGrid.ClearSelection();
            last.Selected = true;
            sequenceGrid.FirstDisplayedScrollingRowIndex = last.Index;
        }
    }

    /// <summary>
    /// On click button to delete a selected key sequence.
    /// </summary>
    private void SequenceRemoveButton_Click(object? sender, EventArgs e)
    {
        var seq = GetSelectedSequence();
        if (seq is null) return;
        if (sequenceGrid.SelectedRows.Count == 0) return;

        int rowIndex = sequenceGrid.SelectedRows[0].Index;
        if (rowIndex < 0 || rowIndex >= seq.Steps.Count) return;

        seq.Steps.RemoveAt(rowIndex);
        RefreshSequenceGridFrom(seq);

        if (sequenceGrid.Rows.Count > 0)
        {
            int idx = Math.Min(rowIndex, sequenceGrid.Rows.Count - 1);
            sequenceGrid.Rows[idx].Selected = true;
        }
    }

    /// <summary>
    /// Enforce numeric delay within the same min/max rules as intervalInput (seconds).
    /// </summary>
    private void SequenceGrid_CellValidating(object? sender, DataGridViewCellValidatingEventArgs e)
    {
        if (sequenceGrid.Columns[e.ColumnIndex].Name != "colDelayMs" || e.RowIndex < 0)
            return;

        var text = e.FormattedValue?.ToString() ?? string.Empty;

        // Allow empty if you're okay with it; otherwise require a number:
        if (!decimal.TryParse(text, out var seconds))
        {
            MessageBox.Show("Delay must be a number in seconds (e.g., 0.5, 1.0, 2.5).",
                "Invalid value", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            e.Cancel = true;
            return;
        }

        var min = TimeUtils.ToSeconds(AppDefault.IntervalMinimum);
        var max = TimeUtils.ToSeconds(AppDefault.IntervalMaximum);

        if (seconds < min || seconds > max)
        {
            MessageBox.Show($"Delay must be between {min:N1} and {max:N1} seconds.",
                "Out of range", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            e.Cancel = true;
        }
    }

    /// <summary>
    /// Rounds the delay to 1 decimal and saves to the selected sequence.
    /// </summary>
    private void SequenceGrid_CellEndEdit(object? sender, DataGridViewCellEventArgs e)
    {
        if (sequenceGrid.Columns[e.ColumnIndex].Name == "colDelayMs" && e.RowIndex >= 0)
        {
            var cell = sequenceGrid.Rows[e.RowIndex].Cells[e.ColumnIndex];
            if (decimal.TryParse(cell.Value?.ToString(), out var seconds))
                cell.Value = Math.Round(seconds, 1).ToString("0.0");
        }

        var sequence = GetSelectedSequence();
        if (sequence is not null) SaveGridIntoSequence(sequence);
    }

    /// <summary>
    /// Limit typing in Delay to digits, control keys, and a single dot.
    /// </summary>
    private void SequenceGrid_EditingControlShowing(object? sender, DataGridViewEditingControlShowingEventArgs e)
    {
        if (sequenceGrid.CurrentCell?.OwningColumn?.Name == "colDelayMs" && e.Control is TextBox tb)
        {
            tb.KeyPress -= DelayTextBox_KeyPress;
            tb.KeyPress += DelayTextBox_KeyPress;
        }
    }

    private void DelayTextBox_KeyPress(object? sender, KeyPressEventArgs e)
    {
        if (char.IsControl(e.KeyChar)) return;
        if (char.IsDigit(e.KeyChar)) return;
        if (e.KeyChar == '.' && sender is TextBox tb && !tb.Text.Contains('.')) return;
        e.Handled = true;
    }

    /// <summary>
    /// Adds a new sequence with a unique default name and selects it.
    /// </summary>
    private void NewSequenceButton_Click(object? sender, EventArgs e)
    {
        if (_currentSequenceIndex >= 0 && _currentSequenceIndex < _sequences.Count)
            SaveGridIntoSequence(_sequences[_currentSequenceIndex]);

        var name = GetNextNewSequenceName();
        var seq = new Sequence { Name = name };
        _sequences.Add(seq);

        RefreshSequencePicker();
        sequencePicker.SelectedIndex = _sequences.Count - 1;
    }

    /// <summary>
    /// Removes the currently selected sequence, guarding the last default.
    /// </summary>
    private void RemoveSequenceButton_Click(object? sender, EventArgs e)
    {
        if (_sequences.Count <= 1)
        {
            MessageBox.Show("You must keep at least one sequence (the default).",
                "Cannot Remove", MessageBoxButtons.OK, MessageBoxIcon.Information);
            return;
        }

        var idx = sequencePicker.SelectedIndex;
        if (idx < 0 || idx >= _sequences.Count) return;

        if (_currentSequenceIndex >= 0 && _currentSequenceIndex < _sequences.Count)
            SaveGridIntoSequence(_sequences[_currentSequenceIndex]);

        var confirm = MessageBox.Show(
            $"Remove sequence \"{_sequences[idx].Name}\"?",
            "Confirm Removal",
            MessageBoxButtons.YesNo, MessageBoxIcon.Question,
            MessageBoxDefaultButton.Button2);

        if (confirm != DialogResult.Yes) return;

        _sequences.RemoveAt(idx);
        RefreshSequencePicker();

        var newIndex = Math.Min(idx, _sequences.Count - 1);
        sequencePicker.SelectedIndex = newIndex;
    }

    /// <summary>
    /// Renames the currently selected sequence and updates the picker item.
    /// </summary>
    private void SequenceNameText_TextChanged(object? sender, EventArgs e)
    {
        if (_isSyncingSequenceName) return;

        var idx = sequencePicker.SelectedIndex;
        if (idx < 0 || idx >= _sequences.Count) return;

        var newName = sequenceNameText.Text;
        if (string.IsNullOrWhiteSpace(newName)) return;

        _sequences[idx].Name = newName;
        sequencePicker.Items[idx] = newName;
    }

    /// <summary>
    /// When user picks a different sequence from the dropdown.
    /// </summary>
    private void SequencePicker_SelectedIndexChanged(object? sender, EventArgs e)
    {
        if (_suppressSequencePickerSync) return;

        var nextIndex = sequencePicker.SelectedIndex;
        if (nextIndex < 0 || nextIndex >= _sequences.Count) return;

        if (_currentSequenceIndex >= 0 && _currentSequenceIndex < _sequences.Count)
            SaveGridIntoSequence(_sequences[_currentSequenceIndex]);

        _currentSequenceIndex = nextIndex;
        SyncSequenceUiFromSelection();
    }

    /// <summary>
    /// Updates the label based on single input or sequence count.
    /// </summary>
    private void SequenceModeCheck_CheckedChanged(object? sender, EventArgs e)
    {
        inputCountLabel.Text = LabelFormatter.SetInputCountLabel(inputCount, sequenceModeCheck.Checked);
        _sequenceStepIndex = 0;
    }

    /// <summary>
    /// On changed handler for the sequence grid.
    /// </summary>
    private void SequenceGrid_CurrentCellDirtyStateChanged(object? sender, EventArgs e)
    {
        if (sequenceGrid.CurrentCell is null) return;

        if (sequenceGrid.IsCurrentCellDirty)
            sequenceGrid.CommitEdit(DataGridViewDataErrorContexts.Commit);
    }

    #endregion

    #region Config Tab

    /// <summary>
    /// On click method to save the current configuration to a file.
    /// Current support is json format.
    /// </summary>
    private void SaveConfigButton_Click(object? sender, EventArgs e)
    {
        TryPromptAndSaveConfig();
    }

    /// <summary>
    /// Attemps to load a file with configuration.
    /// If the format is incorrect, or values are out of range, display a 
    /// "Failed to load message".
    /// </summary>
    private void LoadConfigButton_Click(object? sender, EventArgs e)
    {
        OpenFileDialog openFileDialog = new()
        {
            Filter = AppDefault.FileFormatFilter,
            FilterIndex = 1,
            RestoreDirectory = true,
            InitialDirectory = configPathText.Text,
            Title = "Select a configuration file to load"
        };

        if (openFileDialog.ShowDialog() == DialogResult.OK)
        {
            LoadConfigurationFromFile(openFileDialog.FileName);
        }
    }

    /// <summary>
    /// Opens a folder to set the default config file path location.
    /// </summary>
    private void OpenConfigFolderButton_Click(object? sender, EventArgs e)
    {
        FolderBrowserDialog folderBrowserDialog = new()
        {
            InitialDirectory = configPathText.Text
        };

        if (folderBrowserDialog.ShowDialog() == DialogResult.OK)
        {
            configPathText.Text = folderBrowserDialog.SelectedPath;
        }
    }

    #endregion
}
