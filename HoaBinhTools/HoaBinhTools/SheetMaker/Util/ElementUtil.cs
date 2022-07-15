using System;
using Autodesk.Revit.DB;

namespace SheetDuplicateAndAlignView.Util
{
	public class ElementUtil
	{
		public static String ElementInfo(Element ele)
		{
			if (ele == null)
				return "";

			String typeName = ele.GetType().Name;

			if (ele is FamilyInstance)
			{
				FamilyInstance fi = ele as FamilyInstance;
				typeName = "FamilyInstance: " + fi.StructuralType.ToString();
			}

			String categoryName = (null == ele.Category) ? String.Empty : ele.Category.Name + " ";

			return String.Format("{0} <{1} | {2} | {3}>", typeName, ele.Id.IntegerValue, ele.Name, categoryName);

		}

		public static String ViewElementInfo(View ele)
		{
			if (ele == null)
				return "";

			return String.Format("{0} <{1} | {2}>", ele.Name, ele.ViewType.ToString(), ele.Id.IntegerValue);

		}

	}
}
