using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Runtime.CompilerServices;
using Autodesk.Revit.DB;
using Constant;
using HoaBinhTools.RebarShape2Excel.Setting;
using Utility;

namespace SingleData
{
	public class WPFData : INotifyPropertyChanged
	{
		public static WPFData Instance { get { return Singleton.Instance.WPFData; } }

		#region Variables
		private string savePath = Path.Combine(IOData.Instance.DesktopPath, ConstantValue.DEFAULT_EXCEL_EXPORT_FILENAME);
		private string templatePath = Path.Combine(IOData.Instance.AssemblyDirectoryPath, ConstantValue.EXCEL_TPL_FOLDER, ConstantValue.DEFAULT_EXCEL_TPL_FILENAME);
		private ObservableCollection<ViewSchedule> viewSchedules = WPFUtil.GetViewSchedules();
		private ObservableCollection<ViewSchedule> exportViewSchedules = new ObservableCollection<ViewSchedule>();
		private System.Drawing.Color foreColor;
		private System.Drawing.Color backColor;
		private ViewSchedule selectedViewSchedule;
		private ViewSchedule selectedExportViewSchedule;
		#endregion

		#region Properties
		public string SavePath
		{
			get
			{
				return savePath;
			}
			set
			{
				if (savePath == value) return;
				savePath = value;
				OnPropertyChanged();
			}
		}
		public string TemplatePath
		{
			get
			{
				return templatePath;
			}
			set
			{
				if (templatePath == value) return;
				templatePath = value;
				OnPropertyChanged();
			}
		}
		public ObservableCollection<ViewSchedule> ViewSchedules
		{
			get
			{
				return viewSchedules;
			}
			set
			{
				viewSchedules = value;
				OnPropertyChanged();


			}
		}
		public ObservableCollection<ViewSchedule> ExportViewSchedules
		{
			get
			{
				return exportViewSchedules;
			}
			set
			{
				exportViewSchedules = value;
				OnPropertyChanged();
			}
		}
		public System.Drawing.Color ForeColor
		{
			get
			{
				return foreColor;
			}
			set
			{
				if (foreColor == value) return;
				foreColor = value;
				OnPropertyChanged();
			}
		}
		public System.Drawing.Color BackColor
		{
			get
			{
				return backColor;
			}
			set
			{
				if (backColor == value) return;
				backColor = value;
				OnPropertyChanged();
			}
		}
		public ViewSchedule SelectedViewSchedule
		{
			get
			{
				return selectedViewSchedule;
			}
			set
			{
				selectedViewSchedule = value;
				OnPropertyChanged();
			}
		}
		public ViewSchedule SelectedExportViewSchedule
		{
			get
			{
				return selectedExportViewSchedule;
			}
			set
			{
				selectedExportViewSchedule = value;
				OnPropertyChanged();
			}
		}

		public System.Windows.Controls.ListView ViewSchedulesListView { get; set; }
		public System.Windows.Controls.ListView ExportViewSchedulesListView { get; set; }

		private bool? isOpenExcel;
		public bool? IsOpenExcel
		{
			get
			{
				if (isOpenExcel == null)
				{
					isOpenExcel = Settings1.Default.IsOpenExcel;
				}
				return isOpenExcel;
			}
			set
			{
				if (isOpenExcel == value) return;
				isOpenExcel = value;
				OnPropertyChanged();

				Settings1.Default.IsOpenExcel = value.Value;
				Settings1.Default.Save();
			}
		}
		#endregion

		#region PropertyChanged
		public event PropertyChangedEventHandler PropertyChanged;
		private void OnPropertyChanged([CallerMemberName] string propertyName = null)
		{
			PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
		}
		#endregion
	}
}
