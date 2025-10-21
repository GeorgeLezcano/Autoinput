namespace App;

/// <summary>
/// Application custom logic.
/// </summary>
public partial class MainForm : Form
{
    private bool isRunning = false;
    private int activeTimerSeconds = 0;

    /// <summary>
    /// Default constructor.
    /// </summary>
    public MainForm()
    {
        InitializeComponent();
    }

    /// <summary>
    /// Initialization logic for first load.
    /// </summary>
    private void MainForm_Load(object sender, EventArgs e)
    {
        SetTimeLabel(0);
        startStopButton.BackColor = Color.Green;
    }

    /// <summary>
    /// Start button on click method.
    /// </summary>
    private void StartStopButton_Click(object sender, EventArgs e)
    {
        isRunning = !isRunning;

        (isRunning ? (Action)runTimer.Start : runTimer.Stop)();

        startStopButton.Text = isRunning ? "Stop" : "Start";
        saveButton.Enabled = !isRunning;
        resetButton.Enabled = !isRunning;
        intervalInput.Enabled = !isRunning;
        startStopButton.BackColor = isRunning ? Color.Red : Color.Green;
    }

    /// <summary>
    /// Tick for run timer.
    /// </summary>
    private void RunTimer_Tick(object sender, EventArgs e)
    {
        activeTimerSeconds++;
        SetTimeLabel(activeTimerSeconds);
    }

    /// <summary>
    /// Sets the time elapsed label text.
    /// </summary>
    /// <param name="seconds">The total time in seconds.</param>
    private void SetTimeLabel(int seconds)
    {
        int hours = seconds / 3600;
        int minutes = (seconds % 3600) / 60;
        int secs = seconds % 60;

        timerLabel.Text = $"Active Time: {hours:D2}:{minutes:D2}:{secs:D2}";
    }

    /// <summary>
    /// Saves current configuration values.
    /// </summary>
    private void SaveButton_Click(Object sender, EventArgs e)
    {
        //TODO: Add save button logic.
    }

    /// <summary>
    /// Resets all values to the default state.
    /// </summary>
    private void ResetButton_Click(Object sender, EventArgs e)
    {
        activeTimerSeconds = 0;
        SetTimeLabel(activeTimerSeconds);
        intervalInput.Value = 1000;
    }

}
