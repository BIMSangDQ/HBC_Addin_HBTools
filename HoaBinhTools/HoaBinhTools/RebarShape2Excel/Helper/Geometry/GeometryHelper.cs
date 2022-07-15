using System.Collections.Generic;
using System.Linq;
using Autodesk.Revit.DB;
using SingleData;
using Utility;

namespace Helper
{
	public abstract class AGeometryHelper
	{
		protected Model.Entity.Geometry geo;
		protected Autodesk.Revit.DB.Element e;
		public AGeometryHelper(Model.Entity.Geometry geo, Autodesk.Revit.DB.Element ele)
		{
			this.geo = geo; this.e = ele;
		}
		public abstract void ReadGeometry();

		public void GetTrueHeight()
		{
			BoundingBoxXYZ bb = e.get_BoundingBox(null);
			Outline ol = new Outline(bb.Min, bb.Max);
			Solid s = e.GetSingleSolid().ScaleSolid(1.001);

			BoundingBoxIntersectsFilter bbFilter = new BoundingBoxIntersectsFilter(ol);
			ElementIntersectsSolidFilter eisFilter = new ElementIntersectsSolidFilter(s);
			ElementClassFilter flFilter = new ElementClassFilter(typeof(Floor));
			ElementClassFilter fiFilter = new ElementClassFilter(typeof(FamilyInstance));
			ElementCategoryFilter beamCateFilter = new ElementCategoryFilter(BuiltInCategory.OST_StructuralFraming);
			LogicalAndFilter beamFilter = new LogicalAndFilter(new List<ElementFilter> { fiFilter, beamCateFilter });
			LogicalOrFilter floorOrBeamFilter = new LogicalOrFilter(new List<ElementFilter> { flFilter, beamFilter });

			List<Autodesk.Revit.DB.Element> filterElems = new FilteredElementCollector(RevitData.Instance.Document).WherePasses(floorOrBeamFilter).WherePasses(bbFilter).WherePasses(eisFilter).ToList();

			double zBot = 0, zTop = 0, zLimit = 0;
			bool assBot = false, assTop = false, assLimit = false;
			double zBCheck = geo.Origin.Z + geo.LengthZ / 3, zTCheck = geo.Origin.Z + geo.LengthZ * 2 / 3;

			for (int i = 0; i < filterElems.Count; i++)
			{
				BoundingBoxXYZ bbe = filterElems[i].get_BoundingBox(null);
				double mine = bbe.Min.Z, maxe = bbe.Max.Z;

				if (maxe.IsEqualOrSmaller(zBCheck))
				{
					if (!assBot)
					{
						zBot = maxe;
						assBot = true;
					}
					else
					{
						if (maxe.IsBigger(zBot))
							zBot = maxe;
					}
				}
				if (mine.IsBigger(zTCheck))
				{
					if (!assTop)
					{
						zTop = mine;
						assTop = true;
					}
					else
					{
						if (mine.IsSmaller(zTop))
							zTop = mine;
					}
					if (!assLimit)
					{
						zLimit = maxe;
						assLimit = true;
					}
					else
					{
						if (maxe.IsBigger(zLimit))
							zLimit = maxe;
					}
				}
			}

			if (!assBot) zBot = bb.Min.Z;
			if (!assTop) zTop = bb.Max.Z;
			if (!assLimit) zLimit = bb.Max.Z;

			geo.Origin = new Autodesk.Revit.DB.XYZ(geo.Origin.X, geo.Origin.Y, zBot);
			geo.LengthZ = zTop - zBot;
			geo.LengthLimit = zLimit - zBot;
		}
		public void GetSectionPoints()
		{
			geo.SectionPoints = new List<Autodesk.Revit.DB.XYZ>
			{
				geo.Origin,
				geo.Origin + geo.VectorX * geo.LengthX,
				geo.Origin + geo.VectorX * geo.LengthX + geo.VectorY * geo.LengthY,
				geo.Origin + geo.VectorY * geo.LengthY
			};
		}
	}
}
