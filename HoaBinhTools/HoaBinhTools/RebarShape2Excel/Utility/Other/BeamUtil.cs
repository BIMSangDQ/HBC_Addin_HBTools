using System;
using Autodesk.Revit.DB;
using SingleData;

namespace Utility
{
	public static partial class BeamUtil
	{
		private static RevitModelData revitModelData
		{
			get
			{
				return RevitModelData.Instance;
			}
		}
		public static bool IsContinuousBeam(this Autodesk.Revit.DB.Element elem,
			Autodesk.Revit.DB.Element targetElem)
		{
			if (elem.Category.Id.IntegerValue != targetElem.Category.Id.IntegerValue
				|| elem.Category.Id.IntegerValue != (int)BuiltInCategory.OST_StructuralFraming)
				throw new Exception("Some element in parameters is not Beam.");

			if ((elem as FamilyInstance).Symbol.Family.IsInPlace) return false;

			var line1 = (elem.Location as LocationCurve).Curve as Line;
			var line2 = (targetElem.Location as LocationCurve).Curve as Line;
			if (line1 == null || line2 == null) return false;

			var vec1 = line1.Direction; var vec2 = line2.Direction;
			if (!vec1.IsSameOrOppositeDirection(vec2)) return false;

			var distance2LinesY = line2.DistanceFromPointToLine(line1.Origin);
			if (distance2LinesY.IsBigger((1000).milimeter2Feet())) return false;

			XYZ p11 = line1.GetEndPoint(0), p12 = line1.GetEndPoint(1), p21 = line2.GetEndPoint(0),
				p22 = line2.GetEndPoint(1);
			double distance2Pnts = 0;
			if (vec1.IsSameDirection(vec2))
			{
				var dis1 = p11.DistanceTo(p22);
				var dis2 = p12.DistanceTo(p21);
				distance2Pnts = dis1 < dis2 ? dis1 : dis2;
			}
			else
			{
				var dis1 = p11.DistanceTo(p21);
				var dis2 = p21.DistanceTo(p22);
				distance2Pnts = dis1 < dis2 ? dis1 : dis2;
			}
			var distance2LinesX = Math.Sqrt(distance2Pnts * distance2Pnts - distance2LinesY
				* distance2LinesY);
			if (distance2LinesX.IsBigger((1000).milimeter2Feet())) return false;
			return true;
		}
	}
}
