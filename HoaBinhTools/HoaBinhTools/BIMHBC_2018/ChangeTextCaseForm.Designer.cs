namespace ChangeTextCase
{
    partial class ChangeTextCaseForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ChangeTextCaseForm));
            this.btnUpper = new System.Windows.Forms.Button();
            this.btnLower = new System.Windows.Forms.Button();
            this.btnCapotalize = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // btnUpper
            // 
            this.btnUpper.BackColor = System.Drawing.SystemColors.Control;
            this.btnUpper.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.btnUpper.Location = new System.Drawing.Point(30, 20);
            this.btnUpper.Name = "btnUpper";
            this.btnUpper.Size = new System.Drawing.Size(155, 23);
            this.btnUpper.TabIndex = 0;
            this.btnUpper.Text = "To Upper";
            this.btnUpper.UseVisualStyleBackColor = true;
            this.btnUpper.Click += new System.EventHandler(this.btnUpper_Click);
            // 
            // btnLower
            // 
            this.btnLower.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.btnLower.Location = new System.Drawing.Point(30, 59);
            this.btnLower.Name = "btnLower";
            this.btnLower.Size = new System.Drawing.Size(155, 23);
            this.btnLower.TabIndex = 1;
            this.btnLower.Text = "To Lower";
            this.btnLower.UseVisualStyleBackColor = true;
            this.btnLower.Click += new System.EventHandler(this.btnLower_Click_1);
            // 
            // btnCapotalize
            // 
            this.btnCapotalize.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.btnCapotalize.Location = new System.Drawing.Point(30, 98);
            this.btnCapotalize.Name = "btnCapotalize";
            this.btnCapotalize.Size = new System.Drawing.Size(155, 23);
            this.btnCapotalize.TabIndex = 2;
            this.btnCapotalize.Text = "To Capotalize";
            this.btnCapotalize.UseVisualStyleBackColor = true;
            this.btnCapotalize.Click += new System.EventHandler(this.btnCapotalize_Click);
            // 
            // ChangeTextCaseForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(223, 137);
            this.Controls.Add(this.btnCapotalize);
            this.Controls.Add(this.btnLower);
            this.Controls.Add(this.btnUpper);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "ChangeTextCaseForm";
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "ChangeTextCaseForm";
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button btnUpper;
        private System.Windows.Forms.Button btnLower;
        private System.Windows.Forms.Button btnCapotalize;
    }
}