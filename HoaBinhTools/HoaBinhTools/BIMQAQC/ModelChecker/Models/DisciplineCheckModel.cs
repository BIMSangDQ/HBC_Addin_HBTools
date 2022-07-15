using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Utils;

namespace HoaBinhTools.BIMQAQC.ModelChecker.Models
{
	public class DisciplineCheckModel : ViewModelBase
	{
		public ObservableCollection<DisciplineCheck> Checks { get; set; }
	}

	public class DisciplineCheck : ViewModelBase
	{
		public int Id { get; set; }
		public string Name { get; set; }
		public string Description { get; set; }
		public string Failure_Message { get; set; }
		public string Check_Result { get; set; }
		public bool SetRun { get; set; }
		public string RangeA { get; set; }
		public string RangeB { get; set; }
		public string RangeC { get; set; }
		public string RangeD { get; set; }
		public bool IsImportant { get; set; }
		public ObservableCollection<DisciplineFilter> Filters { get; set; }

		public string ListParaResult { get; set; }
	}

	public class DisciplineFilter : ViewModelBase
	{
		public int ID { get; set; }

		public string Oparator { get; set; }

		public ObservableCollection<string> Oparators {get; set; }

		public string Criteria {get; set;}

		public ObservableCollection<string> Criterias { get; set; }

		public string Property{ get; set; }

		private ObservableCollection<string> properties;
		public ObservableCollection<string> Properties 
		{ 
			get 
			{
				return properties;
			}
			set 
			{
				properties = value;
				OnPropertyChanged("Properties");
			}
		}

		public string Condition { get; set; }

		private ObservableCollection<string> conditions;
		public ObservableCollection<string> Conditions
		{
			get
			{
				return conditions;
			}
			set
			{
				conditions = value;
				OnPropertyChanged("Conditions");
			}
		}

		private string _value;
		public string Value 
		{
			get
			{
				return _value;
			}
			set
			{
				_value = value;
				OnPropertyChanged("Value");
			}
		}

		public DisciplineFilter()
		{
			Oparators = new ObservableCollection<string>(){
				"AND", "OR", "EXCLUDE"};

			Criterias = new ObservableCollection<string>(){
			"CATEGORY",
			"FAMILY",
			"LEVEL",
			"API PARAMETER",
			"PARAMETER",
			"ROOM",
			"STRUCTURAL TYPE",
			"TYPE",
			"TYPE OR INSTANCE",
			"VIEW",
			"WORKSET",
			"WARNING",
			"LOCATION",
			"BASE AND TOP LEVEL",
			"IsMonitoringLinkElement"};

			Conditions = new ObservableCollection<string>(){
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
			"Is Consecutive",
			"Same value, yet different type",
			"Same value, yet different para",
			"Type From Instance"};

			Properties = new ObservableCollection<string>();
		}
	}
}
