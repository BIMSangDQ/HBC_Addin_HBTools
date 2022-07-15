using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;
using Autodesk.Revit.DB;
using Autodesk.Revit.DB.Structure;
using Autodesk.Revit.UI;
using HoaBinhTools.AutocadToRevit.Utils;
using HoaBinhTools.BIMQAQC.ModelChecker.Models;
using HoaBinhTools.BIMQAQC.ModelChecker.Views;
using HoaBinhTools.BIMQAQC.ModelChecker.Views.ProgessBar;
using HoaBinhTools.PurgeViewsV2.Models;
using HoaBinhTools.SynchronizedData.Db;
using Microsoft.Win32;
using OfficeOpenXml.FormulaParsing.Excel.Functions.DateTime;
using Utils;

namespace HoaBinhTools.BIMQAQC.ModelChecker.ViewModels
{
	public class LaunchViewModels : ViewModelBase
	{
		#region feild
		public ModelCheckerLaunchView Wmain { get; set; }

		public static Informations informations;

		private List<string> listPathFile;
		public List<string> ListPathFile
		{
			get
			{
				return listPathFile;
			}
			set
			{
				listPathFile = value;
				OnPropertyChanged("ListPathFile");
			}
		}

		private List<string> allParameter;
		public List<string> AllParameter
		{
			get
			{
				return allParameter;
			}
			set
			{
				allParameter = value;
				OnPropertyChanged("AllParameter");
			}
		}

		private ObservableCollection<string> allParameter2;
		public ObservableCollection<string> AllParameter2
		{
			get
			{
				return allParameter2;
			}
			set
			{
				allParameter2 = value;
				OnPropertyChanged("AllParameter2");
			}
		}

		private ObservableCollection<ClassFailureMessages> test;

		private ObservableCollection<ClassFailureMessages> Test
		{
			get
			{
				return test;
			}
			set
			{
				test = value;
				OnPropertyChanged("Test");
			}
		}

		private ObservableCollection<FileInfor> listFileInfors;

		public ObservableCollection<FileInfor> ListFileInfors
		{
			get
			{
				return listFileInfors;
			}
			set
			{
				listFileInfors = value;
				OnPropertyChanged("ListFileInfors");
			}
		}

		public static UIApplication UIApp { get; set; }

		public ObservableCollection<InfoView> allViews;

		public ObservableCollection<InfoView> AllViews
		{
			get
			{
				return allViews;
			}
			set
			{
				allViews = value;
			}
		}

		public ObservableCollection<InfoView> allViewSheet;

		public ObservableCollection<InfoView> AllViewSheet
		{
			get
			{
				return allViewSheet;
			}
			set
			{
				allViewSheet = value;
			}
		}

		//
		public ObservableCollection<InfoView> allViewsDFile;
		public ObservableCollection<InfoView> AllViewsDFile
		{
			get
			{
				return allViewsDFile;
			}
			set
			{
				allViewsDFile = value;
			}
		}

		public ObservableCollection<InfoView> allSheetDFile;
		public ObservableCollection<InfoView> AllSheetDFile
		{
			get
			{
				return allSheetDFile;
			}
			set
			{
				allSheetDFile = value;
			}
		}

		public RelayCommand btnRunFile { get; set; }
		public RelayCommand btnRunList { get; set; }
		#endregion

		#region FeildSetting

		private bool isAutoRun;
		public bool IsAutoRun
		{
			get
			{
				return isAutoRun;
			}
			set
			{
				isAutoRun = value;
				OnPropertyChanged("IsAutoRun");
			}
		}

		private bool isGGsheet;
		public bool IsGGsheet
		{
			get
			{
				return isGGsheet;
			}
			set
			{
				isGGsheet = value;
				OnPropertyChanged("IsGGsheet");
			}
		}

		private bool isFileSelect;
		public bool IsFileSelect
		{
			get
			{
				return isFileSelect;
			}
			set
			{
				isFileSelect = value;
				OnPropertyChanged("IsFileSelect");
			}
		}

		private ObservableCollection<string> timeTrigers = new ObservableCollection<string>()
		{
			"Day",
			"Week",
			"Month"
		};
		public ObservableCollection<string> TimeTrigers
		{
			get
			{
				return timeTrigers;
			}
			set
			{
				timeTrigers = value;
				OnPropertyChanged("TimeTrigers");
			}
		}

		private string timeTriger;
		public string TimeTriger
		{
			get
			{
				return timeTriger;
			}
			set
			{
				timeTriger = value;
				OnPropertyChanged("TimeTriger");
			}
		}

		private ObservableCollection<string> datesApps = new ObservableCollection<string>()
		{
			"Sunday",
			"Monday",
			"Tueday",
			"Wednesday",
			"Thurday",
			"Friday",
			"Satuday"
		};

		public ObservableCollection<string> DatesApps
		{
			get
			{
				return datesApps;
			}
			set
			{
				datesApps = value;
				OnPropertyChanged("DatesApps");
			}
		}

		private string datesApp;

		public string DatesApp
		{
			get
			{
				return datesApp;
			}
			set
			{
				datesApp = value;
				OnPropertyChanged("DatesApp");
			}
		}

		private ObservableCollection<string> timeApps = new ObservableCollection<string>()
		{
			"~1h am",
			"~2h am",
			"~3h am",
			"~4h am",
			"~5h am",
			"~6h am",
			"~7h am",
			"~8h am",
			"~9h am",
			"~10h am",
			"~11h am",
			"~12h am",
			"~1h pm",
			"~2h pm",
			"~3h pm",
			"~4h pm",
			"~5h pm",
			"~6h pm",
			"~7h pm",
			"~8h pm",
			"~9h pm",
			"~10h pm",
			"~11h pm",
			"~12h pm",
		};

		public ObservableCollection<string> TimeApps
		{
			get
			{
				return timeApps;
			}
			set
			{
				timeApps = value;
				OnPropertyChanged("TimeApps");
			}
		}

		private string timeApp;
		public string TimeApp
		{
			get
			{
				return timeApp;
			}
			set
			{
				timeApp = value;
				OnPropertyChanged("TimeApp");
			}
		}

		private string folderResult;
		public string FolderResult
		{
			get
			{
				return folderResult;
			}
			set
			{
				folderResult = value;
				OnPropertyChanged("FolderResult");
			}
		}

		private ObservableCollection<FileTracking> listFileAutoRun;

		public ObservableCollection<FileTracking> ListFileAutoRun
		{
			get
			{
				return listFileAutoRun;
			}
			set
			{
				listFileAutoRun = value;
				OnPropertyChanged("ListFileAutoRun");
			}
		}

		public RelayCommand SelectFile { get; set; }

		public RelayCommand BtnGetListInGGSheet { get; set; }

		public RelayCommand btnSaveSetting { get; set; }

		public RelayCommand btnDelPath { get; set; }

		public RelayCommand btnClose { get; set; }

		public RelayCommand btnSelectFolder { get; set; }

		public RelayCommand<ElementId> btnFindElement { get; set; }

		public RelayCommand btnFindElement2 { get; set; }

		public RelayCommand btnExportToExcel { get; set; }

		#region Event
		public RelayCommand btnChangeTimeTriger { get; set; }

		public RelayCommand btnGeneral { get; set; }


		#endregion

		#endregion
		public LaunchViewModels(UIApplication uiapp)
		{
			ListFileAutoRun = new ObservableCollection<FileTracking>();

			SelectFile = new RelayCommand(SelectFileRevit);

			BtnGetListInGGSheet = new RelayCommand(GetListInGGSheet);

			btnSaveSetting = new RelayCommand(SaveSetting);

			btnRunList = new RelayCommand(GetInforListFile);

			btnRunFile = new RelayCommand(GetInforFile);

			btnChangeTimeTriger = new RelayCommand(ChangeTimeTriger);

			btnClose = new RelayCommand(WmainClose);

			btnDelPath = new RelayCommand(DelPath);

			btnExportToExcel = new RelayCommand(ExporttoExcel);

			btnGeneral = new RelayCommand(BtnGeneralCheck);

			btnFindElement = new RelayCommand<ElementId>(FindElement);

			btnSelectFolder = new RelayCommand(BtnSelectFolder);

			btnFindElement2 = new RelayCommand(BtnFindElement2);

			UIApp = uiapp;

			GetSetting();
			
		}

		public void Excute()
		{
			Wmain = new ModelCheckerLaunchView(this);

			AllParameter = GetAllParameter(UIApp.ActiveUIDocument.Document);

			AllParameter2 = new ObservableCollection<string>();
			foreach (string item in AllParameter)
			{
				AllParameter2.Add(item);
			}

			Wmain.CheckSetFrame.Content = new GeneralCheckset(new GeneralCheckSetViewModel(UIApp.ActiveUIDocument.Document, Wmain, AllParameter));

			Wmain.DFrame2.Content = new DisciplineCheckSet(new DisciplineCheckSetViewModel(UIApp.ActiveUIDocument.Document, Wmain, AllParameter));
			Wmain.Show();
			ChangeTimeTriger();
		}

		public List<string> GetAllParameter(Document doc)
		{
			List<string> AllParameters = new List<string>();

			BindingMap map = doc.ParameterBindings;
			DefinitionBindingMapIterator it = map.ForwardIterator();
			it.Reset();

			while (it.MoveNext())
			{
				AllParameters.Add(it.Key.Name);
			}

			AllParameters = AllParameters.Distinct().ToList();
			AllParameters.Sort();

			//List<Element> allElements = new FilteredElementCollector(doc)
			//	.WhereElementIsNotElementType()
			//	.Where(x => x.Category != null)
			//	.ToList();

			//allElements = allElements
			//				.Distinct(new IEqualityComparerCategory())
			//				.ToList();

			//foreach (Element e in allElements)
			//{
			//	AllParameters.AddRange(ParameterUtils.GetAllParameters(e, true));
			//}

			//AllParameters = AllParameters.Distinct().ToList();
			//AllParameters.Sort();

			return AllParameters;
		}

		#region Method
		public void GetInforFile()
		{
			Test = new ObservableCollection<ClassFailureMessages>();

			ObservableCollection<FileInfor> trackingFolders = new ObservableCollection<FileInfor>();
			ListFileInfors = new ObservableCollection<FileInfor>();

			ProgressBarViewQAQC prog = new ProgressBarViewQAQC();
			prog.Show();

			try
			{
				DateTime d1 = DateTime.Now;
				Document document = UIApp.ActiveUIDocument.Document;
				if (document != null)
				{

					//File Name
					string fileName = GetInforOrtherFile.GetfileName(document);
					var s = fileName.Split('\\');
					fileName = s[s.Length - 1];

					prog.Create(1, 15, fileName, "Check File size");
					//File size
					double fileSize = new FileInfo(document.PathName).Length;
					fileSize = Math.Round(fileSize / Math.Pow(1024, 2), 2);

					//Warning
					prog.Create2("Check Warning");
					double totalWarnings = document.GetWarnings().Count;

					//Total Element
					prog.Create2("Check Total Element");
					double totalInstances = new FilteredElementCollector(document).WhereElementIsNotElementType().GetElementCount();

					//Purable Element
					prog.Create2("Check Purable Element");
					double PurableElement = GetInforOrtherFile.CountPurgeElement(document);

					//Model Group
					prog.Create2("Check Model Group");
					List<Element> elements = new FilteredElementCollector(document).WhereElementIsNotElementType().ToElements().ToList();
					ElementId elementIdModelGroups = new ElementId(-2000095);
					ElementId elementIdDetailGroups = new ElementId(-2000096);
					int ModelGroupCount = elements.Where(x => x.Category != null && x.Category.Id == elementIdModelGroups).Count();
					int DetailGroupCount = elements.Where(x => x.Category != null && x.Category.Id == elementIdDetailGroups).Count();

					//In Place families
					prog.Create2("Check In Place Families");
					double InPlaceFamilies = GetInforOrtherFile.GetFamiliesInPlace(document);


					//Duplicate_ intances
					prog.Create2("Check Duplicate Intance");
					double Duplicate = GetInforOrtherFile.GetDuplicateIntances(document);

					//View 
					prog.Create2("Check View not on Sheet");
					int ViewCount = 0;
					AllViewsDFile = GetInforOrtherFile.GetInfoView(document);

					foreach (InfoView listView in AllViewsDFile[0].Models)
					{
						ViewCount = ViewCount + listView.Views.Count();
					}

					//Sheet
					prog.Create2("Check Sheet");
					int SheetCount = 0;
					AllSheetDFile = GetInforOrtherFile.GetInfoViewSheet(document);
					foreach (InfoView listView in AllSheetDFile[0].Models)
					{
						SheetCount = SheetCount + listView.Views.Count();
					}

					//Hidden Element
					prog.Create2("Check Hidden Element");
					double HiddenElemnt = GetInforOrtherFile.GetHiddenElement(document);

					IEnumerable<ImportInstance> CADInstances = new FilteredElementCollector(document).OfClass(typeof(ImportInstance)).WhereElementIsNotElementType().Cast<ImportInstance>();
					//Cad Import
					prog.Create2("Check Cad Import");
					double totalImportCAD = CADInstances.Where(x => x != null && !x.IsLinked).Count();

					//Link Cad
					prog.Create2("Check Link Cad");
					double totalLinkCAD = CADInstances.Where(x => x != null && x.IsLinked).Count();


					//Link Revit
					prog.Create2("Check Link Revit");
					double totalRevitLinks = new FilteredElementCollector(document).OfCategory(BuiltInCategory.OST_RvtLinks).WhereElementIsNotElementType().GetElementCount();

					double totalTypes = new FilteredElementCollector(document).WhereElementIsElementType().GetElementCount();

					//Workset count
					prog.Create2("Check Workset");
					IList<Workset> worksetList = new FilteredWorksetCollector(document).OfKind(WorksetKind.UserWorkset).ToWorksets();

					//Test Failure
					//prog.Create2("PerformanceAdviser");
					//ObservableCollection<ClassFailureMessages> Test2 = GetInforOrtherFile.Test(document);
					//foreach (ClassFailureMessages c in Test2)
					//{
					//	Test.Add(c);
					//}

					DateTime d2 = DateTime.Now;
					TimeSpan t = d2 - d1;
					ListFileInfors.Add(new FileInfor()
					{
						FileName = fileName,
						FileSize = fileSize,
						Warnings = (int)totalWarnings,
						Total_Element = (int)totalInstances,
						Purable_Elements = (int)PurableElement,
						Model_Groups = ModelGroupCount,
						Detail_Groups = DetailGroupCount,
						Views = ViewCount,
						Sheets = SheetCount,
						Hidden_Elements = (int)HiddenElemnt,
						In_Place_Families = (int)InPlaceFamilies,
						Duplicate_Intances = (int)Duplicate,
						Link_Revit = (int)totalRevitLinks,
						Linked_Cad = (int)totalLinkCAD,
						Cad_Imports = (int)totalImportCAD,
						Worksets = worksetList.Count(),
						TimeSpan = t.ToString(@"hh\:mm\:ss"),
					});


					//Check General theo bộ Set

					//SendDb(ListFileInfors[ListFileInfors.Count - 1]);

					prog.Create2("General Check");
					GeneralCheck(document, ListFileInfors[ListFileInfors.Count - 1]);

					//Set tên theo bộ current
					DisciplineCheckSet CheckSet = (DisciplineCheckSet)this.Wmain.DFrame2.Content;
					DisciplineCheckSetViewModel a = (DisciplineCheckSetViewModel)CheckSet.DataContext;

					DisciplineChekSetName = a.DesciplineCheckName;
					prog.Create2("Discipline Check");
					DisciplineCheck(document, ListFileInfors[ListFileInfors.Count - 1]);

					//SendDb(ListFileInfors[ListFileInfors.Count - 1]);

					prog.Create1();
					prog.Close();

				}
			}
			catch { }

			//Gửi lên ggdriver
			PutdataDriverQCModel(ListFileInfors[ListFileInfors.Count - 1]);

			Wmain.ResultTab.IsSelected = true;
		}

		public void GetInforListFile()
		{
			Test = new ObservableCollection<ClassFailureMessages>();
			if (IsGGsheet == true)
			{
				GetListInGGSheet();
			}
			ListPathFile = new List<string>();
			ListPathFile = ListFileAutoRun.Select(x => x.Path).ToList();
			ListFileInfors = new ObservableCollection<FileInfor>();
			ProgressBarViewQAQC prog = new ProgressBarViewQAQC();
			prog.Show();
			if (ListPathFile.Count > 0)
			{
				ObservableCollection<FileInfor> trackingFolders = new ObservableCollection<FileInfor>();

				foreach (string Path in ListPathFile)
				{
					prog.Create(ListPathFile.Count, 15, Path, "Check File size");

					try
					{
						DateTime d1 = DateTime.Now;

						Document document;

						document = UIApp.Application.OpenDocumentFile(Path);
						
						if (document != null)
						{

							//File Name
							string fileName = GetInforOrtherFile.GetfileName(document);
							var s = fileName.Split('\\');
							fileName = s[s.Length - 1];

							//File size
							double fileSize = new FileInfo(document.PathName).Length;
							fileSize = Math.Round(fileSize / Math.Pow(1024, 2), 2);

							//Warning
							prog.Create2("Check Warning");
							double totalWarnings = document.GetWarnings().Count;

							//Total Element
							prog.Create2("Check Total Element");
							double totalInstances = new FilteredElementCollector(document).WhereElementIsNotElementType().GetElementCount();

							//Purable Element
							prog.Create2("Check Purable Element");
							double PurableElement = GetInforOrtherFile.CountPurgeElement(document);

							//Model Group
							prog.Create2("Check Model Group");
							List<Element> elements = new FilteredElementCollector(document).WhereElementIsNotElementType().ToElements().ToList();
							ElementId elementIdModelGroups = new ElementId(-2000095);
							ElementId elementIdDetailGroups = new ElementId(-2000096);
							int ModelGroupCount = elements.Where(x => x.Category != null && x.Category.Id == elementIdModelGroups).Count();
							int DetailGroupCount = elements.Where(x => x.Category != null && x.Category.Id == elementIdDetailGroups).Count();

							//In Place families
							prog.Create2("Check In Place Families");
							double InPlaceFamilies = GetInforOrtherFile.GetFamiliesInPlace(document);


							//Duplicate_ intances
							prog.Create2("Check Duplicate Intance");
							double Duplicate = GetInforOrtherFile.GetDuplicateIntances(document);

							//View 
							prog.Create2("Check View not on Sheet");
							int ViewCount = 0;
							AllViewsDFile = GetInforOrtherFile.GetInfoView(document);

							foreach (InfoView listView in AllViewsDFile[0].Models)
							{
								ViewCount = ViewCount + listView.Views.Count();
							}

							//Sheet
							prog.Create2("Check Sheet");
							int SheetCount = 0;
							AllSheetDFile = GetInforOrtherFile.GetInfoViewSheet(document);
							foreach (InfoView listView in AllSheetDFile[0].Models)
							{
								SheetCount = SheetCount + listView.Views.Count();
							}

							//Hidden Element
							prog.Create2("Check Hidden Element");
							double HiddenElemnt = GetInforOrtherFile.GetHiddenElement(document);


							IEnumerable<ImportInstance> CADInstances = new FilteredElementCollector(document).OfClass(typeof(ImportInstance)).WhereElementIsNotElementType().Cast<ImportInstance>();
							//Cad Import
							prog.Create2("Check Cad Import");
							double totalImportCAD = CADInstances.Where(x => x != null && !x.IsLinked).Count();

							//Link Cad
							prog.Create2("Check Link Cad");
							double totalLinkCAD = CADInstances.Where(x => x != null && x.IsLinked).Count();


							//Link Revit
							prog.Create2("Check Link Revit");
							double totalRevitLinks = new FilteredElementCollector(document).OfCategory(BuiltInCategory.OST_RvtLinks).WhereElementIsNotElementType().GetElementCount();

							double totalTypes = new FilteredElementCollector(document).WhereElementIsElementType().GetElementCount();

							//Workset count
							prog.Create2("Check Workset");
							IList<Workset> worksetList = new FilteredWorksetCollector(document).OfKind(WorksetKind.UserWorkset).ToWorksets();

							////Test Failure
							//prog.Create2("PerformanceAdviser");
							//ObservableCollection<ClassFailureMessages> Test2 = GetInforOrtherFile.Test(document);
							//foreach (ClassFailureMessages c in Test2)
							//{
							//	Test.Add(c);
							//}

							DateTime d2 = DateTime.Now;
							TimeSpan t = d2 - d1;
							ListFileInfors.Add(new FileInfor()
							{
								FileName = fileName,
								FileSize = fileSize,
								Warnings = (int)totalWarnings,
								Total_Element = (int)totalInstances,
								Purable_Elements = (int)PurableElement,
								Model_Groups = ModelGroupCount,
								Detail_Groups = DetailGroupCount,
								Views = ViewCount,
								Sheets = SheetCount,
								Hidden_Elements = (int)HiddenElemnt,
								In_Place_Families = (int)InPlaceFamilies,
								Duplicate_Intances = (int)Duplicate,
								Link_Revit = (int)totalRevitLinks,
								Linked_Cad = (int)totalLinkCAD,
								Cad_Imports = (int)totalImportCAD,
								Worksets = worksetList.Count(),
								TimeSpan = t.ToString(@"hh\:mm\:ss")
							});


							SendDb(ListFileInfors[ListFileInfors.Count - 1]);

							prog.Create2("General Check");
							GeneralCheck(document, ListFileInfors[ListFileInfors.Count - 1]);

							DisciplineChekSetName = GetListFile.GetDisciplineCheckName(ListFileInfors[ListFileInfors.Count - 1].FileName);
							prog.Create2("Discipline Check");
							DisciplineCheck(document, ListFileInfors[ListFileInfors.Count - 1]);

							SendDb(ListFileInfors[ListFileInfors.Count - 1]);

							prog.Create1();
							document.Close();
						}

					}
					catch
					{
						prog.Create1();
						continue;
					}
				}
			}

			if (QAQCSetting.Default.IsAutoRun == true)
			{
				prog.Create2("Export Excel");
				ToExcel.ExportExcelWithAutorun(ListFileInfors);
			}
			prog.Close();
		}

		public void SelectFileRevit()
		{
			OpenFileDialog dlg = new OpenFileDialog();

			dlg.Title = "Revit file";
			dlg.Filter = "Revit file (*.rvt)|*.rvt";
			dlg.Multiselect = true;
			dlg.FilterIndex = 0;
			dlg.RestoreDirectory = true;

			Nullable<bool> result = dlg.ShowDialog();

			if (result == true)
			{
				foreach (string st in dlg.FileNames)
				{
					var s = st.Split('\\');
					ListFileAutoRun.Add(new FileTracking
					{
						Path = st,
						FileName = s[s.Length - 1],
					});
				}
			}
		}

		public void GetListInGGSheet()
		{
			Document activeDocument = UIApp.ActiveUIDocument.Document;
			string version = activeDocument.Application.VersionName;

			List<string> ListPath = GetListFile.GetPathRevit(version);
			ListFileAutoRun = new ObservableCollection<FileTracking>();
			try
			{
				foreach (string st in ListPath)
				{
					var s = st.Split('\\');
					ListFileAutoRun.Add(new FileTracking
					{
						Path = st,
						FileName = s[s.Length - 1],
					});
				}
			}
			catch
			{ }

		}

		public void SaveSetting()
		{
			List<string> ListFile = new List<string>();
			ListFile = ListFileAutoRun.Select(x => x.Path).ToList();

			QAQCSetting.Default.IsAutoRun = IsAutoRun;
			QAQCSetting.Default.IsGGsheet = IsGGsheet;
			QAQCSetting.Default.IsFileSelect = IsFileSelect;
			QAQCSetting.Default.TimeTrigers = TimeTriger;
			QAQCSetting.Default.DatesApps = DatesApp;
			QAQCSetting.Default.TimeApps = TimeApp;
			QAQCSetting.Default.ListFileAutoRun = ListFile;
			QAQCSetting.Default.FolderResult = FolderResult;

			QAQCSetting.Default.Save();
		}

		public void GetSetting()
		{
			List<string> ListFile = new List<string>();

			IsAutoRun = QAQCSetting.Default.IsAutoRun;
			IsGGsheet = QAQCSetting.Default.IsGGsheet;
			TimeTriger = QAQCSetting.Default.TimeTrigers;
			DatesApp = QAQCSetting.Default.DatesApps;
			TimeApp = QAQCSetting.Default.TimeApps;
			IsFileSelect = QAQCSetting.Default.IsFileSelect;
			FolderResult = QAQCSetting.Default.FolderResult;
			ListFile = QAQCSetting.Default.ListFileAutoRun;

			ListFileAutoRun = new ObservableCollection<FileTracking>();
			if (IsGGsheet)
			{
				GetListInGGSheet();
			}
			else
			{
				try
				{
					foreach (string st in ListFile)
					{
						var s = st.Split('\\');
						ListFileAutoRun.Add(new FileTracking
						{
							Path = st,
							FileName = s[s.Length - 1],
						});
					}
				}
				catch
				{ }
			}
			ChangeTimeTriger();
		}

		public void ChangeTimeTriger()
		{
			try
			{
				if (TimeTriger == "Day")
				{
					Wmain.TimeTrigerlevel2.IsEnabled = false;
					DatesApps = new ObservableCollection<string>();
				}
				else if (TimeTriger == "Week")
				{
					Wmain.TimeTrigerlevel2.IsEnabled = true;
					DatesApps = new ObservableCollection<string>()
				{
					"Sunday",
					"Monday",
					"Tueday",
					"Wednesday",
					"Thurday",
					"Friday",
					"Satuday"
				};
				}
				else if (TimeTriger == "Month")
				{
					Wmain.TimeTrigerlevel2.IsEnabled = true;
					DatesApps = new ObservableCollection<string>()
				{
					"1st",
					"2nd",
					"3rd",
					"4th",
					"4th",
					"5th",
					"6th",
					"7th",
					"8th",
					"9th",
					"10th",
					"11th",
					"12th",
					"13th",
					"14th",
					"15th",
					"16th",
					"17th",
					"18th",
					"19th",
					"20th",
					"21th",
					"22th",
					"23th",
					"24th",
					"25th",
					"26th",
					"27th",
					"28th",
					"29th",
					"30th",
					"31th",
				};
				}
			}
			catch { }
		}

		public void DelPath()
		{
			FileTracking Path2Del = (FileTracking)Wmain.dgFile.SelectedItem;

			ListFileAutoRun.Remove(Path2Del);
		}

		public void WmainClose()
		{
			Wmain.Close();
		}

		public void SendDb(FileInfor FI) // Check cùng ngày thì update
		{
			DbConnect db = new DbConnect();
			string query = "";

			query = string.Format("Select * from Addin_QAQC where FileName = '{0}'",FI.FileName);

			//Check file trong tháng ngày lớn nhất trong tháng => lấy id rồi ghi đè
			DataTable table = db.Get_DataTable(query);

			string ID = "";
			DateTime Excudteday;
			int max = 0;
			string ex = DateTime.Now.Year.ToString() + "-" + (DateTime.Now.Month < 10 ? "0" + DateTime.Now.Month.ToString() : DateTime.Now.Month.ToString());
			Regex regex = new Regex(@"^(<?year>[0-9]+)-(<?month>[0-9]+)-(<?day>[0-9]+)");


			foreach (DataRow dr in table.Rows)
			{
				int ExDay = 0;
				int ExMonth = 0;

				Excudteday = (DateTime)dr["ExecuteDay"];
				ExDay = Excudteday.Day;
				ExMonth = Excudteday.Month;
				if (ExMonth == DateTime.Now.Month)
				{
						if (ExDay > max)
						{
							max = ExDay;
							ID = dr["ID"].ToString();
						}
				}
			}

			if (ID == "")
			{
				query = string.Format("INSERT INTO Addin_QAQC (ExecuteDay,FileName,FileSize,Warnings,Total_Element,Purable_Elements,Model_Groups," +
				"Detail_Groups,In_Place_Families,Duplicate_Intances,Views_Not_On_Sheet,Sheets,Hidden_Elements,Cad_Imports,Link_Revit,Linked_Cad,Worksets,Certification )" +
				"VALUES('{0}','{1}','{2}','{3}','{4}','{5}','{6}','{7}','{8}','{9}','{10}','{11}','{12}','{13}','{14}','{15}','{16}','{17}')",
				DateTime.Now.Year.ToString() + "-" + (DateTime.Now.Month < 10 ? "0" + DateTime.Now.Month.ToString() : DateTime.Now.Month.ToString())
				+ "-" + (DateTime.Now.Day < 10 ? "0" + DateTime.Now.Day.ToString() : DateTime.Now.Day.ToString()),
				FI.FileName,
				FI.FileSize,
				FI.Warnings,
				FI.Total_Element,
				FI.Purable_Elements,
				FI.Model_Groups,
				FI.Detail_Groups,
				FI.In_Place_Families,
				FI.Duplicate_Intances,
				FI.Views,
				FI.Sheets,
				FI.Hidden_Elements,
				FI.Cad_Imports,
				FI.Link_Revit,
				FI.Linked_Cad,
				FI.Worksets,
				FI.FileResultsDescription);

				db.Execute_SQL(query);
			}
			else
			{
				try
				{

					query = string.Format("UPDATE Addin_QAQC SET FileSize = '{2}',Warnings= '{3}',Total_Element= '{4}',Purable_Elements= '{5}',Model_Groups= '{6}'," +
					"Detail_Groups= '{7}',In_Place_Families= '{8}',Duplicate_Intances= '{9}',Views_Not_On_Sheet= '{10}',Sheets= '{11}',Hidden_Elements= '{12}'," +
					"Cad_Imports= '{13}',Link_Revit= '{14}',Linked_Cad= '{15}',Worksets= '{16}',Certification= '{17}', ExecuteDay = '{0}'" +
					"WHERE ID = '{18}' And FileName = '{1}' ",
					DateTime.Now.Year.ToString() + "-" + (DateTime.Now.Month < 10 ? "0" + DateTime.Now.Month.ToString() : DateTime.Now.Month.ToString())
					+ "-" + (DateTime.Now.Day < 10 ? "0" + DateTime.Now.Day.ToString() : DateTime.Now.Day.ToString()),
					FI.FileName,
					FI.FileSize,
					FI.Warnings,
					FI.Total_Element,
					FI.Purable_Elements,
					FI.Model_Groups,
					FI.Detail_Groups,
					FI.In_Place_Families,
					FI.Duplicate_Intances,
					FI.Views,
					FI.Sheets,
					FI.Hidden_Elements,
					FI.Cad_Imports,
					FI.Link_Revit,
					FI.Linked_Cad,
					FI.Worksets,
					FI.FileResultsDescription,
					ID);

					db.Execute_SQL(query);
				}
				catch
				{
					query = string.Format("INSERT INTO Addin_QAQC (ExecuteDay,FileName,FileSize,Warnings,Total_Element,Purable_Elements,Model_Groups," +
					"Detail_Groups,In_Place_Families,Duplicate_Intances,Views_Not_On_Sheet,Sheets,Hidden_Elements,Cad_Imports,Link_Revit,Linked_Cad,Worksets,Certification )" +
					"VALUES('{0}','{1}','{2}','{3}','{4}','{5}','{6}','{7}','{8}','{9}','{10}','{11}','{12}','{13}','{14}','{15}','{16}','{17}')",
					DateTime.Now.Year.ToString() + "-" + (DateTime.Now.Month < 10 ? "0" + DateTime.Now.Month.ToString() : DateTime.Now.Month.ToString())
					+ "-" + (DateTime.Now.Day < 10 ? "0" + DateTime.Now.Day.ToString() : DateTime.Now.Day.ToString()),
					FI.FileName,
					FI.FileSize,
					FI.Warnings,
					FI.Total_Element,
					FI.Purable_Elements,
					FI.Model_Groups,
					FI.Detail_Groups,
					FI.In_Place_Families,
					FI.Duplicate_Intances,
					FI.Views,
					FI.Sheets,
					FI.Hidden_Elements,
					FI.Cad_Imports,
					FI.Link_Revit,
					FI.Linked_Cad,
					FI.Worksets,
					FI.FileResultsDescription);

					db.Execute_SQL(query);
				}
			}

			try
			{
				query = string.Format("Select * from Addin_QAQC where ExecuteDay = '{0}' AND FileName = '{1}'", DateTime.Now.Year.ToString() + "-" + (DateTime.Now.Month < 10 ? "0" + DateTime.Now.Month.ToString() : DateTime.Now.Month.ToString())
				+ "-" + (DateTime.Now.Day < 10 ? "0" + DateTime.Now.Day.ToString() : DateTime.Now.Day.ToString()),
				FI.FileName);

				table = db.Get_DataTable(query);
				string IDFile = "";
				foreach (DataRow row in table.Rows)
				{
					IDFile = row["ID"].ToString();
				}

				foreach (FileCheckResult result in FI.FileCheckResults)
				{
					query = $"SELECT * FROM Addin_QAQC_DesciplineResult WHERE Id_File = '{IDFile}' AND Id_CheckRow = '{result.Id}'";
					if (db.Get_DataTable(query).Rows.Count > 0)
					{
						query = $"UPDATE Addin_QAQC_DesciplineResult  SET Result = '{result.Result}', CountElement =  '{result.ElementCount}', Rank = '{result.SetRange}' WHERE Id_File = '{IDFile}' AND Id_CheckRow = '{result.Id}'";
						db.Execute_SQL(query);
					}
					else
					{
						query = $"INSERT INTO Addin_QAQC_DesciplineResult (Id_File,Id_CheckRow,Result,CountElement,Rank) VALUES ('{IDFile}','{result.Id}','{result.Result}','{result.ElementCount}','{result.SetRange}')";
						db.Execute_SQL(query);
					}
				}
			}
			catch { }
			db.Close_DB_Connection();
		}

		public void ExporttoExcel()
		{
			ToExcel.ExportToExcel(ListFileInfors);
		}

		public void BtnSelectFolder()
		{
			try
			{
				System.Windows.Forms.FolderBrowserDialog folderDlg = new System.Windows.Forms.FolderBrowserDialog();
				folderDlg.ShowNewFolderButton = true;
				System.Windows.Forms.DialogResult result = folderDlg.ShowDialog();
				if (result == System.Windows.Forms.DialogResult.OK)
				{
					FolderResult = folderDlg.SelectedPath;
					Environment.SpecialFolder root = folderDlg.RootFolder;
				}
			}
			catch { }
		}

		public void BtnGeneralCheck()
		{

		}

		public void FindElement(ElementId elementId)
		{
			
			try
			{
				if (elementId != null)
				{

					List<ElementId> listId = new List<ElementId>();

					listId.Add(elementId);
					Action action = new Action(() =>
						{
							using (Transaction trans = new Transaction(ActiveData.Document))
							{
								trans.Start("Transaction Group");
								if (ActiveData.ActiveView.IsTemporaryHideIsolateActive())
								{
									TemporaryViewMode tempView = TemporaryViewMode.TemporaryHideIsolate;

									ActiveData.ActiveView.DisableTemporaryViewMode(tempView);
								}

								//ActiveData.ActiveView.IsolateElementsTemporary(listId);

								// zoom tới telemt
								ActiveData.UIDoc.Selection.SetElementIds(listId);

								ActiveData.UIDoc.ShowElements(listId);

								ActiveData.Document.Regenerate();
								trans.Commit();
							}
						});
					ExternalEventHandler.Instance.SetAction(action);

					ExternalEventHandler.Instance.Run();
				}
			}
			catch
			{
			}
		}

		public void BtnFindElement2()
		{
			MessageBox.Show("");
		}
		#endregion

		#region CheckGeneral
		public void GeneralCheck(Document doc, FileInfor FI)
		{
			FI.File_name_Check = CheckFileName(doc);
			FI.ProjectInfor_Check = CheckProjectInfor(doc);
			FI.ProjectLocation_Check = CheckProjectLocation(doc);
			FI.Levels_Workset_Check = CheckWorksetLevelAndGrid(doc);
			FI.WrongElement_Check = CheckWrongElementWorksetLevelAndGrid(doc);
		}

		//Check FileName theo BEP 
		public bool CheckFileName(Document doc)
		{
			string GeneralCheckName = "Default";
			DbConnect db = new DbConnect();
			string query = string.Format("SELECT * FROM Addin_QAQC_General_Check WHERE CheckSetName = '{0}'", GeneralCheckName);

			bool RunFileName = false;
			string pattern = "";

			DataTable table = db.Get_DataTable(query);
			foreach (DataRow row in table.Rows)
			{
				RunFileName = bool.Parse(row["Run_FileName"].ToString());
				pattern = row["FileNamePattern"].ToString();
			}

			if (!RunFileName) return true;
			Regex reg = new Regex(pattern);

			string filePath = GetInforOrtherFile.GetfileName(doc);
			var s = filePath.Split('\\');
			string fileName = s[s.Length - 1];

			Match match = reg.Match(fileName);
			bool TF = match.Success;

			string dateexcute = DateTime.Now.Year.ToString() + "-" + (DateTime.Now.Month < 10 ? "0" + DateTime.Now.Month.ToString() : DateTime.Now.Month.ToString())
				+ "-" + (DateTime.Now.Day < 10 ? "0" + DateTime.Now.Day.ToString() : DateTime.Now.Day.ToString());

			query = $"SELECT * FROM Addin_QAQC WHERE FileName = '{fileName}' AND ExecuteDay = '{dateexcute}'";
			string idFile = "";
			foreach (DataRow row in db.Get_DataTable(query).Rows)
			{
				idFile = row["ID"].ToString();
			}

			//CHECK EXITS
			query = $"SELECT * FROM Addin_QAQC_GeneralResult WHERE Id_File = '{idFile}'";
			foreach (DataRow row in db.Get_DataTable(query).Rows)
			{
				db.Close_DB_Connection();
				return TF;
			}
			query = $"INSERT INTO Addin_QAQC_GeneralResult (Id_File,FileName_Check) VALUES ('{idFile}','{TF}')";
			db.Execute_SQL(query);

			db.Close_DB_Connection();
			return TF;
		}

		//Check ProjectInfor đã được điền đủ chưa
		public bool CheckProjectInfor(Document doc)
		{
			string GeneralCheckName = "Default";
			DbConnect db = new DbConnect();
			string query = string.Format("SELECT * FROM Addin_QAQC_General_Check WHERE CheckSetName = '{0}'", GeneralCheckName);

			bool RunProjectInfor = false;
			string para = "";

			DataTable table = db.Get_DataTable(query);
			foreach (DataRow row in table.Rows)
			{
				RunProjectInfor = bool.Parse(row["Run_ProjectInfor"].ToString());
				para = row["ListParaProjectInfor"].ToString();
			}
			if (!RunProjectInfor) return true;

			var paraInfor = para.Split('|');

			List<Element> allElements = new FilteredElementCollector(doc)
				.WhereElementIsNotElementType()
				.Where(x => x.Category != null && x.Category.Name == "Project Information")
				.ToList();
			Element element = allElements[0];

			bool TF = true;
			string parameternotValue = "";
			foreach (string parameter in paraInfor)
			{
				if (parameter == "") continue;
				string valuePara = element.LookupParameter(parameter).AsValueString();
				if (valuePara == null || valuePara == "")
					parameternotValue = parameternotValue + parameter + "|";
			}

			if (parameternotValue != "") TF = false;
			string filePath = GetInforOrtherFile.GetfileName(doc);
			var s = filePath.Split('\\');
			string fileName = s[s.Length - 1];

			string dateexcute = DateTime.Now.Year.ToString() + "-" + (DateTime.Now.Month < 10 ? "0" + DateTime.Now.Month.ToString() : DateTime.Now.Month.ToString())
				+ "-" + (DateTime.Now.Day < 10 ? "0" + DateTime.Now.Day.ToString() : DateTime.Now.Day.ToString());

			query = $"SELECT * FROM Addin_QAQC WHERE FileName = '{fileName}' AND ExecuteDay = '{dateexcute}'";
			string idFile = "";
			foreach (DataRow row in db.Get_DataTable(query).Rows)
			{
				idFile = row["ID"].ToString();
			}

			//CHECK EXITS
			query = $"UPDATE Addin_QAQC_GeneralResult SET ProjectInformation_Check= '{TF}', ProjectInformation_ParameterNotValue = '{parameternotValue}' WHERE Id_File = '{idFile}'";
			db.Execute_SQL(query);

			db.Close_DB_Connection();
			return TF;
		}

		//Check ProjectLocation đã được cập nhật chưa
		public bool CheckProjectLocation(Document doc)
		{
			string GeneralCheckName = "Default";
			DbConnect db = new DbConnect();
			string query = string.Format("SELECT * FROM Addin_QAQC_General_Check WHERE CheckSetName = '{0}'", GeneralCheckName);

			bool RunProjectLocation = false;
			string para = "";

			DataTable table = db.Get_DataTable(query);
			foreach (DataRow row in table.Rows)
			{
				RunProjectLocation = bool.Parse(row["Run_ProjectInfor"].ToString());
				para = row["ListParaProjectLocation"].ToString();
			}
			if (!RunProjectLocation) return true;

			var paraInfor = para.Split('|');

			List<Element> allElements = new FilteredElementCollector(doc)
				.WhereElementIsNotElementType()
				.Where(x => x.Category != null && x.Category.Name == "Project Base Point")
				.ToList();
			Element element = allElements[0];

			bool TF = true;
			string parameternotValue = "";
			foreach (string parameter in paraInfor)
			{
				if (parameter == "") continue;
				double valuePara = element.LookupParameter(parameter).AsDouble();
				if (valuePara == 0)
					parameternotValue = parameternotValue + parameter + "|";
			}

			if (parameternotValue != "") TF = false;
			string filePath = GetInforOrtherFile.GetfileName(doc);
			var s = filePath.Split('\\');
			string fileName = s[s.Length - 1];

			string dateexcute = DateTime.Now.Year.ToString() + "-" + (DateTime.Now.Month < 10 ? "0" + DateTime.Now.Month.ToString() : DateTime.Now.Month.ToString())
				+ "-" + (DateTime.Now.Day < 10 ? "0" + DateTime.Now.Day.ToString() : DateTime.Now.Day.ToString());

			query = $"SELECT * FROM Addin_QAQC WHERE FileName = '{fileName}' AND ExecuteDay = '{dateexcute}'";
			string idFile = "";
			foreach (DataRow row in db.Get_DataTable(query).Rows)
			{
				idFile = row["ID"].ToString();
			}

			//CHECK EXITS
			query = $"UPDATE Addin_QAQC_GeneralResult SET ProjectLocation_Check= '{TF}' WHERE Id_File = '{idFile}'";
			db.Execute_SQL(query);

			db.Close_DB_Connection();
			return TF;
		}

		//Check level and Grid đã dựng đúng workset chưa
		public int CheckWorksetLevelAndGrid(Document doc)
		{
			string GeneralCheckName = "Default";
			DbConnect db = new DbConnect();
			string query = string.Format("SELECT * FROM Addin_QAQC_General_Check WHERE CheckSetName = '{0}'", GeneralCheckName);

			bool RunFileName = false;
			string WorksetGrid = "";

			DataTable table = db.Get_DataTable(query);
			foreach (DataRow row in table.Rows)
			{
				RunFileName = bool.Parse(row["Run_GridWorkset"].ToString());
				WorksetGrid = row["WorksetGrid"].ToString();
			}

			if (!RunFileName) return 0;

			WorksetId id = new WorksetId(-1);
			try
			{
				IList<Workset> worksetList = new FilteredWorksetCollector(doc).OfKind(WorksetKind.UserWorkset).ToWorksets();
				id = worksetList.Where(x => x.Name == WorksetGrid).FirstOrDefault().Id;
			}
			catch
			{
			}
			List<Element> allElements = new FilteredElementCollector(doc)
				.WhereElementIsNotElementType()
				.Where(x => x.Category != null && (x.Category.Name == "Levels" || x.Category.Name == "Grids") && x.WorksetId != id)
				.ToList();

			bool TF = true;
			if (allElements.Count > 0) TF = false;

			string filePath = GetInforOrtherFile.GetfileName(doc);
			var s = filePath.Split('\\');
			string fileName = s[s.Length - 1];

			string dateexcute = DateTime.Now.Year.ToString() + "-" + (DateTime.Now.Month < 10 ? "0" + DateTime.Now.Month.ToString() : DateTime.Now.Month.ToString())
				+ "-" + (DateTime.Now.Day < 10 ? "0" + DateTime.Now.Day.ToString() : DateTime.Now.Day.ToString());

			query = $"SELECT * FROM Addin_QAQC WHERE FileName = '{fileName}' AND ExecuteDay = '{dateexcute}'";
			string idFile = "";
			foreach (DataRow row in db.Get_DataTable(query).Rows)
			{
				idFile = row["ID"].ToString();
			}

			//CHECK EXITS
			query = $"UPDATE Addin_QAQC_GeneralResult SET GridAndLevelWorkset_Check= '{TF}', GridAndLevelWrong_Count = '{allElements.Count}' WHERE Id_File = '{idFile}'";
			db.Execute_SQL(query);

			db.Close_DB_Connection();
			return allElements.Count;
		}

		//Check element dựng có đúng workset chưa
		public int CheckWrongElementWorksetLevelAndGrid(Document doc)
		{
			string GeneralCheckName = "Default";
			DbConnect db = new DbConnect();
			string query = string.Format("SELECT * FROM Addin_QAQC_General_Check WHERE CheckSetName = '{0}'", GeneralCheckName);

			bool Run = false;
			string WorksetGrid = "";

			DataTable table = db.Get_DataTable(query);
			foreach (DataRow row in table.Rows)
			{
				Run = bool.Parse(row["Run_WrongWorkSet"].ToString());
				WorksetGrid = row["WorksetGridElement"].ToString();
			}

			if (!Run) return 0;

			try
			{
				IList<Workset> worksetList = new FilteredWorksetCollector(doc).OfKind(WorksetKind.UserWorkset).ToWorksets();
				WorksetId id = worksetList.Where(x => x.Name == WorksetGrid).FirstOrDefault().Id;

				List<Element> allElements = new FilteredElementCollector(doc)
					.WhereElementIsNotElementType()
					.Where(x => x.Category != null && x.Category.Name != "Levels" && x.Category.Name != "Grids" && x.WorksetId == id)
					.ToList();

				bool TF = true;
				if (allElements.Count > 0) TF = false;

				string filePath = GetInforOrtherFile.GetfileName(doc);
				var s = filePath.Split('\\');
				string fileName = s[s.Length - 1];

				string dateexcute = DateTime.Now.Year.ToString() + "-" + (DateTime.Now.Month < 10 ? "0" + DateTime.Now.Month.ToString() : DateTime.Now.Month.ToString())
					+ "-" + (DateTime.Now.Day < 10 ? "0" + DateTime.Now.Day.ToString() : DateTime.Now.Day.ToString());

				query = $"SELECT * FROM Addin_QAQC WHERE FileName = '{fileName}' AND ExecuteDay = '{dateexcute}'";
				string idFile = "";
				foreach (DataRow row in db.Get_DataTable(query).Rows)
				{
					idFile = row["ID"].ToString();
				}

				//CHECK EXITS
				query = $"UPDATE Addin_QAQC_GeneralResult SET WrongElementWorkset_Check= '{TF}', WrongElementWorkset_Count = '{allElements.Count}' WHERE Id_File = '{idFile}'";
				db.Execute_SQL(query);
				db.Close_DB_Connection();
				return allElements.Count;
			}
			catch
			{
				string filePath = GetInforOrtherFile.GetfileName(doc);
				var s = filePath.Split('\\');
				string fileName = s[s.Length - 1];

				string dateexcute = DateTime.Now.Year.ToString() + "-" + (DateTime.Now.Month < 10 ? "0" + DateTime.Now.Month.ToString() : DateTime.Now.Month.ToString())
					+ "-" + (DateTime.Now.Day < 10 ? "0" + DateTime.Now.Day.ToString() : DateTime.Now.Day.ToString());

				query = $"SELECT * FROM Addin_QAQC WHERE FileName = '{fileName}' AND ExecuteDay = '{dateexcute}'";
				string idFile = "";
				foreach (DataRow row in db.Get_DataTable(query).Rows)
				{
					idFile = row["ID"].ToString();
				}

				//CHECK EXITS
				query = $"UPDATE Addin_QAQC_GeneralResult SET WrongElementWorkset_Check= '{true}', WrongElementWorkset_Count = '{0}' WHERE Id_File = '{idFile}'";
				db.Execute_SQL(query);
				db.Close_DB_Connection();
				return 0;
			}

		}
		#endregion

		#region Check Discipline
		private string disciplineChekSetName;
		public string DisciplineChekSetName
		{
			get
			{
				return disciplineChekSetName;
			}
			set
			{
				disciplineChekSetName = value;
				OnPropertyChanged("DisciplineChekSetName");
			}
		}

		public void DisciplineCheck(Document doc, FileInfor FI)
		{
			DbConnect db = new DbConnect();
			try
			{
				UtilsFilter utilsFilter = new UtilsFilter();
				//Tạo đối tượng Checkset rồi xử lý tiếp
				ObservableCollection<DisciplineCheck> disciplineChecks = new ObservableCollection<DisciplineCheck>();

				string query = $"SELECT * FROM Addin_QAQC_DesciplineCheck WHERE Name = '{DisciplineChekSetName}'";

				DataTable table = db.Get_DataTable(query);

				string IdCheckSet = "";
				foreach (DataRow row in table.Rows)
				{
					IdCheckSet = row["Id"].ToString();
				}

				query = $"SELECT * FROM Addin_QAQC_DesciplineCheckRow WHERE Id_CheckSet = '{IdCheckSet}' ORDER BY [Order], [Id] ASC";
				foreach (DataRow row in db.Get_DataTable(query).Rows)
				{
					disciplineChecks.Add(new DisciplineCheck
					{
						Id = int.Parse(row["Id"].ToString()),
						Name = row["CheckName"].ToString(),
						Description = row["Description"].ToString(),
						Failure_Message = row["Failure_Message"].ToString(),
						Check_Result = row["CheckResult"].ToString(),
						SetRun = bool.Parse(row["SetRun"].ToString()),
						IsImportant = bool.Parse(row["CheckImportant"].ToString()),
						RangeA = row["Range_A"].ToString(),
						RangeB = row["Range_B"].ToString(),
						RangeC = row["Range_C"].ToString(),
						RangeD = row["Range_D"].ToString(),
						ListParaResult = row["ParasInResult"].ToString()
					});
				}

				foreach (DisciplineCheck dcCheck in disciplineChecks)
				{
					if (dcCheck.SetRun == false) continue;
					query = $"SELECT * FROM Addin_QAQC_DesciplineFilter WHERE [Id_CheckRow] = '{dcCheck.Id}' ORDER BY [Id_Filter] ASC";
					ObservableCollection<DisciplineFilter> DesciplineCheckRowFilters = new ObservableCollection<DisciplineFilter>();
					table = db.Get_DataTable(query);
					foreach (DataRow row in table.Rows)
					{
						DesciplineCheckRowFilters.Add(new DisciplineFilter
						{
							ID = int.Parse(row["Id_Filter"].ToString()),
							Oparator = row["Oparator"].ToString(),
							Criteria = row["Criteria"].ToString(),
							Property = row["Property"].ToString(),
							Condition = row["Condition"].ToString(),
							Value = row["Value"].ToString()
						});
					}

					dcCheck.Filters = DesciplineCheckRowFilters;
				}


				//Chạy từng bộ check => Lọc ra từng nhóm check
				FI.FileCheckResults = new ObservableCollection<FileCheckResult>();

				foreach (DisciplineCheck dcCheck in disciplineChecks)
				{
					try
					{
						ObservableCollection<DisciplineFilter> DesciplineCheckRowFilters = dcCheck.Filters;

						ObservableCollection<FilterGroupModel> FilterGroup = new ObservableCollection<FilterGroupModel>();

						FilterGroupModel filterGroupModel = new FilterGroupModel();
						FilterGroupExclude filterGroupExclude = new FilterGroupExclude();
						ObservableCollection<FilterGroupAnd> filterGroupAndGroup = new ObservableCollection<FilterGroupAnd>();
						FilterGroupAnd filterGroupAnd = new FilterGroupAnd();
						bool OfExclude = false;

						foreach (DisciplineFilter Rule in DesciplineCheckRowFilters)
						{
							// Tạo nhóm mới
							if (Rule.Oparator == null || Rule.Oparator == "" || DesciplineCheckRowFilters.IndexOf(Rule) == 0)
							{
								//Tạo 1 nhóm mới
								FilterGroup.Add(new FilterGroupModel());
								OfExclude = false;

								//Tạo Group filter and và add id hàng đó
								filterGroupAndGroup.Add(new FilterGroupAnd() { Id = Rule.ID.ToString() });
							}
							else if (Rule.Oparator == "OR")
							{
								// Lấy Filter Group và gán mục Exclude nếu chạy tới OR
								filterGroupModel = FilterGroup[FilterGroup.Count - 1];
								filterGroupModel.FilterGroupExclude = filterGroupExclude;
								filterGroupModel.Filters = filterGroupAndGroup;

								//Tạo 1 nhóm mới
								FilterGroup.Add(new FilterGroupModel());
								OfExclude = false;

								//Tạo Group filter and và add id hàng đó
								filterGroupAndGroup = new ObservableCollection<FilterGroupAnd>();
								filterGroupAndGroup.Add(new FilterGroupAnd() { Id = Rule.ID.ToString() });

							}
							//Tạo 1 nhóm mới level 2
							else if (Rule.Oparator == "EXCLUDE")
							{
								//Tạo 1 nhóm ngoại trừ mới
								filterGroupModel = FilterGroup[FilterGroup.Count - 1];

								filterGroupExclude = new FilterGroupExclude();

								OfExclude = true;

								//Gán group and vì không dùng nữa
								filterGroupModel.Filters = filterGroupAndGroup;

								//Tạo và gán hàng đó vào group exclude mới
								filterGroupModel.FilterGroupExclude = filterGroupExclude;
								filterGroupAndGroup = new ObservableCollection<FilterGroupAnd>();
								filterGroupAndGroup.Add(new FilterGroupAnd() { Id = Rule.ID.ToString() });
							}
							//Add Filter
							else
							{
								filterGroupModel = FilterGroup[FilterGroup.Count - 1];
								filterGroupAndGroup.Add(new FilterGroupAnd() { Id = Rule.ID.ToString() });
							}
						}

						//kết thúc vòng lặp => Gán
						filterGroupModel = FilterGroup[FilterGroup.Count - 1];
						if (OfExclude)
						{
							filterGroupModel.FilterGroupExclude.Filters = filterGroupAndGroup;
						}
						else
						{
							filterGroupModel.Filters = filterGroupAndGroup;
						}

						ObservableCollection<FileCheckResult> FileCheckResults = new ObservableCollection<FileCheckResult>();

						List<ElementId> elementIds = new List<ElementId>();
						List<List<ElementId>> listResultFinal = new List<List<ElementId>>();

						foreach (FilterGroupModel Check in FilterGroup)
						{
							List<ElementId> listResult = new List<ElementId>();
							List<ElementId> listExclude = new List<ElementId>();
							// Chạy bộ chính
							List<string> Idfilter = Check.Filters.Select(x => x.Id).ToList();

							listResult = CheckGroupLevel3(doc, Idfilter, dcCheck.Id.ToString());

							//Chạy bộ exclude nếu có
							try
							{
								if (Check.FilterGroupExclude.Filters != null)
								{
									List<string> IdfilterExclude = Check.FilterGroupExclude.Filters.Select(x => x.Id).ToList();

									listExclude = CheckGroupLevel3(doc, IdfilterExclude, dcCheck.Id.ToString());
								}
							}
							catch { }

							listResult = listResult.Except(listExclude).ToList();

							listResultFinal.Add(listResult);
						}

						//Gộp các đk hoặc
						foreach (List<ElementId> Result in listResultFinal)
						{
							elementIds = elementIds.Union(Result).ToList();
						}

						//Tạo đối tượng kq bộ check mới
						FileCheckResult fileCheckResult = new FileCheckResult();
						fileCheckResult.Id = dcCheck.Id;
						fileCheckResult.Name = dcCheck.Name;
						fileCheckResult.Description = dcCheck.Description;
						fileCheckResult.Failure_Message = dcCheck.Failure_Message;
						fileCheckResult.IsImportant = dcCheck.IsImportant;
						fileCheckResult.ListParaResult = dcCheck.ListParaResult;
						string para1 = "";
						string para2 = "";
						string para3 = "";
						var str = fileCheckResult.ListParaResult.Split('|');
						try
						{
							if (str[0] == "" || str[0] == null)
							{
								para1 = "HB_ElementName";
							}
							else
							{
								para1 = str[0];
							}
							
						}
						catch
						{
							para1 = "HB_ElementName";
						}
						try
						{
							para2 = str[1];
							para3 = str[2];
						}
						catch
						{
						}
						fileCheckResult.Para1 = para1;
						fileCheckResult.Para2 = para2;
						fileCheckResult.Para3 = para3;

						//Set Range
						fileCheckResult.SetRange = CheckRange(elementIds, dcCheck);

						fileCheckResult.Elements = new ObservableCollection<ListElementResult>();

						List<ElementId> IdResult = new List<ElementId>();
						foreach (ElementId ElementId in elementIds)
						{
							Element e = doc.GetElement(ElementId);

							string Category = "";
							string Type = "";
							string Name = "";
							bool isType = e is ElementType;
							if (isType == true)
							{
								FilteredElementCollector finalCollector = (FilteredElementCollector)new FilteredElementCollector(doc).WhereElementIsNotElementType();

								e = finalCollector.ToList<Element>().Where(x => x.GetTypeId() == e.Id).FirstOrDefault();
								try
								{
									if (e.Category != null)
									{
										Category = e.Category.Name;
									}
								}
								catch
								{
									try
									{
										if (e is null)
										{
											IdResult.Add(ElementId);
											continue;
										}
										else
										{
											Category = e.Category.Name;
										}
									}
									catch { }
								}
								Type = utilsFilter.TypeName(doc, e);
								Name = UtilsFilter.GetFamilyName(e);
							}
							else
							{
								if (e.Category != null)
								{
									Category = e.Category.Name;
								}
								Type = utilsFilter.TypeName(doc, e);
								Name = UtilsFilter.GetFamilyName(e);
							}



							fileCheckResult.Elements.Add(new ListElementResult
							{
								Category = Category,
								Type = Type,
								Name = Name,
								ElementId = e!=null?e.Id: ElementId,
								Para1 = para1,
								Para2 = para2,
								Para3 = para3,
								Para1Value = utilsFilter.LookupParameterValue(e, para1, "String"),
								Para2Value = utilsFilter.LookupParameterValue(e, para2, "String"),
								Para3Value = utilsFilter.LookupParameterValue(e, para3, "String"),
							});
						}

						foreach (ElementId id in IdResult)
						{
							elementIds.Remove(id);
						}


						fileCheckResult.SetRange = CheckRange(elementIds, dcCheck);


						if (dcCheck.Check_Result == "Fail When No Matching Element Are Found")
						{
							fileCheckResult.ElementCount = fileCheckResult.Elements.Count;
							fileCheckResult.Result = fileCheckResult.Elements.Count > 0 ? true : false;
						}
						else if (dcCheck.Check_Result == "Fail When Matching Element Are Found")
						{
							fileCheckResult.ElementCount = fileCheckResult.Elements.Count;
							fileCheckResult.Result = fileCheckResult.Elements.Count > 0 ? false : true;
						}
						else if (dcCheck.Check_Result == "Count of Matching Element Only")
						{
							fileCheckResult.ElementCount = fileCheckResult.Elements.Count;
							fileCheckResult.Result = fileCheckResult.Elements.Count > 0 ? false : true;
						}
						else
						{
							fileCheckResult.ElementCount = fileCheckResult.Elements.Count;
							fileCheckResult.Result = fileCheckResult.Elements.Count > 0 ? false : true;
						}

						FI.FileCheckResults.Add(fileCheckResult);

						
					}
					catch { }
					
				}

				FI.FileCheckResultsDescription = FileCheckResultsDescription(FI.FileCheckResults);
				FI.FileGeneralResultsDescription = FileGeneralResultsDescription(FI);
				FI.FileResultsDescription = FileResultsDescription(FI);
			}
			catch (Exception ex)
			{
			}

			db.Close_DB_Connection();
		}

		//
		public List<ElementId> CheckGroupLevel3(Document doc, List<string> IDs,string IdCheck)
		{
			DbConnect db = new DbConnect();
			UtilsFilter utilsFilter = new UtilsFilter();
			List<ElementId> ListElements = new List<ElementId>();

			try
			{
				string query;

				foreach (string idFilter in IDs)
				{
					query = $"SELECT * FROM Addin_QAQC_DesciplineFilter WHERE Id_Filter = '{idFilter}' AND Id_CheckRow = '{IdCheck}' ORDER BY Id_Filter ASC";
					DataTable table = db.Get_DataTable(query);
					string Criteria = "BoLoc";
					string Property = "";
					string Condition = "";
					string Value = "";
					string ValueType = "";

					foreach (DataRow row in table.Rows)
					{
						Criteria = row["Criteria"].ToString();
						Property = row["Property"].ToString();
						Condition = row["Condition"].ToString();
						Value = row["Value"].ToString();

						double vld;
						bool IsDouble = double.TryParse(Value, out vld);

						int vli;
						bool IsInt = int.TryParse(Value, out vli);

						bool vlb = false;
						bool Isbool = bool.TryParse(Condition, out vlb);

						if (IsDouble == true || IsInt == true)
						{
							ValueType = "Double";
						}
						else if (Isbool == true)
						{
							ValueType = "Bool";
						}
						else
						{
							Regex rgcheck = new Regex($"^<(.+)>$");
							if (rgcheck.Match(Value).Success)
							{
								ValueType = "Parameter";
								Value = Value.Replace("<", "");
								Value = Value.Replace(">", "");
							}
							else
							{
								ValueType = "String";
							}
						}

						//Tạo Filter
						switch (Criteria)
						{
							case "CATEGORY": // Lọc theo Category
								ListElements = ListElements.Union(utilsFilter.FilterByCategory(doc, Property)).ToList();
								ListElements = ListElements.Distinct().ToList();
								break;
							case "FAMILY": // Lọc theo family
								if (Property == "Name")
								{
										ListElements = ListElements.Intersect(utilsFilter.FilterFamilyName(doc, Value, Condition,ListElements)).ToList();
										ListElements = ListElements.Distinct().ToList();
								}
								else //Property == 
								{
									bool IsInPlace = Condition == "=" ? true : false;
									if (IsInPlace)
									{
										ListElements = ListElements.Intersect(utilsFilter.FilterFamilyInPlace(doc, true)).ToList();
									}
									else
									{
										ListElements = ListElements.Except(utilsFilter.FilterFamilyInPlace(doc, true)).ToList();
									}

									ListElements = ListElements.Distinct().ToList();
								}
								break;
							case "LEVEL":

								break;
							case "API PARAMETER":
								ListElements = utilsFilter.FilterAPIParameter(doc, Property, Condition, Value, ValueType, ListElements, IDs, IdCheck);
								break;
							case "PARAMETER":
								ListElements = utilsFilter.FilterParameter(doc, Property, Condition, Value, ValueType, ListElements, IDs, IdCheck);
								break;
							case "ROOM":

								break;
							case "STRUCTURAL TYPE":
								if (ListElements.Count == 0)
								{
									ListElements = ListElements.Union(utilsFilter.FilterStructuralType(doc, Value)).ToList();
								}
								else
								{
									ListElements = ListElements.Intersect(utilsFilter.FilterStructuralType(doc, Value)).ToList();
								}
								break;
							case "WARNING":
								if (ListElements.Count == 0)
								{
									ListElements = ListElements.Union(utilsFilter.FilterWarning(doc, Value)).ToList();
								}
								else
								{
									ListElements = ListElements.Intersect(utilsFilter.FilterWarning(doc, Value)).ToList();
								}
								break;
							case "TYPE":
								if (ListElements.Count == 0)
								{
									ListElements = ListElements.Union(utilsFilter.FilterType(doc, Condition, Value, ListElements)).ToList();
								}
								else
								{
									ListElements = ListElements.Intersect(utilsFilter.FilterType(doc, Condition, Value, ListElements)).ToList();
								}
								break;
							case "TYPE OR INSTANCE":
								if (ListElements.Count == 0)
								{
									ListElements = ListElements.Union(utilsFilter.FilterTypeOrInstance(doc, Condition, Value, ListElements)).ToList();
								}
								else
								{
									ListElements = ListElements.Intersect(utilsFilter.FilterTypeOrInstance(doc, Condition, Value, ListElements)).ToList();
								}
								break;
							case "VIEW":
								if (ListElements.Count == 0)
								{
									ListElements = ListElements.Union(utilsFilter.FilterView(doc, Property, Condition, Value)).ToList();
								}
								else
								{
									ListElements = ListElements.Intersect(utilsFilter.FilterView(doc, Property, Condition, Value)).ToList();
								}
								break;
							case "WORKSET":
								if (ListElements.Count == 0)
								{
									ListElements = ListElements.Union(utilsFilter.FilterWorkset(doc, Property, Condition, Value)).ToList();
								}
								else
								{
									ListElements = ListElements.Intersect(utilsFilter.FilterWorkset(doc, Property, Condition, Value)).ToList();
								}
								break;
							case "LOCATION":
								if (ListElements.Count == 0)
								{
									ListElements = ListElements.Union(utilsFilter.FilterLocation(doc, Property, Condition, Value, ListElements)).ToList();
								}
								else
								{
									ListElements = ListElements.Intersect(utilsFilter.FilterLocation(doc, Property, Condition, Value, ListElements)).ToList();
								}
								break;
							case "BASE AND TOP LEVEL":
								if (ListElements.Count == 0)
								{
									ListElements = ListElements.Union(utilsFilter.FilterbaseLevel(doc, Property, Condition, Value, ListElements)).ToList();
								}
								else
								{
									ListElements = ListElements.Intersect(utilsFilter.FilterbaseLevel(doc, Property, Condition, Value, ListElements)).ToList();
								}
								break;
							case "IsMonitoringLinkElement":
								ListElements = ListElements.Intersect(utilsFilter.FilterIsMonitor(doc, Property, ListElements)).ToList();
								break;
							default:
								break;
						}
					}
				}

			}
			catch
			{
			}
			db.Close_DB_Connection();
			return ListElements;
		}

		public string CheckRange(List<ElementId> elementIds, DisciplineCheck dcCheck)
		{
			int elementcount = elementIds.Count;

			double sominA = 0;
			double somaxA = 0;
			//CheckA
			if (dcCheck.RangeA != "" && dcCheck.RangeA != null)
			{

				bool boolA = double.TryParse(dcCheck.RangeA, out somaxA);
				if (boolA)
				{
					if (elementcount <= somaxA) return "A";
				}
				else
				{
					Regex rg = new Regex(@"^(?<Somin>[0-9]+)-(?<Somax>[0-9]+)$");
						foreach (Match result in rg.Matches(dcCheck.RangeA))
						{
							sominA = double.Parse(result.Groups["Somin"].ToString());
							somaxA = double.Parse(result.Groups["Somax"].ToString());
						}

					if (elementcount >= sominA && elementcount <= somaxA) return "A";
				}
			}

			//CheckB
			double sominB = 0;
			double somaxB = 0;
			if (dcCheck.RangeB != "" && dcCheck.RangeB != null)
			{

				bool boolB = double.TryParse(dcCheck.RangeB, out somaxB);
				if (boolB)
				{
					if (elementcount <= somaxB) return "B";
				}
				else
				{
					Regex rg = new Regex(@"^(?<Somin>[0-9]+)-(?<Somax>[0-9]+)$");
					foreach (Match result in rg.Matches(dcCheck.RangeB))
					{
						sominB = double.Parse(result.Groups["Somin"].ToString());
						somaxB = double.Parse(result.Groups["Somax"].ToString());
					}

					if (elementcount >= sominB && elementcount <= somaxB) return "B";
				}
			}

			//CheckC
			double sominC = 0;
			double somaxC = 0;
			if (dcCheck.RangeC != "" && dcCheck.RangeC != null)
			{

				bool boolC = double.TryParse(dcCheck.RangeC, out somaxC);
				if (boolC)
				{
					if (elementcount <= somaxC) return "C";
				}
				else
				{
					Regex rg = new Regex(@"^(?<Somin>[0-9]+)-(?<Somax>[0-9]+)$");
					foreach (Match result in rg.Matches(dcCheck.RangeC))
					{
						sominC = double.Parse(result.Groups["Somin"].ToString());
						somaxC = double.Parse(result.Groups["Somax"].ToString());
					}

					if (elementcount >= sominC && elementcount <= somaxC) return "C";
					else if (elementcount > somaxC) return "D";
				}
			}
			//CheckD
			if (elementcount == 0)
				return "A";
			else
				return "C";
		}

		public string FileCheckResultsDescription(ObservableCollection<FileCheckResult> fileCheckResult)
		{
			int PassCount = fileCheckResult.Where(x => x.SetRange == "A" || x.SetRange == "B").Count();

			int ImportantCount = fileCheckResult.Where(x => x.SetRange != "A" && x.IsImportant == true).Count();

			return $"{PassCount*100/fileCheckResult.Count}% Pass & {ImportantCount} Mandatory Condition Fail";
		}

		public string FileGeneralResultsDescription(FileInfor FI)
		{
			int count = 0;
			string Di = "";

			DbConnect db = new DbConnect();

			string GeneralCheckName = "Default";
			string query = string.Format("SELECT * FROM Addin_QAQC_General_Check WHERE CheckSetName = '{0}'", GeneralCheckName);

			string ID = "1";
			DataTable table = db.Get_DataTable(query);
			foreach (DataRow row in table.Rows)
			{
				ID = row["ID"].ToString();
			}

			query = $"SELECT * FROM Addin_QAQC_General_Madatory WHERE [Id_General_Check] = '{ID}'";
			table = db.Get_DataTable(query);
			foreach (DataRow row in table.Rows)
			{
				string Criteria = row["Criteria"].ToString();
				double RangeA = double.Parse(row["RangeA"].ToString());
				double RangeB = double.Parse(row["RangeB"].ToString());
				double RangeC = double.Parse(row["RangeC"].ToString());
				switch (Criteria)
				{
					case "File Name":
						if (FI.File_name_Check == false)
						{ 
							count += 1;
							Di=Di == "" ?"File Name" : Di + ", File Name";
						}
						break;
					case "File Size":
						if (FI.FileSize > RangeB)
						{
							count += 1;
							Di = Di == "" ? "File Size" : Di + ", File Size";
						}
						break;
					case "Warrnings/Total Element":
						if ((FI.Warnings/FI.Total_Element)*100 > RangeB)
						{
							count += 1;
							Di = Di == "" ? "Warrnings/Total Element" : Di + ", Warrnings/Total Element";
						}
						break;
					case "Total Element":
						if (FI.Total_Element > RangeB)
						{
							count += 1;
							Di = Di == "" ? "Total Element" : Di + ", Total Element";
						}
						break;
					case "Purable_Elements":
						if (FI.Purable_Elements > RangeB)
						{
							count += 1;
							Di = Di == "" ? "Purable_Elements" : Di + ", Purable_Elements";
						}
						break;
					case "Model Group":
						if (FI.Model_Groups > RangeB)
						{
							count += 1;
							Di = Di == "" ? "Model Group" : Di + ", Model Group";
						}
						break;
					case "Detail Group":
						if (FI.Detail_Groups > RangeB)
						{
							count += 1;
							Di = Di == "" ? "Detail Group" : Di + ", Detail Group";
						}
						break;
					case "In Place Families":
						if (FI.In_Place_Families > RangeB)
						{
							count += 1;
							Di = Di == "" ? "In Place Families" : Di + ", In Place Families";
						}
						break;
					case "Duplicate Intances":
						if (FI.Duplicate_Intances > RangeB)
						{
							count += 1;
							Di = Di == "" ? "Duplicate Intances" : Di + ", Duplicate Intances";
						}
						break;
					case "View not on sheet":
						if (FI.Views > RangeB)
						{
							count += 1;
							Di = Di == "" ? "View not on sheet" : Di + ", View not on sheet";
						}
						break;
					case "Sheets":
						if (FI.Sheets > RangeB)
						{
							count += 1;
							Di = Di == "" ? "Sheets" : Di + ", Sheets";
						}
						break;
					case "Cad Import":
						if (FI.Cad_Imports > RangeB)
						{
							count += 1;
							Di = Di == "" ? "Cad Import" : Di + ", Cad Import";
						}
						break;
					case "Link Revit":
						if (FI.Link_Revit > RangeB)
						{
							count += 1;
							Di = Di == "" ? "Link Revit" : Di + ", Link Revit";
						}
						break;
					case "Link Cad":
						if (FI.Linked_Cad > RangeB)
						{
							count += 1;
							Di = Di == "" ? "Link Cad" : Di + ", Link Cad";
						}
						break;
					case "Worksets":
						if (FI.Worksets > RangeB)
						{
							count += 1;
							Di = Di == "" ? "Worksets" : Di + ", Worksets";
						}
						break;
					case "Project Infor":
						if (FI.ProjectInfor_Check == false)
						{
							count += 1;
							Di = Di == "" ? "Project Infor" : Di + ", Project Infor";
						}
						break;
					case "Project Location":
						if (FI.ProjectLocation_Check == false)
						{
							count += 1;
							Di = Di == "" ? "Project Location" : Di + ", Project Location";
						}
						break;
					case "Level and Grid on wrong workset":
						if (FI.Levels_Workset_Check > RangeB)
						{
							count += 1;
							Di = Di == "" ? "Level and Grid on wrong workset" : Di + ", Level and Grid on wrong workset";
						}
						break;
					case "Wrong element on grid workset":
						if (FI.WrongElement_Check > RangeB)
						{
							count += 1;
							Di = Di == "" ? "Wrong element on grid workset" : Di + ", Wrong element on grid workset";
						}
						break;
					default:
						break;

				}
			}
			if (count == 0)
			{
				return $"{count} Mandatory Condition Fail";
			}
			else
			{
				return $"{count} Mandatory Condition Fail ({Di})";
			}
		}

		public string FileResultsDescription(FileInfor FI)
		{
			string dk1 = FI.FileResultsDescription;
			string dk2 = FI.FileCheckResultsDescription;

			//Check dk 1
			int sl = 0;
			Regex rg = new Regex(@"^(?<Count>[0-9]+) (.)+$");
			foreach (Match result in rg.Matches(dk1))
			{
				sl = int.Parse(result.Groups["Count"].ToString());
			}
			if (sl > 0) return "FAIL";

			int sl1 = 0;
			int sl2 = 0;
			rg = new Regex(@"^(?<sl>[0-9]+)% (.)+ & (?<sl2>[0-9])+ (.)+$");
			foreach (Match result in rg.Matches(dk2))
			{
				sl1 = int.Parse(result.Groups["sl"].ToString());
				sl2 = int.Parse(result.Groups["sl2"].ToString());
			}

			if (sl2 > 0) return "FAIL";
			else if (sl1 < 80) return "FAIL";
			else return "PASS";
		}

		public void PutdataDriverQCModel(FileInfor FI)
		{
			string dk1 = FI.FileResultsDescription;
			string dk2 = FI.FileCheckResultsDescription;
			string license = "";
			//Check dk 1
			int sl = 0;
			Regex rg = new Regex(@"^(?<Count>[0-9]+) (.)+$");
			foreach (Match result in rg.Matches(dk1))
			{
				sl = int.Parse(result.Groups["Count"].ToString());
			}
			if (sl > 0) license = "FAIL";

			int sl1 = 0;
			int sl2 = 0;
			rg = new Regex(@"^(?<sl>[0-9]+)% (.)+ & (?<sl2>[0-9])+ (.)+$");
			foreach (Match result in rg.Matches(dk2))
			{
				sl1 = int.Parse(result.Groups["sl"].ToString());
				sl2 = int.Parse(result.Groups["sl2"].ToString());
			}

			if (sl2 > 0) license = "FAIL";
			else if (sl1 < 80) license = "FAIL";
			else license = "PASS";

			string User = System.Windows.Forms.SystemInformation.ComputerName;
			string FilePath = "";
			try
			{
				var modelPath = ActiveData.Document.GetWorksharingCentralModelPath();

				var centralServerPath = ModelPathUtils.ConvertModelPathToUserVisiblePath(modelPath);

				FilePath = centralServerPath.ToString(); ;
			}
			catch
			{
				FilePath = GetInforOrtherFile.GetfileName(ActiveData.Document);
			}

			string url = $@"https://script.google.com/macros/s/AKfycbw5iBPka1TFTTXDQZDs1_7Z-t1nCdjxZQs-28CDVw/exec?FilePath={FilePath}&FileName={FI.FileName}&User={User}&License={license}";
			HttpWebRequest req;
			HttpWebResponse res = null;
			req = (HttpWebRequest)WebRequest.Create(url);
			res = (HttpWebResponse)req.GetResponse();
			res.Close();
		}
		#endregion
	}
}
