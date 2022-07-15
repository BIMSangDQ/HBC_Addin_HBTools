#region Using
using System.Windows;
using System.Windows.Input;
#endregion

namespace BeyCons.Core.FormUtils.ControlViews
{
    public class GeneralControl
    {
        public ICommand CloseCommand
        {
            get
            {
                return new RelayCommand<Window>((p) => { return true; }, (p) => { p.Close(); });
            }
        }
        public string Copyright { get; set; } = "Copyright - BIM Department - HBC";
    }
}