#region Using
using Autodesk.Revit.DB;
using BeyCons.Core.Extensions;
using BeyCons.Core.FormUtils.ControlViews;
using BeyCons.Core.RevitUtils;
using BeyCons.Core.RevitUtils.AddinIdentity;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
#endregion

namespace BeyCons.Core.FormUtils.Reports
{
    public class ReportData : BaseView
    {
        private static ReportData instance;
        public static ReportData Instance
        {
            get
            {
                if (null == instance) instance = new ReportData();
                return instance;
            }
            set
            {
                instance = value;
                StaticOnPropertyChanged();
            }
        }

        #region Fields
        private ObservableCollection<UnionReport> unionReports;
        private ObservableCollection<VolumeReport> volumeReports;
        private ObservableCollection<WarningAndErrorReport> warningReports;
        private ObservableCollection<WarningAndErrorReport> reportErrors;
        private ObservableCollection<JoinReport> reportsJoins;
        #endregion

        #region Properties
        public ObservableCollection<UnionReport> UnionReports
        {
            get
            {
                if (unionReports == null) unionReports = new ObservableCollection<UnionReport>();
                return unionReports;
            }
            set
            {
                unionReports = value;
                OnPropertyChanged();
            }
        }

        public ObservableCollection<VolumeReport> VolumeReports
        {
            get
            {
                if (volumeReports == null) volumeReports = new ObservableCollection<VolumeReport>();
                return volumeReports;
            }
            set
            {
                volumeReports = value;
                OnPropertyChanged();
            }
        }

        public ObservableCollection<WarningAndErrorReport> WarningReports
        {
            get
            {
                if (warningReports == null) warningReports = new ObservableCollection<WarningAndErrorReport>();
                return warningReports;
            }
            set
            {
                warningReports = value;
                OnPropertyChanged();
            }
        }

        public ObservableCollection<WarningAndErrorReport> ErrorReports
        {
            get
            {
                if (reportErrors == null) reportErrors = new ObservableCollection<WarningAndErrorReport>();
                return reportErrors;
            }
            set
            {
                reportErrors = value;
                OnPropertyChanged();
            }
        }

        public ObservableCollection<JoinReport> JoinReports
        {
            get
            {
                if (reportsJoins == null) reportsJoins = new ObservableCollection<JoinReport>();
                return reportsJoins;
            }
            set
            {
                reportsJoins = value;
                OnPropertyChanged();
            }
        }
        #endregion

        #region Commands
        public ICommand WarningCommand
        {
            get
            {
                return new RelayCommand<ListView>((lv) => { return true; }, (lv) =>
                {
                    DoubleClick(lv.SelectedItem as WarningAndErrorReport, lv);
                });
            }
        }
        public ICommand ErrorCommand
        {
            get
            {
                return new RelayCommand<ListView>((lv) => { return true; }, (lv) =>
                {
                    DoubleClick(lv.SelectedItem as WarningAndErrorReport, lv);
                });
            }
        }
        public ICommand UnionCommand
        {
            get
            {
                return new RelayCommand<ListView>((lv) => { return true; }, (lv) =>
                {
                    DoubleClick(lv.SelectedItem as UnionReport, lv);
                });
            }
        }
        public ICommand VolumeCommand
        {
            get
            {
                return new RelayCommand<ListView>((lv) => { return true; }, (lv) =>
                {
                    DoubleClick(lv.SelectedItem as VolumeReport, lv);
                });
            }
        }
        public ICommand JoinCommand
        {
            get
            {
                return new RelayCommand<ListView>((lv) => { return true; }, (lv) =>
                {
                    DoubleClick(lv.SelectedItem as JoinReport, lv);
                });
            }
        }
        public ICommand SelectAllWarningCommand
        {
            get
            {
                return new RelayCommand<ListView>((lv) => { return true; }, (lv) =>
                {
                    SelectAll(lv.ItemsSource as ObservableCollection<WarningAndErrorReport>, lv);
                });
            }
        }
        public ICommand SelectAllErrorCommand
        {
            get
            {
                return new RelayCommand<ListView>((lv) => { return true; }, (lv) =>
                {
                    SelectAll(lv.ItemsSource as ObservableCollection<WarningAndErrorReport>, lv);
                });
            }
        }
        public ICommand SelectAllUnionCommand
        {
            get
            {
                return new RelayCommand<ListView>((lv) => { return true; }, (lv) =>
                {
                    SelectAll(lv.ItemsSource as ObservableCollection<UnionReport>, lv);
                });
            }
        }
        public ICommand SelectAllVolumeCommand
        {
            get
            {
                return new RelayCommand<ListView>((lv) => { return true; }, (lv) =>
                {
                    SelectAll(lv.ItemsSource as ObservableCollection<VolumeReport>, lv);
                });
            }
        }
        public ICommand SelectAllJoinCommand
        {
            get
            {
                return new RelayCommand<ListView>((lv) => { return true; }, (lv) =>
                {
                    SelectAll(lv.ItemsSource as ObservableCollection<JoinReport>, lv);
                });
            }
        }
        public ICommand ResetView
        {
            get
            {
                return new RelayCommand<Page>((p) => { return true; }, (p) => ResetModeIsoLineHide(p));
            }
        }
        #endregion

        #region Methods
        private void DoubleClick(WarningAndErrorReport report, ListView listView)
        {
            Window.GetWindow(listView).Hide();
            if (!RevitData.Instance.ActiveView.IsInTemporaryViewMode(TemporaryViewMode.TemporaryHideIsolate))
            {
                Action action = new Action(() =>
                {
                    RevitData.Instance.Transaction.Start("Isolate elements");
                    RevitData.Instance.ActiveView.IsolateElementsTemporary(report.ElementIds);
                    RevitData.Instance.Transaction.Commit();
                });
                RevitData.Instance.ExternalEventHandlerReport.SetAction(action);
                RevitData.Instance.ExternalEventReport.Raise();
            }
            else
            {
                NotifyUtils.ResetView();
            }
            Window.GetWindow(listView).Show();
        }
        private void DoubleClick(VolumeReport volumeZero, ListView listView)
        {
            Window.GetWindow(listView).Hide();
            if (!RevitData.Instance.ActiveView.IsInTemporaryViewMode(TemporaryViewMode.TemporaryHideIsolate))
            {
                Action action = new Action(() =>
                {
                    RevitData.Instance.Transaction.Start("Isolate elements");
                    RevitData.Instance.ActiveView.IsolateElementsTemporary(volumeZero.ElementIds);
                    RevitData.Instance.Transaction.Commit();
                });
                RevitData.Instance.ExternalEventHandlerReport.SetAction(action);
                RevitData.Instance.ExternalEventReport.Raise();
            }
            else
            {
                NotifyUtils.ResetView();
            }
            Window.GetWindow(listView).Show();
        }
        private void DoubleClick(UnionReport unionError, ListView listView)
        {
            Window.GetWindow(listView).Hide();
            if (!RevitData.Instance.ActiveView.IsInTemporaryViewMode(TemporaryViewMode.TemporaryHideIsolate))
            {
                Action action = new Action(() =>
                {
                    RevitData.Instance.Transaction.Start("Isolate elements");
                    RevitData.Instance.ActiveView.IsolateElementsTemporary(unionError.ElementIds);
                    RevitData.Instance.Transaction.Commit();
                });
                RevitData.Instance.ExternalEventHandlerReport.SetAction(action);
                RevitData.Instance.ExternalEventReport.Raise();
            }
            else
            {
                NotifyUtils.ResetView();
            }
            Window.GetWindow(listView).Show();
        }
        private void DoubleClick(JoinReport canJoin, ListView listView)
        {
            Window.GetWindow(listView).Hide();
            if (!RevitData.Instance.ActiveView.IsInTemporaryViewMode(TemporaryViewMode.TemporaryHideIsolate))
            {
                Action action = new Action(() =>
                {
                    RevitData.Instance.Transaction.Start("Isolate elements");
                    RevitData.Instance.ActiveView.IsolateElementsTemporary(canJoin.ElementIds);
                    RevitData.Instance.Transaction.Commit();
                });
                RevitData.Instance.ExternalEventHandlerReport.SetAction(action);
                RevitData.Instance.ExternalEventReport.Raise();
            }
            else
            {
                NotifyUtils.ResetView();
            }
            Window.GetWindow(listView).Show();
        }
        private void SelectAll(ObservableCollection<WarningAndErrorReport> reports, ListView listView)
        {
            Window.GetWindow(listView).Hide();
            if (!RevitData.Instance.ActiveView.IsInTemporaryViewMode(TemporaryViewMode.TemporaryHideIsolate))
            {
                if (reports.Count > 0)
                {
                    List<ElementId> elementIds = new List<ElementId>();
                    foreach (WarningAndErrorReport report in reports)
                    {
                        foreach (ElementId elementId in report.ElementIds)
                        {
                            elementIds.Add(elementId);
                        }
                    }
                    Action action = new Action(() =>
                    {
                        RevitData.Instance.Transaction.Start("Isolate elements");
                        RevitData.Instance.ActiveView.IsolateElementsTemporary(elementIds);
                        RevitData.Instance.Transaction.Commit();
                    });
                    RevitData.Instance.ExternalEventHandlerReport.SetAction(action);
                    RevitData.Instance.ExternalEventReport.Raise();
                }
                else
                {
                    NotifyUtils.ItemsSourceEmpty();
                }
            }
            else
            {
                NotifyUtils.ResetView();
            }
            Window.GetWindow(listView).Show();
        }
        private void SelectAll(ObservableCollection<UnionReport> unionErrors, ListView listView)
        {
            Window.GetWindow(listView).Hide();
            if (!RevitData.Instance.ActiveView.IsInTemporaryViewMode(TemporaryViewMode.TemporaryHideIsolate))
            {
                if (unionErrors.Count > 0)
                {
                    List<ElementId> elementIds = new List<ElementId>();
                    foreach (UnionReport unionError in unionErrors)
                    {
                        foreach (ElementId elementId in unionError.ElementIds)
                        {
                            elementIds.Add(elementId);
                        }
                    }
                    Action action = new Action(() =>
                    {
                        RevitData.Instance.Transaction.Start("Isolate elements");
                        RevitData.Instance.ActiveView.IsolateElementsTemporary(elementIds);
                        RevitData.Instance.Transaction.Commit();
                    });
                    RevitData.Instance.ExternalEventHandlerReport.SetAction(action);
                    RevitData.Instance.ExternalEventReport.Raise();
                }
                else
                {
                    NotifyUtils.ItemsSourceEmpty();
                }
            }
            else
            {
                NotifyUtils.ResetView();
            }
            Window.GetWindow(listView).Show();
        }
        private void SelectAll(ObservableCollection<VolumeReport> volumeZeros, ListView listView)
        {
            Window.GetWindow(listView).Hide();
            if (!RevitData.Instance.ActiveView.IsInTemporaryViewMode(TemporaryViewMode.TemporaryHideIsolate))
            {
                if (volumeZeros.Count > 0)
                {
                    List<ElementId> elementIds = new List<ElementId>();
                    foreach (VolumeReport volumeZero in volumeZeros)
                    {
                        foreach (ElementId elementId in volumeZero.ElementIds)
                        {
                            elementIds.Add(elementId);
                        }
                    }
                    Action action = new Action(() =>
                    {
                        RevitData.Instance.Transaction.Start("Isolate elements");
                        RevitData.Instance.ActiveView.IsolateElementsTemporary(elementIds);
                        RevitData.Instance.Transaction.Commit();
                    });
                    RevitData.Instance.ExternalEventHandlerReport.SetAction(action);
                    RevitData.Instance.ExternalEventReport.Raise();
                }
                else
                {
                    NotifyUtils.ItemsSourceEmpty();
                }
            }
            else
            {
                NotifyUtils.ResetView();
            }
            Window.GetWindow(listView).Show();
        }
        private void SelectAll(ObservableCollection<JoinReport> canJoins, ListView listView)
        {
            Window.GetWindow(listView).Hide();
            if (!RevitData.Instance.ActiveView.IsInTemporaryViewMode(TemporaryViewMode.TemporaryHideIsolate))
            {
                if (canJoins.Count > 0)
                {
                    List<ElementId> elementIds = new List<ElementId>();
                    foreach (JoinReport canJoin in canJoins)
                    {
                        foreach (ElementId elementId in canJoin.ElementIds)
                        {
                            elementIds.Add(elementId);
                        }
                    }
                    Action action = new Action(() =>
                    {
                        RevitData.Instance.Transaction.Start("Isolate elements");
                        RevitData.Instance.ActiveView.IsolateElementsTemporary(elementIds);
                        RevitData.Instance.Transaction.Commit();
                    });
                    RevitData.Instance.ExternalEventHandlerReport.SetAction(action);
                    RevitData.Instance.ExternalEventReport.Raise();
                }
                else
                {
                    NotifyUtils.ItemsSourceEmpty();
                }
            }
            else
            {
                NotifyUtils.ResetView();
            }
            Window.GetWindow(listView).Show();
        }
        private void ResetModeIsoLineHide(Page page)
        {
            Window.GetWindow(page).Hide();
            Action action = new Action(() =>
            {
                RevitData.Instance.Transaction.Start("Reset temporary hide isolate");
                RevitData.Instance.ActiveView.DisableTemporaryViewMode(TemporaryViewMode.TemporaryHideIsolate);
                RevitData.Instance.Transaction.Commit();
            });
            RevitData.Instance.ExternalEventHandlerReport.SetAction(action);
            RevitData.Instance.ExternalEventReport.Raise();
            Window.GetWindow(page).Show();
        }
        public void HighLight()
        {
            if (RevitData.Instance.AddInKey == AddinKeys.AutoJoin.Name)
            {
                //Warnings
                if (Instance.WarningReports.Count > 0)
                {
                    UIUtils.TabItem("tiWarnings").FontWeight = FontWeights.Bold;
                }
                else
                {
                    UIUtils.TabItem("tiWarnings").FontWeight = FontWeights.Normal;
                }

                //Errors
                if (Instance.ErrorReports.Count > 0)
                {
                    UIUtils.TabItem("tiErrors").FontWeight = FontWeights.Bold;
                }
                else
                {
                    UIUtils.TabItem("tiErrors").FontWeight = FontWeights.Normal;
                }

                //Volumes
                if (Instance.VolumeReports.Count > 0)
                {
                    UIUtils.TabItem("tiVolumes").FontWeight = FontWeights.Bold;
                }
                else
                {
                    UIUtils.TabItem("tiVolumes").FontWeight = FontWeights.Normal;
                }

                //Joins
                if (Instance.JoinReports.Count > 0)
                {
                    UIUtils.TabItem("tiJoins").FontWeight = FontWeights.Bold;
                }
                else
                {
                    UIUtils.TabItem("tiJoins").FontWeight = FontWeights.Normal;
                }

                //Unions
                if (Instance.UnionReports.Count > 0)
                {
                    UIUtils.TabItem("tiUnions").FontWeight = FontWeights.Bold;
                }
                else
                {
                    UIUtils.TabItem("tiUnions").FontWeight = FontWeights.Normal;
                }

                CheckToNotify();
            }
            else if (RevitData.Instance.AddInKey == AddinKeys.ExtendConcrete.Name)
            {
                //Errors
                if (Instance.ErrorReports.Count > 0)
                {
                    UIUtils.TabItem("tiErrors").FontWeight = FontWeights.Bold;
                }
                else
                {
                    UIUtils.TabItem("tiErrors").FontWeight = FontWeights.Normal;
                }

                CheckToNotify();
            }
        }
        public void ResetReportData()
        {
            Instance.UnionReports = new ObservableCollection<UnionReport>();
            Instance.VolumeReports = new ObservableCollection<VolumeReport>();
            Instance.ErrorReports = new ObservableCollection<WarningAndErrorReport>();
            Instance.WarningReports = new ObservableCollection<WarningAndErrorReport>();
            Instance.JoinReports = new ObservableCollection<JoinReport>();
        }
        public void CheckToNotify()
        {
            if (Instance.UnionReports.Count > 0 || Instance.VolumeReports.Count > 0 || Instance.ErrorReports.Count > 0 || Instance.WarningReports.Count > 0 || Instance.JoinReports.Count > 0)
            {
                Notification.ShowDialog("Please check reports tab to fix errors.", false);
            }
        }
        #endregion

    }
}