using System.Collections.Generic;
using Autodesk.Revit.DB;

namespace HoaBinhTools.AutocadToRevit.Utils
{
	public class IEqualityComparerCategory : IEqualityComparer<Element>
	{
		// Loại bõ 1 trong 2 element trùng nhau nếu kết quả của hàm này trả về true
		public bool Equals(Element x, Element y)
		{
			if (x == null || y == null)
				return false;

			Category firstCategory = x.Category;
			Category secondCategory = y.Category;

			if (firstCategory == null || secondCategory == null)
			{
				return false;
			}

			return x.Category.Name.Equals(y.Category.Name);
		}

		public int GetHashCode(Element obj)
		{
			return 0;
		}
	}
}
