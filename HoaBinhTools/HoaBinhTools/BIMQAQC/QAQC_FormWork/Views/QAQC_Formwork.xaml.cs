using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace HoaBinhTools.BIMQAQC.QAQC_FormWork.Views
{
	/// <summary>
	/// Interaction logic for QAQC_Formwork.xaml
	/// </summary>
	public partial class QAQC_Formwork : Window
	{
		public QAQC_Formwork(Object ob)
		{
			DataContext = ob;
			InitializeComponent();
		}
	}
}
