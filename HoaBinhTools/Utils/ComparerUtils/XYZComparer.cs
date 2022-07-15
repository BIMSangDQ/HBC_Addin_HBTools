using System.Collections.Generic;
using Autodesk.Revit.DB;

namespace Utils
{
	public class XYZComparer : IComparer<XYZ>
	{
		int IComparer<XYZ>.Compare(XYZ first, XYZ second)
		{
			if (MathUtils.RevitEquals(first.Z, second.Z))
			{
				if (!MathUtils.RevitEquals(first.Y, second.Y))
				{
					if (!first.Y.IsGreater(second.Y))
					{
						return -1;
					}
					return 1;
				}

				if (first.X.RevitEquals(second.X))
				{
					return 0;
				}
				if (first.X.IsGreater(second.X))
				{
					return 1;
				}
				return -1;
			}

			if (!first.Z.IsGreater(second.Z))
			{
				return -1;
			}
			return 1;
		}
	}

	public class SortPoint : IComparer<XYZ>
	{
		public XYZ original { get; set; }
		public SortPoint(XYZ original)
		{
			this.original = original;
		}

		int IComparer<XYZ>.Compare(XYZ poly1, XYZ poly2)
		{
			XYZ Poly1 = poly1 as XYZ;

			XYZ Poly2 = poly2 as XYZ;

			if (Poly1.DistanceTo(original) < Poly2.DistanceTo(original))
			{
				return -1;
			}

			else if (Poly1.DistanceTo(original) == Poly2.DistanceTo(original))
			{
				return 0;
			}

			else
			{
				return 1;
			}
		}
	}
}
