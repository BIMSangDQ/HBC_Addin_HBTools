#region Using
using BeyCons.Core.FormUtils.Views;
using Controls = System.Windows.Controls;
#endregion

namespace BeyCons.Core.FormUtils.ControlViews
{
    public class ProgressBarControl : BaseView
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
    public class ProgressBarInstance : BaseView
    {
        private ProgressBarControl progressBarControl;
        private Controls.ProgressBar progressBar;
        public ProgressBarView ProgressBarView { get; set; }

        public double Total { get; set; }
        public double Value { get; set; } = 1;
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

        public ProgressBarInstance(string status, double total)
        {
            ProgressBarControl = new ProgressBarControl() { Status = status }; Total = total;
            ProgressBarView = new ProgressBarView { DataContext = ProgressBarControl }; ProgressBarView.Show();
            ProgressBar = (Controls.ProgressBar)ProgressBarView.FindName("ProgressBar");
        }
        public void Start()
        {
            if (Value <= 100)
            {
                ProgressBar.Dispatcher.Invoke(() => ProgressBar.Value = Value / (Total / 100), System.Windows.Threading.DispatcherPriority.Background);
                ProgressBarControl.Ratio = $"{Value}/{Total}";
            }
            else if (100 < Value && Value <= 500 && Value % 10 == 0)
            {
                ProgressBar.Dispatcher.Invoke(() => ProgressBar.Value = Value / (Total / 100), System.Windows.Threading.DispatcherPriority.Background);
                ProgressBarControl.Ratio = $"{Value}/{Total}";
            }
            else if (500 < Value && Value <= 1000 && Value % 25 == 0)
            {
                ProgressBar.Dispatcher.Invoke(() => ProgressBar.Value = Value / (Total / 100), System.Windows.Threading.DispatcherPriority.Background);
                ProgressBarControl.Ratio = $"{Value}/{Total}";
            }
            else if (1000 < Value && Value <= 4000 && Value % 100 == 0)
            {
                ProgressBar.Dispatcher.Invoke(() => ProgressBar.Value = Value / (Total / 100), System.Windows.Threading.DispatcherPriority.Background);
                ProgressBarControl.Ratio = $"{Value}/{Total}";
            }
            else if (Value > 4000 && Value % 200 == 0)
            {
                ProgressBar.Dispatcher.Invoke(() => ProgressBar.Value = Value / (Total / 100), System.Windows.Threading.DispatcherPriority.Background);
                ProgressBarControl.Ratio = $"{Value}/{Total}";
            }
            if (Value == Total)
            {
                ProgressBarView.Close();
            }
            Value++;
        }
    }
}
