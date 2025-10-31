using System.Runtime.InteropServices;
using App.Constants;
using App.Utils;

namespace App;

/// <summary>
/// Helpers and reusable logic.
/// No UI handlers.
/// </summary>
partial class MainForm
{
    #region Fields & Runtime State

    // Runtime state 
    private bool isRunning = isRunningDefault;
    private bool isScheduled = isScheduledDefault;
    private int activeTimerSeconds = activeTimerSecondsDefault;
    private int inputCount = inputCountDefault;
    private int forcedInputCount = forcedInputCountDefault;

    // Binding state
    private bool isHotKeyBinding = false;
    private bool isTargetKeyBinding = false; 
    private Keys hotKey = hotKeyDefault;
    private Keys targetKey = targetKeyDefault;
    private MouseBindFilter? mouseFilter;

    // Scheduling snapshot
    private DateTime? startDate = null;
    private DateTime? stopTime = null;

    // Internal scheduler poller for Start Time
    private System.Windows.Forms.Timer? scheduleTimer;

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
        catch { /*do nothing*/ }
    }

    #endregion

    #region Schedule helpers

    /// <summary>
    /// Copy current schedule UI into snapshot fields.
    /// </summary>
    private void SnapshotScheduleSelections()
    {
        startDate = scheduleEnableStartCheck.Checked ? scheduleStartPicker.Value : null;
        stopTime = scheduleEnableStopCheck.Checked ? scheduleStopPicker.Value : null;
    }

    /// <summary>
    /// Clear a stale stop time.
    /// </summary>
    private void NormalizeStopTime()
    {
        if (stopTime.HasValue && stopTime.Value <= DateTime.Now)
        {
            stopTime = null;
            scheduleEnableStopCheck.Checked = false;
        }
    }

    /// <summary>
    /// True when a scheduled stop time is set and reached.
    /// </summary>
    private bool ShouldStopNow() => stopTime.HasValue && DateTime.Now >= stopTime.Value;

    /// <summary>
    /// Enter scheduled mode, disabling inputs and polling start time.
    /// </summary>
    private void EnterScheduledMode()
    {
        isRunning = true;
        isScheduled = true;

        runTimer.Start();
        inputCountTimer.Stop();

        SetStartButtonScheduledVisuals(true);
        resetButton.Enabled = false;

        // Disable most inputs while scheduled
        intervalInput.Enabled = false;

        scheduleStartPicker.Enabled = false;
        scheduleEnableStartCheck.Enabled = false;
        scheduleStopPicker.Enabled = scheduleEnableStopCheck.Checked;
        scheduleEnableStopCheck.Enabled = false;

        runUntilStoppedRadio.Enabled = false;
        runForCountRadio.Enabled = false;
        runCountInput.Enabled = false;

        sequenceIntervalInput.Enabled = false;
        sequenceAddButton.Enabled = false;
        sequenceEditButton.Enabled = false;
        sequenceRemoveButton.Enabled = false;
        sequenceMoveUpButton.Enabled = false;
        sequenceMoveDownButton.Enabled = false;

        keybindButton.Enabled = false;
        targetKeyButton.Enabled = false;

        saveConfigButton.Enabled = false;
        loadConfigButton.Enabled = false;
        loadOnStartupCheck.Enabled = false;
        configPathText.Enabled = false;
        openConfigFolderButton.Enabled = false;

        StartScheduleTimer();
    }

    /// <summary>
    /// Cancel scheduled mode and return to idle, re-enabling controls.
    /// </summary>
    private void ExitScheduledMode()
    {
        isScheduled = false;
        StopScheduleTimer();

        isRunning = false;
        runTimer.Stop();
        inputCountTimer.Stop();

        startDate = null;
        stopTime = null;

        RestoreUItoIddle();
        EnableAllControls();
    }

    /// <summary>
    /// Starts the timer for the schedule state.
    /// </summary>
    private void StartScheduleTimer()
    {
        scheduleTimer ??= new System.Windows.Forms.Timer { Interval = 200 };
        scheduleTimer.Tick -= ScheduleTimer_Tick;
        scheduleTimer.Tick += ScheduleTimer_Tick;
        scheduleTimer.Start();
    }

    /// <summary>
    /// Stops the timer for the schedule state.
    /// </summary>
    private void StopScheduleTimer()
    {
        if (scheduleTimer is null) return;
        scheduleTimer.Stop();
        scheduleTimer.Tick -= ScheduleTimer_Tick;
    }

    #endregion

    #region UI state helpers

    /// <summary>
    /// Apply “running” or “idle” UI state and start/stop timers accordingly.
    /// </summary>
    private void ApplyRunningUiState(bool running)
    {
        (running ? (Action)runTimer.Start : runTimer.Stop)();
        (running ? (Action)inputCountTimer.Start : inputCountTimer.Stop)();

        // Top bar items
        startStopButton.Text = running ? stopBtnLabel : startBtnLabel;
        startStopButton.BackColor = running ? UiColors.StopRed : UiColors.StartGreen;
        SetStartButtonScheduledVisuals(false);
        resetButton.Enabled = !running;

        // General tab
        intervalInput.Enabled = !running;

        // Schedule tab
        scheduleStartPicker.Enabled = !running && scheduleEnableStartCheck.Checked;
        scheduleEnableStartCheck.Enabled = !running;
        scheduleStopPicker.Enabled = !running && scheduleEnableStopCheck.Checked;
        scheduleEnableStopCheck.Enabled = !running;

        // Run mode tab
        runUntilStoppedRadio.Enabled = !running;
        runForCountRadio.Enabled = !running;
        runCountInput.Enabled = !running && runForCountRadio.Checked;

        // Sequence tab
        sequenceIntervalInput.Enabled = !running;
        sequenceAddButton.Enabled = !running;
        sequenceEditButton.Enabled = !running;
        sequenceRemoveButton.Enabled = !running;
        sequenceMoveUpButton.Enabled = !running;
        sequenceMoveDownButton.Enabled = !running;

        // Keys
        keybindButton.Enabled = !running;
        targetKeyButton.Enabled = !running;

        // Config
        saveConfigButton.Enabled = !running;
        loadConfigButton.Enabled = !running;
        loadOnStartupCheck.Enabled = !running;
        configPathText.Enabled = !running;
        openConfigFolderButton.Enabled = !running;
    }

    /// <summary>
    /// Restore UI to idle defaults.
    /// </summary>
    private void RestoreUItoIddle()
    {
        startStopButton.Text = startBtnLabel;
        startStopButton.BackColor = UiColors.StartGreen;
        SetStartButtonScheduledVisuals(false);
        resetButton.Enabled = true;
    }

    /// <summary>
    /// Re-enable all application controls (used when leaving scheduled).
    /// </summary>
    private void EnableAllControls()
    {
        intervalInput.Enabled = true;

        scheduleStartPicker.Enabled = scheduleEnableStartCheck.Checked;
        scheduleEnableStartCheck.Enabled = true;
        scheduleStopPicker.Enabled = scheduleEnableStopCheck.Checked;
        scheduleEnableStopCheck.Enabled = true;

        runUntilStoppedRadio.Enabled = true;
        runForCountRadio.Enabled = true;
        runCountInput.Enabled = runForCountRadio.Checked;

        sequenceIntervalInput.Enabled = true;
        sequenceAddButton.Enabled = true;
        sequenceEditButton.Enabled = true;
        sequenceRemoveButton.Enabled = true;
        sequenceMoveUpButton.Enabled = true;
        sequenceMoveDownButton.Enabled = true;

        keybindButton.Enabled = true;
        targetKeyButton.Enabled = true;

        saveConfigButton.Enabled = true;
        loadConfigButton.Enabled = true;
        loadOnStartupCheck.Enabled = true;
        configPathText.Enabled = true;
        openConfigFolderButton.Enabled = true;
    }

    /// <summary>
    /// Apply or clear “Scheduled” styling on Start/Stop button.
    /// </summary>
    private void SetStartButtonScheduledVisuals(bool scheduled)
    {
        if (scheduled)
        {
            startStopButton.Text = scheduleBtnLabel;
            startStopButton.BackColor = UiColors.ScheduledOrange;
            startStopButton.ForeColor = Color.White;
            startStopButton.FlatAppearance.MouseOverBackColor = UiColors.ScheduledOrangeHover;
            startStopButton.FlatAppearance.MouseDownBackColor = UiColors.ScheduledOrangeDown;
        }
        else
        {
            startStopButton.FlatAppearance.MouseOverBackColor = UiColors.ButtonBackDefault;
            startStopButton.FlatAppearance.MouseDownBackColor = UiColors.ButtonBackDefault;
        }
    }

    #endregion

    #region Binding helpers

    /// <summary>
    /// Global mouse capture during target binding.
    /// </summary>
    private void EnableMouseBinding()
    {
        if (mouseFilter != null) return;
        mouseFilter = new MouseBindFilter(OnMouseBind);
        Application.AddMessageFilter(mouseFilter);
    }

    /// <summary>
    /// Stop mouse capture.
    /// </summary>
    private void DisableMouseBinding()
    {
        if (mouseFilter == null) return;
        Application.RemoveMessageFilter(mouseFilter);
        mouseFilter = null;
    }

    /// <summary>
    /// Finalize mouse selection as target key.
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
        isTargetKeyBinding = false;
        DisableMouseBinding();
        SetBindingStyle(targetKeyButton, false);
    }

    /// <summary>
    /// Apply/clear “binding” visual style on a button.
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
    /// Cancel hotkey binding and restore.
    /// </summary>
    private void CancelHotkeyBinding()
    {
        isHotKeyBinding = false;
        keybindButton.Text = hotKey.ToString();
        SetBindingStyle(keybindButton, false);
    }

    /// <summary>
    /// Cancel target binding and restore.
    /// </summary>
    private void CancelTargetBinding()
    {
        isTargetKeyBinding = false;
        targetKeyButton.Text = targetKey.ToString();
        SetBindingStyle(targetKeyButton, false);
        DisableMouseBinding();
    }

    /// <summary>
    /// Send currently bound target input (mouse or key).
    /// </summary>
    private void PerformTargetInput()
    {
        if (NativeInput.IsMouseKey(targetKey))
            NativeInput.ClickMouseButton(targetKey);
        else
            NativeInput.SendKeyPress(targetKey);
    }

    #endregion

    #region Configuration Helpers


    /// <summary>
    /// Saves the current configuration to a file.
    /// </summary>
    /// <param name="path">String path of the saved file.</param>
    private void SaveConfigurationToFile(string path)
    {
        // Implementation for serializing and writing your settings goes here
        // The idea is to save is as json for now. Open to changes.
        File.WriteAllText(path, "Placeholder configuration data.");
    }

    /// <summary>
    /// Loads a config file.
    /// Shows an error message if file is invalid.
    /// </summary>
    /// <param name="path"></param>
    private void LoadConfigurationFromFile(string path)
    {
        MessageBox.Show($"TODO: Load settings from file at {path}", "Not implemented",
            MessageBoxButtons.OK, MessageBoxIcon.Information);
    }

    /// <summary>
    /// Gets the current values for all configuration
    /// in the tabs and fields.
    /// </summary>
    private void GetCurrentConfigValuesFromApp()
    {

    }
    
    /// <summary>
    /// Applies the configuration to the app.
    /// </summary>
    private void ApplyLoadedConfigToApp()
    {
        
    }

    #endregion
}
