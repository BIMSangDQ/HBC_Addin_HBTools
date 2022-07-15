using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autodesk.Revit.DB;
using Utils;

namespace HoaBinhTools.BIMQAQC.QAQC_Quantity.Models
{
	public class ExcludeFilters : ViewModelBase
	{
		private string categorySelect;
		public string CategorySelect
		{
			get
			{
				return categorySelect;
			}
			set
			{
				categorySelect = value;
				OnPropertyChanged("CategorySelect");
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
				criteria = value;
				OnPropertyChanged("Criteria");
			}
		}

		private string _property;
		public string Property
		{
			get
			{
				return _property;
			}
			set
			{
				_property = value;
				OnPropertyChanged("Property");
			}
		}

		private bool isTypeName = false ;
		public bool IsTypeName
		{
			get
			{
				return isTypeName;
			}
			set
			{
				isTypeName = value;
				OnPropertyChanged("IsTypeName");
			}
		}

		private string _Condition;
		public string Condition
		{
			get
			{
				return _Condition;
			}
			set
			{
				_Condition = value;
				OnPropertyChanged("Condition");
			}
		}

		private string _Value;
		public string Value
		{
			get
			{
				return _Value;
			}
			set
			{
				_Value = value;
				OnPropertyChanged("Value");
			}
		}

		public List<string> Categories { get; set; }
		public List<string> ListCriteria { get; set; }

		private List<string> _ListCondition;
		public List<string> ListCondition
		{
			get
			{
				return _ListCondition;
			}
			set
			{
				_ListCondition = value;
				OnPropertyChanged("ListCondition");
			}
		}

		public ExcludeFilters()
		{
			Categories = new List<string>()
			{
				"Structural Framing",
				"Structural Column",
				"Walls",
				"Floors"
			};

			ListCriteria = new List<string>()
			{
			"Type Name",
			"Parameter Value"
			};

			ListCondition = new List<string>()
			{
			"=",
			"!=",
			"Contains",
			"Does not contains",
			"Match Regex"
			};
		}
	}
}
