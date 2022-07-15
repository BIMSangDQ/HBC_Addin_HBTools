using System.Collections.Generic;
using SingleData;

namespace Utility
{
	public static class RebarCurveUtil
	{
		private static RevitModelData revitModelData
		{
			get { return RevitModelData.Instance; }
		}

		public static List<Autodesk.Revit.DB.Curve> ModifyCurve(this List<Autodesk.Revit.DB.Curve> curves,
			Autodesk.Revit.DB.Transform transform, string name)
		{
			Autodesk.Revit.DB.XYZ offsetVec1 = null, offsetVec2 = null;
			var fstCs = new List<Autodesk.Revit.DB.Curve>();

			for (int i = 0; i < curves.Count; i++)
			{
				Autodesk.Revit.DB.Curve curve = curves[i];
				if (revitModelData.ExcludeSquareStirrupShapeNames.Contains(name))
				{
					if (offsetVec1 == null)
					{
						offsetVec1 = -(curves[2] as Autodesk.Revit.DB.Line).Direction
							* 50.milimeter2Feet();
						offsetVec2 = (curves[curves.Count - 3] as Autodesk.Revit.DB.Line).Direction
							* 50.milimeter2Feet();
					}
					if (i <= 1)
					{
						curve = curve.CreateTransformed
							(Autodesk.Revit.DB.Transform.CreateTranslation(offsetVec1));
					}
					if (i == 2)
					{
						Autodesk.Revit.DB.XYZ p1 = curve.GetEndPoint(0), p2 = curve.GetEndPoint(1);
						curve = Autodesk.Revit.DB.Line.CreateBound(p1 + offsetVec1, p2);
					}
					if (i == curves.Count - 3)
					{
						Autodesk.Revit.DB.XYZ p1 = curve.GetEndPoint(0), p2 = curve.GetEndPoint(1);
						curve = Autodesk.Revit.DB.Line.CreateBound(p1, p2 + offsetVec2);
					}
					if (i >= curves.Count - 2)
					{
						curve = curve.CreateTransformed
							(Autodesk.Revit.DB.Transform.CreateTranslation(offsetVec2));
					}
				}

				if (revitModelData.ExcludeAntiTwistingSquareStirrupShapeNames.Contains(name))
				{
					if (offsetVec1 == null)
					{
						offsetVec1 = -(curves[2] as Autodesk.Revit.DB.Line).Direction
							* 50.milimeter2Feet();
						offsetVec2 = (curves[curves.Count - 3] as Autodesk.Revit.DB.Line).Direction
							* 50.milimeter2Feet();
					}
					if (i <= 1)
					{
						curve = curve.CreateTransformed
							(Autodesk.Revit.DB.Transform.CreateTranslation(offsetVec1));
					}
					if (i == 2)
					{
						Autodesk.Revit.DB.XYZ p1 = curve.GetEndPoint(0), p2 = curve.GetEndPoint(1);
						curve = Autodesk.Revit.DB.Line.CreateBound(p1 + offsetVec1, p2);
					}
					if (i == curves.Count - 3)
					{
						Autodesk.Revit.DB.XYZ p1 = curve.GetEndPoint(0), p2 = curve.GetEndPoint(1);
						curve = Autodesk.Revit.DB.Line.CreateBound(p1, p2 + offsetVec2);
					}
					if (i >= curves.Count - 2)
					{
						curve = curve.CreateTransformed
							(Autodesk.Revit.DB.Transform.CreateTranslation(offsetVec2));
					}
				}

				fstCs.Add(curve.CreateTransformed(transform));
			}
			return fstCs;
		}
	}
}
