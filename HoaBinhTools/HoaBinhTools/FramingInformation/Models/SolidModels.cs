using System.Collections.Generic;
using Autodesk.Revit.DB;


namespace HoaBinhTools.FramingInformation.Models
{
	public static class SolidModels
	{
		public static Solid GetSolidToCurveloop(this CurveLoop curloop)
		{

			var Trans = Transform.CreateTranslation(XYZ.BasisZ * 80);

			curloop.Transform(Trans);

			return GeometryCreationUtilities.CreateExtrusionGeometry(new List<CurveLoop>() { curloop }, -XYZ.BasisZ, 250);
		}
	}
}
