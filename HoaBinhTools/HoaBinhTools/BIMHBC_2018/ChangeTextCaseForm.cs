using System;
using System.Windows.Forms;
using CreateRibbonTab;

namespace ChangeTextCase
{
	public partial class ChangeTextCaseForm : Form
	{
		private TextCase mCase;

		public TextCase Case
		{
			get { return mCase; }
		}

		public ChangeTextCaseForm()
		{
			InitializeComponent();
		}

		private void ChangeTextCaseForm_Load(object sender, EventArgs e)
		{
			this.mCase = TextCase.NOTHING;
		}

		private void btnUpper_Click(object sender, EventArgs e)
		{
			mCase = TextCase.UPPER;
			this.Close();
		}

		private void btnCapotalize_Click(object sender, EventArgs e)
		{
			mCase = TextCase.CAPITALIZE;
			this.Close();
		}

		private void btnLower_Click_1(object sender, EventArgs e)
		{
			mCase = TextCase.LOWER;
			this.Close();
		}
	}
}
