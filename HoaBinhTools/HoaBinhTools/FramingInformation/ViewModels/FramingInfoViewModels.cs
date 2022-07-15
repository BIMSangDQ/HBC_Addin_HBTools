using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.IO;
using System.Linq;
using System.Windows;
using Autodesk.Revit.DB;
using Autodesk.Revit.DB.Structure;
using Autodesk.Revit.UI.Selection;
using HoaBinhTools.FramingInformation.Db;
using HoaBinhTools.FramingInformation.Models;
using HoaBinhTools.FramingInformation.Models.DrawSectionCanvas;
using HoaBinhTools.FramingInformation.Views;
using OfficeOpenXml;
using Utils;

namespace HoaBinhTools.FramingInformation.ViewModels
{
	public class FramingInfoViewModels : ViewModelBase
	{
		private List<Element> eleFramings { get; set; }
		public Document doc { get; set; } = ActiveData.Document;
		public RelayCommand btnOk { get; set; }
		public RelayCommand btnsaveStirrupInfos { get; set; }
		public RelayCommand btnAddStirrup { get; set; }
		public RelayCommand btnDelStirrup { get; set; }
		public RelayCommand btnsaveAddT1 { get; set; }
		public RelayCommand btnSaveSetting { get; set; }
		public RelayCommand btnUpdateData { get; set; }
		public RelayCommand ShowErroCmd { get; set; }
		public static ExcelPackage excelPackage { get; set; }
		FramingInfoView FormMain { get; set; }
		public static List<ElementId> UpdateElement { get; set; }
		public int CountBeamSystemsGroup { get; set; }
		public RelayCommand<IList> DGSelectionChangedCommand { get; private set; }
		public RelayCommand CloseCommand { get; set; }
		public RelayCommand<IList> btnRemove { get; set; }
		public RelayCommand<IList> btnRemoveZoomeView { get; set; }
		public RelayCommand<IList> ShowErrorCmd { get; set; }
		protected ObservableCollection<BeamSystemsGroup> systemFramings;
		public ObservableCollection<ErrorModels> listErro = null;
		public ObservableCollection<ErrorModels> ListErro
		{
			get
			{
				return listErro;
			}
			set
			{

				listErro = value; OnPropertyChanged(nameof(ListErro));
			}
		}

		#region Thay thế cho db
		public ObservableCollection<SStirrup> listSStirrup = null;
		public ObservableCollection<SStirrup> ListSStirrup
		{
			get
			{
				return listSStirrup;
			}
			set
			{

				listSStirrup = value; OnPropertyChanged(nameof(ListSStirrup));
			}
		}

		public ObservableCollection<Stirrup> listStirrup = null;
		public ObservableCollection<Stirrup> ListStirrup
		{
			get
			{
				return listStirrup;
			}
			set
			{

				listStirrup = value; OnPropertyChanged(nameof(ListStirrup));
			}
		}
		public ObservableCollection<DbAddT1> listAddT1 = null;
		public ObservableCollection<DbAddT1> ListAddT1
		{
			get
			{
				return listAddT1;
			}
			set
			{

				listAddT1 = value; OnPropertyChanged(nameof(ListAddT1));
			}
		}
		public ObservableCollection<DbAddT2> listAddT2 = null;
		public ObservableCollection<DbAddT2> ListAddT2
		{
			get
			{
				return listAddT2;
			}
			set
			{

				listAddT2 = value; OnPropertyChanged(nameof(ListAddT2));
			}
		}
		public ObservableCollection<DbAddB1> listAddB1 = null;
		public ObservableCollection<DbAddB1> ListAddB1
		{
			get
			{
				return listAddB1;
			}
			set
			{

				listAddB1 = value; OnPropertyChanged(nameof(ListAddB1));
			}
		}
		public ObservableCollection<DbAddB2> listAddB2 = null;
		public ObservableCollection<DbAddB2> ListAddB2
		{
			get
			{
				return listAddB2;
			}
			set
			{

				listAddB2 = value; OnPropertyChanged(nameof(ListAddB2));
			}
		}
		#endregion

		public ObservableCollection<BeamSystemsGroup> SystemFramings
		{
			get
			{
				return systemFramings;
			}
			set
			{
				this.systemFramings = value;

				this.OnPropertyChanged(nameof(SystemFramings));
			}

		}

		#region RebarDiameterList
		private List<string> rebarDiameter()
		{
			List<Element> _elems = new FilteredElementCollector(doc)
				.OfClass(typeof(RebarBarType))
				.ToList();

			List<string> BarDiameter = new List<string>();

			foreach (Element el in _elems)
			{
				RebarBarType rebar = el as RebarBarType;
				BarDiameter.Add(rebar.Name.ToString());
			}

			return BarDiameter;
		}

		public List<string> RebarDiameter
		{
			get
			{
				return rebarDiameter();
			}
		}
		#endregion

		#region thép đai

		private string stirrupDiameter;
		public string StirrupDiameter
		{
			get
			{
				return stirrupDiameter;
				this.OnPropertyChanged(nameof(StirrupDiameter));
			}
			set
			{
				this.stirrupDiameter = value;
				this.OnPropertyChanged(nameof(StirrupDiameter));
			}
		}


		private string kc_dai1;
		public string Kc_dai1
		{
			get
			{
				return kc_dai1;
				this.OnPropertyChanged(nameof(Kc_dai1));
			}
			set
			{
				this.kc_dai1 = value;
				this.OnPropertyChanged(nameof(Kc_dai1));
			}
		}

		private string kc_dai2;
		public string Kc_dai2
		{
			get
			{
				return kc_dai2;
				this.OnPropertyChanged(nameof(Kc_dai2));
			}
			set
			{
				this.kc_dai2 = value;
				this.OnPropertyChanged(nameof(Kc_dai2));
			}
		}

		private string kc_dai3;
		public string Kc_dai3
		{
			get
			{
				return kc_dai3;
				this.OnPropertyChanged(nameof(Kc_dai3));
			}
			set
			{
				this.kc_dai3 = value;
				this.OnPropertyChanged(nameof(Kc_dai3));
			}
		}
		#endregion

		#region SideBar

		private bool isCheckHorizontalC;
		public bool IsCheckHorizontalC
		{
			get
			{
				return isCheckHorizontalC;
			}
			set
			{
				this.isCheckHorizontalC = value;
			}
		}

		private int layerSide;
		public int LayerSide
		{
			get
			{
				return layerSide;
			}
			set
			{
				this.layerSide = value;
			}
		}

		private int countSideBar;
		public int CountSideBar
		{
			get
			{
				return countSideBar;
			}
			set
			{
				this.countSideBar = value;
			}
		}

		private string sideBar_Dia;
		public string SideBar_Dia
		{
			get
			{
				return sideBar_Dia;
			}
			set
			{
				this.sideBar_Dia = value;
			}
		}
		#endregion

		#region Thép đai tăng cường
		private string addStirrupDiameter;
		public string AddStirrupDiameter
		{
			get
			{
				return addStirrupDiameter;
			}
			set
			{
				this.addStirrupDiameter = value;
			}
		}

		private double kC_AddStirrup;
		public double KC_AddStirrup
		{
			get
			{
				return kC_AddStirrup;
			}
			set
			{
				this.kC_AddStirrup = value;
			}
		}

		private bool isCheckAdditionalStirrup;
		public bool IsCheckAdditionalStirrup
		{
			get
			{
				return isCheckAdditionalStirrup;
			}
			set
			{
				this.isCheckAdditionalStirrup = value;
			}
		}

		private bool isCheckMainStirrup;
		public bool IsCheckMainStirrup
		{
			get
			{
				return isCheckMainStirrup;
			}
			set
			{
				this.isCheckMainStirrup = value;
			}
		}
		#endregion

		#region T1_Rebar
		private int sL_T1;
		public int SL_T1
		{
			get
			{
				return sL_T1;
			}
			set
			{
				this.sL_T1 = value;
			}
		}
		private string topMainBarDia;
		public string TopMainBarDia
		{
			get
			{
				return topMainBarDia;
			}
			set
			{
				this.topMainBarDia = value;
			}
		}
		#endregion

		#region T2_Rebar
		private int sL_T2;
		public int SL_T2
		{
			get
			{
				return sL_T2;
			}
			set
			{
				this.sL_T2 = value;
			}
		}
		private string topMainBarDia_T2;
		public string TopMainBarDia_T2
		{
			get
			{
				return topMainBarDia_T2;
			}
			set
			{
				this.topMainBarDia_T2 = value;
			}
		}
		#endregion

		#region Add_T1
		private int count_AddT1_1;
		public int Count_AddT1_1
		{
			get
			{
				return count_AddT1_1;
			}
			set
			{
				this.count_AddT1_1 = value;
				this.OnPropertyChanged(nameof(Count_AddT1_1));
			}
		}
		private string addT1_1_Dia;
		public string AddT1_1_Dia
		{
			get
			{
				return addT1_1_Dia;
			}
			set
			{
				this.addT1_1_Dia = value;
				this.OnPropertyChanged(nameof(AddT1_1_Dia));
			}
		}

		private int count_AddT1_2;
		public int Count_AddT1_2
		{
			get
			{
				return count_AddT1_2;
			}
			set
			{
				this.count_AddT1_2 = value;
				this.OnPropertyChanged(nameof(Count_AddT1_2));
			}
		}
		private string addT1_2_Dia;
		public string AddT1_2_Dia
		{
			get
			{
				return addT1_2_Dia;
			}
			set
			{
				this.addT1_2_Dia = value;
				this.OnPropertyChanged(nameof(AddT1_2_Dia));
			}
		}

		private int count_AddT1_3;
		public int Count_AddT1_3
		{
			get
			{
				return count_AddT1_3;
			}
			set
			{
				this.count_AddT1_3 = value;
				this.OnPropertyChanged(nameof(Count_AddT1_3));
			}
		}
		private string addT1_3_Dia;
		public string AddT1_3_Dia
		{
			get
			{
				return addT1_3_Dia;
			}
			set
			{
				this.addT1_3_Dia = value;
				this.OnPropertyChanged(nameof(AddT1_3_Dia));
			}
		}
		#endregion

		#region Add_T2
		private int count_AddT2_1;
		public int Count_AddT2_1
		{
			get
			{
				return count_AddT2_1;
			}
			set
			{
				this.count_AddT2_1 = value;
				this.OnPropertyChanged(nameof(Count_AddT2_1));
			}
		}
		private string addT2_1_Dia;
		public string AddT2_1_Dia
		{
			get
			{
				return addT2_1_Dia;
			}
			set
			{
				this.addT2_1_Dia = value;
				this.OnPropertyChanged(nameof(AddT2_1_Dia));
			}
		}

		private int count_AddT2_2;
		public int Count_AddT2_2
		{
			get
			{
				return count_AddT2_2;
			}
			set
			{
				this.count_AddT2_2 = value;
				this.OnPropertyChanged(nameof(Count_AddT2_2));
			}
		}
		private string addT2_2_Dia;
		public string AddT2_2_Dia
		{
			get
			{
				return addT2_2_Dia;
			}
			set
			{
				this.addT2_2_Dia = value;
				this.OnPropertyChanged(nameof(AddT2_2_Dia));
			}
		}

		private int count_AddT2_3;
		public int Count_AddT2_3
		{
			get
			{
				return count_AddT2_3;
			}
			set
			{
				this.count_AddT2_3 = value;
				this.OnPropertyChanged(nameof(Count_AddT2_3));
			}
		}
		private string addT2_3_Dia;
		public string AddT2_3_Dia
		{
			get
			{
				return addT2_3_Dia;
			}
			set
			{
				this.addT2_3_Dia = value;
				this.OnPropertyChanged(nameof(AddT2_3_Dia));
			}
		}
		#endregion

		#region Add_B1
		private int count_AddB1_1;
		public int Count_AddB1_1
		{
			get
			{
				return count_AddB1_1;
			}
			set
			{
				this.count_AddB1_1 = value;
				this.OnPropertyChanged(nameof(Count_AddB1_1));
			}
		}
		private string addB1_1_Dia;
		public string AddB1_1_Dia
		{
			get
			{
				return addB1_1_Dia;
			}
			set
			{
				this.addB1_1_Dia = value;
				this.OnPropertyChanged(nameof(AddB1_1_Dia));
			}
		}

		private int count_AddB1_2;
		public int Count_AddB1_2
		{
			get
			{
				return count_AddB1_2;
			}
			set
			{
				this.count_AddB1_2 = value;
				this.OnPropertyChanged(nameof(Count_AddB1_2));
			}
		}
		private string addB1_2_Dia;
		public string AddB1_2_Dia
		{
			get
			{
				return addB1_2_Dia;
			}
			set
			{
				this.addB1_2_Dia = value;
				this.OnPropertyChanged(nameof(AddB1_2_Dia));
			}
		}

		private int count_AddB1_3;
		public int Count_AddB1_3
		{
			get
			{
				return count_AddB1_3;
			}
			set
			{
				this.count_AddB1_3 = value;
				this.OnPropertyChanged(nameof(Count_AddB1_3));
			}
		}
		private string addB1_3_Dia;
		public string AddB1_3_Dia
		{
			get
			{
				return addB1_3_Dia;
			}
			set
			{
				this.addB1_3_Dia = value;
				this.OnPropertyChanged(nameof(AddB1_3_Dia));
			}
		}
		#endregion

		#region Add_B2
		private int count_AddB2_1;
		public int Count_AddB2_1
		{
			get
			{
				return count_AddB2_1;
			}
			set
			{
				this.count_AddB2_1 = value;
				this.OnPropertyChanged(nameof(Count_AddB2_1));
			}
		}
		private string addB2_1_Dia;
		public string AddB2_1_Dia
		{
			get
			{
				return addB2_1_Dia;
			}
			set
			{
				this.addB2_1_Dia = value;
				this.OnPropertyChanged(nameof(AddB2_1_Dia));
			}
		}

		private int count_AddB2_2;
		public int Count_AddB2_2
		{
			get
			{
				return count_AddB2_2;
			}
			set
			{
				this.count_AddB2_2 = value;
				this.OnPropertyChanged(nameof(Count_AddB2_2));
			}
		}
		private string addB2_2_Dia;
		public string AddB2_2_Dia
		{
			get
			{
				return addB2_2_Dia;
			}
			set
			{
				this.addB2_2_Dia = value;
				this.OnPropertyChanged(nameof(AddB2_2_Dia));
			}
		}

		private int count_AddB2_3;
		public int Count_AddB2_3
		{
			get
			{
				return count_AddB2_3;
			}
			set
			{
				this.count_AddB2_3 = value;
				this.OnPropertyChanged(nameof(Count_AddB2_3));
			}
		}
		private string addB2_3_Dia;
		public string AddB2_3_Dia
		{
			get
			{
				return addB2_3_Dia;
			}
			set
			{
				this.addB2_3_Dia = value;
				this.OnPropertyChanged(nameof(AddB2_3_Dia));
			}
		}
		#endregion

		#region B2_Rebar
		private int sL_B2;
		public int SL_B2
		{
			get
			{
				return sL_B2;
			}
			set
			{
				this.sL_B2 = value;
			}
		}
		private string botMainBarDia_B2;
		public string BotMainBarDia_B2
		{
			get
			{
				return botMainBarDia_B2;
			}
			set
			{
				this.botMainBarDia_B2 = value;
			}
		}
		#endregion

		#region B1_Rebar
		private int sL_B1;
		public int SL_B1
		{
			get
			{
				return sL_B1;
			}
			set
			{
				this.sL_B1 = value;
			}
		}
		private string botMainBarDia;
		public string BotMainBarDia
		{
			get
			{
				return botMainBarDia;
			}
			set
			{
				this.botMainBarDia = value;
			}
		}
		#endregion

		#region SETTING

		#region ConcreteCover
		private string concreteCover;
		public string ConcreteCover
		{
			get
			{
				return concreteCover;
			}
			set
			{
				this.concreteCover = value;
			}
		}
		#endregion

		#region Stirrup
		private string stirrupDiaSet;
		public string StirrupDiaSet
		{
			get
			{
				return stirrupDiaSet;
			}
			set
			{
				this.stirrupDiaSet = value;
			}
		}

		private string stirrupSup;
		public string StirrupSup
		{
			get
			{
				return stirrupSup;
			}
			set
			{
				this.stirrupSup = value;
			}
		}

		private string stirrupSpan;
		public string StirrupSpan
		{
			get
			{
				return stirrupSpan;
			}
			set
			{
				this.stirrupSpan = value;
			}
		}

		#endregion

		#region HookType

		private List<string> hookName()
		{

			List<Element> _elems = new FilteredElementCollector(doc)
				.OfClass(typeof(RebarHookType))
				.ToList();

			List<string> RebarHookName = new List<string>();

			foreach (Element el in _elems)
			{
				RebarHookType rebarHookType = el as RebarHookType;
				RebarHookName.Add(rebarHookType.Name.ToString());
			}
			return RebarHookName;
		}
		public List<string> HookName
		{
			get
			{
				return hookName();
			}
		}

		private string stirrup_HookStart;
		public string Stirrup_HookStart
		{
			get
			{
				return stirrup_HookStart;
			}
			set
			{
				this.stirrup_HookStart = value;
			}
		}
		private string stirrup_HookEnd;
		public string Stirrup_HookEnd
		{
			get
			{
				return stirrup_HookEnd;
			}
			set
			{
				this.stirrup_HookEnd = value;
			}
		}
		private string link_HookStart;
		public string Link_HookStart
		{
			get
			{
				return link_HookStart;
			}
			set
			{
				this.link_HookStart = value;
			}
		}
		private string link_HookEnd;
		public string Link_HookEnd
		{
			get
			{
				return link_HookEnd;
			}
			set
			{
				this.link_HookEnd = value;
			}
		}

		#endregion

		#region LAP
		private string top_Anchorage;
		public string Top_Anchorage
		{
			get
			{
				return top_Anchorage;
			}
			set
			{
				this.top_Anchorage = value;
			}
		}

		private string bot_Anchorage;
		public string Bot_Anchorage
		{
			get
			{
				return bot_Anchorage;
			}
			set
			{
				this.bot_Anchorage = value;
			}
		}

		private string lap_Tension;
		public string Lap_Tension
		{
			get
			{
				return lap_Tension;
			}
			set
			{
				this.lap_Tension = value;
			}
		}

		private string lap_Comp;
		public string Lap_Comp
		{
			get
			{
				return lap_Comp;
			}
			set
			{
				this.lap_Comp = value;
			}
		}
		#endregion

		#region Tỉ lệ
		private string tL_top;
		public string TL_top
		{
			get
			{
				return tL_top;
			}
			set
			{
				this.tL_top = value;
			}
		}

		private string tL_bot;
		public string TL_bot
		{
			get
			{
				return tL_bot;
			}
			set
			{
				this.tL_bot = value;
			}
		}
		#endregion

		private bool isCheckCstirrup;
		public bool IsCheckCstirrup
		{
			get
			{
				return isCheckCstirrup;
			}
			set
			{
				this.isCheckCstirrup = value;
			}
		}
		#endregion

		#region Special Stirrup
		private bool isCStirrup;
		public bool IsCStirrup
		{
			get
			{
				return isCStirrup;
			}
			set
			{
				this.isCStirrup = value;
			}
		}

		private bool isSetAllSpan;
		public bool IsSetAllSpan
		{
			get
			{
				return isSetAllSpan;
			}
			set
			{
				this.isSetAllSpan = value;
			}
		}

		private int vitri1;
		public int Vitri1
		{
			get
			{
				return vitri1;
			}
			set
			{
				this.vitri1 = value;
			}
		}

		private int vitri2;
		public int Vitri2
		{
			get
			{
				return vitri2;
			}
			set
			{
				this.vitri2 = value;
			}
		}

		#endregion

		protected string elem_ID;

		public string Elem_ID
		{
			get
			{
				return elem_ID;
			}
			set
			{
				this.elem_ID = value;
				this.OnPropertyChanged(nameof(Elem_ID));
			}

		}

		protected string nameHost;

		public string NameHost
		{
			get
			{
				return nameHost;
			}
			set
			{
				this.nameHost = value;

				this.OnPropertyChanged(nameof(NameHost));
			}

		}

		public FramingInfoViewModels(List<List<Element>> refeFramings)
		{
			ListErro = new ObservableCollection<ErrorModels>();

			//Thay cho db
			ListSStirrup = new ObservableCollection<SStirrup>();

			ListStirrup = new ObservableCollection<Stirrup>();

			ListAddT1 = new ObservableCollection<DbAddT1>();

			ListAddT2 = new ObservableCollection<DbAddT2>();

			ListAddB2 = new ObservableCollection<DbAddB2>();

			ListAddB1 = new ObservableCollection<DbAddB1>();
			//
			FileInfo fileInfo = new FileInfo(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "\\Autodesk\\Revit\\Addins" + "\\" + ActiveData.Application.VersionNumber + "\\BIM_HBC\\DirectorySupport\\Library\\HB_Main.xlsm");

			excelPackage = new ExcelPackage(fileInfo);

			SystemFramings = (new MovableRestBeam(this)).GetBeamSystemsGroup(refeFramings);

			CountBeamSystemsGroup = SystemFramings.Count();
		}

		// Ham nay dung de khai bao delegate va khoi dong chay
		public void Run()
		{
			try
			{
				GetSetting();

				btnOk = new RelayCommand(ButtonOK);

				btnSaveSetting = new RelayCommand(BtnSaveSetting);

				btnsaveStirrupInfos = new RelayCommand(BtnsaveStirrupInfos);

				btnsaveAddT1 = new RelayCommand(BtnsaveAddT1);

				btnAddStirrup = new RelayCommand(BtnAddStirrup);

				btnDelStirrup = new RelayCommand(BtnDelStirrup);

				btnRemove = new RelayCommand<IList>(ButtonRemove);

				DGSelectionChangedCommand = new RelayCommand<IList>(DataGridSelectionChanged);

				btnRemoveZoomeView = new RelayCommand<IList>(para => ButtonRemoveZoomeView(para));

				CloseCommand = new RelayCommand(CloseFormMain);

				ShowErrorCmd = new RelayCommand<IList>(ShowError);

				IsCStirrup = true;

				IsSetAllSpan = true;

				FormMain = new FramingInfoView(this);

				FormMain.Show();

				if (ListErro.Count > 0)
				{
					if (FormMain.Erro.IsSelected != true)
					{
						FormMain.Erro.IsSelected = true;
					}
				}

				//PVC = new PreviewControl(doc, FramingInfoView3D.GetViewHB().Id)

				//{ IsManipulationEnabled = true };

				//FormMain.GridViewCotrol.Children.Add(PVC);

				//if (PVC != null)
				//{
				//    PVC.Dispose();
				//}


			}
			catch (Exception ex)
			{
				ListErro.Add(new ErrorModels() { ErrorID = ElementId.InvalidElementId, Category = "Loi toan cuc", InfoErro = ex.ToString() });
			}
		}

		public void ShowError(IList Select)
		{

			try
			{

				var Err = Select[0] as ErrorModels;

				if (Err != null && Err.ErrorID != ElementId.InvalidElementId)
				{
					ActiveData.Selection.SetElementIds(new List<ElementId> { Err.ErrorID });
				}
			}
			catch
			{

			}



		}

		private void CloseFormMain()
		{
			//FormMain.GridViewCotrol.Children.Remove(PVC);

			//FormMain.GridViewCotrol.Children.Clear();

			//if (PVC != null)
			//{
			//    PVC.Dispose();
			//}
		}


		public string ViewGroupID = null;

		private void DataGridSelectionChanged(IList Select)
		{
			try
			{

				var Movab = Select[0] as MovableRestBeam;

				var GroupID = Movab.GroupID;

				if (ViewGroupID != GroupID)
				{
					ViewGroupID = GroupID;

					List<ElementId> EleID = null;

					foreach (var BeamSys in SystemFramings)
					{
						if (GroupID == BeamSys.GroupID)
						{
							EleID = BeamSys.Movable.Select(e => e.Id).ToList();

							View view_HB = FramingInfoView3D.GetViewHB();

							ObservableOverrideColor(BeamSys.Movable);

							break;
						}
					}

					if (EleID != null)
					{
						FramingInfoView3D.GetViewHB().IsolateFraming(doc, EleID);
					}
				}
				ActiveData.Selection.SetElementIds(new List<ElementId> { Movab.Id });

				//Lấy lại thông tin nhịp cũ
				Elem_ID = Movab.Id.ToString();
				GanDB_Stirrup(Elem_ID);

				SectionStirrup section = new SectionStirrup();
				section.DrawSection(this, FormMain, Movab);
				section.DrawSection2(this, FormMain, Movab);
			}
			catch (Exception ex)
			{
			}
		}

		public void ObservableOverrideColor(ObservableCollection<MovableRestBeam> Movab)
		{
			View view_HB = FramingInfoView3D.GetViewHB();

			foreach (MovableRestBeam Mov in Movab)
			{
				if (Mov.Type == TypeAdjSupport.GT)
				{
					ElementOverridesModels.ColorElement(ActiveData.Document, view_HB, Mov.Id, 255, 0, 0);
				}
				if (Mov.Type == TypeAdjSupport.DP)
				{
					ElementOverridesModels.ColorElement(ActiveData.Document, view_HB, Mov.Id, 030, 011, 204);
				}
				if (Mov.Type == TypeAdjSupport.ND)
				{
					ElementOverridesModels.ColorElement(ActiveData.Document, view_HB, Mov.Id, 0, 255, 0);
				}
			}
		}


		public void OverrideColor(TypeAdjSupport Type, MovableRestBeam Movab)
		{
			if (CountBeamSystemsGroup > 0)
			{
				View view_HB = FramingInfoView3D.GetViewHB();

				if (Type == TypeAdjSupport.GT)
				{
					ElementOverridesModels.ColorElement(ActiveData.Document, view_HB, Movab.Id, 255, 0, 0);
				}
				if (Type == TypeAdjSupport.DP)
				{
					ElementOverridesModels.ColorElement(ActiveData.Document, view_HB, Movab.Id, 030, 011, 204);
				}
				if (Type == TypeAdjSupport.ND)
				{
					ElementOverridesModels.ColorElement(ActiveData.Document, view_HB, Movab.Id, 0, 255, 0);
				}

				var ViewID = Movab.GroupID;

				foreach (var BeamSys in SystemFramings)
				{
					if (ViewID == BeamSys.GroupID)
					{
						var EleID = BeamSys.Movable.Select(e => e.Id).ToList();

						if (EleID != null)
						{
							FramingInfoView3D.GetViewHB().IsolateFraming(doc, EleID);
						}
					}
				}
			}
			ActiveData.Selection.SetElementIds(new List<ElementId> { ElementId.InvalidElementId });
		}


		protected void ButtonRemoveZoomeView(IList SelectDataGrid)
		{
			FormMain.Hide();
			//FormMain.GridViewCotrol.Children.Remove(PVC);

			//FormMain.GridViewCotrol.Children.Clear();

			//if (PVC != null)
			//{
			//    PVC.Dispose();
			//}

			FramingInfoView3D.ActivewHB();

			Action action = new Action(() =>
			{
				try
				{
					using (Transaction transGroup = new Transaction(ActiveData.Document))
					{
						transGroup.Start("Update View");

						List<ElementId> ListHide = new List<ElementId>();

						var IDGroup = (SelectDataGrid[0] as MovableRestBeam).GroupID;

						var ListMovable = SelectDataGrid as IList<MovableRestBeam>;

						FramingInfoView3D.GetViewHB().IsolateFraming(doc, ListMovable.Select(e => e.Id).ToList());

						var ListDataGrid = ListMovable.Where(e => e.Type == TypeAdjSupport.ND).Select(e => e.Id);

						while (true)
						{
							try
							{
								Reference refeHide = ActiveData.Selection.PickObject(ObjectType.Element, new FilterCategoryUtils
								{
									FuncElement = e => e.Category.Id.IntegerValue == (int)BuiltInCategory.OST_StructuralFraming ||
														e.Category.Id.IntegerValue == (int)BuiltInCategory.OST_StructuralFoundation ||
														 e.Category.Id.IntegerValue == (int)BuiltInCategory.OST_Walls ||
														  e.Category.Id.IntegerValue == (int)BuiltInCategory.OST_StructuralFraming ||
														   e.Category.Id.IntegerValue == (int)BuiltInCategory.OST_StructuralColumns
								});

								var ElementHide = ActiveData.Document.GetElement(refeHide).Id;

								if (!ListDataGrid.Contains(ElementHide))
								{
									ListHide.Add(ElementHide);

									FramingInfoView3D.GetViewHB().HideElementTemporary(ElementHide);
								}
							}
							catch
							{
								break;
							}
						}

						var result = MessageBox.Show("Bạn có chắc chắn remove đối tượng này ", "Thông báo ", MessageBoxButton.YesNo, MessageBoxImage.Warning);

						if (ListHide.Count != 0)
						{
							if (result == MessageBoxResult.Yes)
							{
								try
								{
									var MoveEle = SystemFramings.Where(e => e.GroupID == IDGroup).First().Movable;

									foreach (var eleID in ListHide)
									{
										var movas = MoveEle.Where(e => e.Id == eleID);

										if (movas.Count() > 0)
										{
											SystemFramings.Where(e => e.GroupID == IDGroup).First().Movable.Remove(movas.First());
										}
									}

									UpdateInfo(IDGroup);
								}
								catch (Exception e)
								{
									MessageBox.Show(e.ToString());
								}
							}
						}
						else
						{
							// khôi phục lại 
						}

						//FormMain.GridViewCotrol.Children.Add(PVC);

						//FramingInfoView3D.CloseViewHB();

						FormMain.Show();

						transGroup.Commit();
					}

					FramingInfoView3D.ActivewHB();

				}
				catch (Exception ex)
				{
					MessageBox.Show(ex.ToString());
				}
			}
			);

			ExternalEventHandler.Instance.SetAction(action);

			ExternalEventHandler.Instance.Run();
		}


		protected void ButtonRemove(IList SelectDataGrid)
		{
			var IDs = ActiveData.Selection.GetElementIds();

			if (IDs.Count > 0)
			{
				//FormMain.Hide();

				//FormMain.GridViewCotrol.Children.Remove(PVC);

				//FormMain.GridViewCotrol.Children.Clear();

				//if (PVC != null)
				//{
				//    PVC.Dispose();
				//}
				Action action = new Action(() =>
				{
					try
					{
						using (Transaction transGroup = new Transaction(ActiveData.Document))
						{
							transGroup.Start("Update View");

							var ID = IDs.First();

							var GroupID = (SelectDataGrid[0] as MovableRestBeam).GroupID;

							foreach (var System in SystemFramings)
							{
								if (System.GroupID == GroupID)
								{
									var Sys = System.Movable.Where(e => e.Id == ID && e.Type != TypeAdjSupport.ND).First();

									FramingInfoView3D.GetViewHB().HideElementTemporary(Sys.Id);

									System.Movable.Remove(Sys);

									break;
								}
							}
							UpdateInfo(GroupID);

							//FormMain.GridViewCotrol.Children.Add(PVC);

							transGroup.Commit();

						}
					}
					catch (Exception ex)
					{
						MessageBox.Show(ex.ToString());
					}

					FormMain.Show();

				});


				ExternalEventHandler.Instance.SetAction(action);

				ExternalEventHandler.Instance.Run();
			}
			else
			{
				MessageBox.Show("Bạn Chưa Chọn Đối Tượng Nào");
			}
		}


		protected void UpdateInfo(string IDGroup)
		{
			try
			{
				SystemFramings = (new MovableRestBeam(this)).UpdataBeamSystemsGroup(IDGroup, SystemFramings);
			}
			catch (Exception ex)
			{
				ListErro.Add(new ErrorModels() { ErrorID = ElementId.InvalidElementId, Category = "Loi toan cuc", InfoErro = ex.ToString() });
			}

			FramingInfoView3D.ActivewHB();



		}


		protected void ButtonOK()
		{

			using (Transaction transGroup = new Transaction(ActiveData.Document))
			{

				Action action = new Action(() =>
				{

					//try
					//{
					var Ex = new ExcelManager();

					if (Ex.IsRevitRunning())
					{
						Ex.CloseRevitApp();
					}
					using (Transaction trans = new Transaction(ActiveData.Document))
					{
						trans.Start("Transaction Group");

						Ex.Exprot(this, doc);

						trans.Commit();

					}
				});

				ExternalEventHandler.Instance.SetAction(action);

				ExternalEventHandler.Instance.Run();

			}

		}

		#region db Stirrup
		private void BtnsaveStirrupInfos() //Gán thông tin thép đai theo nhịp
		{
			try
			{
				ProgressBarView BIMExport = new ProgressBarView();

				BIMExport.Show();

				if (Elem_ID != null)
				{
					//string SQL_String = string.Format("DELETE FROM Stirrup WHERE ElementID = '{0}'", Elem_ID);
					//LocalDb.Execute_SQL(SQL_String);
					#region xoá db cũ
					bool check = true;
					while (check == true)
					{
						int count = 0;
						foreach (Stirrup c in this.ListStirrup)
						{
							if (c.EleID == Elem_ID)
							{
								ListStirrup.Remove(c);
								check = true;
								count++;
								break;
							}
						}
						if (count == 0) { check = false; }
					}
					#endregion

					if (Kc_dai2 == null) Kc_dai2 = Kc_dai1;
					if (Kc_dai3 == null) Kc_dai3 = Kc_dai1;

					ListStirrup.Add(new Stirrup()
					{
						EleID = Elem_ID,
						Vitri = 1,
						Diameter = StirrupDiameter,
						Space = Kc_dai1
					});
					ListStirrup.Add(new Stirrup()
					{
						EleID = Elem_ID,
						Vitri = 2,
						Diameter = StirrupDiameter,
						Space = Kc_dai2
					});
					ListStirrup.Add(new Stirrup()
					{
						EleID = Elem_ID,
						Vitri = 3,
						Diameter = StirrupDiameter,
						Space = Kc_dai3
					});
				}
				else
				{
					FilteredElementCollector allElementsInView = new FilteredElementCollector(doc, doc.ActiveView.Id);
					IList elementsInView = (IList)allElementsInView.ToElements();

					foreach (Element e in elementsInView)
					{
						string id = e.Id.ToString();

						#region xoá db cũ
						bool check = true;
						while (check == true)
						{
							int count = 0;
							foreach (Stirrup c in this.ListStirrup)
							{
								if (c.EleID == id)
								{
									ListStirrup.Remove(c);
									check = true;
									count++;
									break;
								}
							}
							if (count == 0) { check = false; }
						}
						#endregion

						if (Kc_dai2 == null) Kc_dai2 = Kc_dai1;
						if (Kc_dai3 == null) Kc_dai3 = Kc_dai1;

						ListStirrup.Add(new Stirrup()
						{
							EleID = id,
							Vitri = 1,
							Diameter = StirrupDiameter,
							Space = Kc_dai1
						});
						ListStirrup.Add(new Stirrup()
						{
							EleID = id,
							Vitri = 2,
							Diameter = StirrupDiameter,
							Space = Kc_dai2
						});
						ListStirrup.Add(new Stirrup()
						{
							EleID = id,
							Vitri = 3,
							Diameter = StirrupDiameter,
							Space = Kc_dai3
						});

					}
				}

				BIMExport.Close();
			}
			catch (Exception e)
			{
			}

		}
		private void GanDB_Stirrup(string ELementId) //Getdb đã khai báo các nhịp cũ
		{
			foreach (Stirrup c in this.ListStirrup)
			{
				if (c.EleID == ELementId && c.Vitri == 1)
				{
					Kc_dai1 = c.Space.ToString();
					StirrupDiameter = c.Diameter;
				}
				else if (c.EleID == ELementId && c.Vitri == 2)
				{
					Kc_dai2 = c.Space.ToString();
					StirrupDiameter = c.Diameter;
				}
				else if (c.EleID == ELementId && c.Vitri == 3)
				{
					Kc_dai3 = c.Space.ToString();
					StirrupDiameter = c.Diameter;
				}
			}

			#region T1
			foreach (DbAddT1 span in this.ListAddT1)
			{
				if (span.Vitri.ToString() == "1" && span.EleID == ELementId)
				{
					Count_AddT1_1 = int.Parse(span.Count.ToString());
					AddT1_1_Dia = span.Diameter.ToString();

				}
				else if (span.Vitri.ToString() == "2" && span.EleID == ELementId)
				{
					Count_AddT1_2 = int.Parse(span.Count.ToString());
					AddT1_2_Dia = span.Diameter.ToString();
				}
				else if (span.Vitri.ToString() == "3" && span.EleID == ELementId)
				{
					Count_AddT1_3 = int.Parse(span.Count.ToString());
					AddT1_3_Dia = span.Diameter.ToString();
				}
			}
			#endregion
			#region T2
			foreach (DbAddT2 span in this.ListAddT2)
			{
				if (span.Vitri.ToString() == "1" && span.EleID == ELementId)
				{
					Count_AddT2_1 = int.Parse(span.Count.ToString());
					AddT2_1_Dia = span.Diameter.ToString();

				}
				else if (span.Vitri.ToString() == "2" && span.EleID == ELementId)
				{
					Count_AddT2_2 = int.Parse(span.Count.ToString());
					AddT2_2_Dia = span.Diameter.ToString();
				}
				else if (span.Vitri.ToString() == "3" && span.EleID == ELementId)
				{
					Count_AddT2_3 = int.Parse(span.Count.ToString());
					AddT2_3_Dia = span.Diameter.ToString();
				}
			}
			#endregion
			#region B2
			foreach (DbAddB2 span in this.ListAddB2)
			{
				if (span.Vitri.ToString() == "1" && span.EleID == ELementId)
				{
					Count_AddB2_1 = int.Parse(span.Count.ToString());
					AddB2_1_Dia = span.Diameter.ToString();

				}
				else if (span.Vitri.ToString() == "2" && span.EleID == ELementId)
				{
					Count_AddB2_2 = int.Parse(span.Count.ToString());
					AddB2_2_Dia = span.Diameter.ToString();
				}
				else if (span.Vitri.ToString() == "3" && span.EleID == ELementId)
				{
					Count_AddB2_3 = int.Parse(span.Count.ToString());
					AddB2_3_Dia = span.Diameter.ToString();
				}
			}
			#endregion
			#region B1
			foreach (DbAddB1 span in this.ListAddB1)
			{
				if (span.Vitri.ToString() == "1" && span.EleID == ELementId)
				{
					Count_AddB1_1 = int.Parse(span.Count.ToString());
					AddB1_1_Dia = span.Diameter.ToString();

				}
				else if (span.Vitri.ToString() == "2" && span.EleID == ELementId)
				{
					Count_AddB1_2 = int.Parse(span.Count.ToString());
					AddB1_2_Dia = span.Diameter.ToString();
				}
				else if (span.Vitri.ToString() == "3" && span.EleID == ELementId)
				{
					Count_AddB1_3 = int.Parse(span.Count.ToString());
					AddB1_3_Dia = span.Diameter.ToString();
				}
			}
			#endregion
		}

		#endregion

		#region db AddT1
		private void BtnsaveAddT1() //Gán thông tin thép gia cường T1
		{
			try
			{
				ProgressBarView BIMExport = new ProgressBarView();

				BIMExport.Show();

				if (Elem_ID != null)
				{
					#region AddT1
					#region xoá db cũ
					bool check = true;
					while (check == true)
					{
						int count = 0;
						foreach (DbAddT1 c in this.ListAddT1)
						{
							if (c.EleID == Elem_ID)
							{
								ListAddT1.Remove(c);
								check = true;
								count++;
								break;
							}
						}
						if (count == 0) { check = false; }
					}
					#endregion

					if (Count_AddT1_1 != null && Count_AddT1_1 != 0)
					{
						ListAddT1.Add(new DbAddT1()
						{
							EleID = Elem_ID,
							Vitri = 1,
							Diameter = AddT1_1_Dia,
							Count = Count_AddT1_1
						});
					}

					if (Count_AddT1_2 != null && Count_AddT1_2 != 0)
					{
						ListAddT1.Add(new DbAddT1()
						{
							EleID = Elem_ID,
							Vitri = 2,
							Diameter = AddT1_2_Dia,
							Count = Count_AddT1_2
						});
					}

					if (Count_AddT1_3 != null && Count_AddT1_3 != 0)
					{
						ListAddT1.Add(new DbAddT1()
						{
							EleID = Elem_ID,
							Vitri = 3,
							Diameter = AddT1_3_Dia,
							Count = Count_AddT1_3
						});
					}
					#endregion

					#region AddT2
					#region xoá db cũ
					check = true;
					while (check == true)
					{
						int count = 0;
						foreach (DbAddT2 c in this.ListAddT2)
						{
							if (c.EleID == Elem_ID)
							{
								ListAddT2.Remove(c);
								check = true;
								count++;
								break;
							}
						}
						if (count == 0) { check = false; }
					}
					#endregion

					if (Count_AddT2_1 != null && Count_AddT2_1 != 0)
					{
						ListAddT2.Add(new DbAddT2()
						{
							EleID = Elem_ID,
							Vitri = 1,
							Diameter = AddT2_1_Dia,
							Count = Count_AddT2_1
						});
					}

					if (Count_AddT2_2 != null && Count_AddT2_2 != 0)
					{
						ListAddT2.Add(new DbAddT2()
						{
							EleID = Elem_ID,
							Vitri = 2,
							Diameter = AddT2_2_Dia,
							Count = Count_AddT2_2
						});
					}

					if (Count_AddT2_3 != null && Count_AddT2_3 != 0)
					{
						ListAddT2.Add(new DbAddT2()
						{
							EleID = Elem_ID,
							Vitri = 3,
							Diameter = AddT2_3_Dia,
							Count = Count_AddT2_3
						});
					}
					#endregion

					#region AddB1
					#region xoá db cũ
					check = true;
					while (check == true)
					{
						int count = 0;
						foreach (DbAddB1 c in this.ListAddB1)
						{
							if (c.EleID == Elem_ID)
							{
								ListAddB1.Remove(c);
								check = true;
								count++;
								break;
							}
						}
						if (count == 0) { check = false; }
					}
					#endregion

					if (Count_AddB1_1 != null && Count_AddB1_1 != 0)
					{
						ListAddB1.Add(new DbAddB1()
						{
							EleID = Elem_ID,
							Vitri = 1,
							Diameter = AddB1_1_Dia,
							Count = Count_AddB1_1
						});
					}


					if (Count_AddB1_2 != null && Count_AddB1_2 != 0)
					{
						ListAddB1.Add(new DbAddB1()
						{
							EleID = Elem_ID,
							Vitri = 2,
							Diameter = AddB1_2_Dia,
							Count = Count_AddB1_2
						});
					}

					if (Count_AddB1_3 != null && Count_AddB1_3 != 0)
					{
						ListAddB1.Add(new DbAddB1()
						{
							EleID = Elem_ID,
							Vitri = 3,
							Diameter = AddB1_3_Dia,
							Count = Count_AddB1_3
						});
					}
					#endregion

					#region AddB2
					#region xoá db cũ
					check = true;
					while (check == true)
					{
						int count = 0;
						foreach (DbAddB2 c in this.ListAddB2)
						{
							if (c.EleID == Elem_ID)
							{
								ListAddB2.Remove(c);
								check = true;
								count++;
								break;
							}
						}
						if (count == 0) { check = false; }
					}
					#endregion

					if (Count_AddB2_1 != null && Count_AddB2_1 != 0)
					{
						ListAddB2.Add(new DbAddB2()
						{
							EleID = Elem_ID,
							Vitri = 1,
							Diameter = AddB2_1_Dia,
							Count = Count_AddB2_1
						});
					}

					if (Count_AddB2_2 != null && Count_AddB2_2 != 0)
					{
						ListAddB2.Add(new DbAddB2()
						{
							EleID = Elem_ID,
							Vitri = 2,
							Diameter = AddB2_2_Dia,
							Count = Count_AddB2_2
						});
					}

					if (Count_AddB2_3 != null && Count_AddB2_3 != 0)
					{
						ListAddB2.Add(new DbAddB2()
						{
							EleID = Elem_ID,
							Vitri = 3,
							Diameter = AddB2_3_Dia,
							Count = Count_AddB2_3
						});
					}
					#endregion
				}
				else
				{
					FilteredElementCollector allElementsInView = new FilteredElementCollector(doc, doc.ActiveView.Id);
					IList elementsInView = (IList)allElementsInView.ToElements();

					foreach (Element e in elementsInView)
					{
						string id = e.Id.ToString();

						#region AddT1
						#region xoá db cũ
						bool check = true;
						while (check == true)
						{
							int count = 0;
							foreach (DbAddT1 c in this.ListAddT1)
							{
								if (c.EleID == id)
								{
									ListAddT1.Remove(c);
									check = true;
									count++;
									break;
								}
							}
							if (count == 0) { check = false; }
						}
						#endregion

						if (Count_AddT1_1 != null && Count_AddT1_1 != 0)
						{
							ListAddT1.Add(new DbAddT1()
							{
								EleID = id,
								Vitri = 1,
								Diameter = AddT1_1_Dia,
								Count = Count_AddT1_1
							});
						}

						if (Count_AddT1_2 != null && Count_AddT1_2 != 0)
						{
							ListAddT1.Add(new DbAddT1()
							{
								EleID = id,
								Vitri = 2,
								Diameter = AddT1_2_Dia,
								Count = Count_AddT1_2
							});
						}

						if (Count_AddT1_3 != null && Count_AddT1_3 != 0)
						{
							ListAddT1.Add(new DbAddT1()
							{
								EleID = id,
								Vitri = 3,
								Diameter = AddT1_3_Dia,
								Count = Count_AddT1_3
							});
						}
						#endregion

						#region AddT2
						#region xoá db cũ
						check = true;
						while (check == true)
						{
							int count = 0;
							foreach (DbAddT2 c in this.ListAddT2)
							{
								if (c.EleID == id)
								{
									ListAddT2.Remove(c);
									check = true;
									count++;
									break;
								}
							}
							if (count == 0) { check = false; }
						}
						#endregion

						if (Count_AddT2_1 != null && Count_AddT2_1 != 0)
						{
							ListAddT2.Add(new DbAddT2()
							{
								EleID = id,
								Vitri = 1,
								Diameter = AddT2_1_Dia,
								Count = Count_AddT2_1
							});
						}

						if (Count_AddT2_2 != null && Count_AddT2_2 != 0)
						{
							ListAddT2.Add(new DbAddT2()
							{
								EleID = id,
								Vitri = 2,
								Diameter = AddT2_2_Dia,
								Count = Count_AddT2_2
							});
						}

						if (Count_AddT2_3 != null && Count_AddT2_3 != 0)
						{
							ListAddT2.Add(new DbAddT2()
							{
								EleID = id,
								Vitri = 3,
								Diameter = AddT2_3_Dia,
								Count = Count_AddT2_3
							});
						}
						#endregion

						#region AddB1
						#region xoá db cũ
						check = true;
						while (check == true)
						{
							int count = 0;
							foreach (DbAddB1 c in this.ListAddB1)
							{
								if (c.EleID == id)
								{
									ListAddB1.Remove(c);
									check = true;
									count++;
									break;
								}
							}
							if (count == 0) { check = false; }
						}
						#endregion

						if (Count_AddB1_1 != null && Count_AddB1_1 != 0)
						{
							ListAddB1.Add(new DbAddB1()
							{
								EleID = id,
								Vitri = 1,
								Diameter = AddB1_1_Dia,
								Count = Count_AddB1_1
							});
						}


						if (Count_AddB1_2 != null && Count_AddB1_2 != 0)
						{
							ListAddB1.Add(new DbAddB1()
							{
								EleID = id,
								Vitri = 2,
								Diameter = AddB1_2_Dia,
								Count = Count_AddB1_2
							});
						}

						if (Count_AddB1_3 != null && Count_AddB1_3 != 0)
						{
							ListAddB1.Add(new DbAddB1()
							{
								EleID = id,
								Vitri = 3,
								Diameter = AddB1_3_Dia,
								Count = Count_AddB1_3
							});
						}
						#endregion

						#region AddB2
						#region xoá db cũ
						check = true;
						while (check == true)
						{
							int count = 0;
							foreach (DbAddB2 c in this.ListAddB2)
							{
								if (c.EleID == id)
								{
									ListAddB2.Remove(c);
									check = true;
									count++;
									break;
								}
							}
							if (count == 0) { check = false; }
						}
						#endregion

						if (Count_AddB2_1 != null && Count_AddB2_1 != 0)
						{
							ListAddB2.Add(new DbAddB2()
							{
								EleID = id,
								Vitri = 1,
								Diameter = AddB2_1_Dia,
								Count = Count_AddB2_1
							});
						}

						if (Count_AddB2_2 != null && Count_AddB2_2 != 0)
						{
							ListAddB2.Add(new DbAddB2()
							{
								EleID = id,
								Vitri = 2,
								Diameter = AddB2_2_Dia,
								Count = Count_AddB2_2
							});
						}

						if (Count_AddB2_3 != null && Count_AddB2_3 != 0)
						{
							ListAddB2.Add(new DbAddB2()
							{
								EleID = id,
								Vitri = 3,
								Diameter = AddB2_3_Dia,
								Count = Count_AddB2_3
							});
						}
						#endregion

					}
				}

				BIMExport.Close();

			}
			catch (Exception e)
			{
			}

		}
		private void GanDB_AddT1(string ELementId) //Getdb đã khai báo các nhịp cũ
		{
			//string SQL_String = string.Format("SELECT * FROM AddT1 WHERE ElementID = '{0}' ORDER BY Vitri", ELementId);
			//System.Data.DataTable SID = LocalDb.Get_DataTable(SQL_String);

			//foreach (System.Data.DataRow span in SID.Rows)
			//{
			//	if (span["Vitri"].ToString() == "1")
			//	{
			//		Count_AddT1_1 = int.Parse(span["Count"].ToString());
			//		AddT1_1_Dia = span["Diameter"].ToString();

			//	}
			//	else if (span["Vitri"].ToString() == "2")
			//	{
			//		Count_AddT1_2 = int.Parse(span["Count"].ToString());
			//		AddT1_2_Dia = span["Diameter"].ToString();
			//	}
			//	else if (span["Vitri"].ToString() == "3")
			//	{
			//		Count_AddT1_3 = int.Parse(span["Count"].ToString());
			//		AddT1_3_Dia = span["Diameter"].ToString();
			//	}
			//}
		}

		#endregion

		protected void BtnSaveSetting() //Save Setting
		{
			Save.Default.ConcreteCover = this.concreteCover;
			Save.Default.StirrupDia = this.stirrupDiaSet;
			Save.Default.StirrupSup = this.stirrupSup;
			Save.Default.StirrupSpan = this.stirrupSpan;
			Save.Default.Stirrup_HookStart = this.stirrup_HookStart;
			Save.Default.Stirrup_HookEnd = this.stirrup_HookEnd;
			Save.Default.Link_HookStart = this.link_HookStart;
			Save.Default.Link_HookEnd = this.link_HookEnd;
			Save.Default.Anchorage_Top = this.top_Anchorage;
			Save.Default.Anchorage_Bot = this.bot_Anchorage;
			Save.Default.Lap_Tension = this.lap_Tension;
			Save.Default.Lap_Comp = this.lap_Comp;
			Save.Default.TL_top = this.tL_top;
			Save.Default.TL_bot = this.tL_bot;
			Save.Default.AddStirup_Dia = this.addStirrupDiameter;
			Save.Default.AddStirrup_Spc = this.kC_AddStirrup.ToString();
			if (this.isCheckMainStirrup == true)
			{
				Save.Default.Is_SameAsMain = 0;
			}
			else
			{
				Save.Default.Is_SameAsMain = 1;
			}

			Save.Default.Is_CStirrup = this.isCheckCstirrup;
			Save.Default.Save();
		}

		private void GetSetting()
		{
			this.concreteCover = Save.Default.ConcreteCover;
			this.stirrupDiaSet = Save.Default.StirrupDia;
			this.stirrupSup = Save.Default.StirrupSup;
			this.stirrupSpan = Save.Default.StirrupSpan;
			this.stirrup_HookStart = Save.Default.Stirrup_HookStart;
			this.stirrup_HookEnd = Save.Default.Stirrup_HookEnd;
			this.link_HookStart = Save.Default.Link_HookStart;
			this.link_HookEnd = Save.Default.Link_HookEnd;
			this.top_Anchorage = Save.Default.Anchorage_Top;
			this.bot_Anchorage = Save.Default.Anchorage_Bot;
			this.lap_Tension = Save.Default.Lap_Tension;
			this.lap_Comp = Save.Default.Lap_Comp;
			this.tL_top = Save.Default.TL_top;
			this.tL_bot = Save.Default.TL_bot;
			this.addStirrupDiameter = Save.Default.AddStirup_Dia;
			this.kC_AddStirrup = double.Parse(Save.Default.AddStirrup_Spc);
			if (Save.Default.Is_SameAsMain == 0)
			{
				this.isCheckMainStirrup = true;
				//this.isCheckAdditionalStirrup = false;
			}
			else
			{
				this.isCheckMainStirrup = false;
				//this.IsCheckAdditionalStirrup = true;
			}

			this.isCheckCstirrup = Save.Default.Is_CStirrup;
		}

		protected void BtnAddStirrup()
		{
			try
			{
				ProgressBarView BIMExport = new ProgressBarView();

				BIMExport.Show();

				string type = this.IsCStirrup == true ? "DaiC" : "Dai";

				if (this.IsSetAllSpan == false)
				{
					ListSStirrup.Add(new SStirrup() { EleID = Elem_ID, Type = type, Vitri1 = vitri1, Vitri2 = vitri2 });

				}
				else
				{
					FilteredElementCollector allElementsInView = new FilteredElementCollector(doc, doc.ActiveView.Id);
					IList elementsInView = (IList)allElementsInView.ToElements();

					foreach (Element e in elementsInView)
					{
						string id = e.Id.ToString();
						ListSStirrup.Add(new SStirrup() { EleID = id, Type = type, Vitri1 = vitri1, Vitri2 = vitri2 });
					}
				}

				SectionStirrup section = new SectionStirrup();
				section.DrawC(this, FormMain, Elem_ID);
				section.DrawV(this, FormMain, Elem_ID);

				BIMExport.Close();

			}
			catch (Exception e)
			{
				throw;
			}
		}

		protected void BtnDelStirrup()
		{
			try
			{
				string type = this.IsCStirrup == true ? "DaiC" : "Dai";
				if (this.IsSetAllSpan == false)
				{
					foreach (SStirrup c in this.ListSStirrup)
					{
						if (c.EleID == Elem_ID &&
							c.Type == type &&
							c.Vitri1 == vitri1 &&
							c.Vitri2 == vitri2)
						{
							ListSStirrup.Remove(c);
							break;
						}
					}
				}
				else
				{
					FilteredElementCollector allElementsInView = new FilteredElementCollector(doc, doc.ActiveView.Id);
					IList elementsInView = (IList)allElementsInView.ToElements();

					foreach (Element e in elementsInView)
					{
						string id = e.Id.ToString();
						foreach (SStirrup c in this.ListSStirrup)
						{
							if (c.EleID == id &&
								c.Type == type &&
								c.Vitri1 == vitri1 &&
								c.Vitri2 == vitri2)
							{
								ListSStirrup.Remove(c);
								break;
							}
						}
					}
				}

				//xóa hình vẽ
				string ten = this.isCStirrup == true ? "DaiC_" + this.vitri1 : "Dai_" + this.vitri1 + "_" + this.vitri2;

				List<UIElement> itemsToremove = new List<UIElement>();
				foreach (UIElement ui in FormMain.myCanvas.Children)
				{
					if (ui.GetType().ToString() == "System.Windows.Shapes.Line")
					{
						System.Windows.Shapes.Line L = (System.Windows.Shapes.Line)ui;
						if (L.Name.IndexOf(ten) >= 0)
						{
							itemsToremove.Add(L);

						}
					}
					else if (ui.GetType().ToString() == "System.Windows.Shapes.Ellipse")
					{
						System.Windows.Shapes.Ellipse L = (System.Windows.Shapes.Ellipse)ui;
						if (L.Name.IndexOf(ten) >= 0)
						{
							itemsToremove.Add(L);
						}
					}
				}

				foreach (UIElement ui in itemsToremove)
				{
					FormMain.myCanvas.Children.Remove(ui);
				}
			}
			catch (Exception e)
			{
				throw;
			}
		}


	}

}




