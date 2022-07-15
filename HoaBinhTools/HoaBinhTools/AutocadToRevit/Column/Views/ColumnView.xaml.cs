using System.Windows;

namespace HoaBinhTools.AutocadToRevit.Column.Views
{
	/// <summary>
	/// Interaction logic for ColumnView.xaml
	/// </summary>
	public partial class ColumnView : Window
	{
		public ColumnView(object ob)
		{
			DataContext = ob;
			InitializeComponent();
		}
	}
}
