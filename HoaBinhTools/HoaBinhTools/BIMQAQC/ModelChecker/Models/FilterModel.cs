using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autodesk.Revit.DB;
using HoaBinhTools.AutocadToRevit.Utils;
using Utils;

namespace HoaBinhTools.BIMQAQC.ModelChecker.Models
{
	public class FilterModel : ViewModelBase
	{
		public int id { get; set; }
		public bool isChecked { get; set; }
		public List<string> OperatorEnum
		{
			get
			{
				return new List<string>()
				{
						"AND",
						"OR",
						"EXCLUDE"
				};
			}
		}
		public string Operator { get; set; }
		public List<string> CriteriaEnum
		{
			get
			{
				return new List<string>()
				{
					"API_PARAMETER",
					"API_TYPE",
					"CATEGORY",
					"DESIGN_OPTION",
					"FAMILY",
					"HOST",
					"HOST_PARAMETER",
					"LEVEL",
					"PARAMETER",
					"PARAMETER IN TYPE",
					"PHASE_CREATED",
					"PHASE_DEMOLISHED",
					"PHASE_STATUS",
					"REDUNDANT",
					"ROOM",
					"SPACE",
					"STRUCTURAL_TYPE",
					"TYPE",
					"TYPE_OR_INSTANCE",
					"VIEW",
					"WORKSET"
				};
			}
		}
		private string criteria;
		public string Criteria
		{
			get
			{
				return criteria;
			}
			set
			{

				criteria = value; OnPropertyChanged(nameof(Criteria));
				FilterUtilModel filterUtilModel = new FilterUtilModel();
				Properties = filterUtilModel.getProperties(Criteria);
			}

		}
		private ObservableCollection<string> properties;
		public ObservableCollection<string> Properties
		{
			get
			{

				return properties;
			}
			set
			{

				properties = value; OnPropertyChanged(nameof(Properties));
			}
		}
		public string Property { get; set; }
		public ObservableCollection<string> Conditions { get; set; }
		public string Condition { get; set; }
		public string Value { get; set; }
	}

	public class FilterUtilModel
	{
		public ObservableCollection<string> getProperties(string Criteria)
		{
			ObservableCollection<string> Properties = new ObservableCollection<string>();
			switch (Criteria)
			{
				case "API_PARAMETER":
					break;
				case "API_TYPE":
					Properties.Add("Full Class Name");
					break;
				case "CATEGORY":
					Properties.Add("Name");
					var enumlist = System.Enum.GetValues(typeof(Autodesk.Revit.DB.BuiltInCategory)).Cast<Autodesk.Revit.DB.BuiltInCategory>().ToList();
					foreach (var enumitem in enumlist)
					{
						Properties.Add(enumitem.ToString());
					}
					break;
				case "DESIGN_OPTION":
					Properties.Add("Name");
					break;
				case "FAMILY":
					Properties.Add("Name");
					Properties.Add("Is In-Place");
					break;
				case "HOST":
					Properties.Add("Is Defined");
					break;
				case "HOST_PARAMETER":
					break;
				case "LEVEL":
					Properties.Add("Name");
					Properties.Add("Elevation");
					break;
				case "PARAMETER":
					IList<Element> allElements = new FilteredElementCollector(ActiveData.Document)
						.WhereElementIsNotElementType()
						.Where(e => e.Category != null)
						.ToList();

					allElements =allElements.Distinct(new IEqualityComparerCategory()).ToList();

					List<string> AllParameters = new List<string>();

					foreach (Element e in allElements)
					{
						AllParameters.AddRange(ParameterUtils.GetAllParameters(e));
					}

					AllParameters = AllParameters.Distinct().ToList();
					AllParameters.Sort();
					foreach (string parametername in AllParameters)
					{
						Properties.Add(parametername);
					}
					break;
				case "PARAMETER IN TYPE":
					break;
				case "PHASE_CREATED":
					Properties.Add("Name");
					break;
				case "PHASE_DEMOLISHED":
					Properties.Add("Name");
					break;
				case "PHASE_STATUS":
					break;
				case "REDUNDANT":
					Properties.Add("Location");
					Properties.Add("Type");
					break;
				case "ROOM":
					Properties.Add("Name");
					Properties.Add("Number");
					Properties.Add("Is Defined");
					break;
				case "SPACE":
					Properties.Add("Name");
					Properties.Add("Number");
					Properties.Add("Is Defined");
					break;
				case "STRUCTURAL_TYPE":
					Properties.Add("Value");
					break;
				case "TYPE":
					Properties.Add("Name");
					break;
				case "TYPE_OR_INSTANCE":
					Properties.Add("Is Element Type");
					break;
				case "VIEW":
					Properties.Add("Name");
					Properties.Add("Is Defined");
					break;
				case "WORKSET":
					Properties.Add("Name");
					break;
				default:
					break;
			}
			return Properties;
		}
	}


}
