using System.Collections.Generic;
using Autodesk.Revit.DB;

namespace Utility
{
	public static partial class LocationUtil
	{
		public static Model.Entity.Location GetLocation(this Model.Entity.Element elem)
		{

			var revitElem = elem.RevitElement;
			Model.Entity.Location loc = new Model.Entity.Location(elem);
			loc.Points = new List<Model.Entity.XYZ>();

			switch (elem.Category.BuiltInCategory)
			{
				case BuiltInCategory.OST_StructuralFraming:
				case BuiltInCategory.OST_Walls:
					{
						var locCurve = (revitElem.Location as LocationCurve).Curve;

						locCurve.GetEndPoint(0).GetPoint(loc);
						//locCurve.GetEndPoint(1).GetPoint(loc);
					}
					break;
				case BuiltInCategory.OST_StructuralColumns:
					{
						var locPnt = (revitElem.Location as LocationPoint).Point;

						locPnt.GetPoint(loc);
					}
					break;
				case BuiltInCategory.OST_Floors:
				case BuiltInCategory.OST_StructuralFoundation:
					{
						var locPnt = elem.Solid.ComputeCentroid();

						locPnt.GetPoint(loc);
					}
					break;
				default:
					throw new Model.Exception.CaseNotCheckException();
			}
			return loc;
		}
	}
}
