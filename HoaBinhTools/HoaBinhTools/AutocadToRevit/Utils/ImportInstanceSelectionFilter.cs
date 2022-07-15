#region Namespaces

using Autodesk.Revit.DB;
using Autodesk.Revit.UI.Selection;
#endregion

namespace HoaBinhTools.AutocadToRevit.Utils
{
	public class ImportInstanceSelectionFilter : ISelectionFilter
	{
		public bool AllowElement(Element elem)
		{
			return elem is ImportInstance;
		}
		public bool AllowReference(Reference reference, XYZ position)
		{
			return false;
		}
	}
}
