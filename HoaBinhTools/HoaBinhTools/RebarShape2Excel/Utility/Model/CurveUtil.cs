using System.Collections.Generic;
using SingleData;

namespace Utility
{
	public static class CurveUtil
	{
		private static RevitData revitData
		{
			get { return RevitData.Instance; }
		}
		public static void EvaluateCurveOn2D(this IEnumerable<Autodesk.Revit.DB.Curve> curves)
		{
			var doc = revitData.Document;
			var view = revitData.ActiveView;
			var sel = revitData.Selection;

			var workPlane = revitData.WorkPlane;

			var pnt = sel.PickPoint();
			Autodesk.Revit.DB.XYZ vec = null;
			var vecX = view.RightDirection;
			var vecY = view.UpDirection;
			Autodesk.Revit.DB.XYZ orgPnt = null;
			Autodesk.Revit.DB.Transform transform = null;

			foreach (var item in curves)
			{
				if (vec == null)
				{
					orgPnt = item.GetEndPoint(0);
					vec = pnt - orgPnt;
					transform = Autodesk.Revit.DB.Transform.CreateTranslation(vec);
				}
				//var vec1 = item.GetEndPoint(0) - orgPnt;
				//var pnt1 = pnt + vecX * vec1.X + vecY * vec1.Y;
				//var vec2 = item.GetEndPoint(1) - orgPnt;
				//var pnt2 = pnt + vecX * vec2.X + vecY * vec2.Y;

				//var line = Autodesk.Revit.DB.Line.CreateBound(pnt1, pnt2);
				//doc.Create.NewDetailCurve(revitData.ActiveView, line);

				doc.Create.NewDetailCurve(revitData.ActiveView, item.CreateTransformed(transform));
			}

		}
	}
}
