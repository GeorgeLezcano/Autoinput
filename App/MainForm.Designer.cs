namespace App;

partial class MainForm
{
    /// <summary>
    ///  Required designer variable.
    /// </summary>
    private System.ComponentModel.IContainer components = null;

    /// <summary>
    ///  Clean up any resources being used.
    /// </summary>
    /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
    protected override void Dispose(bool disposing)
    {
        if (disposing && (components != null))
        {
            components.Dispose();
        }
        base.Dispose(disposing);
    }

    #region Windows Form Designer generated code

    /// <summary>
    ///  Required method for Designer support - do not modify
    ///  the contents of this method with the code editor.
    /// </summary>
    private void InitializeComponent()
    {
        System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MainForm));
        startButton = new Button();
        stopButton = new Button();
        SuspendLayout();
        // 
        // startButton
        // 
        startButton.Location = new Point(237, 388);
        startButton.Name = "startButton";
        startButton.Size = new Size(196, 79);
        startButton.TabIndex = 0;
        startButton.Text = "Start";
        startButton.UseVisualStyleBackColor = true;
        startButton.Click += StartButton_Click;
        // 
        // stopButton
        // 
        stopButton.Location = new Point(525, 388);
        stopButton.Name = "stopButton";
        stopButton.Size = new Size(196, 79);
        stopButton.TabIndex = 1;
        stopButton.Text = "Stop";
        stopButton.UseVisualStyleBackColor = true;
        stopButton.Click += StopButton_Click;
        // 
        // MainForm
        // 
        AutoScaleDimensions = new SizeF(8F, 20F);
        AutoScaleMode = AutoScaleMode.Font;
        ClientSize = new Size(961, 547);
        Controls.Add(stopButton);
        Controls.Add(startButton);
        FormBorderStyle = FormBorderStyle.FixedDialog;
        Icon = (Icon)resources.GetObject("$this.Icon");
        MaximizeBox = false;
        Name = "MainForm";
        Text = "AutoInput";
        ResumeLayout(false);
    }

    #endregion

    private Button startButton;
    private Button stopButton;
}
