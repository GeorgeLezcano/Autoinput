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
        // THEME (form-level)
        // 
        BackColor = Color.FromArgb(30, 32, 46);
        ForeColor = Color.FromArgb(235, 238, 245);
        // 
        // startStopButton
        // 
        startStopButton.Font = new Font("Segoe UI", 14F, FontStyle.Regular, GraphicsUnit.Point, 0);
        startStopButton.ForeColor = Color.White;
        startStopButton.BackColor = Color.Green;
        startStopButton.FlatStyle = FlatStyle.Flat;
        startStopButton.FlatAppearance.BorderColor = Color.FromArgb(70, 75, 95);
        startStopButton.FlatAppearance.BorderSize = 1;
        startStopButton.Location = new Point(170, 32);
        startStopButton.Name = "startStopButton";
        startStopButton.Size = new Size(220, 64);
        startStopButton.TabIndex = 0;
        startStopButton.Text = "Start";
        startStopButton.UseVisualStyleBackColor = false;
        startStopButton.Click += StartStopButton_Click;
        // 
        // timerLabel
        // 
        timerLabel.AutoSize = true;
        timerLabel.ForeColor = Color.FromArgb(200, 204, 214);
        timerLabel.Location = new Point(16, 372);
        timerLabel.Name = "timerLabel";
        timerLabel.Size = new Size(122, 20);
        timerLabel.TabIndex = 1;
        timerLabel.Text = "Active Time: 00:00:00";
        // 
        // runTimer
        // 
        runTimer.Interval = 1000;
        runTimer.Tick += RunTimer_Tick;
        // 
        // intervalLabel
        // 
        intervalLabel.AutoSize = true;
        intervalLabel.ForeColor = Color.FromArgb(200, 204, 214);
        intervalLabel.Location = new Point(196, 116);
        intervalLabel.Name = "intervalLabel";
        intervalLabel.Size = new Size(154, 20);
        intervalLabel.TabIndex = 4;
        intervalLabel.Text = "Interval (Milliseconds)";
        // 
        // intervalInput
        // 
        intervalInput.Increment = new decimal(new int[] { 100, 0, 0, 0 });
        intervalInput.Location = new Point(200, 139);
        intervalInput.Maximum = new decimal(new int[] { 10000, 0, 0, 0 });
        intervalInput.Minimum = new decimal(new int[] { 100, 0, 0, 0 });
        intervalInput.Name = "intervalInput";
        intervalInput.Size = new Size(160, 27);
        intervalInput.TabIndex = 6;
        intervalInput.Value = new decimal(new int[] { 1000, 0, 0, 0 });
        intervalInput.BorderStyle = BorderStyle.FixedSingle;
        intervalInput.BackColor = Color.FromArgb(52, 56, 72);
        intervalInput.ForeColor = Color.FromArgb(235, 238, 245);
        // 
        // resetButton
        // 
        resetButton.Location = new Point(408, 364);
        resetButton.Name = "resetButton";
        resetButton.Size = new Size(120, 32);
        resetButton.TabIndex = 8;
        resetButton.Text = "Reset";
        resetButton.FlatStyle = FlatStyle.Flat;
        resetButton.FlatAppearance.BorderColor = Color.FromArgb(70, 75, 95);
        resetButton.FlatAppearance.BorderSize = 1;
        resetButton.BackColor = Color.FromArgb(60, 64, 82);
        resetButton.ForeColor = Color.FromArgb(235, 238, 245);
        resetButton.UseVisualStyleBackColor = false;
        resetButton.Click += ResetButton_Click;
        // 
        // inputCountLabel
        // 
        inputCountLabel.AutoSize = true;
        inputCountLabel.ForeColor = Color.FromArgb(200, 204, 214);
        inputCountLabel.Location = new Point(16, 344);
        inputCountLabel.Name = "inputCountLabel";
        inputCountLabel.Size = new Size(100, 20);
        inputCountLabel.TabIndex = 9;
        inputCountLabel.Text = "Input Count: 0";
        // 
        // inputCountTimer
        // 
        inputCountTimer.Interval = 1000;
        inputCountTimer.Tick += InputCount_Tick;
        // 
        // keybindButton
        // 
        keybindButton.Location = new Point(200, 206);
        keybindButton.Name = "keybindButton";
        keybindButton.Size = new Size(160, 32);
        keybindButton.TabIndex = 11;
        keybindButton.Text = "F8";
        keybindButton.FlatStyle = FlatStyle.Flat;
        keybindButton.FlatAppearance.BorderColor = Color.FromArgb(70, 75, 95);
        keybindButton.FlatAppearance.BorderSize = 1;
        keybindButton.BackColor = Color.FromArgb(60, 64, 82);
        keybindButton.ForeColor = Color.FromArgb(235, 238, 245);
        keybindButton.UseVisualStyleBackColor = false;
        keybindButton.Click += KeybindButton_Click;
        // 
        // keybindLabel
        // 
        keybindLabel.AutoSize = true;
        keybindLabel.ForeColor = Color.FromArgb(200, 204, 214);
        keybindLabel.Location = new Point(206, 183);
        keybindLabel.Name = "keybindLabel";
        keybindLabel.Size = new Size(135, 20);
        keybindLabel.TabIndex = 12;
        keybindLabel.Text = "Start/Stop Keybind";
        // 
        // targetKeyButton
        // 
        targetKeyButton.Location = new Point(200, 276);
        targetKeyButton.Name = "targetKeyButton";
        targetKeyButton.Size = new Size(160, 32);
        targetKeyButton.TabIndex = 13;
        targetKeyButton.Text = "LMB";
        targetKeyButton.FlatStyle = FlatStyle.Flat;
        targetKeyButton.FlatAppearance.BorderColor = Color.FromArgb(70, 75, 95);
        targetKeyButton.FlatAppearance.BorderSize = 1;
        targetKeyButton.BackColor = Color.FromArgb(60, 64, 82);
        targetKeyButton.ForeColor = Color.FromArgb(235, 238, 245);
        targetKeyButton.UseVisualStyleBackColor = false;
        targetKeyButton.Click += TargetInputKeyButton_Click;
        // 
        // targetKeyLabel
        // 
        targetKeyLabel.AutoSize = true;
        targetKeyLabel.ForeColor = Color.FromArgb(200, 204, 214);
        targetKeyLabel.Location = new Point(220, 253);
        targetKeyLabel.Name = "targetKeyLabel";
        targetKeyLabel.Size = new Size(116, 20);
        targetKeyLabel.TabIndex = 14;
        targetKeyLabel.Text = "Target Input Key";
        // 
        // MainForm
        // 
        AutoScaleDimensions = new SizeF(8F, 20F);
        AutoScaleMode = AutoScaleMode.Font;
        ClientSize = new Size(560, 408);
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
        StartPosition = FormStartPosition.CenterScreen;
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
