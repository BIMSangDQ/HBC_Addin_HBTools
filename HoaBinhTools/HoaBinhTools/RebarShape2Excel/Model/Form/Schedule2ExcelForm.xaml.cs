using System.Windows;
using SingleData;

namespace Model.Form
{
	/// <summary>
	/// Interaction logic for Schedule2ExcelForm.xaml
	/// </summary>
	public partial class Schedule2ExcelForm : Window
	{
		public Schedule2ExcelForm()
		{
			InitializeComponent();

			var wpfData = WPFData.Instance;
			DataContext = wpfData;
			wpfData.ViewSchedulesListView = availableScheduleListView;
			wpfData.ExportViewSchedulesListView = exportScheduleListView;
		}

		private void Button_Click(object sender, RoutedEventArgs e)
		{

		}

		private void Button_Click_1(object sender, RoutedEventArgs e)
		{

		}
	}
}
