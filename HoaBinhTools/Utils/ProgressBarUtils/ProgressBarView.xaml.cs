using System;
using System.Windows;
using System.Windows.Threading;

namespace Utils
{
	public partial class ProgressBarView : Window
	{
		public ProgressBarView()
		{
			InitializeComponent();
		}
		private delegate void ProgressBarDelegate();
		private bool flag = true;

		public bool Create(int max, string name, bool isNewProcess = false)
		{
			if (isNewProcess)
			{
				pb.Minimum = 0;
			}
			pb.Maximum = max;
			Title = "Loading... " + name + " " + Convert.ToInt32(pb.Value) + "/" + pb.Maximum; ;
			TbPercent.Text = Math.Round(pb.Value * 100 / pb.Maximum, 1) + "%";
			pb.Dispatcher.Invoke(new ProgressBarDelegate(UpdateProgress), DispatcherPriority.Background);
			return flag;
		}




		private void ButtonBase_OnClick(object sender, RoutedEventArgs e)
		{
			flag = false;
		}

		private void UpdateProgress()
		{
			pb.Value++;
		}
		private void ProgressBarView_OnClosed(object sender, EventArgs e)
		{
			flag = false;
		}

		private void BtClose_OnClick(object sender, RoutedEventArgs e)
		{
			flag = false;
		}
	}
}
