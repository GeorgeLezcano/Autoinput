namespace App;

/// <summary>
/// Application custom logic.
/// </summary>
public partial class MainForm : Form
{
    /// <summary>
    /// Default constructor.
    /// </summary>
    public MainForm()
    {
        InitializeComponent();
        InitDefautState();
    }

    /// <summary>
    /// Default state setup for initialization.
    /// </summary>
    private void InitDefautState()
    {
        stopButton.Enabled = false;
    }

    /// <summary>
    /// Start button on click method.
    /// </summary>
    private void StartButton_Click(object sender, EventArgs e)
    {
        stopButton.Enabled = true;
        startButton.Enabled = false;
    }

    /// <summary>
    /// Stop button on click method.
    /// </summary>
    private void StopButton_Click(object sender, EventArgs e)
    {
        startButton.Enabled = true;
        stopButton.Enabled = false;
    }

}
