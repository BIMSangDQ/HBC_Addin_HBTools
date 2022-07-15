using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.Windows;
using Autodesk.Revit.DB;
using Autodesk.Revit.DB.Visual;
using Autodesk.Revit.UI.Selection;
using HoaBinhTools.SynchronizedData.Converters;
using HoaBinhTools.SynchronizedData.Db;
using HoaBinhTools.SynchronizedData.Models;
using HoaBinhTools.SynchronizedData.Models.CenterData;
using HoaBinhTools.SynchronizedData.Models.GroupFileModel;
using HoaBinhTools.SynchronizedData.Models.MaterialModel;
using HoaBinhTools.SynchronizedData.Models.WallTypeModel;
using HoaBinhTools.SynchronizedData.Views;
using Utils;

namespace HoaBinhTools.SynchronizedData.ViewModels
{
	public class SynchronizedDataViewModels : ViewModelBase
	{
		#region Properties
		private List<string> categories;
		public List<string> Categories
		{
			get
			{
				return new List<string>()
				{
				"Walls",
				"Floors",
				"Materials" };
			}
			set
			{
				categories = value;
			}
		}
		public string CategoryName { get; set; }

		private List<string> types;
		public List<string> Types
		{
			get
			{
				return types;
			}
			set
			{
				types = value;
			}
		}
		private string selectType;
		public string SelectType
		{
			get
			{
				return selectType;
			}
			set
			{
				selectType = value;
				OnPropertyChanged(nameof(SelectType));
			}
		}

		public List<Material> ListMaterial { get; set; }
		public List<WallType> WallTypes { get; set; }

		public List<FloorType> FloorTypes { get; set; }

		private WallType wallType = null;
		public WallType WallType
		{
			get { return wallType; }
			set { this.wallType = value; }
		}

		private WallTypeItem wallTypeItem;
		public WallTypeItem WallTypeItem
		{
			get { return wallTypeItem; }
			set { this.wallTypeItem = value; }
		}

		private CurtainWallItem curtainWallItem;
		public CurtainWallItem CurtainWallItem
		{
			get { return curtainWallItem; }
			set { this.curtainWallItem = value; }
		}

		private FloorTypeItem floorTypeItem;
		public FloorTypeItem FloorTypeItem
		{
			get { return floorTypeItem; }
			set { this.floorTypeItem = value; }
		}

		private MaterialModel materialModel;
		public MaterialModel MaterialModel
		{
			get { return materialModel; }
			set { this.materialModel = value; }
		}
		public List<MaterialItem> Materials { get; set; }

		private string newGroupName;
		public string NewGroupName
		{
			get { return newGroupName; }
			set { this.newGroupName = value; }
		}

		private GroupFileModel currentGroup;
		public GroupFileModel CurrentGroup
		{
			get { return currentGroup; }
			set
			{
				this.currentGroup = value;
				OnPropertyChanged(nameof(CurrentGroup));
			}
		}

		private ObservableCollection<FileModel> fileModelList = null;
		public ObservableCollection<FileModel> FileList
		{
			get { return fileModelList; }
			set
			{
				this.fileModelList = value;
				OnPropertyChanged(nameof(FileList));
			}
		}

		private ObservableCollection<CenterData> centerData = null;
		public ObservableCollection<CenterData> CenterDb
		{
			get { return centerData; }
			set
			{
				this.centerData = value;
				OnPropertyChanged(nameof(CenterDb));
			}
		}

		private List<GroupFileModel> groups;
		public List<GroupFileModel> GroupsList
		{
			get { return groups; }
			set
			{
				this.groups = value;
				OnPropertyChanged(nameof(GroupsList));
			}
		}

		private bool isCheckAll;
		public bool IsCheckAll
		{
			get { return isCheckAll; }
			set
			{
				this.isCheckAll = value;
				OnPropertyChanged(nameof(IsCheckAll));
			}
		}

		public RelayCommand btnOk { get; set; }
		public RelayCommand btnPick { get; set; }
		public RelayCommand btnCreate { get; set; }
		public RelayCommand btnCreateGroup { get; set; }
		public RelayCommand btnGetFilePath { get; set; }
		public RelayCommand btnDeleteFileFromGroup { get; set; }
		public RelayCommand btnChangeGroups { get; set; }
		public RelayCommand btnEditGroup { get; set; }
		public RelayCommand btnTabChange { get; set; }
		public RelayCommand btnCheckAll { get; set; }
		public RelayCommand btnUnCheckAll { get; set; }
		public RelayCommand btnChangeCategory { get; set; }

		Main wd1 { get; set; }
		#endregion
		public void Execute()
		{

			WallTypes = new FilteredElementCollector(ActiveData.Document)
			.OfClass(typeof(WallType))
			.Cast<WallType>()
			.ToList();

			FloorTypes = new FilteredElementCollector(ActiveData.Document)
			.OfClass(typeof(FloorType))
			.Cast<FloorType>()
			.ToList();

			ListMaterial = new FilteredElementCollector(ActiveData.Document)
			.OfClass(typeof(Material))
			.Cast<Material>()
			.ToList();

			Materials = new List<MaterialItem>();
			foreach (Material m in ListMaterial)
			{
				MaterialItem materialItem = new MaterialItem(m.Name, m.Id);
				Materials.Add(materialItem);
			}

			FileList = new ObservableCollection<FileModel>();

			GroupsList = new List<GroupFileModel>();
			GroupsList = GetListGroup();

			IsCheckAll = true;

			CenterDb = new ObservableCollection<CenterData>();

			btnOk = new RelayCommand(CopyWalltype);
			btnPick = new RelayCommand(Pick2Select);
			btnCreate = new RelayCommand(Download);
			btnCreateGroup = new RelayCommand(CreateGroup);
			btnGetFilePath = new RelayCommand(AddFile2Group);
			btnChangeGroups = new RelayCommand(SelectGroup);
			btnEditGroup = new RelayCommand(EditGroup);
			btnTabChange = new RelayCommand(WindowResize);
			btnCheckAll = new RelayCommand(CheckAllCenter);
			btnUnCheckAll = new RelayCommand(UnCheckAllCenter);
			btnDeleteFileFromGroup = new RelayCommand(DelFileFromGroup);
			btnChangeCategory = new RelayCommand(SelectCategory);
			wd1 = new Main(this);
			wd1.Show();
		}

		#region Window
		public void WindowResize()
		{
			string st = wd1.TabMain.SelectedItem.ToString();
			st = st.Replace("System.Windows.Controls.TabItem Header:", "");
			st = st.Replace("Content:", "");
			st = st.Replace(" ", "");

			if (st == "Synchronized")
			{
				wd1.Width = 700;
			}
			else
			{ wd1.Width = 300; }
		}

		#endregion

		#region Method Group
		//Select Category
		public void SelectCategory()
		{
			List<string> categories = new List<string>();

			switch (CategoryName)
			{
				case "Walls":
					categories = new List<string>();
					foreach (WallType wt in WallTypes)
					{
						categories.Add(wt.Name);
					}
					break;
				case "Floors":
					categories = new List<string>();
					foreach (FloorType ft in FloorTypes)
					{
						categories.Add(ft.Name);
					}
					break;
				default:
					categories = new List<string>();
					foreach (Material mat in ListMaterial)
					{
						categories.Add(mat.Name);
					}
					break;
			}
			Types = categories;
			wd1.Type.ItemsSource = Types;
		}
		//Create Group File
		public void CreateGroup()
		{
			DbConnect db = new DbConnect();

			//Check tên group
			string query = string.Format("SELECT * FROM Addin_GroupData WHERE Group_Name = '{0}'", NewGroupName);
			DataTable table = db.Get_DataTable(query);
			foreach (DataRow row in table.Rows)
			{
				if (row["Group_Name"].ToString() == NewGroupName)
				{
					MessageBox.Show("The group name already exists", "Revit SynchronizedData", MessageBoxButton.OK, MessageBoxImage.Exclamation);
					db.Close_DB_Connection();
					return;
				}
			}

			//Check số lượng file
			if (FileList.Count <= 0)
			{
				MessageBox.Show("Chọn nhiều hơn 1 file để tạo Group", "Revit SynchronizedData", MessageBoxButton.OK, MessageBoxImage.Exclamation);
				db.Close_DB_Connection();
				return;
			}

			//Add file hiện hành
			DbUtils dbUtils = new DbUtils();
			FileList.Add(dbUtils.GetCurrentFileModel());
			//Tạo list file
			foreach (FileModel fm in FileList)
			{
				query = string.Format("SELECT * FROM Addin_FileData WHERE File_Path = '{0}'", fm.File_Path);
				table = db.Get_DataTable(query);
				if (table.Rows.Count == 0)
				{
					query = string.Format("INSERT INTO Addin_FileData (File_Path, File_Name) VALUES('{0}','{1}'); ", fm.File_Path, fm.File_Name);
					db.Execute_SQL(query);
				}
			}

			FileList.Add(new FileModel());
			//Tạo chuỗi id File
			string FileIds = "";
			foreach (FileModel fm in FileList)
			{
				query = string.Format("SELECT * FROM Addin_FileData WHERE File_Path = '{0}'", fm.File_Path);
				table = db.Get_DataTable(query);
				foreach (DataRow row in table.Rows)
				{
					FileIds = FileIds + row["Id_File"].ToString() + ".";
				}
			}

			//Tạo Group
			query = string.Format("INSERT INTO Addin_GroupData (Group_Name,FileIds) VALUES('{0}','{1}'); ", NewGroupName, FileIds);
			db.Execute_SQL(query);
			db.Close_DB_Connection();

			GroupsList = new List<GroupFileModel>();
			GroupsList = GetListGroup();

			MessageBox.Show("Successful group creation!", "Revit SynchronizedData", MessageBoxButton.OK, MessageBoxImage.Information);
		}

		//Select File
		public void AddFile2Group()
		{
			Microsoft.Win32.OpenFileDialog dlg = new Microsoft.Win32.OpenFileDialog();

			dlg.Title = "Select Revit file to add Group";
			dlg.Multiselect = true;
			dlg.DefaultExt = ".rvt";
			dlg.Filter = "Revit Files (*.rvt)|*.rvt";
			Nullable<bool> result = dlg.ShowDialog();

			if (result == true)
			{
				foreach (string st in dlg.FileNames)
				{
					var s = st.Split('\\');
					string stt = st.Replace(@"\\192.168.1.100\doc_center", "T:");
					FileList.Add(new FileModel()
					{
						File_Path = stt,
						File_Name = s[s.Length - 1]
					});
				}
			}
		}

		//Select Group
		public void SelectGroup()
		{
			FileList = new ObservableCollection<FileModel>();

			DbConnect db = new DbConnect();
			string query = "";

			string IDs = CurrentGroup.FileIds;
			var Id = IDs.Split('.');
			foreach (string s in Id)
			{
				query = string.Format("SELECT * FROM Addin_FileData WHERE Id_File = '{0}'", s);
				DataTable table = db.Get_DataTable(query);
				foreach (DataRow row in table.Rows)
				{
					var ss = row["File_Path"].ToString().Split('\\');
					FileList.Add(new FileModel()
					{
						File_Path = row["File_Path"].ToString(),
						File_Name = ss[ss.Length - 1]
					});
				}
			}

			db.Close_DB_Connection();

			DbUtils dbUtils = new DbUtils();
			CenterDb = dbUtils.CheckUpdateWithCurrentFile(CurrentGroup);
			IsCheckAll = true;
		}

		//Edit Group
		public void EditGroup()
		{
			DbConnect db = new DbConnect();
			string query = "";
			DataTable table;

			//Tạo list file
			foreach (FileModel fm in FileList)
			{
				query = string.Format("SELECT * FROM Addin_FileData WHERE File_Path = '{0}'", fm.File_Path);
				table = db.Get_DataTable(query);
				if (table.Rows.Count == 0)
				{
					query = string.Format("INSERT INTO Addin_FileData (File_Path, File_Name) VALUES('{0}','{1}'); ", fm.File_Path, fm.File_Name);
					db.Execute_SQL(query);
				}
			}

			//Tạo chuỗi id File
			string FileIds = "";
			foreach (FileModel fm in FileList)
			{
				query = string.Format("SELECT * FROM Addin_FileData WHERE File_Path = '{0}'", fm.File_Path);
				table = db.Get_DataTable(query);
				foreach (DataRow row in table.Rows)
				{
					FileIds = FileIds + row["Id_File"].ToString() + ".";
				}
			}

			//Tạo Group
			query = string.Format("UPDATE Addin_GroupData SET FileIds = '{0}' WHERE Group_Name = '{1}';", FileIds, CurrentGroup.Group_Name);
			db.Execute_SQL(query);
			db.Close_DB_Connection();

			MessageBox.Show($"Group {CurrentGroup.Group_Name} update thành công!", "Revit SynchronizedData", MessageBoxButton.OK, MessageBoxImage.Information);
		}

		//Get List Group
		public List<GroupFileModel> GetListGroup()
		{
			List<GroupFileModel> list = new List<GroupFileModel>();

			string fname = "";
			try
			{
				var modelPath = ActiveData.Document.GetWorksharingCentralModelPath();

				var centralServerPath = ModelPathUtils.ConvertModelPathToUserVisiblePath(modelPath);

				fname = centralServerPath.ToString();
			}
			catch
			{
				fname = ActiveData.Document.PathName;
			}

			DbConnect db = new DbConnect();

			string query = string.Format("SELECT * FROM Addin_GroupData");
			DataTable tableGroup = db.Get_DataTable(query);

			fname = fname.Replace(@"\\192.168.1.100\doc_center", "T:");
			query = string.Format("SELECT * FROM Addin_FileData WHERE File_Path = '{0}'", fname);
			DataTable table = db.Get_DataTable(query);
			try
			{
				string IdFile = table.Rows[0]["Id_File"].ToString();

				foreach (DataRow row in tableGroup.Rows)
				{

					var Ids = row["FileIds"].ToString().Split('.');
					foreach (string s in Ids)
					{
						if (s == IdFile)
						{
							list.Add(
								new GroupFileModel
								{
									Group_Name = row["Group_Name"].ToString(),
									FileIds = row["FileIds"].ToString()
								});
							break;
						}
					}
				}

				db.Close_DB_Connection();

				return list;
			}
			catch { return list; }
		}

		//Check All
		public void CheckAllCenter()
		{
			DbUtils dbUtils = new DbUtils();
			ObservableCollection<CenterData> t = new ObservableCollection<CenterData>();
			t = CenterDb;
			CenterDb = new ObservableCollection<CenterData>();
			CenterDb = t;
			foreach (CenterData c in CenterDb)
			{
				c.IsCheck = true;
			}
		}

		//UnCheck All
		public void UnCheckAllCenter()
		{
			DbUtils dbUtils = new DbUtils();
			ObservableCollection<CenterData> t = new ObservableCollection<CenterData>();
			t = CenterDb;
			CenterDb = new ObservableCollection<CenterData>();
			CenterDb = t;
			foreach (CenterData c in CenterDb)
			{
				c.IsCheck = false;
			}
		}

		//Delete File
		public void DelFileFromGroup()
		{
			//FileModel fileModel = wd1.ListFile.SelectedItem as FileModel;

		}
		#endregion

		#region Send Db
		//Send data WallType
		public void CopyWalltype()
		{
			if (CurrentGroup is null)
			{
				MessageBox.Show("Group is null! \nPlz, select Group.", "Revit SynchronizedData");
				return;
			}

			if (CategoryName == "Walls")
			{
				WallType = WallTypes.Where(x => x.Name == SelectType).FirstOrDefault();

				if (WallType.FamilyName == "Stacked Wall")
				{
					MessageBox.Show(WallType.FamilyName + "\nThis tool don't support for this family.", "Revit SynchronizedData");
					return;
				}

				CopyWallTypeDb(WallType);
				MessageBox.Show(WallType.Name + "\n Was Copied!", "Revit SynchronizedData");
			}
			else if (CategoryName == "Floors")
			{
				FloorType FloorType = FloorTypes.Where(x => x.Name == SelectType).FirstOrDefault();
				CopyWallTypeDb(WallType);
				MessageBox.Show(FloorType.Name + "\n Was Copied!", "Revit SynchronizedData");
			}
			else if (CategoryName == "Materials")
			{
				Material mat = ListMaterial.Where(x => x.Name == SelectType).FirstOrDefault();
				CopyMaterialDb(mat);
				MessageBox.Show(mat.Name + "\n Was Copied!", "Revit SynchronizedData");
			}
			wd1.Close();

		}

		public void Pick2Select()
		{
			try
			{
				if (CurrentGroup is null)
				{
					MessageBox.Show("Group is null! \nPlz, select Group.", "Revit SynchronizedData");
					return;
				}

				List<ElementId> selected = ActiveData.Selection.GetElementIds().Where(e =>
					ActiveData.Document.GetElement(e).Category.Id.IntegerValue == (int)BuiltInCategory.OST_Walls ||
					ActiveData.Document.GetElement(e).Category.Id.IntegerValue == (int)BuiltInCategory.OST_Floors ||
					ActiveData.Document.GetElement(e).Category.Id.IntegerValue == (int)BuiltInCategory.OST_Doors ||
					ActiveData.Document.GetElement(e).Category.Id.IntegerValue == (int)BuiltInCategory.OST_Windows ||
					ActiveData.Document.GetElement(e).Category.Id.IntegerValue == (int)BuiltInCategory.OST_WallTags ||
					ActiveData.Document.GetElement(e).Category.Id.IntegerValue == (int)BuiltInCategory.OST_CaseworkTags ||
					ActiveData.Document.GetElement(e).Category.Id.IntegerValue == (int)BuiltInCategory.OST_CeilingTags ||
					ActiveData.Document.GetElement(e).Category.Id.IntegerValue == (int)BuiltInCategory.OST_DoorTags ||
					ActiveData.Document.GetElement(e).Category.Id.IntegerValue == (int)BuiltInCategory.OST_FloorTags ||
					ActiveData.Document.GetElement(e).Category.Id.IntegerValue == (int)BuiltInCategory.OST_FurnitureTags ||
					ActiveData.Document.GetElement(e).Category.Id.IntegerValue == (int)BuiltInCategory.OST_FurnitureSystemTags ||
					ActiveData.Document.GetElement(e).Category.Id.IntegerValue == (int)BuiltInCategory.OST_GenericModelTags ||
					ActiveData.Document.GetElement(e).Category.Id.IntegerValue == (int)BuiltInCategory.OST_ParkingTags ||
					ActiveData.Document.GetElement(e).Category.Id.IntegerValue == (int)BuiltInCategory.OST_PlantingTags ||
					ActiveData.Document.GetElement(e).Category.Id.IntegerValue == (int)BuiltInCategory.OST_PlumbingFixtureTags ||
					ActiveData.Document.GetElement(e).Category.Id.IntegerValue == (int)BuiltInCategory.OST_RailingSystemTags ||
					ActiveData.Document.GetElement(e).Category.Id.IntegerValue == (int)BuiltInCategory.OST_RevisionCloudTags ||
					ActiveData.Document.GetElement(e).Category.Id.IntegerValue == (int)BuiltInCategory.OST_RoofTags ||
					ActiveData.Document.GetElement(e).Category.Id.IntegerValue == (int)BuiltInCategory.OST_RoomTags ||
					ActiveData.Document.GetElement(e).Category.Id.IntegerValue == (int)BuiltInCategory.OST_StairsTags ||
					ActiveData.Document.GetElement(e).Category.Id.IntegerValue == (int)BuiltInCategory.OST_WallTags ||
					ActiveData.Document.GetElement(e).Category.Id.IntegerValue == (int)BuiltInCategory.OST_WindowTags
					).ToList();

				if (selected.Count() < 1)
				{
					selected = ActiveData.Selection.PickObjects
					(ObjectType.Element, new FilterCategoryUtils
					{
						FuncElement = x =>
						x.Category.Id.IntegerValue == (int)BuiltInCategory.OST_Walls ||
						x.Category.Id.IntegerValue == (int)BuiltInCategory.OST_Floors ||
						x.Category.Id.IntegerValue == (int)BuiltInCategory.OST_Doors ||
						x.Category.Id.IntegerValue == (int)BuiltInCategory.OST_Windows ||
						x.Category.Id.IntegerValue == (int)BuiltInCategory.OST_WallTags ||
						x.Category.Id.IntegerValue == (int)BuiltInCategory.OST_CaseworkTags ||
						x.Category.Id.IntegerValue == (int)BuiltInCategory.OST_CeilingTags ||
						x.Category.Id.IntegerValue == (int)BuiltInCategory.OST_DoorTags ||
						x.Category.Id.IntegerValue == (int)BuiltInCategory.OST_FloorTags ||
						x.Category.Id.IntegerValue == (int)BuiltInCategory.OST_FurnitureTags ||
						x.Category.Id.IntegerValue == (int)BuiltInCategory.OST_FurnitureSystemTags ||
						x.Category.Id.IntegerValue == (int)BuiltInCategory.OST_GenericModelTags ||
						x.Category.Id.IntegerValue == (int)BuiltInCategory.OST_ParkingTags ||
						x.Category.Id.IntegerValue == (int)BuiltInCategory.OST_PlantingTags ||
						x.Category.Id.IntegerValue == (int)BuiltInCategory.OST_PlumbingFixtureTags ||
						x.Category.Id.IntegerValue == (int)BuiltInCategory.OST_RailingSystemTags ||
						x.Category.Id.IntegerValue == (int)BuiltInCategory.OST_RevisionCloudTags ||
						x.Category.Id.IntegerValue == (int)BuiltInCategory.OST_RoofTags ||
						x.Category.Id.IntegerValue == (int)BuiltInCategory.OST_RoomTags ||
						x.Category.Id.IntegerValue == (int)BuiltInCategory.OST_StairsTags ||
						x.Category.Id.IntegerValue == (int)BuiltInCategory.OST_WallTags ||
						x.Category.Id.IntegerValue == (int)BuiltInCategory.OST_WindowTags
					},
						  "Select some element to copy WallType"
					)
					.Select(e => ActiveData.Document.GetElement(e).Id).ToList();
				}


				List<WallType> selectedWT = new List<WallType>();
				List<FloorType> selectedFT = new List<FloorType>();
				List<FamilySymbol> selectedFS = new List<FamilySymbol>();

				foreach (ElementId elementid in selected)
				{
					Element element = ActiveData.Document.GetElement(elementid);

					if (element.Category.Name == "Walls")
					{
						Wall wall = ActiveData.Document.GetElement(elementid) as Wall;
						WallType wt = wall.WallType;
						try
						{
							WallType WtinList = selectedWT.Where(x => x.Name == wt.Name).FirstOrDefault();
							if (WtinList is null)
							{
								selectedWT.Add(wt);
							}
						}
						catch
						{
							selectedWT.Add(wt);
						}
					}
					else if (element.Category.Name == "Floors")
					{
						Floor floor = ActiveData.Document.GetElement(elementid) as Floor;
						FloorType ft = floor.FloorType;
						try
						{
							FloorType FtinList = selectedFT.Where(x => x.Name == ft.Name).FirstOrDefault();
							if (FtinList is null)
							{
								selectedFT.Add(ft);
							}
						}
						catch
						{
							selectedFT.Add(ft);
						}
					}
					else
					{
						FamilySymbol familySymbol = ActiveData.Document.GetElement(element.GetTypeId()) as FamilySymbol;

						try
						{
							FamilySymbol FtinList = selectedFS.Where(x => x.Name == familySymbol.Name).FirstOrDefault();
							if (FtinList is null)
							{
								selectedFS.Add(familySymbol);
							}
						}
						catch
						{
							selectedFS.Add(familySymbol);
						}
					}
				}

				string TypeList = "";
				foreach (WallType wt in selectedWT)
				{
					if (wt.FamilyName == "Basic Wall")
					{
						if (!CheckWallTypeSplitByVertical.IsWallTypeSplitByVertical(wt))
						{
							CopyWallTypeDb(wt);
							TypeList = TypeList + wt.Name + "\n";
						}
						else
						{
							MessageBox.Show($"{wt.Name} isn't supported to copy", "Revit SynchronizedData", MessageBoxButton.OK, MessageBoxImage.Error);
						}
					}
					else if (wt.FamilyName == "Curtain Wall")
					{
						CopyCurtainWallTypeDb(wt);
						TypeList = TypeList + wt.Name + "\n";
					}
					else if (wt.FamilyName == "Stacked Wall")
					{
						MessageBox.Show($"{wt.FamilyName} isn't supported to copy", "Revit SynchronizedData", MessageBoxButton.OK, MessageBoxImage.Error);
					}
				}
				foreach (FloorType wt in selectedFT)
				{
					CopyFloorTypeDb(wt);
					TypeList = TypeList + wt.Name + "\n";
				}
				foreach (FamilySymbol fs in selectedFS)
				{
					CopyFamilySymbolDb(fs);
					TypeList = TypeList + fs.Name + "\n";
				}

				if (TypeList != "") MessageBox.Show(TypeList + "\n Was Copied!", "Revit SynchronizedData");
				wd1.Close();
			}
			catch
			{ return; }
		}

		//Basic Wall
		public void CopyWallTypeDb(WallType wt)
		{
			List<CompoundStructureWallType> listLayers = new List<CompoundStructureWallType>();
			DbUtils dbUtils = new DbUtils();
			string IdWallLayer = "";
			try
			{
				string WallTypeName = wt.Name;
				//Create Centerdata 
				dbUtils.Create_CenterData("Wall", WallTypeName, CurrentGroup);
				//GetId of Center
				string IDCenter = dbUtils.Get_CenterId("Wall", WallTypeName, CurrentGroup);

				string Wallfunction = wt.Function.ToString();
				string FamilyName = wt.FamilyName.ToString();
				CompoundStructure compoundStructure = wt.GetCompoundStructure();

				//compoundStructure.GetExtendableRegionIds(true);

				IList<CompoundStructureLayer> layers = compoundStructure.GetLayers();

				Parameter ScaleFillColorParam = wt.LookupParameter("Coarse Scale Fill Color");
				string ScaleFillColorParamValue = ScaleFillColorParam.AsInteger().ToString();

				Parameter ScaleFillPatternParam = wt.LookupParameter("Coarse Scale Fill Pattern");
				string ScaleFillPatternParamValue = ScaleFillPatternParam.AsValueString();

				double minLayerV = 10000;
				double maxLayerV = 0;

				IList<int> Extendable_Top = compoundStructure.GetExtendableRegionIds(true);
				IList<int> Extendable_Bot = compoundStructure.GetExtendableRegionIds(false);

				for (int i = 0; i < layers.Count; i++)
				{
					CompoundStructureLayer Layer = layers[i];
					string LayerFunction = Layer.Function.ToString();
					int layerid = Layer.LayerId;
					double layerwidth = Layer.Width;
					string MaterialId = Layer.MaterialId.ToString();
					ElementId id = Layer.MaterialId;
					Material m = ActiveData.Document.GetElement(id) as Material;
					bool TopExtension = false;
					bool BotExtension = false;

					#region test
					IList<int> ints = compoundStructure.GetRegionsAssociatedToLayer(i);
					foreach (int i2 in ints)
					{
						if (TopExtension == false) TopExtension = Extendable_Top.IndexOf(i2) > -1 ? true : false;
						if (BotExtension == false) BotExtension = Extendable_Bot.IndexOf(i2) > -1 ? true : false;
					}
					double minU = 100;
					double minV = 100;
					double maxU = -100;
					double maxV = -100;

					foreach (int RegionId in ints)
					{
						IList<int> SegIds = compoundStructure.GetSegmentIds();
						foreach (int segId in SegIds)
						{
							IList<int> intsRegions = compoundStructure.GetAdjacentRegions(segId);
							if (intsRegions.Contains(RegionId))
							{
								UV endPoint1 = UV.Zero;
								UV endPoint2 = UV.Zero;
								compoundStructure.GetSegmentEndPoints(segId, RegionId, out endPoint1, out endPoint2);

								if (minU >= endPoint1.U) minU = endPoint1.U;
								if (minV >= endPoint1.V) minV = endPoint1.V;
								if (maxU <= endPoint1.U) maxU = endPoint1.U;
								if (maxV <= endPoint1.V) maxV = endPoint1.V;

								if (minU >= endPoint2.U) minU = endPoint2.U;
								if (minV >= endPoint2.V) minV = endPoint2.V;
								if (maxU <= endPoint2.U) maxU = endPoint2.U;
								if (maxV <= endPoint2.V) maxV = endPoint2.V;
							}
						}
					}

					if (Math.Round(Math.Abs(maxU - minU), 5) != Math.Round(layerwidth, 5)) layerwidth = 0;
					string materialName = "<By Category>";
					try
					{
						materialName = m.Name;
					}
					catch
					{ }

					#endregion
					CompoundStructureWallType la = new CompoundStructureWallType(LayerFunction, layerid, layerwidth, materialName, minU, minV, maxU, maxV, TopExtension, BotExtension);
					listLayers.Add(la);
					//Db
					IdWallLayer = IdWallLayer + dbUtils.CreateDb_CompoundStructureWallType(la, IDCenter) + ".";

					if (minLayerV > minV)
						minLayerV = minV;

					if (maxLayerV < maxV)
						maxLayerV = maxV;

				}

				WallTypeItem = new WallTypeItem(
					WallTypeName,
					Wallfunction,
					FamilyName,
					listLayers,
					compoundStructure.GetNumberOfShellLayers(ShellLayerType.Exterior),
					compoundStructure.GetNumberOfShellLayers(ShellLayerType.Interior),
					compoundStructure.StructuralMaterialIndex,
					minLayerV,
					maxLayerV,
					ScaleFillColorParamValue,
					ScaleFillPatternParamValue
					);
				dbUtils.CreateWallTypeDb(WallTypeItem, IdWallLayer, IDCenter);

			}
			catch
			{
			}
		}

		//Basic Floors
		public void CopyFloorTypeDb(FloorType ft)
		{
			List<CompoundStructureFloorType> listLayers = new List<CompoundStructureFloorType>();
			DbUtils dbUtils = new DbUtils();
			string IdFloorLayer = "";
			try
			{
				string FloorTypeName = ft.Name;
				//Create Centerdata 
				dbUtils.Create_CenterData("Floors", FloorTypeName, CurrentGroup);
				//GetId of Center
				string IDCenter = dbUtils.Get_CenterId("Floors", FloorTypeName, CurrentGroup);

				string FamilyName = ft.FamilyName.ToString();
				CompoundStructure compoundStructure = ft.GetCompoundStructure();

				IList<CompoundStructureLayer> layers = compoundStructure.GetLayers();

				Parameter ScaleFillColorParam = ft.LookupParameter("Coarse Scale Fill Color");
				string ScaleFillColorParamValue = ScaleFillColorParam.AsInteger().ToString();

				Parameter ScaleFillPatternParam = ft.LookupParameter("Coarse Scale Fill Pattern");
				string ScaleFillPatternParamValue = ScaleFillPatternParam.AsValueString();

				for (int i = 0; i < layers.Count; i++)
				{
					CompoundStructureLayer Layer = layers[i];
					string LayerFunction = Layer.Function.ToString();
					int layerid = Layer.LayerId;
					double layerwidth = Layer.Width;
					string MaterialId = Layer.MaterialId.ToString();
					ElementId id = Layer.MaterialId;
					Material m = ActiveData.Document.GetElement(id) as Material;

					string materialName = "<By Category>";
					try
					{
						materialName = m.Name;
					}
					catch
					{ }

					CompoundStructureFloorType la = new CompoundStructureFloorType(LayerFunction, i, layerwidth, materialName);
					listLayers.Add(la);
					//Db
					IdFloorLayer = IdFloorLayer + dbUtils.CreateDb_CompoundStructureFloorType(la, IDCenter) + ".";

				}

				Parameter functionParam = ft.LookupParameter("Function");
				string stfunctionParam = "";
				if (functionParam.AsInteger() == 0)
				{
					stfunctionParam = "In";
				}
				else if (functionParam.AsInteger() == 1)
				{
					stfunctionParam = "Ex";
				}

				FloorTypeItem = new FloorTypeItem(
					FloorTypeName,
					FamilyName,
					listLayers,
					compoundStructure.GetNumberOfShellLayers(ShellLayerType.Exterior),
					compoundStructure.GetNumberOfShellLayers(ShellLayerType.Interior),
					compoundStructure.StructuralMaterialIndex,
					ScaleFillColorParamValue,
					ScaleFillPatternParamValue,
					stfunctionParam
					);
				//Create Db
				dbUtils.CreateFloorTypeDb(FloorTypeItem, IdFloorLayer, IDCenter);

			}
			catch
			{
			}
		}

		//Material
		public void CopyMaterialDb(Material mat)
		{
			DbUtils dbUtils = new DbUtils();
			//Create Centerdata 
			dbUtils.Create_CenterData("Material", mat.Name, CurrentGroup);
			//GetId of Center
			string IDCenter = dbUtils.Get_CenterId("Material", mat.Name, CurrentGroup);

			List<FillPatternElement> FillPatterns = new FilteredElementCollector(ActiveData.Document)
			.OfClass(typeof(FillPatternElement))
			.Cast<FillPatternElement>()
			.ToList();

			FillPatternElement fillPatternElement1 = null;
			FillPatternElement fillPatternElement2 = null;
			FillPatternElement fillPatternElement3 = null;
			FillPatternElement fillPatternElement4 = null;

			try
			{
				fillPatternElement1 = FillPatterns.Where(x => x.Id == mat.CutBackgroundPatternId).FirstOrDefault();
			}
			catch { }
			try
			{
				fillPatternElement2 = FillPatterns.Where(x => x.Id == mat.CutForegroundPatternId).FirstOrDefault();
			}
			catch { }
			try
			{
				fillPatternElement3 = FillPatterns.Where(x => x.Id == mat.SurfaceBackgroundPatternId).FirstOrDefault();
			}
			catch { }
			try
			{
				fillPatternElement4 = FillPatterns.Where(x => x.Id == mat.SurfaceForegroundPatternId).FirstOrDefault();
			}
			catch { }

			AppearanceAssetElement AppearanceAsset = null;
			try
			{
				AppearanceAsset = mat.Document.GetElement(mat.AppearanceAssetId) as AppearanceAssetElement;
				//Asset theAsset = AppearanceAsset.GetRenderingAsset();
				//List<AssetProperty> assets = new List<AssetProperty>();
				//for (int idx = 0; idx < theAsset.Size; idx++)
				//{
				//	AssetProperty ap = theAsset[idx];
				//	assets.Add(ap);
				//}

				//dbUtils.CreateMaterialAssetProperties(assets, IDCenter);
			}
			catch
			{ }

			List<PropertySetElement> PropertySetElements = new FilteredElementCollector(ActiveData.Document)
			.OfClass(typeof(PropertySetElement))
			.Cast<PropertySetElement>()
			.ToList();
			PropertySetElement StructuralAsset = null;
			PropertySetElement ThermalAsset = null;
			try
			{
				StructuralAsset = PropertySetElements.Where(x => x.Id == mat.StructuralAssetId).FirstOrDefault();
			}
			catch
			{ }
			try
			{
				ThermalAsset = PropertySetElements.Where(x => x.Id == mat.ThermalAssetId).FirstOrDefault();
			}
			catch
			{ }

			ConvertColor convertColor = new ConvertColor();

			try
			{
				MaterialModel materialModel = new MaterialModel()
				{
					Name = mat.Name,
					Description = mat.LookupParameter("Description").AsValueString(),
					KeyNote = mat.LookupParameter("Keynote").AsInteger(),
					Mark = mat.LookupParameter("Mark").AsValueString(),
					UserRenderAppearance = mat.UseRenderAppearanceForShading,
					Color = mat.LookupParameter("Color").AsInteger(),
					Tranferancy = mat.LookupParameter("Transparency").AsInteger(),
					CutBackgroundPatternColor = convertColor.Color2String(mat.CutBackgroundPatternColor),
					CutBackgroundPatternName = fillPatternElement1 == null ? "" : fillPatternElement1.Name,
					CutForegroundPatternColor = convertColor.Color2String(mat.CutForegroundPatternColor),
					CutForegroundPatternName = fillPatternElement2 == null ? "" : fillPatternElement2.Name,
					SurfaceBackgroundPatternColor = convertColor.Color2String(mat.SurfaceBackgroundPatternColor),
					SurfaceBackgroundPatternName = fillPatternElement3 == null ? "" : fillPatternElement3.Name,
					SurfaceForegroundPatternColor = convertColor.Color2String(mat.SurfaceForegroundPatternColor),
					SurfaceForegroundPatternName = fillPatternElement4 == null ? "" : fillPatternElement4.Name,
					MaterialCategory = mat.MaterialCategory,
					MaterialClass = mat.MaterialClass,
					Shininess = mat.Shininess,
					Smoothness = mat.Smoothness,
					AppearanceName = AppearanceAsset == null ? "" : AppearanceAsset.Name,
					StructuralAssetName = StructuralAsset == null ? "" : StructuralAsset.Name,
					ThemalAssetName = ThermalAsset == null ? "" : ThermalAsset.Name
				};

				dbUtils.CreateMaterialDb(materialModel, IDCenter);
			}
			catch (Exception ex)
			{ }

		}

		//Curtain Wall
		public void CopyCurtainWallTypeDb(WallType wt)
		{
			DbUtils dbUtils = new DbUtils();
			try
			{
				string WallTypeName = wt.Name;
				//Create Centerdata  
				dbUtils.Create_CenterData("CurtainWall", WallTypeName, CurrentGroup);
				//GetId of Center
				string IDCenter = dbUtils.Get_CenterId("CurtainWall", WallTypeName, CurrentGroup);
				string FamilyName = wt.FamilyName.ToString();

				//Get function parameter
				Parameter functionParam = wt.LookupParameter("Function");
				string stfunctionParam = functionParam.AsValueString();

				//Get Automatically Embed parameter
				Parameter automaticallyEmbedParam = wt.LookupParameter("Automatically Embed");
				int intAutomaticallyEmbedParam = automaticallyEmbedParam.AsInteger();

				//Get Curtain Panel parameter
				Parameter curtainPanelParam = wt.LookupParameter("Curtain Panel");
				string stcurtainPanelParam = curtainPanelParam.AsValueString();

				//Get Join Condition parameter
				Parameter joinConditionParam = wt.LookupParameter("Join Condition");
				int intjoinConditionParam = joinConditionParam.AsInteger();

				//Get Structural Material
				Parameter structuralMaterialParam = wt.LookupParameter("Structural Material");
				string ststructuralMaterialParam = structuralMaterialParam.AsValueString();

				//Vertical Grid
				BuiltInParameter paraIndex = BuiltInParameter.SPACING_LAYOUT_VERT;
				Parameter layoutVertparameter = wt.get_Parameter(paraIndex);
				int stlayoutVertparameter = layoutVertparameter.AsInteger();

				paraIndex = BuiltInParameter.SPACING_LENGTH_VERT;
				Parameter spacingVertparameter = wt.get_Parameter(paraIndex);
				double stspacingVertparameter = spacingVertparameter.AsDouble();

				paraIndex = BuiltInParameter.CURTAINGRID_ADJUST_BORDER_VERT;
				Parameter adjustborderVertparameter = wt.get_Parameter(paraIndex);
				int intadjustborderVertparameter = adjustborderVertparameter.AsInteger();

				//Horizontal Grid
				paraIndex = BuiltInParameter.SPACING_LAYOUT_HORIZ;
				Parameter layoutHorizparameter = wt.get_Parameter(paraIndex);
				int stlayoutHorizparameter = layoutHorizparameter.AsInteger();

				paraIndex = BuiltInParameter.SPACING_LENGTH_HORIZ;
				Parameter spacingHorizparameter = wt.get_Parameter(paraIndex);
				double stspacingHorizparameter = spacingHorizparameter.AsDouble();

				paraIndex = BuiltInParameter.CURTAINGRID_ADJUST_BORDER_HORIZ;
				Parameter adjustborderHorizparameter = wt.get_Parameter(paraIndex);
				int intadjustborderHorizparameter = adjustborderHorizparameter.AsInteger();

				//Vertical Mullions
				paraIndex = BuiltInParameter.AUTO_MULLION_INTERIOR_VERT;
				Parameter interiorTypeVertparameter = wt.get_Parameter(paraIndex);
				string stinteriorTypeVertparameter = interiorTypeVertparameter.AsValueString();

				paraIndex = BuiltInParameter.AUTO_MULLION_BORDER1_VERT;
				Parameter border1TypeVertparameter = wt.get_Parameter(paraIndex);
				string stborder1TypeVertparameter = border1TypeVertparameter.AsValueString();

				paraIndex = BuiltInParameter.AUTO_MULLION_BORDER2_VERT;
				Parameter border2TypeVertparameter = wt.get_Parameter(paraIndex);
				string stborder2TypeVertparameter = border2TypeVertparameter.AsValueString();
				//Horizontal Mullions
				paraIndex = BuiltInParameter.AUTO_MULLION_INTERIOR_HORIZ;
				Parameter interiorTypeHorizparameter = wt.get_Parameter(paraIndex);
				string stinteriorTypeHorizparameter = interiorTypeHorizparameter.AsValueString();

				paraIndex = BuiltInParameter.AUTO_MULLION_BORDER1_HORIZ;
				Parameter border1TypeHorizparameter = wt.get_Parameter(paraIndex);
				string stborder1TypeHorizparameter = border1TypeHorizparameter.AsValueString();

				paraIndex = BuiltInParameter.AUTO_MULLION_BORDER2_HORIZ;
				Parameter border2TypeHorizparameter = wt.get_Parameter(paraIndex);
				string stborder2TypeHorizparameter = border2TypeHorizparameter.AsValueString();

				//Create CurtainWall Item
				CurtainWallItem curtainWallItem = new CurtainWallItem
				{
					WallTypeName = WallTypeName,
					Wallfunction = stfunctionParam,
					Wall_AutomaticallyEmbed = intAutomaticallyEmbedParam,
					JoinCondition = intjoinConditionParam,
					CurtainPanel = stcurtainPanelParam,
					StructuralMaterial = ststructuralMaterialParam,
					LayoutVert = stlayoutVertparameter,
					SpacingVert = stspacingVertparameter,
					AdjustBorderVert = intadjustborderVertparameter,
					LayoutHoriz = stlayoutHorizparameter,
					SpacingHoriz = stspacingHorizparameter,
					AdjustBorderHoriz = intadjustborderHorizparameter,
					InteriorTypeVert = stinteriorTypeVertparameter,
					Border1Vert = stborder1TypeVertparameter,
					Border2Vert = stborder2TypeVertparameter,
					InteriorTypeHoriz = stinteriorTypeHorizparameter,
					Border1Horiz = stborder1TypeHorizparameter,
					Border2Horiz = stborder2TypeHorizparameter,
				};

				dbUtils.CreateCurtainWallTypeDb(curtainWallItem, IDCenter);
			}
			catch
			{ }
		}

		//FamilySymbol
		public void CopyFamilySymbolDb(FamilySymbol fs)
		{
			DbUtils dbUtils = new DbUtils();

			string Namef = fs.FamilyName;
			string Nametype = fs.Name;

			//Create Centerdata  
			dbUtils.Create_CenterData(fs.Category.Name, $"<{Namef} -- {Nametype}>", CurrentGroup);
			//GetId of Center
			string IDCenter = dbUtils.Get_CenterId(fs.Category.Name, $"<{Namef} -- {Nametype}>", CurrentGroup);

			dbUtils.CreateFamilySymbolDb(fs, IDCenter);
		}
		#endregion

		//Db WallType
		public void Download()
		{
			wd1.Hide();
			DbUtils db = new DbUtils();
			string TypeList = "";
			Action action = new Action(() =>
			{
				using (Transaction trans = new Transaction(ActiveData.Document))
				{
					trans.Start("Transaction Group");
					foreach (CenterData c in CenterDb)
					{
						if (c.IsCheck)
						{
							switch (c.Type.Replace(" ", ""))
							{
								case "Wall":
									CreateWallTypeWithId(c);
									CreateWallType();
									CreateWallType();

									db.DeleteCenterData(c.Id);
									TypeList = TypeList + c.TypeName + "\n";
									break;
								case "CurtainWall":
									CreateCurtainWallTypeWithId(c);
									CreateCurtainWallType();

									db.DeleteCenterData(c.Id);
									TypeList = TypeList + c.TypeName + "\n";
									break;
								case "Floors":
									CreateFloorTypeWithId(c);
									CreateFloorType();

									db.DeleteCenterData(c.Id);
									TypeList = TypeList + c.TypeName + "\n";
									break;
								case "Material":
									CreateMaterialWithId(c);
									CreateMaterial();

									db.DeleteCenterData(c.Id);
									TypeList = TypeList + c.TypeName + "\n";
									break;
								default://Door, Window, Tag
									CreateFamilySymbol(c);

									db.DeleteCenterData(c.Id);
									TypeList = TypeList + c.TypeName + "\n";
									break;
							}
						}
					}

					MessageBox.Show(TypeList + "\n Was Update!", "Revit SynchronizedData");
					trans.Commit();
				}
			});

			ExternalEventHandler.Instance.SetAction(action);

			ExternalEventHandler.Instance.Run();

			wd1.Close();
		}

		#region WallType
		public void CreateWallTypeWithId(CenterData cd)
		{
			DbConnect db = new DbConnect();
			string query = "";
			try
			{
				List<CompoundStructureWallType> listLayers = new List<CompoundStructureWallType>();
				query = string.Format("SELECT * FROM Addin_WallType_CompoundStructure WHERE IdCenter = '{0}' ORDER BY Id ASC", cd.Id);
				DataTable table = db.Get_DataTable(query);

				foreach (DataRow row in table.Rows)
				{
					CompoundStructureWallType la = new CompoundStructureWallType(
						row["LayerFunction"].ToString().Replace(" ", ""),
						int.Parse(row["Layerid"].ToString()),
						double.Parse(row["Layerwidth"].ToString()),
						row["MaterialName"].ToString(),
						double.Parse(row["MinU"].ToString()),
						double.Parse(row["MinV"].ToString()),
						double.Parse(row["MaxU"].ToString()),
						double.Parse(row["MaxV"].ToString()),
						bool.Parse(row["TopExtension"].ToString()),
						bool.Parse(row["BotExtension"].ToString()));
					listLayers.Add(la);
				}

				query = string.Format("SELECT * FROM Addin_WallTypeItem WHERE Id_Center = '{0}'", cd.Id);
				table = db.Get_DataTable(query);
				foreach (DataRow row in table.Rows)
				{
					WallTypeItem = new WallTypeItem(
					row["WallTypeName"].ToString(),
					row["Wallfunction"].ToString(),
					row["FamilyName"].ToString(),
					listLayers,
					int.Parse(row["Exterior"].ToString()),
					int.Parse(row["Interior"].ToString()),
					int.Parse(row["StructureIndex"].ToString()),
					double.Parse(row["MinLayerV"].ToString()),
					double.Parse(row["MaxLayerV"].ToString()),
					row["ScaleFillColor"].ToString(),
					row["ScaleFillPattern"].ToString()
					);
				}
				db.Close_DB_Connection();
			}
			catch
			{ }
		}

		public void CreateCurtainWallTypeWithId(CenterData cd)
		{
			DbConnect db = new DbConnect();
			string query = "";
			try
			{
				query = string.Format("SELECT * FROM Addin_CurtainWallItem WHERE Id_Center = '{0}'", cd.Id);
				DataTable table = db.Get_DataTable(query);
				foreach (DataRow row in table.Rows)
				{
					CurtainWallItem = new CurtainWallItem
					{
						WallTypeName = row["WallTypeName"].ToString(),
						Wallfunction = row["Wallfunction"].ToString(),
						Wall_AutomaticallyEmbed = int.Parse(row["AutomaticallyEmbed"].ToString()),
						CurtainPanel = row["CurtainPanel"].ToString(),
						JoinCondition = int.Parse(row["JoinCondition"].ToString()),
						StructuralMaterial = row["StructuralMaterial"].ToString(),
						LayoutVert = int.Parse(row["LayoutVert"].ToString()),
						SpacingVert = double.Parse(row["SpacingVert"].ToString()),
						AdjustBorderVert = int.Parse(row["AdjustBorderVert"].ToString()),
						LayoutHoriz = int.Parse(row["LayoutHoriz"].ToString()),
						SpacingHoriz = double.Parse(row["SpacingHoriz"].ToString()),
						AdjustBorderHoriz = int.Parse(row["AdjustBorderHoriz"].ToString()),
						InteriorTypeVert = row["InteriorTypeVert"].ToString(),
						Border1Vert = row["Border1Vert"].ToString(),
						Border2Vert = row["Border2Vert"].ToString(),
						InteriorTypeHoriz = row["InteriorTypeHoriz"].ToString(),
						Border1Horiz = row["Border1Horiz"].ToString(),
						Border2Horiz = row["Border2Horiz"].ToString(),
					};
				}
				db.Close_DB_Connection();
			}
			catch
			{ }
		}
		//Create WallType
		public void CreateWallType()
		{
			double OldWidth = 0;
			bool IsMerge = false;
			bool IsBot = false;
			bool IsTop = false;
			double Bot = 0;
			double Top = 0;
			double WTminV = 10;
			double WTmaxV = 0;
			try
			{
				WallType new_WT = null;
				try
				{
					new_WT = WallTypes[1].Duplicate(WallTypeItem.WallTypeName) as WallType;
				}
				catch
				{
					new_WT = WallTypes.Where(x => x.Name == WallTypeItem.WallTypeName).FirstOrDefault();
				}
				List<CompoundStructureWallType> LayersOfWall = WallTypeItem.CompoundStructureWallTypeLayers;

				CompoundStructure compoundStructure = new_WT.GetCompoundStructure();

				IList<CompoundStructureLayer> layers = new List<CompoundStructureLayer>();

				//Find max V 
				IList<int> SegIds = compoundStructure.GetSegmentIds();
				foreach (int segId in SegIds)
				{
					int regionId = compoundStructure.GetAdjacentRegions(segId)[0];
					UV endPoint1 = UV.Zero;
					UV endPoint2 = UV.Zero;
					compoundStructure.GetSegmentEndPoints(segId, regionId, out endPoint1, out endPoint2);
					if (Math.Max(endPoint1.V, endPoint2.V) > WTmaxV) WTmaxV = Math.Max(endPoint1.V, endPoint2.V);
					if (Math.Min(endPoint1.V, endPoint2.V) < WTminV) WTminV = Math.Max(endPoint1.V, endPoint2.V);
				}

				//Create CompoundStructureWallType
				for (int i = 0; i < LayersOfWall.Count; i++)
				{
					//Tìm đoạn cắt
					if (i > 0 && LayersOfWall.ElementAt(i).MinV != 100)
					{
						Bot = Math.Abs(LayersOfWall.ElementAt(i).MinV - LayersOfWall.ElementAt(i - 1).MinV);
						Top = Math.Abs(LayersOfWall.ElementAt(i).MaxV - LayersOfWall.ElementAt(i - 1).MaxV);
						if (Bot > 0) IsBot = true;
						if (Top > 0) IsTop = true;
					}

					CompoundStructureWallType c = LayersOfWall.ElementAt(i);

					MaterialFunctionAssignment materialFunctionAssignment;
					switch (c.LayerFunction)
					{
						case "Structure":
							materialFunctionAssignment = (MaterialFunctionAssignment)1;
							break;
						case "Substrate":
							materialFunctionAssignment = (MaterialFunctionAssignment)2;
							break;
						case "Insulation":
							materialFunctionAssignment = (MaterialFunctionAssignment)3;
							break;
						case "Finish1":
							materialFunctionAssignment = (MaterialFunctionAssignment)4;
							break;
						case "Finish2":
							materialFunctionAssignment = (MaterialFunctionAssignment)5;
							break;
						case "Membrane":
							materialFunctionAssignment = (MaterialFunctionAssignment)100;
							break;
						case "StructuralDeck":
							materialFunctionAssignment = (MaterialFunctionAssignment)200;
							break;
						default:
							materialFunctionAssignment = (MaterialFunctionAssignment)0;
							break;
					}

					ElementId materialid;
					try
					{
						MaterialItem materialLayer = Materials.Where(x => x.MaterialName == c.MaterialName).FirstOrDefault();
						materialid = materialLayer.MaterialId;
					}
					catch
					{
						materialid = new ElementId(-1);
					}
					CompoundStructureLayer compoundStructureLayer;

					if (c.Layerwidth > 0)
					{
						compoundStructureLayer = new CompoundStructureLayer(c.Layerwidth, materialFunctionAssignment, materialid);
						OldWidth = c.Layerwidth;
					}
					// Lớp vật liệu bị chia cắt
					else
					{
						IsMerge = true;

						double currentWith = 0;
						if (c.MaxU != -100)
						{
							if (OldWidth != 0)
							{
								currentWith = Math.Abs((c.MaxU - c.MinU)) - OldWidth;
							}
							else
							{
								currentWith = Math.Abs((c.MaxU - c.MinU)) - LayersOfWall.ElementAt(i + 1).Layerwidth;
							}
						}
						compoundStructureLayer = new CompoundStructureLayer(Math.Abs(currentWith), materialFunctionAssignment, materialid);
					}
					layers.Insert(i, compoundStructureLayer);
				}

				//compoundStructure = CompoundStructure.CreateSimpleCompoundStructure(layers);
				compoundStructure.SetLayers(layers);

				IList<int> TopExtend = new List<int>();
				IList<int> BotExtend = new List<int>();
				//Create Extendable Region
				for (int i = 0; i < LayersOfWall.Count; i++)
				{
					if (LayersOfWall.ElementAt(i).TopExtension == true)
					{
						foreach (int a in compoundStructure.GetRegionsAssociatedToLayer(i))
						{
							TopExtend.Add(a);
						}
					}
					if (LayersOfWall.ElementAt(i).BotExtension == true)
					{
						foreach (int a in compoundStructure.GetRegionsAssociatedToLayer(i))
						{
							BotExtend.Add(a);
						}
					}
				}

				compoundStructure.SetExtendableRegionIds(true, TopExtend);
				compoundStructure.SetExtendableRegionIds(false, BotExtend);

				compoundStructure.SetNumberOfShellLayers(ShellLayerType.Exterior, WallTypeItem.Exterior);
				compoundStructure.SetNumberOfShellLayers(ShellLayerType.Interior, WallTypeItem.Interior);
				compoundStructure.StructuralMaterialIndex = WallTypeItem.StructureIndex;

				new_WT.SetCompoundStructure(compoundStructure);

				WallFunction wallFunction;
				switch (WallTypeItem.Wallfunction)
				{
					case "Interior":
						wallFunction = (WallFunction)0;
						break;
					case "Exterior":
						wallFunction = (WallFunction)1;
						break;
					case "Foundation":
						wallFunction = (WallFunction)2;
						break;
					case "Retaining":
						wallFunction = (WallFunction)3;
						break;
					case "Soffit":
						wallFunction = (WallFunction)4;
						break;
					default:
						wallFunction = (WallFunction)5;
						break;
				}

				new_WT.Function = wallFunction;

				new_WT.LookupParameter("Coarse Scale Fill Color").Set(int.Parse(WallTypeItem.ScaleFillColor));
				//Tìm Pattern
				FilteredElementCollector elements = new FilteredElementCollector(ActiveData.Document);
				FillPatternElement FillPattern = elements.OfClass(typeof(FillPatternElement))
					.Cast<FillPatternElement>().Where(x => x.Name == WallTypeItem.ScaleFillPattern).FirstOrDefault();

				new_WT.LookupParameter("Coarse Scale Fill Pattern").Set(FillPattern.Id);

				#region Merge Region //MergeRegionsAdjacentToSegment
				if (IsMerge == true)
				{
					try
					{
						compoundStructure = new_WT.GetCompoundStructure();
						//Test
						if (IsBot == true)
						{
							SegIds = compoundStructure.GetSegmentIds();
							foreach (int segId in SegIds)
							{
								//Tìm segment thuộc 2 lớp vật liệu
								if (compoundStructure.GetAdjacentRegions(segId).Count() == 2)
								{
									int regionId = compoundStructure.GetAdjacentRegions(segId)[0];//CHECK ID VÙNG SẼ BỊ MERGE
									UV endPoint1 = UV.Zero;
									UV endPoint2 = UV.Zero;
									compoundStructure.GetSegmentEndPoints(segId, regionId, out endPoint1, out endPoint2);

									// Tạo region mới 
									RectangularGridSegmentOrientation splitOrientation =
									(RectangularGridSegmentOrientation)(((int)(compoundStructure.GetSegmentOrientation(segId)) + 1) % 2);

									UV splitUV = new UV(endPoint1.U, Math.Min(endPoint1.V, endPoint2.V) + Bot);
									int newRegionId = compoundStructure.SplitRegion(splitUV, splitOrientation);
									break;
								}
							}

							SegIds = compoundStructure.GetSegmentIds();
							foreach (int segId in SegIds)
							{
								//Tìm segment thuộc 2 lớp vật liệu có đúng chiều dài đoạn bot
								if (compoundStructure.GetAdjacentRegions(segId).Count() == 2)
								{
									int regionId = compoundStructure.GetAdjacentRegions(segId)[0];
									UV endPoint1 = UV.Zero;
									UV endPoint2 = UV.Zero;
									compoundStructure.GetSegmentEndPoints(segId, regionId, out endPoint1, out endPoint2);

									if (Math.Abs(endPoint1.V - endPoint2.V) == Bot &&
									Math.Min(endPoint1.V, endPoint2.V) == WTminV)
									{
										compoundStructure.MergeRegionsAdjacentToSegment(segId, compoundStructure.GetAdjacentRegions(segId)[0]);
										try
										{
											compoundStructure.VariableLayerIndex = 0;
										}
										catch
										{ 
										}
										new_WT.SetCompoundStructure(compoundStructure);
									}
								}
							}
						}

						compoundStructure = new_WT.GetCompoundStructure();
						if (IsTop == true)
						{
							SegIds = compoundStructure.GetSegmentIds();
							foreach (int segId in SegIds)
							{
								//Tìm segment thuộc 2 lớp vật liệu
								if (compoundStructure.GetAdjacentRegions(segId).Count() == 2)
								{
									int regionId = compoundStructure.GetAdjacentRegions(segId)[0];
									UV endPoint1 = UV.Zero;
									UV endPoint2 = UV.Zero;
									compoundStructure.GetSegmentEndPoints(segId, regionId, out endPoint1, out endPoint2);

									if (Math.Max(endPoint1.V, endPoint2.V) == WTmaxV)
									{
										RectangularGridSegmentOrientation splitOrientation =
										(RectangularGridSegmentOrientation)(((int)(compoundStructure.GetSegmentOrientation(segId)) + 1) % 2);

										UV splitUV = new UV(endPoint1.U, Math.Max(endPoint1.V, endPoint2.V) - Top);
										int newRegionId = compoundStructure.SplitRegion(splitUV, splitOrientation);
										break;
									}
								}
							}

							SegIds = compoundStructure.GetSegmentIds();
							foreach (int segId in SegIds)
							{
								//Tìm segment thuộc 2 lớp vật liệu và có đỉnh = đỉnh tường
								if (compoundStructure.GetAdjacentRegions(segId).Count() == 2)
								{
									int regionId = compoundStructure.GetAdjacentRegions(segId)[0];
									UV endPoint1 = UV.Zero;
									UV endPoint2 = UV.Zero;
									compoundStructure.GetSegmentEndPoints(segId, regionId, out endPoint1, out endPoint2);

									if (Math.Max(endPoint1.V, endPoint2.V) == WTmaxV)
									{
										compoundStructure.MergeRegionsAdjacentToSegment(segId, compoundStructure.GetAdjacentRegions(segId)[1]);
										new_WT.SetCompoundStructure(compoundStructure);
									}
								}
							}
						}
					}
					catch (Exception ex) { }
				}
				#endregion
			}
			catch (Exception ex) { }
		}

		public void CreateCurtainWallType()
		{
			try
			{
				WallType new_WT = null;
				try
				{
					WallType wt = WallTypes.Where(x => x.Name == CurtainWallItem.WallTypeName && x.FamilyName == "Curtain Wall").FirstOrDefault();
					new_WT = wt;
				}
				catch
				{
					WallType wt = WallTypes.Where(x => x.FamilyName == "Curtain Wall").FirstOrDefault();
					new_WT = wt.Duplicate(CurtainWallItem.WallTypeName) as WallType;
				}

				//Function
				WallFunction wallFunction;
				switch (CurtainWallItem.Wallfunction)
				{
					case "Interior":
						wallFunction = (WallFunction)0;
						break;
					case "Exterior":
						wallFunction = (WallFunction)1;
						break;
					case "Foundation":
						wallFunction = (WallFunction)2;
						break;
					case "Retaining":
						wallFunction = (WallFunction)3;
						break;
					case "Soffit":
						wallFunction = (WallFunction)4;
						break;
					default:
						wallFunction = (WallFunction)5;
						break;
				}
				new_WT.Function = wallFunction;

				//Automatically Embed
				new_WT.LookupParameter("Automatically Embed").Set(CurtainWallItem.Wall_AutomaticallyEmbed);

				//Curtain Panel
				List<PanelType> panelTypes = new FilteredElementCollector(ActiveData.Document)
				.OfClass(typeof(PanelType))
				.Cast<PanelType>()
				.ToList();

				string panel = CurtainWallItem.CurtainPanel.Replace(" : ", "/");
				var v = panel.Split('/');
				ElementId eid = new ElementId(-1);
				try
				{
					if (v[0] == "System Panel" || v[0] == "Empty System Panel")
					{
						PanelType pt = panelTypes.Where(x => x.Name == v[1]).FirstOrDefault();
						eid = pt.Id;
					}
					else if (v[0] == "Basic Wall" || v[0] == "Curtain Wall" || v[0] == "Stacked Wall")
					{
						WallType wt = WallTypes.Where(x => x.Name == v[1]).FirstOrDefault();
						eid = wt.Id;
					}
				}
				catch { }
				new_WT.LookupParameter("Curtain Panel").Set(eid);

				//Join Condition
				new_WT.LookupParameter("Join Condition").Set(CurtainWallItem.JoinCondition);

				//Vertical Grid
				BuiltInParameter paraIndex = BuiltInParameter.SPACING_LAYOUT_VERT;
				new_WT.get_Parameter(paraIndex).Set(CurtainWallItem.LayoutVert);
				//if (CurtainWallItem.LayoutVert != 0)
				//{
				try
				{
					paraIndex = BuiltInParameter.SPACING_LENGTH_VERT;
					new_WT.get_Parameter(paraIndex).Set(CurtainWallItem.SpacingVert);
				}
				catch { }
				try
				{
					paraIndex = BuiltInParameter.CURTAINGRID_ADJUST_BORDER_VERT;
					new_WT.get_Parameter(paraIndex).Set(CurtainWallItem.AdjustBorderVert);
				}
				catch { }
				//}

				//Horiz Grid
				paraIndex = BuiltInParameter.SPACING_LAYOUT_HORIZ;
				new_WT.get_Parameter(paraIndex).Set(CurtainWallItem.LayoutHoriz);
				//if (CurtainWallItem.LayoutHoriz != 0)
				//{
				try
				{
					paraIndex = BuiltInParameter.SPACING_LENGTH_HORIZ;
					new_WT.get_Parameter(paraIndex).Set(CurtainWallItem.SpacingHoriz);
				}
				catch { }
				try
				{
					paraIndex = BuiltInParameter.CURTAINGRID_ADJUST_BORDER_HORIZ;
					new_WT.get_Parameter(paraIndex).Set(CurtainWallItem.AdjustBorderHoriz);
				}
				catch { }
				//}

				//Vertical Mullions
				List<MullionType> mullionTypes = new FilteredElementCollector(ActiveData.Document)
				.OfClass(typeof(MullionType))
				.Cast<MullionType>()
				.ToList();
				eid = new ElementId(-1);
				string mullion = CurtainWallItem.InteriorTypeVert.Replace(" : ", "/");
				v = mullion.Split('/');
				try
				{
					MullionType mt = mullionTypes.Where(x => x.Name == v[1]).FirstOrDefault();
					eid = mt.Id;
				}
				catch { }
				paraIndex = BuiltInParameter.AUTO_MULLION_INTERIOR_VERT;
				new_WT.get_Parameter(paraIndex).Set(eid);

				mullion = CurtainWallItem.Border1Vert.Replace(" : ", "/");
				v = mullion.Split('/');
				eid = new ElementId(-1);
				try
				{
					MullionType mt = mullionTypes.Where(x => x.Name == v[1]).FirstOrDefault();
					eid = mt.Id;
				}
				catch { }
				paraIndex = BuiltInParameter.AUTO_MULLION_BORDER1_VERT;
				new_WT.get_Parameter(paraIndex).Set(eid);

				mullion = CurtainWallItem.Border2Vert.Replace(" : ", "/");
				v = mullion.Split('/');
				eid = new ElementId(-1);
				try
				{
					MullionType mt = mullionTypes.Where(x => x.Name == v[1]).FirstOrDefault();
					eid = mt.Id;
				}
				catch { }
				paraIndex = BuiltInParameter.AUTO_MULLION_BORDER2_VERT;
				new_WT.get_Parameter(paraIndex).Set(eid);

				//Horiz Mullions
				mullion = CurtainWallItem.InteriorTypeHoriz.Replace(" : ", "/");
				v = mullion.Split('/');
				eid = new ElementId(-1);
				try
				{
					MullionType mt = mullionTypes.Where(x => x.Name == v[1]).FirstOrDefault();
					eid = mt.Id;
				}
				catch { }
				paraIndex = BuiltInParameter.AUTO_MULLION_INTERIOR_HORIZ;
				new_WT.get_Parameter(paraIndex).Set(eid);

				mullion = CurtainWallItem.Border1Horiz.Replace(" : ", "/");
				v = mullion.Split('/');
				eid = new ElementId(-1);
				try
				{
					MullionType mt = mullionTypes.Where(x => x.Name == v[1]).FirstOrDefault();
					eid = mt.Id;
				}
				catch { }
				paraIndex = BuiltInParameter.AUTO_MULLION_BORDER1_HORIZ;
				new_WT.get_Parameter(paraIndex).Set(eid);

				mullion = CurtainWallItem.Border2Horiz.Replace(" : ", "/");
				v = mullion.Split('/');
				eid = new ElementId(-1);
				try
				{
					MullionType mt = mullionTypes.Where(x => x.Name == v[1]).FirstOrDefault();
					eid = mt.Id;
				}
				catch { }
				paraIndex = BuiltInParameter.AUTO_MULLION_BORDER2_HORIZ;
				new_WT.get_Parameter(paraIndex).Set(eid);

			}
			catch (Exception ex)
			{ }
		}
		#endregion

		#region FloorType
		public void CreateFloorTypeWithId(CenterData cd)
		{
			DbConnect db = new DbConnect();
			string query = "";
			try
			{
				List<CompoundStructureFloorType> listLayers = new List<CompoundStructureFloorType>();
				query = string.Format("SELECT * FROM Addin_WallType_CompoundStructure WHERE IdCenter = '{0}' ORDER BY Id ASC", cd.Id);
				DataTable table = db.Get_DataTable(query);

				foreach (DataRow row in table.Rows)
				{
					CompoundStructureFloorType la = new CompoundStructureFloorType(
						row["LayerFunction"].ToString().Replace(" ", ""),
						int.Parse(row["Layerid"].ToString()),
						double.Parse(row["Layerwidth"].ToString()),
						row["MaterialName"].ToString());
					listLayers.Add(la);
				}

				query = string.Format("SELECT * FROM Addin_WallTypeItem WHERE Id_Center = '{0}'", cd.Id);
				table = db.Get_DataTable(query);
				foreach (DataRow row in table.Rows)
				{
					FloorTypeItem = new FloorTypeItem(
					row["WallTypeName"].ToString(),
					row["FamilyName"].ToString(),
					listLayers,
					int.Parse(row["Exterior"].ToString()),
					int.Parse(row["Interior"].ToString()),
					int.Parse(row["StructureIndex"].ToString()),
					row["ScaleFillColor"].ToString(),
					row["ScaleFillPattern"].ToString(),
					row["Wallfunction"].ToString()
					);
				}
				db.Close_DB_Connection();
			}
			catch
			{ }
		}

		public void CreateFloorType()
		{
			try
			{
				FloorType new_FT = null;
				try
				{
					new_FT = FloorTypes[1].Duplicate(FloorTypeItem.FloorTypeName) as FloorType;
				}
				catch (Exception ex)
				{
					new_FT = FloorTypes.Where(x => x.Name == FloorTypeItem.FloorTypeName && x.FamilyName == FloorTypeItem.FamilyName).FirstOrDefault();
				}
				List<CompoundStructureFloorType> LayersOfFloor = FloorTypeItem.CompoundStructureFloorTypeLayers;

				CompoundStructure compoundStructure = new_FT.GetCompoundStructure();

				IList<CompoundStructureLayer> layers = new List<CompoundStructureLayer>();

				//Create CompoundStructureWallType
				for (int i = 0; i < LayersOfFloor.Count; i++)
				{

					CompoundStructureFloorType c = LayersOfFloor.ElementAt(i);

					MaterialFunctionAssignment materialFunctionAssignment;
					switch (c.LayerFunction)
					{
						case "Structure":
							materialFunctionAssignment = (MaterialFunctionAssignment)1;
							break;
						case "Substrate":
							materialFunctionAssignment = (MaterialFunctionAssignment)2;
							break;
						case "Insulation":
							materialFunctionAssignment = (MaterialFunctionAssignment)3;
							break;
						case "Finish1":
							materialFunctionAssignment = (MaterialFunctionAssignment)4;
							break;
						case "Finish2":
							materialFunctionAssignment = (MaterialFunctionAssignment)5;
							break;
						case "Membrane":
							materialFunctionAssignment = (MaterialFunctionAssignment)100;
							break;
						case "StructuralDeck":
							materialFunctionAssignment = (MaterialFunctionAssignment)200;
							break;
						default:
							materialFunctionAssignment = (MaterialFunctionAssignment)0;
							break;
					}

					ElementId materialid;
					try
					{
						MaterialItem materialLayer = Materials.Where(x => x.MaterialName == c.MaterialName).FirstOrDefault();
						materialid = materialLayer.MaterialId;
					}
					catch
					{
						materialid = new ElementId(-1);
					}
					CompoundStructureLayer compoundStructureLayer;

					compoundStructureLayer = new CompoundStructureLayer(c.Layerwidth, materialFunctionAssignment, materialid);

					layers.Insert(i, compoundStructureLayer);
				}

				//compoundStructure = CompoundStructure.CreateSimpleCompoundStructure(layers);
				compoundStructure.SetLayers(layers);

				compoundStructure.SetNumberOfShellLayers(ShellLayerType.Exterior, FloorTypeItem.Exterior);
				compoundStructure.SetNumberOfShellLayers(ShellLayerType.Interior, FloorTypeItem.Interior);
				compoundStructure.StructuralMaterialIndex = FloorTypeItem.StructureIndex;

				new_FT.SetCompoundStructure(compoundStructure);

				new_FT.LookupParameter("Coarse Scale Fill Color").Set(int.Parse(FloorTypeItem.ScaleFillColor));
				new_FT.LookupParameter("Function").Set(FloorTypeItem.FloorFunction == "In" ? 0 : 1);
				//Tìm Pattern
				FilteredElementCollector elements = new FilteredElementCollector(ActiveData.Document);
				FillPatternElement FillPattern = elements.OfClass(typeof(FillPatternElement))
					.Cast<FillPatternElement>().Where(x => x.Name == FloorTypeItem.ScaleFillPattern).FirstOrDefault();

				new_FT.LookupParameter("Coarse Scale Fill Pattern").Set(FillPattern.Id);
			}
			catch (Exception ex) { }
		}
		#endregion

		#region Material
		public void CreateMaterialWithId(CenterData cd)
		{
			DbConnect db = new DbConnect();
			string query = "";
			try
			{
				query = string.Format("SELECT * FROM Addin_MaterialItem WHERE Id_Center = '{0}' ORDER BY Id_Center ASC", cd.Id);
				DataTable table = db.Get_DataTable(query);
				MaterialModel matModel = new MaterialModel();
				foreach (DataRow row in table.Rows)
				{
					matModel.Name = row["Name"].ToString();
					matModel.Description = row["Description"].ToString();
					matModel.KeyNote = int.Parse(row["KeyNote"].ToString());
					matModel.Mark = row["Mark"].ToString();
					matModel.UserRenderAppearance = bool.Parse(row["UserRenderAppearance"].ToString());
					matModel.Color = int.Parse(row["Color"].ToString());
					matModel.Tranferancy = int.Parse(row["Tranferancy"].ToString());
					matModel.CutBackgroundPatternColor = row["CutBackgroundPatternColor"].ToString();
					matModel.CutBackgroundPatternName = row["CutBackgroundPatternName"].ToString();
					matModel.CutForegroundPatternColor = row["CutForegroundPatternColor"].ToString();
					matModel.CutForegroundPatternName = row["CutForegroundPatternName"].ToString();
					matModel.SurfaceBackgroundPatternColor = row["SurfaceBackgroundPatternColor"].ToString();
					matModel.SurfaceBackgroundPatternName = row["SurfaceBackgroundPatternName"].ToString();
					matModel.SurfaceForegroundPatternColor = row["SurfaceForegroundPatternColor"].ToString();
					matModel.SurfaceForegroundPatternName = row["SurfaceForegroundPatternName"].ToString();
					matModel.MaterialCategory = row["MaterialCategory"].ToString();
					matModel.MaterialClass = row["MaterialClass"].ToString();
					matModel.Shininess = int.Parse(row["Shininess"].ToString());
					matModel.Smoothness = int.Parse(row["Smoothness"].ToString());
					matModel.AppearanceName = row["AppearanceName"].ToString();
					matModel.StructuralAssetName = row["StructuralAssetName"].ToString();
					matModel.ThemalAssetName = row["ThemalAssetName"].ToString();
				}
				MaterialModel = new MaterialModel();
				MaterialModel = matModel;
			}
			catch { }
		}
		public void CreateMaterial()
		{
			ConvertColor convertColor = new ConvertColor();
			List<FillPatternElement> FillPatterns = new FilteredElementCollector(ActiveData.Document)
			.OfClass(typeof(FillPatternElement))
			.Cast<FillPatternElement>()
			.ToList();
			try
			{
				Material new_Material = null;
				try
				{
					new_Material = ListMaterial[1].Duplicate(MaterialModel.Name) as Material;
				}
				catch (Exception ex)
				{
					new_Material = ListMaterial.Where(x => x.Name == MaterialModel.Name).FirstOrDefault();
				}

				new_Material.LookupParameter("Description").Set(MaterialModel.Description);

				try
				{
					new_Material.LookupParameter("KeyNote").Set(MaterialModel.KeyNote.ToString());
				}
				catch { }

				new_Material.LookupParameter("Mark").Set(MaterialModel.Mark);

				new_Material.UseRenderAppearanceForShading = MaterialModel.UserRenderAppearance;

				try
				{
					new_Material.LookupParameter("Color").Set(MaterialModel.Color);
					new_Material.LookupParameter("Tranferancy").Set(MaterialModel.Tranferancy);
				}
				catch { }

				new_Material.CutBackgroundPatternColor = convertColor.String2Color(MaterialModel.CutBackgroundPatternColor);

				ElementId patternId = new ElementId(-1);
				try
				{
					FillPatternElement fillPatternElement = FillPatterns.Where(x => x.Name == MaterialModel.CutBackgroundPatternName).FirstOrDefault();
					patternId = fillPatternElement.Id;
				}
				catch { }
				new_Material.CutBackgroundPatternId = patternId;

				new_Material.CutForegroundPatternColor = convertColor.String2Color(MaterialModel.CutForegroundPatternColor);

				patternId = new ElementId(-1);
				try
				{
					FillPatternElement fillPatternElement = FillPatterns.Where(x => x.Name == MaterialModel.CutForegroundPatternName).FirstOrDefault();
					patternId = fillPatternElement.Id;
				}
				catch { }
				new_Material.CutForegroundPatternId = patternId;

				new_Material.SurfaceBackgroundPatternColor = convertColor.String2Color(MaterialModel.SurfaceBackgroundPatternColor);

				patternId = new ElementId(-1);
				try
				{
					FillPatternElement fillPatternElement = FillPatterns.Where(x => x.Name == MaterialModel.SurfaceBackgroundPatternName).FirstOrDefault();
					patternId = fillPatternElement.Id;
				}
				catch { }
				new_Material.SurfaceBackgroundPatternId = patternId;

				new_Material.SurfaceForegroundPatternColor = convertColor.String2Color(MaterialModel.SurfaceForegroundPatternColor);

				patternId = new ElementId(-1);
				try
				{
					FillPatternElement fillPatternElement = FillPatterns.Where(x => x.Name == MaterialModel.SurfaceForegroundPatternName).FirstOrDefault();
					patternId = fillPatternElement.Id;
				}
				catch { }
				new_Material.SurfaceForegroundPatternId = patternId;

				new_Material.MaterialCategory = MaterialModel.MaterialCategory;

				new_Material.MaterialClass = MaterialModel.MaterialClass;

				new_Material.Shininess = MaterialModel.Shininess;

				new_Material.Smoothness = MaterialModel.Smoothness;

				List<AppearanceAssetElement> AppearanceAssets = new FilteredElementCollector(ActiveData.Document)
				.OfClass(typeof(AppearanceAssetElement))
				.Cast<AppearanceAssetElement>()
				.ToList();
				AppearanceAssetElement AppearanceAsset = null;
				ElementId AppearanceAssetId = new ElementId(-1);
				try
				{
					//AppearanceAsset = AppearanceAssets.Where(x => x.Name == MaterialModel.AppearanceName).FirstOrDefault();
					AppearanceAsset = AppearanceAssetElement.GetAppearanceAssetElementByName(ActiveData.Document, MaterialModel.AppearanceName);
					AppearanceAssetId = AppearanceAsset.Id;
				}
				catch { }
				new_Material.AppearanceAssetId = AppearanceAssetId;

				List<PropertySetElement> PropertySetElements = new FilteredElementCollector(ActiveData.Document)
				.OfClass(typeof(PropertySetElement))
				.Cast<PropertySetElement>()
				.ToList();
				PropertySetElement StructuralAsset = null;
				PropertySetElement ThermalAsset = null;
				ElementId StructuralAssetId = new ElementId(-1);
				ElementId ThermalAssetId = new ElementId(-1);
				try
				{
					StructuralAsset = PropertySetElements.Where(x => x.Name == MaterialModel.StructuralAssetName).FirstOrDefault();
					StructuralAssetId = StructuralAsset.Id;
				}
				catch { }
				try
				{
					ThermalAsset = PropertySetElements.Where(x => x.Name == MaterialModel.ThemalAssetName).FirstOrDefault();
					ThermalAssetId = ThermalAsset.Id;
				}
				catch { }
				new_Material.SetMaterialAspectByPropertySet(MaterialAspect.Structural, StructuralAssetId);
				new_Material.SetMaterialAspectByPropertySet(MaterialAspect.Thermal, ThermalAssetId);
			}
			catch (Exception ex) { }
		}
		#endregion

		#region FamilySymbol
		public void CreateFamilySymbol(CenterData cd)
		{
			DbConnect db = new DbConnect();

			string Cdtypename = cd.TypeName;
			Cdtypename = Cdtypename.Replace(" -- ", "/");
			var v = Cdtypename.Split('/');
			try
			{
				//Get family Symbol
				FamilySymbol fs = null;
				try
				{
					fs = new FilteredElementCollector(ActiveData.Document)
						  .OfClass(typeof(FamilySymbol))
						  .Cast<FamilySymbol>()
						  .Where(x => x.FamilyName == v[0].Replace("<", "") && x.Name == v[1].Replace(">", ""))
						  .FirstOrDefault();
					if (fs == null)
					{
						List<FamilySymbol> familySymbols = new FilteredElementCollector(ActiveData.Document)
						  .OfClass(typeof(FamilySymbol))
						  .Cast<FamilySymbol>()
						  .Where(x => x.FamilyName == v[0].Replace("<", ""))
						  .ToList();

						ElementType s1 = familySymbols[0].Duplicate(v[1].Replace(">", ""));
						fs = s1 as FamilySymbol;
					}
				}
				catch (Exception ex)
				{
					List<FamilySymbol> familySymbols = new FilteredElementCollector(ActiveData.Document)
						  .OfClass(typeof(FamilySymbol))
						  .Cast<FamilySymbol>()
						  .Where(x => x.FamilyName == v[0].Replace("<", ""))
						  .ToList();

					ElementType s1 = familySymbols[0].Duplicate(v[1].Replace(">", ""));
					fs = s1 as FamilySymbol;
				}

				//Set Parameter
				string query = string.Format("SELECT * FROM Addin_Parameter WHERE Id_Center = '{0}'", cd.Id);
				DataTable dataTable = db.Get_DataTable(query);
				foreach (DataRow row in dataTable.Rows)
				{
					string paraName = row["ParaName"].ToString().Replace(" ", "");
					double AsDouble = double.Parse(row["AsDouble"].ToString());
					int AsInt = int.Parse(row["AsInt"].ToString());
					string AsStr = row["AsStr"].ToString();
					string EleID = row["AsEleId"].ToString().Replace(" -- ", "/");

					//Set Para
					try
					{
						BuiltInParameter paraIndex = (BuiltInParameter)int.Parse(row["BuiltInParameter"].ToString());
						Parameter parameter = fs.get_Parameter(paraIndex);
						if (AsStr != "" && AsStr != null)
						{
							parameter.Set(AsStr);
						}
						else if (AsInt != 0)
						{
							parameter.Set(AsInt);
						}
						else if (AsDouble != 0)
						{
							parameter.Set(AsDouble);
						}
						else if (EleID != "-1")
						{
							var s = EleID.Split('/');
							string category = s[0];
							string TypeName = s[1];
							if (category == "BuiltInCategory")
							{
								parameter.Set(new ElementId(int.Parse(s[1])));
							}
							else
							{
								Categories categories = ActiveData.Document.Settings.Categories;
								Category category1 = null;
								foreach (Category c in categories)
								{
									if (c.Name == s[0])
									{
										category1 = c;
										break;
									}
								}

								if (category1 != null)
								{
									FilteredElementCollector collector
									= new FilteredElementCollector(ActiveData.Document)
									.OfCategory((BuiltInCategory)category1.Id.IntegerValue);

									Element e = collector.Where(x => x.Name == s[1]).FirstOrDefault();

									parameter.Set(e.Id);
								}
								//FamilyName
								else 
								{
									IList<ElementType> a = new FilteredElementCollector(ActiveData.Document)
									.OfClass(typeof(ElementType))
									.Cast<ElementType>()
									.ToList();

									ElementType e = a.Where(x => x.FamilyName == s[0] && x.Name == s[1]).FirstOrDefault();
									parameter.Set(e.Id);
								}
							}
						}
					}
					catch
					{ }
				}
				db.Close_DB_Connection();
			}
			catch
			{ }
		}
		#endregion
	}
}
