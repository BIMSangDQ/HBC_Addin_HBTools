using System.Windows;
using System.Windows.Input;

namespace HoaBinhTools.PurgeViewsV2
{
	/// <summary>
	/// Interaction logic for PurgeViews.xaml
	/// </summary>
	public partial class PurgeViews : Window
	{
		public PurgeViews(object vm)
		{
			this.DataContext = vm;

			InitializeComponent();
		}

		private void MainTreeView_MouseDoubleClick(object sender, MouseButtonEventArgs e)
		{


		}


	}
}
