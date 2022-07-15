using System.Windows;

namespace HoaBinhTools.ProjectWarnings.Views
{
	/// <summary>
	/// Interaction logic for ViewWarning.xaml
	/// </summary>
	public partial class ViewWarning : Window
	{
		public ViewWarning(object obj)
		{
			this.DataContext = obj;

			InitializeComponent();
		}
	}
}
