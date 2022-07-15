using System;
using System.Windows.Forms;

namespace CreateRibbonTab
{
	public partial class AutoSheetNumberForm : Form
	{
		private String mSheetNumber;

		public String SheetNumber
		{
			set { this.mSheetNumber = value; }
		}

		private String mPrefixSheet;
		public String PrefixSheet
		{
			get { return mPrefixSheet; }
		}

		private int mPostNumber;

		public int PostNumber
		{
			get { return mPostNumber; }
		}

		public AutoSheetNumberForm()
		{
			InitializeComponent();
		}

		private void AutoSheetNumberForm_Load(object sender, EventArgs e)
		{
			this.txtSheetNumber.Text = this.mSheetNumber;
		}

		private void BtnOK_Click(object sender, EventArgs e)
		{
			String temp = this.txtSheetNumber.Text;
			String character = this.txtCharacter.Text;

			int pos = temp.LastIndexOf(character);
			//int pos = temp.LastIndexOf(".");

			//if (pos == -1)
			//{
			//    pos = temp.LastIndexOf("-");
			//}

			String _number = temp.Substring(pos + 1);

			if (!Int32.TryParse(_number, out this.mPostNumber))
			{
				this.mPostNumber = -1;
				this.mPrefixSheet = "";
			}
			else
			{

				this.mPrefixSheet = temp.Substring(0, pos + 1);
			}

		}
		public override String ToString()
		{
			return this.mPrefixSheet + this.mPostNumber;
		}
	}
}
