#region Using
using Model.Form;
using Utility;
using Controls = System.Windows.Controls;
#endregion

namespace Model.Control
{
	public class ProgressBarControl : NotifyClass
	{
		private string status;
		private string ratio;
		public string Status
		{
			get
			{
				return "Status: " + status;
			}
			set
			{
				status = value;
				OnPropertyChanged();
			}
		}
		public string Ratio
		{
			get
			{
				return ratio;
			}
			set
			{
				ratio = value;
				OnPropertyChanged();
			}
		}
	}

	public class ProgressBarInstance : NotifyClass
	{
		private ProgressBarControl progressBarControl;
		private Controls.ProgressBar progressBar;
		public ProgressBarView ProgressBarView { get; set; }

		public int Total { get; set; }
		public int Value { get; set; } = 0;
		public StepType StepType { get; set; }
		public ProgressBarControl ProgressBarControl
		{
			get { return progressBarControl; }
			set
			{
				progressBarControl = value;
				OnPropertyChanged();
			}
		}
		public Controls.ProgressBar ProgressBar
		{
			get { return progressBar; }
			set
			{
				progressBar = value;
				OnPropertyChanged();
			}
		}

		public ProgressBarInstance(string status, int total)
		{
			ProgressBarControl = new ProgressBarControl() { Status = status };
			Total = total;
			StepType = GetStepType(Total);
			ProgressBarView = new ProgressBarView { DataContext = ProgressBarControl }; ProgressBarView.Show();
			ProgressBar = (Controls.ProgressBar)ProgressBarView.FindName("ProgressBar");
		}
		public void Step()
		{
			Value += 1;
			if (Value == Total)
			{
				ProgressBarView.Close();
			}

			if (StepType != StepType.By1 && Value % (int)StepType != 0) return;

			ProgressBar.Dispatcher.Invoke(() => ProgressBar.Value = Value / ((double)Total / 100), System.Windows.Threading.DispatcherPriority.Background);
			ProgressBarControl.Ratio = $"{Value}/{Total}";

			//Value++;
		}
		public StepType GetStepType(int count)
		{
			if (count < 100)
			{
				return StepType.By1;
			}
			if (count < 250)
			{
				return StepType.By2;
			}
			if (count < 500)
			{
				return StepType.By5;
			}
			if (count < 1000)
			{
				return StepType.By1;
			}
			if (count < 10000)
			{
				return StepType.By10;
			}
			if (count < 100000)
			{
				return StepType.By100;
			}
			if (count < 1000000)
			{
				return StepType.By1000;
			}
			if (count < 10000000)
			{
				return StepType.By10000;
			}
			return StepType.By100000;
		}
	}
	public enum StepType
	{
		By1 = 1,
		By2 = 2,
		By5 = 5,
		By10 = 10,
		By100 = 100,
		By1000 = 1000,
		By10000 = 10000,
		By100000 = 100000
	}
}