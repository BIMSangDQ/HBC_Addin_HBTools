using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autodesk.Revit.DB;
using Utils;

namespace HoaBinhTools.BIMQAQC.QAQC_FormWork.Models
{
	public class CategoryOp: ViewModelBase
	{
		public bool isChecked = false;
		public bool IsChecked
		{
			get
			{
				return isChecked;
			}
			set
			{
				isChecked = value;
				OnPropertyChanged("IsChecked");
			}
		}

		public Category Category { get; set; }

		public string name;
		public string Name
		{
			get
			{
				return name;
			}
			set
			{
				name = value;
				OnPropertyChanged("Name");
			}
		}

	}
}
