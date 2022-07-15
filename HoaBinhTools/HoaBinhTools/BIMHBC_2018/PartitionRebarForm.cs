using System;
using System.Windows.Forms;

namespace CreateRibbonTab
{
	public partial class PartitionRebarForm : Form
	{
		private String mBeamName;

		public String BeamName
		{
			set { this.mBeamName = value; }

			get { return this.mBeamName; }
		}

		public PartitionRebarForm()
		{
			InitializeComponent();
		}

		private void PartitionRebarForm_Load(object sender, EventArgs e)
		{
			this.txtPartition.Text = this.mBeamName;
		}

		private void btnOK_Click(object sender, EventArgs e)
		{
			if (this.txtPartition.Text != "")
				this.mBeamName = this.txtPartition.Text;
		}
	}
}
