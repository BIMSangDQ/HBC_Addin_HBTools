namespace SheetDuplicateAndAlignView.Forms
{
    partial class ViewDuplicaterForm
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
            this.label5 = new System.Windows.Forms.Label();
            this.nudStartNumber = new System.Windows.Forms.NumericUpDown();
            this.label4 = new System.Windows.Forms.Label();
            this.chkViewQuantity = new System.Windows.Forms.CheckBox();
            this.nudViewQuantity = new System.Windows.Forms.NumericUpDown();
            this.statusStrip1 = new System.Windows.Forms.StatusStrip();
            this.toolStripStatusSelectedView = new System.Windows.Forms.ToolStripStatusLabel();
            this.rtbLog = new System.Windows.Forms.RichTextBox();
            this.btnRemoveSelected = new System.Windows.Forms.Button();
            this.btnAddSelected = new System.Windows.Forms.Button();
            this.lbSelectedScopeBox = new System.Windows.Forms.ListBox();
            this.lbScopeBox = new System.Windows.Forms.ListBox();
            this.btnDuplicateView = new System.Windows.Forms.Button();
            this.btnClose = new System.Windows.Forms.Button();
            this.label3 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.cboViewTemplate = new System.Windows.Forms.ComboBox();
            this.cboDuplicateView = new System.Windows.Forms.ComboBox();
            this.label1 = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.nudStartNumber)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudViewQuantity)).BeginInit();
            this.statusStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(81, 397);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(25, 13);
            this.label5.TabIndex = 36;
            this.label5.Text = "Log";
            // 
            // nudStartNumber
            // 
            this.nudStartNumber.Location = new System.Drawing.Point(380, 323);
            this.nudStartNumber.Maximum = new decimal(new int[] {
            1000,
            0,
            0,
            0});
            this.nudStartNumber.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.nudStartNumber.Name = "nudStartNumber";
            this.nudStartNumber.Size = new System.Drawing.Size(86, 20);
            this.nudStartNumber.TabIndex = 30;
            this.nudStartNumber.Value = new decimal(new int[] {
            1,
            0,
            0,
            0});
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(290, 328);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(69, 13);
            this.label4.TabIndex = 35;
            this.label4.Text = "Start Number";
            // 
            // chkViewQuantity
            // 
            this.chkViewQuantity.AutoSize = true;
            this.chkViewQuantity.Location = new System.Drawing.Point(16, 327);
            this.chkViewQuantity.Name = "chkViewQuantity";
            this.chkViewQuantity.Size = new System.Drawing.Size(91, 17);
            this.chkViewQuantity.TabIndex = 28;
            this.chkViewQuantity.Text = "View Quantity";
            this.chkViewQuantity.UseVisualStyleBackColor = true;
            this.chkViewQuantity.CheckedChanged += new System.EventHandler(this.chkViewQuantity_CheckedChanged);
            // 
            // nudViewQuantity
            // 
            this.nudViewQuantity.Location = new System.Drawing.Point(112, 323);
            this.nudViewQuantity.Maximum = new decimal(new int[] {
            1000,
            0,
            0,
            0});
            this.nudViewQuantity.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.nudViewQuantity.Name = "nudViewQuantity";
            this.nudViewQuantity.Size = new System.Drawing.Size(86, 20);
            this.nudViewQuantity.TabIndex = 29;
            this.nudViewQuantity.Value = new decimal(new int[] {
            1,
            0,
            0,
            0});
            // 
            // statusStrip1
            // 
            this.statusStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripStatusSelectedView});
            this.statusStrip1.Location = new System.Drawing.Point(0, 512);
            this.statusStrip1.Name = "statusStrip1";
            this.statusStrip1.Size = new System.Drawing.Size(898, 22);
            this.statusStrip1.TabIndex = 34;
            this.statusStrip1.Text = "statusStrip1";
            // 
            // toolStripStatusSelectedView
            // 
            this.toolStripStatusSelectedView.Name = "toolStripStatusSelectedView";
            this.toolStripStatusSelectedView.Size = new System.Drawing.Size(85, 17);
            this.toolStripStatusSelectedView.Text = "Selected View: ";
            // 
            // rtbLog
            // 
            this.rtbLog.Location = new System.Drawing.Point(112, 349);
            this.rtbLog.Name = "rtbLog";
            this.rtbLog.ReadOnly = true;
            this.rtbLog.Size = new System.Drawing.Size(776, 120);
            this.rtbLog.TabIndex = 31;
            this.rtbLog.Text = "";
            // 
            // btnRemoveSelected
            // 
            this.btnRemoveSelected.Location = new System.Drawing.Point(482, 195);
            this.btnRemoveSelected.Name = "btnRemoveSelected";
            this.btnRemoveSelected.Size = new System.Drawing.Size(33, 23);
            this.btnRemoveSelected.TabIndex = 27;
            this.btnRemoveSelected.Text = "<";
            this.btnRemoveSelected.UseVisualStyleBackColor = true;
            this.btnRemoveSelected.Click += new System.EventHandler(this.btnRemoveSelected_Click);
            // 
            // btnAddSelected
            // 
            this.btnAddSelected.Location = new System.Drawing.Point(482, 166);
            this.btnAddSelected.Name = "btnAddSelected";
            this.btnAddSelected.Size = new System.Drawing.Size(33, 23);
            this.btnAddSelected.TabIndex = 24;
            this.btnAddSelected.Text = ">";
            this.btnAddSelected.UseVisualStyleBackColor = true;
            this.btnAddSelected.Click += new System.EventHandler(this.btnAddSelected_Click);
            // 
            // lbSelectedScopeBox
            // 
            this.lbSelectedScopeBox.FormattingEnabled = true;
            this.lbSelectedScopeBox.HorizontalScrollbar = true;
            this.lbSelectedScopeBox.Location = new System.Drawing.Point(534, 62);
            this.lbSelectedScopeBox.Name = "lbSelectedScopeBox";
            this.lbSelectedScopeBox.SelectionMode = System.Windows.Forms.SelectionMode.MultiExtended;
            this.lbSelectedScopeBox.Size = new System.Drawing.Size(354, 251);
            this.lbSelectedScopeBox.TabIndex = 25;
            this.lbSelectedScopeBox.MouseDoubleClick += new System.Windows.Forms.MouseEventHandler(this.lbSelectedScopeBox_MouseDoubleClick);
            // 
            // lbScopeBox
            // 
            this.lbScopeBox.FormattingEnabled = true;
            this.lbScopeBox.HorizontalScrollbar = true;
            this.lbScopeBox.Location = new System.Drawing.Point(112, 62);
            this.lbScopeBox.Name = "lbScopeBox";
            this.lbScopeBox.SelectionMode = System.Windows.Forms.SelectionMode.MultiExtended;
            this.lbScopeBox.Size = new System.Drawing.Size(354, 251);
            this.lbScopeBox.TabIndex = 23;
            this.lbScopeBox.MouseDoubleClick += new System.Windows.Forms.MouseEventHandler(this.lbScopeBox_MouseDoubleClick);
            // 
            // btnDuplicateView
            // 
            this.btnDuplicateView.Location = new System.Drawing.Point(690, 475);
            this.btnDuplicateView.Name = "btnDuplicateView";
            this.btnDuplicateView.Size = new System.Drawing.Size(117, 23);
            this.btnDuplicateView.TabIndex = 32;
            this.btnDuplicateView.Text = "Duplicate View";
            this.btnDuplicateView.UseVisualStyleBackColor = true;
            this.btnDuplicateView.Click += new System.EventHandler(this.btnDuplicateView_Click);
            // 
            // btnClose
            // 
            this.btnClose.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnClose.Location = new System.Drawing.Point(813, 475);
            this.btnClose.Name = "btnClose";
            this.btnClose.Size = new System.Drawing.Size(75, 23);
            this.btnClose.TabIndex = 33;
            this.btnClose.Text = "Close";
            this.btnClose.UseVisualStyleBackColor = true;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(47, 175);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(59, 13);
            this.label3.TabIndex = 26;
            this.label3.Text = "Scope Box";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(29, 38);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(77, 13);
            this.label2.TabIndex = 22;
            this.label2.Text = "View Template";
            // 
            // cboViewTemplate
            // 
            this.cboViewTemplate.FormattingEnabled = true;
            this.cboViewTemplate.Location = new System.Drawing.Point(112, 32);
            this.cboViewTemplate.Name = "cboViewTemplate";
            this.cboViewTemplate.Size = new System.Drawing.Size(354, 21);
            this.cboViewTemplate.TabIndex = 21;
            // 
            // cboDuplicateView
            // 
            this.cboDuplicateView.AutoCompleteMode = System.Windows.Forms.AutoCompleteMode.SuggestAppend;
            this.cboDuplicateView.FormattingEnabled = true;
            this.cboDuplicateView.Location = new System.Drawing.Point(112, 5);
            this.cboDuplicateView.Name = "cboDuplicateView";
            this.cboDuplicateView.Size = new System.Drawing.Size(354, 21);
            this.cboDuplicateView.TabIndex = 20;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(20, 11);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(86, 13);
            this.label1.TabIndex = 19;
            this.label1.Text = "Duplicate Option";
            // 
            // ViewDuplicateForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.btnClose;
            this.ClientSize = new System.Drawing.Size(898, 534);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.nudStartNumber);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.chkViewQuantity);
            this.Controls.Add(this.nudViewQuantity);
            this.Controls.Add(this.statusStrip1);
            this.Controls.Add(this.rtbLog);
            this.Controls.Add(this.btnRemoveSelected);
            this.Controls.Add(this.btnAddSelected);
            this.Controls.Add(this.lbSelectedScopeBox);
            this.Controls.Add(this.lbScopeBox);
            this.Controls.Add(this.btnDuplicateView);
            this.Controls.Add(this.btnClose);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.cboViewTemplate);
            this.Controls.Add(this.cboDuplicateView);
            this.Controls.Add(this.label1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            this.MaximizeBox = false;
            this.Name = "ViewDuplicateForm";
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Duplicate View";
            this.Load += new System.EventHandler(this.ViewDuplicateForm_Load);
            ((System.ComponentModel.ISupportInitialize)(this.nudStartNumber)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudViewQuantity)).EndInit();
            this.statusStrip1.ResumeLayout(false);
            this.statusStrip1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.NumericUpDown nudStartNumber;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.CheckBox chkViewQuantity;
        private System.Windows.Forms.NumericUpDown nudViewQuantity;
        private System.Windows.Forms.StatusStrip statusStrip1;
        private System.Windows.Forms.ToolStripStatusLabel toolStripStatusSelectedView;
        private System.Windows.Forms.RichTextBox rtbLog;
        private System.Windows.Forms.Button btnRemoveSelected;
        private System.Windows.Forms.Button btnAddSelected;
        private System.Windows.Forms.ListBox lbSelectedScopeBox;
        private System.Windows.Forms.ListBox lbScopeBox;
        private System.Windows.Forms.Button btnDuplicateView;
        private System.Windows.Forms.Button btnClose;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.ComboBox cboViewTemplate;
        private System.Windows.Forms.ComboBox cboDuplicateView;
        private System.Windows.Forms.Label label1;
    }
}