using System.Windows;

namespace HoaBinhTools.AutocadToRevit.Wall.Views
{
	/// <summary>
	/// Interaction logic for WallView.xaml
	/// </summary>
	public partial class WallView : Window
	{
		public WallView(object ob)
		{
			DataContext = ob;
			InitializeComponent();
		}
	}
}
