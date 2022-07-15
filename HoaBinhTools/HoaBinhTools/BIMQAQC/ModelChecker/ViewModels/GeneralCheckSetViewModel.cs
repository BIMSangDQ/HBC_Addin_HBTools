using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Linq;
using Autodesk.Revit.DB;
using DocumentFormat.OpenXml.Drawing.Charts;
using HoaBinhTools.AutocadToRevit.Utils;
using HoaBinhTools.BIMQAQC.ModelChecker.Models;
using HoaBinhTools.BIMQAQC.ModelChecker.Models.GeneralCheckSet;
using HoaBinhTools.BIMQAQC.ModelChecker.Views;
using HoaBinhTools.BIMQAQC.ModelChecker.Views.ProgessBar;
using HoaBinhTools.SynchronizedData.Db;
using Utils;
using DataTable = System.Data.DataTable;

namespace HoaBinhTools.BIMQAQC.ModelChecker.ViewModels
{
	public class GeneralCheckSetViewModel : ViewModelBase
	{
		#region feild
		public GeneralCheckset GetGeneralCheckSet { get; set; }

		public ModelCheckerLaunchView Wmain { get; set; }

		private ObservableCollection<string> generalCheckSets;
		public ObservableCollection<string> GeneralCheckSets
		{
			get
			{
				return generalCheckSets;
			}
			set
			{
				generalCheckSets = value;
				OnPropertyChanged("GeneralCheckSets");
			}
		}

		private string generalCheckSet;
		public string GeneralCheckSet
		{
			get
			{
				return generalCheckSet;
			}
			set
			{
				generalCheckSet = value;
				OnPropertyChanged("GeneralCheckSet");
			}
		}

		private string worksetGrid;
		public string WorksetGrid
		{
			get
			{
				return worksetGrid;
			}
			set
			{
				worksetGrid = value;
				OnPropertyChanged("WorksetGrid");
			}
		}

		private string worksetGridElement;
		public string WorksetGridElement
		{
			get
			{
				return worksetGridElement;
			}
			set
			{
				worksetGridElement = value;
				OnPropertyChanged("WorksetGridElement");
			}
		}

		private List<string> listBParameter;
		public List<string> ListBParameter
		{
			get
			{ return listBParameter; }
			set
			{
				listBParameter = value;
				OnPropertyChanged("ListBParameter");
			}
		}

		private List<string> listParameter;
		public List<string> ListParameter
		{
			get
			{ return listParameter; }
			set
			{
				listParameter = value;
				OnPropertyChanged("ListParameter");
			}
		}

		private List<string> listCategories;
		public List<string> ListCategories
		{
			get
			{ return listCategories; }
			set
			{
				listCategories = value;
				OnPropertyChanged("ListCategories");
			}
		}
		//File Name
		private string fileNamePattern;
		public string FileNamePattern
		{
			get
			{
				return fileNamePattern;
			}
			set
			{
				fileNamePattern = value;
				OnPropertyChanged("FileNamePattern");
			}
		}

		//Project Information
		private ObservableCollection<ParaProjectInfors> listParaProjectInfor;
		public ObservableCollection<ParaProjectInfors> ListParaProjectInfor
		{
			get
			{
				return listParaProjectInfor;
			}
			set
			{
				listParaProjectInfor = value;
				OnPropertyChanged("ListParaProjectInfor");
			}
		}

		//Project Locationmation
		private ObservableCollection<ParaProjectInfors> listParaProjectLocation;
		public ObservableCollection<ParaProjectInfors> ListParaProjectLocation
		{
			get
			{
				return listParaProjectLocation;
			}
			set
			{
				listParaProjectLocation = value;
				OnPropertyChanged("ListParaProjectLocation");
			}
		}

		//MandatoryCondition
		private ObservableCollection<GeneralMandatoryCondition> listMandatory;
		public ObservableCollection<GeneralMandatoryCondition> ListMandatory
		{
			get
			{
				return listMandatory;
			}
			set
			{
				listMandatory = value;
				OnPropertyChanged("ListMandatory");
			}
		}

		//Check to run
		private bool run_FileName;
		public bool Run_FileName
		{
			get
			{
				return run_FileName;
			}
			set
			{
				run_FileName = value;
				OnPropertyChanged("Run_FileName");
			}
		}

		private bool run_ProjectInfor;
		public bool Run_ProjectInfor
		{
			get
			{
				return run_ProjectInfor;
			}
			set
			{
				run_ProjectInfor = value;
				OnPropertyChanged("Run_ProjectInfor");
			}
		}

		private bool run_ProjectLocation;
		public bool Run_ProjectLocation
		{
			get
			{
				return run_ProjectLocation;
			}
			set
			{
				run_ProjectLocation = value;
				OnPropertyChanged("Run_ProjectLocation");
			}
		}

		private bool run_GridWorkset;
		public bool Run_GridWorkset
		{
			get
			{
				return run_GridWorkset;
			}
			set
			{
				run_GridWorkset = value;
				OnPropertyChanged("Run_GridWorkset");
			}
		}

		private bool run_WrongWorkSet;
		public bool Run_WrongWorkSet
		{
			get
			{
				return run_WrongWorkSet;
			}
			set
			{
				run_WrongWorkSet = value;
				OnPropertyChanged("Run_WrongWorkSet");
			}
		}

		//
		public RelayCommand btnCheckSetChange { get; set; }
		public RelayCommand btnSave { get; set; }
		public RelayCommand btnDuplicate { get; set; }
		public RelayCommand btnDelete { get; set; }

		public RelayCommand btnDelParameterProjectInfor { get; set; }
		public RelayCommand AddParameterProjectInfor { get; set; }

		public RelayCommand btnDelParameterProjectLocation { get; set; }
		public RelayCommand AddParameterProjectLocation { get; set; }
		public RelayCommand btnAddMandatoryCondition { get; set; }

		public RelayCommand btnDelMandatory { get; set; }
		public void Excute()
		{
			GetGeneralCheckSet = (GeneralCheckset)this.Wmain.CheckSetFrame.Content;
		}
		#endregion

		public GeneralCheckSetViewModel(Document doc, ModelCheckerLaunchView Wm, List<string> AllPara)
		{
			GeneralCheckSets = new ObservableCollection<string>();

			btnCheckSetChange = new RelayCommand(GetCheckSet);

			btnSave = new RelayCommand(BtnSave);

			btnDuplicate = new RelayCommand(BtnDuplicate);

			btnDelete = new RelayCommand(BtnDelete);

			ListParaProjectInfor = new ObservableCollection<ParaProjectInfors>();

			AddParameterProjectInfor = new RelayCommand(BtnAddParameterProjectInfor);

			btnDelParameterProjectInfor = new RelayCommand(BtnDelParameterProjectInfor);

			ListParaProjectLocation = new ObservableCollection<ParaProjectInfors>();

			AddParameterProjectLocation = new RelayCommand(BtnAddParameterProjectLocation);

			btnDelParameterProjectLocation = new RelayCommand(BtnDelParameterProjectLocation);

			btnAddMandatoryCondition = new RelayCommand(BtnAddMandatoryCondition);

			btnDelMandatory = new RelayCommand(DelMandatory);

			ListParameter = new List<string>();
			AllPara.Add("Project Issue Date");
			AllPara.Add("Project Status");
			AllPara.Add("Client Name");
			AllPara.Add("Project Address");
			AllPara.Add("Project Number");
			AllPara.Add("N/S");
			AllPara.Add("E/W");
			ListParameter = AllPara;
			ListCategories = GetAllCategory(doc);
			ListMandatory = new ObservableCollection<GeneralMandatoryCondition>();
			Wmain = Wm;
			GetSetCheckList();
			GeneralCheckSet = "Default";
			GetCheckSet();
			Excute();
		}

		#region Method
		public List<string> GetAllParameter(Document doc)
		{
			List<Element> allElements = new FilteredElementCollector(doc)
				.WhereElementIsNotElementType()
				.Where(x => x.Category != null)
				.ToList();

			allElements = allElements
							.Distinct(new IEqualityComparerCategory())
							.ToList();

			List<string> AllParameters = new List<string>();
			foreach (Element e in allElements)
			{
				AllParameters.AddRange(ParameterUtils.GetAllParameters(e,true));
			}

			AllParameters = AllParameters.Distinct().ToList();
			AllParameters.Sort();

			return AllParameters;
		}

		public List<string> GetAllCategory(Document doc)
		{
			Categories categories = doc.Settings.Categories;

			List<string> myCategories = new List<string>();

			foreach (Category c in categories)
			{
				myCategories.Add(c.Name);
			}
			myCategories.Sort();

			return myCategories;
		}

		public List<string> GetAllBuilinPara()
		{
			return System.Enum.GetNames(typeof(BuiltInParameter)).ToList();
		}

		public void BtnAddParameterProjectInfor()
		{
			ListParaProjectInfor.Add(new ParaProjectInfors(ListParameter, null));
		}

		public void BtnDelParameterProjectInfor()
		{
			GetGeneralCheckSet = (GeneralCheckset)this.Wmain.CheckSetFrame.Content;
			ParaProjectInfors Path2Del = (ParaProjectInfors)GetGeneralCheckSet.dgparaProjectInfor.SelectedItem;
			ListParaProjectInfor.Remove(Path2Del);
		}

		public void BtnAddParameterProjectLocation()
		{
			ListParaProjectLocation.Add(new ParaProjectInfors(ListParameter, null));
		}

		public void BtnDelParameterProjectLocation()
		{
			GetGeneralCheckSet = (GeneralCheckset)this.Wmain.CheckSetFrame.Content;
			ParaProjectInfors Path2Del = (ParaProjectInfors)GetGeneralCheckSet.dgparaProjectLocation.SelectedItem;
			ListParaProjectLocation.Remove(Path2Del);
		}

		public void BtnAddMandatoryCondition()
		{
			ListMandatory.Add(new GeneralMandatoryCondition());
		}

		public void DelMandatory()
		{
			GetGeneralCheckSet = (GeneralCheckset)this.Wmain.CheckSetFrame.Content;
			GeneralMandatoryCondition Path2Del = (GeneralMandatoryCondition)GetGeneralCheckSet.dgMadatory.SelectedItem;
			ListMandatory.Remove(Path2Del);
		}

		public void BtnSave()
		{
			DbConnect db = new DbConnect();

			string ParaProjectInfor = "";
			foreach (ParaProjectInfors v in ListParaProjectInfor)
			{
				ParaProjectInfor = ParaProjectInfor + v.Name + "|";
			}

			string ParaProjectLocation = "";
			foreach (ParaProjectInfors v in ListParaProjectLocation)
			{
				ParaProjectLocation = ParaProjectLocation + v.Name + "|";
			}

			try
			{
				string query = $"UPDATE Addin_QAQC_General_Check " +
					$"SET FileNamePattern = '{FileNamePattern}'," +
					$"ListParaProjectInfor = '{ParaProjectInfor}'," +
					$"ListParaProjectLocation = '{ParaProjectLocation}'," +
					$"WorksetGrid = '{WorksetGrid}'," +
					$"WorksetGridElement = '{WorksetGridElement}'," +
					$"Run_FileName = '{Convert.ToByte(Run_FileName).ToString()}'," +
					$"Run_ProjectInfor = '{Convert.ToByte(Run_ProjectInfor).ToString()}'," +
					$"Run_ProjectLocation = '{Convert.ToByte(Run_ProjectLocation).ToString()}'," +
					$"Run_GridWorkset = '{Convert.ToByte(Run_GridWorkset).ToString()}'," +
					$"Run_WrongWorkSet = '{Convert.ToByte(Run_WrongWorkSet).ToString()}'" +
					$"WHERE CheckSetName = '{GeneralCheckSet}';";
				db.Execute_SQL(query);

				//Xóa cũ lưu mới
				

				string ID = "1";
				query = $"SELECT * FROM Addin_QAQC_General_Check WHERE CheckSetName = '{GeneralCheckSet}';";
				DataTable dt = db.Get_DataTable(query);
				foreach (DataRow row in dt.Rows)
				{
					ID = row["ID"].ToString();
				}

				query = $"DELETE FROM Addin_QAQC_General_Madatory WHERE Id_General_Check = '{ID}';";
				db.Execute_SQL(query);

				foreach (GeneralMandatoryCondition GMC in ListMandatory)
				{
					query = $"INSERT INTO Addin_QAQC_General_Madatory (Id_General_Check,[Order],Criteria,RangeA,Rangeb,RangeC)" +
						$"VALUES ('{ID}','{ListMandatory.IndexOf(GMC)}','{GMC.Criteria}','{GMC.RangeA}','{GMC.RangeB}','{GMC.RangeC}')";
					db.Execute_SQL(query);
				}

				System.Windows.Forms.MessageBox.Show("CheckSet was save!", "ModelChecker");
			}
			catch (Exception ex)
			{
				System.Windows.Forms.MessageBox.Show("CheckSet wasn't save!", "ModelChecker");
			}
			db.Close_DB_Connection();
		}

		public void BtnDuplicate()
		{
			string input = Microsoft.VisualBasic.Interaction.InputBox("Enter new name", "ModelChecker", GeneralCheckSet+"_1");
			DbConnect db = new DbConnect();

			string query = string.Format("SELECT * FROM Addin_QAQC_General_Check WHERE CheckSetName = '{0}'", input);

			DataTable table = db.Get_DataTable(query);

			foreach (DataRow row in table.Rows)
			{
				System.Windows.Forms.MessageBox.Show("This name is already in use!", "ModelChecker");
				db.Close_DB_Connection();
				return;
			}

			try
			{
				query = string.Format("SELECT * FROM Addin_QAQC_General_Check WHERE CheckSetName = '{0}'", GeneralCheckSet);
				table = db.Get_DataTable(query);
				foreach (DataRow row in table.Rows)
				{
					FileNamePattern = row["FileNamePattern"].ToString();

					ListParaProjectInfor = new ObservableCollection<ParaProjectInfors>();
					string ProjectInfor = row["ListParaProjectInfor"].ToString();
					var vv = ProjectInfor.Split('|');
					foreach (string v in vv)
					{
						if (v != null || v != "")
							ListParaProjectInfor.Add(new ParaProjectInfors(ListParameter, v));
					}

					ListParaProjectLocation = new ObservableCollection<ParaProjectInfors>();
					string ProjectLocation = row["ListParaProjectLocation"].ToString();
					vv = ProjectLocation.Split('|');
					foreach (string v in vv)
					{
						if (v != null || v != "")
							ListParaProjectLocation.Add(new ParaProjectInfors(ListParameter, v));
					}

					WorksetGrid = row["WorksetGrid"].ToString();

					WorksetGridElement = row["WorksetGridElement"].ToString();

					Run_FileName = bool.Parse(row["Run_FileName"].ToString());

					Run_ProjectInfor = bool.Parse(row["Run_ProjectInfor"].ToString());

					Run_ProjectLocation = bool.Parse(row["Run_ProjectLocation"].ToString());

					Run_GridWorkset = bool.Parse(row["Run_GridWorkset"].ToString());

					Run_WrongWorkSet = bool.Parse(row["Run_WrongWorkSet"].ToString());
				}
				query = $"INSERT INTO Addin_QAQC_General_Check (CheckSetName) VALUES ('{input}')";
				db.Execute_SQL(query);

				GetListFile.AddCheckSetName(input);

				GetSetCheckList();

				string ParaProjectInfor = "";
				foreach (ParaProjectInfors v in ListParaProjectInfor)
				{
					ParaProjectInfor = ParaProjectInfor + v.Name + "|";
				}

				string ParaProjectLocation = "";
				foreach (ParaProjectInfors v in ListParaProjectLocation)
				{
					ParaProjectLocation = ParaProjectLocation + v.Name + "|";
				}

				query = $"UPDATE Addin_QAQC_General_Check " +
									$"SET FileNamePattern = '{FileNamePattern}'," +
									$"ListParaProjectInfor = '{ParaProjectInfor}'," +
									$"ListParaProjectLocation = '{ParaProjectLocation}'," +
									$"WorksetGrid = '{WorksetGrid}'," +
									$"WorksetGridElement = '{WorksetGridElement}'," +
									$"Run_FileName = '{Convert.ToByte(Run_FileName).ToString()}'," +
									$"Run_ProjectInfor = '{Convert.ToByte(Run_ProjectInfor).ToString()}'," +
									$"Run_ProjectLocation = '{Convert.ToByte(Run_ProjectLocation).ToString()}'," +
									$"Run_GridWorkset = '{Convert.ToByte(Run_GridWorkset).ToString()}'," +
									$"Run_WrongWorkSet = '{Convert.ToByte(Run_WrongWorkSet).ToString()}'," +
									$"WHERE CheckSetName = '{input}';";

				db.Execute_SQL(query);

				GeneralCheckSet = input;
			}
			catch { }

			db.Close_DB_Connection();
		}

		public void BtnDelete()
		{
			DbConnect db = new DbConnect();

			try { }
			catch { }
			db.Close_DB_Connection();
		}

		public void GetSetCheckList()
		{
			DbConnect db = new DbConnect();

			string query = string.Format("SELECT * FROM Addin_QAQC_General_Check");

			GeneralCheckSets = new ObservableCollection<string>();

			DataTable table = db.Get_DataTable(query);

			foreach (DataRow row in table.Rows)
			{
				GeneralCheckSets.Add(row["CheckSetName"].ToString());
			}

			db.Close_DB_Connection();
		}

		public void GetCheckSet()
		{
			DbConnect db = new DbConnect();

			string query = string.Format("SELECT * FROM Addin_QAQC_General_Check WHERE CheckSetName = '{0}'", GeneralCheckSet);

			DataTable table = db.Get_DataTable(query);

			foreach (DataRow row in table.Rows)
			{
				FileNamePattern = row["FileNamePattern"].ToString();

				ListParaProjectInfor = new ObservableCollection<ParaProjectInfors>();
				string ParaProjectInfor = row["ListParaProjectInfor"].ToString();
				var vv = ParaProjectInfor.Split('|');
				foreach (string v in vv)
				{
					if (v != null && v != "")
						ListParaProjectInfor.Add(new ParaProjectInfors(ListParameter, v));
				}

				ListParaProjectLocation = new ObservableCollection<ParaProjectInfors>();
				string ParaProjectLocation = row["ListParaProjectLocation"].ToString();
				vv = ParaProjectLocation.Split('|');
				foreach (string v in vv)
				{
					if (v != null && v != "")
						ListParaProjectLocation.Add(new ParaProjectInfors(ListParameter, v));
				}

				WorksetGrid = row["WorksetGrid"].ToString();

				WorksetGridElement = row["WorksetGridElement"].ToString();

				Run_FileName = bool.Parse(row["Run_FileName"].ToString());

				Run_ProjectInfor = bool.Parse(row["Run_ProjectInfor"].ToString());

				Run_ProjectLocation = bool.Parse(row["Run_ProjectLocation"].ToString());

				Run_GridWorkset = bool.Parse(row["Run_GridWorkset"].ToString());

				Run_WrongWorkSet = bool.Parse(row["Run_WrongWorkSet"].ToString());
			}

			//GET MANDATORY
			string ID = "1";
			query = $"SELECT * FROM Addin_QAQC_General_Check WHERE CheckSetName = '{GeneralCheckSet}';";
			DataTable dt = db.Get_DataTable(query);
			foreach (DataRow row in dt.Rows)
			{
				ID = row["ID"].ToString();
			}

			ListMandatory = new ObservableCollection<GeneralMandatoryCondition>();
			query = $"SELECT * FROM Addin_QAQC_General_Madatory WHERE Id_General_Check = '{ID}';";
			table = db.Get_DataTable(query);
			foreach (DataRow row in table.Rows)
			{
				ListMandatory.Add(new GeneralMandatoryCondition()
				{
					Criteria = row["Criteria"].ToString(),
					RangeA = double.Parse(row["RangeA"].ToString()),
					RangeB = double.Parse(row["RangeB"].ToString()),
					RangeC = double.Parse(row["RangeC"].ToString()),
				});
			}

			db.Close_DB_Connection();
		}
		#endregion
	}
}
