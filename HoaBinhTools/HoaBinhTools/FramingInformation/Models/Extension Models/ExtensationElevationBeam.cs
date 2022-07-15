using System;
using Autodesk.Revit.DB;
using Utils;

namespace HoaBinhTools.FramingInformation.Models
{
	public static class ElevationBeam
	{
		public static double? GetBeamElevationTop(this Element Ele)
		{
			double? Elevationtop = Ele.get_Parameter(BuiltInParameter.STRUCTURAL_ELEVATION_AT_TOP).AsDouble().FootToMm();

			if (Elevationtop != 0)
			{

				return Math.Round((double)Elevationtop, 3);
			}
			else
			{
				// edit ddaay lay face Top
				return Math.Round((Ele as FamilyInstance).GetLocationCurve().Midpoint().Z, 3);
			}
		}


		public static double? GetBeamElevationBottom(this Element Ele)
		{
			double? ElevationBottom = Ele.get_Parameter(BuiltInParameter.STRUCTURAL_ELEVATION_AT_BOTTOM).AsDouble().FootToMm();

			if (ElevationBottom != 0)
			{
				return Math.Round((double)ElevationBottom, 3);
			}
			else
			{
				// edit ddaay lay face Top
				return Math.Round((Ele as FamilyInstance).GetLocationCurve().Midpoint().Z, 3);
			}
		}


		public static string GetReferenceLevel(this Element Ele)
		{
			return Ele.LookupParameter("Reference Level").AsValueString();
		}
	}
}
