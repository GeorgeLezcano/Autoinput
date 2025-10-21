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
        components = new System.ComponentModel.Container();
        System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MainForm));
        startStopButton = new Button();
        timerLabel = new Label();
        runTimer = new System.Windows.Forms.Timer(components);
        intervalLabel = new Label();
        intervalInput = new NumericUpDown();
        saveButton = new Button();
        resetButton = new Button();
        ((System.ComponentModel.ISupportInitialize)intervalInput).BeginInit();
        SuspendLayout();
        // 
        // startStopButton
        // 
        startStopButton.Font = new Font("Segoe UI", 13.8F, FontStyle.Regular, GraphicsUnit.Point, 0);
        startStopButton.ForeColor = SystemColors.ButtonHighlight;
        startStopButton.Location = new Point(145, 42);
        startStopButton.Name = "startStopButton";
        startStopButton.Size = new Size(196, 79);
        startStopButton.TabIndex = 0;
        startStopButton.Text = "Start";
        startStopButton.UseVisualStyleBackColor = true;
        startStopButton.Click += StartStopButton_Click;
        // 
        // timerLabel
        // 
        timerLabel.AutoSize = true;
        timerLabel.Location = new Point(12, 518);
        timerLabel.Name = "timerLabel";
        timerLabel.Size = new Size(44, 20);
        timerLabel.TabIndex = 1;
        timerLabel.Text = "timer";
        // 
        // runTimer
        // 
        runTimer.Interval = 1000;
        runTimer.Tick += RunTimer_Tick;
        // 
        // intervalLabel
        // 
        intervalLabel.AutoSize = true;
        intervalLabel.Location = new Point(163, 145);
        intervalLabel.Name = "intervalLabel";
        intervalLabel.Size = new Size(154, 20);
        intervalLabel.TabIndex = 4;
        intervalLabel.Text = "Interval (Milliseconds)";
        // 
        // intervalInput
        // 
        intervalInput.Location = new Point(164, 168);
        intervalInput.Maximum = new decimal(new int[] { 10000, 0, 0, 0 });
        intervalInput.Name = "intervalInput";
        intervalInput.Size = new Size(150, 27);
        intervalInput.TabIndex = 6;
        intervalInput.Value = new decimal(new int[] { 1000, 0, 0, 0 });
        // 
        // saveButton
        // 
        saveButton.Location = new Point(185, 243);
        saveButton.Name = "saveButton";
        saveButton.Size = new Size(94, 29);
        saveButton.TabIndex = 7;
        saveButton.Text = "Save";
        saveButton.UseVisualStyleBackColor = true;
        saveButton.Click += SaveButton_Click;
        // 
        // resetButton
        // 
        resetButton.Location = new Point(185, 295);
        resetButton.Name = "resetButton";
        resetButton.Size = new Size(94, 29);
        resetButton.TabIndex = 8;
        resetButton.Text = "Reset";
        resetButton.UseVisualStyleBackColor = true;
        resetButton.Click += ResetButton_Click;
        // 
        // MainForm
        // 
        AutoScaleDimensions = new SizeF(8F, 20F);
        AutoScaleMode = AutoScaleMode.Font;
        ClientSize = new Size(503, 547);
        Controls.Add(resetButton);
        Controls.Add(saveButton);
        Controls.Add(intervalInput);
        Controls.Add(intervalLabel);
        Controls.Add(timerLabel);
        Controls.Add(startStopButton);
        FormBorderStyle = FormBorderStyle.FixedDialog;
        Icon = (Icon)resources.GetObject("$this.Icon");
        MaximizeBox = false;
        Name = "MainForm";
        Text = "AutoInput";
        Load += MainForm_Load;
        ((System.ComponentModel.ISupportInitialize)intervalInput).EndInit();
        ResumeLayout(false);
        PerformLayout();
    }

    #endregion

    private Button startStopButton;
    private Label timerLabel;
    private System.Windows.Forms.Timer runTimer;
    private Label intervalLabel;
    private NumericUpDown intervalInput;
    private Button saveButton;
    private Button resetButton;
}
