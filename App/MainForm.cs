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
        scheduleStartPicker.Enabled = !isRunning;
        scheduleEnableStopCheck.Enabled = !isRunning;
        scheduleStopPicker.Enabled = !isRunning;

        // Run Mode Tab
        runUntilStoppedRadio.Enabled = !isRunning;
        runForCountRadio.Enabled = !isRunning;
        runCountInput.Enabled = !isRunning;

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
        intervalInput.Value = intervalDefault;
        runCountInput.Value = runCountInputDefault;
        runUntilStoppedRadio.Checked = true;
    }

    /// <summary>
    /// Placeholder for keybind capture (not implemented).
    /// </summary>
    /// <param name="sender">Keybind button.</param>
    /// <param name="e">Event arguments.</param>
    private void KeybindButton_Click(object sender, EventArgs e)
    {
        MessageBox.Show(
            "TODO: Implement keybind capture.",
            "Not implemented",
            MessageBoxButtons.OK,
            MessageBoxIcon.Information);
    }

    /// <summary>
    /// Placeholder for target key/click selection (not implemented).
    /// </summary>
    /// <param name="sender">Target key button.</param>
    /// <param name="e">Event arguments.</param>
    private void TargetInputKeyButton_Click(object sender, EventArgs e)
    {
        MessageBox.Show(
            "TODO: Implement target key/click selection.",
            "Not implemented",
            MessageBoxButtons.OK,
            MessageBoxIcon.Information);
    }

    /// <summary>
    /// Enables or disables the stop time picker based on the checkbox state.
    /// </summary>
    /// <param name="sender">Enable-stop checkbox.</param>
    /// <param name="e">Event arguments.</param>
    private void ScheduleEnableStopCheck_CheckedChanged(object? sender, EventArgs e)
    {
        scheduleStopPicker.Enabled = scheduleEnableStopCheck.Checked;
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
        MessageBox.Show("TODO: Move selected step down.", "Not implemented",
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

}