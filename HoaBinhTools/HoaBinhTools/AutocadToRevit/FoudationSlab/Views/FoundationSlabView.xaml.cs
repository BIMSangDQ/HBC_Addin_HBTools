using System.Windows;

namespace HoaBinhTools.AutocadToRevit.FoudationSlab.Views
{
	/// <summary>
	/// Interaction logic for FoundationSlabView.xaml
	/// </summary>
	public partial class FoundationSlabView : Window
	{
		public FoundationSlabView(object ob)
		{
			DataContext = ob;
			InitializeComponent();
		}

	}
}
