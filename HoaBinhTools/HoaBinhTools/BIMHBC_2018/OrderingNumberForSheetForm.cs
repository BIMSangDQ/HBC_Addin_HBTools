using System;
using System.Windows.Forms;

namespace CreateRibbonTab
{

	public partial class OrderingNumberForSheetForm : Form
	{
		private int mStartNumber;

		public int StartNumber
		{
			get
			{
				return this.mStartNumber;
			}
		}
		public OrderingNumberForSheetForm()
		{
			InitializeComponent();
		}

		private void OrderingNumberForSheetForm_Load(object sender, EventArgs e)
		{

		}

		private void btnOK_Click(object sender, EventArgs e)
		{
			int n = 0;
			bool b = Int32.TryParse(this.txtStartNumber.Text, out n);
			if (b)
			{
				this.mStartNumber = n;
			}
			else
			{
				this.mStartNumber = 0;
			}
		}
	}
}
