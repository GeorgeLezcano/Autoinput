using System.Runtime.InteropServices;
using App.Constants;
using App.Utils;

namespace App;

/// <summary>
/// Main application form containing runtime logic.
/// </summary>
public partial class MainForm : Form
{
    #region Fields & Runtime State

    // ---- Runtime state ----
    private bool isRunning = isRunningDefault;
    private int activeTimerSeconds = activeTimerSecondsDefault;
    private int inputCount = inputCountDefault;
    private int forcedInputCount = forcedInputCountDefault;

    // Binding state
    private bool isHotKeyBinding = false;
    private bool isTargetKeyBinging = false;
    private Keys hotKey = hotKeyDefault;
    private Keys targetKey = targetKeyDefault;
    private MouseBindFilter? mouseFilter;

    // Reserved (not used yet)
    private DateTime? startDate = null;
    private DateTime? stopTime = null;

    #endregion

    #region Win32 Global Hotkey

    [DllImport("user32.dll", SetLastError = true)]
    private static extern bool RegisterHotKey(IntPtr hWnd, int id, uint fsModifiers, uint vk);

    [DllImport("user32.dll", SetLastError = true)]
    private static extern bool UnregisterHotKey(IntPtr hWnd, int id);

    /// <summary>
    /// Registers the provided key as a system-wide hotkey for start/stop toggling.
    /// </summary>
    private void RegisterGlobalHotkey(Keys key)
    {
        try
        {
            if (IsHandleCreated)
            {
                UnregisterHotKey(Handle, Win32Hotkey.HOTKEY_ID);
                _ = RegisterHotKey(Handle, Win32Hotkey.HOTKEY_ID, Win32Hotkey.MOD_NONE, (uint)key);
            }
        }
        catch { /* do nothing */ }
    }

    #endregion

    #region Lifecycle 

    /// <summary>
    /// Initializes a new instance of the MainForm class.
    /// </summary>
    public MainForm()
    {
        InitializeComponent();
    }

    /// <summary>
    /// Handles first-load initialization (binds labels, sets defaults).
    /// </summary>
    private void MainForm_Load(object sender, EventArgs e)
    {
        // Status labels
        inputCountLabel.Text = Formatter.SetInputCountLabel(inputCount);
        timerLabel.Text = Formatter.SetTimeLabel(activeTimerSeconds);
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
        configPathText.Text = "<not set>";
        loadOnStartupCheck.Checked = false;

        // Ensure the hotkey works minimized/unfocused.
        RegisterGlobalHotkey(hotKey);
    }

    /// <summary>
    /// Ensures global hotkey is unregistered when handle is destroyed.
    /// </summary>
    protected override void OnHandleDestroyed(EventArgs e)
    {
        try { UnregisterHotKey(Handle, Win32Hotkey.HOTKEY_ID); } catch { }
        base.OnHandleDestroyed(e);
    }

    /// <summary>
    /// Handles WM_HOTKEY to toggle start/stop regardless of focus/minimized state.
    /// </summary>
    protected override void WndProc(ref Message m)
    {
        if (m.Msg == Win32Hotkey.WM_HOTKEY && m.WParam == (IntPtr)Win32Hotkey.HOTKEY_ID)
        {
            StartStopButton_Click(startStopButton, EventArgs.Empty);
            return;
        }
        base.WndProc(ref m);
    }

    #endregion

    #region General Tab: Run / Timers

    /// <summary>
    /// Toggles the running state and starts/stops timers accordingly.
    /// </summary>
    private void StartStopButton_Click(object sender, EventArgs e)
    {
        // Apply field values before starting
        if (!isRunning)
        {
            inputCountTimer.Interval = (int)intervalInput.Value;
        }

        // Toggle state
        isRunning = !isRunning;

        // Start/stop timers
        (isRunning ? (Action)runTimer.Start : runTimer.Stop)();
        (isRunning ? (Action)inputCountTimer.Start : inputCountTimer.Stop)();

        // Top Bar Items
        startStopButton.Text = isRunning ? "Stop" : "Start";
        startStopButton.BackColor = isRunning ? UiColors.StopRed : UiColors.StartGreen;
        resetButton.Enabled = !isRunning;

        // General Tab
        intervalInput.Enabled = !isRunning;

        // Schedule Tab
        scheduleStartPicker.Enabled = !isRunning && scheduleEnableStartCheck.Checked;
        scheduleEnableStartCheck.Enabled = !isRunning;
        scheduleStopPicker.Enabled = !isRunning && scheduleEnableStopCheck.Checked;
        scheduleEnableStopCheck.Enabled = !isRunning;

        // Run Mode Tab
        runUntilStoppedRadio.Enabled = !isRunning;
        runForCountRadio.Enabled = !isRunning;
        runCountInput.Enabled = !isRunning && runForCountRadio.Checked;

        // Sequence Tab
        sequenceIntervalInput.Enabled = !isRunning;
        sequenceAddButton.Enabled = !isRunning;
        sequenceEditButton.Enabled = !isRunning;
        sequenceRemoveButton.Enabled = !isRunning;
        sequenceMoveUpButton.Enabled = !isRunning;
        sequenceMoveDownButton.Enabled = !isRunning;

        // Keys controls
        keybindButton.Enabled = !isRunning;
        targetKeyButton.Enabled = !isRunning;

        // Config Tab
        saveConfigButton.Enabled = !isRunning;
        loadConfigButton.Enabled = !isRunning;
        loadOnStartupCheck.Enabled = !isRunning;
        configPathText.Enabled = !isRunning;
        openConfigFolderButton.Enabled = !isRunning;
    }

    /// <summary>
    /// Increments the active timer and updates the timer label once per tick.
    /// </summary>
    private void RunTimer_Tick(object sender, EventArgs e)
    {
        activeTimerSeconds++;
        timerLabel.Text = Formatter.SetTimeLabel(activeTimerSeconds);
    }

    /// <summary>
    /// Sends the target input each tick, increments counters, and auto-stops if needed.
    /// </summary>
    private void InputCount_Tick(object sender, EventArgs e)
    {
        PerformTargetInput();

        inputCount++;
        inputCountLabel.Text = Formatter.SetInputCountLabel(inputCount);

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
    /// Enables/disables "run for count" input based on radio selection.
    /// </summary>
    private void RunForInputsSelectedChanged(object sender, EventArgs e)
    {
        runCountInput.Enabled = runForCountRadio.Checked;
    }

    /// <summary>
    /// Resets time, count, intervals, and bindings back to defaults.
    /// </summary>
    private void ResetButton_Click(object sender, EventArgs e)
    {
        // General tab
        activeTimerSeconds = activeTimerSecondsDefault;
        inputCount = inputCountDefault;
        forcedInputCount = forcedInputCountDefault;
        timerLabel.Text = Formatter.SetTimeLabel(activeTimerSeconds);
        inputCountLabel.Text = Formatter.SetInputCountLabel(inputCount);
        intervalInput.Value = inputIntervalDefault;
        runCountInput.Value = runCountInputDefault;
        runUntilStoppedRadio.Checked = true;

        // Binding state/UI
        isHotKeyBinding = false;
        isTargetKeyBinging = false;
        hotKey = hotKeyDefault;
        keybindButton.Text = hotKey.ToString();
        targetKey = targetKeyDefault;
        targetKeyButton.Text = targetKey.ToString();

        // Schedule tab
        scheduleEnableStartCheck.Checked = false;
        scheduleEnableStopCheck.Checked = false;

        // Re-apply default global hotkey
        RegisterGlobalHotkey(hotKey);
    }

    #endregion

    #region General Tab: Key Bindings

    /// <summary>
    /// Enters/exits hotkey binding mode with visual feedback.
    /// </summary>
    private void KeybindButton_Click(object sender, EventArgs e)
    {
        isHotKeyBinding = !isHotKeyBinding;

        if (isHotKeyBinding)
        {
            keybindButton.Text = "Press Any Key (Esc to cancel)...";
            SetBindingStyle(keybindButton, true);
        }
        else
        {
            keybindButton.Text = hotKey.ToString();
            SetBindingStyle(keybindButton, false);
            RegisterGlobalHotkey(hotKey); // keep global registration in sync
        }
    }

    /// <summary>
    /// Enters/exits target binding mode for keyboard or mouse (with visual feedback).
    /// </summary>
    private void TargetInputKeyButton_Click(object sender, EventArgs e)
    {
        isTargetKeyBinging = !isTargetKeyBinging;

        if (isTargetKeyBinging)
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
    /// Handles keystrokes during binding modes and optional in-focus toggle.
    /// </summary>
    private void MainForm_KeyDown(object? sender, KeyEventArgs e)
    {
        // Esc cancels binding modes
        if (isHotKeyBinding && e.KeyCode == Keys.Escape) { CancelHotkeyBinding(); e.Handled = true; return; }
        if (isTargetKeyBinging && e.KeyCode == Keys.Escape) { CancelTargetBinding(); e.Handled = true; return; }

        // Capture hotkey during binding
        if (isHotKeyBinding)
        {
            hotKey = e.KeyCode;
            keybindButton.Text = hotKey.ToString();
            isHotKeyBinding = false;
            SetBindingStyle(keybindButton, false);
            RegisterGlobalHotkey(hotKey); // update global registration
            e.Handled = true;
            return;
        }

        // Capture target key during binding
        if (isTargetKeyBinging)
        {
            targetKey = e.KeyCode;
            targetKeyButton.Text = targetKey.ToString();
            isTargetKeyBinging = false;
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
    /// Handles mouse clicks during target binding (L/R/M).
    /// </summary>
    private void MainForm_MouseDown(object? sender, MouseEventArgs e)
    {
        if (!isTargetKeyBinging) return;

        targetKey = e.Button switch
        {
            MouseButtons.Left => Keys.LButton,
            MouseButtons.Right => Keys.RButton,
            MouseButtons.Middle => Keys.MButton,
            _ => targetKey
        };

        targetKeyButton.Text = targetKey.ToString();
        isTargetKeyBinging = false;
        DisableMouseBinding();
        SetBindingStyle(targetKeyButton, false);
    }

    /// <summary>
    /// Sends the currently bound target input (mouse click or key press).
    /// </summary>
    private void PerformTargetInput()
    {
        if (NativeInput.IsMouseKey(targetKey))
            NativeInput.ClickMouseButton(targetKey);
        else
            NativeInput.SendKeyPress(targetKey);
    }

    #endregion

    #region Binding Helpers

    /// <summary>
    /// Activates global mouse click capture during target binding.
    /// </summary>
    private void EnableMouseBinding()
    {
        if (mouseFilter != null) return;
        mouseFilter = new MouseBindFilter(OnMouseBind);
        Application.AddMessageFilter(mouseFilter);
    }

    /// <summary>
    /// Deactivates global mouse click capture during/after target binding.
    /// </summary>
    private void DisableMouseBinding()
    {
        if (mouseFilter == null) return;
        Application.RemoveMessageFilter(mouseFilter);
        mouseFilter = null;
    }

    /// <summary>
    /// Finalizes mouse selection for the target key.
    /// </summary>
    private void OnMouseBind(MouseButtons button)
    {
        targetKey = button switch
        {
            MouseButtons.Left => Keys.LButton,
            MouseButtons.Right => Keys.RButton,
            MouseButtons.Middle => Keys.MButton,
            _ => targetKey
        };
        targetKeyButton.Text = targetKey.ToString();
        isTargetKeyBinging = false;
        DisableMouseBinding();
        SetBindingStyle(targetKeyButton, false);
    }

    /// <summary>
    /// Applies/clears visual "binding" style on a button.
    /// </summary>
    private void SetBindingStyle(Button btn, bool isBinding)
    {
        if (isBinding)
        {
            btn.Enabled = false;
            btn.BackColor = UiColors.BindingBack;
            btn.ForeColor = UiColors.BindingFore;
        }
        else
        {
            btn.Enabled = true;
            btn.BackColor = UiColors.ButtonBackDefault;
            btn.ForeColor = UiColors.FormFore;
        }
    }

    /// <summary>
    /// Cancels hotkey binding and restores the button UI.
    /// </summary>
    private void CancelHotkeyBinding()
    {
        isHotKeyBinding = false;
        keybindButton.Text = hotKey.ToString();
        SetBindingStyle(keybindButton, false);
    }

    /// <summary>
    /// Cancels target binding and restores the button UI.
    /// </summary>
    private void CancelTargetBinding()
    {
        isTargetKeyBinging = false;
        targetKeyButton.Text = targetKey.ToString();
        SetBindingStyle(targetKeyButton, false);
        DisableMouseBinding();
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
    {
        MessageBox.Show("TODO: Add sequence step dialog.", "Not implemented",
            MessageBoxButtons.OK, MessageBoxIcon.Information);
    }

    private void SequenceEditButton_Click(object? sender, EventArgs e)
    {
        MessageBox.Show("TODO: Edit selected sequence step.", "Not implemented",
            MessageBoxButtons.OK, MessageBoxIcon.Information);
    }

    private void SequenceRemoveButton_Click(object? sender, EventArgs e)
    {
        MessageBox.Show("TODO: Remove selected sequence step.", "Not implemented",
            MessageBoxButtons.OK, MessageBoxIcon.Information);
    }

    private void SequenceMoveUpButton_Click(object? sender, EventArgs e)
    {
        MessageBox.Show("TODO: Move selected step up.", "Not implemented",
            MessageBoxButtons.OK, MessageBoxIcon.Information);
    }

    private void SequenceMoveDownButton_Click(object? sender, EventArgs e)
    {
        MessageBox.Show("TODO: Move selected sequence step.", "Not implemented",
            MessageBoxButtons.OK, MessageBoxIcon.Information);
    }

    #endregion

    #region Config Tab

    private void SaveConfigButton_Click(object? sender, EventArgs e)
    {
        MessageBox.Show("TODO: Serialize settings to file.", "Not implemented",
            MessageBoxButtons.OK, MessageBoxIcon.Information);
    }

    private void LoadConfigButton_Click(object? sender, EventArgs e)
    {
        MessageBox.Show("TODO: Load settings from file.", "Not implemented",
            MessageBoxButtons.OK, MessageBoxIcon.Information);
    }

    private void LoadOnStartupCheck_CheckedChanged(object? sender, EventArgs e)
    {
        // TODO: persist preference
    }

    private void OpenConfigFolderButton_Click(object? sender, EventArgs e)
    {
        MessageBox.Show("TODO: Open configuration folder.", "Not implemented",
            MessageBoxButtons.OK, MessageBoxIcon.Information);
    }

    #endregion
}
