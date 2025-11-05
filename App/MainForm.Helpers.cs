using System.Runtime.InteropServices;
using System.Text.Json;
using App.Constants;
using App.Models;
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

    private JsonSerializerOptions jsonSerializerOption = new()
    {
        WriteIndented = true
    };

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
            if (!IsHandleCreated) return;

            try { UnregisterHotKey(Handle, Win32Hotkey.HOTKEY_ID); } catch { }

            bool ok = RegisterHotKey(Handle, Win32Hotkey.HOTKEY_ID, Win32Hotkey.MOD_NONE, (uint)key);
            if (!ok)
            {
                if (key != hotKeyDefault)
                {
                    bool fallback = RegisterHotKey(Handle, Win32Hotkey.HOTKEY_ID, Win32Hotkey.MOD_NONE, (uint)hotKeyDefault);
                    if (fallback)
                    {
                        hotKey = hotKeyDefault;
                        keybindButton.Text = hotKey.ToString();
                        MessageBox.Show(
                            "That hotkey is already in use by another app. Reverted to F8.",
                            "Hotkey in use", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    }
                }
            }
        }
        catch { /* do nothing */ }
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

        sequenceAddButton.Enabled = false;
        sequenceRemoveButton.Enabled = false;

        keybindButton.Enabled = false;
        targetKeyButton.Enabled = false;

        saveConfigButton.Enabled = false;
        loadConfigButton.Enabled = false;
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

        RestoreUItoIdle();
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
        sequenceAddButton.Enabled = !running;
        sequenceRemoveButton.Enabled = !running;
        newSequenceButton.Enabled = !running;
        removeSequenceButton.Enabled = !running;
        sequencePicker.Enabled = !running;
        sequenceNameText.Enabled = !running;
        sequenceGrid.Enabled = !running;

        // Keys
        keybindButton.Enabled = !running;
        targetKeyButton.Enabled = !running;

        // Config
        saveConfigButton.Enabled = !running;
        loadConfigButton.Enabled = !running;
        configPathText.Enabled = !running;
        openConfigFolderButton.Enabled = !running;
        sequenceModeCheck.Enabled = !running;

        // Info
        userGuideButton.Enabled = !running;
    }

    /// <summary>
    /// Restore UI to idle defaults.
    /// </summary>
    private void RestoreUItoIdle()
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

        sequenceAddButton.Enabled = true;
        sequenceRemoveButton.Enabled = true;

        keybindButton.Enabled = true;
        targetKeyButton.Enabled = true;

        saveConfigButton.Enabled = true;
        loadConfigButton.Enabled = true;
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
            startStopButton.FlatAppearance.MouseOverBackColor = Color.Empty;
            startStopButton.FlatAppearance.MouseDownBackColor = Color.Empty;
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
        if (targetKey == Keys.None)
            return;

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
        AppConfig appConfig = GetCurrentConfigValuesFromApp();
        string jsonString = JsonSerializer.Serialize(appConfig, jsonSerializerOption);
        File.WriteAllText(path, jsonString);
    }

    /// <summary>
    /// Loads a config file.
    /// Shows an error message if file is invalid.
    /// </summary>
    /// <param name="path"></param>
    private void LoadConfigurationFromFile(string path)
    {
        if (!File.Exists(path))
        {
            MessageBox.Show("The configuration file does not exist.", "Open configuration",
                MessageBoxButtons.OK, MessageBoxIcon.Error);
            return;
        }

        try
        {
            string jsonString = File.ReadAllText(path);
            AppConfig? appConfig = JsonSerializer.Deserialize<AppConfig>(jsonString, jsonSerializerOption);

            if (appConfig is null || !TryValidate(appConfig))
                throw new InvalidDataException();

            ApplyLoadedConfigToApp(appConfig);
        }
        catch
        {
            MessageBox.Show("Invalid configuration file.", "Open configuration",
                MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }

    /// <summary>
    /// Validates a deserialized AppConfig before applying it to the UI.
    /// Ensures ranges, required fields, and parseable keys.
    /// </summary>
    private bool TryValidate(AppConfig c)
    {
        if (c.IntervalMilliseconds < intervalMinimum || c.IntervalMilliseconds > intervalMaximum)
            return false;

        if (string.IsNullOrWhiteSpace(c.StartStopKeybind) || !TryParseKeys(c.StartStopKeybind, out var parsedHotkey))
            return false;

        if (string.IsNullOrWhiteSpace(c.TargetInputKey) || !TryParseKeys(c.TargetInputKey, out var parsedTarget))
            return false;

        if (parsedHotkey == parsedTarget)
            return false;

        if (c.RunUntilSetCountActive && (c.StopInputCount <= 0 ||
            c.StopInputCount < (int)runCountInput.Minimum ||
            c.StopInputCount > (int)runCountInput.Maximum))
            return false;

        if (c.ScheduleStartEnabled && c.ScheduleStartTime == default)
            return false;

        if (c.ScheduleStopEnabled && c.ScheduleStopTime == default)
            return false;

        return true;
    }

    /// <summary>
    /// Gets the current values for all configuration
    /// in the tabs and fields.
    /// </summary>
    private AppConfig GetCurrentConfigValuesFromApp()
    {
        AppConfig appConfig = new()
        {
            IntervalMilliseconds = TimeUtils.ToMilliseconds(intervalInput.Value),
            RunUntilStopActive = runUntilStoppedRadio.Checked,
            RunUntilSetCountActive = runForCountRadio.Checked,
            StopInputCount = (int)runCountInput.Value,
            StartStopKeybind = hotKey.ToString(),
            TargetInputKey = targetKey.ToString(),
            ScheduleStartEnabled = scheduleEnableStartCheck.Checked,
            ScheduleStartTime = scheduleStartPicker.Value,
            ScheduleStopEnabled = scheduleEnableStopCheck.Checked,
            ScheduleStopTime = scheduleStopPicker.Value,
            ConfigFolderPath = configPathText.Text,
            SelectedSequenceIndex = _currentSequenceIndex,
            Sequences = _sequences,
            SequenceModeActive = sequenceModeCheck.Checked
        };
        return appConfig;
    }

    /// <summary>
    /// Applies the configuration to the app.
    /// </summary>
    private void ApplyLoadedConfigToApp(AppConfig appConfig)
    {
        try
        {
            static DateTime Clamp(DateTime dt, DateTime min, DateTime max)
                => dt < min ? min : (dt > max ? max : dt);

            static decimal ClampDec(decimal v, decimal min, decimal max)
                => v < min ? min : (v > max ? max : v);

            intervalInput.Value = TimeUtils.ClampSeconds(
                appConfig.IntervalMilliseconds / 1000M,
                intervalMinimum,
                intervalMaximum);

            bool runForCount = appConfig.RunUntilSetCountActive;
            runUntilStoppedRadio.Checked = !runForCount;
            runForCountRadio.Checked = runForCount;

            runCountInput.Value = ClampDec(appConfig.StopInputCount, runCountInput.Minimum, runCountInput.Maximum);
            runCountInput.Enabled = runForCountRadio.Checked;

            scheduleEnableStartCheck.Checked = appConfig.ScheduleStartEnabled;
            scheduleStartPicker.Value = Clamp(appConfig.ScheduleStartTime, scheduleStartPicker.MinDate, scheduleStartPicker.MaxDate);

            scheduleEnableStopCheck.Checked = appConfig.ScheduleStopEnabled;
            scheduleStopPicker.Value = Clamp(appConfig.ScheduleStopTime, scheduleStopPicker.MinDate, scheduleStopPicker.MaxDate);

            configPathText.Text = appConfig.ConfigFolderPath;

            hotKey = TryParseKeys(appConfig.StartStopKeybind, out var parsedHotkey) ? parsedHotkey : hotKeyDefault;
            keybindButton.Text = hotKey.ToString();
            RegisterGlobalHotkey(hotKey);

            targetKey = TryParseKeys(appConfig.TargetInputKey, out var parsedTarget) ? parsedTarget : targetKeyDefault;

            if (targetKey == hotKey)
            {
                targetKey = targetKeyDefault;
                MessageBox.Show(
                    "The loaded configuration used the same key for Start/Stop and Target. Reverted target to default.",
                    "Key conflict", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }

            targetKeyButton.Text = targetKey.ToString();

            LoadSequencesFromConfig(appConfig);
            sequenceModeCheck.Checked = appConfig.SequenceModeActive;
        }
        catch
        {
            MessageBox.Show("Invalid configuration file.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }

    /// <summary>
    /// Attempts to parse the string into a Keys enum.
    /// </summary>
    /// <param name="text">The text to be parsed</param>
    /// <param name="result">The output Keys value</param>
    /// <returns>True if it was successful or false if parsing fails</returns>
    private static bool TryParseKeys(string text, out Keys result)
    {
        return Enum.TryParse(text, true, out result);
    }

    #endregion

    #region Sequence Helpers

    /// <summary>
    /// Re-numbers the sequence steps after any add/remove.
    /// </summary>
    private void RenumberSequenceSteps()
    {
        for (int i = 0; i < sequenceGrid.Rows.Count; i++)
        {
            sequenceGrid.Rows[i].Cells["colStep"].Value = i + 1;
        }
    }

    /// <summary>
    /// Fills the colKey combo with a curated set of Keys values.
    /// </summary>
    private void PopulateKeyDropdown()
    {
        if (colKey is not DataGridViewComboBoxColumn combo) return;

        combo.Items.Clear();
        var common = new List<Keys>
    {
        Keys.LButton, Keys.RButton, Keys.MButton,
        Keys.Space, Keys.Enter, Keys.Tab, Keys.Escape,
        Keys.Up, Keys.Down, Keys.Left, Keys.Right
    };

        // Letters A-Z
        common.AddRange(Enumerable.Range((int)Keys.A, 26).Select(i => (Keys)i));

        // Top-row digits 0-9 (D0..D9)
        common.AddRange(Enumerable.Range((int)Keys.D0, 10).Select(i => (Keys)i));

        // Numpad digits 0-9
        common.AddRange(Enumerable.Range((int)Keys.NumPad0, 10).Select(i => (Keys)i));

        // Function keys F1..F24
        common.AddRange(Enumerable.Range((int)Keys.F1, 24).Select(i => (Keys)i));

        // Remove duplicates and any weird flagged entries (just in case)
        foreach (var k in common.Distinct().OrderBy(k => k.ToString()))
            combo.Items.Add(k.ToString());
    }

    /// <summary>
    /// Refreshes the sequence dropdown items from the _sequences list.
    /// </summary>
    private void RefreshSequencePicker()
    {
        if (sequencePicker is null) return;

        sequencePicker.BeginUpdate();
        sequencePicker.Items.Clear();
        foreach (var seq in _sequences)
            sequencePicker.Items.Add(seq.Name);
        sequencePicker.EndUpdate();

        UpdateRemoveButtonEnabled();
    }

    /// <summary>
    /// Updates name textbox and the steps grid from the currently selected sequence.
    /// </summary>
    private void SyncSequenceUiFromSelection()
    {
        var idx = sequencePicker.SelectedIndex;
        if (idx < 0 || idx >= _sequences.Count) return;

        var selected = _sequences[idx];

        _isSyncingSequenceName = true;
        if (!string.Equals(sequenceNameText.Text, selected.Name, StringComparison.Ordinal))
            sequenceNameText.Text = selected.Name;
        _isSyncingSequenceName = false;

        RefreshSequenceGridFrom(selected);
    }

    /// <summary>
    /// Rebuilds the grid rows to reflect a given sequence.
    /// </summary>
    private void RefreshSequenceGridFrom(Sequence sequence)
    {
        sequenceGrid.Rows.Clear();
        var stepNum = 1;
        foreach (var step in sequence.Steps)
        {
            var keyLabel = step.Key.ToString();
            var delaySec = (step.DelayMS / 1000m).ToString("0.0");
            sequenceGrid.Rows.Add(stepNum++, keyLabel, delaySec);
        }
    }

    /// <summary>
    /// Generates a unique "New Sequence", "New Sequence (2)" ... name.
    /// </summary>
    private string GetNextNewSequenceName()
    {
        const string baseName = "New Sequence";
        if (_sequences.All(s => !string.Equals(s.Name, baseName, StringComparison.OrdinalIgnoreCase)))
            return baseName;

        int n = 1;
        while (_sequences.Any(s => string.Equals(s.Name, $"{baseName} ({n})", StringComparison.OrdinalIgnoreCase)))
            n++;

        return $"{baseName} ({n})";
    }

    /// <summary>
    /// Enables/disables the Remove button based on count.
    /// </summary>
    private void UpdateRemoveButtonEnabled()
    {
        if (removeSequenceButton is null) return;
        removeSequenceButton.Enabled = _sequences.Count > 1;
    }

    /// <summary>
    /// Saves the DataGridView rows back into the given Sequence.Steps.
    /// Expects colKey (string) and colDelayMs (seconds, 1 decimal).
    /// </summary>
    private void SaveGridIntoSequence(Sequence seq)
    {
        var newSteps = new List<SequenceStep>();

        foreach (DataGridViewRow row in sequenceGrid.Rows)
        {
            if (row.IsNewRow) continue;

            var keyLabel = row.Cells["colKey"].Value?.ToString()?.Trim();
            var delayText = row.Cells["colDelayMs"].Value?.ToString()?.Trim();

            if (string.IsNullOrEmpty(keyLabel) || string.IsNullOrEmpty(delayText)) continue;
            if (!Enum.TryParse<Keys>(keyLabel, ignoreCase: true, out var key)) continue;
            if (!decimal.TryParse(delayText, out var seconds)) continue;

            var ms = (int)Math.Round(seconds * 1000m, MidpointRounding.AwayFromZero);
            newSteps.Add(new SequenceStep { Key = key, DelayMS = ms });
        }
        seq.Steps = newSteps;
    }

    /// <summary>
    /// Gets the selected sequence.
    /// </summary>
    private Sequence? GetSelectedSequence()
    {
        var idx = sequencePicker.SelectedIndex;
        return (idx >= 0 && idx < _sequences.Count) ? _sequences[idx] : null;
    }

    /// <summary>
    /// Resets all sequences back to a single default one and updates the UI.
    /// </summary>
    private void ResetSequences()
    {
        sequenceGrid.EndEdit();

        _suppressSequencePickerSync = true;
        try
        {
            _currentSequenceIndex = 0;
            _sequences.Clear();
            _sequences.Add(new Sequence());

            RefreshSequencePicker();
            sequencePicker.SelectedIndex = 0;

            sequenceGrid.Rows.Clear();
            sequenceGrid.CurrentCell = null;
            sequenceGrid.ClearSelection();

            _isSyncingSequenceName = true;
            sequenceNameText.Text = _sequences[0].Name;
            _isSyncingSequenceName = false;

            RefreshSequenceGridFrom(_sequences[0]);
            UpdateRemoveButtonEnabled();
        }
        finally
        {
            _suppressSequencePickerSync = false;
        }
    }

    /// <summary>
    /// Loads sequences from the given config into runtime state and syncs the UI.
    /// Uses existing helpers to refresh the picker and grid. Falls back to a single
    /// default sequence if none are provided or everything sanitizes away.
    /// </summary>
    private void LoadSequencesFromConfig(AppConfig appConfig)
    {
        sequenceGrid.EndEdit();

        var sanitized = SanitizeSequences(
            appConfig.Sequences,
            intervalMinimum,
            intervalMaximum
        );

        if (sanitized.Count == 0)
            sanitized.Add(new Sequence());

        int selected = ClampIndex(appConfig.SelectedSequenceIndex, sanitized.Count);

        ApplySequencesToUi(sanitized, selected);
    }

    /// <summary>
    /// Clamps the the index.
    /// </summary>
    private static int ClampIndex(int i, int count)
    => (i < 0 || i >= count) ? 0 : i;

    /// <summary>
    /// Sanitizes the sequence and returns a cleaned
    /// sequences with valid ranges and values.
    /// </summary>
    private static Sequence SanitizeSequence(Sequence? sequence, int minMs, int maxMs)
    {
        sequence ??= new Sequence();

        var name = string.IsNullOrWhiteSpace(sequence.Name) ? "New Sequence" : sequence.Name.Trim();
        var steps = sequence.Steps ?? [];

        var cleaned = new List<SequenceStep>(steps.Count);
        foreach (var step in steps)
        {
            if (step is null) continue;
            if (step.Key == Keys.None) continue;

            var ms = step.DelayMS;
            if (ms < minMs) ms = minMs;
            if (ms > maxMs) ms = maxMs;

            cleaned.Add(new SequenceStep { Key = step.Key, DelayMS = ms });
        }

        return new Sequence { Name = name, Steps = cleaned };
    }

    /// <summary>
    /// Sanitizes the list of sequences.
    /// </summary>
    private static List<Sequence> SanitizeSequences(IEnumerable<Sequence>? sequences, int minMs, int maxMs)
    {
        var list = new List<Sequence>();
        if (sequences is null) return list;

        foreach (var seq in sequences)
            list.Add(SanitizeSequence(seq, minMs, maxMs));

        return list;
    }

    /// <summary>
    /// Populates the UI with the loaded sequences.
    /// </summary>
    private void ApplySequencesToUi(List<Sequence> sequences, int selectedIndex)
    {
        _suppressSequencePickerSync = true;
        try
        {
            _sequences = sequences;

            RefreshSequencePicker();

            sequencePicker.SelectedIndex = selectedIndex;
            _currentSequenceIndex = selectedIndex;

            _isSyncingSequenceName = true;
            sequenceNameText.Text = _sequences[_currentSequenceIndex].Name;
            _isSyncingSequenceName = false;

            PopulateKeyDropdown();

            RefreshSequenceGridFrom(_sequences[_currentSequenceIndex]);
            UpdateRemoveButtonEnabled();

            if (sequenceGrid.Rows.Count > 0)
            {
                sequenceGrid.ClearSelection();
                sequenceGrid.CurrentCell = null;
            }
        }
        finally
        {
            _suppressSequencePickerSync = false;
        }
        SyncSequenceUiFromSelection();
    }

    #endregion
}
