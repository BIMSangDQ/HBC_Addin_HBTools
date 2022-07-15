using System;
using Autodesk.Revit.DB;

namespace Schedule2Excel2k16.Util
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
	}
}
