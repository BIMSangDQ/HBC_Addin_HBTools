using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Utils;

namespace HoaBinhTools.BIMQAQC.ModelChecker.Models
{
	public class FilterGroupModel : ViewModelBase
	{
		public ObservableCollection<FilterGroupAnd> Filters	{get;set;}

		public FilterGroupExclude FilterGroupExclude {get;set;}
	}

	public class FilterGroupExclude : ViewModelBase
	{
		public ObservableCollection<FilterGroupAnd> Filters { get; set; }
	}

	public class FilterGroupAnd : ViewModelBase
	{
		public string Id { get; set; }
	}
}
