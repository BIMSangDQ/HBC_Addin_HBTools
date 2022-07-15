#region Using
using BeyCons.Core.FormUtils.Views;
using System.Windows;
using System.Windows.Input;
#endregion

namespace BeyCons.Core.FormUtils.ControlViews
{
    public class NotificationControl : BaseView
    {
        private string content = string.Empty;

        public ICommand CloseCommand
        {
            get
            {
                return new RelayCommand<Window>((p) => { return true; }, (p) => { p.Close(); NotifyResult = NotifyResult.Close; });
            }
        }
        public ICommand YesCommand
        {
            get
            {
                return new RelayCommand<Window>((p) => { return true; }, (p) => { p.Close(); NotifyResult = NotifyResult.Yes; });
            }
        }
        public ICommand NoCommand
        {
            get
            {
                return new RelayCommand<Window>((p) => { return true; }, (p) => { p.Close(); NotifyResult = NotifyResult.No; });
            }
        }
        public ICommand ESCCommand
        {
            get
            {
                return new RelayCommand<Window>((p) => { return true; }, (p) => { p.Close(); NotifyResult = NotifyResult.Close; });
            }
        }
        public NotifyResult NotifyResult { get; set; } = NotifyResult.Close;
        public Visibility VisibilityClose { get; set; } = Visibility.Visible;
        public Visibility VisibilityYesNo { get; set; } = Visibility.Visible;

        public string Content
        {
            get { return content; }
            set
            {
                content = value;
                OnPropertyChanged();
            }
        }
    }

    public static class Notification
    {
        public static NotifyResult ShowDialog(string content, bool isYesNo = false)
        {
            NotificationControl notificationControl = new NotificationControl { Content = content };
            if (isYesNo)
            {
                notificationControl.VisibilityClose = Visibility.Collapsed;
            }
            else
            {
                notificationControl.VisibilityYesNo = Visibility.Collapsed;
            }
            NotificationView notificationView = new NotificationView() { DataContext = notificationControl };
            notificationView.ShowDialog();
            return notificationControl.NotifyResult;
        }
    }

    public enum NotifyResult
    {
        Yes,
        No,
        Close
    }
}