using System.Collections.Generic;
using Autodesk.Revit.DB;
using Autodesk.Revit.DB.Structure;


namespace Utils
{
	public static class RebarCurves
	{
		public static List<Curve> GetRebarCurves(this List<Rebar> rebars)
		{
			List<Curve> curves = new List<Curve>();

			int n, nElements = 0, nCurves = 0;

			foreach (Rebar rebar in rebars)
			{
				++nElements;

				n = rebar.NumberOfBarPositions;

				nCurves += n;

				for (int i = 0; i < n; ++i)
				{
					IList<Curve> centerlineCurves = rebar.GetCenterlineCurves(true, false, false, MultiplanarOption.IncludeAllMultiplanarCurves, i);

					// Move the curves to their position.

					if (rebar.IsRebarShapeDriven())
					{
						RebarShapeDrivenAccessor accessor = rebar.GetShapeDrivenAccessor();

						Transform trf = accessor.GetBarPositionTransform(i);

						foreach (Curve c in centerlineCurves)
						{
							curves.Add(c.CreateTransformed(trf));
						}
					}
					else
					{
						// This is a Free Form Rebar
						foreach (Curve c in centerlineCurves)
						{
							curves.Add(c);
						}
					}
				}
			}
			return curves;
		}

		public static List<Curve> GetRebarCurves(this Rebar rebar)
		{
			List<Curve> curves = new List<Curve>();

			int n = rebar.NumberOfBarPositions;

			for (int i = 0; i < n; ++i)
			{

				IList<Curve> centerlineCurves = rebar.GetCenterlineCurves(true, false, false, MultiplanarOption.IncludeAllMultiplanarCurves, i);

				// Move the curves to their position.

				if (rebar.IsRebarShapeDriven())
				{
					RebarShapeDrivenAccessor accessor = rebar.GetShapeDrivenAccessor();

					Transform trf = accessor.GetBarPositionTransform(i);

					foreach (Curve c in centerlineCurves)
					{
						curves.Add(c.CreateTransformed(trf));
					}
				}
				else
				{
					// This is a Free Form Rebar
					foreach (Curve c in centerlineCurves)
					{
						curves.Add(c);
					}
				}
			}
			return curves;
		}
	}
}
