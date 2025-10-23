using App.Utils;

namespace App;

/// <summary>
/// Main application form containing UI layout.
/// </summary>
partial class MainForm
{
    /// <summary>
    /// Required designer variable for container-managed components.
    /// </summary>
    private System.ComponentModel.IContainer components = null;

    /// <summary>
    /// Dispose managed/unmanaged resources.
    /// </summary>
    /// <param name="disposing">
    /// True to dispose managed resources; otherwise false.
    /// </param>
    protected override void Dispose(bool disposing)
    {
        if (disposing && (components is not null))
        {
            components.Dispose();
        }
        base.Dispose(disposing);
    }

    #region Windows Form Designer generated code

    /// <summary>
    /// Designer-generated UI creation. Do not modify in code editor outside of layout tweaks.
    /// </summary>
    private void InitializeComponent()
    {
        components = new System.ComponentModel.Container();
        System.ComponentModel.ComponentResourceManager resources =
            new System.ComponentModel.ComponentResourceManager(typeof(MainForm));

        // ---- Controls (fields are declared at the bottom) ----
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

        mainTabs = new TabControl();
        tabGeneral = new TabPage();
        tabSchedule = new TabPage();
        tabSequence = new TabPage();
        tabConfig = new TabPage();

        // General tab contents
        groupRun = new GroupBox();
        labelIntervalHint = new Label();

        // Run Mode controls
        runUntilStoppedRadio = new RadioButton();
        runForCountRadio = new RadioButton();
        runCountInput = new NumericUpDown();
        runCountLabel = new Label();

        // Keys controls
        groupKeys = new GroupBox();

        // Schedule tab contents
        groupSchedule = new GroupBox();
        scheduleStartPicker = new DateTimePicker();
        scheduleStopPicker = new DateTimePicker();
        scheduleStartLabel = new Label();
        scheduleStopLabel = new Label();
        scheduleEnableStopCheck = new CheckBox();
        scheduleEnableStartCheck = new CheckBox();

        // Sequence tab contents
        groupSequence = new GroupBox();
        sequenceGrid = new DataGridView();
        colStep = new DataGridViewTextBoxColumn();
        colKey = new DataGridViewTextBoxColumn();
        colDelayMs = new DataGridViewTextBoxColumn();
        sequenceButtonsPanel = new Panel();
        sequenceAddButton = new Button();
        sequenceEditButton = new Button();
        sequenceRemoveButton = new Button();
        sequenceMoveUpButton = new Button();
        sequenceMoveDownButton = new Button();
        sequenceIntervalLabel = new Label();
        sequenceIntervalInput = new NumericUpDown();

        // Config tab contents
        groupConfig = new GroupBox();
        saveConfigButton = new Button();
        loadConfigButton = new Button();
        loadOnStartupCheck = new CheckBox();
        configPathLabel = new Label();
        configPathText = new TextBox();
        openConfigFolderButton = new Button();

        ((System.ComponentModel.ISupportInitialize)intervalInput).BeginInit();
        ((System.ComponentModel.ISupportInitialize)runCountInput).BeginInit();
        ((System.ComponentModel.ISupportInitialize)sequenceGrid).BeginInit();
        ((System.ComponentModel.ISupportInitialize)sequenceIntervalInput).BeginInit();
        SuspendLayout();

        #region Theme & Form Chrome

        BackColor = System.Drawing.Color.FromArgb(30, 32, 46);
        ForeColor = System.Drawing.Color.FromArgb(235, 238, 245);

        // Common colors kept as locals for readability
        var panelBg = System.Drawing.Color.FromArgb(40, 44, 60);
        var textSecondary = System.Drawing.Color.FromArgb(200, 204, 214);
        var border = System.Drawing.Color.FromArgb(70, 75, 95);

        #endregion

        #region Top Row (Start/Stop, Reset, Status labels)

        // Start/Stop
        startStopButton.Font = new System.Drawing.Font("Segoe UI", 14F);
        startStopButton.ForeColor = System.Drawing.Color.White;
        startStopButton.BackColor = System.Drawing.Color.Green;
        startStopButton.FlatStyle = FlatStyle.Flat;
        startStopButton.FlatAppearance.BorderColor = border;
        startStopButton.FlatAppearance.BorderSize = 1;
        startStopButton.Location = new System.Drawing.Point(20, 18);
        startStopButton.Name = "startStopButton";
        startStopButton.Size = new System.Drawing.Size(180, 52);
        startStopButton.TabIndex = 0;
        startStopButton.Text = "Start";
        startStopButton.UseVisualStyleBackColor = false;
        startStopButton.Click += StartStopButton_Click;

        // Reset
        resetButton.Location = new System.Drawing.Point(210, 18);
        resetButton.Name = "resetButton";
        resetButton.Size = new System.Drawing.Size(120, 52);
        resetButton.TabIndex = 1;
        resetButton.Text = "Reset";
        resetButton.FlatStyle = FlatStyle.Flat;
        resetButton.FlatAppearance.BorderColor = border;
        resetButton.FlatAppearance.BorderSize = 1;
        resetButton.BackColor = System.Drawing.Color.FromArgb(60, 64, 82);
        resetButton.ForeColor = System.Drawing.Color.FromArgb(235, 238, 245);
        resetButton.UseVisualStyleBackColor = false;
        resetButton.Click += ResetButton_Click;

        // Timer label
        timerLabel.AutoSize = true;
        timerLabel.ForeColor = textSecondary;
        timerLabel.Location = new System.Drawing.Point(350, 24);
        timerLabel.Name = "timerLabel";
        timerLabel.Size = new System.Drawing.Size(145, 20);
        timerLabel.TabIndex = 2;
        timerLabel.Text = Formatter.SetTimeLabel(activeTimerSecondsDefault);

        // Input count
        inputCountLabel.AutoSize = true;
        inputCountLabel.ForeColor = textSecondary;
        inputCountLabel.Location = new System.Drawing.Point(350, 50);
        inputCountLabel.Name = "inputCountLabel";
        inputCountLabel.Size = new System.Drawing.Size(116, 20);
        inputCountLabel.TabIndex = 3;
        inputCountLabel.Text = Formatter.SetInputCountLabel(inputCountDefault);

        // Timers
        runTimer.Interval = activeTimerIntervalDefault;
        runTimer.Tick += RunTimer_Tick;

        inputCountTimer.Interval = inputIntervalDefault;
        inputCountTimer.Tick += InputCount_Tick;

        #endregion

        #region Tabs: Container

        mainTabs.Appearance = TabAppearance.Normal;
        mainTabs.Location = new System.Drawing.Point(20, 86);
        mainTabs.Name = "mainTabs";
        mainTabs.Size = new System.Drawing.Size(840, 510);
        mainTabs.TabIndex = 10;
        mainTabs.Padding = new Point(15, 5);

        mainTabs.Controls.Add(tabGeneral);
        mainTabs.Controls.Add(tabSchedule);
        mainTabs.Controls.Add(tabSequence);
        mainTabs.Controls.Add(tabConfig);

        #endregion

        #region Tab: General

        tabGeneral.Text = "General";
        tabGeneral.BackColor = panelBg;

        // ----- Group: Run Controls -----
        groupRun.Text = "Run Controls";
        groupRun.ForeColor = ForeColor;
        groupRun.BackColor = panelBg;
        groupRun.Location = new System.Drawing.Point(16, 16);
        groupRun.Size = new System.Drawing.Size(790, 200);

        // Interval controls
        intervalLabel.AutoSize = true;
        intervalLabel.ForeColor = textSecondary;
        intervalLabel.Location = new System.Drawing.Point(20, 35);
        intervalLabel.Name = "intervalLabel";
        intervalLabel.Size = new System.Drawing.Size(148, 20);
        intervalLabel.Text = "Interval (milliseconds)";

        intervalInput.Increment = new decimal(new int[] { intervalInputIncrement, 0, 0, 0 });
        intervalInput.Maximum = new decimal(new int[] { intervalMaximum, 0, 0, 0 });
        intervalInput.Minimum = new decimal(new int[] { intervalMinimum, 0, 0, 0 });
        intervalInput.Value = new decimal(new int[] { inputIntervalDefault, 0, 0, 0 });
        intervalInput.BorderStyle = BorderStyle.FixedSingle;
        intervalInput.BackColor = System.Drawing.Color.FromArgb(52, 56, 72);
        intervalInput.ForeColor = ForeColor;
        intervalInput.Location = new System.Drawing.Point(24, 58);
        intervalInput.Size = new System.Drawing.Size(160, 27);
        intervalInput.Name = "intervalInput";
        intervalInput.TabIndex = 20;
        intervalInput.Enabled = true;

        labelIntervalHint.AutoSize = true;
        labelIntervalHint.ForeColor = textSecondary;
        labelIntervalHint.Location = new System.Drawing.Point(200, 61);
        labelIntervalHint.Text = $"{intervalMinimum} – {intervalMaximum} ms";

        // Run Mode controls
        runUntilStoppedRadio.AutoSize = true;
        runUntilStoppedRadio.ForeColor = ForeColor;
        runUntilStoppedRadio.Location = new System.Drawing.Point(24, 100);
        runUntilStoppedRadio.Text = "Run until stopped";
        runUntilStoppedRadio.Checked = true;

        runForCountRadio.AutoSize = true;
        runForCountRadio.ForeColor = ForeColor;
        runForCountRadio.Location = new System.Drawing.Point(24, 132);
        runForCountRadio.Text = "Run for a number of inputs:";
        runForCountRadio.CheckedChanged += RunForInputsSelectedChanged;

        runCountInput.BorderStyle = BorderStyle.FixedSingle;
        runCountInput.BackColor = System.Drawing.Color.FromArgb(52, 56, 72);
        runCountInput.ForeColor = ForeColor;
        runCountInput.Minimum = new decimal(new int[] { runCountInputMinimum, 0, 0, 0 });
        runCountInput.Maximum = new decimal(new int[] { runCountInputMaximum, 0, 0, 0 });
        runCountInput.Value = new decimal(new int[] { runCountInputDefault, 0, 0, 0 });
        runCountInput.Location = new System.Drawing.Point(264, 130);
        runCountInput.Size = new System.Drawing.Size(120, 27);
        runCountInput.TabIndex = 40;
        runCountInput.Enabled = false;

        runCountLabel.AutoSize = true;
        runCountLabel.ForeColor = textSecondary;
        runCountLabel.Location = new System.Drawing.Point(24, 168);
        runCountLabel.Text = "When count is reached, stop automatically.";

        groupRun.Controls.Add(intervalLabel);
        groupRun.Controls.Add(intervalInput);
        groupRun.Controls.Add(labelIntervalHint);
        groupRun.Controls.Add(runUntilStoppedRadio);
        groupRun.Controls.Add(runForCountRadio);
        groupRun.Controls.Add(runCountInput);
        groupRun.Controls.Add(runCountLabel);

        tabGeneral.Controls.Add(groupRun);

        // Group: Key Settings
        groupKeys.Text = "Key Settings";
        groupKeys.ForeColor = ForeColor;
        groupKeys.BackColor = panelBg;
        groupKeys.Location = new System.Drawing.Point(16, 230);
        groupKeys.Size = new System.Drawing.Size(790, 200);

        keybindLabel.AutoSize = true;
        keybindLabel.ForeColor = textSecondary;
        keybindLabel.Location = new System.Drawing.Point(20, 40);
        keybindLabel.Name = "keybindLabel";
        keybindLabel.Size = new System.Drawing.Size(135, 20);
        keybindLabel.TabIndex = 12;
        keybindLabel.Text = "Start/Stop Keybind";

        keybindButton.Location = new System.Drawing.Point(24, 63);
        keybindButton.Name = "keybindButton";
        keybindButton.Size = new System.Drawing.Size(160, 32);
        keybindButton.TabIndex = 11;
        keybindButton.Text = hotKeyDefault.ToString();
        keybindButton.FlatStyle = FlatStyle.Flat;
        keybindButton.FlatAppearance.BorderColor = border;
        keybindButton.FlatAppearance.BorderSize = 1;
        keybindButton.BackColor = System.Drawing.Color.FromArgb(60, 64, 82);
        keybindButton.ForeColor = ForeColor;
        keybindButton.UseVisualStyleBackColor = false;
        keybindButton.Click += KeybindButton_Click;

        targetKeyLabel.AutoSize = true;
        targetKeyLabel.ForeColor = textSecondary;
        targetKeyLabel.Location = new System.Drawing.Point(220, 40);
        targetKeyLabel.Name = "targetKeyLabel";
        targetKeyLabel.Size = new System.Drawing.Size(116, 20);
        targetKeyLabel.TabIndex = 14;
        targetKeyLabel.Text = "Target Input Key";

        targetKeyButton.Location = new System.Drawing.Point(224, 63);
        targetKeyButton.Name = "targetKeyButton";
        targetKeyButton.Size = new System.Drawing.Size(160, 32);
        targetKeyButton.TabIndex = 13;
        targetKeyButton.Text = targetKeyDefault.ToString();
        targetKeyButton.FlatStyle = FlatStyle.Flat;
        targetKeyButton.FlatAppearance.BorderColor = border;
        targetKeyButton.FlatAppearance.BorderSize = 1;
        targetKeyButton.BackColor = System.Drawing.Color.FromArgb(60, 64, 82);
        targetKeyButton.ForeColor = ForeColor;
        targetKeyButton.UseVisualStyleBackColor = false;
        targetKeyButton.Click += TargetInputKeyButton_Click;

        groupKeys.Controls.Add(keybindLabel);
        groupKeys.Controls.Add(keybindButton);
        groupKeys.Controls.Add(targetKeyLabel);
        groupKeys.Controls.Add(targetKeyButton);

        tabGeneral.Controls.Add(groupKeys);

        #endregion

        #region Tab: Schedule

        tabSchedule.Text = "Schedule";
        tabSchedule.BackColor = panelBg;

        groupSchedule.Text = "Start/Stop Schedule (Not Implemented Yet)";
        groupSchedule.ForeColor = ForeColor;
        groupSchedule.BackColor = panelBg;
        groupSchedule.Location = new System.Drawing.Point(16, 16);
        groupSchedule.Size = new System.Drawing.Size(790, 180);

        // Enable Start
        scheduleEnableStartCheck.AutoSize = true;
        scheduleEnableStartCheck.ForeColor = textSecondary;
        scheduleEnableStartCheck.Location = new System.Drawing.Point(20, 34);
        scheduleEnableStartCheck.Size = new System.Drawing.Size(158, 24);
        scheduleEnableStartCheck.Text = "Enable start time";
        scheduleEnableStartCheck.CheckedChanged += ScheduleEnableStartCheck_CheckedChanged;

        // Start
        scheduleStartLabel.AutoSize = true;
        scheduleStartLabel.ForeColor = textSecondary;
        scheduleStartLabel.Location = new System.Drawing.Point(40, 66);
        scheduleStartLabel.Text = "Start at:";

        scheduleStartPicker.CustomFormat = "yyyy-MM-dd HH:mm:ss";
        scheduleStartPicker.Format = DateTimePickerFormat.Custom;
        scheduleStartPicker.Location = new System.Drawing.Point(100, 62);
        scheduleStartPicker.Name = "scheduleStartPicker";
        scheduleStartPicker.Size = new System.Drawing.Size(220, 27);
        scheduleStartPicker.TabIndex = 30;
        scheduleStartPicker.Enabled = false;

        // Enable Stop
        scheduleEnableStopCheck.AutoSize = true;
        scheduleEnableStopCheck.ForeColor = textSecondary;
        scheduleEnableStopCheck.Location = new System.Drawing.Point(20, 106);
        scheduleEnableStopCheck.Size = new System.Drawing.Size(158, 24);
        scheduleEnableStopCheck.Text = "Enable stop time";
        scheduleEnableStopCheck.CheckedChanged += ScheduleEnableStopCheck_CheckedChanged;

        // Stop
        scheduleStopLabel.AutoSize = true;
        scheduleStopLabel.ForeColor = textSecondary;
        scheduleStopLabel.Location = new System.Drawing.Point(40, 138);
        scheduleStopLabel.Text = "Stop at:";

        scheduleStopPicker.CustomFormat = "yyyy-MM-dd HH:mm:ss";
        scheduleStopPicker.Format = DateTimePickerFormat.Custom;
        scheduleStopPicker.Location = new System.Drawing.Point(100, 134);
        scheduleStopPicker.Name = "scheduleStopPicker";
        scheduleStopPicker.Size = new System.Drawing.Size(220, 27);
        scheduleStopPicker.TabIndex = 31;
        scheduleStopPicker.Enabled = false;

        // Add to group
        groupSchedule.Controls.Add(scheduleEnableStartCheck);
        groupSchedule.Controls.Add(scheduleStartLabel);
        groupSchedule.Controls.Add(scheduleStartPicker);
        groupSchedule.Controls.Add(scheduleEnableStopCheck);
        groupSchedule.Controls.Add(scheduleStopLabel);
        groupSchedule.Controls.Add(scheduleStopPicker);

        tabSchedule.Controls.Add(groupSchedule);

        #endregion

        #region Tab: Sequence

        tabSequence.Text = "Sequence";
        tabSequence.BackColor = panelBg;

        groupSequence.Text = "Key Sequence (Not Implemented Yet)";
        groupSequence.ForeColor = ForeColor;
        groupSequence.BackColor = panelBg;
        groupSequence.Location = new System.Drawing.Point(16, 16);
        groupSequence.Size = new System.Drawing.Size(790, 420);

        sequenceGrid.AllowUserToAddRows = false;
        sequenceGrid.AllowUserToDeleteRows = false;
        sequenceGrid.AllowUserToResizeRows = false;
        sequenceGrid.BackgroundColor = System.Drawing.Color.FromArgb(52, 56, 72);
        sequenceGrid.BorderStyle = System.Windows.Forms.BorderStyle.None;
        sequenceGrid.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
        sequenceGrid.EnableHeadersVisualStyles = false;
        sequenceGrid.GridColor = border;
        sequenceGrid.Location = new System.Drawing.Point(20, 34);
        sequenceGrid.MultiSelect = false;
        sequenceGrid.Name = "sequenceGrid";
        sequenceGrid.RowHeadersVisible = false;
        sequenceGrid.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
        sequenceGrid.Size = new System.Drawing.Size(600, 300);
        sequenceGrid.ReadOnly = true;

        // Columns
        colStep.HeaderText = "#";
        colStep.Name = "colStep";
        colStep.ReadOnly = true;
        colStep.Width = 40;

        colKey.HeaderText = "Key / Click";
        colKey.Name = "colKey";
        colKey.ReadOnly = true;
        colKey.AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;

        colDelayMs.HeaderText = "Delay (ms)";
        colDelayMs.Name = "colDelayMs";
        colDelayMs.ReadOnly = true;
        colDelayMs.Width = 120;

        sequenceGrid.Columns.AddRange(new DataGridViewColumn[] { colStep, colKey, colDelayMs });

        sequenceButtonsPanel.Location = new System.Drawing.Point(630, 34);
        sequenceButtonsPanel.Size = new System.Drawing.Size(140, 300);

        sequenceAddButton.Text = "Add";
        sequenceAddButton.FlatStyle = FlatStyle.Flat;
        sequenceAddButton.FlatAppearance.BorderColor = border;
        sequenceAddButton.FlatAppearance.BorderSize = 1;
        sequenceAddButton.BackColor = System.Drawing.Color.FromArgb(60, 64, 82);
        sequenceAddButton.ForeColor = ForeColor;
        sequenceAddButton.Size = new System.Drawing.Size(120, 30);
        sequenceAddButton.Location = new System.Drawing.Point(10, 10);
        sequenceAddButton.Click += SequenceAddButton_Click;

        sequenceEditButton.Text = "Edit";
        sequenceEditButton.FlatStyle = FlatStyle.Flat;
        sequenceEditButton.FlatAppearance.BorderColor = border;
        sequenceEditButton.FlatAppearance.BorderSize = 1;
        sequenceEditButton.BackColor = System.Drawing.Color.FromArgb(60, 64, 82);
        sequenceEditButton.ForeColor = ForeColor;
        sequenceEditButton.Size = new System.Drawing.Size(120, 30);
        sequenceEditButton.Location = new System.Drawing.Point(10, 50);
        sequenceEditButton.Click += SequenceEditButton_Click;

        sequenceRemoveButton.Text = "Remove";
        sequenceRemoveButton.FlatStyle = FlatStyle.Flat;
        sequenceRemoveButton.FlatAppearance.BorderColor = border;
        sequenceRemoveButton.FlatAppearance.BorderSize = 1;
        sequenceRemoveButton.BackColor = System.Drawing.Color.FromArgb(60, 64, 82);
        sequenceRemoveButton.ForeColor = ForeColor;
        sequenceRemoveButton.Size = new System.Drawing.Size(120, 30);
        sequenceRemoveButton.Location = new System.Drawing.Point(10, 90);
        sequenceRemoveButton.Click += SequenceRemoveButton_Click;

        sequenceMoveUpButton.Text = "Move Up";
        sequenceMoveUpButton.FlatStyle = FlatStyle.Flat;
        sequenceMoveUpButton.FlatAppearance.BorderColor = border;
        sequenceMoveUpButton.FlatAppearance.BorderSize = 1;
        sequenceMoveUpButton.BackColor = System.Drawing.Color.FromArgb(60, 64, 82);
        sequenceMoveUpButton.ForeColor = ForeColor;
        sequenceMoveUpButton.Size = new System.Drawing.Size(120, 30);
        sequenceMoveUpButton.Location = new System.Drawing.Point(10, 130);
        sequenceMoveUpButton.Click += SequenceMoveUpButton_Click;

        sequenceMoveDownButton.Text = "Move Down";
        sequenceMoveDownButton.FlatStyle = FlatStyle.Flat;
        sequenceMoveDownButton.FlatAppearance.BorderColor = border;
        sequenceMoveDownButton.FlatAppearance.BorderSize = 1;
        sequenceMoveDownButton.BackColor = System.Drawing.Color.FromArgb(60, 64, 82);
        sequenceMoveDownButton.ForeColor = ForeColor;
        sequenceMoveDownButton.Size = new System.Drawing.Size(120, 30);
        sequenceMoveDownButton.Location = new System.Drawing.Point(10, 170);
        sequenceMoveDownButton.Click += SequenceMoveDownButton_Click;

        sequenceButtonsPanel.Controls.Add(sequenceAddButton);
        sequenceButtonsPanel.Controls.Add(sequenceEditButton);
        sequenceButtonsPanel.Controls.Add(sequenceRemoveButton);
        sequenceButtonsPanel.Controls.Add(sequenceMoveUpButton);
        sequenceButtonsPanel.Controls.Add(sequenceMoveDownButton);

        sequenceIntervalLabel.AutoSize = true;
        sequenceIntervalLabel.ForeColor = textSecondary;
        sequenceIntervalLabel.Location = new System.Drawing.Point(20, 350);
        sequenceIntervalLabel.Text = "Sequence interval (ms):";

        sequenceIntervalInput.BorderStyle = BorderStyle.FixedSingle;
        sequenceIntervalInput.BackColor = System.Drawing.Color.FromArgb(52, 56, 72);
        sequenceIntervalInput.ForeColor = ForeColor;
        sequenceIntervalInput.Minimum = new decimal(new int[] { intervalMinimum, 0, 0, 0 });
        sequenceIntervalInput.Maximum = new decimal(new int[] { sequenceIntervalInputMaximum, 0, 0, 0 });
        sequenceIntervalInput.Value = new decimal(new int[] { inputIntervalDefault, 0, 0, 0 });
        sequenceIntervalInput.Location = new System.Drawing.Point(190, 346);
        sequenceIntervalInput.Size = new System.Drawing.Size(120, 27);
        sequenceIntervalInput.Increment = intervalInputIncrement;

        groupSequence.Controls.Add(sequenceGrid);
        groupSequence.Controls.Add(sequenceButtonsPanel);
        groupSequence.Controls.Add(sequenceIntervalLabel);
        groupSequence.Controls.Add(sequenceIntervalInput);

        tabSequence.Controls.Add(groupSequence);

        #endregion

        #region Tab: Config

        tabConfig.Text = "Config";
        tabConfig.BackColor = panelBg;

        groupConfig.Text = "Configuration (Not Implemented Yet)";
        groupConfig.ForeColor = ForeColor;
        groupConfig.BackColor = panelBg;
        groupConfig.Location = new System.Drawing.Point(16, 16);
        groupConfig.Size = new System.Drawing.Size(790, 200);

        saveConfigButton.Text = "Save Config";
        saveConfigButton.FlatStyle = FlatStyle.Flat;
        saveConfigButton.FlatAppearance.BorderColor = border;
        saveConfigButton.FlatAppearance.BorderSize = 1;
        saveConfigButton.BackColor = System.Drawing.Color.FromArgb(60, 64, 82);
        saveConfigButton.ForeColor = ForeColor;
        saveConfigButton.Location = new System.Drawing.Point(24, 40);
        saveConfigButton.Size = new System.Drawing.Size(140, 32);
        saveConfigButton.Click += SaveConfigButton_Click;

        loadConfigButton.Text = "Load Config";
        loadConfigButton.FlatStyle = FlatStyle.Flat;
        loadConfigButton.FlatAppearance.BorderColor = border;
        loadConfigButton.FlatAppearance.BorderSize = 1;
        loadConfigButton.BackColor = System.Drawing.Color.FromArgb(60, 64, 82);
        loadConfigButton.ForeColor = ForeColor;
        loadConfigButton.Location = new System.Drawing.Point(174, 40);
        loadConfigButton.Size = new System.Drawing.Size(140, 32);
        loadConfigButton.Click += LoadConfigButton_Click;

        loadOnStartupCheck.AutoSize = true;
        loadOnStartupCheck.ForeColor = textSecondary;
        loadOnStartupCheck.Location = new System.Drawing.Point(24, 86);
        loadOnStartupCheck.Text = "Load saved configuration on startup";
        loadOnStartupCheck.CheckedChanged += LoadOnStartupCheck_CheckedChanged;

        configPathLabel.AutoSize = true;
        configPathLabel.ForeColor = textSecondary;
        configPathLabel.Location = new System.Drawing.Point(24, 120);
        configPathLabel.Text = "Config path:";

        configPathText.ReadOnly = true;
        configPathText.BorderStyle = BorderStyle.FixedSingle;
        configPathText.BackColor = System.Drawing.Color.FromArgb(52, 56, 72);
        configPathText.ForeColor = ForeColor;
        configPathText.Location = new System.Drawing.Point(24, 144);
        configPathText.Size = new System.Drawing.Size(520, 27);
        configPathText.Text = configPathTextDefault;

        openConfigFolderButton.Text = "Open Folder";
        openConfigFolderButton.FlatStyle = FlatStyle.Flat;
        openConfigFolderButton.FlatAppearance.BorderColor = border;
        openConfigFolderButton.FlatAppearance.BorderSize = 1;
        openConfigFolderButton.BackColor = System.Drawing.Color.FromArgb(60, 64, 82);
        openConfigFolderButton.ForeColor = ForeColor;
        openConfigFolderButton.Location = new System.Drawing.Point(554, 143);
        openConfigFolderButton.Size = new System.Drawing.Size(120, 29);
        openConfigFolderButton.Click += OpenConfigFolderButton_Click;

        groupConfig.Controls.Add(saveConfigButton);
        groupConfig.Controls.Add(loadConfigButton);
        groupConfig.Controls.Add(loadOnStartupCheck);
        groupConfig.Controls.Add(configPathLabel);
        groupConfig.Controls.Add(configPathText);
        groupConfig.Controls.Add(openConfigFolderButton);

        tabConfig.Controls.Add(groupConfig);

        #endregion

        #region Form Setup

        AutoScaleDimensions = new System.Drawing.SizeF(8F, 20F);
        AutoScaleMode = AutoScaleMode.Font;
        ClientSize = new System.Drawing.Size(880, 620);
        Controls.Add(mainTabs);
        Controls.Add(inputCountLabel);
        Controls.Add(timerLabel);
        Controls.Add(resetButton);
        Controls.Add(startStopButton);
        FormBorderStyle = FormBorderStyle.FixedDialog;
        Icon = (System.Drawing.Icon)resources.GetObject("$this.Icon");
        MaximizeBox = false;
        Name = "MainForm";
        StartPosition = FormStartPosition.CenterScreen;
        Text = "AutoInput";
        KeyPreview = true;
        Load += MainForm_Load;
        KeyDown += MainForm_KeyDown;     
        MouseDown += MainForm_MouseDown;

        #endregion

        ((System.ComponentModel.ISupportInitialize)intervalInput).EndInit();
        ((System.ComponentModel.ISupportInitialize)runCountInput).EndInit();
        ((System.ComponentModel.ISupportInitialize)sequenceGrid).EndInit();
        ((System.ComponentModel.ISupportInitialize)sequenceIntervalInput).EndInit();

        ResumeLayout(false);
        PerformLayout();
    }

    #endregion

    #region Field Declarations

    // Existing controls
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

    // Containers/tabs
    private TabControl mainTabs;
    private TabPage tabGeneral;
    private TabPage tabSchedule;
    private TabPage tabSequence;
    private TabPage tabConfig;

    // General
    private GroupBox groupRun;
    private Label labelIntervalHint;

    // Run Mode
    private RadioButton runUntilStoppedRadio;
    private RadioButton runForCountRadio;
    private NumericUpDown runCountInput;
    private Label runCountLabel;

    // Keys 
    private GroupBox groupKeys;

    // Schedule
    private GroupBox groupSchedule;
    private DateTimePicker scheduleStartPicker;
    private DateTimePicker scheduleStopPicker;
    private Label scheduleStartLabel;
    private Label scheduleStopLabel;
    private CheckBox scheduleEnableStopCheck;
    private CheckBox scheduleEnableStartCheck;

    // Sequence
    private GroupBox groupSequence;
    private DataGridView sequenceGrid;
    private DataGridViewTextBoxColumn colStep;
    private DataGridViewTextBoxColumn colKey;
    private DataGridViewTextBoxColumn colDelayMs;
    private Panel sequenceButtonsPanel;
    private Button sequenceAddButton;
    private Button sequenceEditButton;
    private Button sequenceRemoveButton;
    private Button sequenceMoveUpButton;
    private Button sequenceMoveDownButton;
    private Label sequenceIntervalLabel;
    private NumericUpDown sequenceIntervalInput;

    // Config
    private GroupBox groupConfig;
    private Button saveConfigButton;
    private Button loadConfigButton;
    private CheckBox loadOnStartupCheck;
    private Label configPathLabel;
    private TextBox configPathText;
    private Button openConfigFolderButton;

    #endregion
}
