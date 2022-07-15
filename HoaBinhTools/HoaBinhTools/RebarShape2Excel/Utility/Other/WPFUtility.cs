using System.Collections.ObjectModel;
using System.Linq;
using Autodesk.Revit.DB;
using SingleData;

namespace Utility
{
	public static class WPFUtil
	{
		public static ObservableCollection<ViewSchedule> GetViewSchedules()
		{
			ObservableCollection<ViewSchedule> viewSchedules = new ObservableCollection<ViewSchedule>();
			RevitData.Instance.ViewSchedules.Where(x => x.Definition.CategoryId.IntegerValue == (int)BuiltInCategory.OST_Rebar).OrderBy(x => x.Name).ToList().ForEach(x => viewSchedules.Add(x));

			return viewSchedules;
		}
	}
}
