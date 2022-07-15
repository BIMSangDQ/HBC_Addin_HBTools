using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Forms;
using Autodesk.Revit.DB;
using Autodesk.Revit.DB.Structure;
using Autodesk.Revit.UI.Selection;
using BeyCons.Core.Extensions;
using BeyCons.Core.FormUtils;
using BeyCons.Core.FormUtils.ControlViews;
using BeyCons.Core.FormUtils.Reports;
using BeyCons.Core.Libraries.Geometries;
using BeyCons.Core.RevitUtils.DataUtils;
using HoaBinhTools.BIMQAQC.ModelChecker.ViewModels;
using HoaBinhTools.BIMQAQC.ModelChecker.Views.ProgessBar;
using HoaBinhTools.BIMQAQC.QAQC_Quantity.Models;
using HoaBinhTools.BIMQAQC.QAQC_Quantity.Views;
using MoreLinq;
using Utils;
using HoaBinhTools.BIMQAQC.ModelChecker.ViewModels;
using System.Text.RegularExpressions;

namespace HoaBinhTools.BIMQAQC.QAQC_Quantity.ViewModels
{
	public class QuantityStrViewModel : ViewModelBase
	{
		#region feild
		public QuantityStr Wmain { get; set; }

		public ObservableCollection<string> CategoryActive { get; set; } = new ObservableCollection<string>() { "Structural Framing", "Floors", "Structural Columns", "Columns", "Walls", "Generic Models", "Structural Foundations", "Stairs", "Wall Sweeps", "Doors", "Windows", "Ramps", "Roofs" };

		private Options options = new Options() { DetailLevel = ViewDetailLevel.Fine };

		public BasicFilter BasicFilters { get; set; }

		private ObservableCollection<CategoryOptions> categoryOptions;
		public ObservableCollection<CategoryOptions> CategoryOptions
		{
			get { return categoryOptions ?? (categoryOptions = new ObservableCollection<CategoryOptions>()); }
			set
			{
				categoryOptions = value;
				OnPropertyChanged();
			}
		}

		private ObservableCollection<DetectOverlap> detectOverlaps;
		public ObservableCollection<DetectOverlap> DetectOverlaps
		{
			get
			{
				return detectOverlaps;
			}
			set
			{
				detectOverlaps = value;
				OnPropertyChanged("DetectOverlaps");
			}
		}

		private ObservableCollection<ExcludeFilters> _ExcludeFilters;
		public ObservableCollection<ExcludeFilters> ExcludeFilters
		{
			get
			{
				return _ExcludeFilters;
			}
			set
			{
				_ExcludeFilters = value;
				OnPropertyChanged("ExcludeFilters");
			}
		}

		private ExcludeFilters _ExcludeFilter;
		public ExcludeFilters ExcludeFilter
		{
			get
			{
				return _ExcludeFilter;
			}
			set
			{
				_ExcludeFilter = value;
				OnPropertyChanged("ExcludeFilter");
			}
		}

		private DetectOverlap detectElement;
		public DetectOverlap DetectElement
		{
			get
			{
				return detectElement;
			}
			set
			{
				detectElement = value;
				OnPropertyChanged("DetectElement");
			}
		}

		private string timeSpan;
		public string TimeSpan
		{
			get
			{
				return timeSpan;
			}
			set
			{
				timeSpan = value;
				OnPropertyChanged("TimeSpan");
			}
		}
		public object ElementsOfRevit
		{
			get
			{
				return GetElementsFromProperty(CategoryOptions);
			}
			set { }
		}

		private bool isDetectOverlap = false;
		public bool IsDetectOverlap
		{
			get
			{
				return isDetectOverlap;
			}
			set
			{
				isDetectOverlap = value;
				OnPropertyChanged("IsDetectOverlap");
			}
		}

		private bool isUnion = false;
		public bool IsUnion
		{
			get
			{
				return isUnion;
			}
			set
			{
				isUnion = value;
				OnPropertyChanged("IsUnion");
			}
		}

		private bool isSwitchJoin = false;
		public bool IsSwitchJoin
		{
			get
			{
				return isSwitchJoin;
			}
			set
			{
				isSwitchJoin = value;
				OnPropertyChanged("IsSwitchJoin");
			}
		}

		private bool isEnableButton;
		public bool IsEnableButton
		{
			get
			{
				return isEnableButton;
			}
			set
			{
				isEnableButton = value;
				OnPropertyChanged("IsEnableButton");
			}
		}

		private double geometryVolume;
		public double GeometryVolume
		{
			get
			{
				return geometryVolume;
			}
			set
			{
				geometryVolume = value;
				OnPropertyChanged("GeometryVolume");
			}
		}

		private double deviation;
		public double Deviation
		{
			get
			{
				return deviation;
			}
			set
			{
				deviation = value;
				OnPropertyChanged("Deviation");
			}
		}

		private double unionVolume;
		public double UnionVolume
		{
			get
			{
				return unionVolume;
			}
			set
			{
				unionVolume = value;
				OnPropertyChanged("UnionVolume");
			}
		}

		private double sumLackofConcrete;
		public double SumLackofConcrete
		{
			get
			{
				return sumLackofConcrete;
			}
			set
			{
				sumLackofConcrete = value;
				OnPropertyChanged("SumLackofConcrete");
			}
		}

		public int IndexUnion { get; set; } = 1;

		private int selectElementCount;
		public int SelectElementCount
		{
			get
			{
				return selectElementCount;
			}
			set
			{
				selectElementCount = value;
				OnPropertyChanged("SelectElementCount");
			}
		}

		private int elementCount;
		public int ElementCount
		{
			get
			{
				return elementCount;
			}
			set
			{
				elementCount = value;
				OnPropertyChanged("ElementCount");
			}
		}

		private int elementOverlapCount = 0;
		public int ElementOverlapCount
		{
			get
			{
				return elementOverlapCount;
			}
			set
			{
				elementOverlapCount = value;
				OnPropertyChanged("ElementOverlapCount");
			}
		}

		private double sumVolumneOverlap;
		public double SumVolumneOverlap
		{
			get
			{
				return sumVolumneOverlap;
			}
			set
			{
				sumVolumneOverlap = value;
				OnPropertyChanged("SumVolumneOverlap");
			}
		}

		private DateTime _t1;
		public DateTime t1
		{
			get
			{
				return _t1;
			}
			set
			{
				_t1 = value;
				OnPropertyChanged("t1");
			}
		}

		public string FullPath { get; set; } = string.Empty;

		public Utils.RelayCommand btnCheckModel { get; set; }
		public Utils.RelayCommand btnCreateTrusted { get; set; }
		public Utils.RelayCommand btnFindElements { get; set; }
		public Utils.RelayCommand btnExport { get; set; }
		public Utils.RelayCommand btnImport { get; set; }
		public Utils.RelayCommand btnAddExclude { get; set; }
		public Utils.RelayCommand btnChangeCriteriaFilter { get; set; }
		public Utils.RelayCommand btnSaveSetting { get; set; }
		public Utils.RelayCommand btnIsolateAll { get; set; }
		public Utils.RelayCommand btndgfilterDelete { get; set; }
		#endregion

		private bool isRunDetectOverLap;
		public bool IsRunDetectOverLap
		{
			get
			{
				return isRunDetectOverLap;
			}
			set
			{
				isRunDetectOverLap = value;
				OnPropertyChanged("IsRunDetectOverLap");
			}
		}

		private bool isRunCheckVoid;
		public bool IsRunCheckVoid
		{
			get
			{
				return isRunCheckVoid;
			}
			set
			{
				isRunCheckVoid = value;
				OnPropertyChanged("IsRunCheckVoid");
			}
		}

		public QuantityStrViewModel()
		{
			btnCheckModel = new Utils.RelayCommand(BtnCheckModel);

			btnCreateTrusted = new Utils.RelayCommand(BtnCreateTrusted);

			btnFindElements = new Utils.RelayCommand(BtnFindElement);

			btnExport = new Utils.RelayCommand(BtnExport);

			btnImport = new Utils.RelayCommand(BtnImport);

			btnAddExclude = new Utils.RelayCommand(BtnAddExclude);

			btnChangeCriteriaFilter = new Utils.RelayCommand(BtnChangeCriteriaFilter);

			btnSaveSetting = new Utils.RelayCommand(BtnSaveSetting);

			btnIsolateAll = new Utils.RelayCommand(BtnIsolateAll);

			btndgfilterDelete = new Utils.RelayCommand(BtndgfilterDelete);

			DetectOverlaps = new ObservableCollection<DetectOverlap>();

			ExcludeFilters = new ObservableCollection<ExcludeFilters>();

			Excute();
		}

		public void Excute()
		{
			Wmain = new QuantityStr(this);

			GetSetting();

			Wmain.Show();
		}

		public void BtnExport()
		{
			try
			{
				ToExcel.ExportToExcelOverLap(DetectOverlaps);
			}
			catch
			{ }

		}

		public void BtnImport()
		{
			try
			{
				DetectOverlaps = new ObservableCollection<DetectOverlap>();
				OpenFileDialog openFileDialog1 = new OpenFileDialog
				{
					InitialDirectory = @"D:\",
					Title = "Browse Excel Files",

					CheckFileExists = true,
					CheckPathExists = true,

					DefaultExt = "xlsx",
					Filter = "excel files (*.xlsx)|*.xlsx",
					FilterIndex = 2,
					RestoreDirectory = true,

					ReadOnlyChecked = true,
					ShowReadOnly = true
				};

				if (openFileDialog1.ShowDialog() == DialogResult.OK)
				{
					string filename = openFileDialog1.FileName;

					Microsoft.Office.Interop.Excel.Application xlApp = new Microsoft.Office.Interop.Excel.Application();
					Microsoft.Office.Interop.Excel.Workbook xlWorkbook = xlApp.Workbooks.Open(filename);
					Microsoft.Office.Interop.Excel._Worksheet xlWorksheet = xlWorkbook.Sheets["OverLap"];
					Microsoft.Office.Interop.Excel.Range xlRange = xlWorksheet.UsedRange;

					int rowCount = xlRange.Rows.Count;
					int colCount = xlRange.Columns.Count;
					//(string)(xlWorksheet.Cells[i, 1] as Microsoft.Office.Interop.Excel.Range).Value
					for (int i = 2; i <= rowCount; i++)
					{
						ElementId Element1 = new ElementId(int.Parse(xlWorksheet.Cells[i, 1].Value2.ToString()));
						string dE1 = xlWorksheet.Cells[i, 2].Value2.ToString();
						ElementId Element2 = new ElementId(-1);
						string dE2 = "";
						try
						{
							Element2 = new ElementId(int.Parse(xlWorksheet.Cells[i, 3].Value2.ToString()));
							dE2 = xlWorksheet.Cells[i, 4].Value2.ToString();
						}
						catch { }
						double OverlapVl = double.Parse(xlWorksheet.Cells[i, 5].Value2.ToString());

						DetectOverlaps.Add(new DetectOverlap
						{
							Element1 = Element1,
							Element1CateGory = dE1,
							Element2 = Element2,
							Element2CateGory = dE2,
							OverlapVolume = OverlapVl,
						});

					}

					//cleanup
					GC.Collect();
					GC.WaitForPendingFinalizers();

					Marshal.ReleaseComObject(xlRange);
					Marshal.ReleaseComObject(xlWorksheet);

					//close and release
					xlWorkbook.Close();
					Marshal.ReleaseComObject(xlWorkbook);

					//quit and release
					xlApp.Quit();
					Marshal.ReleaseComObject(xlApp);
				}
			}
			catch { }
		}

		public void BtnCheckModel()
		{
			Wmain.Hide();
			DialogResult result = DialogResult.No;
			IsRunDetectOverLap = false;
			IsRunCheckVoid = false;
			t1 = DateTime.Now;
			try
			{
				try
				{
					ModelPath modelPath = null;
					try
					{
						modelPath = ActiveData.Document.GetWorksharingCentralModelPath();
					}
					catch { }
					if (modelPath != null)
					{
						FullPath = Path.Combine(Path.GetDirectoryName(ModelPathUtils.ConvertModelPathToUserVisiblePath(modelPath)), HoaBinhTools.BIMQAQC.QAQC_Quantity.Models.Folders.RevitTemp, $"{ActiveData.Document.Title}_{typeof(SaveModel).Name}.json");
					}
					else
					{
						FullPath = Path.Combine(Path.GetDirectoryName(ActiveData.Document.PathName), HoaBinhTools.BIMQAQC.QAQC_Quantity.Models.Folders.BackupModel, $"{ActiveData.Document.Title}_{typeof(SaveModel).Name}.json");
					}
				}
				catch (Autodesk.Revit.Exceptions.InvalidOperationException)
				{
					if (ActiveData.Document.PathName != string.Empty)
					{
						FullPath = Path.Combine(Path.GetDirectoryName(ActiveData.Document.PathName), HoaBinhTools.BIMQAQC.QAQC_Quantity.Models.Folders.BackupModel, $"{ActiveData.Document.Title}_{typeof(SaveModel).Name}.json");
					}
				}

				if ("" == "")
				{
					try
					{
						List<Element> ListElement = ActiveData.Selection.PickObjects(ObjectType.Element, "Select elements in your project.")
							.Select(x => ActiveData.Document.GetElement(x)).ToList();
						try
						{
							#region Tính toán số phần tử trong file
							FilteredElementCollector filteredElementCollector = new FilteredElementCollector(ActiveData.Document);
							IList<ElementFilter> list = new List<ElementFilter>();

							List<BuiltInCategory> builtInCategories = new List<BuiltInCategory>()
								{
								BuiltInCategory.OST_Columns,
								BuiltInCategory.OST_GenericModel,
								BuiltInCategory.OST_StructuralColumns,
								BuiltInCategory.OST_StructuralFoundation,
								BuiltInCategory.OST_StructuralFraming,
								BuiltInCategory.OST_Walls,
								BuiltInCategory.OST_Stairs,
								BuiltInCategory.OST_Floors,
								};

							foreach (BuiltInCategory num in builtInCategories)
							{
								BuiltInCategory builtInCategory = num;
								list.Add(new ElementCategoryFilter(builtInCategory));
							}
							LogicalOrFilter logicalOrFilter = new LogicalOrFilter(list);
							filteredElementCollector.WherePasses(logicalOrFilter);

							List<Element> FilterElements = filteredElementCollector.WhereElementIsNotElementType().ToList<Element>();

							int Count_Formwork = FilterElements.Where(x => x.LookupParameter("HB_Formwork_Category").AsValueString() != null).Count();

							#endregion

							SelectElementCount = ListElement.Count;
							ElementCount = (FilterElements.Count - Count_Formwork);

							double VolumebyParameter = 0;

							if (((double)SelectElementCount / ElementCount) < 0.8)
							{
								result = System.Windows.Forms.MessageBox.Show($"Số phần tử được chọn {SelectElementCount}/{ElementCount} chưa đủ để tạo chứng nhận sau khi check." +
									$"\nBạn có muốn tiếp tục không?", "BIM_QAQC", MessageBoxButtons.YesNo);
								if (result == DialogResult.No)
								{
									Wmain.Show();
									return;
								}
							}
							else
							{
								result = System.Windows.Forms.MessageBox.Show("Kết quả sẽ được ghi nhận sau khi chạy.",
									"BIM_QAQC", MessageBoxButtons.YesNo);
								if (result == DialogResult.No)
								{
									Wmain.Show();
									return;
								}
							}
							#region Tính toán theo parameter

							foreach (Element e in ListElement)
							{
								foreach (string c in CategoryActive)
								{
									if (e.Category.Name == c)
									{
										try
										{
											double VolumeElement = 0;
											List<Solid> Solids = GeometryUtils.GetSolidsFromInstanceElement(e, options, true);
											foreach (Solid s in Solids)
											{
												VolumeElement += s.Volume * 0.3048 * 0.3048 * 0.3048;
											}

											VolumebyParameter += VolumeElement;
										}
										catch (Exception ex)
										{ }
									}
								}
							}

							GeometryVolume = Math.Round(VolumebyParameter, 3);
							#endregion

						}
						catch { }
						try
						{
							if (IsDetectOverlap)
								DetectOverlap(ListElement);
						}
						catch { }
						try
						{
							if (IsSwitchJoin)
								SwitchJoin(ListElement);
						}
						catch { }
					}
					catch (Exception ex)
					{
					}
				}
				else
				{ }
			}
			catch { }
			CategoryOptions = null;
			DateTime t2 = DateTime.Now;
			TimeSpan = (t1 - t2).ToString(@"hh\:mm\:ss");
			//Wmain.Show();
		}

		public void BtnCreateTrusted()
		{
			if (IsRunDetectOverLap == false)
			{
				System.Windows.MessageBox.Show("Vui lòng kiểm tra Overlap trước khi tạo chứng nhận.", "BIM_QAQC");
				return;
			}

			if (IsRunCheckVoid == false)
			{
				System.Windows.MessageBox.Show("Vui lòng kiểm tra Void trước khi tạo chứng nhận.", "BIM_QAQC");
				return;
			}

			if ((SelectElementCount / ElementCount) < 0.8)
			{
				System.Windows.MessageBox.Show("Vui lòng chọn nhiều đối tượng hơn để kết quả được chính xác hơn.", "BIM_QAQC");
				return;
			}

			//PutResult2googleSheet();
		}

		public void BtnFindElement()
		{
			try
			{
				List<ElementId> listId = new List<ElementId>();

				listId.Add(DetectElement.Element1);
				try
				{
					if (DetectElement.Element2 == null || DetectElement.Element2.IntegerValue == -1)
					{ }
					else
					{
						listId.Add(DetectElement.Element2);
					}
				}
				catch { }

				Action action = new Action(() =>
				{
					using (Transaction trans = new Transaction(ActiveData.Document))
					{
						TransactionStatus transactionStatus = trans.Start("Transaction Group");

						if (ActiveData.ActiveView.IsTemporaryHideIsolateActive())
						{
							TemporaryViewMode tempView = TemporaryViewMode.TemporaryHideIsolate;

							ActiveData.ActiveView.DisableTemporaryViewMode(tempView);
						}

						ActiveData.ActiveView.IsolateElementsTemporary(listId);

						ActiveData.UIDoc.Selection.SetElementIds(listId);

						ActiveData.UIDoc.ShowElements(listId);

						ActiveData.Document.Regenerate();
						trans.Commit();
					}
				});
				ExternalEventHandler.Instance.SetAction(action);

				ExternalEventHandler.Instance.Run();
			}
			catch (Exception ex)
			{
			}
		}

		public void BtnIsolateAll()
		{
			try
			{
				List<ElementId> listId = new List<ElementId>();

				foreach (DetectOverlap DetectElement in DetectOverlaps)
				{
					listId.Add(DetectElement.Element1);
					try
					{
						if (DetectElement.Element2 == null || DetectElement.Element2.IntegerValue == -1)
						{ }
						else
						{
							listId.Add(DetectElement.Element2);
						}
					}
					catch { }
				}

				Action action = new Action(() =>
				{
					using (Transaction trans = new Transaction(ActiveData.Document))
					{
						TransactionStatus transactionStatus = trans.Start("Transaction Group");

						if (ActiveData.ActiveView.IsTemporaryHideIsolateActive())
						{
							TemporaryViewMode tempView = TemporaryViewMode.TemporaryHideIsolate;

							ActiveData.ActiveView.DisableTemporaryViewMode(tempView);
						}

						ActiveData.ActiveView.IsolateElementsTemporary(listId);

						ActiveData.UIDoc.Selection.SetElementIds(listId);

						ActiveData.UIDoc.ShowElements(listId);

						ActiveData.Document.Regenerate();
						trans.Commit();
					}
				});
				ExternalEventHandler.Instance.SetAction(action);

				ExternalEventHandler.Instance.Run();
			}
			catch (Exception ex)
			{
			}
		}

		public void BtnAddExclude()
		{
			ExcludeFilters.Add(new ExcludeFilters());
		}

		public object GetElementsFromProperty(ObservableCollection<CategoryOptions> categoriesRevitLinks)
		{
			List<Element> elementsActiveView = new List<Element>();
			foreach (CategoryOptions categoriesRevitLink in categoriesRevitLinks)
			{
				if (categoriesRevitLink.IsChecked == true)
				{
					BasicFilters[categoriesRevitLink.Name].ForEach(x => elementsActiveView.Add(x));
				}
			}
			return elementsActiveView;
		}

		public List<Category> CustomCategories(List<Element> elements)
		{
			List<Category> CustomCategories = new List<Category>();

			foreach (Element element in elements)
			{
				if (element.Category != null)
				{
					switch (element.Category.Id.IntegerValue)
					{
						case (int)BuiltInCategory.OST_StructuralFraming:
							if (element.get_Parameter(BuiltInParameter.HOST_VOLUME_COMPUTED).AsDouble() > GeometryLib.EPSILON_VOLUME)
							{
								Category a = CustomCategories.Where(x => x.Name == element.Category.Name).FirstOrDefault();
								if (a != null)
								{ }
								else
								{
									CustomCategories.Add(element.Category);
								}
							}
							break;
						case (int)BuiltInCategory.OST_StructuralFoundation:
							if (element.get_Parameter(BuiltInParameter.HOST_VOLUME_COMPUTED).AsDouble() > GeometryLib.EPSILON_VOLUME)
							{
								Category a = CustomCategories.Where(x => x.Name == element.Category.Name).FirstOrDefault();
								if (a != null)
								{ }
								else
								{
									CustomCategories.Add(element.Category);
								}
							}
							break;
						case (int)BuiltInCategory.OST_StructuralColumns:
							if (element.get_Parameter(BuiltInParameter.HOST_VOLUME_COMPUTED).AsDouble() > GeometryLib.EPSILON_VOLUME)
							{
								Category a = CustomCategories.Where(x => x.Name == element.Category.Name).FirstOrDefault();
								if (a != null)
								{ }
								else
								{
									CustomCategories.Add(element.Category);
								}
							}
							break;
						case (int)BuiltInCategory.OST_Walls:
							if (element.get_Parameter(BuiltInParameter.HOST_VOLUME_COMPUTED).AsDouble() > GeometryLib.EPSILON_VOLUME)
							{
								Category a = CustomCategories.Where(x => x.Name == element.Category.Name).FirstOrDefault();
								if (a != null)
								{ }
								else
								{
									CustomCategories.Add(element.Category);
								}
							}
							break;
						case (int)BuiltInCategory.OST_Cornices:
							if (element.get_Parameter(BuiltInParameter.HOST_VOLUME_COMPUTED).AsDouble() > GeometryLib.EPSILON_VOLUME)
							{
								Category a = CustomCategories.Where(x => x.Name == element.Category.Name).FirstOrDefault();
								if (a != null)
								{ }
								else
								{
									CustomCategories.Add(element.Category);
								}
							}
							break;
						case (int)BuiltInCategory.OST_Floors:
							if (element.get_Parameter(BuiltInParameter.HOST_VOLUME_COMPUTED).AsDouble() > GeometryLib.EPSILON_VOLUME)
							{
								Category a = CustomCategories.Where(x => x.Name == element.Category.Name).FirstOrDefault();
								if (a != null)
								{ }
								else
								{
									CustomCategories.Add(element.Category);
								}
							}
							break;
						case (int)BuiltInCategory.OST_EdgeSlab:
							if (element.get_Parameter(BuiltInParameter.HOST_VOLUME_COMPUTED).AsDouble() > GeometryLib.EPSILON_VOLUME)
							{
								Category a = CustomCategories.Where(x => x.Name == element.Category.Name).FirstOrDefault();
								if (a != null)
								{ }
								else
								{
									CustomCategories.Add(element.Category);
								}
							}
							break;
						case (int)BuiltInCategory.OST_Roofs:
							if (element.get_Parameter(BuiltInParameter.HOST_VOLUME_COMPUTED).AsDouble() > GeometryLib.EPSILON_VOLUME)
							{
								Category a = CustomCategories.Where(x => x.Name == element.Category.Name).FirstOrDefault();
								if (a != null)
								{ }
								else
								{
									CustomCategories.Add(element.Category);
								}
							}
							break;
						case (int)BuiltInCategory.OST_Columns:
							if (element.get_Parameter(BuiltInParameter.HOST_VOLUME_COMPUTED).AsDouble() > GeometryLib.EPSILON_VOLUME)
							{
								Category a = CustomCategories.Where(x => x.Name == element.Category.Name).FirstOrDefault();
								if (a != null)
								{ }
								else
								{
									CustomCategories.Add(element.Category);
								}
							}
							break;
						case (int)BuiltInCategory.OST_GenericModel:
							if (element is FamilyInstance && element.get_Parameter(BuiltInParameter.HOST_VOLUME_COMPUTED).AsDouble() > GeometryLib.EPSILON_VOLUME)
							{
								Category a = CustomCategories.Where(x => x.Name == element.Category.Name).FirstOrDefault();
								if (a != null)
								{ }
								else
								{
									CustomCategories.Add(element.Category);
								}
							}
							break;
						case (int)BuiltInCategory.OST_Stairs:
							Category b = CustomCategories.Where(x => x.Name == element.Category.Name).FirstOrDefault();
							if (b != null)
							{ }
							else
							{
								CustomCategories.Add(element.Category);
							}
							break;
						case (int)BuiltInCategory.OST_Ramps:
							Category d = CustomCategories.Where(x => x.Name == element.Category.Name).FirstOrDefault();
							if (d != null)
							{ }
							else
							{
								CustomCategories.Add(element.Category);
							}
							break;
						case (int)BuiltInCategory.OST_IOSModelGroups:
							List<Element> elementsGroup = (element as Autodesk.Revit.DB.Group).GetMemberIds().Select(x => ActiveData.Document.GetElement(x)).ToList();
							//CustomFillter(elementsGroup);
							break;
					}
					//ElementsAll.Add(element);
					//UserWorkSet.Add(element.ToUserWorkset());
				}
			}

			return CustomCategories;
		}

		public void DetectOverlap(List<Element> ListElement)
		{
			try
			{
				#region Tính toán khối union
				DetectOverlaps = new ObservableCollection<DetectOverlap>();

				Category category = Category.GetCategory(ActiveData.Document, BuiltInCategory.OST_GenericModel);
				//string fullPathFamily = CreateFamilyUtils.GetPathDefault();
				List<Solid> solidList = new List<Solid>();

				ProgressBarInstance progressBarInstance = new ProgressBarInstance("DetectOverlap.", ListElement.Count);
				for (int i = 0; i < ListElement.Count; i++)
				{
					progressBarInstance.Start();
					Element elementIntersect = ListElement[i]; //Element1

					try
					{
						List<Solid> solidIntersects = GeometryUtils.GetSolidsFromInstanceElement(elementIntersect, options, true).ToUnionSolids();

						List<Element> listElementIntersect = IntersectUtils.DoIntersect(elementIntersect, ListElement, options, true, 0);

						for (int j = 0; j < listElementIntersect.Count; j++)
						{
							Element element = listElementIntersect[j]; //Element2

							List<Solid> solidResults = GeometryUtils.GetSolidsFromInstanceElement(element, options, true).ToUnionSolids();

							foreach (Solid solidResult in solidResults)
							{
								List<Solid> ListSolid = new List<Solid>();
								foreach (Solid solidIntersect in solidIntersects)
								{
									if (IntersectUtils.DoesIntersect(solidIntersect, solidResult))
									{
										Solid solid = BooleanOperationsUtils.ExecuteBooleanOperation(solidResult, solidIntersect, BooleanOperationsType.Intersect);
										if (solid.Volume > GeometryLib.EPSILON_VOLUME)
										{
											ListSolid.Add(solid);
											solidList.Add(solid);
											// Check Exis
											ElementId e1 = elementIntersect.Id;
											ElementId e2 = element.Id;

											int count = DetectOverlaps.Where(x => (x.Element1 == e1 && x.Element2 == e2) ||
												(x.Element1 == e2 && x.Element2 == e1)).Count();

											if (count == 0)
												DetectOverlaps.Add(new DetectOverlap()
												{
													Element1 = elementIntersect.Id,
													Element1CateGory = $"{elementIntersect.Id.ToString()} <{elementIntersect.Category.Name}>",
													Element2 = element.Id,
													Element2CateGory = $"{element.Id.ToString()} <{element.Category.Name}>",
													OverlapVolume = Math.Round(solid.Volume * 0.3048 * 0.3048 * 0.3048, 3),
												});
										}
									}
								}
							}

						}
					}
					catch { }
					IsRunDetectOverLap = true;
				}

				//Check Model in Place
				progressBarInstance = new ProgressBarInstance("DetectOverlap Model In-Place.", ListElement.Count);
				for (int i = 0; i < ListElement.Count; i++)
				{
					progressBarInstance.Start();
					Element elementInPlace = ListElement[i];

					try
					{
						FamilyInstance familyInstance = elementInPlace as FamilyInstance;
						if (familyInstance.Symbol.Family.IsInPlace)
						{

							List<Solid> solidResults = GeometryUtils.GetSolidsFromInstanceElement(elementInPlace, options, true);

							double volumebySolid = solidResults.Sum(x => x.Volume);

							solidResults = solidResults.ToUnionSolids();

							double volumeInPlace = solidResults.Sum(x => x.Volume);

							if (Math.Round(volumebySolid, 2) != Math.Round(volumeInPlace, 2))
							{
								DetectOverlaps.Add(new DetectOverlap()
								{
									Element1 = elementInPlace.Id,
									Element1CateGory = $"{elementInPlace.Id.ToString()} <Model In-PLace>",
									OverlapVolume = Math.Round((volumebySolid - volumeInPlace) * 0.3048 * 0.3048 * 0.3048, 3),
								});
							}
						}
					}
					catch { }
					ElementOverlapCount = DetectOverlaps.Count();
					SumVolumneOverlap = DetectOverlaps.Sum(x => x.OverlapVolume);

					if (IsUnion)
					{
						solidList = solidList.ToUnionSolids();
						SumVolumneOverlap = solidList.Sum(x => x.Volume);

					}
				}
				#endregion

			}
			catch (Autodesk.Revit.Exceptions.OperationCanceledException)
			{
			}
			if (IsDetectOverlap)
			{
				if (SelectElementCount*1.0 / ElementCount >= 0.8)
				{
					PutResult2googleSheet();
				}
			}

			if (IsSwitchJoin == false)
			{
				Wmain.Show();
			}
		}

		public void SwitchJoin(List<Element> ListElement)
		{
			
			//List<Element> ListCheck = CheckVoid(ListElement);

			CheckSJ(true, ListElement);
		}

		private void CheckSJ(bool isAdvanceFilter, List<Element> ListElement)
		{
			Category c1 = Category.GetCategory(ActiveData.Document, BuiltInCategory.OST_StructuralColumns);
			Category c2 = Category.GetCategory(ActiveData.Document, BuiltInCategory.OST_Walls);
			Category c3 = Category.GetCategory(ActiveData.Document, BuiltInCategory.OST_Floors);
			Category c4 = Category.GetCategory(ActiveData.Document, BuiltInCategory.OST_StructuralFraming);

			List<ElementId> Listvoid = new List<ElementId>();

			try
			{
				
				//List<Element> ElementsColumn = ListElement.Where(x => x.Category.Name == c1.Name).ToList();
				//List<Element> ElementsWalls = ListElement.Where(x => x.Category.Name == c2.Name).ToList();
				//List<Element> ElementsFloors = ListElement.Where(x => x.Category.Name == c3.Name).ToList();

				//ProgressBarInstance progressBarInstance = new ProgressBarInstance("Check void.", (ElementsColumn.Count + ElementsWalls.Count + ElementsColumn.Count));

				Action action = new Action(() =>
				{
					using (TransactionGroup transactionGroup = new TransactionGroup(ActiveData.Document, "GroupSwitchJoins"))
					{
						//transactionGroup.Start();
						
						List<Element> ListCheck = CheckVoid(ListAfterFilter(ListElement));
						List<Element> Column_Wall = ListCheck.Where(x => x.Category.Name == c1.Name && QAQCUtils.IntersectWithCategory(x, ListElement, options, c2.Name) == true).ToList();
						List<Element> Column_Floors = ListCheck.Where(x => x.Category.Name == c1.Name && QAQCUtils.IntersectWithCategory(x, ListElement, options, c3.Name) == true).ToList();
						List<Element> Walls_Floors = ListCheck.Where(x => x.Category.Name == c2.Name && QAQCUtils.IntersectWithCategory(x, ListElement, options, c4.Name) == true).ToList();
						if (Column_Wall.Count + Column_Floors.Count + Walls_Floors.Count > 0)
						{
							ProgressBarInstance progressBarInstance = new ProgressBarInstance("Check Void.", Column_Wall.Count + Column_Floors.Count + Walls_Floors.Count);

							foreach (Element element in Column_Wall)
							{
								progressBarInstance.Start();
								if (Listvoid.Contains(element.Id)) continue;

								double v1 = 0;
								double v2 = 0;
								//Tính toán kl trước khi switch join
								List<Element> listElementIntersect = IntersectUtils.DoIntersect(element, ListElement, options, true, 0);
								listElementIntersect.Add(element);

								#region Tính toán theo union
								BasicFilters = new BasicFilter(listElementIntersect);

								List<Category> categories = CustomCategories(ListElement).Distinct().ToList();
								CategoryOptions = new ObservableCollection<CategoryOptions>();
								foreach (Category category in categories)
								{
									CategoryOptions.Add(new CategoryOptions() { Name = category.Name, IsChecked = true });
								}

								bool isCheck = false;
								CategoryOptions.ToList().ForEach(x => isCheck = isCheck || x.IsChecked);

								GenerateModel createGeneric = new GenerateModel()
								{
									ElementsOfRevit = ElementsOfRevit,
									CategorySetup = Category.GetCategory(ActiveData.Document, BuiltInCategory.OST_GenericModel),
									FullPath = FullPath,
									FamilyName = Guid.NewGuid().ToString(),
									CheckUnionAllElements = true,
								};
								try
								{
									v1 = Math.Round(createGeneric.GetVolumeGenericModel(), 3);
								}
								catch { }

								#endregion

								//Switch joint
								using (TransactionGroup transactionJoin = new TransactionGroup(ActiveData.Document, "Auto Join Geometry"))
								{
									transactionJoin.Start();

									using (Transaction transaction = new Transaction(ActiveData.Document, "Auto"))
									{
										WarningUtils ignoreWarning = new WarningUtils();
										FailureHandlingOptions optionsignoreWarning = ignoreWarning.GetFailureHandling(transaction);
										transaction.Start();

										if (element.LookupParameter("Volume").AsDouble() > GeometryLib.EPSILON_VOLUME)
										{
											foreach (Element e in listElementIntersect)
											{
												if (e.Category.Name == c2.Name)
												{
													try
													{
														Autodesk.Revit.DB.JoinGeometryUtils.SwitchJoinOrder(ActiveData.Document, element, e);
													}
													catch { }
												}
											}

										}
										transaction.Commit(optionsignoreWarning);

										if (ignoreWarning.WarningDiscard.Warnings.Count > 0
										|| ignoreWarning.WarningDiscard.Errors.Count > 0)
										{
											continue;
										}
									}

									try
									{
										v2 = Math.Round(createGeneric.GetVolumeGenericModel(), 3);
									}
									catch { }

									//So sánh V1 vs V2
									if (v2 > v1)
									{
										DetectOverlaps.Add(new DetectOverlap()
										{
											Element1 = element.Id,
											Element1CateGory = $"{element.Id.ToString()} <{element.Category.Name}>",
											OverlapVolume = Math.Round((v1 - v2), 3),
										});

										SumLackofConcrete = SumLackofConcrete + Math.Round((v1 - v2), 3);
										ElementOverlapCount = DetectOverlaps.Count();
										Listvoid.Add(element.Id);
									}
									transactionJoin.Dispose();
								}
							}

							foreach (Element element in Column_Floors)
							{
								progressBarInstance.Start();
								if (Listvoid.Contains(element.Id)) continue;
								double v1 = 0;
								double v2 = 0;
								//Tính toán kl trước khi switch join
								List<Element> listElementIntersect = IntersectUtils.DoIntersect(element, ListElement, options, true, 0);
								listElementIntersect.Add(element);

								#region Tính toán theo union
								BasicFilters = new BasicFilter(listElementIntersect);

								List<Category> categories = CustomCategories(ListElement).Distinct().ToList();
								CategoryOptions = new ObservableCollection<CategoryOptions>();
								foreach (Category category in categories)
								{
									CategoryOptions.Add(new CategoryOptions() { Name = category.Name, IsChecked = true });
								}

								bool isCheck = false;
								CategoryOptions.ToList().ForEach(x => isCheck = isCheck || x.IsChecked);

								GenerateModel createGeneric = new GenerateModel()
								{
									ElementsOfRevit = ElementsOfRevit,
									CategorySetup = Category.GetCategory(ActiveData.Document, BuiltInCategory.OST_GenericModel),
									FullPath = FullPath,
									FamilyName = Guid.NewGuid().ToString(),
									CheckUnionAllElements = true,
								};
								try
								{
									v1 = Math.Round(createGeneric.GetVolumeGenericModel(), 3);
								}
								catch { }

								#endregion

								//Switch joint
								using (TransactionGroup transactionJoin = new TransactionGroup(ActiveData.Document, "Auto Join Geometry"))
								{
									transactionJoin.Start();

									using (Transaction transaction = new Transaction(ActiveData.Document, "Auto"))
									{
										WarningUtils ignoreWarning = new WarningUtils();
										FailureHandlingOptions optionsignoreWarning = ignoreWarning.GetFailureHandling(transaction);
										transaction.Start();

										if (element.LookupParameter("Volume").AsDouble() > GeometryLib.EPSILON_VOLUME)
										{
											foreach (Element e in listElementIntersect)
											{
												if (e.Category.Name == c3.Name)
												{
													try
													{
														Autodesk.Revit.DB.JoinGeometryUtils.SwitchJoinOrder(ActiveData.Document, element, e);
													}
													catch { }
												}
											}

										}
										transaction.Commit(optionsignoreWarning);

										if (ignoreWarning.WarningDiscard.Warnings.Count > 0
										|| ignoreWarning.WarningDiscard.Errors.Count > 0)
										{
											continue;
										}
									}

									try
									{
										v2 = Math.Round(createGeneric.GetVolumeGenericModel(), 3);
									}
									catch { }

									//So sánh V1 vs V2
									if (v2 > v1)
									{
										DetectOverlaps.Add(new DetectOverlap()
										{
											Element1 = element.Id,
											Element1CateGory = $"{element.Id.ToString()} <{element.Category.Name}>",
											OverlapVolume = Math.Round((v1 - v2), 3),
										});

										SumLackofConcrete = SumLackofConcrete + Math.Round((v1 - v2), 3);
										ElementOverlapCount = DetectOverlaps.Count();
										Listvoid.Add(element.Id);
									}
									transactionJoin.Dispose();
								}
							}

							foreach (Element element in Walls_Floors)
							{
								progressBarInstance.Start();
								if (Listvoid.Contains(element.Id)) continue;
								double v1 = 0;
								double v2 = 0;
								//Tính toán kl trước khi switch join
								List<Element> listElementIntersect = IntersectUtils.DoIntersect(element, ListElement, options, true, 0);
								listElementIntersect.Add(element);

								foreach (Element e in QAQCUtils.ListFloorInTopLevel(ListElement, element))
								{
									listElementIntersect.Add(e);
								}
								listElementIntersect.Distinct();

								#region Tính toán theo union
								BasicFilters = new BasicFilter(listElementIntersect);

								List<Category> categories = CustomCategories(ListElement).Distinct().ToList();
								CategoryOptions = new ObservableCollection<CategoryOptions>();
								foreach (Category category in categories)
								{
									CategoryOptions.Add(new CategoryOptions() { Name = category.Name, IsChecked = true });
								}

								bool isCheck = false;
								CategoryOptions.ToList().ForEach(x => isCheck = isCheck || x.IsChecked);

								GenerateModel createGeneric = new GenerateModel()
								{
									ElementsOfRevit = ElementsOfRevit,
									CategorySetup = Category.GetCategory(ActiveData.Document, BuiltInCategory.OST_GenericModel),
									FullPath = FullPath,
									FamilyName = Guid.NewGuid().ToString(),
									CheckUnionAllElements = true,
								};
								try
								{
									v1 = Math.Round(createGeneric.GetVolumeGenericModel(), 3);
								}
								catch { }

								#endregion

								//Switch joint
								using (TransactionGroup transactionJoin = new TransactionGroup(ActiveData.Document, "Auto Join Geometry"))
								{
									transactionJoin.Start();

									using (Transaction transaction = new Transaction(ActiveData.Document, "Auto"))
									{
										WarningUtils ignoreWarning = new WarningUtils();
										FailureHandlingOptions optionsignoreWarning = ignoreWarning.GetFailureHandling(transaction);
										transaction.Start();

										if (element.LookupParameter("Volume").AsDouble() > GeometryLib.EPSILON_VOLUME)
										{
											foreach (Element e in listElementIntersect)
											{
												if (e.Category.Name == c3.Name)
												{
													try
													{
														Autodesk.Revit.DB.JoinGeometryUtils.SwitchJoinOrder(ActiveData.Document, element, e);
													}
													catch { }
												}
											}

										}
										transaction.Commit(optionsignoreWarning);

										if (ignoreWarning.WarningDiscard.Warnings.Count > 0
										|| ignoreWarning.WarningDiscard.Errors.Count > 0)
										{
											continue;
										}
									}

									try
									{
										v2 = Math.Round(createGeneric.GetVolumeGenericModel(), 3);
									}
									catch { }

									//So sánh V1 vs V2
									if (v2 > v1)
									{
										DetectOverlaps.Add(new DetectOverlap()
										{
											Element1 = element.Id,
											Element1CateGory = $"{element.Id.ToString()} <{element.Category.Name}>",
											OverlapVolume = Math.Round((v1 - v2), 3),
										});

										SumLackofConcrete = SumLackofConcrete + Math.Round((v1 - v2), 3);
										ElementOverlapCount = DetectOverlaps.Count();
										Listvoid.Add(element.Id);
									}

									transactionJoin.Dispose();
								}
							}
						}
						transactionGroup.Dispose();
					}
					ElementOverlapCount = DetectOverlaps.Count();
					DateTime t2 = DateTime.Now;
					TimeSpan = (t1 - t2).ToString(@"hh\:mm\:ss");

					if (IsSwitchJoin == true)
					{
						if (SelectElementCount*1.0 / ElementCount >= 0.8)
						{
							PutResult2googleSheet();
						}
					}

					IsRunCheckVoid = true;
					Wmain.Show();
				});
				ExternalEventHandler.Instance.SetAction(action);

				ExternalEventHandler.Instance.Run();
			}
			catch { }


		}

		public List<Element> CheckVoid(List<Element> ListElement)
		{
			List<ElementId> ListElementHaveVoid = new List<ElementId>();
			List<ElementId> ListElementIdHaveVoid = new List<ElementId>();
			double increment = 400 / 304.8;
			ProgressBarViewQAQC prog;

			#region Walls
			List<Element> ListWalls = ListElement.Where(x => x.Category.Name == "Walls").ToList();

			if (ListWalls.Count > 0)
			{
				prog = new ProgressBarViewQAQC();
				prog.Show();

				foreach (Element Element in ListWalls)
				{
					prog.Create(ListWalls.Count, 1, "Check Walls", $"ElementId: <{Element.Id}>");

					try
					{
						if (ListElementHaveVoid.Contains(Element.Id))
						{
							prog.Create1();
							continue;
						}

						#region Create Solid Element intersect with Wall
						List<Element> listElementIntersect = IntersectUtils.DoIntersect(Element, ListElement, options, true, 0);
						if (!QAQCUtils.CheckInterSecElement(listElementIntersect, Element.Category, Element))
						{
							prog.Create1();
							continue;
						}
						//Add floor cùng level đỉnh
						foreach (Element e in QAQCUtils.ListFloorInTopLevel(ListElement, Element))
						{
							listElementIntersect.Add(e);
						}
						listElementIntersect.Add(Element);

						listElementIntersect.Distinct();

						BasicFilters = new BasicFilter(listElementIntersect);

						List<Category> categories = CustomCategories(ListElement).Distinct().ToList();
						CategoryOptions = new ObservableCollection<CategoryOptions>();
						foreach (Category category in categories)
						{
							CategoryOptions.Add(new CategoryOptions() { Name = category.Name, IsChecked = true });
						}

						bool isCheck = false;
						CategoryOptions.ToList().ForEach(x => isCheck = isCheck || x.IsChecked);

						GenerateModel createGeneric = new GenerateModel()
						{
							ElementsOfRevit = ElementsOfRevit,
							CategorySetup = Category.GetCategory(ActiveData.Document, BuiltInCategory.OST_GenericModel),
							FullPath = FullPath,
							FamilyName = Guid.NewGuid().ToString(),
							CheckUnionAllElements = true,
						};
						List<Solid> SolidGeneric = createGeneric.GetSolidGenericModel().ToUnionSolids();

						#endregion

						double Height = Element.GetProximityHeight();
						double Width = QAQCUtils.GetProximityWidthWall(Element) / 2;

						List<Solid> solids = GeometryUtils.GetSolidsFromInstanceElement(Element, options, true).ToUnionSolids();

						XYZ Zvt = new XYZ(0, 0, 1);
						XYZ Yvt = new XYZ();
						XYZ Yvt2 = new XYZ();

						bool IsFail = false;
						foreach (Solid solid in solids)
						{
							List<PlanarFace> p = GeometryLib.GetPlanarFaceHaveNormalVectorParallelVector(solid, new XYZ(0, 0, 1));
							PlanarFace face = p[0];

							Yvt = face.YVector;
							Yvt2 = -1 * Yvt;
							LocationCurve locationCurve = Element.Location as LocationCurve;
							if (locationCurve.Curve is Line locationLine)
							{
								XYZ lineDirection = locationLine.Direction;
								double lenght = locationLine.Length;

								int maxstepHor = (int)(lenght / increment);
								int maxstepVert = (int)((500 / 304.8) / increment);

								int maxstepHorY = 1;
								if (Width >= (250 / 304.8))
								{
									maxstepHorY = (int)(Width / (increment * 2.5));
								}
								else
								{
									maxstepHorY = 1;
								}


								XYZ checkPoint = QAQCUtils.LocationStartPointWall(Element, Height, Width);
								//Check trung tâm
								XYZ checkp = checkPoint + (lenght/2)* lineDirection - (80/304.8)* Zvt;
								Solid solidfromcheckp = GeometryUtils.CreateCubeSolid(checkp, 5 / 304.8);
								foreach (Solid solidintersec in SolidGeneric)
								{
									IsFail = false;
									//Không giao thì check tiếp
									if (QAQCUtils.DoesIntersect(solidfromcheckp, solidintersec) == false)
									{
										continue;
									}
									//Có giao
									else
									{
										IsFail = true;
										break;
									}
								}

								if (IsFail == false)
								{
									ListElementHaveVoid.Add(Element.Id);
									ListElementIdHaveVoid.Add(Element.Id);

									List<Element> elements = IntersectUtils.DoIntersect(Element, ListElement, options, true, 0);
									foreach (Element e in elements)
									{
										ListElementHaveVoid.Add(e.Id);
									}
									break;
								}

								for (int i = 1; i < maxstepHor - 1; i++)
								{
									XYZ newXYZ = checkPoint + i * increment * lineDirection - (checkPoint.Z - QAQCUtils.MinElevationZ(listElementIntersect, Element)) * Zvt / 2;
									for (int k = 0; k < maxstepHorY; k++)
									{
										XYZ newXYZHor = newXYZ + k * increment * Yvt;
										Solid solidfromPoint = GeometryUtils.CreateCubeSolid(newXYZHor, 5 / 304.8);

										foreach (Solid solidintersec in SolidGeneric)
										{
											IsFail = false;
											//Không giao thì check tiếp
											if (QAQCUtils.DoesIntersect(solidfromPoint, solidintersec) == false)
											{
												continue;
											}
											//Có giao
											else
											{
												IsFail = true;
												break;
											}
										}

										if (IsFail)
										{
											continue;
										}
										else break;
									}
									if (IsFail) continue;
									else break;
								}

								if (IsFail == false)
								{
									ListElementHaveVoid.Add(Element.Id);
									ListElementIdHaveVoid.Add(Element.Id);

									List<Element> elements = IntersectUtils.DoIntersect(Element, ListElement, options, true, 0);
									foreach (Element e in elements)
									{
										ListElementHaveVoid.Add(e.Id);
									}
									break;
								}
							}
						}
					}
					catch { }

					prog.Create1();
				}
				prog.Close();
			}
			#endregion

			#region Columns
			List<Element> ListColumn = ListElement.Where(x => x.Category.Name == "Structural Columns").ToList();

			if (ListColumn.Count > 0)
			{
				prog = new ProgressBarViewQAQC();
				prog.Show();

				foreach (Element Element in ListColumn)
				{
					prog.Create(ListColumn.Count, 1, "Check Columns", $"ElementId: <{Element.Id}>");

					try
					{
						if (ListElementHaveVoid.Contains(Element.Id))
						{
							prog.Create1();
							continue;
						}

						#region Create Solid Element intersect with Wall
						List<Element> listElementIntersect = IntersectUtils.DoIntersect(Element, ListElement, options, true, 0);
						if (!QAQCUtils.CheckInterSecElement(listElementIntersect, Element.Category, Element))
						{
							prog.Create1();
							continue;
						}

						listElementIntersect.Add(Element);

						BasicFilters = new BasicFilter(listElementIntersect);

						List<Category> categories = CustomCategories(ListElement).Distinct().ToList();
						CategoryOptions = new ObservableCollection<CategoryOptions>();
						foreach (Category category in categories)
						{
							CategoryOptions.Add(new CategoryOptions() { Name = category.Name, IsChecked = true });
						}

						bool isCheck = false;
						CategoryOptions.ToList().ForEach(x => isCheck = isCheck || x.IsChecked);

						GenerateModel createGeneric = new GenerateModel()
						{
							ElementsOfRevit = ElementsOfRevit,
							CategorySetup = Category.GetCategory(ActiveData.Document, BuiltInCategory.OST_GenericModel),
							FullPath = FullPath,
							FamilyName = Guid.NewGuid().ToString(),
							CheckUnionAllElements = true,
						};
						List<Solid> SolidGeneric = createGeneric.GetSolidGenericModel().ToUnionSolids();

						#endregion

						List<Solid> solids = GeometryUtils.GetSolidsFromInstanceElement(Element, options, true).ToUnionSolids();
						List<PlanarFace> planarFaces = new List<PlanarFace>();
						XYZ Zvt = new XYZ(0, 0, 1);

						bool IsFail = false;
						XYZ XYZFail = new XYZ();
						foreach (Solid solid in solids)
						{
							List<PlanarFace> p = GeometryLib.GetPlanarFaceHaveNormalVectorParallelVector(solid, Zvt);
							PlanarFace face = p[0];

							List<XYZ> CheckPoint = QAQCUtils.GetCheckPointOfColumn(Element,options);
							foreach (XYZ point in CheckPoint)
							{
								XYZ newXYZ = point - (point.Z - QAQCUtils.MinElevationZ(listElementIntersect, Element)) * Zvt / 2;
								Solid solidfromPoint = GeometryUtils.CreateCubeSolid(newXYZ, 5 / 304.8);

								foreach (Solid solidintersec in SolidGeneric)
								{
									IsFail = false;
									//Không giao thì check tiếp
									if (IntersectUtils.DoesIntersect(solidfromPoint, solidintersec) == false)
									{
										continue;
									}
									//Có giao
									else
									{
										IsFail = true;
										break;
									}
								}

								if (IsFail)
								{
									continue;
								}
								else break;
							}

							if (IsFail == false)
							{
								ListElementHaveVoid.Add(Element.Id);
								ListElementIdHaveVoid.Add(Element.Id);

								List<Element> elements = IntersectUtils.DoIntersect(Element, ListElement, options, true, 0);
								foreach (Element e in elements)
								{
									ListElementHaveVoid.Add(e.Id);
								}
								break;
							}
						}
					}
					catch { }
					prog.Create1();
				}
				prog.Close();
			}
			#endregion

			ListElementIdHaveVoid = ListElementIdHaveVoid.Distinct().ToList();

			List<Element> ListElementCheck = new List<Element>();
			foreach (ElementId eid in ListElementIdHaveVoid)
			{
				Element e = ActiveData.Document.GetElement(eid);
				ListElementCheck.Add(e);
			}

			return ListElementCheck;
		}

		public void BeginSwitchJoinOrder(Category categoryFirst, Category categorySecond, bool isAdvanceFilter, List<Element> ListElement)
		{
			bool IsTrue = false;
			try
			{
				List<Element> ElementsOne = ListElement.Where(x => x.Category.Name == categoryFirst.Name).ToList();
				List<Element> ElementsTwo = ListElement.Where(x => x.Category.Name == categorySecond.Name).ToList();

				if (ElementsOne.Count != 0 && ElementsTwo.Count != 0)
				{
					Action action = new Action(() =>
					{
						using (TransactionGroup transactionGroup = new TransactionGroup(ActiveData.Document, "GroupSwitchJoins"))
						{
							transactionGroup.Start();
							//Check 3 đối tượng để xem độ ưu tiên category trước:

							List<Element> Column_Wall = ListElement.Where(x => x.Category.Name == categoryFirst.Name && QAQCUtils.IntersectWithCategory(x, ListElement, options, categorySecond.Name) == true).ToList();

							#region Check 3 Element
							List<Element> ColumnCheck = new List<Element>();
							if (Column_Wall.Count > 3)
							{
								do
								{
									Random TenBienRanDom = new Random();
									int i = TenBienRanDom.Next(0, Column_Wall.Count - 1);

									ColumnCheck.Add(Column_Wall[i]);
								}
								while (ColumnCheck.Count < 3);
							}
							else
							{
								ColumnCheck = Column_Wall;
							}

							List<bool> bools = new List<bool>();
							foreach (Element element in ColumnCheck)
							{
								double Volumepre = element.LookupParameter("Volume").AsDouble();
								double Volume2 = 0;
								List<Element> listElementIntersect = IntersectUtils.DoIntersect(element, ListElement, options, true, 0);
								foreach (Element e in listElementIntersect)
								{
									using (Transaction transactionUnJoin = new Transaction(ActiveData.Document, "Switch Join Order"))
									{
										transactionUnJoin.Start();
										try
										{
											Autodesk.Revit.DB.JoinGeometryUtils.SwitchJoinOrder(ActiveData.Document, element, e);
											if (Volumepre > Volume2) bools.Add(true);
											else bools.Add(false);
											Autodesk.Revit.DB.JoinGeometryUtils.SwitchJoinOrder(ActiveData.Document, element, e);
										}
										catch { }
										transactionUnJoin.RollBack();
									}
									break;
								}
							}

							if ((bools.Where(x => x == true).Count() / bools.Count) < 0.5)
							{
								IsTrue = false;
							}
							#endregion

							#region Tính toán theo union
							BasicFilters = new BasicFilter(ListElement);

							List<Category> categories = CustomCategories(ListElement).Distinct().ToList();

							foreach (Category category in categories)
							{
								CategoryOptions.Add(new CategoryOptions() { Name = category.Name, IsChecked = true });
							}

							bool isCheck = false;
							CategoryOptions.ToList().ForEach(x => isCheck = isCheck || x.IsChecked);
							GenerateModel createGeneric = new GenerateModel()
							{
								ElementsOfRevit = ElementsOfRevit,
								CategorySetup = Category.GetCategory(ActiveData.Document, BuiltInCategory.OST_GenericModel),
								FullPath = FullPath,
								FamilyName = Guid.NewGuid().ToString(),
								CheckUnionAllElements = true,
							};

							double VolumePre = createGeneric.GetVolumeGenericModel();
							#endregion

							#region SwitchJoinOrder
							foreach (Element element in Column_Wall)
							{
								using (Transaction transactionJoin = new Transaction(ActiveData.Document, "Auto Join Geometry"))
								{
									WarningUtils ignoreWarning = new WarningUtils();
									FailureHandlingOptions options = ignoreWarning.GetFailureHandling(transactionJoin);
									transactionJoin.Start();
									if (element.LookupParameter("Volume").AsDouble() > GeometryLib.EPSILON_VOLUME)
									{
										List<Element> elementsTwo = ElementsTwo.Select(x => x).ToList();
										elementsTwo.Remove(element);
										if (elementsTwo.Count > 0)
										{
											InteractionUtils interactionElements = new InteractionUtils()
											{
												ElementsStock = new ObservableCollection<Element>(elementsTwo),
												ElementIntersect = element
											};
											interactionElements.SingleSwitchJoinOrder(isAdvanceFilter);
										}
									}
									transactionJoin.Commit(options);
								}
							}
							#endregion

							#region Tính toán theo union2
							BasicFilters = new BasicFilter(ListElement);

							categories = CustomCategories(ListElement).Distinct().ToList();

							CategoryOptions = new ObservableCollection<CategoryOptions>();
							foreach (Category category in categories)
							{
								CategoryOptions.Add(new CategoryOptions() { Name = category.Name, IsChecked = true });
							}

							isCheck = false;
							CategoryOptions.ToList().ForEach(x => isCheck = isCheck || x.IsChecked);

							createGeneric = new GenerateModel()
							{
								ElementsOfRevit = ElementsOfRevit,
								CategorySetup = Category.GetCategory(ActiveData.Document, BuiltInCategory.OST_GenericModel),
								FullPath = FullPath,
								FamilyName = Guid.NewGuid().ToString(),
								CheckUnionAllElements = true,
							};

							double VolumeSwitch = createGeneric.GetVolumeGenericModel();
							#endregion

							System.Windows.MessageBox.Show($"{VolumePre}");
							System.Windows.MessageBox.Show($"{VolumeSwitch}");

							#region So sánh tìm ra void
							if (Math.Round(VolumeSwitch, 3) - Math.Round(VolumePre, 3) > 0)
							{
								foreach (Element e in Column_Wall)
								{

								}
							}
							#endregion
							transactionGroup.Assimilate();
						}
					});
					ExternalEventHandler.Instance.SetAction(action);

					ExternalEventHandler.Instance.Run();
				}
			}
			catch { }

			Wmain.Show();
		}

		public void PutResult2googleSheet()
		{
			try
			{
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

				var s = FilePath.Split('\\');

				string FileName = s[s.Length - 1];

				string User = System.Windows.Forms.SystemInformation.ComputerName;
				double geometryVolume1 = GeometryVolume;
				string RealisticConcrete = geometryVolume1.ToString();
				string ConcreteDeviation = "";
				string PConcreteDeviation = "";
				if (IsDetectOverlap)
				{
					ConcreteDeviation = SumVolumneOverlap.ToString();
					PConcreteDeviation = Math.Round((SumVolumneOverlap * 100 / GeometryVolume), 2).ToString();
				}
				string LackOfConcrete = "";
				string PLackOfConcrete = "";
				if (IsSwitchJoin)
				{
					LackOfConcrete = SumLackofConcrete.ToString();
					PLackOfConcrete = Math.Round((SumLackofConcrete * 100 / GeometryVolume), 2).ToString();
				}
				string url = $@"https://script.google.com/macros/s/AKfycbyNlKmonPr-FINdeXizIt5I3l7gDhg0ZjabtJGd9xaeasDQxCQ/exec?FilePath={FilePath}&FileName={FileName}&User={User}&RealisticConcrete={RealisticConcrete}&ConcreteDeviation={ConcreteDeviation}&PConcreteDeviation={PConcreteDeviation}&LackOfConcrete={LackOfConcrete}&PLackOfConcrete={PLackOfConcrete}";
				HttpWebRequest req;
				HttpWebResponse res = null;
				req = (HttpWebRequest)WebRequest.Create(url);
				res = (HttpWebResponse)req.GetResponse();
				res.Close();
			}
			catch { }
			Wmain.Show();
		}

		public void BtnChangeCriteriaFilter()
		{
			try
			{
				ExcludeFilter = (ExcludeFilters)Wmain.dgFilter.SelectedItem;

				if (ExcludeFilter.Criteria == "Type Name")
				{
					ExcludeFilter.IsTypeName = false;
					ExcludeFilter.Property = "";
				}
				else
				{
					ExcludeFilter.IsTypeName = true;
				}
			}
			catch
			{ }
		}

		public void BtndgfilterDelete()
		{
			try
			{
				ExcludeFilter = (ExcludeFilters)Wmain.dgFilter.SelectedItem;
				ExcludeFilters.Remove(ExcludeFilter);
			}
			catch
			{ }
		}
		public void BtnSaveSetting()
		{
			try
			{
				List<string> Q_category = new List<string>();
				List<string> Q_Criteria = new List<string>();
				List<string> Q_Property = new List<string>();
				List<string> Q_Condition = new List<string>();
				List<string> Q_Value = new List<string>();

				foreach (ExcludeFilters ex in ExcludeFilters)
				{
					Q_category.Add(ex.CategorySelect);
					Q_Criteria.Add(ex.Criteria);
					Q_Property.Add(ex.Property);
					Q_Condition.Add(ex.Condition);
					Q_Value.Add(ex.Value);
				}

				QAQCSetting.Default.Q_Category = Q_category;
				QAQCSetting.Default.Q_Criteria = Q_Criteria;
				QAQCSetting.Default.Q_Property = Q_Property;
				QAQCSetting.Default.Q_Condition = Q_Condition;
				QAQCSetting.Default.Q_Value = Q_Value;

				QAQCSetting.Default.Save();
			}
			catch
			{ }
			(new System.Threading.Thread(CloseIt)).Start();
			System.Windows.Forms.MessageBox.Show("Setting was save");
		}

		public void GetSetting()
		{
			try
			{
				List<string> Q_category = new List<string>();
				List<string> Q_Criteria = new List<string>();
				List<string> Q_Property = new List<string>();
				List<string> Q_Condition = new List<string>();
				List<string> Q_Value = new List<string>();

				Q_category=QAQCSetting.Default.Q_Category;
				Q_Criteria=QAQCSetting.Default.Q_Criteria;
				Q_Property=QAQCSetting.Default.Q_Property;
				Q_Condition=QAQCSetting.Default.Q_Condition;
				Q_Value=QAQCSetting.Default.Q_Value;

				for (int i = 0; i < Q_category.Count; i++)
				{
					bool isTypeName = false;
					if (Q_Criteria[i] != "Type Name")
					{
						isTypeName = true;
					}

					ExcludeFilters.Add(new ExcludeFilters()
					{
						CategorySelect = Q_category[i],
						IsTypeName = isTypeName,
						Criteria = Q_Criteria[i],
						Property = Q_Property[i],
						Condition = Q_Condition[i],
						Value = Q_Value[i],
					});
				}
			}
			catch
			{ }
		}

		#region Filter
		public List<Element> ListAfterFilter(List<Element> ListElement)
		{
			try
			{
				List<Element> elements = new List<Element>();
				elements = ListElement;

				foreach (ExcludeFilters ex in ExcludeFilters)
				{
					if (ex.Criteria == "Type Name")
					{
						elements = elements.Except(TypeNameFilter(elements, ex)).ToList();
					}
					else
					{
						elements = elements.Except(ParaFilter(elements, ex)).ToList();
					}

				}

				return elements;
			}
			catch
			{
				return ListElement;
			}
		}

		private List<Element> TypeNameFilter(List<Element> ListElement, ExcludeFilters ex)
		{

			UtilsFilter utilsFilter = new UtilsFilter();
			List<Element> elements = new List<Element>();
			Regex rg;
			try
			{
				elements = ListElement.Where(x => x.Category.Name == ex.CategorySelect).ToList();

				switch (ex.Condition)
				{
					case "=":
						elements = elements.Where(x => utilsFilter.TypeName(ActiveData.Document, x) == ex.Value).ToList();
						break;
					case "!=":
						elements = elements.Where(x => utilsFilter.TypeName(ActiveData.Document, x) != ex.Value).ToList();
						break;
					case "Contains":
						elements = elements.Where(x => utilsFilter.TypeName(ActiveData.Document, x).Contains(ex.Value)).ToList();
						break;
					case "Does not contains":
						elements = elements.Where(x => utilsFilter.TypeName(ActiveData.Document, x).Contains(ex.Value) == false).ToList();
						break;
					case "Match Regex":
						rg = new Regex(ex.Value);
						elements = elements.Where(x => rg.Match(utilsFilter.TypeName(ActiveData.Document, x)).Success == true).ToList();
						break;
					default:
						break;
				}
			}
			catch { }
			return elements;

		}

		private List<Element> ParaFilter(List<Element> ListElement, ExcludeFilters ex)
		{
			UtilsFilter utilsFilter = new UtilsFilter();
			List<Element> elements = new List<Element>();
			Regex rg;

			try
			{
				elements = ListElement.Where(x => x.Category.Name == ex.CategorySelect).ToList();

				switch (ex.Condition)
				{
					case "=":
						elements = elements.Where(x => utilsFilter.LookupParameterValue(x, ex.Property, "String") == ex.Value).ToList();
						break;
					case "!=":
						elements = elements.Where(x => utilsFilter.LookupParameterValue(x, ex.Property, "String") != ex.Value).ToList();
						break;
					case "Contains":
						elements = elements.Where(x => utilsFilter.LookupParameterValue(x, ex.Property, "String").Contains(ex.Value)).ToList();
						break;
					case "Does not contains":
						elements = elements.Where(x => utilsFilter.LookupParameterValue(x, ex.Property, "String").Contains(ex.Value) == false).ToList();
						break;
					case "Match Regex":
						rg = new Regex(ex.Value);
						elements = elements.Where(x => rg.Match(utilsFilter.LookupParameterValue(x, ex.Property, "String")).Success == true).ToList();
						break;
					default:
						break;
				}
			}
			catch { }
			return elements;
		}

		public void CloseIt()
		{
			System.Threading.Thread.Sleep(1000);
			Microsoft.VisualBasic.Interaction.AppActivate(System.Diagnostics.Process.GetCurrentProcess().Id);
			System.Windows.Forms.SendKeys.SendWait(" ");
		}
		#endregion
	}
}


