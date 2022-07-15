namespace Schedule2Excel2k16.Forms
{
    partial class Excel2ScheduleForm
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
			this.grbExcel = new System.Windows.Forms.GroupBox();
			this.lbExcelSheet = new System.Windows.Forms.ListBox();
			this.btnSelectExcel = new System.Windows.Forms.Button();
			this.label3 = new System.Windows.Forms.Label();
			this.grbSchedule = new System.Windows.Forms.GroupBox();
			this.label1 = new System.Windows.Forms.Label();
			this.lbScheduleList = new System.Windows.Forms.ListBox();
			this.label2 = new System.Windows.Forms.Label();
			this.lbScheduleFieldList = new System.Windows.Forms.ListBox();
			this.btnFillData = new System.Windows.Forms.Button();
			this.rtbLog = new System.Windows.Forms.RichTextBox();
			this.pgbImport = new System.Windows.Forms.ProgressBar();
			this.btnClose = new System.Windows.Forms.Button();
			this.openFileDialogExcel = new System.Windows.Forms.OpenFileDialog();
			this.grbExcel.SuspendLayout();
			this.grbSchedule.SuspendLayout();
			this.SuspendLayout();
			// 
			// grbExcel
			// 
			this.grbExcel.Controls.Add(this.lbExcelSheet);
			this.grbExcel.Controls.Add(this.btnSelectExcel);
			this.grbExcel.Controls.Add(this.label3);
			this.grbExcel.Location = new System.Drawing.Point(12, 12);
			this.grbExcel.Name = "grbExcel";
			this.grbExcel.Size = new System.Drawing.Size(205, 235);
			this.grbExcel.TabIndex = 12;
			this.grbExcel.TabStop = false;
			this.grbExcel.Text = "Excel File";
			// 
			// lbExcelSheet
			// 
			this.lbExcelSheet.FormattingEnabled = true;
			this.lbExcelSheet.Location = new System.Drawing.Point(9, 36);
			this.lbExcelSheet.Name = "lbExcelSheet";
			this.lbExcelSheet.Size = new System.Drawing.Size(184, 160);
			this.lbExcelSheet.TabIndex = 3;
			// 
			// btnSelectExcel
			// 
			this.btnSelectExcel.Location = new System.Drawing.Point(9, 202);
			this.btnSelectExcel.Name = "btnSelectExcel";
			this.btnSelectExcel.Size = new System.Drawing.Size(184, 23);
			this.btnSelectExcel.TabIndex = 2;
			this.btnSelectExcel.Text = "Select Excel File...";
			this.btnSelectExcel.UseVisualStyleBackColor = true;
			this.btnSelectExcel.Click += new System.EventHandler(this.btnSelectExcel_Click);
			// 
			// label3
			// 
			this.label3.AutoSize = true;
			this.label3.Location = new System.Drawing.Point(6, 18);
			this.label3.Name = "label3";
			this.label3.Size = new System.Drawing.Size(69, 13);
			this.label3.TabIndex = 8;
			this.label3.Text = "Excel Sheets";
			// 
			// grbSchedule
			// 
			this.grbSchedule.Controls.Add(this.label1);
			this.grbSchedule.Controls.Add(this.lbScheduleList);
			this.grbSchedule.Controls.Add(this.label2);
			this.grbSchedule.Controls.Add(this.lbScheduleFieldList);
			this.grbSchedule.Controls.Add(this.btnFillData);
			this.grbSchedule.Location = new System.Drawing.Point(223, 12);
			this.grbSchedule.Name = "grbSchedule";
			this.grbSchedule.Size = new System.Drawing.Size(363, 235);
			this.grbSchedule.TabIndex = 13;
			this.grbSchedule.TabStop = false;
			this.grbSchedule.Text = "Schedule";
			// 
			// label1
			// 
			this.label1.AutoSize = true;
			this.label1.Location = new System.Drawing.Point(6, 18);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(71, 13);
			this.label1.TabIndex = 5;
			this.label1.Text = "Schedule List";
			// 
			// lbScheduleList
			// 
			this.lbScheduleList.FormattingEnabled = true;
			this.lbScheduleList.HorizontalScrollbar = true;
			this.lbScheduleList.Location = new System.Drawing.Point(9, 36);
			this.lbScheduleList.Name = "lbScheduleList";
			this.lbScheduleList.Size = new System.Drawing.Size(184, 160);
			this.lbScheduleList.TabIndex = 0;
			this.lbScheduleList.SelectedIndexChanged += new System.EventHandler(this.lbScheduleList_SelectedIndexChanged);
			// 
			// label2
			// 
			this.label2.AutoSize = true;
			this.label2.Location = new System.Drawing.Point(196, 18);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(89, 13);
			this.label2.TabIndex = 6;
			this.label2.Text = "Editable Field List";
			// 
			// lbScheduleFieldList
			// 
			this.lbScheduleFieldList.FormattingEnabled = true;
			this.lbScheduleFieldList.HorizontalScrollbar = true;
			this.lbScheduleFieldList.Location = new System.Drawing.Point(199, 36);
			this.lbScheduleFieldList.Name = "lbScheduleFieldList";
			this.lbScheduleFieldList.SelectionMode = System.Windows.Forms.SelectionMode.MultiExtended;
			this.lbScheduleFieldList.Size = new System.Drawing.Size(154, 160);
			this.lbScheduleFieldList.TabIndex = 1;
			this.lbScheduleFieldList.SelectedIndexChanged += new System.EventHandler(this.lbScheduleFieldList_SelectedIndexChanged);
			// 
			// btnFillData
			// 
			this.btnFillData.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.btnFillData.ForeColor = System.Drawing.Color.Firebrick;
			this.btnFillData.Location = new System.Drawing.Point(9, 202);
			this.btnFillData.Name = "btnFillData";
			this.btnFillData.Size = new System.Drawing.Size(345, 23);
			this.btnFillData.TabIndex = 4;
			this.btnFillData.Text = "Fill Data";
			this.btnFillData.UseVisualStyleBackColor = true;
			this.btnFillData.Click += new System.EventHandler(this.btnFillData_Click);
			// 
			// rtbLog
			// 
			this.rtbLog.Location = new System.Drawing.Point(12, 274);
			this.rtbLog.Name = "rtbLog";
			this.rtbLog.ReadOnly = true;
			this.rtbLog.Size = new System.Drawing.Size(574, 109);
			this.rtbLog.TabIndex = 16;
			this.rtbLog.Text = "";
			// 
			// pgbImport
			// 
			this.pgbImport.Location = new System.Drawing.Point(12, 248);
			this.pgbImport.Name = "pgbImport";
			this.pgbImport.Size = new System.Drawing.Size(574, 20);
			this.pgbImport.TabIndex = 15;
			// 
			// btnClose
			// 
			this.btnClose.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.btnClose.Location = new System.Drawing.Point(511, 389);
			this.btnClose.Name = "btnClose";
			this.btnClose.Size = new System.Drawing.Size(75, 23);
			this.btnClose.TabIndex = 14;
			this.btnClose.Text = "Close";
			this.btnClose.UseVisualStyleBackColor = true;
			// 
			// openFileDialogExcel
			// 
			this.openFileDialogExcel.FileName = "openFileDialog1";
			// 
			// Excel2ScheduleForm
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.CancelButton = this.btnClose;
			this.ClientSize = new System.Drawing.Size(597, 420);
			this.Controls.Add(this.rtbLog);
			this.Controls.Add(this.pgbImport);
			this.Controls.Add(this.btnClose);
			this.Controls.Add(this.grbSchedule);
			this.Controls.Add(this.grbExcel);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
			this.Name = "Excel2ScheduleForm";
			this.ShowInTaskbar = false;
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
			this.Text = "Excel 2 Schedule";
			this.Load += new System.EventHandler(this.Excel2ScheduleForm_Load);
			this.grbExcel.ResumeLayout(false);
			this.grbExcel.PerformLayout();
			this.grbSchedule.ResumeLayout(false);
			this.grbSchedule.PerformLayout();
			this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.GroupBox grbExcel;
        private System.Windows.Forms.ListBox lbExcelSheet;
        private System.Windows.Forms.Button btnSelectExcel;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.GroupBox grbSchedule;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.ListBox lbScheduleList;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.ListBox lbScheduleFieldList;
        private System.Windows.Forms.Button btnFillData;
        private System.Windows.Forms.RichTextBox rtbLog;
        private System.Windows.Forms.ProgressBar pgbImport;
        private System.Windows.Forms.Button btnClose;
        private System.Windows.Forms.OpenFileDialog openFileDialogExcel;
    }
}