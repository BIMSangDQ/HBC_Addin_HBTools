#region Using
using BeyCons.Core.Extensions;
using BeyCons.Core.FormUtils.ControlViews;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Forms;
using System.Windows.Input;
#endregion

namespace BeyCons.Core.FormUtils.Events
{
    public class VisibleView : BaseView
    {
        private static VisibleView instance;
        public static VisibleView Instance
        {
            get
            {
                if (instance == null) instance = new VisibleView();
                return instance;
            }
            set
            {
                instance = value;
                StaticOnPropertyChanged();
            }
        }
        public ICommand CommandShowForm
        {
            get
            {
                return new RelayCommand<Window>((w) => { return true; }, (w) =>
                {
                    Autodesk.Windows.RibbonControl ribbonControl = Autodesk.Windows.ComponentManager.Ribbon;
                    foreach (Autodesk.Windows.RibbonTab ribbonTab in ribbonControl.Tabs)
                    {
                        if (Regex.Replace(ribbonTab.Title, @"[0-9.\s]", string.Empty) == Regex.Replace(RibbonName.RibbonTab, @"\s", string.Empty))
                        {
                            foreach (Autodesk.Windows.RibbonPanel ribbonPanel in ribbonTab.Panels)
                            {
                                ribbonPanel.IsEnabled = false;
                            }
                            break;
                        }
                    }
                });
            }
        }
        public ICommand CommandCloseForm
        {
            get
            {
                return new RelayCommand<Window>((w) => { return true; }, (w) =>
                {
                    Autodesk.Windows.RibbonControl ribbonControl = Autodesk.Windows.ComponentManager.Ribbon;
                    foreach (Autodesk.Windows.RibbonTab ribbonTab in ribbonControl.Tabs)
                    {
                        if (Regex.Replace(ribbonTab.Title, "[0-9.\\s]", string.Empty) == Regex.Replace(RibbonName.RibbonTab, @"\s", string.Empty))
                        {
                            foreach (Autodesk.Windows.RibbonPanel ribbonPanel in ribbonTab.Panels)
                            {
                                ribbonPanel.IsEnabled = true;
                            }
                            break;
                        }
                    }
                });
            }
        }
    }
}