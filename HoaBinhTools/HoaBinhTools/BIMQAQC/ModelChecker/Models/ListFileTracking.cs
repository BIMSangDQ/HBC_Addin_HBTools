using System.Collections.ObjectModel;
using Autodesk.Revit.DB;
using Utils;

namespace HoaBinhTools.BIMQAQC.ModelChecker.Models
{
	public class FileTracking : ViewModelBase
	{
		public string Path { get; set; }
		public string FileName { get; set; }
	}
}
