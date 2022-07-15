using System.Windows.Forms;

namespace SingleData
{
	public class FormData
	{
		public static FormData Instance { get { return Singleton.Instance.FormData; } }

		private SaveFileDialog saveFileDialog;
		public SaveFileDialog SaveFileDialog
		{
			get
			{
				if (saveFileDialog == null)
				{
					saveFileDialog = new SaveFileDialog();
				}
				return saveFileDialog;
			}
		}
	}
}
