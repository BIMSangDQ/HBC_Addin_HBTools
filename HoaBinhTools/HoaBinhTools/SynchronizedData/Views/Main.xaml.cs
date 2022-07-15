using System.Windows;
using System.Windows.Data;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HoaBinhTools.SynchronizedData.Views
{
	/// <summary>
	/// Interaction logic for Window1.xaml
	/// </summary>
	public partial class Main : Window
	{
		public Main(object ob)
		{
			DataContext = ob;
			InitializeComponent();
		}
	}
}
