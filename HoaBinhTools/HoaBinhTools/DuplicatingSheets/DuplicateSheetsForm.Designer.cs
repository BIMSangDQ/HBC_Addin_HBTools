namespace DuplicatingSheets
{
	public partial class DuplicateSheetsForm : global::System.Windows.Forms.Form
	{
		protected override void Dispose(bool disposing)
		{
			bool flag = disposing && this.components != null;
			if (flag)
			{
				this.components.Dispose();
			}
			base.Dispose(disposing);
		}

		private void InitializeComponent()
		{
			global::System.ComponentModel.ComponentResourceManager componentResourceManager = new global::System.ComponentModel.ComponentResourceManager(typeof(global::DuplicatingSheets.DuplicateSheetsForm));
			this.nudQuantity = new global::System.Windows.Forms.NumericUpDown();
			this.label1 = new global::System.Windows.Forms.Label();
			this.chbViews = new global::System.Windows.Forms.CheckBox();
			this.chbLegends = new global::System.Windows.Forms.CheckBox();
			this.chbSchedules = new global::System.Windows.Forms.CheckBox();
			this.btnOK = new global::System.Windows.Forms.Button();
			this.btnCancel = new global::System.Windows.Forms.Button();
			this.cmbDuplicate = new global::System.Windows.Forms.ComboBox();
			this.label2 = new global::System.Windows.Forms.Label();
			((global::System.ComponentModel.ISupportInitialize)this.nudQuantity).BeginInit();
			base.SuspendLayout();
			this.nudQuantity.Location = new global::System.Drawing.Point(137, 27);
			this.nudQuantity.Margin = new global::System.Windows.Forms.Padding(4);
			global::System.Windows.Forms.NumericUpDown numericUpDown = this.nudQuantity;
			int[] array = new int[4];
			array[0] = 1;
			numericUpDown.Minimum = new decimal(array);
			this.nudQuantity.Name = "nudQuantity";
			this.nudQuantity.Size = new global::System.Drawing.Size(160, 22);
			this.nudQuantity.TabIndex = 1;
			global::System.Windows.Forms.NumericUpDown numericUpDown2 = this.nudQuantity;
			int[] array2 = new int[4];
			array2[0] = 1;
			numericUpDown2.Value = new decimal(array2);
			this.label1.AutoSize = true;
			this.label1.Location = new global::System.Drawing.Point(32, 29);
			this.label1.Margin = new global::System.Windows.Forms.Padding(4, 0, 4, 0);
			this.label1.Name = "label1";
			this.label1.Size = new global::System.Drawing.Size(61, 17);
			this.label1.TabIndex = 2;
			this.label1.Text = "Quantity";
			this.chbViews.AutoSize = true;
			this.chbViews.Checked = true;
			this.chbViews.CheckState = global::System.Windows.Forms.CheckState.Checked;
			this.chbViews.Location = new global::System.Drawing.Point(67, 73);
			this.chbViews.Margin = new global::System.Windows.Forms.Padding(4);
			this.chbViews.Name = "chbViews";
			this.chbViews.Size = new global::System.Drawing.Size(66, 21);
			this.chbViews.TabIndex = 3;
			this.chbViews.Text = "Views";
			this.chbViews.UseVisualStyleBackColor = true;
			this.chbLegends.AutoSize = true;
			this.chbLegends.Checked = true;
			this.chbLegends.CheckState = global::System.Windows.Forms.CheckState.Checked;
			this.chbLegends.Location = new global::System.Drawing.Point(67, 116);
			this.chbLegends.Margin = new global::System.Windows.Forms.Padding(4);
			this.chbLegends.Name = "chbLegends";
			this.chbLegends.Size = new global::System.Drawing.Size(85, 21);
			this.chbLegends.TabIndex = 4;
			this.chbLegends.Text = "Legends";
			this.chbLegends.UseVisualStyleBackColor = true;
			this.chbSchedules.AutoSize = true;
			this.chbSchedules.Checked = true;
			this.chbSchedules.CheckState = global::System.Windows.Forms.CheckState.Checked;
			this.chbSchedules.Location = new global::System.Drawing.Point(67, 159);
			this.chbSchedules.Margin = new global::System.Windows.Forms.Padding(4);
			this.chbSchedules.Name = "chbSchedules";
			this.chbSchedules.Size = new global::System.Drawing.Size(96, 21);
			this.chbSchedules.TabIndex = 5;
			this.chbSchedules.Text = "Schedules";
			this.chbSchedules.UseVisualStyleBackColor = true;
			this.btnOK.DialogResult = global::System.Windows.Forms.DialogResult.OK;
			this.btnOK.Location = new global::System.Drawing.Point(52, 223);
			this.btnOK.Margin = new global::System.Windows.Forms.Padding(4);
			this.btnOK.Name = "btnOK";
			this.btnOK.Size = new global::System.Drawing.Size(100, 28);
			this.btnOK.TabIndex = 6;
			this.btnOK.Text = "OK";
			this.btnOK.UseVisualStyleBackColor = true;
			this.btnOK.Click += new global::System.EventHandler(this.btnOK_Click);
			this.btnCancel.DialogResult = global::System.Windows.Forms.DialogResult.Cancel;
			this.btnCancel.Location = new global::System.Drawing.Point(232, 223);
			this.btnCancel.Margin = new global::System.Windows.Forms.Padding(4);
			this.btnCancel.Name = "btnCancel";
			this.btnCancel.Size = new global::System.Drawing.Size(100, 28);
			this.btnCancel.TabIndex = 7;
			this.btnCancel.Text = "Cancel";
			this.btnCancel.UseVisualStyleBackColor = true;
			this.cmbDuplicate.FormattingEnabled = true;
			this.cmbDuplicate.Items.AddRange(new object[]
			{
				"Duplicate",
				"with Detailing",
				"as Dependent"
			});
			this.cmbDuplicate.Location = new global::System.Drawing.Point(232, 73);
			this.cmbDuplicate.Name = "cmbDuplicate";
			this.cmbDuplicate.Size = new global::System.Drawing.Size(131, 24);
			this.cmbDuplicate.TabIndex = 9;
			this.cmbDuplicate.Text = "with Detailing";
			this.label2.AutoSize = true;
			this.label2.Location = new global::System.Drawing.Point(158, 76);
			this.label2.Margin = new global::System.Windows.Forms.Padding(4, 0, 4, 0);
			this.label2.Name = "label2";
			this.label2.Size = new global::System.Drawing.Size(67, 17);
			this.label2.TabIndex = 10;
			this.label2.Text = "Duplicate";
			base.AcceptButton = this.btnOK;
			base.AutoScaleDimensions = new global::System.Drawing.SizeF(8f, 16f);
			base.AutoScaleMode = global::System.Windows.Forms.AutoScaleMode.Font;
			base.CancelButton = this.btnCancel;
			base.ClientSize = new global::System.Drawing.Size(392, 308);
			base.Controls.Add(this.label2);
			base.Controls.Add(this.cmbDuplicate);
			base.Controls.Add(this.btnCancel);
			base.Controls.Add(this.btnOK);
			base.Controls.Add(this.chbSchedules);
			base.Controls.Add(this.chbLegends);
			base.Controls.Add(this.chbViews);
			base.Controls.Add(this.label1);
			base.Controls.Add(this.nudQuantity);
			base.FormBorderStyle = global::System.Windows.Forms.FormBorderStyle.FixedToolWindow;
			//base.Icon = (global::System.Drawing.Icon)componentResourceManager.GetObject("$this.Icon");
			base.Margin = new global::System.Windows.Forms.Padding(4);
			base.Name = "DuplicateSheetsForm";
			base.ShowInTaskbar = false;
			base.StartPosition = global::System.Windows.Forms.FormStartPosition.CenterParent;
			this.Text = "DuplicateSheets";
			((global::System.ComponentModel.ISupportInitialize)this.nudQuantity).EndInit();
			base.ResumeLayout(false);
			base.PerformLayout();
		}

		// Token: 0x04000005 RID: 5
		private global::System.ComponentModel.IContainer components = null;

		// Token: 0x04000006 RID: 6
		private global::System.Windows.Forms.NumericUpDown nudQuantity;

		// Token: 0x04000007 RID: 7
		private global::System.Windows.Forms.Label label1;

		// Token: 0x04000008 RID: 8
		private global::System.Windows.Forms.CheckBox chbViews;

		// Token: 0x04000009 RID: 9
		private global::System.Windows.Forms.CheckBox chbLegends;

		// Token: 0x0400000A RID: 10
		private global::System.Windows.Forms.CheckBox chbSchedules;

		// Token: 0x0400000B RID: 11
		private global::System.Windows.Forms.Button btnOK;

		// Token: 0x0400000C RID: 12
		private global::System.Windows.Forms.Button btnCancel;

		// Token: 0x0400000D RID: 13
		private global::System.Windows.Forms.ComboBox cmbDuplicate;

		// Token: 0x0400000E RID: 14
		private global::System.Windows.Forms.Label label2;
	}
}
