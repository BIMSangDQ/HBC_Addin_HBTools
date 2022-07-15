using System.Collections.Generic;
using Autodesk.Revit.DB;

namespace HoaBinhTools.FramingInformation.Models
{
	public class SortGrid : IComparer<Grid>
	{
		public XYZ original { get; set; }
		public SortGrid(XYZ original)
		{
			this.original = original;
		}

		int IComparer<Grid>.Compare(Grid poly1, Grid poly2)
		{
			Grid Poly1 = poly1 as Grid;

			Grid Poly2 = poly2 as Grid;

			if (GetSmallestDistance(Poly1) < GetSmallestDistance(Poly2))
			{
				return -1;
			}

			else if (GetSmallestDistance(Poly1) == GetSmallestDistance(Poly2))
			{
				return 0;
			}

			else
			{
				return 1;
			}
		}


		public double GetSmallestDistance(Grid Cur)
		{
			var p0 = Cur.Curve.GetEndPoint(0).DistanceTo(original);

			var p1 = Cur.Curve.GetEndPoint(1).DistanceTo(original);

			return p0 < p1 ? p0 : p1;

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

	public class SortContiuousBeam : IComparer<Element>
	{
		public XYZ original { get; set; }
		public SortContiuousBeam(XYZ original)
		{
			this.original = original;
		}

		int IComparer<Element>.Compare(Element Ele1, Element Ele2)
		{
			XYZ Poly1 = (Ele1 as FamilyInstance).GetLocationCurve().Evaluate(0.5, false);

			XYZ Poly2 = (Ele2 as FamilyInstance).GetLocationCurve().Evaluate(0.5, false);

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
