using System;
using System.Windows;
using System.Windows.Threading;

namespace HoaBinhTools.BIMQAQC.ModelChecker.Views.ProgessBar
{
	public partial class ProgressBarViewQAQC : Window
	{
		public ProgressBarViewQAQC()
		{
			InitializeComponent();
		}
		private delegate void ProgressBarDelegate();
		private bool flag = true;

		public bool Create(int max, int max2, string name, string Status, bool isNewProcess = false)
		{
			if (isNewProcess)
			{
				pb.Minimum = 0;
			}
			pb2.Minimum = 0;

			pb.Maximum = max;
			Title = "Loading... " + name + " " + Convert.ToInt32(pb.Value) + "/" + pb.Maximum; ;
			TbPercent.Text = Math.Round(pb.Value * 100 / pb.Maximum, 1) + "%";
			

			pb2.Maximum = max2;
			TbPercent2.Text = Status;
			pb2.Value = 0;
			pb2.Dispatcher.Invoke(new ProgressBarDelegate(UpdateProgress2), DispatcherPriority.Background);
			return flag;
		}

		public bool Create1(bool isNewProcess = false)
		{
			pb.Dispatcher.Invoke(new ProgressBarDelegate(UpdateProgress), DispatcherPriority.Background);
			return flag;
		}

		public bool Create2(string Status, bool isNewProcess = false)
		{
			TbPercent2.Text = Status;
			pb2.Dispatcher.Invoke(new ProgressBarDelegate(UpdateProgress2), DispatcherPriority.Background);
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

		private void UpdateProgress2()
		{
			pb2.Value++;
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
