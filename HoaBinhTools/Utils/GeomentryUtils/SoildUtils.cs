using System;
using System.Collections.Generic;
using System.Linq;
using Autodesk.Revit.DB;
using Autodesk.Revit.DB.Structure;

namespace Utils
{
	public static class UtilsSolid
	{

		public static Solid CreateGlobular(this XYZ center, double radius)
		{
			List<Curve> profile = new List<Curve>();

			// first create sphere with 2' radius

			XYZ profilePlus = center + new XYZ(0, radius, 0);

			XYZ profileMinus = center - new XYZ(0, radius, 0);

			profile.Add(Line.CreateBound(profilePlus, profileMinus));

			profile.Add(Arc.Create(profileMinus, profilePlus, center + new XYZ(radius, 0, 0)));

			CurveLoop curveLoop = CurveLoop.Create(profile);

			Frame frame = new Frame(center, XYZ.BasisX, -XYZ.BasisZ, XYZ.BasisY);

			Plane.Create(frame);

			return GeometryCreationUtilities.CreateRevolvedGeometry(frame, new CurveLoop[] { curveLoop }, 0, 2 * Math.PI);
		}

		public static Solid GetOriginSolidFramingSweptGeometry(this FamilyInstance framing, Document doc)
		{
			if (framing.Location is LocationCurve locationCurve)
			{
				Curve framingLocation = locationCurve.Curve;

				Solid solid = GetSolidsFromOriginalFamilyInstance(framing);

				IEnumerable<PlanarFace> planarFaces = solid.Faces.OfType<PlanarFace>();

				if (planarFaces.Count() > 0)
				{
					foreach (PlanarFace planarFace in planarFaces)
					{

						if (planarFace.FaceNormal.AngleBetweenTwoVectors(framingLocation.ComputeDerivatives(0, true).BasisX, false) < 1)
						{
							planarFace.Intersect(framingLocation, out IntersectionResultArray intersectionResultArray);
							if (null != intersectionResultArray)
							{
								return GeometryCreationUtilities.CreateSweptGeometry(CurveLoop.Create(new List<Curve>() { framingLocation }), 0, intersectionResultArray.get_Item(0).Parameter, planarFace.GetEdgesAsCurveLoops());
							}
							intersectionResultArray?.Dispose();
						}
					}
					foreach (PlanarFace planarFace in planarFaces)
					{
						if (planarFace.FaceNormal.AngleBetweenTwoVectors(framingLocation.ComputeDerivatives(1, true).BasisX, false) < 1)
						{
							planarFace.Intersect(framingLocation, out IntersectionResultArray intersectionResultArray);

							if (null != intersectionResultArray)
							{
								return GeometryCreationUtilities.CreateSweptGeometry(
																						 CurveLoop.Create(new List<Curve>() { framingLocation })

																						 , 0
																						 , intersectionResultArray.get_Item(0).Parameter
																						 , planarFace.GetEdgesAsCurveLoops()
																					 );
							}
							intersectionResultArray?.Dispose();
						}
					}
				}
			}

			return null;
		}




		/// <summary>
		/// Lấy soild bị cắt bởi các vật thể xung quanh
		/// </summary>
		public static Solid GetSoildIntersectElement(this Solid ExtrusionSoild, Document doc)
		{
			var Eles = ExtrusionSoild.GetElementIntersectsSolid(doc);

			foreach (Element Ele in Eles)
			{
				Solid soil = null;

				if (Ele is FamilyInstance)
				{
					var Fami = Ele as FamilyInstance;

					soil = Fami.GetAllSolids().First();

				}
				else
				{
					soil = Ele.GetAllSolids().First();
				}
				try
				{
					BooleanOperationsUtils.ExecuteBooleanOperationModifyingOriginalSolid(ExtrusionSoild, soil, BooleanOperationsType.Difference);
				}
				catch { }


			}
			return ExtrusionSoild;

		}



		public static Solid GetSolidFromMinMaxPoints(this XYZ minPoint, XYZ maxPoint)
		{

			XYZ point0 = new XYZ(minPoint.X, minPoint.Y, minPoint.Z);
			XYZ point1 = new XYZ(maxPoint.X, minPoint.Y, minPoint.Z);
			XYZ point2 = new XYZ(maxPoint.X, maxPoint.Y, minPoint.Z);
			XYZ point3 = new XYZ(minPoint.X, maxPoint.Y, minPoint.Z);

			Line line0 = Line.CreateBound(point0, point1);
			Line line1 = Line.CreateBound(point1, point2);
			Line line2 = Line.CreateBound(point2, point3);
			Line line3 = Line.CreateBound(point3, point0);
			List<Curve> curves = new List<Curve>() { line0, line1, line2, line3 };
			List<CurveLoop> curveLoops = new List<CurveLoop>() { CurveLoop.Create(curves) };
			return GeometryCreationUtilities.CreateExtrusionGeometry(curveLoops, XYZ.BasisZ, Math.Abs(maxPoint.Z - minPoint.Z));

		}



		public static List<Solid> GetAllSolids(this FamilyInstance instance)
		{
			var transform = Transform.CreateTranslation(XYZ.Zero);
			List<Solid> solidList = new List<Solid>();
			if (instance == null)
				return solidList;
			GeometryElement geometryElement = instance.get_Geometry(new Options()
			{
				ComputeReferences = true
			});
			List<GeometryObject> geometryObjectList = new List<GeometryObject>();
			bool flag = false;
			foreach (GeometryObject geometryObject1 in geometryElement)
			{
				geometryObjectList.Add(geometryObject1);
				GeometryInstance geometryInstance = geometryObject1 as GeometryInstance;
				if (null != geometryInstance)
				{
					var tf = geometryInstance.Transform;
					foreach (GeometryObject geometryObject2 in geometryInstance.GetSymbolGeometry())
					{
						Solid solid = geometryObject2 as Solid;
						if (!(null == solid) && solid.Faces.Size != 0 && solid.Edges.Size != 0)
						{
							solidList.Add(SolidUtils.CreateTransformed(solid, tf));
						}
					}
				}
				Solid solid1 = geometryObject1 as Solid;
				if (!(null == solid1) && solid1.Faces.Size != 0 && solid1.Edges.Size != 0)
					solidList.Add(solid1);
			}
			if (flag)
				transform = instance.GetTransform();
			return solidList;
		}

		public static Solid GetSolidZero()
		{

			var Cr = new CurveLoop();

			Cr.Append(Line.CreateBound(new XYZ(0, 0, 0), new XYZ(0, 10, 0)));

			Cr.Append(Line.CreateBound(new XYZ(0, 10, 0), new XYZ(10, 10, 0)));

			Cr.Append(Line.CreateBound(new XYZ(10, 10, 0), new XYZ(10, 0, 0)));

			Cr.Append(Line.CreateBound(new XYZ(10, 0, 0), new XYZ(0, 0, 0)));

			var soild = GeometryCreationUtilities.CreateExtrusionGeometry(new List<CurveLoop>() { Cr }, -XYZ.BasisZ, 5);


			return BooleanOperationsUtils.ExecuteBooleanOperation(soild, soild, BooleanOperationsType.Difference);

		}

		static public Solid Clone(this Solid solid)
		{
			return SolidUtils.Clone(solid);
		}

		public static Solid UnionSoild(this Solid solid1, Solid soild2)
		{
			if (solid1 == null || soild2 == null)
			{
				return null;
			}
			return BooleanOperationsUtils.ExecuteBooleanOperation(solid1, soild2, BooleanOperationsType.Union);
		}


		public static Solid UnionSoilds(this List<Solid> solids)
		{
			Solid SoliTM = null;

			foreach (var soli in solids)
			{

				if (soli == null)
				{
					return null;
				}

				if (SoliTM == null)
				{
					SoliTM = soli;
				}
				else
				{
					SoliTM = BooleanOperationsUtils.ExecuteBooleanOperation(soli, SoliTM, BooleanOperationsType.Union);
				}

			}
			return SoliTM;
		}





		public static Solid GetSolidsFromOriginalFamilyInstance(this FamilyInstance framing)
		{
			GeometryElement GeoOri = framing.GetOriginalGeometry(new Options());

			List<Solid> Soli = new List<Solid>();

			foreach (var Geo in GeoOri)
			{
				var s = Geo as Solid;

				if (s == null) continue;

				if (s.Volume > 0)
				{
					Soli.Add(SolidUtils.CreateTransformed(s, framing.GetTransform()));
				}

			}
			return Soli.UnionSoilds();
		}


		public static List<Solid> GetAllSolids(this Element instance, bool transformedSolid = false, View view = null)
		{
			List<Solid> solidList = new List<Solid>();
			if (instance == null)
				return solidList;

			GeometryElement geometryElement = instance.get_Geometry(new Options()
			{
				ComputeReferences = true
			});


			foreach (GeometryObject geometryObject1 in geometryElement)
			{

				GeometryInstance geometryInstance = geometryObject1 as GeometryInstance;
				if (null != geometryInstance)
				{
					var tf = geometryInstance.Transform;
					foreach (GeometryObject geometryObject2 in geometryInstance.GetSymbolGeometry())
					{
						Solid solid = geometryObject2 as Solid;
						if (!(null == solid) && solid.Faces.Size != 0 && solid.Edges.Size != 0)
						{
							if (transformedSolid)
							{
								solidList.Add(SolidUtils.CreateTransformed(solid, tf));
							}
							solidList.Add(solid);
						}
					}
				}
				Solid solid1 = geometryObject1 as Solid;
				if (!(null == solid1) && solid1.Faces.Size != 0)
					solidList.Add(solid1);
			}
			return solidList;
		}



		public static Solid GetSolidOriginalFromFamilyInstance(this FamilyInstance Fami)
		{
			var tranf = Fami.GetTotalTransform();

			Solid SolidColumns = null;

			foreach (var Geo in Fami.GetOriginalGeometry(new Options()))
			{
				if (Geo is Solid Soli)
				{
					Soli = SolidUtils.CreateTransformed(Soli, tranf);

					if (SolidColumns == null)
					{
						SolidColumns = Soli;
					}
					else
					{
						SolidColumns = SolidColumns.UnionSoild(Soli);
					}
				}
			}
			return SolidColumns;
		}




		private static Solid GetSolidOrginSlab(this Element floor, Document doc, double Thickness, double bottomElevationFoundation)
		{
			SubTransaction Sub = new SubTransaction(doc);

			Sub.Start();

			ICollection<ElementId> elementIds = doc.Delete(floor.Id);

			Sub.RollBack();

			Sub.Dispose();

			CurveLoop sketchBoundaryFloor = new CurveLoop();

			bool sketchFound = false;

			foreach (ElementId elementId in elementIds)
			{
				if (doc.GetElement(elementId) is ModelCurve modelCurve)
				{
					Curve curve = modelCurve.GeometryCurve;


					if (Math.Abs(curve.Evaluate(0.5, true).Z - bottomElevationFoundation) < 0.001)
					{
						sketchBoundaryFloor.Append(curve);
						sketchFound = true;
					}
					else
					{
						Transform transform = Transform.CreateTranslation(-XYZ.BasisZ * (curve.Evaluate(0.5, true).Z - bottomElevationFoundation));
						sketchBoundaryFloor.Append(curve.CreateTransformed(transform));
						sketchFound = true;
					}
				}
				else
				{
					if (sketchFound) break;
				}
			}
			return GeometryCreationUtilities.CreateExtrusionGeometry(new List<CurveLoop> { sketchBoundaryFloor }, XYZ.BasisZ, Thickness);
		}


		public static Solid GetQuickSolidOrigin(this Element Ele, Document doc)
		{
			try
			{
				// Neu La FamilyIntance
				if (Ele is FamilyInstance Fami)
				{
					// Neu La model Plane thi lay binh thuong
					if (Fami.Symbol.Family.IsInPlace)
					{
						return Ele.GetSlowSolidOrigin(doc);
					}
					else
					{
						// truong Hop cotOST_StructuralColumns
						if (Fami.Category.Name.Contains("Columns"))
						{
							return GetSolidOriginalFromFamilyInstance(Fami);

						}
						else if (Fami.Category.Name.Contains("Framing"))
						{
							return Fami.GetOriginSolidFramingSweptGeometry(doc);
						}
						else
						{
							// Mong
							return Ele.GetSlowSolidOrigin(doc);

						}
					}
				}
				else
				{
					if (Ele.Category.Name.Contains("Walls"))
					{
						//Truong Hop Nay la Tuong thang Dung
						Wall LocationCurveWall = (Ele as Wall);

						Curve CurveWall = (LocationCurveWall.Location as LocationCurve).Curve;

						var Baseofset = LocationCurveWall.get_Parameter(BuiltInParameter.WALL_BASE_OFFSET).AsDouble();

						var VectorWall = Transform.CreateTranslation(XYZ.BasisZ * Baseofset);

						CurveWall = CurveWall.CreateTransformed(VectorWall);

						var Curloop = CurveLoop.CreateViaThicken(CurveWall, LocationCurveWall.Width, XYZ.BasisZ);

						var Height = LocationCurveWall.LookupParameter("Unconnected Height").AsDouble();

						var solid = GeometryCreationUtilities.CreateExtrusionGeometry(new List<CurveLoop> { Curloop }, XYZ.BasisZ, Height);

						return solid;


					}
					else if (Ele.Category.Name.Contains("Foundation") || Ele.Category.Name.Contains("Floors"))
					{
						var Bottom = Ele.LookupParameter("Elevation at Bottom").AsDouble();

						if (!Bottom.IsZero())
						{
							var thickness = Ele.LookupParameter("Thickness").AsDouble();

							return GetSolidOrginSlab(Ele, doc, thickness, Bottom);
						}
						else
						{
							return Ele.GetSlowSolidOrigin(doc);
						}
					}

					else
					{
						return Ele.GetSlowSolidOrigin(doc);
					}
				}
			}
			catch
			{
				return Ele.GetSlowSolidOrigin(doc);
			}
		}


		public static Solid GetSlowSolidOrigin(this Element e, Document doc)
		{

			Solid soild = null;

			using (SubTransaction t = new SubTransaction(ActiveData.Document))
			{
				t.Start();

				var Listelejoin = JoinGeometryUtils.GetJoinedElements(doc, e);

				foreach (ElementId i in Listelejoin)
				{
					var EleJoin = doc.GetElement(i);

					JoinGeometryUtils.UnjoinGeometry(doc, e, EleJoin);
				}


				if (e.Category.Name.Contains("Framing") && e is FamilyInstance Fami)
				{
					if (!Fami.Symbol.Family.IsInPlace)
					{

						if (StructuralFramingUtils.IsJoinAllowedAtEnd(Fami, 0))
						{
							StructuralFramingUtils.DisallowJoinAtEnd(Fami, 0);
						}

						if (StructuralFramingUtils.IsJoinAllowedAtEnd(Fami, 1))
						{
							StructuralFramingUtils.DisallowJoinAtEnd(Fami, 1);
						}


					}

				}

				if (e.Category.Name.Contains("Walls") && e is Wall wall)
				{
					if (WallUtils.IsWallJoinAllowedAtEnd(wall, 0))
					{
						WallUtils.DisallowWallJoinAtEnd(wall, 0);
					}

					if (WallUtils.IsWallJoinAllowedAtEnd(wall, 1))
					{
						WallUtils.DisallowWallJoinAtEnd(wall, 1);
					}

				}

				ActiveData.Document.Regenerate();

				soild = e.GetAllSolids().UnionSoilds().Clone();

				t.RollBack();

				t.Dispose();

			}

			return soild;
		}

		public static double GetProximityHeight(this Element e)
		{

			var bb = e.get_BoundingBox(null);
			return bb.Max.Z - bb.Min.Z;
		}

		//public static bool DoesIntersect(Solid firstSolid, Solid secondSolid)
		//{
		//    if (firstSolid == null || secondSolid == null)
		//    {
		//        return false;
		//    }
		//    else
		//    {

		//        Solid solidOne = SolidUtils.Clone(firstSolid);

		//        Solid solidTwo = SolidUtils.Clone(secondSolid);

		//        Solid solidUnion = new List<Solid>() { solidOne, solidTwo }.ToUnionSolid();

		//        if (null == solidUnion)
		//        {
		//            try
		//            {
		//                Solid solidIntersect = BooleanOperationsUtils.ExecuteBooleanOperation(solidOne, solidTwo, BooleanOperationsType.Intersect);

		//                if (solidIntersect.Volume > GeometryLib.EPSILON_VOLUME)
		//                {
		//                    return true;
		//                }
		//                else
		//                {
		//                    if (IntersectSolids(solidOne, solidTwo))
		//                    {
		//                        return true;
		//                    }
		//                    else if (IntersectSolids(solidTwo, solidOne))
		//                    {
		//                        return true;
		//                    }
		//                    else
		//                    {
		//                        return false;
		//                    }
		//                }
		//            }
		//            catch
		//            {
		//                try
		//                {
		//                    Solid solidDifference = BooleanOperationsUtils.ExecuteBooleanOperation(solidOne, solidTwo, BooleanOperationsType.Difference);

		//                    if (Math.Abs(solidOne.Volume - solidDifference.Volume) > GeometryLib.EPSILON_VOLUME)
		//                    {
		//                        return true;
		//                    }
		//                    else
		//                    {
		//                        if (IntersectSolids(solidOne, solidTwo))
		//                        {
		//                            return true;
		//                        }
		//                        else if (IntersectSolids(solidTwo, solidOne))
		//                        {
		//                            return true;
		//                        }
		//                        else
		//                        {
		//                            return false;
		//                        }
		//                    }
		//                }
		//                catch
		//                {
		//                    if (IntersectSolids(solidOne, solidTwo))
		//                    {
		//                        return true;
		//                    }
		//                    else if (IntersectSolids(solidTwo, solidOne))
		//                    {
		//                        return true;
		//                    }
		//                    else
		//                    {
		//                        return false;
		//                    }
		//                }
		//            }
		//        }
		//        else
		//        {
		//            bool booArea = Math.Abs(solidUnion.SurfaceArea - solidOne.SurfaceArea - solidTwo.SurfaceArea) > GeometryLib.EPSILON_VOLUME;
		//            bool booEdge = Math.Abs(solidOne.Edges.Size + solidTwo.Edges.Size - solidUnion.Edges.Size) > 0;
		//            if (booArea || booEdge)
		//            {
		//                return true;
		//            }
		//            else
		//            {
		//                if (IntersectSolids(solidOne, solidTwo))
		//                {
		//                    return true;
		//                }
		//                else if (IntersectSolids(solidTwo, solidOne))
		//                {
		//                    return true;
		//                }
		//                else
		//                {
		//                    return false;
		//                }
		//            }
		//        }
		//    }
		//}



		//public static bool IntersectSolids(Solid solidOne, Solid solidTwo)
		//{
		//    foreach (Face faceOne in solidOne.Faces)
		//    {
		//        foreach (Edge edge in solidTwo.Edges)
		//        {
		//            IList<XYZ> epts = edge.Tessellate();
		//            foreach (XYZ ept in epts)
		//            {
		//                try
		//                {
		//                    IntersectionResult intersectionResult = faceOne.Project(ept);
		//                    if (null != intersectionResult)
		//                    {
		//                        if (Math.Abs(intersectionResult.Distance) < GeometryLib.EPSILON)
		//                        {
		//                            intersectionResult.Dispose();
		//                            return true;
		//                        }
		//                        intersectionResult.Dispose();
		//                    }
		//                }
		//                catch { }
		//            }
		//        }
		//    }
		//    return false;
		//}


		//public static Solid ToUnionSolid(this IList<Solid> solids)
		//{
		//    if (solids.Count == 0)
		//    {
		//        return null;
		//    }
		//    if (solids.Count == 1)
		//    {
		//        return solids.First();
		//    }
		//    else
		//    {
		//        Solid solid = solids[0];
		//        for (int i = 1; i < solids.Count; i++)
		//        {
		//            try
		//            {
		//                solid = BooleanOperationsUtils.ExecuteBooleanOperation(solid, solids[i], BooleanOperationsType.Union);
		//            }
		//            catch
		//            {
		//                return null;
		//            }
		//        }
		//        return solid;
		//    }
		//}
	}
}
