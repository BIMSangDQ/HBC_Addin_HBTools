using System;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI.Selection;

namespace Utils
{
	public class FilterCategoryUtils : ISelectionFilter
	{

		public Func<Element, bool> FuncElement { get; set; }
		public Func<Reference, bool> FuncReference { get; set; }


		public bool AllowElement(Element elem)
		{
			return FuncElement(elem);
		}

		public bool AllowReference(Reference reference, XYZ position)
		{
			return FuncReference(reference);
		}
	}



}
