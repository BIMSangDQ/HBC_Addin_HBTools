using System;
using System.Windows.Forms;

namespace CreateRibbonTab
{
	public partial class BeamLevelNumberForm : Form
	{
		private bool mButtonVal;

		public bool ButtonVal
		{
			get { return this.mButtonVal; }
		}


		private String mLevelText;

		public String LevelText
		{
			get { return this.mLevelText; }
		}
		public BeamLevelNumberForm()
		{
			InitializeComponent();
		}

		private void BeamLevelNumberForm_Load(object sender, EventArgs e)
		{
			this.mButtonVal = false;
			this.mLevelText = "";
		}

		private void btnOK_Click(object sender, EventArgs e)
		{
			this.mLevelText = this.txtLevel.Text.ToUpper().Replace(" ", "");
			this.mButtonVal = true;
		}

		private void btnCancel_Click(object sender, EventArgs e)
		{
			this.mLevelText = "";
			this.mButtonVal = false;
		}
	}
}
