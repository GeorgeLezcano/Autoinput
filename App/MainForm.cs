namespace App;

public partial class MainForm : Form
{
    public MainForm()
    {
        InitializeComponent();
        stopButton.Enabled = false;
    }

    private void StartButton_Click(object sender, EventArgs e)
    {
        stopButton.Enabled = true;
        startButton.Enabled = false;
    }

    private void StopButton_Click(object sender, EventArgs e)
    {
        startButton.Enabled = true;
        stopButton.Enabled = false;
    }

}
