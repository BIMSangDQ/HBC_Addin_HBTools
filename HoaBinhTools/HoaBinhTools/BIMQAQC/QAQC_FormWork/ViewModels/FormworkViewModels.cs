using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Utils;
using HoaBinhTools.BIMQAQC.QAQC_FormWork.Views;
using Autodesk.Revit.DB;
using System.Windows;
using Autodesk.Revit.UI.Selection;
using BeyCons.Core.FormUtils.ControlViews;
using System.Collections.ObjectModel;
using RelayCommand = Utils.RelayCommand;
using HoaBinhTools.BIMQAQC.QAQC_FormWork.Models;
using Autodesk.Revit.DB.Architecture;
using BeyCons.Core.Libraries.Geometries;
using HoaBinhTools.BIMQAQC.QAQC_FormWork.Command.Excutions;
using IntersectElement = HoaBinhTools.BIMQAQC.QAQC_FormWork.Command.Excutions.IntersectElement;
using BeyCons.Core.Libraries.Units;

namespace HoaBinhTools.BIMQAQC.QAQC_FormWork.ViewModels
{
	public class FormworkViewModels : ViewModelBase
	{
		public QAQC_Formwork Wmain { get; set; }

		#region Setting
		private double a1;
		public double A1
		{
			get
			{
				return a1;
			}
			set
			{
				a1 = value;
				OnPropertyChanged("A1");
			}
		}

		private double a2;
		public double A2
		{
			get
			{
				return a2;
			}
			set
			{
				a2 = value;
				OnPropertyChanged("A2");
			}
		}

		private List<string> a3s = new List<string>()
		{
			"Trừ phần dầm giao",
			"Không trừ dầm giao, cốp pha dầm nếu có giao sàn sẽ không tính đáy"
		};
		public List<string> A3s
		{
			get
			{
				return a3s;
			}
			set
			{
				a3s = value;
				OnPropertyChanged("A3s");
			}
		}

		private string a3;
		public string A3
		{
			get
			{
				return a3;
			}
			set
			{
				a3 = value;
				OnPropertyChanged("A3");
			}
		}

		private double a4;
		public double A4
		{
			get
			{
				return a4;
			}
			set
			{
				a4 = value;
				OnPropertyChanged("A4");
			}
		}

		private double a5;
		public double A5
		{
			get
			{
				return a5;
			}
			set
			{
				a5 = value;
				OnPropertyChanged("A5");
			}
		}

		private double a6;
		public double A6
		{
			get
			{
				return a6;
			}
			set
			{
				a6 = value;
				OnPropertyChanged("A6");
			}
		}

		private double b1;
		public double B1
		{
			get
			{
				return b1;
			}
			set
			{
				b1 = value;
				OnPropertyChanged("B1");
			}
		}

		private double b2;
		public double B2
		{
			get
			{
				return b2;
			}
			set
			{
				b2 = value;
				OnPropertyChanged("B2");
			}
		}

		private double b3;
		public double B3
		{
			get
			{
				return b3;
			}
			set
			{
				b3 = value;
				OnPropertyChanged("B3");
			}
		}

		private double b4;
		public double B4
		{
			get
			{
				return b4;
			}
			set
			{
				b4 = value;
				OnPropertyChanged("B4");
			}
		}

		private List<string> includes = new List<string>()
		{
			"Project",
			"Selection"
		};
		public List<string> Includes
		{
			get
			{
				return includes;
			}
			set
			{
				includes = value;
				OnPropertyChanged("Includes");
			}
		}

		private string include;
		public string Include
		{
			get
			{
				return include;
			}
			set
			{
				include = value;
				OnPropertyChanged("Include");
			}
		}

		private bool isActiveView;
		public bool IsActiveView
		{
			get
			{
				return isActiveView;
			}
			set
			{
				isActiveView = value;
				OnPropertyChanged("IsActiveView");
			}
		}

		private bool isSelectAll;
		public bool IsSelectAll
		{
			get
			{
				return isSelectAll;
			}
			set
			{
				isSelectAll = value;
				OnPropertyChanged("IsSelectAll");
			}
		}

		private bool isAccurateExplanation;
		public bool IsAccurateExplanation
		{
			get
			{
				return isAccurateExplanation;
			}
			set
			{
				isAccurateExplanation = value;
				OnPropertyChanged("IsAccurateExplanation");
			}
		}
		#endregion

		public static Options Options { get; set; } = new Options() { DetailLevel = ViewDetailLevel.Fine };

		private ObservableCollection<CategoryOp> categoryOps;
		public ObservableCollection<CategoryOp> CategoryOps
		{
			get
			{
				return categoryOps;
			}
			set
			{
				categoryOps = value;
				OnPropertyChanged("CategoryOps");
			}
		}

		private List<Element> elementsForFormworkFilter;
		public List<Element> ElementsForFormworkFilter
		{
			get
			{
				return elementsForFormworkFilter;
			}
			set
			{
				elementsForFormworkFilter = value;
				OnPropertyChanged("ElementsForFormworkFilter");
			}
		}

		private static List<Category> categories;
		public static List<Category> Categories
		{
			get
			{
				if (null == categories)
				{
					categories = new List<Category>()
					{
						Category.GetCategory(ActiveData.Document, BuiltInCategory.OST_StructuralColumns),
						Category.GetCategory(ActiveData.Document, BuiltInCategory.OST_Walls),
						Category.GetCategory(ActiveData.Document, BuiltInCategory.OST_StructuralFraming),
						Category.GetCategory(ActiveData.Document, BuiltInCategory.OST_Floors),
						Category.GetCategory(ActiveData.Document, BuiltInCategory.OST_EdgeSlab),
						Category.GetCategory(ActiveData.Document, BuiltInCategory.OST_StructuralFoundation),
						Category.GetCategory(ActiveData.Document, BuiltInCategory.OST_GenericModel),
						Category.GetCategory(ActiveData.Document, BuiltInCategory.OST_Stairs),
						Category.GetCategory(ActiveData.Document, BuiltInCategory.OST_Parts)
					};
				}
				return categories;
			}
		}

		public static double EpsilonLenght { get; set; } = 0.05.ToFeet();
		public static double EpsilonArea { get; set; } = 0.0025.ToSquareFeet();
		public static double EpsilonVolume { get; set; } = 0.000125.ToCubicFeet();

		public static double Thickness { get; set; } = 20.0.ToFeet();

		#region button
		public RelayCommand btnSaveSetting { get; set; }
		public RelayCommand btnRun { get; set; }
		public RelayCommand btnSelect { get; set; }
		#endregion

		public FormworkViewModels()
		{
			btnSelect = new RelayCommand(BtnSelect);
			btnSaveSetting = new RelayCommand(BtnSaveSetting);
			btnRun = new RelayCommand(BtnRun);
			GetSetting();
		}

		public void Excute()
		{
			Wmain = new QAQC_Formwork(this);
			Wmain.Show();
		}

		#region Method
		public void BtnSelect()
		{
			if (ActiveData.ActiveView is View3D)
			{
				try
				{
					Wmain.Hide();
					ElementsForFormworkFilter = new List<Element>();
					IList<Reference> references = 
						ActiveData.Selection.PickObjects(ObjectType.Element,
						SelectionFilter.GetElementFilter(e => null != e && !(e is Group) && !(e is RevitLinkInstance)), "Select the structural elements.");
					
					if (references.Count > 0)
					{
						ProgressBarInstance progressBarInstance = new ProgressBarInstance("Checking input information", references.Count);

						foreach (Reference reference in references)
						{
							progressBarInstance.Start();
							Element element = ActiveData.Document.GetElement(reference);

							if (CheckInputInformation(element))
							{
								ElementsForFormworkFilter.Add(element);
							}
						}

						CategoryOps = new ObservableCollection<CategoryOp>();
						foreach (string category in ElementsForFormworkFilter.Select(x => x.Category.Name).Distinct().OrderBy(x => x))
						{
							CategoryOps.Add(new CategoryOp() 
							{
								IsChecked = true,
								Name = category
							});
						}
						Wmain.Show();
					}

					else
					{
						MessageBox.Show($"Please check in Warning Addin!", "Revit");
						Wmain.Close();
					}

				}
				catch
				{
					Wmain.Show();
				}
			}
			else
			{
				MessageBox.Show("Only support the 3D view.");
			}
		}

		public void GetSetting()
		{

		}

		public void BtnSaveSetting()
		{

		}

		public void BtnRun()
		{
			if (CheckInputInfor())
			{
				Wmain.Hide();

				Action action = new Action(() =>
				{
					if (!ParameterEntity.CheckParameters)
					{
						ParameterEntity.CreateParameters();
					}

					List<string> filterCategories = CategoryOps.Where(c => c.IsChecked).Select(n => n.Name).ToList();
					List<Element> elements = ElementsForFormworkFilter.Where(e => filterCategories.Contains(e.Category.Name)).ToList();

					if (elements.Count > 0)
					{
						ProgressBarInstance progressBarInstance = new ProgressBarInstance("Calculating formwork ...", elements.Count);
						ResultFace resultFace;

						foreach (Element element in elements)
						{
							progressBarInstance.Start();

							resultFace = FactoryFace.GetFaceFilter(element, ElementsForFormworkFilter, new ParaUtil()
							{
								SelectInclusion = Include,
								IsActiveView = IsActiveView,
								IsAccurateExplanation = IsAccurateExplanation
							});


						}
					}
					Wmain.Show();
				});

				ExternalEventHandler.Instance.SetAction(action);

				ExternalEventHandler.Instance.Run();
			}
		}
		#endregion

		#region Utils
		public static bool CheckInputInformation(Element element)
		{
			if (null != element && null != element?.Category?.Name && Categories.Select(x => x.Name).Contains(element.Category.Name))
			{
				if (!(element is Wall) && !(element is Floor) && !(element is Stairs) && !(element is DirectShape) && !(element is Part))
				{
					if (element.get_Parameter(BuiltInParameter.HOST_VOLUME_COMPUTED).AsDouble() > GeometryLib.EPSILON_VOLUME &&
						IntersectElement.CanCalculateFormwork(element))
					{
						return true;
					}
				}

				if (element is Wall wall && wall.get_Parameter(BuiltInParameter.WALL_STRUCTURAL_SIGNIFICANT).AsInteger() == 1)
				{
					if (wall.get_Parameter(BuiltInParameter.HOST_VOLUME_COMPUTED).AsDouble() > GeometryLib.EPSILON_VOLUME && wall.WallType.Kind == WallKind.Basic)
					{
						return true;
					}
				}

				if (element is Floor floor && floor.get_Parameter(BuiltInParameter.FLOOR_PARAM_IS_STRUCTURAL).AsInteger() == 1)
				{
					if (floor.get_Parameter(BuiltInParameter.HOST_VOLUME_COMPUTED).AsDouble() > GeometryLib.EPSILON_VOLUME)
					{
						return true;
					}
				}

				if (element is Part part)
				{
					if (part.get_Parameter(BuiltInParameter.DPART_VOLUME_COMPUTED).AsDouble() > GeometryLib.EPSILON_VOLUME)
					{
						return true;
					}
				}

				if (element is Stairs stairs)
				{
					if (stairs.MultistoryStairsId.IntegerValue == -1)
					{
						if (CheckStairs(stairs))
						{
							return true;
						}
					}
				}
			}
			return false;
		}

		private static bool CheckStairs(Stairs stairs)
		{
			StairsType stairsType = stairs.Document.GetElement(stairs.GetTypeId()) as StairsType;
			if (stairsType.ConstructionMethod == StairsConstructionMethod.CastInPlace)
			{
				StairsRunType stairsRunType = stairs.Document.GetElement(stairsType.RunType) as StairsRunType;
				StairsLandingType stairsLandingType = stairs.Document.GetElement(stairsType.LandingType) as StairsLandingType;
				if (!stairsRunType.HasTreads && !stairsRunType.HasRisers && stairsRunType.NosingProfile == ElementId.InvalidElementId && stairsLandingType.IsMonolithic)
				{
					return true;
				}
			}
			return false;
		}

		private bool CheckInputInfor()
		{
			//if (null == ElementsForFormworkFilter || ElementsForFormworkFilter.Count > 0)
			//{
			//	if (AngleTopAreaFrom < AngleTopAreaTo)
			//	{
			//		if (AngleTopAreaTo <= 45 && AngleBottomAreaTo <= 45)
			//		{
			//			if (CategoryOptions.Count > 0 && CategoryOptions.Select(x => x.IsChecked).Contains(true))
			//			{
			//				if (DataReports.Count == 0)
			//				{
			//					return true;
			//				}
			//				else
			//				{
			//					NotifyUtils.SelectAgain();
			//					return false;
			//				}
			//			}
			//			else
			//			{
			//				Notification.ShowDialog("No categories were checked.", false);
			//				return false;
			//			}
			//		}
			//		else
			//		{
			//			Notification.ShowDialog("The upper bound angle greater than 45.", false);
			//			return false;
			//		}
			//	}
			//	else
			//	{
			//		Notification.ShowDialog("Input incorrect at top area setting.", false);
			//		return false;
			//	}
			//}
			//else
			//{
			//	NotifyUtils.SelectAgain();
			//	return false;
			//}
			return true;
		}
		#endregion
	}
}
