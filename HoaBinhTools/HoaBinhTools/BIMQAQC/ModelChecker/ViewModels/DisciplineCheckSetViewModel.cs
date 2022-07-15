using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;
using Autodesk.Revit.DB;
using HoaBinhTools.AutocadToRevit.Utils;
using HoaBinhTools.BIMQAQC.ModelChecker.Models;
using HoaBinhTools.BIMQAQC.ModelChecker.Views;
using HoaBinhTools.BIMQAQC.ModelChecker.Views.ProgessBar;
using HoaBinhTools.SynchronizedData.Db;
using Utils;

namespace HoaBinhTools.BIMQAQC.ModelChecker.ViewModels
{
	public class DisciplineCheckSetViewModel : ViewModelBase
	{
		#region feild
		public DisciplineCheckSet DisciplineCheckSet { get; set; }

		public ModelCheckerLaunchView Wmain { get; set; }

		private ObservableCollection<string> desciplineCheck;
		public ObservableCollection<string> DesciplineCheck
		{
			get
			{
				return desciplineCheck;
			}
			set
			{
				desciplineCheck = value;
				OnPropertyChanged("DesciplineCheck");
			}
		}

		private List<string> admins;
		public List<string> Admins
		{
			get
			{
				return admins;
			}
			set
			{
				admins = value;
				OnPropertyChanged("Admins");
			}
		}

		public string username = ActiveData.Username;

		private ObservableCollection<DisciplineCheck> desciplineCheckRows;
		public ObservableCollection<DisciplineCheck> DesciplineCheckRows
		{
			get
			{
				return desciplineCheckRows;
			}
			set
			{
				desciplineCheckRows = value;
				OnPropertyChanged("DesciplineCheckRows");
			}
		}

		private ObservableCollection<DisciplineFilter> desciplineCheckRowFilters;
		public ObservableCollection<DisciplineFilter> DesciplineCheckRowFilters
		{
			get
			{
				return desciplineCheckRowFilters;
			}
			set
			{
				desciplineCheckRowFilters = value;
				OnPropertyChanged("DesciplineCheckRowFilters");
			}
		}

		private DisciplineFilter desciplineCheckRowFilter;
		public DisciplineFilter DesciplineCheckRowFilter
		{
			get
			{
				return desciplineCheckRowFilter;
			}
			set
			{
				desciplineCheckRowFilter = value;
				OnPropertyChanged("DesciplineCheckRowFilter");
			}
		}

		private DisciplineCheck desciplineCheckRow;
		public DisciplineCheck DesciplineCheckRow
		{
			get
			{
				return desciplineCheckRow;
			}
			set
			{
				desciplineCheckRow = value;
				OnPropertyChanged("DesciplineCheckRow");
			}
		}

		private string desciplineCheckName;
		public string DesciplineCheckName
		{
			get
			{
				return desciplineCheckName;
			}
			set
			{
				desciplineCheckName = value;
				OnPropertyChanged("DesciplineCheckName");
			}
		}

		private ObservableCollection<string> listBuilinParameter;
		public ObservableCollection<string> ListBuilinParameter
		{
			get
			{ return listBuilinParameter; }
			set
			{
				listBuilinParameter = value;
				OnPropertyChanged("ListBuilinParameter");
			}
		}

		private ObservableCollection<string> listParameter;
		public ObservableCollection<string> ListParameter
		{
			get
			{ return listParameter; }
			set
			{
				listParameter = value;
				OnPropertyChanged("ListParameter");
			}
		}

		private ObservableCollection<string> listCategories;
		public ObservableCollection<string> ListCategories
		{
			get
			{ return listCategories; }
			set
			{
				listCategories = value;
				OnPropertyChanged("ListCategories");
			}
		}

		private string rangeA = "";
		public string RangeA
		{
			get
			{
				return rangeA;
			}
			set
			{
				rangeA = value;
				OnPropertyChanged("RangeA");
			}
		}

		private string rangeB = "";
		public string RangeB
		{
			get
			{
				return rangeB;
			}
			set
			{
				rangeB = value;
				OnPropertyChanged("RangeB");
			}
		}

		private string rangeC = "";
		public string RangeC
		{
			get
			{
				return rangeC;
			}
			set
			{
				rangeC = value;
				OnPropertyChanged("RangeC");
			}
		}

		private string rangeD = "";
		public string RangeD
		{
			get
			{
				return rangeD;
			}
			set
			{
				rangeD = value;
				OnPropertyChanged("RangeD");
			}
		}

		private string _IdCheck;
		public string IdCheckProgram
		{
			get
			{
				return _IdCheck;
			}
			set
			{
				_IdCheck = value;
				OnPropertyChanged("IdCheckProgram");
			}
		}

		private string checkName;
		public string CheckName
		{
			get
			{
				return checkName;
			}
			set
			{
				checkName = value;
				OnPropertyChanged("CheckName");
			}
		}

		private string description = "";
		public string Description
		{
			get
			{
				return description;
			}
			set
			{
				description = value;
				OnPropertyChanged("Description");
			}
		}

		private string check_Result;
		public string Check_Result
		{
			get
			{
				return check_Result;
			}
			set
			{
				check_Result = value;
				OnPropertyChanged("Check_Result");
			}
		}

		private string failure_Message = "";
		public string Failure_Message
		{
			get
			{
				return failure_Message;
			}
			set
			{
				failure_Message = value;
				OnPropertyChanged("Failure_Message");
			}
		}

		private string parasInResult = "";
		public string ParasInResult
		{
			get
			{
				return parasInResult;
			}
			set
			{
				parasInResult = value;
				OnPropertyChanged("ParasInResult");
			}
		}

		private List<string> check_Results = new List<string>
		{
			"Fail When No Matching Element Are Found",
			"Fail When Matching Element Are Found",
			"Count of Matching Elements Only",
			"Count and list of Matching Elements Only",
		};
		public List<string> Check_Results
		{
			get
			{
				return check_Results;
			}
			set
			{
				check_Results = value;
				OnPropertyChanged("Check_Results");
			}
		}

		private bool run_ThisCheck;
		public bool Run_ThisCheck
		{
			get
			{
				return run_ThisCheck;
			}
			set
			{
				run_ThisCheck = value;
				OnPropertyChanged("Run_ThisCheck");
			}
		}

		private bool isImpotant;
		public bool IsImpotant
		{
			get
			{
				return isImpotant;
			}
			set
			{
				isImpotant = value;
				OnPropertyChanged("IsImpotant");
			}
		}

		private ObservableCollection<DisciplineCheck> disciplineCheck { get; set; }
		public ObservableCollection<DisciplineCheck> DisciplineCheck
		{
			get
			{
				return disciplineCheck;
			}
			set
			{
				disciplineCheck = value;
				OnPropertyChanged("DisciplineCheck");
			}
		}
		#endregion

		#region Button
		public RelayCommand btnCheckSetChange { get; set; }
		public RelayCommand btnSaveCheckSet { get; set; }
		public RelayCommand btnDuplicate { get; set; }
		public RelayCommand btnDeleteCheckSet { get; set; }
		public RelayCommand btnAddCheck { get; set; }
		public RelayCommand btnDeleteCheck { get; set; }
		public RelayCommand btnSaveCheck { get; set; }
		public RelayCommand btnSelectCheck { get; set; }
		public RelayCommand btnAddFilter { get; set; }
		public RelayCommand btnDeleteFilter { get; set; }

		public RelayCommand btnChangeCriteriaFilter { get; set; }
		public RelayCommand btndgfilterDelete { get; set; }
		public RelayCommand btnChangeConditions { get; set; }
		public RelayCommand btnChangeProperty { get; set; }
		public RelayCommand btndgfilterMoveDown { get; set; }
		public RelayCommand btndgfilterMoveUp { get; set; }
		public RelayCommand btnFilterProperty { get; set; }
		public RelayCommand<string> btndgCheckMoveDown { get; set; }
		public RelayCommand<string> btndgCheckMoveUp { get; set; }
		public RelayCommand<string> btnCheckRangeA { get; set; }
		#endregion

		public DisciplineCheckSetViewModel(Document doc, ModelCheckerLaunchView Wm, List<string> AllPara)
		{
			ListParameter = new ObservableCollection<string>(AllPara);
			ListParameter = GetAllParameter(doc);

			ListCategories = new ObservableCollection<string>();
			ListCategories = GetAllCategory(doc);

			ListBuilinParameter = new ObservableCollection<string>();
			ListBuilinParameter = GetAllBuilinPara();
			ListBuilinParameter = new ObservableCollection<string>(GetAllBuilinPara().OrderBy(q => q).ToList());
			GetCheckSet();

			DesciplineCheckRows = new ObservableCollection<DisciplineCheck>();
			DesciplineCheckRowFilters = new ObservableCollection<DisciplineFilter>();

			btnDuplicate = new RelayCommand(DupCheckSet);

			btnCheckSetChange = new RelayCommand(GetListCheck);

			btnAddCheck = new RelayCommand(AddCheck);

			btnSelectCheck = new RelayCommand(SelectCheck);

			btnDeleteCheck = new RelayCommand(DelThisCheck);

			btnAddFilter = new RelayCommand(AddFilter);

			btnSaveCheck = new RelayCommand(SaveCheck);

			btnChangeCriteriaFilter = new RelayCommand(BtnChangeCriteriaFilter);

			btndgfilterDelete = new RelayCommand(DelFilter);

			btnChangeConditions = new RelayCommand(BtnChangeConditions);

			btnChangeProperty = new RelayCommand(ConditionSource);

			btndgfilterMoveDown = new RelayCommand(MoveDown);

			btndgfilterMoveUp = new RelayCommand(MoveUp);

			btndgCheckMoveDown = new RelayCommand<string>(BtnMoveDown_Check);

			btndgCheckMoveUp = new RelayCommand<string>(BtnMoveUp_Check);

			btnFilterProperty = new RelayCommand(BtnFilterProperty);

			btnCheckRangeA = new RelayCommand<string>(BtnCheckRangeA);

			Admins = GetListFile.GetListAdmin();

			Wmain = Wm;

			DesciplineCheckName = "Default";
			SelectCheckSet();
		}

		#region Method
		public ObservableCollection<string> GetAllParameter(Document doc)
		{
			//List<string> Warnings = new List<string>();
			//var registry = Autodesk.Revit.ApplicationServices.Application.GetFailureDefinitionRegistry();
			//var allFailures = registry.ListAllFailureDefinitions();
			//foreach (var i in allFailures)
			//{
			//	if (i.GetSeverity().ToString() == "Warning")
			//		Warnings.Add(i.GetDescriptionText());
			//}


			List<string> AllParameters = new List<string>();

			BindingMap map = doc.ParameterBindings;
			DefinitionBindingMapIterator it = map.ForwardIterator();
			it.Reset();

			while (it.MoveNext())
			{
				AllParameters.Add(it.Key.Name);
			}

			AllParameters.Sort();

			ObservableCollection<string> collection = new ObservableCollection<string>(AllParameters);


			return collection;
		}

		public ObservableCollection<string> GetAllCategory(Document doc)
		{
			Categories categories = doc.Settings.Categories;

			List<string> myCategories = new List<string>();

			foreach (Category c in categories)
			{
				myCategories.Add(c.Name);
			}
			myCategories.Sort();

			ObservableCollection<string> collection = new ObservableCollection<string>(myCategories);
			return collection;
		}

		public ObservableCollection<string> GetAllBuilinPara()
		{
			return new ObservableCollection<string>(System.Enum.GetNames(typeof(BuiltInParameter)).ToList());
		}


		//Lấy list checkset bộ môn
		public void GetCheckSet()
		{
			try
			{
				DbConnect db = new DbConnect();

				string query = string.Format("SELECT * FROM Addin_QAQC_DesciplineCheck");

				DesciplineCheck = new ObservableCollection<string>();

				DataTable table = db.Get_DataTable(query);

				foreach (DataRow row in table.Rows)
				{
					DesciplineCheck.Add(row["Name"].ToString());
				}

				db.Close_DB_Connection();
			}
			catch { }
		}

		//Chọn bộ checkset
		public void SelectCheckSet()
		{
			try
			{
				GetListCheck();
			}
			catch { }
		}

		public void DupCheckSet()
		{
			if (Admins.Contains(username))
			{
				string input = Microsoft.VisualBasic.Interaction.InputBox("Enter new name", "ModelChecker", DesciplineCheckName + "_1");

				DbConnect db = new DbConnect();

				string query = string.Format("SELECT * FROM Addin_QAQC_DesciplineCheck WHERE Name = N'{0}'", input);

				DataTable table = db.Get_DataTable(query);

				foreach (DataRow row in table.Rows)
				{
					System.Windows.Forms.MessageBox.Show("This name is already in use!", "ModelChecker");
					db.Close_DB_Connection();
					return;
				}

				try
				{
					query = $"INSERT INTO Addin_QAQC_DesciplineCheck (Name) VALUES (N'{input}')";
					db.Execute_SQL(query);
					//Tạo bản coppy

					//Id cũ
					query = string.Format("SELECT * FROM Addin_QAQC_DesciplineCheck WHERE Name = N'{0}'", DesciplineCheckName);
					table = db.Get_DataTable(query);

					string Id_Old = "0";
					foreach (DataRow row in table.Rows)
					{
						Id_Old = row["Id"].ToString();
					}
					//Id mới
					query = string.Format("SELECT * FROM Addin_QAQC_DesciplineCheck WHERE Name = N'{0}'", input);
					table = db.Get_DataTable(query);

					string Id_new = "0";
					foreach (DataRow row in table.Rows)
					{
						Id_new = row["Id"].ToString();
					}

					query = string.Format("SELECT * FROM Addin_QAQC_DesciplineCheckRow WHERE Id_CheckSet = '{0}'", Id_Old);
					table = db.Get_DataTable(query);
					foreach (DataRow row in table.Rows)
					{
						string idCheck = row["Id"].ToString();
						string checkname = row["CheckName"].ToString();
						string Description = row["Description"].ToString();
						string CheckResult = row["CheckResult"].ToString();
						string Failure_Message = row["Failure_Message"].ToString();
						string SetRun = row["SetRun"].ToString();
						string IsImportant = row["CheckImportant"].ToString();
						string Order = row["Order"].ToString();
						string RangeA = "";
						string RangeB = "";
						string RangeC = "";
						string RangeD = "";
						string ParaInResult = "";
						try
						{
							RangeA = row["Range_A"].ToString();
							RangeB = row["Range_B"].ToString();
							RangeC = row["Range_C"].ToString();
							RangeD = row["Range_D"].ToString();
							ParaInResult = row["ParasInResult"].ToString();
						}
						catch { }
						//Tạo db row mới
						query = $"INSERT INTO Addin_QAQC_DesciplineCheckRow (Id_CheckSet, CheckName, SetRun,Description,Failure_Message,CheckResult,[Order],[Range_A],[Range_B],[Range_C],[Range_D],[CheckImportant],[ParasInResult])" +
							$" VALUES ('{Id_new}',N'{checkname}','{SetRun}',N'{Description}',N'{Failure_Message}','{CheckResult}','{Order}','{RangeA}','{RangeB}','{RangeC}','{RangeD}','{IsImportant}','{ParaInResult}');";
						db.Execute_SQL(query);

						query = $"SELECT * FROM Addin_QAQC_DesciplineCheckRow WHERE Id_CheckSet = '{Id_Old}' AND CheckName = N'{checkname}'";
						string idcheck = "0";
						foreach (DataRow row2 in db.Get_DataTable(query).Rows)
						{
							idcheck = row2["Id"].ToString();
						}

						query = $"SELECT * FROM Addin_QAQC_DesciplineCheckRow WHERE Id_CheckSet = '{Id_new}' AND CheckName = N'{checkname}'";
						string idchecknew = "0";
						foreach (DataRow row2 in db.Get_DataTable(query).Rows)
						{
							idchecknew = row2["Id"].ToString();
						}

						//Copfilter
						query = $"SELECT * FROM Addin_QAQC_DesciplineFilter WHERE Id_CheckRow = {idCheck} ORDER BY Id_Filter ASC";
						int i = 1;
						foreach (DataRow row2 in db.Get_DataTable(query).Rows)
						{
							string Oparator = row2["Oparator"].ToString();
							string Criteria = row2["Criteria"].ToString();
							string Property = row2["Property"].ToString();
							string Condition = row2["Condition"].ToString();
							string Value = row2["Value"].ToString();

							query = $"INSERT INTO Addin_QAQC_DesciplineFilter ([Id_CheckRow],[Id_Filter],[Oparator],[Criteria],[Property],[Condition],[Value])" +
								$"VALUES ('{idchecknew}','{i}', N'{Oparator}', N'{Criteria}', N'{Property}', N'{Condition}', N'{Value}')";
							db.Execute_SQL(query);
							i = i + 1;
						}
					}

					GetCheckSet();
					DesciplineCheckName = input;

					//Đẩy lên ggsheet
					GetListFile.AddCheckSetDisciplineName(input);
				}
				catch { }

				db.Close_DB_Connection();
			}
		}

		public void DelCheckSet()
		{
			if (Admins.Contains(username))
			{
				DbConnect db = new DbConnect();
				try
				{
					DialogResult result = MessageBox.Show("This Checkset will be delete!", "Model Checker", MessageBoxButtons.OKCancel);
					if (result == System.Windows.Forms.DialogResult.Cancel) return;

					string query = $"SELECT * FROM Addin_QAQC_DesciplineCheck WHERE Name = N'{DesciplineCheckName}'";

					DataTable table = db.Get_DataTable(query);

					string IdCheckSet = "";
					foreach (DataRow row in table.Rows)
					{
						IdCheckSet = row["Id"].ToString();
					}


				}
				catch { }
			}
		}

		//Lấy list check trong bộ checkset
		public void GetListCheck()
		{
			try
			{
				try
				{
					DesciplineCheckRows = new ObservableCollection<DisciplineCheck>();
				}
				catch { }
				DbConnect db = new DbConnect();

				string query = $"SELECT * FROM Addin_QAQC_DesciplineCheck WHERE Name = N'{DesciplineCheckName}'";

				DataTable table = db.Get_DataTable(query);

				string IdCheckSet = "";
				foreach (DataRow row in table.Rows)
				{
					IdCheckSet = row["Id"].ToString();
				}

				query = $"SELECT * FROM Addin_QAQC_DesciplineCheckRow WHERE Id_CheckSet = '{IdCheckSet}' ORDER BY [Order] ASC";
				foreach (DataRow row in db.Get_DataTable(query).Rows)
				{
					DesciplineCheckRows.Add(new DisciplineCheck
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
						ListParaResult = row["ParasInResult"].ToString(),
					});
				}

				db.Close_DB_Connection();
			}
			catch { }
		}

		//Add thêm bộ check mới
		public void AddCheck()
		{
			if (Admins.Contains(username))
			{
				try
				{
					//Check Tên trong checkset
					DbConnect db = new DbConnect();

					string query = $"SELECT * FROM Addin_QAQC_DesciplineCheck WHERE Name = N'{DesciplineCheckName}'";

					DataTable table = db.Get_DataTable(query);

					string IdCheckSet = "";

					int index = DesciplineCheckRows.Select(x => x.Name).ToList().IndexOf(CheckName) + 1;
					foreach (DataRow row in table.Rows)
					{
						IdCheckSet = row["Id"].ToString();
					}

					query = $"SELECT * FROM Addin_QAQC_DesciplineCheckRow WHERE Id_CheckSet = '{IdCheckSet}'";

					foreach (DataRow row in db.Get_DataTable(query).Rows)
					{
						if (row["CheckName"].ToString() == CheckName)
						{
							CheckName = CheckName + "_Copy1";
							break;
						}
					}

					//Tạo db row mới
					query = $"INSERT INTO Addin_QAQC_DesciplineCheckRow (Id_CheckSet, CheckName, SetRun,Description,Failure_Message,[Order],[Range_A],[Range_B],[Range_C],[Range_D],[CheckImportant],[ParasInResult])" +
						$" VALUES ('{IdCheckSet}',N'{CheckName}','{Run_ThisCheck}',N'{Description}',N'{Failure_Message}','{index}','{RangeA}','{RangeB}','{RangeC}','{RangeD}','{IsImpotant}','{ParasInResult}');";

					db.Execute_SQL(query);

					string IdCheckNew = "";
					query = $"SELECT * FROM Addin_QAQC_DesciplineCheckRow WHERE Id_CheckSet = '{IdCheckSet}' AND CheckName = N'{CheckName}'";
					foreach (DataRow row in db.Get_DataTable(query).Rows)
					{
						IdCheckNew = row["Id"].ToString();
					}

					//SaveThisCheck();
					if (IdCheckProgram != "")
					{
						query = $"SELECT * FROM Addin_QAQC_DesciplineFilter WHERE Id_CheckRow = '{IdCheckProgram}'";
						foreach (DataRow row in db.Get_DataTable(query).Rows)
						{
							query = $"INSERT INTO Addin_QAQC_DesciplineFilter ([Id_CheckRow],[Oparator],[Criteria],[Property],[Condition],[Value],[Id_Filter])" +
								$"VALUES ('{IdCheckNew}', '{row["Oparator"].ToString()}', '{row["Criteria"].ToString()}', '{row["Property"].ToString()}', '{row["Condition"].ToString()}', N'{row["Value"].ToString()}','{row["Id_Filter"].ToString()}')";
							db.Execute_SQL(query);
						}
					}

					IdCheckProgram = IdCheckNew;
					//Cập nhật ListCheck
					db.Close_DB_Connection();
					GetListCheck();
				}
				catch { }
			}
		}

		//Select bộ check -> Filter
		public void SelectCheck()
		{
			if (Admins.Contains(username))
			{
				try
				{
					SaveThisCheck();
				}
				catch { }
			}
			try
			{
				DisciplineCheckSet = (DisciplineCheckSet)this.Wmain.DFrame2.Content;
				DesciplineCheckRow = (DisciplineCheck)DisciplineCheckSet.dgCheck.SelectedItem;

				string id = DesciplineCheckRow.Id.ToString();
				try
				{
					DbConnect db = new DbConnect();
					string idcheckset = "";
					string query = $"SELECT * FROM Addin_QAQC_DesciplineCheck WHERE Name = '{DesciplineCheckName}'";
					foreach (DataRow row in db.Get_DataTable(query).Rows)
					{
						idcheckset = row["Id"].ToString();
					}

					query = $"SELECT * FROM Addin_QAQC_DesciplineCheckRow WHERE Id = '{id}' And Id_CheckSet = '{idcheckset}' ";

					foreach (DataRow row in db.Get_DataTable(query).Rows)
					{
						CheckName = row["CheckName"].ToString();
						IdCheckProgram = row["Id"].ToString();
						Description = row["Description"].ToString();
						Check_Result = row["CheckResult"].ToString();
						Failure_Message = row["Failure_Message"].ToString();
						Run_ThisCheck = bool.Parse(row["SetRun"].ToString());
						IsImpotant = bool.Parse(row["CheckImportant"].ToString());
						RangeA = row["Range_A"].ToString();
						RangeB = row["Range_B"].ToString();
						RangeC = row["Range_C"].ToString();
						RangeD = row["Range_D"].ToString();
						ParasInResult = row["ParasInResult"].ToString();
					}
					db.Close_DB_Connection();
				}
				catch
				{ }

				GetListFilter();
				GetListCheck();
			}
			catch { }
		}

		//Save bộ check
		public void SaveThisCheck()
		{
			if (Admins.Contains(username))
			{
				try
				{
					DbConnect db = new DbConnect();

					string query = $"SELECT * FROM Addin_QAQC_DesciplineCheck WHERE Name = N'{DesciplineCheckName}'";

					DataTable table = db.Get_DataTable(query);

					string IdCheckSet = "";
					foreach (DataRow row in table.Rows)
					{
						IdCheckSet = row["Id"].ToString();
					}

					string IdCheck = "";

					if (IdCheckProgram == "")
					{
						query = $"SELECT * FROM Addin_QAQC_DesciplineCheckRow WHERE Id_CheckSet = '{IdCheckSet}'" +
							$"AND CheckName = N'{CheckName}'";
						foreach (DataRow row in db.Get_DataTable(query).Rows)
						{
							IdCheck = row["Id"].ToString();
						}
					}
					else
					{
						IdCheck = IdCheckProgram;
					}
					//Update This Check
					foreach (DisciplineCheck dg in DesciplineCheckRows)
					{
						query = $"UPDATE Addin_QAQC_DesciplineCheckRow SET [Order] = '{DesciplineCheckRows.IndexOf(dg) + 1}'" +
						$"WHERE [Id_CheckSet] = '{IdCheckSet}' AND [Id] = '{dg.Id}'";
						db.Execute_SQL(query);
					}

					query = $"UPDATE Addin_QAQC_DesciplineCheckRow SET CheckName = N'{CheckName}', [Description] = N'{Description}', [CheckResult] = '{Check_Result}', SetRun = '{Convert.ToByte(Run_ThisCheck).ToString()}', Failure_Message = N'{Failure_Message}'," +
						$"[Range_A] = '{RangeA}',[Range_B] = '{RangeB}',[Range_C] = '{RangeC}',[Range_D] = '{RangeD}',[CheckImportant] = '{Convert.ToByte(IsImpotant).ToString()}',[ParasInResult] = '{ParasInResult}'" +
						$"WHERE [Id_CheckSet] = '{IdCheckSet}' AND [Id] = '{IdCheck}'";
					db.Execute_SQL(query);

					try
					{
						query = $"DELETE FROM Addin_QAQC_DesciplineFilter WHERE [Id_CheckRow] = '{IdCheck}';";
						db.Execute_SQL(query);
					}
					catch { }

					foreach (DisciplineFilter df in DesciplineCheckRowFilters)
					{
						query = $"INSERT INTO Addin_QAQC_DesciplineFilter ([Id_CheckRow],[Id_Filter],[Oparator],[Criteria],[Property],[Condition],[Value])" +
							$"VALUES ('{IdCheck}','{DesciplineCheckRowFilters.IndexOf(df) + 1}', '{df.Oparator}', '{df.Criteria}', '{df.Property}', '{df.Condition}', N'{df.Value}')";
						db.Execute_SQL(query);
					}

					db.Close_DB_Connection();
				}
				catch { }
			}
		}

		public void SaveCheck()
		{
			if (Admins.Contains(username))
			{
				try
				{
					DbConnect db = new DbConnect();

					string query = $"SELECT * FROM Addin_QAQC_DesciplineCheck WHERE Name = N'{DesciplineCheckName}'";

					DataTable table = db.Get_DataTable(query);

					string IdCheckSet = "";
					foreach (DataRow row in table.Rows)
					{
						IdCheckSet = row["Id"].ToString();
					}

					string IdCheck = "";

					if (IdCheckProgram == "")
					{
						query = $"SELECT * FROM Addin_QAQC_DesciplineCheckRow WHERE Id_CheckSet = '{IdCheckSet}'" +
							$"AND CheckName = N'{CheckName}'";
						foreach (DataRow row in db.Get_DataTable(query).Rows)
						{
							IdCheck = row["Id"].ToString();
						}
					}
					else
					{
						IdCheck = IdCheckProgram;
					}
					//Update This Check
					foreach (DisciplineCheck dg in DesciplineCheckRows)
					{
						query = $"UPDATE Addin_QAQC_DesciplineCheckRow SET [Order] = '{DesciplineCheckRows.IndexOf(dg) + 1}'" +
						$"WHERE [Id_CheckSet] = '{IdCheckSet}' AND [Id] = '{dg.Id}'";
						db.Execute_SQL(query);
					}

					query = $"UPDATE Addin_QAQC_DesciplineCheckRow SET CheckName = N'{CheckName}', [Description] = N'{Description}', [CheckResult] = '{Check_Result}', SetRun = '{Convert.ToByte(Run_ThisCheck).ToString()}', Failure_Message = N'{Failure_Message}'," +
						$"[Range_A] = '{RangeA}',[Range_B] = '{RangeB}',[Range_C] = '{RangeC}',[Range_D] = '{RangeD}',[CheckImportant] = '{Convert.ToByte(IsImpotant).ToString()}', [ParasInResult] = '{ParasInResult}'" +
						$"WHERE [Id_CheckSet] = '{IdCheckSet}' AND [Id] = '{IdCheck}'";
					db.Execute_SQL(query);

					try
					{
						query = $"DELETE FROM Addin_QAQC_DesciplineFilter WHERE [Id_CheckRow] = '{IdCheck}';";
						db.Execute_SQL(query);
					}
					catch { }

					foreach (DisciplineFilter df in DesciplineCheckRowFilters)
					{
						query = $"INSERT INTO Addin_QAQC_DesciplineFilter ([Id_CheckRow],[Id_Filter],[Oparator],[Criteria],[Property],[Condition],[Value])" +
							$"VALUES ('{IdCheck}','{DesciplineCheckRowFilters.IndexOf(df) + 1}', '{df.Oparator}', '{df.Criteria}', '{df.Property}', '{df.Condition}', N'{df.Value}')";
						db.Execute_SQL(query);
					}

					db.Close_DB_Connection();
				}
				catch
				{
					MessageBox.Show("Có lỗi xảy ra!", "Model Checker");
				}
				GetListCheck();
			}
		}

		//Del bộ check del cả filter => Check lại cái này
		public void DelThisCheck()
		{
			if (Admins.Contains(username))
			{
				try
				{
					DialogResult result = MessageBox.Show("This Check will be delete!", "Model Checker", MessageBoxButtons.OKCancel);
					if (result == System.Windows.Forms.DialogResult.Cancel) return;

					DisciplineCheckSet = (DisciplineCheckSet)this.Wmain.DFrame2.Content;
					DesciplineCheckRow = (DisciplineCheck)DisciplineCheckSet.dgCheck.SelectedItem;

					DbConnect db = new DbConnect();

					string query = $"SELECT * FROM Addin_QAQC_DesciplineCheck WHERE Name = N'{DesciplineCheckName}'";

					DataTable table = db.Get_DataTable(query);

					string IdCheckSet = "";
					foreach (DataRow row in table.Rows)
					{
						IdCheckSet = row["Id"].ToString();
					}

					query = $"SELECT * FROM Addin_QAQC_DesciplineCheckRow WHERE Id_CheckSet = '{IdCheckSet}' AND CheckName = N'{CheckName}'";
					table = db.Get_DataTable(query);
					string id = "";
					foreach (DataRow row in table.Rows)
					{
						id = row["Id"].ToString();
					}

					query = $"DELETE FROM Addin_QAQC_DesciplineFilter WHERE Id_CheckRow = '{id}'";
					db.Execute_SQL(query);

					//query = $"DELETE FROM Addin_QAQC_DesciplineCheckRow WHERE Id_CheckSet = '{id}' AND Id = '{id}'";
					query = $"DELETE FROM Addin_QAQC_DesciplineCheckRow WHERE Id = '{id}'";
					db.Execute_SQL(query);

					db.Close_DB_Connection();
					GetListCheck();
				}
				catch
				{ }
			}
		}

		//Lấy list filter của bộ check
		public void GetListFilter()
		{
			try
			{
				DbConnect db = new DbConnect();

				string query = $"SELECT * FROM Addin_QAQC_DesciplineCheck WHERE Name = N'{DesciplineCheckName}'";

				DataTable table = db.Get_DataTable(query);

				string IdCheckSet = "";
				foreach (DataRow row in table.Rows)
				{
					IdCheckSet = row["Id"].ToString();
				}

				string IdCheck = "";
				query = $"SELECT * FROM Addin_QAQC_DesciplineCheckRow WHERE Id_CheckSet = '{IdCheckSet}'" +
					$"AND CheckName = N'{CheckName}'";
				table = db.Get_DataTable(query);
				foreach (DataRow row in table.Rows)
				{
					IdCheck = row["Id"].ToString();
				}

				query = $"SELECT * FROM Addin_QAQC_DesciplineFilter WHERE [Id_CheckRow] = '{IdCheck}' ORDER BY [Id_Filter] ASC";
				DesciplineCheckRowFilters = new ObservableCollection<DisciplineFilter>();
				table = db.Get_DataTable(query);
				foreach (DataRow row in table.Rows)
				{
					DesciplineCheckRowFilters.Add(new DisciplineFilter
					{
						ID = int.Parse(row["Id_Filter"].ToString()),
						Oparator = row["Oparator"].ToString(),
						Criteria = row["Criteria"].ToString(),
						Property = row["Property"].ToString(),
						Properties = GetProperties(row["Criteria"].ToString(), row["Property"].ToString()),
						Condition = row["Condition"].ToString(),
						Value = row["Value"].ToString()
					});
				}

				db.Close_DB_Connection();
			}
			catch { }
		}

		//Thêm filter vào bộ check
		public void AddFilter()
		{
			if (Admins.Contains(username))
			{
				try
				{
					if (DesciplineCheckRowFilters.Count > 0)
					{
						DesciplineCheckRowFilters.Add(new DisciplineFilter() { Oparator = "AND" });
					}
					else
					{
						DesciplineCheckRowFilters.Add(new DisciplineFilter());
					}
				}
				catch { }
			}
		}

		//Xóa filter trong bộ check
		public void DelFilter()
		{
			if (Admins.Contains(username))
			{
				try
				{
					DisciplineCheckSet = (DisciplineCheckSet)this.Wmain.DFrame2.Content;
					DesciplineCheckRowFilter = (DisciplineFilter)DisciplineCheckSet.dgFilter.SelectedItem;

					DesciplineCheckRowFilters.Remove(DesciplineCheckRowFilter);
				}
				catch { }
			}
		}

		public void MoveDown()
		{
			if (Admins.Contains(username))
			{
				try
				{
					DisciplineCheckSet = (DisciplineCheckSet)this.Wmain.DFrame2.Content;
					DesciplineCheckRowFilter = (DisciplineFilter)DisciplineCheckSet.dgFilter.SelectedItem;

					string Property = DesciplineCheckRowFilter.Property;
					int index = DesciplineCheckRowFilters.IndexOf(DesciplineCheckRowFilter);
					DisciplineFilter k = DesciplineCheckRowFilters[index + 1];
					DesciplineCheckRowFilters[index + 1] = DesciplineCheckRowFilter;
					DesciplineCheckRowFilters[index + 1].Property = Property;
					DesciplineCheckRowFilters[index] = k;

					ObservableCollection<DisciplineFilter> a = new ObservableCollection<DisciplineFilter>();
					a = DesciplineCheckRowFilters;

					DesciplineCheckRowFilters = new ObservableCollection<DisciplineFilter>();
					foreach (DisciplineFilter row in a)
					{
						DesciplineCheckRowFilters.Add(new DisciplineFilter
						{
							ID = row.ID,
							Oparator = row.Oparator,
							Criteria = row.Criteria,
							Properties = GetProperties(row.Criteria, row.Property),
							Property = row.Property,
							Condition = row.Condition,
							Value = row.Value
						});
					}
				}
				catch { }
			}
		}

		public void MoveUp()
		{
			if (Admins.Contains(username))
			{
				try
				{
					DisciplineCheckSet = (DisciplineCheckSet)this.Wmain.DFrame2.Content;
					DesciplineCheckRowFilter = (DisciplineFilter)DisciplineCheckSet.dgFilter.SelectedItem;

					string Property = DesciplineCheckRowFilter.Property;
					int index = DesciplineCheckRowFilters.IndexOf(DesciplineCheckRowFilter);
					DisciplineFilter k = DesciplineCheckRowFilters[index - 1];
					DesciplineCheckRowFilters[index - 1] = DesciplineCheckRowFilter;
					DesciplineCheckRowFilters[index - 1].Property = Property;
					DesciplineCheckRowFilters[index] = k;

					ObservableCollection<DisciplineFilter> a = new ObservableCollection<DisciplineFilter>();
					a = DesciplineCheckRowFilters;

					DesciplineCheckRowFilters = new ObservableCollection<DisciplineFilter>();
					foreach (DisciplineFilter row in a)
					{
						DesciplineCheckRowFilters.Add(new DisciplineFilter
						{
							ID = row.ID,
							Oparator = row.Oparator,
							Criteria = row.Criteria,
							Property = row.Property,
							Properties = GetProperties(row.Criteria, row.Property),
							Condition = row.Condition,
							Value = row.Value
						});
					}
				}
				catch { }
			}
		}
		//OK
		public void BtnChangeCriteriaFilter()
		{
			try
			{
				DisciplineCheckSet = (DisciplineCheckSet)this.Wmain.DFrame2.Content;
				DesciplineCheckRowFilter = (DisciplineFilter)DisciplineCheckSet.dgFilter.SelectedItem;

				DesciplineCheckRowFilter.Properties = new ObservableCollection<string>();
				switch (DesciplineCheckRowFilter.Criteria)
				{
					case "CATEGORY":
						DesciplineCheckRowFilter.Properties = ListCategories;
						break;
					case "FAMILY":
						DesciplineCheckRowFilter.Properties = new ObservableCollection<string>()
						{
							"Name",
							"Is In-Place"
						};
						break;
					case "LEVEL":
						DesciplineCheckRowFilter.Properties = new ObservableCollection<string>()
						{
							"Name",
							"Elevation"
						};
						break;
					case "API PARAMETER":
						DesciplineCheckRowFilter.Properties = ListBuilinParameter;
						break;
					case "PARAMETER":
						DesciplineCheckRowFilter.Properties = ListParameter;
						break;
					case "STRUCTURAL TYPE":
						DesciplineCheckRowFilter.Properties = new ObservableCollection<string>()
						{
							"Name"
						};
						break;
					case "TYPE":
						DesciplineCheckRowFilter.Properties = new ObservableCollection<string>()
						{
							"Name"
						};
						break;
					case "TYPE OR INSTANCE":
						DesciplineCheckRowFilter.Properties = new ObservableCollection<string>()
						{
							"Is Element Type"
						};
						break;
					case "ROOM":
						DesciplineCheckRowFilter.Properties = new ObservableCollection<string>()
						{
							"Name", "Number", "Is Defined"
						};
						break;
					case "VIEW":
						DesciplineCheckRowFilter.Properties = new ObservableCollection<string>()
						{
							"Name"
						};
						break;
					case "WORKSET":
						DesciplineCheckRowFilter.Properties = new ObservableCollection<string>()
						{
							"Name"
						};
						break;
					case "WARNING":
						DesciplineCheckRowFilter.Properties = new ObservableCollection<string>()
						{
							"Name"
						};
						break;
					case "LOCATION":
						DesciplineCheckRowFilter.Properties = new ObservableCollection<string>()
						{
							"With Grids"
						};
						break;
					case "BASE AND TOP LEVEL":
						DesciplineCheckRowFilter.Properties = new ObservableCollection<string>()
						{
							"Is Consecutive"
						};
						break;
					case "IsMonitoringLinkElement":
						DesciplineCheckRowFilter.Properties = new ObservableCollection<string>()
						{
							"True", "False"
						};
						break;
					default:
						break;
				}
			}
			catch { }
		}

		public ObservableCollection<string> GetProperties(string Criteria, string property)
		{

			ObservableCollection<string> Properties = new ObservableCollection<string>();
			switch (Criteria)
			{
				case "CATEGORY":
					Properties = ListCategories;
					break;
				case "FAMILY":
					Properties = new ObservableCollection<string>()
						{
							"Name",
							"Is In-Place"
						};
					break;
				case "LEVEL":
					Properties = new ObservableCollection<string>()
						{
							"Name",
							"Elevation"
						};
					break;
				case "API PARAMETER":
					Properties = ListBuilinParameter;
					break;
				case "PARAMETER":
					Properties = ListParameter;
					break;
				case "STRUCTURAL TYPE":
					Properties = new ObservableCollection<string>()
						{
							"Name"
						};
					break;
				case "TYPE":
					Properties = new ObservableCollection<string>()
						{
							"Name"
						};
					break;
				case "TYPE OR INSTANCE":
					Properties = new ObservableCollection<string>()
						{
							"Is Element Type"
						};
					break;
				case "VIEW":
					Properties = new ObservableCollection<string>()
						{
							"Name"
						};
					break;
				case "ROOM":
					Properties = new ObservableCollection<string>()
						{
							"Name","Number","Is Defined"
						};
					break;
				case "WORKSET":
					Properties = new ObservableCollection<string>()
						{
							"Name"
						};
					break;
				case "WARNING":
					Properties = new ObservableCollection<string>()
						{
							"Name"
						};
					break;
				case "LOCATION":
					Properties = new ObservableCollection<string>()
						{
							"With Grids"
						};
					break;
				case "BASE AND TOP LEVEL":
					Properties = new ObservableCollection<string>()
						{
							"Is Consecutive"
						};
					break;
				case "IsMonitoringLinkElement":
					Properties = new ObservableCollection<string>()
						{
							"True", "False"
						};
					break;
				default:
					break;
			}

			Properties.Add(property);
			Properties = new ObservableCollection<string>(Properties.Distinct().ToList());



			return Properties;
		}
		//OK
		public void BtnChangeConditions()
		{
			try
			{
				DisciplineCheckSet = (DisciplineCheckSet)this.Wmain.DFrame2.Content;
				DesciplineCheckRowFilter = (DisciplineFilter)DisciplineCheckSet.dgFilter.SelectedItem;

				switch (DesciplineCheckRowFilter.Condition)
				{
					case "Has Value":
						DesciplineCheckRowFilter.Value = "true";
						break;
					case "Has No Value":
						DesciplineCheckRowFilter.Value = "true";
						break;
					case "Defined":
						DesciplineCheckRowFilter.Value = "true";
						break;
					case "Undefined":
						DesciplineCheckRowFilter.Value = "true";
						break;
					default:
						break;
				}
			}
			catch { }
		}

		public void ConditionSource()
		{
			try
			{
				DisciplineCheckSet = (DisciplineCheckSet)this.Wmain.DFrame2.Content;
				DesciplineCheckRowFilter = (DisciplineFilter)DisciplineCheckSet.dgFilter.SelectedItem;

				//ObservableCollection<string> Condition = new ObservableCollection<string>();

				if (DesciplineCheckRowFilter.Criteria == "CATEGORY")
				{
					DesciplineCheckRowFilter.Conditions = new ObservableCollection<string>()
						{
							"Include",
						};
				}
				else if (DesciplineCheckRowFilter.Criteria == "FAMILY" && DesciplineCheckRowFilter.Property == "Name")
				{
					DesciplineCheckRowFilter.Conditions = new ObservableCollection<string>()
				{
				"=",
				"!=",
				"Contains",
				"Does Not Contain",
				"Matches Regex",
				"Does Not Match RegEx",
				"Matches with Regex group"};
				}
				else if (DesciplineCheckRowFilter.Criteria == "FAMILY" && DesciplineCheckRowFilter.Property == "Is In-Place")
				{
					DesciplineCheckRowFilter.Conditions = new ObservableCollection<string>()
						{
							"=","!="
						};
				}
				else if (DesciplineCheckRowFilter.Criteria == "LEVEL" && DesciplineCheckRowFilter.Property == "Name")
				{
					DesciplineCheckRowFilter.Conditions = new ObservableCollection<string>()
				{
				"=",
				"!=",
				"Contains",
				"Does Not Contain",
				"Matches Regex",
				"Does Not Match RegEx",
				"Matches with Regex group",
				"Defined",
				"Undefined"};
				}
				else if (DesciplineCheckRowFilter.Criteria == "LEVEL" && DesciplineCheckRowFilter.Property == "Elevation")
				{
					DesciplineCheckRowFilter.Conditions = new ObservableCollection<string>()
				{
					"=",
					"!=",
					">",
					"<",
					">=",
					"<="};
				}
				else if (DesciplineCheckRowFilter.Criteria == "ROOM" && DesciplineCheckRowFilter.Property == "Is Defined")
				{
					DesciplineCheckRowFilter.Conditions = new ObservableCollection<string>()
				{
					"=",
					"!="};
				}
				else if (DesciplineCheckRowFilter.Criteria == "ROOM")
				{
					DesciplineCheckRowFilter.Conditions = new ObservableCollection<string>()
				{
				"=",
				"!=",
				"Contains",
				"Does Not Contain",
				"Matches Regex",
				"Does Not Match RegEx",
				"Matches with Regex group",
				"Defined",
				"Undefined"};
				}
				else if (DesciplineCheckRowFilter.Criteria == "STRUCTURAL TYPE" || DesciplineCheckRowFilter.Criteria == "WARNING")
				{
					DesciplineCheckRowFilter.Conditions = new ObservableCollection<string>()
				{
				"="};
				}
				else if (DesciplineCheckRowFilter.Criteria == "TYPE" || DesciplineCheckRowFilter.Criteria == "WORKSET")
				{
					DesciplineCheckRowFilter.Conditions = new ObservableCollection<string>()
				{
				"=",
				"!=",
				"Contains",
				"Does Not Contain",
				"Matches Regex",
				"Does Not Match RegEx",
				"Matches with Regex group",
				"Does Not Match RegEx group"};
				}
				else if (DesciplineCheckRowFilter.Criteria == "TYPE OR INSTANCE")
				{
					DesciplineCheckRowFilter.Conditions = new ObservableCollection<string>()
				{
				"=","!=","Type From Instance"
				};
				}
				else if (DesciplineCheckRowFilter.Criteria == "VIEW")
				{
					DesciplineCheckRowFilter.Conditions = new ObservableCollection<string>()
				{
				"=",
				"!=",
				"Contains",
				"Does Not Contain",
				"Matches Regex",
				"Does Not Match RegEx",
				"Matches with Regex group",
				"Defined",
				"Undefined"};
				}
				else if (DesciplineCheckRowFilter.Criteria == "LOCATION")
				{
					DesciplineCheckRowFilter.Conditions = new ObservableCollection<string>()
				{
				"<"};
				}
				else if (DesciplineCheckRowFilter.Criteria == "BASE AND TOP LEVEL")
				{
					DesciplineCheckRowFilter.Conditions = new ObservableCollection<string>()
				{
				"=","!="};
				}
				else
				{
					DesciplineCheckRowFilter.Conditions = new ObservableCollection<string>(){
					"=",
					"!=",
					">",
					"<",
					">=",
					"<=",
					"Contains",
					"Does Not Contain",
					"Matches Regex",
					"Does Not Match RegEx",
					"Contains with Regex group",
					"Does Not Contains RegEx group",
					"Matches with Regex group",
					"Does Not Match RegEx group",
					"Has Value",
					"Has No Value",
					"Defined",
					"Undefined",
					"Include",
					"Same value, yet different type",
					"Same value, yet different para",
					"Type From Instance"};
				}
			}
			catch { }
		}

		//CheckSet => List<Check> => List<List<Filter>>

		public void BtnMoveDown_Check(string checkname)
		{
			DbConnect db = new DbConnect();
			try
			{

				string query = $"SELECT * FROM Addin_QAQC_DesciplineCheck WHERE Name = N'{DesciplineCheckName}'";

				DataTable table = db.Get_DataTable(query);

				string IdCheckSet = "";
				foreach (DataRow row in table.Rows)
				{
					IdCheckSet = row["Id"].ToString();
				}

				string IdCheck = "";


				query = $"SELECT * FROM Addin_QAQC_DesciplineCheckRow WHERE Id_CheckSet = '{IdCheckSet}'" +
					$"AND CheckName = N'{checkname}'";
				foreach (DataRow row in db.Get_DataTable(query).Rows)
				{
					IdCheck = row["Id"].ToString();
				}


				DisciplineCheck dg = DesciplineCheckRows.Where(x => x.Id == int.Parse(IdCheck)).FirstOrDefault();
				int index1 = DesciplineCheckRows.IndexOf(dg);
				DisciplineCheck dg2 = DesciplineCheckRows[index1 + 1];

				query = $"UPDATE Addin_QAQC_DesciplineCheckRow SET [Order] = '{index1 + 2}'" +
					$"WHERE Id = '{dg.Id}'";
				db.Execute_SQL(query);

				query = $"UPDATE Addin_QAQC_DesciplineCheckRow SET [Order] = '{index1 + 1}'" +
				$"WHERE Id = '{dg2.Id}'";
				db.Execute_SQL(query);

				GetListCheck();
			}
			catch { }
			db.Close_DB_Connection();
		}

		public void BtnMoveUp_Check(string checkname)
		{
			DbConnect db = new DbConnect();
			try
			{

				string query = $"SELECT * FROM Addin_QAQC_DesciplineCheck WHERE Name = N'{DesciplineCheckName}'";

				DataTable table = db.Get_DataTable(query);

				string IdCheckSet = "";
				foreach (DataRow row in table.Rows)
				{
					IdCheckSet = row["Id"].ToString();
				}

				string IdCheck = "";

				query = $"SELECT * FROM Addin_QAQC_DesciplineCheckRow WHERE Id_CheckSet = '{IdCheckSet}'" +
					$"AND CheckName = N'{checkname}'";
				foreach (DataRow row in db.Get_DataTable(query).Rows)
				{
					IdCheck = row["Id"].ToString();
				}

				DisciplineCheck dg = DesciplineCheckRows.Where(x => x.Id == int.Parse(IdCheck)).FirstOrDefault();
				int index1 = DesciplineCheckRows.IndexOf(dg);
				DisciplineCheck dg2 = DesciplineCheckRows[index1 - 1];

				query = $"UPDATE Addin_QAQC_DesciplineCheckRow SET [Order] = '{index1}'" +
					$"WHERE Id = '{dg.Id}'";
				db.Execute_SQL(query);

				query = $"UPDATE Addin_QAQC_DesciplineCheckRow SET [Order] = '{index1 + 1}'" +
				$"WHERE Id = '{dg2.Id}'";
				db.Execute_SQL(query);

				GetListCheck();
			}
			catch { }
			db.Close_DB_Connection();
		}

		public void BtnFilterProperty()
		{
			//try
			//{
			//	DisciplineCheckSet = (DisciplineCheckSet)this.Wmain.DFrame2.Content;
			//	DesciplineCheckRowFilter = (DisciplineFilter)DisciplineCheckSet.dgFilter.SelectedItem;

			//	string a = DesciplineCheckRowFilter.Property;
			//	ObservableCollection<string> Properties = new ObservableCollection<string>();
			//	foreach (string Property in DesciplineCheckRowFilter.Properties)
			//	{
			//		if (Property.Contains(a))
			//		{
			//			Properties.Add(Property);
			//		}
			//	}
			//	DesciplineCheckRowFilter.Properties = new ObservableCollection<string>();
			//	DesciplineCheckRowFilter.Properties = Properties;
			//}
			//catch { }
		}

		public void BtnCheckRangeA(string RangeA)
		{
			//Ghi 1 số hoặc dạng "0-150"
			try
			{
				//Check cú pháp rangeA
				if (RangeA == "")
				{
					return;
				}
				else
				{
					double range = 0;
					bool a = double.TryParse(RangeA, out range);
					if (a)
					{
						if (double.Parse(RangeA) >= 0)
						{
							return;
						}
						else
						{
							MessageBox.Show("Input Fail");
						}
					}
					else
					{
						Regex rg = new Regex(@"^(?<Somin>[0-9]+)-(?<Somax>[0-9]+)$");
						if (rg.Match(RangeA).Success)
						{
							int somin = 0;
							int somax = 0;
							foreach (Match result in rg.Matches(rangeA))
							{
								somin = int.Parse(result.Groups["Somin"].ToString());
								somax = int.Parse(result.Groups["Somax"].ToString());
							}
						}
						else
						{
							MessageBox.Show("Input Fail", "Model Checker");
						}
					}
				}
			}
			catch { }
		}
		#endregion
	}

}
