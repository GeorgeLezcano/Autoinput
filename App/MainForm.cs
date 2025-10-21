using System.Windows.Forms;

namespace App;

/// <summary>
/// Application custom logic.
/// </summary>
public partial class MainForm : Form
{
    private const int defaultInterval = 1000;

    private bool isRunning = false;
    private int activeTimerSeconds = 0;
    private int inputCount = 0;

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
        SetInputCountLabel(inputCount);
        SetTimeLabel(activeTimerSeconds);
        startStopButton.BackColor = Color.Green;
    }

    /// <summary>
    /// Start button on click method.
    /// </summary>
    private void StartStopButton_Click(object sender, EventArgs e)
    {
        //Apply field values before starting
        if (!isRunning)
        {
            inputCountTimer.Interval = (int)intervalInput.Value;
        }

        // Toggle state with every click
        isRunning = !isRunning;

        // Start timers
        (isRunning ? (Action)runTimer.Start : runTimer.Stop)();
        (isRunning ? (Action)inputCountTimer.Start : inputCountTimer.Stop)();

        // UI changes based on state
        startStopButton.Text = isRunning ? "Stop" : "Start";
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
    /// Tick for key input count timer.
    /// </summary>
    private void InputCount_Tick(object sender, EventArgs e)
    {
        inputCount++;
        SetInputCountLabel(inputCount);
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
    /// Sets the input count label text.
    /// </summary>
    /// <param name="count">The key press count.</param>
    private void SetInputCountLabel(int count)
    {
        inputCountLabel.Text = $"Input Count: {count}";
    }

    /// <summary>
    /// Resets all values to the default state.
    /// </summary>
    private void ResetButton_Click(Object sender, EventArgs e)
    {
        activeTimerSeconds = 0;
        inputCount = 0;

        SetTimeLabel(activeTimerSeconds);
        SetInputCountLabel(inputCount);

        intervalInput.Value = defaultInterval;
    }

    /// <summary>
    /// Sets the keybind to start/stop the autoinput. 
    /// Default is F8.
    /// </summary>
    private void KeybindButton_Click(object sender, EventArgs e)
    {
        MessageBox.Show("Not implemented");
    }

    /// <summary>
    /// Sets the target key to be used in the autoiput.
    /// Default is Left Mouse Click.
    /// </summary>
    private void TargetInputKeyButton_Click(object sender, EventArgs e)
    {
        MessageBox.Show("Not implemented");
    }
}
