using System.Collections.Generic;
using Autodesk.Revit.DB;

namespace Utils
{

	// xóa các điểm giống nhau
	public class XYZEqualityComparer : IEqualityComparer<XYZ>
	{
		bool IEqualityComparer<XYZ>.Equals(XYZ b1, XYZ b2)
		{
			if (b2 == null && b1 == null)
				return true;
			else if (b1 == null || b2 == null) return false;

			// lấy tổng chiều dài
			if (b2.DistanceTo(b1) < 0.1)
			{
				return true;
			}

			else
			{
				return false;
			}
		}

		public int GetHashCode(XYZ obj)
		{
			return "NguyenChiLinh".GetHashCode();

		}

	}


	public class ComparerElementID : IEqualityComparer<Element>
	{
		bool IEqualityComparer<Element>.Equals(Element b1, Element b2)
		{
			if (b2 == null && b1 == null)
				return true;
			else if (b1 == null || b2 == null) return false;


			if (b2.Id == b1.Id)
			{
				return true;
			}

			else
			{
				return false;
			}
		}

		public int GetHashCode(Element obj)
		{
			return "NguyenChiLinh".GetHashCode();

		}

	}





	// xóa các đường curve giống nhau




}
