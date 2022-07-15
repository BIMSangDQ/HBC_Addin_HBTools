using System.Windows;

namespace HoaBinhTools.AutocadToRevit.Beam.Views
{
	/// <summary>
	/// Interaction logic for BeamView.xaml
	/// </summary>
	public partial class BeamView : Window
	{
		public BeamView(object ob)
		{
			DataContext = ob;
			InitializeComponent();
		}

		private void SetName_Checked(object sender, RoutedEventArgs e)
		{

		}
	}
}
