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
            inputCountTimer.Interval = (int)intervalInput.Value;
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

            inputCountTimer.Interval = (int)intervalInput.Value;
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
        // General tab
        activeTimerSeconds = activeTimerSecondsDefault;
        inputCount = inputCountDefault;
        forcedInputCount = forcedInputCountDefault;
        timerLabel.Text = LabelFormatter.SetTimeLabel(activeTimerSeconds);
        inputCountLabel.Text = LabelFormatter.SetInputCountLabel(inputCount);
        intervalInput.Value = inputIntervalDefault;
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

    private void SequenceAddButton_Click(object? sender, EventArgs e)
        => MessageBox.Show("TODO: Add sequence step dialog.", "Not implemented",
            MessageBoxButtons.OK, MessageBoxIcon.Information);

    private void SequenceEditButton_Click(object? sender, EventArgs e)
        => MessageBox.Show("TODO: Edit selected sequence step.", "Not implemented",
            MessageBoxButtons.OK, MessageBoxIcon.Information);

    private void SequenceRemoveButton_Click(object? sender, EventArgs e)
        => MessageBox.Show("TODO: Remove selected sequence step.", "Not implemented",
            MessageBoxButtons.OK, MessageBoxIcon.Information);

    private void SequenceMoveUpButton_Click(object? sender, EventArgs e)
        => MessageBox.Show("TODO: Move selected step up.", "Not implemented",
            MessageBoxButtons.OK, MessageBoxIcon.Information);

    private void SequenceMoveDownButton_Click(object? sender, EventArgs e)
        => MessageBox.Show("TODO: Move selected sequence step.", "Not implemented",
            MessageBoxButtons.OK, MessageBoxIcon.Information);

    #endregion

    #region Config Tab

    private void SaveConfigButton_Click(object? sender, EventArgs e)
        => MessageBox.Show("TODO: Serialize settings to file.", "Not implemented",
            MessageBoxButtons.OK, MessageBoxIcon.Information);

    private void LoadConfigButton_Click(object? sender, EventArgs e)
        => MessageBox.Show("TODO: Load settings from file.", "Not implemented",
            MessageBoxButtons.OK, MessageBoxIcon.Information);

    private void LoadOnStartupCheck_CheckedChanged(object? sender, EventArgs e)
    {
        // TODO: implement persist preference
    }

    private void OpenConfigFolderButton_Click(object? sender, EventArgs e)
        => MessageBox.Show("TODO: Open configuration folder.", "Not implemented",
            MessageBoxButtons.OK, MessageBoxIcon.Information);

    #endregion
}
