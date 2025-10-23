using System;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using App.Utils;

namespace App;

/// <summary>
/// Main application form containing runtime logic.
/// </summary>
public partial class MainForm : Form
{
    // ---- Runtime state ----
    private bool isRunning = isRunningDefault;
    private int activeTimerSeconds = activeTimerSecondsDefault;
    private int inputCount = inputCountDefault;
    private int forcedInputCount = forcedInputCountDefault;
    private bool isHotKeyBinding = false;
    private bool isTargetKeyBinging = false;
    private Keys hotKey = hotKeyDefault;
    private Keys targetKey = targetKeyDefault;
    private MouseBindFilter? _mouseFilter;

    private DateTime? startDate = null; // Not used yet
    private DateTime? stopTime = null; // Not used yet

    // ===== Global hotkey minimal shim =====
    private const int WM_HOTKEY = 0x0312;
    private const int HOTKEY_ID = 0xA11;
    private const uint MOD_NONE = 0x0000;

    [DllImport("user32.dll", SetLastError = true)]
    private static extern bool RegisterHotKey(IntPtr hWnd, int id, uint fsModifiers, uint vk);

    [DllImport("user32.dll", SetLastError = true)]
    private static extern bool UnregisterHotKey(IntPtr hWnd, int id);

    private void RegisterGlobalHotkey(Keys key)
    {
        try
        {
            if (IsHandleCreated)
            {
                // best-effort cleanup, ignore result
                UnregisterHotKey(this.Handle, HOTKEY_ID);
                _ = RegisterHotKey(this.Handle, HOTKEY_ID, MOD_NONE, (uint)key);
            }
        }
        catch { /* ignore */ }
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="MainForm"/> class.
    /// </summary>
    public MainForm()
    {
        InitializeComponent();
    }

    /// <summary>
    /// Handles first-load initialization (binds labels, sets defaults for new UI elements).
    /// </summary>
    /// <param name="sender">Event sender (the form).</param>
    /// <param name="e">Event arguments.</param>
    private void MainForm_Load(object sender, EventArgs e)
    {
        // Existing labels
        inputCountLabel.Text = Formatter.SetInputCountLabel(inputCount);
        timerLabel.Text = Formatter.SetTimeLabel(activeTimerSeconds);
        startStopButton.BackColor = Color.Green;

        // Placeholder defaults for new UI elements (no logic yet)
        // Schedule
        scheduleStartPicker.Value = DateTime.Now.AddMinutes(1);
        scheduleEnableStopCheck.Checked = false;
        scheduleStopPicker.Enabled = false;
        scheduleStopPicker.Value = DateTime.Now.AddMinutes(10);

        // Run mode
        runUntilStoppedRadio.Checked = true;
        runForCountRadio.Checked = false;

        // Config
        configPathText.Text = "<not set>";
        loadOnStartupCheck.Checked = false;

        // NEW: make the hotkey global from the start
        RegisterGlobalHotkey(hotKey);
    }

    protected override void OnHandleDestroyed(EventArgs e)
    {
        try { UnregisterHotKey(this.Handle, HOTKEY_ID); } catch { }
        base.OnHandleDestroyed(e);
    }

    protected override void WndProc(ref Message m)
    {
        if (m.Msg == WM_HOTKEY && m.WParam == (IntPtr)HOTKEY_ID)
        {
            // Toggle start/stop regardless of focus/minimized state
            StartStopButton_Click(startStopButton, EventArgs.Empty);
            return; // consume
        }
        base.WndProc(ref m);
    }

    /// <summary>
    /// Toggles the running state and starts/stops timers accordingly.
    /// </summary>
    /// <param name="sender">The Start/Stop button.</param>
    /// <param name="e">Event arguments.</param>
    private void StartStopButton_Click(object sender, EventArgs e)
    {
        // Apply field values before starting
        if (!isRunning)
        {
            inputCountTimer.Interval = (int)intervalInput.Value;
        }

        // Toggle state with every click
        isRunning = !isRunning;

        // Start/stop timers
        (isRunning ? (Action)runTimer.Start : runTimer.Stop)();
        (isRunning ? (Action)inputCountTimer.Start : inputCountTimer.Stop)();

        // Top Bar Items
        startStopButton.Text = isRunning ? "Stop" : "Start";
        startStopButton.BackColor = isRunning ? Color.Red : Color.Green;
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

        // Keys Tab
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
    /// <param name="sender">Run timer.</param>
    /// <param name="e">Event arguments.</param>
    private void RunTimer_Tick(object sender, EventArgs e)
    {
        activeTimerSeconds++;
        timerLabel.Text = Formatter.SetTimeLabel(activeTimerSeconds);
    }

    /// <summary>
    /// Increments the input counter and updates the label once per tick.
    /// </summary>
    /// <param name="sender">Input count timer.</param>
    /// <param name="e">Event arguments.</param>
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
    /// On changed method for run mode radio button.
    /// Disables if radio button is uncheckd.
    /// </summary>
    /// <param name="sender">Input count timer.</param>
    /// <param name="e">Event arguments.</param>
    private void RunForInputsSelectedChanged(object sender, EventArgs e)
    {
        runCountInput.Enabled = runForCountRadio.Checked;
    }

    /// <summary>
    /// Resets time, count, and interval values to defaults.
    /// </summary>
    /// <param name="sender">Reset button.</param>
    /// <param name="e">Event arguments.</param>
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

    /// <summary>
    /// Keybind button: enter/exit binding with visual indicator.
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
            RegisterGlobalHotkey(hotKey); // ensure global registration reflects current key
        }
    }

    /// <summary>
    /// Target key button: enter/exit binding with visual indicator (keyboard or mouse).
    /// </summary>
    private void TargetInputKeyButton_Click(object sender, EventArgs e)
    {
        isTargetKeyBinging = !isTargetKeyBinging;

        if (isTargetKeyBinging)
        {
            targetKeyButton.Text = "Press Any Key or Click (Esc to cancel)...";
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

    /// <summary>
    /// Placeholder to add a sequence step (not implemented).
    /// </summary>
    private void SequenceAddButton_Click(object? sender, EventArgs e)
    {
        MessageBox.Show("TODO: Add sequence step dialog.", "Not implemented",
            MessageBoxButtons.OK, MessageBoxIcon.Information);
    }

    /// <summary>
    /// Placeholder to edit a selected sequence step (not implemented).
    /// </summary>
    private void SequenceEditButton_Click(object? sender, EventArgs e)
    {
        MessageBox.Show("TODO: Edit selected sequence step.", "Not implemented",
            MessageBoxButtons.OK, MessageBoxIcon.Information);
    }

    /// <summary>
    /// Placeholder to remove a selected sequence step (not implemented).
    /// </summary>
    private void SequenceRemoveButton_Click(object? sender, EventArgs e)
    {
        MessageBox.Show("TODO: Remove selected sequence step.", "Not implemented",
            MessageBoxButtons.OK, MessageBoxIcon.Information);
    }

    /// <summary>
    /// Placeholder to move the selected step up (not implemented).
    /// </summary>
    private void SequenceMoveUpButton_Click(object? sender, EventArgs e)
    {
        MessageBox.Show("TODO: Move selected step up.", "Not implemented",
            MessageBoxButtons.OK, MessageBoxIcon.Information);
    }

    /// <summary>
    /// Placeholder to move the selected step down (not implemented).
    /// </summary>
    private void SequenceMoveDownButton_Click(object? sender, EventArgs e)
    {
        MessageBox.Show("TODO: Move selected sequence step.", "Not implemented",
            MessageBoxButtons.OK, MessageBoxIcon.Information);
    }

    /// <summary>
    /// Placeholder for reacting to target type selection changes (not implemented).
    /// </summary>
    private void TargetTypeCombo_SelectedIndexChanged(object? sender, EventArgs e)
    {
        // TODO: toggle UI based on target type if needed
    }

    /// <summary>
    /// Placeholder to serialize settings to a configuration file (not implemented).
    /// </summary>
    private void SaveConfigButton_Click(object? sender, EventArgs e)
    {
        MessageBox.Show("TODO: Serialize settings to file.", "Not implemented",
            MessageBoxButtons.OK, MessageBoxIcon.Information);
    }

    /// <summary>
    /// Placeholder to load settings from a configuration file (not implemented).
    /// </summary>
    private void LoadConfigButton_Click(object? sender, EventArgs e)
    {
        MessageBox.Show("TODO: Load settings from file.", "Not implemented",
            MessageBoxButtons.OK, MessageBoxIcon.Information);
    }

    /// <summary>
    /// Placeholder to persist preference for loading configuration on startup (not implemented).
    /// </summary>
    private void LoadOnStartupCheck_CheckedChanged(object? sender, EventArgs e)
    {
        // TODO: persist preference
    }

    /// <summary>
    /// Placeholder to open the configuration folder (not implemented).
    /// </summary>
    private void OpenConfigFolderButton_Click(object? sender, EventArgs e)
    {
        MessageBox.Show("TODO: Open configuration folder.", "Not implemented",
            MessageBoxButtons.OK, MessageBoxIcon.Information);
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

    /// <summary>
    /// Handles keyboard input for binding and hotkey toggle.
    /// </summary>
    private void MainForm_KeyDown(object? sender, KeyEventArgs e)
    {
        // Esc cancels binding modes
        if (isHotKeyBinding && e.KeyCode == Keys.Escape) { CancelHotkeyBinding(); e.Handled = true; return; }
        if (isTargetKeyBinging && e.KeyCode == Keys.Escape) { CancelTargetBinding(); e.Handled = true; return; }

        if (isHotKeyBinding)
        {
            hotKey = e.KeyCode;
            keybindButton.Text = hotKey.ToString();
            isHotKeyBinding = false;
            SetBindingStyle(keybindButton, false);   // restore visual state
            RegisterGlobalHotkey(hotKey);            // NEW: update global registration
            e.Handled = true;
            return;
        }

        if (isTargetKeyBinging)
        {
            // Keyboard target
            targetKey = e.KeyCode;
            targetKeyButton.Text = targetKey.ToString();
            isTargetKeyBinging = false;
            DisableMouseBinding();
            SetBindingStyle(targetKeyButton, false); // restore visual state
            e.Handled = true;
            return;
        }

        // Optional: focused-window hotkey toggle (still works when focused)
        if (e.KeyCode == hotKey)
        {
            StartStopButton_Click(startStopButton, EventArgs.Empty);
            e.Handled = true;
        }
    }

    /// <summary>
    /// Handles mouse input when binding a target (L/R/M).
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
        SetBindingStyle(targetKeyButton, false); // restore visual state
    }

    // ---- Mouse binding shim (captures clicks anywhere in app while binding) ----
    private sealed class MouseBindFilter : IMessageFilter
    {
        private readonly Action<MouseButtons> _onClick;
        public MouseBindFilter(Action<MouseButtons> onClick) => _onClick = onClick;

        private const int WM_LBUTTONDOWN = 0x0201;
        private const int WM_RBUTTONDOWN = 0x0204;
        private const int WM_MBUTTONDOWN = 0x0207;

        public bool PreFilterMessage(ref Message m)
        {
            switch (m.Msg)
            {
                case WM_LBUTTONDOWN: _onClick(MouseButtons.Left); return true;
                case WM_RBUTTONDOWN: _onClick(MouseButtons.Right); return true;
                case WM_MBUTTONDOWN: _onClick(MouseButtons.Middle); return true;
                default: return false;
            }
        }
    }

    private void EnableMouseBinding()
    {
        if (_mouseFilter != null) return;
        _mouseFilter = new MouseBindFilter(OnMouseBind);
        Application.AddMessageFilter(_mouseFilter);
    }

    private void DisableMouseBinding()
    {
        if (_mouseFilter == null) return;
        Application.RemoveMessageFilter(_mouseFilter);
        _mouseFilter = null;
    }

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
        SetBindingStyle(targetKeyButton, false); // restore visual state
    }

    private void SetBindingStyle(Button btn, bool isBinding)
    {
        if (isBinding)
        {
            btn.Enabled = false;
            btn.BackColor = Color.Goldenrod;
            btn.ForeColor = Color.Black;
        }
        else
        {
            btn.Enabled = true;
            btn.BackColor = Color.FromArgb(60, 64, 82);
            btn.ForeColor = ForeColor;
        }
    }

    // Cancel routines for Esc key
    private void CancelHotkeyBinding()
    {
        isHotKeyBinding = false;
        keybindButton.Text = hotKey.ToString();
        SetBindingStyle(keybindButton, false);
    }

    private void CancelTargetBinding()
    {
        isTargetKeyBinging = false;
        targetKeyButton.Text = targetKey.ToString();
        SetBindingStyle(targetKeyButton, false);
        DisableMouseBinding();
    }
}
