using App.Constants;
using App.Utils;

namespace App;

/// <summary>
/// Main application form – primarily UI event handlers.
/// </summary>
public partial class MainForm : Form
{
    #region Lifecycle

    /// <summary>
    /// Constructor wires up the Designer.
    /// </summary>
    public MainForm()
    {
        InitializeComponent();
    }

    /// <summary>
    /// First-load: set defaults and register hotkey.
    /// </summary>
    private void MainForm_Load(object sender, EventArgs e)
    {
        // Status labels
        inputCountLabel.Text = LabelFormatter.SetInputCountLabel(inputCount);
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
        configPathText.Text = configPathTextDefault;
        loadOnStartupCheck.Checked = false;

        // Ensure the hotkey works minimized/unfocused.
        RegisterGlobalHotkey(hotKey);
        PopulateKeyDropdown();
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
            NormalizeStopTime();

            if (startDate.HasValue && startDate.Value > DateTime.Now)
            {
                EnterScheduledMode();
                return;
            }
            inputCountTimer.Interval = TimeUtils.ToMilliseconds(intervalInput.Value);
        }

        isRunning = !isRunning;
        ApplyRunningUiState(isRunning);
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

            inputCountTimer.Interval = TimeUtils.ToMilliseconds(intervalInput.Value);
            inputCountTimer.Start();

            startStopButton.Text = "Stop";
            startStopButton.BackColor = UiColors.StopRed;
            SetStartButtonScheduledVisuals(false);
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
    /// Tick: send target input + counts + run-for-count enforcement.
    /// </summary>
    private void InputCount_Tick(object sender, EventArgs e)
    {
        if (ShouldStopNow())
        {
            StartStopButton_Click(sender, EventArgs.Empty);
            scheduleEnableStopCheck.Checked = false;
            return;
        }

        PerformTargetInput();

        inputCount++;
        inputCountLabel.Text = LabelFormatter.SetInputCountLabel(inputCount);

        if (runForCountRadio.Checked)
        {
            forcedInputCount++;
            if (forcedInputCount >= runCountInput.Value)
            {
                StartStopButton_Click(sender, e);
                forcedInputCount = forcedInputCountDefault;
                return;
            }
        }
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
            activeTimerSeconds = activeTimerSecondsDefault;
            inputCount = inputCountDefault;
            forcedInputCount = forcedInputCountDefault;
            timerLabel.Text = LabelFormatter.SetTimeLabel(activeTimerSeconds);
            inputCountLabel.Text = LabelFormatter.SetInputCountLabel(inputCount);
            intervalInput.Value = TimeUtils.ToSeconds(inputIntervalDefault);
            runCountInput.Value = runCountInputDefault;
            runUntilStoppedRadio.Checked = true;

            // Binding state/UI
            isHotKeyBinding = false;
            isTargetKeyBinding = false;
            hotKey = hotKeyDefault;
            keybindButton.Text = hotKey.ToString();
            targetKey = targetKeyDefault;
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
            startStopButton.Text = "Start";
            startStopButton.BackColor = UiColors.StartGreen;

            // Re-apply default global hotkey
            RegisterGlobalHotkey(hotKey);

            configPathText.Text = configPathTextDefault;
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
        int nextStep = sequenceGrid.Rows.Count + 1;

        decimal delay = TimeUtils.ToSeconds(inputIntervalDefault);
        string key = Keys.LButton.ToString();

        sequenceGrid.Rows.Add(nextStep, key, delay);

        sequenceGrid.ClearSelection();
        sequenceGrid.Rows[^1].Selected = true;
    }

    /// <summary>
    /// On click button to delete a selected key sequence.
    /// </summary>
    private void SequenceRemoveButton_Click(object? sender, EventArgs e)
    {
        if (sequenceGrid.SelectedRows.Count > 0)
        {
            int rowIndex = sequenceGrid.SelectedRows[0].Index;
            sequenceGrid.Rows.RemoveAt(rowIndex);
            RenumberSequenceSteps();
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

        var min = TimeUtils.ToSeconds(intervalMinimum);
        var max = TimeUtils.ToSeconds(intervalMaximum);

        if (seconds < min || seconds > max)
        {
            MessageBox.Show($"Delay must be between {min:N1} and {max:N1} seconds.",
                "Out of range", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            e.Cancel = true;
        }
    }

    /// <summary>
    /// Rounds the delay value to 1 decimal place when editing ends.
    /// </summary>
    private void SequenceGrid_CellEndEdit(object? sender, DataGridViewCellEventArgs e)
    {
        if (sequenceGrid.Columns[e.ColumnIndex].Name != "colDelayMs" || e.RowIndex < 0)
            return;

        var cell = sequenceGrid.Rows[e.RowIndex].Cells[e.ColumnIndex];
        var text = cell.Value?.ToString() ?? string.Empty;

        if (decimal.TryParse(text, out var seconds))
        {
            cell.Value = Math.Round(seconds, 1).ToString("0.0");
        }
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
        // Allow control keys
        if (char.IsControl(e.KeyChar)) return;

        // Allow digits
        if (char.IsDigit(e.KeyChar)) return;

        // Allow one dot
        if (e.KeyChar == '.' && sender is TextBox tb && !tb.Text.Contains('.')) return;

        // Otherwise block
        e.Handled = true;
    }

    #endregion

    #region Config Tab

    /// <summary>
    /// On click method to save the current configuration to a file.
    /// Current support is json format.
    /// </summary>
    private void SaveConfigButton_Click(object? sender, EventArgs e)
    {
        SaveFileDialog saveFileDialog = new()
        {
            Filter = fileFormatFilter,
            FilterIndex = 1,
            RestoreDirectory = true,
            InitialDirectory = configPathText.Text,
            FileName = defaultConfigFileName,
            Title = "Select a location to save current configuration"
        };

        if (saveFileDialog.ShowDialog() == DialogResult.OK)
        {
            string filePath = saveFileDialog.FileName;
            SaveConfigurationToFile(filePath);
            MessageBox.Show($"Settings successfully saved to: {filePath}", "Save Successful", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }
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
            Filter = fileFormatFilter,
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
    /// Loads the file at startup when its checked.
    /// </summary>
    private void LoadOnStartupCheck_CheckedChanged(object? sender, EventArgs e)
    {
        // TODO: Decide what to do for persistance and startup config

        if (loadOnStartupCheck.Checked)
        {
            MessageBox.Show("Startup config coming soon...", "Not implemented",
            MessageBoxButtons.OK, MessageBoxIcon.Information);
            loadOnStartupCheck.Checked = false;
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
