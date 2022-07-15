using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Utils;

namespace HoaBinhTools.BIMQAQC.ModelChecker.Models
{
	public class GeneralMandatoryCondition: ViewModelBase
	{
		public ObservableCollection<string> criterias = new ObservableCollection<string>()
		{
			"File Name",
			"File Size",
			"Warrnings/Total Element",
			"Total Element",
			"Purable_Elements",
			"Model Group",
			"Detail Group",
			"In Place Families",
			"Duplicate Intances",
			"View not on sheet",
			"Sheets",
			"Hidden Elements",
			"Cad Import",
			"Link Revit",
			"Link Cad",
			"Worksets",
			"Project Infor",
			"Project Location",
			"Level and Grid on wrong workset",
			"Wrong element on grid workset"
		};

		public ObservableCollection<string> Criterias
		{
			get
			{
				return criterias;
			}
			set
			{
				criterias = value;
				OnPropertyChanged("Criterias");
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
		private double rangeA;
		private double rangeB;
		private double rangeC;
		public double RangeA
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
		public double RangeB
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
		public double RangeC
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
	}
}
