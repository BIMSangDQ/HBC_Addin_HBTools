namespace SheetDuplicateAndAlignView.Forms
{
    partial class AlignViewForm
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
            this.label4 = new System.Windows.Forms.Label();
            this.statusStrip1 = new System.Windows.Forms.StatusStrip();
            this.txtAlignViewSearch = new System.Windows.Forms.TextBox();
            this.btnRemoveSelected = new System.Windows.Forms.Button();
            this.btnAddSelected = new System.Windows.Forms.Button();
            this.rtbLog = new System.Windows.Forms.RichTextBox();
            this.btnApply = new System.Windows.Forms.Button();
            this.btnClose = new System.Windows.Forms.Button();
            this.label3 = new System.Windows.Forms.Label();
            this.cboAlignPosition = new System.Windows.Forms.ComboBox();
            this.lbSelectedAlignedView = new System.Windows.Forms.ListBox();
            this.lbAlignedView = new System.Windows.Forms.ListBox();
            this.label2 = new System.Windows.Forms.Label();
            this.cboPrimaryView = new System.Windows.Forms.ComboBox();
            this.label1 = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(83, 432);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(25, 13);
            this.label5.TabIndex = 29;
            this.label5.Text = "Log";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(556, 45);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(118, 13);
            this.label4.TabIndex = 28;
            this.label4.Text = "Selected Aligned Views";
            // 
            // statusStrip1
            // 
            this.statusStrip1.Location = new System.Drawing.Point(0, 548);
            this.statusStrip1.Name = "statusStrip1";
            this.statusStrip1.Size = new System.Drawing.Size(945, 22);
            this.statusStrip1.TabIndex = 27;
            this.statusStrip1.Text = "statusStrip1";
            // 
            // txtAlignViewSearch
            // 
            this.txtAlignViewSearch.Location = new System.Drawing.Point(117, 38);
            this.txtAlignViewSearch.Name = "txtAlignViewSearch";
            this.txtAlignViewSearch.Size = new System.Drawing.Size(374, 20);
            this.txtAlignViewSearch.TabIndex = 16;
            this.txtAlignViewSearch.TextChanged += new System.EventHandler(this.txtAlignViewSearch_TextChanged);
            // 
            // btnRemoveSelected
            // 
            this.btnRemoveSelected.Location = new System.Drawing.Point(508, 200);
            this.btnRemoveSelected.Name = "btnRemoveSelected";
            this.btnRemoveSelected.Size = new System.Drawing.Size(33, 23);
            this.btnRemoveSelected.TabIndex = 21;
            this.btnRemoveSelected.Text = "<";
            this.btnRemoveSelected.UseVisualStyleBackColor = true;
            this.btnRemoveSelected.Click += new System.EventHandler(this.btnRemoveSelected_Click);
            // 
            // btnAddSelected
            // 
            this.btnAddSelected.Location = new System.Drawing.Point(508, 171);
            this.btnAddSelected.Name = "btnAddSelected";
            this.btnAddSelected.Size = new System.Drawing.Size(33, 23);
            this.btnAddSelected.TabIndex = 19;
            this.btnAddSelected.Text = ">";
            this.btnAddSelected.UseVisualStyleBackColor = true;
            this.btnAddSelected.Click += new System.EventHandler(this.btnAddSelected_Click);
            // 
            // rtbLog
            // 
            this.rtbLog.Location = new System.Drawing.Point(117, 381);
            this.rtbLog.Name = "rtbLog";
            this.rtbLog.ReadOnly = true;
            this.rtbLog.Size = new System.Drawing.Size(817, 120);
            this.rtbLog.TabIndex = 24;
            this.rtbLog.Text = "";
            // 
            // btnApply
            // 
            this.btnApply.Location = new System.Drawing.Point(778, 507);
            this.btnApply.Name = "btnApply";
            this.btnApply.Size = new System.Drawing.Size(75, 23);
            this.btnApply.TabIndex = 25;
            this.btnApply.Text = "Apply";
            this.btnApply.UseVisualStyleBackColor = true;
            this.btnApply.Click += new System.EventHandler(this.btnApply_Click);
            // 
            // btnClose
            // 
            this.btnClose.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnClose.Location = new System.Drawing.Point(859, 507);
            this.btnClose.Name = "btnClose";
            this.btnClose.Size = new System.Drawing.Size(75, 23);
            this.btnClose.TabIndex = 26;
            this.btnClose.Text = "Close";
            this.btnClose.UseVisualStyleBackColor = true;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(38, 351);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(70, 13);
            this.label3.TabIndex = 22;
            this.label3.Text = "Align Position";
            // 
            // cboAlignPosition
            // 
            this.cboAlignPosition.FormattingEnabled = true;
            this.cboAlignPosition.Location = new System.Drawing.Point(117, 345);
            this.cboAlignPosition.Name = "cboAlignPosition";
            this.cboAlignPosition.Size = new System.Drawing.Size(374, 21);
            this.cboAlignPosition.TabIndex = 23;
            // 
            // lbSelectedAlignedView
            // 
            this.lbSelectedAlignedView.FormattingEnabled = true;
            this.lbSelectedAlignedView.HorizontalScrollbar = true;
            this.lbSelectedAlignedView.Location = new System.Drawing.Point(559, 64);
            this.lbSelectedAlignedView.Name = "lbSelectedAlignedView";
            this.lbSelectedAlignedView.SelectionMode = System.Windows.Forms.SelectionMode.MultiExtended;
            this.lbSelectedAlignedView.Size = new System.Drawing.Size(374, 264);
            this.lbSelectedAlignedView.TabIndex = 20;
            this.lbSelectedAlignedView.MouseDoubleClick += new System.Windows.Forms.MouseEventHandler(this.lbSelectedAlignedView_MouseDoubleClick);
            // 
            // lbAlignedView
            // 
            this.lbAlignedView.FormattingEnabled = true;
            this.lbAlignedView.HorizontalScrollbar = true;
            this.lbAlignedView.Location = new System.Drawing.Point(117, 64);
            this.lbAlignedView.Name = "lbAlignedView";
            this.lbAlignedView.SelectionMode = System.Windows.Forms.SelectionMode.MultiExtended;
            this.lbAlignedView.Size = new System.Drawing.Size(374, 264);
            this.lbAlignedView.TabIndex = 18;
            this.lbAlignedView.MouseDoubleClick += new System.Windows.Forms.MouseEventHandler(this.lbAlignedView_MouseDoubleClick);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(12, 192);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(99, 13);
            this.label2.TabIndex = 17;
            this.label2.Text = "Views to be aligned";
            // 
            // cboPrimaryView
            // 
            this.cboPrimaryView.FormattingEnabled = true;
            this.cboPrimaryView.Location = new System.Drawing.Point(117, 6);
            this.cboPrimaryView.Name = "cboPrimaryView";
            this.cboPrimaryView.Size = new System.Drawing.Size(374, 21);
            this.cboPrimaryView.TabIndex = 15;
            this.cboPrimaryView.SelectedIndexChanged += new System.EventHandler(this.cboPrimaryView_SelectedIndexChanged);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(43, 11);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(67, 13);
            this.label1.TabIndex = 14;
            this.label1.Text = "Primary View";
            // 
            // AlignViewForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(945, 570);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.statusStrip1);
            this.Controls.Add(this.txtAlignViewSearch);
            this.Controls.Add(this.btnRemoveSelected);
            this.Controls.Add(this.btnAddSelected);
            this.Controls.Add(this.rtbLog);
            this.Controls.Add(this.btnApply);
            this.Controls.Add(this.btnClose);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.cboAlignPosition);
            this.Controls.Add(this.lbSelectedAlignedView);
            this.Controls.Add(this.lbAlignedView);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.cboPrimaryView);
            this.Controls.Add(this.label1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            this.MaximizeBox = false;
            this.Name = "AlignViewForm";
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Align View";
            this.Load += new System.EventHandler(this.AlignViewForm_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.StatusStrip statusStrip1;
        private System.Windows.Forms.TextBox txtAlignViewSearch;
        private System.Windows.Forms.Button btnRemoveSelected;
        private System.Windows.Forms.Button btnAddSelected;
        private System.Windows.Forms.RichTextBox rtbLog;
        private System.Windows.Forms.Button btnApply;
        private System.Windows.Forms.Button btnClose;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.ComboBox cboAlignPosition;
        private System.Windows.Forms.ListBox lbSelectedAlignedView;
        private System.Windows.Forms.ListBox lbAlignedView;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.ComboBox cboPrimaryView;
        private System.Windows.Forms.Label label1;
    }
}