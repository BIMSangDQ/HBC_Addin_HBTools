using System.Windows;

namespace HoaBinhTools.FramingInformation.Views
{
	/// <summary>
	/// Interaction logic for SettingGeoInfoRevit.xaml
	/// </summary>
	public partial class SettingGeoInfoRevit : Window
	{
		public SettingGeoInfoRevit(object obj)
		{

			this.DataContext = obj;

			InitializeComponent();

		}
	}
}
