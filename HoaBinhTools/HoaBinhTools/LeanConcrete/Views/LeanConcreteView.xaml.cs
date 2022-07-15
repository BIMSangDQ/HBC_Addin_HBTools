using System.Windows;

namespace HoaBinhTools.LeanConcrete.Views
{
	/// <summary>
	/// Interaction logic for LeanConcreteView.xaml
	/// </summary>
	public partial class LeanConcreteView : Window
	{
		public LeanConcreteView(object ob)
		{
			DataContext = ob;
			InitializeComponent();
		}


	}
}
