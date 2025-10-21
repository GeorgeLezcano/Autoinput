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
        resetButton = new Button();
        inputCountLabel = new Label();
        inputCountTimer = new System.Windows.Forms.Timer(components);
        keybindButton = new Button();
        keybindLabel = new Label();
        targetKeyButton = new Button();
        targetKeyLabel = new Label();
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
        timerLabel.Size = new Size(63, 20);
        timerLabel.TabIndex = 1;
        timerLabel.Text = "00:00:00";
        // 
        // runTimer
        // 
        runTimer.Interval = 1000;
        runTimer.Tick += RunTimer_Tick;
        // 
        // intervalLabel
        // 
        intervalLabel.AutoSize = true;
        intervalLabel.Location = new Point(166, 145);
        intervalLabel.Name = "intervalLabel";
        intervalLabel.Size = new Size(154, 20);
        intervalLabel.TabIndex = 4;
        intervalLabel.Text = "Interval (Milliseconds)";
        // 
        // intervalInput
        // 
        intervalInput.Increment = new decimal(new int[] { 100, 0, 0, 0 });
        intervalInput.Location = new Point(168, 168);
        intervalInput.Maximum = new decimal(new int[] { 10000, 0, 0, 0 });
        intervalInput.Minimum = new decimal(new int[] { 100, 0, 0, 0 });
        intervalInput.Name = "intervalInput";
        intervalInput.Size = new Size(150, 27);
        intervalInput.TabIndex = 6;
        intervalInput.Value = new decimal(new int[] { 1000, 0, 0, 0 });
        // 
        // resetButton
        // 
        resetButton.Location = new Point(168, 446);
        resetButton.Name = "resetButton";
        resetButton.Size = new Size(153, 29);
        resetButton.TabIndex = 8;
        resetButton.Text = "Reset";
        resetButton.UseVisualStyleBackColor = true;
        resetButton.Click += ResetButton_Click;
        // 
        // inputCountLabel
        // 
        inputCountLabel.AutoSize = true;
        inputCountLabel.Location = new Point(12, 492);
        inputCountLabel.Name = "inputCountLabel";
        inputCountLabel.Size = new Size(84, 20);
        inputCountLabel.TabIndex = 9;
        inputCountLabel.Text = "input count";
        // 
        // inputCountTimer
        // 
        inputCountTimer.Interval = 1000;
        inputCountTimer.Tick += InputCount_Tick;
        // 
        // keybindButton
        // 
        keybindButton.Location = new Point(166, 239);
        keybindButton.Name = "keybindButton";
        keybindButton.Size = new Size(150, 29);
        keybindButton.TabIndex = 11;
        keybindButton.Text = "F8";
        keybindButton.UseVisualStyleBackColor = true;
        keybindButton.Click += KeybindButton_Click;
        // 
        // keybindLabel
        // 
        keybindLabel.AutoSize = true;
        keybindLabel.Location = new Point(175, 216);
        keybindLabel.Name = "keybindLabel";
        keybindLabel.Size = new Size(135, 20);
        keybindLabel.TabIndex = 12;
        keybindLabel.Text = "Start/Stop Keybind";
        // 
        // targetKeyButton
        // 
        targetKeyButton.Location = new Point(168, 314);
        targetKeyButton.Name = "targetKeyButton";
        targetKeyButton.Size = new Size(148, 29);
        targetKeyButton.TabIndex = 13;
        targetKeyButton.Text = "LMB";
        targetKeyButton.UseVisualStyleBackColor = true;
        targetKeyButton.Click += TargetInputKeyButton_Click;
        // 
        // targetKeyLabel
        // 
        targetKeyLabel.AutoSize = true;
        targetKeyLabel.Location = new Point(182, 291);
        targetKeyLabel.Name = "targetKeyLabel";
        targetKeyLabel.Size = new Size(116, 20);
        targetKeyLabel.TabIndex = 14;
        targetKeyLabel.Text = "Target Input Key";
        // 
        // MainForm
        // 
        AutoScaleDimensions = new SizeF(8F, 20F);
        AutoScaleMode = AutoScaleMode.Font;
        ClientSize = new Size(503, 547);
        Controls.Add(targetKeyLabel);
        Controls.Add(targetKeyButton);
        Controls.Add(keybindLabel);
        Controls.Add(keybindButton);
        Controls.Add(inputCountLabel);
        Controls.Add(resetButton);
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
    private Button resetButton;
    private Label inputCountLabel;
    private System.Windows.Forms.Timer inputCountTimer;
    private Button keybindButton;
    private Label keybindLabel;
    private Button targetKeyButton;
    private Label targetKeyLabel;
}
