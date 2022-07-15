using System.Windows;

namespace HoaBinhTools.QLUser.Views
{
	/// <summary>
	/// Interaction logic for User.xaml
	/// </summary>
	public partial class UserView : Window
	{
		public UserView(object ob)
		{
			DataContext = ob;
			InitializeComponent();
		}
	}
}
