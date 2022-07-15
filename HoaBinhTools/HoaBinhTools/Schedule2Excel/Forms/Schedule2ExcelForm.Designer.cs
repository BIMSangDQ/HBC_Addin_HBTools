namespace Schedule2Excel2k16.Forms
{
    partial class Schedule2ExcelForm
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
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
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.grbFileOption = new System.Windows.Forms.GroupBox();
            this.btnBrowse = new System.Windows.Forms.Button();
            this.txtSavePath = new System.Windows.Forms.TextBox();
            this.label4 = new System.Windows.Forms.Label();
            this.btnSelectFile = new System.Windows.Forms.Button();
            this.txtTemplateFile = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.chkUseDefaultTpl = new System.Windows.Forms.CheckBox();
            this.grbExportOption = new System.Windows.Forms.GroupBox();
            this.chkExportElementId = new System.Windows.Forms.CheckBox();
            this.chkExportFolder = new System.Windows.Forms.CheckBox();
            this.chkOpenExportFile = new System.Windows.Forms.CheckBox();
            this.chkFromLine = new System.Windows.Forms.CheckBox();
            this.nudStartLine = new System.Windows.Forms.NumericUpDown();
            this.grbTitleOption = new System.Windows.Forms.GroupBox();
            this.nudFontSize = new System.Windows.Forms.NumericUpDown();
            this.label6 = new System.Windows.Forms.Label();
            this.btnBackColor = new System.Windows.Forms.Button();
            this.btnForeColor = new System.Windows.Forms.Button();
            this.lblPreviewColor = new System.Windows.Forms.Label();
            this.grbSchedule2Excel = new System.Windows.Forms.GroupBox();
            this.cbIsMap = new System.Windows.Forms.CheckBox();
            this.btnExport = new System.Windows.Forms.Button();
            this.btnRemove = new System.Windows.Forms.Button();
            this.btnAdd = new System.Windows.Forms.Button();
            this.chkCheckAllExport = new System.Windows.Forms.CheckBox();
            this.lbExport2ExcelList = new System.Windows.Forms.ListBox();
            this.label2 = new System.Windows.Forms.Label();
            this.chkCheckAllSchedule = new System.Windows.Forms.CheckBox();
            this.lbScheduleList = new System.Windows.Forms.ListBox();
            this.label1 = new System.Windows.Forms.Label();
            this.grbOutput = new System.Windows.Forms.GroupBox();
            this.rtbLog = new System.Windows.Forms.RichTextBox();
            this.pgbExport = new System.Windows.Forms.ProgressBar();
            this.btnClose = new System.Windows.Forms.Button();
            this.myBackColorDialog = new System.Windows.Forms.ColorDialog();
            this.myForeColorDialog = new System.Windows.Forms.ColorDialog();
            this.mySaveFileDialog = new System.Windows.Forms.SaveFileDialog();
            this.myOpenFileDialog = new System.Windows.Forms.OpenFileDialog();
            this.chkShowWarning = new System.Windows.Forms.CheckBox();
            this.grbFileOption.SuspendLayout();
            this.grbExportOption.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.nudStartLine)).BeginInit();
            this.grbTitleOption.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.nudFontSize)).BeginInit();
            this.grbSchedule2Excel.SuspendLayout();
            this.grbOutput.SuspendLayout();
            this.SuspendLayout();
            // 
            // grbFileOption
            // 
            this.grbFileOption.Controls.Add(this.btnBrowse);
            this.grbFileOption.Controls.Add(this.txtSavePath);
            this.grbFileOption.Controls.Add(this.label4);
            this.grbFileOption.Controls.Add(this.btnSelectFile);
            this.grbFileOption.Controls.Add(this.txtTemplateFile);
            this.grbFileOption.Controls.Add(this.label3);
            this.grbFileOption.Controls.Add(this.chkUseDefaultTpl);
            this.grbFileOption.Location = new System.Drawing.Point(16, 15);
            this.grbFileOption.Margin = new System.Windows.Forms.Padding(4);
            this.grbFileOption.Name = "grbFileOption";
            this.grbFileOption.Padding = new System.Windows.Forms.Padding(4);
            this.grbFileOption.Size = new System.Drawing.Size(340, 185);
            this.grbFileOption.TabIndex = 0;
            this.grbFileOption.TabStop = false;
            this.grbFileOption.Text = "File Options";
            // 
            // btnBrowse
            // 
            this.btnBrowse.Location = new System.Drawing.Point(229, 139);
            this.btnBrowse.Margin = new System.Windows.Forms.Padding(4);
            this.btnBrowse.Name = "btnBrowse";
            this.btnBrowse.Size = new System.Drawing.Size(100, 28);
            this.btnBrowse.TabIndex = 8;
            this.btnBrowse.Text = "Browse...";
            this.btnBrowse.UseVisualStyleBackColor = true;
            this.btnBrowse.Click += new System.EventHandler(this.btnBrowse_Click);
            // 
            // txtSavePath
            // 
            this.txtSavePath.Location = new System.Drawing.Point(8, 142);
            this.txtSavePath.Margin = new System.Windows.Forms.Padding(4);
            this.txtSavePath.Name = "txtSavePath";
            this.txtSavePath.Size = new System.Drawing.Size(212, 22);
            this.txtSavePath.TabIndex = 7;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(4, 122);
            this.label4.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(60, 17);
            this.label4.TabIndex = 6;
            this.label4.Text = "Save As";
            // 
            // btnSelectFile
            // 
            this.btnSelectFile.Enabled = false;
            this.btnSelectFile.Location = new System.Drawing.Point(229, 76);
            this.btnSelectFile.Margin = new System.Windows.Forms.Padding(4);
            this.btnSelectFile.Name = "btnSelectFile";
            this.btnSelectFile.Size = new System.Drawing.Size(100, 28);
            this.btnSelectFile.TabIndex = 5;
            this.btnSelectFile.Text = "Select...";
            this.btnSelectFile.UseVisualStyleBackColor = true;
            this.btnSelectFile.Click += new System.EventHandler(this.btnSelectFile_Click);
            // 
            // txtTemplateFile
            // 
            this.txtTemplateFile.Location = new System.Drawing.Point(8, 79);
            this.txtTemplateFile.Margin = new System.Windows.Forms.Padding(4);
            this.txtTemplateFile.Name = "txtTemplateFile";
            this.txtTemplateFile.ReadOnly = true;
            this.txtTemplateFile.Size = new System.Drawing.Size(212, 22);
            this.txtTemplateFile.TabIndex = 4;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(4, 59);
            this.label3.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(93, 17);
            this.label3.TabIndex = 3;
            this.label3.Text = "Template File";
            // 
            // chkUseDefaultTpl
            // 
            this.chkUseDefaultTpl.AutoSize = true;
            this.chkUseDefaultTpl.Checked = true;
            this.chkUseDefaultTpl.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chkUseDefaultTpl.Location = new System.Drawing.Point(8, 23);
            this.chkUseDefaultTpl.Margin = new System.Windows.Forms.Padding(4);
            this.chkUseDefaultTpl.Name = "chkUseDefaultTpl";
            this.chkUseDefaultTpl.Size = new System.Drawing.Size(167, 21);
            this.chkUseDefaultTpl.TabIndex = 0;
            this.chkUseDefaultTpl.Text = "Use Default Template";
            this.chkUseDefaultTpl.UseVisualStyleBackColor = true;
            this.chkUseDefaultTpl.CheckedChanged += new System.EventHandler(this.chkUseDefaultTpl_CheckedChanged);
            // 
            // grbExportOption
            // 
            this.grbExportOption.Controls.Add(this.chkExportElementId);
            this.grbExportOption.Controls.Add(this.chkExportFolder);
            this.grbExportOption.Controls.Add(this.chkOpenExportFile);
            this.grbExportOption.Controls.Add(this.chkFromLine);
            this.grbExportOption.Controls.Add(this.nudStartLine);
            this.grbExportOption.Location = new System.Drawing.Point(16, 207);
            this.grbExportOption.Margin = new System.Windows.Forms.Padding(4);
            this.grbExportOption.Name = "grbExportOption";
            this.grbExportOption.Padding = new System.Windows.Forms.Padding(4);
            this.grbExportOption.Size = new System.Drawing.Size(340, 159);
            this.grbExportOption.TabIndex = 12;
            this.grbExportOption.TabStop = false;
            this.grbExportOption.Text = "Export Options";
            // 
            // chkExportElementId
            // 
            this.chkExportElementId.AutoSize = true;
            this.chkExportElementId.Checked = true;
            this.chkExportElementId.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chkExportElementId.Location = new System.Drawing.Point(12, 119);
            this.chkExportElementId.Margin = new System.Windows.Forms.Padding(4);
            this.chkExportElementId.Name = "chkExportElementId";
            this.chkExportElementId.Size = new System.Drawing.Size(252, 21);
            this.chkExportElementId.TabIndex = 18;
            this.chkExportElementId.Text = "Show \"Element_Unique_Id\" Column";
            this.chkExportElementId.UseVisualStyleBackColor = true;
            // 
            // chkExportFolder
            // 
            this.chkExportFolder.AutoSize = true;
            this.chkExportFolder.Location = new System.Drawing.Point(12, 59);
            this.chkExportFolder.Margin = new System.Windows.Forms.Padding(4);
            this.chkExportFolder.Name = "chkExportFolder";
            this.chkExportFolder.Size = new System.Drawing.Size(172, 21);
            this.chkExportFolder.TabIndex = 16;
            this.chkExportFolder.Text = "Open Exporting Folder";
            this.chkExportFolder.UseVisualStyleBackColor = true;
            // 
            // chkOpenExportFile
            // 
            this.chkOpenExportFile.AutoSize = true;
            this.chkOpenExportFile.Checked = true;
            this.chkOpenExportFile.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chkOpenExportFile.Location = new System.Drawing.Point(12, 91);
            this.chkOpenExportFile.Margin = new System.Windows.Forms.Padding(4);
            this.chkOpenExportFile.Name = "chkOpenExportFile";
            this.chkOpenExportFile.Size = new System.Drawing.Size(185, 21);
            this.chkOpenExportFile.TabIndex = 15;
            this.chkOpenExportFile.Text = "Open File After Exported";
            this.chkOpenExportFile.UseVisualStyleBackColor = true;
            // 
            // chkFromLine
            // 
            this.chkFromLine.AutoSize = true;
            this.chkFromLine.Checked = true;
            this.chkFromLine.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chkFromLine.Location = new System.Drawing.Point(12, 28);
            this.chkFromLine.Margin = new System.Windows.Forms.Padding(4);
            this.chkFromLine.Name = "chkFromLine";
            this.chkFromLine.Size = new System.Drawing.Size(166, 21);
            this.chkFromLine.TabIndex = 14;
            this.chkFromLine.Text = "Insert Data From Line";
            this.chkFromLine.UseVisualStyleBackColor = true;
            this.chkFromLine.CheckedChanged += new System.EventHandler(this.chkFromLine_CheckedChanged);
            // 
            // nudStartLine
            // 
            this.nudStartLine.Location = new System.Drawing.Point(189, 22);
            this.nudStartLine.Margin = new System.Windows.Forms.Padding(4);
            this.nudStartLine.Maximum = new decimal(new int[] {
            30,
            0,
            0,
            0});
            this.nudStartLine.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.nudStartLine.Name = "nudStartLine";
            this.nudStartLine.Size = new System.Drawing.Size(63, 22);
            this.nudStartLine.TabIndex = 12;
            this.nudStartLine.Value = new decimal(new int[] {
            10,
            0,
            0,
            0});
            // 
            // grbTitleOption
            // 
            this.grbTitleOption.Controls.Add(this.nudFontSize);
            this.grbTitleOption.Controls.Add(this.label6);
            this.grbTitleOption.Controls.Add(this.btnBackColor);
            this.grbTitleOption.Controls.Add(this.btnForeColor);
            this.grbTitleOption.Controls.Add(this.lblPreviewColor);
            this.grbTitleOption.Location = new System.Drawing.Point(16, 373);
            this.grbTitleOption.Margin = new System.Windows.Forms.Padding(4);
            this.grbTitleOption.Name = "grbTitleOption";
            this.grbTitleOption.Padding = new System.Windows.Forms.Padding(4);
            this.grbTitleOption.Size = new System.Drawing.Size(340, 148);
            this.grbTitleOption.TabIndex = 15;
            this.grbTitleOption.TabStop = false;
            this.grbTitleOption.Text = "Title Color && Size";
            // 
            // nudFontSize
            // 
            this.nudFontSize.Location = new System.Drawing.Point(81, 105);
            this.nudFontSize.Margin = new System.Windows.Forms.Padding(4);
            this.nudFontSize.Maximum = new decimal(new int[] {
            72,
            0,
            0,
            0});
            this.nudFontSize.Minimum = new decimal(new int[] {
            8,
            0,
            0,
            0});
            this.nudFontSize.Name = "nudFontSize";
            this.nudFontSize.Size = new System.Drawing.Size(68, 22);
            this.nudFontSize.TabIndex = 4;
            this.nudFontSize.Value = new decimal(new int[] {
            14,
            0,
            0,
            0});
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(8, 112);
            this.label6.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(67, 17);
            this.label6.TabIndex = 3;
            this.label6.Text = "Font Size";
            // 
            // btnBackColor
            // 
            this.btnBackColor.Location = new System.Drawing.Point(12, 62);
            this.btnBackColor.Margin = new System.Windows.Forms.Padding(4);
            this.btnBackColor.Name = "btnBackColor";
            this.btnBackColor.Size = new System.Drawing.Size(140, 28);
            this.btnBackColor.TabIndex = 2;
            this.btnBackColor.Text = "Back Color";
            this.btnBackColor.UseVisualStyleBackColor = true;
            this.btnBackColor.Click += new System.EventHandler(this.btnBackColor_Click);
            // 
            // btnForeColor
            // 
            this.btnForeColor.Location = new System.Drawing.Point(12, 26);
            this.btnForeColor.Margin = new System.Windows.Forms.Padding(4);
            this.btnForeColor.Name = "btnForeColor";
            this.btnForeColor.Size = new System.Drawing.Size(140, 28);
            this.btnForeColor.TabIndex = 1;
            this.btnForeColor.Text = "Fore Color";
            this.btnForeColor.UseVisualStyleBackColor = true;
            this.btnForeColor.Click += new System.EventHandler(this.btnForeColor_Click);
            // 
            // lblPreviewColor
            // 
            this.lblPreviewColor.BackColor = System.Drawing.Color.SeaGreen;
            this.lblPreviewColor.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lblPreviewColor.Font = new System.Drawing.Font("Microsoft Sans Serif", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblPreviewColor.ForeColor = System.Drawing.Color.White;
            this.lblPreviewColor.Location = new System.Drawing.Point(177, 26);
            this.lblPreviewColor.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lblPreviewColor.Name = "lblPreviewColor";
            this.lblPreviewColor.Size = new System.Drawing.Size(133, 64);
            this.lblPreviewColor.TabIndex = 0;
            this.lblPreviewColor.Text = "Text";
            this.lblPreviewColor.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // grbSchedule2Excel
            // 
            this.grbSchedule2Excel.Controls.Add(this.cbIsMap);
            this.grbSchedule2Excel.Controls.Add(this.btnExport);
            this.grbSchedule2Excel.Controls.Add(this.btnRemove);
            this.grbSchedule2Excel.Controls.Add(this.btnAdd);
            this.grbSchedule2Excel.Controls.Add(this.chkCheckAllExport);
            this.grbSchedule2Excel.Controls.Add(this.lbExport2ExcelList);
            this.grbSchedule2Excel.Controls.Add(this.label2);
            this.grbSchedule2Excel.Controls.Add(this.chkCheckAllSchedule);
            this.grbSchedule2Excel.Controls.Add(this.lbScheduleList);
            this.grbSchedule2Excel.Controls.Add(this.label1);
            this.grbSchedule2Excel.Location = new System.Drawing.Point(364, 15);
            this.grbSchedule2Excel.Margin = new System.Windows.Forms.Padding(4);
            this.grbSchedule2Excel.Name = "grbSchedule2Excel";
            this.grbSchedule2Excel.Padding = new System.Windows.Forms.Padding(4);
            this.grbSchedule2Excel.Size = new System.Drawing.Size(731, 315);
            this.grbSchedule2Excel.TabIndex = 16;
            this.grbSchedule2Excel.TabStop = false;
            this.grbSchedule2Excel.Text = "Schedule Data";
            // 
            // cbIsMap
            // 
            this.cbIsMap.AutoSize = true;
            this.cbIsMap.Location = new System.Drawing.Point(317, 43);
            this.cbIsMap.Name = "cbIsMap";
            this.cbIsMap.Size = new System.Drawing.Size(99, 21);
            this.cbIsMap.TabIndex = 20;
            this.cbIsMap.Text = "Map Image";
            this.cbIsMap.UseVisualStyleBackColor = true;
            // 
            // btnExport
            // 
            this.btnExport.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnExport.ForeColor = System.Drawing.Color.Maroon;
            this.btnExport.Location = new System.Drawing.Point(316, 249);
            this.btnExport.Margin = new System.Windows.Forms.Padding(4);
            this.btnExport.Name = "btnExport";
            this.btnExport.Size = new System.Drawing.Size(100, 55);
            this.btnExport.TabIndex = 21;
            this.btnExport.Text = "Export";
            this.btnExport.UseVisualStyleBackColor = true;
            this.btnExport.Click += new System.EventHandler(this.btnExport_Click);
            // 
            // btnRemove
            // 
            this.btnRemove.Location = new System.Drawing.Point(316, 145);
            this.btnRemove.Margin = new System.Windows.Forms.Padding(4);
            this.btnRemove.Name = "btnRemove";
            this.btnRemove.Size = new System.Drawing.Size(100, 28);
            this.btnRemove.TabIndex = 20;
            this.btnRemove.Text = "<==";
            this.btnRemove.UseVisualStyleBackColor = true;
            this.btnRemove.Click += new System.EventHandler(this.btnRemove_Click);
            // 
            // btnAdd
            // 
            this.btnAdd.Location = new System.Drawing.Point(316, 98);
            this.btnAdd.Margin = new System.Windows.Forms.Padding(4);
            this.btnAdd.Name = "btnAdd";
            this.btnAdd.Size = new System.Drawing.Size(100, 28);
            this.btnAdd.TabIndex = 19;
            this.btnAdd.Text = "==>";
            this.btnAdd.UseVisualStyleBackColor = true;
            this.btnAdd.Click += new System.EventHandler(this.btnAdd_Click);
            // 
            // chkCheckAllExport
            // 
            this.chkCheckAllExport.AutoSize = true;
            this.chkCheckAllExport.Location = new System.Drawing.Point(625, 20);
            this.chkCheckAllExport.Margin = new System.Windows.Forms.Padding(4);
            this.chkCheckAllExport.Name = "chkCheckAllExport";
            this.chkCheckAllExport.Size = new System.Drawing.Size(88, 21);
            this.chkCheckAllExport.TabIndex = 18;
            this.chkCheckAllExport.Text = "Select All";
            this.chkCheckAllExport.UseVisualStyleBackColor = true;
            this.chkCheckAllExport.Click += new System.EventHandler(this.chkCheckAllExport_Click);
            // 
            // lbExport2ExcelList
            // 
            this.lbExport2ExcelList.FormattingEnabled = true;
            this.lbExport2ExcelList.HorizontalScrollbar = true;
            this.lbExport2ExcelList.ItemHeight = 16;
            this.lbExport2ExcelList.Location = new System.Drawing.Point(456, 43);
            this.lbExport2ExcelList.Margin = new System.Windows.Forms.Padding(4);
            this.lbExport2ExcelList.Name = "lbExport2ExcelList";
            this.lbExport2ExcelList.SelectionMode = System.Windows.Forms.SelectionMode.MultiExtended;
            this.lbExport2ExcelList.Size = new System.Drawing.Size(265, 260);
            this.lbExport2ExcelList.TabIndex = 17;
            this.lbExport2ExcelList.MouseDoubleClick += new System.Windows.Forms.MouseEventHandler(this.lbExport2ExcelList_MouseDoubleClick);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(452, 21);
            this.label2.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(134, 17);
            this.label2.TabIndex = 16;
            this.label2.Text = "Schedule2Excel List";
            // 
            // chkCheckAllSchedule
            // 
            this.chkCheckAllSchedule.AutoSize = true;
            this.chkCheckAllSchedule.Location = new System.Drawing.Point(177, 20);
            this.chkCheckAllSchedule.Margin = new System.Windows.Forms.Padding(4);
            this.chkCheckAllSchedule.Name = "chkCheckAllSchedule";
            this.chkCheckAllSchedule.Size = new System.Drawing.Size(88, 21);
            this.chkCheckAllSchedule.TabIndex = 15;
            this.chkCheckAllSchedule.Text = "Select All";
            this.chkCheckAllSchedule.UseVisualStyleBackColor = true;
            this.chkCheckAllSchedule.Click += new System.EventHandler(this.chkCheckAllSchedule_Click);
            // 
            // lbScheduleList
            // 
            this.lbScheduleList.FormattingEnabled = true;
            this.lbScheduleList.HorizontalScrollbar = true;
            this.lbScheduleList.ItemHeight = 16;
            this.lbScheduleList.Location = new System.Drawing.Point(8, 43);
            this.lbScheduleList.Margin = new System.Windows.Forms.Padding(4);
            this.lbScheduleList.Name = "lbScheduleList";
            this.lbScheduleList.SelectionMode = System.Windows.Forms.SelectionMode.MultiExtended;
            this.lbScheduleList.Size = new System.Drawing.Size(265, 260);
            this.lbScheduleList.TabIndex = 14;
            this.lbScheduleList.MouseDoubleClick += new System.Windows.Forms.MouseEventHandler(this.lbScheduleList_MouseDoubleClick);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(4, 21);
            this.label1.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(93, 17);
            this.label1.TabIndex = 13;
            this.label1.Text = "Schedule List";
            // 
            // grbOutput
            // 
            this.grbOutput.Controls.Add(this.rtbLog);
            this.grbOutput.Controls.Add(this.pgbExport);
            this.grbOutput.Location = new System.Drawing.Point(364, 337);
            this.grbOutput.Margin = new System.Windows.Forms.Padding(4);
            this.grbOutput.Name = "grbOutput";
            this.grbOutput.Padding = new System.Windows.Forms.Padding(4);
            this.grbOutput.Size = new System.Drawing.Size(731, 204);
            this.grbOutput.TabIndex = 17;
            this.grbOutput.TabStop = false;
            this.grbOutput.Text = "Output";
            // 
            // rtbLog
            // 
            this.rtbLog.Location = new System.Drawing.Point(8, 54);
            this.rtbLog.Margin = new System.Windows.Forms.Padding(4);
            this.rtbLog.Name = "rtbLog";
            this.rtbLog.ReadOnly = true;
            this.rtbLog.ScrollBars = System.Windows.Forms.RichTextBoxScrollBars.ForcedVertical;
            this.rtbLog.Size = new System.Drawing.Size(713, 142);
            this.rtbLog.TabIndex = 16;
            this.rtbLog.Text = "";
            // 
            // pgbExport
            // 
            this.pgbExport.Location = new System.Drawing.Point(8, 23);
            this.pgbExport.Margin = new System.Windows.Forms.Padding(4);
            this.pgbExport.Name = "pgbExport";
            this.pgbExport.Size = new System.Drawing.Size(715, 25);
            this.pgbExport.TabIndex = 9;
            // 
            // btnClose
            // 
            this.btnClose.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnClose.Location = new System.Drawing.Point(995, 549);
            this.btnClose.Margin = new System.Windows.Forms.Padding(4);
            this.btnClose.Name = "btnClose";
            this.btnClose.Size = new System.Drawing.Size(100, 28);
            this.btnClose.TabIndex = 18;
            this.btnClose.Text = "Close";
            this.btnClose.UseVisualStyleBackColor = true;
            this.btnClose.Click += new System.EventHandler(this.btnClose_Click);
            // 
            // myOpenFileDialog
            // 
            this.myOpenFileDialog.FileName = "openFileDialog1";
            // 
            // chkShowWarning
            // 
            this.chkShowWarning.AutoSize = true;
            this.chkShowWarning.Location = new System.Drawing.Point(725, 554);
            this.chkShowWarning.Margin = new System.Windows.Forms.Padding(4);
            this.chkShowWarning.Name = "chkShowWarning";
            this.chkShowWarning.Size = new System.Drawing.Size(244, 21);
            this.chkShowWarning.TabIndex = 19;
            this.chkShowWarning.Text = "Shows warning when add-in starts";
            this.chkShowWarning.UseVisualStyleBackColor = true;
            this.chkShowWarning.Click += new System.EventHandler(this.chkShowWarning_Click);
            // 
            // Schedule2ExcelForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.btnClose;
            this.ClientSize = new System.Drawing.Size(1111, 592);
            this.Controls.Add(this.chkShowWarning);
            this.Controls.Add(this.btnClose);
            this.Controls.Add(this.grbOutput);
            this.Controls.Add(this.grbSchedule2Excel);
            this.Controls.Add(this.grbTitleOption);
            this.Controls.Add(this.grbExportOption);
            this.Controls.Add(this.grbFileOption);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            this.Margin = new System.Windows.Forms.Padding(4);
            this.Name = "Schedule2ExcelForm";
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Schedule 2 Excel";
            this.Load += new System.EventHandler(this.Schedule2ExcelForm_Load);
            this.Shown += new System.EventHandler(this.Schedule2ExcelForm_Shown);
            this.grbFileOption.ResumeLayout(false);
            this.grbFileOption.PerformLayout();
            this.grbExportOption.ResumeLayout(false);
            this.grbExportOption.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.nudStartLine)).EndInit();
            this.grbTitleOption.ResumeLayout(false);
            this.grbTitleOption.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.nudFontSize)).EndInit();
            this.grbSchedule2Excel.ResumeLayout(false);
            this.grbSchedule2Excel.PerformLayout();
            this.grbOutput.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.GroupBox grbFileOption;
        private System.Windows.Forms.TextBox txtTemplateFile;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.CheckBox chkUseDefaultTpl;
        private System.Windows.Forms.Button btnSelectFile;
        private System.Windows.Forms.Button btnBrowse;
        private System.Windows.Forms.TextBox txtSavePath;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.GroupBox grbExportOption;
        private System.Windows.Forms.CheckBox chkExportFolder;
        private System.Windows.Forms.CheckBox chkOpenExportFile;
        private System.Windows.Forms.CheckBox chkFromLine;
        private System.Windows.Forms.NumericUpDown nudStartLine;
        private System.Windows.Forms.GroupBox grbTitleOption;
        private System.Windows.Forms.NumericUpDown nudFontSize;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Button btnBackColor;
        private System.Windows.Forms.Button btnForeColor;
        private System.Windows.Forms.Label lblPreviewColor;
        private System.Windows.Forms.GroupBox grbSchedule2Excel;
        private System.Windows.Forms.CheckBox chkCheckAllExport;
        private System.Windows.Forms.ListBox lbExport2ExcelList;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.CheckBox chkCheckAllSchedule;
        private System.Windows.Forms.ListBox lbScheduleList;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button btnExport;
        private System.Windows.Forms.Button btnRemove;
        private System.Windows.Forms.Button btnAdd;
        private System.Windows.Forms.GroupBox grbOutput;
        private System.Windows.Forms.RichTextBox rtbLog;
        private System.Windows.Forms.ProgressBar pgbExport;
        private System.Windows.Forms.Button btnClose;
        private System.Windows.Forms.ColorDialog myBackColorDialog;
        private System.Windows.Forms.ColorDialog myForeColorDialog;
        private System.Windows.Forms.SaveFileDialog mySaveFileDialog;
        private System.Windows.Forms.OpenFileDialog myOpenFileDialog;
        private System.Windows.Forms.CheckBox chkExportElementId;
        private System.Windows.Forms.CheckBox chkShowWarning;
        private System.Windows.Forms.CheckBox cbIsMap;
    }
}