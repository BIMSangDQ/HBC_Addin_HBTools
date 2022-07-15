using System.Collections.Generic;
using System.Linq;
using Autodesk.Revit.DB;
using HoaBinhTools.FramingInformation.Models;
using Utils;

namespace HoaBinhTools.FramingInformation
{
	public static class GridModels
	{
		public static List<Grid> GetGridForFraming(this Curve curFraming, XYZ SortPoint)
		{
			List<Grid> ListGrid = new List<Grid>();

			Document doc = ActiveData.Document;

			var planeZ = Plane.CreateByNormalAndOrigin(XYZ.BasisZ, XYZ.Zero);

			XYZ VectoFraming = curFraming.Direction();

			foreach (var grid in new FilteredElementCollector(doc).WhereElementIsNotElementType().OfCategory(BuiltInCategory.OST_Grids).Cast<Grid>().ToList())
			{
				var curveGrid = grid.Curve.ProjectCurveToPlane(planeZ);

				XYZ vectorcurrve = curveGrid.Direction();

				SetComparisonResult Inter = curFraming.Intersect(curveGrid);

				var test = vectorcurrve.DotProduct(VectoFraming);

				if (vectorcurrve.DotProduct(VectoFraming) < 0.01 && Inter == SetComparisonResult.Overlap)
				{
					ListGrid.Add(grid);
				}
			}

			if (ListGrid.Count == 0)
			{
				return null;
			}

			ListGrid.Sort(new SortGrid(SortPoint));

			return ListGrid;
		}
	}
}
