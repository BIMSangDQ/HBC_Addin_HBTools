using System.Collections.Generic;
using Autodesk.Revit.DB;

namespace HoaBinhTools.LeanConcrete.Models
{
	class ElementSortMax : IComparer<CurveLoop>
	{
		int IComparer<CurveLoop>.Compare(CurveLoop poly1, CurveLoop poly2)
		{
			CurveLoop Poly1 = poly1 as CurveLoop;

			CurveLoop Poly2 = poly2 as CurveLoop;

			if (Poly1.GetExactLength() > Poly2.GetExactLength())
			{
				return -1;
			}

			else if (Poly1.GetExactLength() == Poly2.GetExactLength())
			{
				return 0;
			}

			else
			{
				return 1;
			}
		}

	}

	class ElementSortMin : IComparer<CurveLoop>
	{
		int IComparer<CurveLoop>.Compare(CurveLoop poly1, CurveLoop poly2)
		{
			CurveLoop Poly1 = poly1 as CurveLoop;

			CurveLoop Poly2 = poly2 as CurveLoop;

			if (Poly1.GetExactLength() < Poly2.GetExactLength())
			{
				return -1;
			}

			else if (Poly1.GetExactLength() == Poly2.GetExactLength())
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
