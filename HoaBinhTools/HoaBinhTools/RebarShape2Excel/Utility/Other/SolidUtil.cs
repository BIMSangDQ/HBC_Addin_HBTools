using System;
using System.Collections.Generic;
using System.Linq;
using Autodesk.Revit.DB;
using SingleData;

namespace Utility
{
	public static class SolidUtil
	{
		public static Solid GetOriginalSolid(Element e)
		{
			var revitData = RevitData.Instance;
			var doc = revitData.Document;

			Solid solid = null;
			if (e is FamilyInstance)
			{
				var fi = e as FamilyInstance;
				foreach (var geoObj in fi.GetOriginalGeometry(new Options()))
				{
					if (geoObj is Solid)
					{
						Solid s = geoObj as Solid;
						if (s.Volume != 0)
						{
							s = SolidUtils.CreateTransformed(s, fi.GetTotalTransform());
							solid = s;
						}
					}
				}
			}
			else if (e is Floor)
			{
				try
				{
					var rf = HostObjectUtils.GetBottomFaces(e as HostObject).First();
					var face = e.GetGeometryObjectFromReference(rf) as Face;
					var normal = face.ComputeNormal(UV.Zero);
					solid = GeometryCreationUtilities.CreateExtrusionGeometry(face.GetEdgesAsCurveLoops(), -normal, e.GetProximityHeight());
				}
				catch (Autodesk.Revit.Exceptions.InternalException)
				{
					foreach (var geoObj in e.get_Geometry(new Options()))
					{
						if (geoObj is Solid)
						{
							Solid s = geoObj as Solid;
							if (s.Volume != 0)
							{
								solid = s;
							}
						}
					}
				}
				catch (Autodesk.Revit.Exceptions.ArgumentException)
				{
					foreach (var geoObj in e.get_Geometry(new Options()))
					{
						if (geoObj is Solid)
						{
							Solid s = geoObj as Solid;
							if (s.Volume != 0)
							{
								solid = s;
							}
						}
					}
				}
			}
			else if (e is Wall)
			{
				try
				{
					var rf = HostObjectUtils.GetSideFaces(e as HostObject, ShellLayerType.Exterior).First();
					var face = e.GetGeometryObjectFromReference(rf) as Face;
					var normal = face.ComputeNormal(UV.Zero);
					solid = GeometryCreationUtilities.CreateExtrusionGeometry(face.GetEdgesAsCurveLoops(), -normal, (e as Wall).Width);
				}
				catch (Autodesk.Revit.Exceptions.InternalException)
				{
					foreach (var geoObj in e.get_Geometry(new Options()))
					{
						if (geoObj is Solid)
						{
							Solid s = geoObj as Solid;
							if (s.Volume != 0)
							{
								solid = s;
							}
						}
					}
				}
				catch (Autodesk.Revit.Exceptions.ArgumentException)
				{
					foreach (var geoObj in e.get_Geometry(new Options()))
					{
						if (geoObj is Solid)
						{
							Solid s = geoObj as Solid;
							if (s.Volume != 0)
							{
								solid = s;
							}
						}
					}
				}
			}
			else
			{
				foreach (var geoObj in e.get_Geometry(new Options()))
				{
					if (geoObj is Solid)
					{
						Solid s = geoObj as Solid;
						if (s.Volume != 0)
						{
							solid = s;
						}
					}
				}
			}
			return solid;
		}
		public static Solid GetSingleSolid(this Element e)
		{
			Solid s = null;
			foreach (GeometryObject geoObj in e.get_Geometry(new Options()))
			{
				if (geoObj is Solid)
				{
					s = geoObj as Solid;
					if (s != null)
						if (s.Faces.Size != 0 && s.Edges.Size != 0) goto L1;
				}
				if (geoObj is GeometryInstance)
					foreach (GeometryObject geoObj2 in (geoObj as GeometryInstance).GetInstanceGeometry())
					{
						s = geoObj2 as Solid;
						if (s != null)
							if (s.Faces.Size != 0 && s.Edges.Size != 0) goto L1;
					}
			}
			L1:
			return s;
		}
		public static Solid CreateMergeSolid(this IEnumerable<Model.Entity.Element> elems)
		{
			Solid fullSolid = null;
			try
			{
				foreach (var elem in elems)
				{
					if (fullSolid == null) fullSolid = elem.Solid;
					else { fullSolid = BooleanOperationsUtils.ExecuteBooleanOperation(fullSolid, elem.Solid, BooleanOperationsType.Union); }
				}
			}
			catch
			{
				throw;
			}
			return fullSolid;
		}
		public static IEnumerable<Model.Entity.Element> GetIntersectElements(this Model.Entity.Element e, List<BuiltInCategory> categories)
		{
			var revitData = RevitData.Instance;
			var modelData = ModelData.Instance;
			var doc = revitData.Document;

			var bb = e.RevitElement.get_BoundingBox(null);
			bb = bb.ScaleBoundingBox(1.001);

			Solid s = e.Solid;
			s = s.ScaleSolid(1.001);

			var elems = new FilteredElementCollector(doc).WhereElementIsNotElementType()
				.WherePasses(new ElementMulticategoryFilter(categories))
				.WherePasses(new BoundingBoxIntersectsFilter(new Outline(bb.Min, bb.Max)))
				.WherePasses(new ElementIntersectsSolidFilter(s)).GetEntityElements(x => x.Guid != e.Guid);

			return elems;
		}
		public static bool IsInOriginialSolidFace(this Solid s, Face face)
		{
			foreach (Face face2 in s.Faces)
			{
				try
				{
					if (face2.ComputeNormal(UV.Zero).IsSameDirection(face.ComputeNormal(UV.Zero)))
					{
						if (face2.Project(face.Evaluate(UV.Zero)).Distance.IsEqual(0))
						{
							return true;
						}
					}
				}
				catch
				{
					continue;
				}
			}
			return false;
		}
		public static Solid CreateSolidFromGeometryInfo(this Model.Entity.Geometry geo)
		{
			List<Level> levels = SingleData.RevitData.Instance.Levels.OrderBy(x => x.Elevation).ToList();

			double offset = 10000.milimeter2Feet();

			XYZ origin = new XYZ(geo.Origin.X, geo.Origin.Y, levels.First().Elevation - offset);
			List<Curve> curves = new List<Curve>();

			curves.Add(Line.CreateBound(origin, origin + geo.VectorX * geo.LengthX));
			curves.Add(Line.CreateBound(origin + geo.VectorX * geo.LengthX, origin + geo.VectorX * geo.LengthX + geo.VectorY * geo.LengthY));
			curves.Add(Line.CreateBound(origin + geo.VectorX * geo.LengthX + geo.VectorY * geo.LengthY, origin + geo.VectorY * geo.LengthY));
			curves.Add(Line.CreateBound(origin + geo.VectorY * geo.LengthY, origin));

			CurveLoop curloop = CurveLoop.Create(curves);
			return GeometryCreationUtilities.CreateExtrusionGeometry(new List<CurveLoop> { curloop }, XYZ.BasisZ, levels.Last().Elevation + offset - levels.First().Elevation);
		}
		public static List<Element> GetOverlapColumnWith(this Element e, Func<Element, bool> filterElem)
		{
			Model.Entity.Element elem = new Model.Entity.Element();

			Model.Entity.Geometry geometry = new Model.Entity.Geometry();
			geometry.Element = elem; elem.Geometry = geometry;
			geometry.ReadGeometry();

			Solid solid = geometry.CreateSolidFromGeometryInfo();
			ElementIntersectsSolidFilter eisFilter = new ElementIntersectsSolidFilter(solid);
			List<Element> elems = new FilteredElementCollector(SingleData.RevitData.Instance.Document).WhereElementIsNotElementType().WherePasses(eisFilter)
				.Where(filterElem).ToList();

			elems = elems.OrderBy(x => (SingleData.RevitData.Instance.Document.GetElement(x.LevelId) as Level).Elevation).ToList();
			return elems;
		}
		public static Solid ScaleSolid(this Solid solid, double x)
		{
			XYZ centralPnt = solid.ComputeCentroid();
			Transform tf = Transform.Identity;
			tf.BasisX = XYZ.BasisX * x;
			tf.BasisY = XYZ.BasisY * x;
			tf.BasisZ = XYZ.BasisZ * x;

			Solid s = SolidUtils.CreateTransformed(solid, tf);
			tf = Transform.CreateTranslation(centralPnt - s.ComputeCentroid());
			s = SolidUtils.CreateTransformed(solid, tf);
			return s;
		}
		public static List<Solid> GetTargetSolidFromElementAndIntersectPaths(
			this Model.Entity.Element element, List<BuiltInCategory> categories,
			Model.Entity.Mass mass)
		{
			var revitData = RevitData.Instance;
			var app = revitData.Application;
			var doc = revitData.Document;

			Solid s1 = element.Solid;

			Solid s2 = null, fullSolid = null;
			try
			{
				s2 = element.GetIntersectElements(categories).CreateMergeSolid();
				if (s2 == null)
				{
					return new List<Solid> { s1 };
				}
				fullSolid = BooleanOperationsUtils.ExecuteBooleanOperation(s1, s2, BooleanOperationsType.Union);
			}
			catch (Autodesk.Revit.Exceptions.InvalidOperationException)
			{
				var failure = FailureUtil.GetFailure(Model.Entity.FailureType.CalculatingGeometry);
				mass.Failure = failure;
				return null;
			}
			catch (Autodesk.Revit.Exceptions.InternalException)
			{
				var failure = FailureUtil.GetFailure(Model.Entity.FailureType.Unknown);
				mass.Failure = failure;
				return null;
			}

			if (mass.MassType == Model.Entity.MassType.Concrete)
			{
				return new List<Solid> { fullSolid, s2 };
			}
			return new List<Solid> { fullSolid };
		}
		public static BoundingBoxXYZ ScaleBoundingBox(this BoundingBoxXYZ bb, double x)
		{
			var min = bb.Min; var max = bb.Max; var origin = (min + max) / 2;
			return new BoundingBoxXYZ { Min = origin + (min - origin) * x, Max = origin + (max - origin) * x };
		}
		public static double GetProximityHeight(this Element e)
		{
			var bb = e.get_BoundingBox(null);
			return bb.Max.Z - bb.Min.Z;
		}

	}
}
