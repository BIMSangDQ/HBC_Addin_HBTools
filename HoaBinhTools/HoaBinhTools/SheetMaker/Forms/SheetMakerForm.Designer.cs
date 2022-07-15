namespace SheetDuplicateAndAlignView.Forms
{
    partial class SheetMakerForm
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
            this.components = new System.ComponentModel.Container();
            this.label4 = new System.Windows.Forms.Label();
            this.rtbLog = new System.Windows.Forms.RichTextBox();
            this.grbSheet = new System.Windows.Forms.GroupBox();
            this.btnView = new System.Windows.Forms.Button();
            this.txtSearch = new System.Windows.Forms.TextBox();
            this.cboViewportType = new System.Windows.Forms.ComboBox();
            this.label3 = new System.Windows.Forms.Label();
            this.btnClearAllView = new System.Windows.Forms.Button();
            this.lbViewSheet = new System.Windows.Forms.ListBox();
            this.btnClearAllSchedule = new System.Windows.Forms.Button();
            this.btnLegend = new System.Windows.Forms.Button();
            this.btnClearAllLegend = new System.Windows.Forms.Button();
            this.btnSchedule = new System.Windows.Forms.Button();
            this.btnClose = new System.Windows.Forms.Button();
            this.btnInsertView = new System.Windows.Forms.Button();
            this.grbData = new System.Windows.Forms.GroupBox();
            this.btnCreateSheet = new System.Windows.Forms.Button();
            this.cboTitleBlock = new System.Windows.Forms.ComboBox();
            this.label2 = new System.Windows.Forms.Label();
            this.btnSelectExcelFile = new System.Windows.Forms.Button();
            this.txtExcelFile = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.statusStrip1 = new System.Windows.Forms.StatusStrip();
            this.cmsViewSheet = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.addViewsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.addLegendToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.addScheduleToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.myOpenFileDialog = new System.Windows.Forms.OpenFileDialog();
            this.grbSheet.SuspendLayout();
            this.grbData.SuspendLayout();
            this.cmsViewSheet.SuspendLayout();
            this.SuspendLayout();
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(12, 435);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(25, 13);
            this.label4.TabIndex = 20;
            this.label4.Text = "Log";
            // 
            // rtbLog
            // 
            this.rtbLog.Location = new System.Drawing.Point(12, 453);
            this.rtbLog.Name = "rtbLog";
            this.rtbLog.ReadOnly = true;
            this.rtbLog.Size = new System.Drawing.Size(749, 135);
            this.rtbLog.TabIndex = 19;
            this.rtbLog.Text = "";
            // 
            // grbSheet
            // 
            this.grbSheet.Controls.Add(this.btnView);
            this.grbSheet.Controls.Add(this.txtSearch);
            this.grbSheet.Controls.Add(this.cboViewportType);
            this.grbSheet.Controls.Add(this.label3);
            this.grbSheet.Controls.Add(this.btnClearAllView);
            this.grbSheet.Controls.Add(this.lbViewSheet);
            this.grbSheet.Controls.Add(this.btnClearAllSchedule);
            this.grbSheet.Controls.Add(this.btnLegend);
            this.grbSheet.Controls.Add(this.btnClearAllLegend);
            this.grbSheet.Controls.Add(this.btnSchedule);
            this.grbSheet.Location = new System.Drawing.Point(12, 104);
            this.grbSheet.Name = "grbSheet";
            this.grbSheet.Size = new System.Drawing.Size(749, 317);
            this.grbSheet.TabIndex = 18;
            this.grbSheet.TabStop = false;
            this.grbSheet.Text = "Sheets";
            // 
            // btnView
            // 
            this.btnView.Location = new System.Drawing.Point(560, 47);
            this.btnView.Name = "btnView";
            this.btnView.Size = new System.Drawing.Size(183, 23);
            this.btnView.TabIndex = 13;
            this.btnView.Text = "Add Views";
            this.btnView.UseVisualStyleBackColor = true;
            this.btnView.Click += new System.EventHandler(this.btnView_Click);
            // 
            // txtSearch
            // 
            this.txtSearch.Location = new System.Drawing.Point(9, 19);
            this.txtSearch.Name = "txtSearch";
            this.txtSearch.Size = new System.Drawing.Size(545, 20);
            this.txtSearch.TabIndex = 12;
            this.txtSearch.TextChanged += new System.EventHandler(this.txtSearch_TextChanged);
            // 
            // cboViewportType
            // 
            this.cboViewportType.FormattingEnabled = true;
            this.cboViewportType.Location = new System.Drawing.Point(9, 283);
            this.cboViewportType.Name = "cboViewportType";
            this.cboViewportType.Size = new System.Drawing.Size(545, 21);
            this.cboViewportType.TabIndex = 11;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(6, 267);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(75, 13);
            this.label3.TabIndex = 10;
            this.label3.Text = "Viewport Type";
            // 
            // btnClearAllView
            // 
            this.btnClearAllView.Location = new System.Drawing.Point(560, 177);
            this.btnClearAllView.Name = "btnClearAllView";
            this.btnClearAllView.Size = new System.Drawing.Size(183, 23);
            this.btnClearAllView.TabIndex = 9;
            this.btnClearAllView.Text = "Clear All Views";
            this.btnClearAllView.UseVisualStyleBackColor = true;
            this.btnClearAllView.Click += new System.EventHandler(this.btnClearAllView_Click);
            // 
            // lbViewSheet
            // 
            this.lbViewSheet.FormattingEnabled = true;
            this.lbViewSheet.Location = new System.Drawing.Point(9, 45);
            this.lbViewSheet.Name = "lbViewSheet";
            this.lbViewSheet.SelectionMode = System.Windows.Forms.SelectionMode.MultiExtended;
            this.lbViewSheet.Size = new System.Drawing.Size(545, 212);
            this.lbViewSheet.TabIndex = 2;
            this.lbViewSheet.MouseDoubleClick += new System.Windows.Forms.MouseEventHandler(this.lbViewSheet_MouseDoubleClick);
            this.lbViewSheet.MouseDown += new System.Windows.Forms.MouseEventHandler(this.lbViewSheet_MouseDown);
            this.lbViewSheet.MouseUp += new System.Windows.Forms.MouseEventHandler(this.lbViewSheet_MouseUp);
            // 
            // btnClearAllSchedule
            // 
            this.btnClearAllSchedule.Location = new System.Drawing.Point(560, 235);
            this.btnClearAllSchedule.Name = "btnClearAllSchedule";
            this.btnClearAllSchedule.Size = new System.Drawing.Size(183, 23);
            this.btnClearAllSchedule.TabIndex = 8;
            this.btnClearAllSchedule.Text = "Clear All Schedules";
            this.btnClearAllSchedule.UseVisualStyleBackColor = true;
            this.btnClearAllSchedule.Click += new System.EventHandler(this.btnClearAllSchedule_Click);
            // 
            // btnLegend
            // 
            this.btnLegend.Location = new System.Drawing.Point(560, 76);
            this.btnLegend.Name = "btnLegend";
            this.btnLegend.Size = new System.Drawing.Size(183, 23);
            this.btnLegend.TabIndex = 4;
            this.btnLegend.Text = "Add Legends For Sheets";
            this.btnLegend.UseVisualStyleBackColor = true;
            this.btnLegend.Click += new System.EventHandler(this.btnLegend_Click);
            // 
            // btnClearAllLegend
            // 
            this.btnClearAllLegend.Location = new System.Drawing.Point(560, 206);
            this.btnClearAllLegend.Name = "btnClearAllLegend";
            this.btnClearAllLegend.Size = new System.Drawing.Size(183, 23);
            this.btnClearAllLegend.TabIndex = 7;
            this.btnClearAllLegend.Text = "Clear All Legends";
            this.btnClearAllLegend.UseVisualStyleBackColor = true;
            this.btnClearAllLegend.Click += new System.EventHandler(this.btnClearAllLegend_Click);
            // 
            // btnSchedule
            // 
            this.btnSchedule.Location = new System.Drawing.Point(560, 105);
            this.btnSchedule.Name = "btnSchedule";
            this.btnSchedule.Size = new System.Drawing.Size(183, 23);
            this.btnSchedule.TabIndex = 5;
            this.btnSchedule.Text = "Add Schedules For Sheets";
            this.btnSchedule.UseVisualStyleBackColor = true;
            this.btnSchedule.Click += new System.EventHandler(this.btnSchedule_Click);
            // 
            // btnClose
            // 
            this.btnClose.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnClose.Location = new System.Drawing.Point(686, 598);
            this.btnClose.Name = "btnClose";
            this.btnClose.Size = new System.Drawing.Size(75, 23);
            this.btnClose.TabIndex = 17;
            this.btnClose.Text = "Close";
            this.btnClose.UseVisualStyleBackColor = true;
            // 
            // btnInsertView
            // 
            this.btnInsertView.Location = new System.Drawing.Point(605, 598);
            this.btnInsertView.Name = "btnInsertView";
            this.btnInsertView.Size = new System.Drawing.Size(75, 23);
            this.btnInsertView.TabIndex = 16;
            this.btnInsertView.Text = "Insert Views";
            this.btnInsertView.UseVisualStyleBackColor = true;
            this.btnInsertView.Click += new System.EventHandler(this.btnInsertView_Click);
            // 
            // grbData
            // 
            this.grbData.Controls.Add(this.btnCreateSheet);
            this.grbData.Controls.Add(this.cboTitleBlock);
            this.grbData.Controls.Add(this.label2);
            this.grbData.Controls.Add(this.btnSelectExcelFile);
            this.grbData.Controls.Add(this.txtExcelFile);
            this.grbData.Controls.Add(this.label1);
            this.grbData.Location = new System.Drawing.Point(12, 6);
            this.grbData.Name = "grbData";
            this.grbData.Size = new System.Drawing.Size(749, 80);
            this.grbData.TabIndex = 15;
            this.grbData.TabStop = false;
            this.grbData.Text = "Load Sheet Data From Excel";
            // 
            // btnCreateSheet
            // 
            this.btnCreateSheet.Location = new System.Drawing.Point(560, 46);
            this.btnCreateSheet.Name = "btnCreateSheet";
            this.btnCreateSheet.Size = new System.Drawing.Size(183, 23);
            this.btnCreateSheet.TabIndex = 5;
            this.btnCreateSheet.Text = "Create Sheets";
            this.btnCreateSheet.UseVisualStyleBackColor = true;
            this.btnCreateSheet.Click += new System.EventHandler(this.btnCreateSheet_Click);
            // 
            // cboTitleBlock
            // 
            this.cboTitleBlock.FormattingEnabled = true;
            this.cboTitleBlock.Location = new System.Drawing.Point(102, 47);
            this.cboTitleBlock.Name = "cboTitleBlock";
            this.cboTitleBlock.Size = new System.Drawing.Size(452, 21);
            this.cboTitleBlock.TabIndex = 4;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(6, 53);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(90, 13);
            this.label2.TabIndex = 3;
            this.label2.Text = "Select Title Block";
            // 
            // btnSelectExcelFile
            // 
            this.btnSelectExcelFile.Location = new System.Drawing.Point(560, 19);
            this.btnSelectExcelFile.Name = "btnSelectExcelFile";
            this.btnSelectExcelFile.Size = new System.Drawing.Size(183, 23);
            this.btnSelectExcelFile.TabIndex = 2;
            this.btnSelectExcelFile.Text = "Browse...";
            this.btnSelectExcelFile.UseVisualStyleBackColor = true;
            this.btnSelectExcelFile.Click += new System.EventHandler(this.btnSelectExcelFile_Click);
            // 
            // txtExcelFile
            // 
            this.txtExcelFile.Location = new System.Drawing.Point(102, 21);
            this.txtExcelFile.Name = "txtExcelFile";
            this.txtExcelFile.ReadOnly = true;
            this.txtExcelFile.Size = new System.Drawing.Size(452, 20);
            this.txtExcelFile.TabIndex = 1;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(6, 27);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(85, 13);
            this.label1.TabIndex = 0;
            this.label1.Text = "Select Excel File";
            // 
            // statusStrip1
            // 
            this.statusStrip1.Location = new System.Drawing.Point(0, 640);
            this.statusStrip1.Name = "statusStrip1";
            this.statusStrip1.Size = new System.Drawing.Size(773, 22);
            this.statusStrip1.TabIndex = 14;
            this.statusStrip1.Text = "statusStrip1";
            // 
            // cmsViewSheet
            // 
            this.cmsViewSheet.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.addViewsToolStripMenuItem,
            this.addLegendToolStripMenuItem,
            this.addScheduleToolStripMenuItem});
            this.cmsViewSheet.Name = "cmsViewSheet";
            this.cmsViewSheet.Size = new System.Drawing.Size(201, 70);
            // 
            // addViewsToolStripMenuItem
            // 
            this.addViewsToolStripMenuItem.Name = "addViewsToolStripMenuItem";
            this.addViewsToolStripMenuItem.Size = new System.Drawing.Size(200, 22);
            this.addViewsToolStripMenuItem.Text = "Add/Remove Views";
            // 
            // addLegendToolStripMenuItem
            // 
            this.addLegendToolStripMenuItem.Name = "addLegendToolStripMenuItem";
            this.addLegendToolStripMenuItem.Size = new System.Drawing.Size(200, 22);
            this.addLegendToolStripMenuItem.Text = "Add/Remove Legends";
            // 
            // addScheduleToolStripMenuItem
            // 
            this.addScheduleToolStripMenuItem.Name = "addScheduleToolStripMenuItem";
            this.addScheduleToolStripMenuItem.Size = new System.Drawing.Size(200, 22);
            this.addScheduleToolStripMenuItem.Text = "Add/Remove Schedules";
            // 
            // myOpenFileDialog
            // 
            this.myOpenFileDialog.FileName = "openFileDialog1";
            // 
            // SheetMakerForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(773, 662);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.rtbLog);
            this.Controls.Add(this.grbSheet);
            this.Controls.Add(this.btnClose);
            this.Controls.Add(this.btnInsertView);
            this.Controls.Add(this.grbData);
            this.Controls.Add(this.statusStrip1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            this.MaximizeBox = false;
            this.Name = "SheetMakerForm";
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "SheetMakerForm";
            this.Load += new System.EventHandler(this.SheetMakerForm_Load);
            this.grbSheet.ResumeLayout(false);
            this.grbSheet.PerformLayout();
            this.grbData.ResumeLayout(false);
            this.grbData.PerformLayout();
            this.cmsViewSheet.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.RichTextBox rtbLog;
        private System.Windows.Forms.GroupBox grbSheet;
        private System.Windows.Forms.Button btnView;
        private System.Windows.Forms.TextBox txtSearch;
        private System.Windows.Forms.ComboBox cboViewportType;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Button btnClearAllView;
        private System.Windows.Forms.ListBox lbViewSheet;
        private System.Windows.Forms.Button btnClearAllSchedule;
        private System.Windows.Forms.Button btnLegend;
        private System.Windows.Forms.Button btnClearAllLegend;
        private System.Windows.Forms.Button btnSchedule;
        private System.Windows.Forms.Button btnClose;
        private System.Windows.Forms.Button btnInsertView;
        private System.Windows.Forms.GroupBox grbData;
        private System.Windows.Forms.Button btnCreateSheet;
        private System.Windows.Forms.ComboBox cboTitleBlock;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Button btnSelectExcelFile;
        private System.Windows.Forms.TextBox txtExcelFile;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.StatusStrip statusStrip1;
        private System.Windows.Forms.ContextMenuStrip cmsViewSheet;
        private System.Windows.Forms.ToolStripMenuItem addViewsToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem addLegendToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem addScheduleToolStripMenuItem;
        private System.Windows.Forms.OpenFileDialog myOpenFileDialog;
    }
}