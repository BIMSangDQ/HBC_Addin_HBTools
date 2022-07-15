using System.Windows;
using System.Windows.Controls;
using SingleData;

namespace Property.WPF
{
	public partial class BaseAttachedProperty
	{
		public static readonly DependencyProperty BrowseFileClickedProperty = DependencyProperty.RegisterAttached(
			"BrowseFileClicked", typeof(string), typeof(BaseAttachedProperty), new PropertyMetadata(null, new PropertyChangedCallback(OnBrowseFileClickedPropertyChanged)));

		public static string GetBrowseFileClickedProperty(DependencyObject obj)
		{
			return (string)obj.GetValue(BrowseFileClickedProperty);
		}
		public static void SetBrowseFileClickedProperty(DependencyObject obj, string value)
		{
			obj.SetValue(BrowseFileClickedProperty, value);
		}
		private static void OnBrowseFileClickedPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			Button btn = d as Button;
			if (btn == null) return;

			btn.Click -= BrowseFileClicked;
			if ((string)e.NewValue != null)
			{
				btn.Click += BrowseFileClicked;
			}
		}

		private static void BrowseFileClicked(object sender, RoutedEventArgs e)
		{
			var wpfData = WPFData.Instance;

			var sfd = FormData.Instance.SaveFileDialog;
			sfd.Filter = "Excel Files (*.xlxs)|*.xlsx";

			var savePath = wpfData.SavePath;
			if (savePath != "")
				sfd.FileName = System.IO.Path.GetFileName(savePath);
			else
				sfd.FileName = Constant.ConstantValue.DEFAULT_EXCEL_EXPORT_FILENAME;

			if (sfd.ShowDialog() == System.Windows.Forms.DialogResult.OK)
			{
				wpfData.SavePath = sfd.FileName;
			}
		}
	}
}
