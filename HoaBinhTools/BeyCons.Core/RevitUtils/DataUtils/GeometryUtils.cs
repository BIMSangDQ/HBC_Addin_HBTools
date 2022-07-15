#region Using
using Autodesk.Revit.DB;
using BeyCons.Core.Extensions;
using BeyCons.Core.FormUtils.ControlViews;
using BeyCons.Core.RevitUtils.DataUtils.Models;
using BeyCons.Core.Libraries.Geometries;
using BeyCons.Core.Libraries.Units;
using System;
using System.Collections.Generic;
using System.Linq;
#endregion

namespace BeyCons.Core.RevitUtils.DataUtils
{
    public static class GeometryUtils
    {

        #region Fields
        private static List<Solid> solids;
        private static List<GenericForm> genericForms;
        private static List<Curve> curves;
        private static List<Face> faces;
        private static List<Mesh> meshes;
        private static List<Surface> surfaces;
        private static List<GeometryObject> geometryObjects;
        #endregion

        #region Properties
        private static List<Solid> Solids
        {
            get
            {
                if (solids == null) solids = new List<Solid>();
                return solids;
            }
            set
            {
                solids = value;
            }
        }
        private static List<GenericForm> GenericForms
        {
            get
            {
                if (genericForms == null) genericForms = new List<GenericForm>();
                return genericForms;
            }
            set
            {
                genericForms = value;
            }
        }
        private static List<Curve> Curves
        {
            get
            {
                if (curves == null) curves = new List<Curve>();
                return curves;
            }
            set
            {
                curves = value;
            }
        }
        private static List<Face> Faces
        {
            get
            {
                if (faces == null) faces = new List<Face>();
                return faces;
            }
            set
            {
                faces = value;
            }
        }
        private static List<Mesh> Meshes
        {
            get
            {
                if (meshes == null) meshes = new List<Mesh>();
                return meshes;
            }
            set
            {
                meshes = value;
            }
        }
        private static List<Surface> Surfaces
        {
            get
            {
                if (surfaces == null) surfaces = new List<Surface>();
                return surfaces;
            }
            set
            {
                surfaces = value;
            }
        }
        private static List<GeometryObject> GeometryObjects
        {
            get
            {
                if (geometryObjects == null) geometryObjects = new List<GeometryObject>();
                return geometryObjects;
            }
            set
            {
                geometryObjects = value;
            }
        }
        #endregion

        #region Method
        public static List<GeometryObject> GetGeometryObjectsFromInstanceElement(Element element, Options options, bool isActual)
        {
            GeometryObjects = new List<GeometryObject>();
            if (element is Wall wall && wall.IsStackedWall)
            {
                IList<ElementId> elementIds = wall.GetStackedWallMemberIds();
                foreach (ElementId elementId in elementIds)
                {
                    Element memberWallStack = element.Document.GetElement(elementId);
                    GeometryElement geometryElement = memberWallStack.get_Geometry(options);
                    if (geometryElement != null)
                    {
                        RecursiveGeometryObjects(geometryElement, isActual);
                    }
                }
            }
            else
            {
                GeometryElement geometryElement = element.get_Geometry(options);
                if (geometryElement != null)
                {
                    RecursiveGeometryObjects(geometryElement, isActual);
                }
            }
            return GeometryObjects;
        }
        private static void RecursiveGeometryObjects(GeometryElement geometry, bool isActual)
        {
            foreach (GeometryObject geometryObject in geometry)
            {
                if (geometryObject is GeometryInstance)
                {
                    GeometryInstance instance = geometryObject as GeometryInstance;
                    if (isActual)
                    {
                        GeometryElement geo = instance.GetInstanceGeometry();
                        if (geo != null)
                        {
                            RecursiveGeometryObjects(geo, isActual);
                        }
                    }
                    else
                    {
                        GeometryElement geo = instance.GetSymbolGeometry();
                        if (geo != null)
                        {
                            RecursiveGeometryObjects(geo, isActual);
                        }
                    }
                }
                else
                {
                    if (geometryObject != null)
                    {
                        GeometryObjects.Add(geometryObject);
                    }
                }
            }
        }
        public static List<Solid> GetSolidsFromInstanceElement(Element element, Options options, bool isActual)
        {
            return GetGeometryObjectsFromInstanceElement(element, options, isActual).OfType<Solid>().Where(x => x.Volume > GeometryLib.EPSILON_VOLUME).ToList();
        }
        public static List<Solid> GetSolidsFromOriginalFamilyInstance(FamilyInstance familyInstance, Options options)
        {
            Solids = new List<Solid>();
            GeometryElement geometryElement = familyInstance.GetOriginalGeometry(options);
            if (geometryElement != null)
            {
                RecursiveOriginalInstanceSolids(geometryElement, familyInstance.GetTotalTransform());
            }
            return Solids;
        }
        private static void RecursiveOriginalInstanceSolids(GeometryElement geometry, Transform transform)
        {
            foreach (GeometryObject geometryObject in geometry)
            {
                if (geometryObject is GeometryInstance)
                {
                    GeometryInstance instance = geometryObject as GeometryInstance;
                    GeometryElement geo = instance.GetInstanceGeometry();
                    if (geo != null)
                    {
                        RecursiveOriginalInstanceSolids(geo, transform);
                    }
                }
                else if (geometryObject is Solid)
                {
                    Solid solid = geometryObject as Solid;
                    if (solid.Volume > GeometryLib.EPSILON_VOLUME)
                    {
                        Solids.Add(SolidUtils.CreateTransformed(solid, transform));
                    }
                }
            }
        }
        public static List<GenericForm> GetGenericFormsFromFamily(FamilyInstance familyInstance, bool isSolid)
        {
            GenericForms = new List<GenericForm>();
            if (familyInstance != null)
            {
                RecursiveGenericForms(familyInstance, isSolid);
            }
            return GenericForms;
        }
        private static void RecursiveGenericForms(FamilyInstance familyInstance, bool isSolid)
        {
            Family family = familyInstance.Symbol.Family;
            if (family.IsEditable)
            {
                try
                {
                    Document documentFamily = familyInstance.Document.EditFamily(family);
                    List<GenericForm> genericForms = new FilteredElementCollector(documentFamily).OfClass(typeof(GenericForm)).Cast<GenericForm>().ToList();
                    foreach (GenericForm genericForm in genericForms)
                    {
                        if (isSolid)
                        {
                            if (genericForm.IsSolid)
                            {
                                GenericForms.Add(genericForm);
                            }
                        }
                        else
                        {
                            if (!genericForm.IsSolid)
                            {
                                GenericForms.Add(genericForm);
                            }
                        }
                    }
                    List<FamilyInstance> familyInstancesNesteds = new FilteredElementCollector(documentFamily).OfClass(typeof(FamilyInstance)).Cast<FamilyInstance>().ToList();
                    if (familyInstancesNesteds.Count > 0)
                    {
                        foreach (FamilyInstance familyInstancesNested in familyInstancesNesteds)
                        {
                            RecursiveGenericForms(familyInstancesNested, isSolid);
                        }
                    }
                }
                catch { }
            }
        }
        public static List<Curve> GetAllCurvesFromInstanceElement(Element element, Options options, bool isActual)
        {
            List<GeometryObject> geometryObjects = GetGeometryObjectsFromInstanceElement(element, options, isActual);
            Curves = geometryObjects.OfType<Curve>().Where(c => c.Length > element.Document.Application.ShortCurveTolerance).ToList();
            IEnumerable<FaceArray> faceArrays = geometryObjects.OfType<Solid>().Where(s => s.Volume > GeometryLib.EPSILON_VOLUME).Select(x => x.Faces).Where(y => !y.IsEmpty);
            foreach (FaceArray faceArray in faceArrays)
            {
                foreach (Face face in faceArray)
                {
                    if (face.Area > GeometryLib.EPSILON_AREA)
                    {
                        Curves.AddRange(face.GetEdgesAsCurveLoops().SelectMany(y => y).Where(l => l.Length > element.Document.Application.ShortCurveTolerance));
                    }
                }
            }
            return Curves;
        }
        public static List<Curve> GetCurvesFromInstanceElement(Element element, Options options, bool isActual)
        {
            return GetGeometryObjectsFromInstanceElement(element, options, isActual).OfType<Curve>().Where(c => c.Length > element.Document.Application.ShortCurveTolerance).ToList();
        }
        public static List<Point> GetPointsFromInstanceElement(Element element, Options options, bool isActual)
        {
            return GetGeometryObjectsFromInstanceElement(element, options, isActual).OfType<Point>().ToList();
        }
        public static List<PolyLine> GetPolyLinesFromInstanceElement(Element element, Options options, bool isActual)
        {
            return GetGeometryObjectsFromInstanceElement(element, options, isActual).OfType<PolyLine>().ToList();
        }
        public static List<Edge> GetEdgesFromInstanceElement(Element element, Options options, bool isActual)
        {
            return GetGeometryObjectsFromInstanceElement(element, options, isActual).OfType<Edge>().Where(c => c.ApproximateLength > element.Document.Application.ShortCurveTolerance).ToList();
        }
        public static List<Profile> GetProfilesFromInstanceElement(Element element, Options options, bool isActual)
        {
            return GetGeometryObjectsFromInstanceElement(element, options, isActual).OfType<Profile>().ToList();
        }
        public static List<Face> GetFacesFromInstanceElement(Element element, Options options, bool isActual)
        {
            List<GeometryObject> geometryObjects = GetGeometryObjectsFromInstanceElement(element, options, isActual);
            Faces = geometryObjects.OfType<Face>().ToList();
            IEnumerable<FaceArray> faceArrays = geometryObjects.OfType<Solid>().Where(s => s.Volume > GeometryLib.EPSILON_VOLUME).Select(x => x.Faces).Where(y => !y.IsEmpty);
            foreach (FaceArray faceArray in faceArrays)
            {
                foreach (Face face in faceArray)
                {
                    if (face.Area > GeometryLib.EPSILON_AREA)
                    {
                        Faces.Add(face);
                    }
                }
            }
            return Faces;
        }
        public static List<Mesh> GetMeshsFromInstanceElement(Element element, Options options, bool isActual)
        {
            List<GeometryObject> geometryObjects = GetGeometryObjectsFromInstanceElement(element, options, isActual);
            Meshes = geometryObjects.OfType<Mesh>().ToList();
            IEnumerable<FaceArray> faceArrays = geometryObjects.OfType<Solid>().Where(s => s.Volume > GeometryLib.EPSILON_VOLUME).Select(x => x.Faces).Where(y => !y.IsEmpty);
            foreach (FaceArray faceArray in faceArrays)
            {
                foreach (Face face in faceArray)
                {
                    if (face.Area > GeometryLib.EPSILON_AREA)
                    {
                        Meshes.Add(face.Triangulate(1));
                    }
                }
            }
            return Meshes;
        }
        public static List<Surface> GetSurfacesFromInstanceElement(Element element, Options options, bool isActual)
        {
            List<GeometryObject> geometryObjects = GetGeometryObjectsFromInstanceElement(element, options, isActual);
            List<Face> faces = geometryObjects.OfType<Face>().ToList();
            IEnumerable<FaceArray> faceArrays = geometryObjects.OfType<Solid>().Where(s => s.Volume > GeometryLib.EPSILON_VOLUME).Select(x => x.Faces).Where(y => !y.IsEmpty);
            foreach (FaceArray faceArray in faceArrays)
            {
                foreach (Face face in faceArray)
                {
                    if (face.Area > GeometryLib.EPSILON_AREA)
                    {
                        faces.Add(face);
                    }
                }
            }
            foreach (Face face in faces)
            {
                Surfaces.Add(face.GetSurface());
            }
            return Surfaces;
        }
        public static List<Face> GetFacesFromSolid(Solid solid)
        {
            List<Face> faces = new List<Face>();
            if (solid.Volume > GeometryLib.EPSILON_VOLUME)
            {
                foreach (Face face in solid.Faces)
                {
                    if (face.Area > GeometryLib.EPSILON_AREA)
                    {
                        faces.Add(face);
                    }
                }
            }
            return faces;
        }
        public static Solid ToUnionSolid(this IEnumerable<Solid> solids)
        {
            List<Solid> solidClone = solids.ToList();
            if (solidClone.Count == 0)
            {
                return null;
            }
            if (solidClone.Count == 1)
            {
                return solids.First();
            }
            else
            {
                Solid solid = solidClone[0];
                for (int i = 1; i < solidClone.Count; i++)
                {
                    try
                    {
                        solid = BooleanOperationsUtils.ExecuteBooleanOperation(solid, solidClone[i], BooleanOperationsType.Union);
                    }
                    catch
                    {
                        return null;
                    }
                }
                return solid;
            }
        }
        public static List<Solid> ToUnionSolids(this IEnumerable<Solid> solids)
        {
            List<Solid> solidResult = new List<Solid>();
            IList<Solid> solidsLoop = solids.ToList();
            while (solidsLoop.Count > 0)
            {
                Solid solidUnion = null;
                List<Solid> solidsRemain = new List<Solid>();

                ProgressBarInstance progressBarInstance = null;
                if (solidsLoop.Count > 25)
                {
                    progressBarInstance = new ProgressBarInstance("Unioning all the solid.", solidsLoop.Count);
                }

                foreach (Solid solid in solidsLoop)
                {
                    if (solidsLoop.Count > 25)
                    {
                        progressBarInstance.Start();
                    }

                    if (solidUnion == null)
                    {
                        solidUnion = solid;
                    }
                    else
                    {
                        try
                        {
                            solidUnion = BooleanOperationsUtils.ExecuteBooleanOperation(solidUnion, solid, BooleanOperationsType.Union);
                        }
                        catch
                        {
                            solidsRemain.Add(solid);
                        }
                    }
                }
                solidResult.Add(SolidUtils.Clone(solidUnion));
                solidsLoop = solidsRemain;
            }
            return solidResult;
        }
        public static List<PlanarFace> GetBottomPlanarFacesFromInstanceElement(Element element, Options options, bool isActual, double epsilonAreaFeet = GeometryLib.EPSILON_AREA, double epsilonAngleRadian = GeometryLib.EPSILON)
        {
            Solid solidUnion = GetSolidsFromInstanceElement(element, options, isActual).ToUnionSolid();
            if (solidUnion != null)
            {
                List<Solid> solidsSplit = SolidUtils.SplitVolumes(solidUnion).ToList();
                List<PlanarFace> planarFaces = new List<PlanarFace>();
                foreach (Solid solid in solidsSplit)
                {
                    List<Face> faces = GetFacesFromSolid(solid);
                    foreach (Face face in faces)
                    {
                        if (face is PlanarFace planarFace)
                        {
                            if (Math.Abs((planarFace.FaceNormal.Z + 1)) < epsilonAngleRadian)
                            {
                                if (planarFace.Area > epsilonAreaFeet)
                                {
                                    planarFaces.Add(planarFace);
                                }
                            }
                        }
                    }
                }
                return planarFaces.Count > 0 ? planarFaces : null;
            }
            return null;
        }
        public static List<PlanarFace> GetBottomPlanarFacesFromSolid(Solid solid, double epsilonAreaFeet = GeometryLib.EPSILON_AREA, double epsilonAngleRadian = GeometryLib.EPSILON)
        {
            IEnumerable<PlanarFace> planarFaces = GetFacesFromSolid(solid).OfType<PlanarFace>();
            if (planarFaces.Count() > 0)
            {
                List<PlanarFace> planarFacesFilter = new List<PlanarFace>();
                foreach (PlanarFace planarFace in planarFaces)
                {
                    if (Math.Abs((planarFace.FaceNormal.Z + 1)) < epsilonAngleRadian)
                    {
                        if (planarFace.Area > epsilonAreaFeet)
                        {
                            planarFacesFilter.Add(planarFace);
                        }
                    }
                }
                return planarFacesFilter;
            }
            return null;
        }
        public static List<PlanarFace> GetTopPlanarFacesFromInstanceElement(Element element, Options options, bool isActual, double epsilonAreaFeet = GeometryLib.EPSILON_AREA, double epsilonAngleRadian = GeometryLib.EPSILON)
        {
            Solid solidUnion = GetSolidsFromInstanceElement(element, options, isActual).ToUnionSolid();
            if (solidUnion != null)
            {
                List<Solid> solidsSplit = SolidUtils.SplitVolumes(solidUnion).ToList();
                List<PlanarFace> planarFaces = new List<PlanarFace>();
                foreach (Solid solid in solidsSplit)
                {
                    List<Face> faces = GetFacesFromSolid(solid);
                    foreach (Face face in faces)
                    {
                        if (face is PlanarFace planarFace)
                        {
                            if (Math.Abs((planarFace.FaceNormal.Z - 1)) < epsilonAngleRadian)
                            {
                                if (planarFace.Area > epsilonAreaFeet)
                                {
                                    planarFaces.Add(planarFace);
                                }
                            }
                        }
                    }
                }
                return planarFaces.Count > 0 ? planarFaces : null;
            }
            return null;
        }
        public static List<PlanarFace> GetTopPlanarFacesFromSolid(Solid solid, double epsilonAreaFeet = GeometryLib.EPSILON_AREA, double epsilonAngleRadian = GeometryLib.EPSILON)
        {
            IEnumerable<PlanarFace> planarFaces = GetFacesFromSolid(solid).OfType<PlanarFace>();
            if (planarFaces.Count() > 0)
            {
                List<PlanarFace> planarFacesFilter = new List<PlanarFace>();
                foreach (PlanarFace planarFace in planarFaces)
                {
                    if (Math.Abs((planarFace.FaceNormal.Z - 1)) < epsilonAngleRadian)
                    {
                        if (planarFace.Area > epsilonAreaFeet)
                        {
                            planarFacesFilter.Add(planarFace);
                        }
                    }
                }
                return planarFacesFilter;
            }
            return null;
        }
        public static Outline GetOutlineFromSolidInProject(Solid solid)
        {
            List<Curve> curves = new List<Curve>();
            foreach (Edge edge in solid.Edges)
            {
                curves.Add(edge.AsCurve());
            }
            List<XYZ> points = curves.Select(x => x.Tessellate()).SelectMany(y => y).ToList();
            XYZ minPoint = new XYZ(points.Select(x => x.X).Min(), points.Select(y => y.Y).Min(), points.Select(z => z.Z).Min());
            XYZ maxPoint = new XYZ(points.Select(x => x.X).Max(), points.Select(y => y.Y).Max(), points.Select(z => z.Z).Max());
            return new Outline(minPoint, maxPoint);
        }
        public static Solid GetSolidFromMinMaxPoints(XYZ minPoint, XYZ maxPoint)
        {
            if (minPoint.X != maxPoint.X && minPoint.Y != maxPoint.Y && minPoint.Z != maxPoint.Z)
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
                return GeometryCreationUtilities.CreateExtrusionGeometry(curveLoops, XYZ.BasisZ, maxPoint.Z - minPoint.Z);
            }
            return null;
        }
        public static Solid CreateSphereSolid(XYZ point, double radiusFeet)
        {
            Arc arc = Arc.Create(point + radiusFeet * XYZ.BasisZ, point - radiusFeet * XYZ.BasisZ, point + radiusFeet * XYZ.BasisX);
            Line line = Line.CreateBound(arc.GetEndPoint(1), arc.GetEndPoint(0));
            CurveLoop curveLoop = CurveLoop.Create(new List<Curve>() { arc, line });
            Frame frame = new Frame(point, XYZ.BasisX, XYZ.BasisY, XYZ.BasisZ);
            return GeometryCreationUtilities.CreateRevolvedGeometry(frame, new List<CurveLoop>() { curveLoop }, 0, 2 * Math.PI);
        }
        public static Solid CreateCubeSolid(XYZ point, double lengthEdgeFeet)
        {
            XYZ pointCenterBottom = point - lengthEdgeFeet / 2 * XYZ.BasisZ;
            XYZ pointOne = GeometryLib.TranslatingPoint(pointCenterBottom, XYZ.BasisX + XYZ.BasisY, Math.Sqrt(2) * lengthEdgeFeet / 2);
            XYZ pointThree = GeometryLib.TranslatingPoint(pointCenterBottom, -XYZ.BasisX - XYZ.BasisY, Math.Sqrt(2) * lengthEdgeFeet / 2);
            XYZ pointTwo = GeometryLib.TranslatingPoint(pointOne, -XYZ.BasisX, lengthEdgeFeet);
            XYZ pointFour = GeometryLib.TranslatingPoint(pointThree, XYZ.BasisX, lengthEdgeFeet);
            CurveLoop curveLoop = CurveLoop.Create(new List<Curve>() { Line.CreateBound(pointOne, pointTwo), Line.CreateBound(pointTwo, pointThree), Line.CreateBound(pointThree, pointFour), Line.CreateBound(pointFour, pointOne) });
            return GeometryCreationUtilities.CreateExtrusionGeometry(new List<CurveLoop>() { curveLoop }, XYZ.BasisZ, lengthEdgeFeet);
        }
        public static Solid CreateCylinderSolid(Plane plane, double radiusFeet, double heightFeet)
        {
            Arc arcOne = Arc.Create(plane, radiusFeet, 0, Math.PI);
            Arc arcTwo = Arc.Create(plane, radiusFeet, Math.PI, 2 * Math.PI);
            return GeometryCreationUtilities.CreateExtrusionGeometry(new List<CurveLoop>() { CurveLoop.Create(new List<Curve>() { arcOne, arcTwo }) }, plane.Normal, heightFeet);
        }
        public static Solid CreateSolidFromCylindricalFace(CylindricalFace cylindricalFace, double thicknessFeet)
        {
            List<XYZ> pointsBoudaryOfCylindricalFace = cylindricalFace.GetEdgesAsCurveLoops().SelectMany(cl => cl).Select(c => c.Tessellate()).SelectMany(p => p).ToList();
            Plane planeWithOrigin = Plane.CreateByNormalAndOrigin(cylindricalFace.Axis, cylindricalFace.Origin);
            List<List<XYZ>> pointsDivide = GeometryLib.DividePointsByPlane(planeWithOrigin, pointsBoudaryOfCylindricalFace);

            List<XYZ> pointsDitermineLengthCylinder = new List<XYZ>();
            List<PointAndDistance> pointAndDistances = new List<PointAndDistance>();

            if (pointsDivide[0].Count == 0)
            {
                pointsDivide[1].ForEach(x => pointAndDistances.Add(new PointAndDistance() { Point = x, Distance = GeometryLib.DistanceFromPointToPlane(x, planeWithOrigin) }));
                pointsDitermineLengthCylinder.AddRange(new List<XYZ>() { pointAndDistances.MaxBy(x => x.Distance).Point, pointAndDistances.MinBy(x => x.Distance).Point });
            }
            else if (pointsDivide[1].Count == 0)
            {
                pointsDivide[0].ForEach(x => pointAndDistances.Add(new PointAndDistance() { Point = x, Distance = GeometryLib.DistanceFromPointToPlane(x, planeWithOrigin) }));
                pointsDitermineLengthCylinder.AddRange(new List<XYZ>() { pointAndDistances.MaxBy(x => x.Distance).Point, pointAndDistances.MinBy(x => x.Distance).Point });
            }
            else
            {
                pointsDivide[0].ForEach(x => pointAndDistances.Add(new PointAndDistance() { Point = x, Distance = GeometryLib.DistanceFromPointToPlane(x, planeWithOrigin) }));
                pointsDitermineLengthCylinder.Add(pointAndDistances.MaxBy(x => x.Distance).Point);
                pointAndDistances.Clear();
                pointsDivide[1].ForEach(x => pointAndDistances.Add(new PointAndDistance() { Point = x, Distance = GeometryLib.DistanceFromPointToPlane(x, planeWithOrigin) }));
                pointsDitermineLengthCylinder.Add(pointAndDistances.MaxBy(x => x.Distance).Point);
            }

            planeWithOrigin = Plane.CreateByNormalAndOrigin(cylindricalFace.Axis, pointsDitermineLengthCylinder[0]);
            List<XYZ> pointsBoudaryOfCylindricalFaceOnPlane = new List<XYZ>();
            foreach (XYZ point in pointsBoudaryOfCylindricalFace)
            {
                XYZ pointOnPlane = GeometryLib.IntersectLineAndPlane(Line.CreateUnbound(point, planeWithOrigin.Normal), planeWithOrigin);
                if (null != pointOnPlane)
                {
                    pointsBoudaryOfCylindricalFaceOnPlane.Add(pointOnPlane);
                }
            }

            Line bowstring = GeometryLib.GetLinesFromPoints(pointsBoudaryOfCylindricalFaceOnPlane).MaxBy(l => l.Length);
            XYZ origin = GeometryLib.IntersectLineAndPlane(Line.CreateUnbound(cylindricalFace.Origin, planeWithOrigin.Normal), planeWithOrigin);
            double radius = GeometryLib.DistanceBetweenTwoPoints(bowstring.GetEndPoint(0), origin);
            XYZ pointThree = GeometryLib.IntersectLineAndPlane(Line.CreateUnbound(cylindricalFace.GetEdgesAsCurveLoops().SelectMany(cl => cl).Where(x => !(x is Line)).First().Evaluate(0.5, true), planeWithOrigin.Normal), planeWithOrigin);

            Arc arc = Arc.Create(bowstring.GetEndPoint(0), bowstring.GetEndPoint(1), pointThree);
            Arc arcOffsetOne = arc.CreateOffset(thicknessFeet, planeWithOrigin.Normal) as Arc;
            Arc arcOffsetTwo = arc.CreateOffset(-thicknessFeet, planeWithOrigin.Normal) as Arc;

            BoundingBoxUV boundingBoxUV = cylindricalFace.GetBoundingBox();
            UV pointUV = new UV((boundingBoxUV.Min.U + boundingBoxUV.Max.U) / 2, (boundingBoxUV.Min.V + boundingBoxUV.Max.V) / 2);
            XYZ translatePoint = GeometryLib.TranslatingPoint(cylindricalFace.Evaluate(pointUV), cylindricalFace.ComputeNormal(pointUV), 50.0.ToFeet());
            double distanceCheck = GeometryLib.IntersectLineAndPlane(Line.CreateUnbound(translatePoint, planeWithOrigin.Normal), planeWithOrigin).DistanceTo(origin);

            Arc chooseArc = radius > distanceCheck ? (arcOffsetOne.Length > arcOffsetTwo.Length ? arcOffsetTwo : arcOffsetOne) : (arcOffsetOne.Length > arcOffsetTwo.Length ? arcOffsetOne : arcOffsetTwo);

            CurveLoop curveLoop = CurveLoop.Create(new List<Curve>() { arc, Line.CreateBound(arc.GetEndPoint(1), chooseArc.GetEndPoint(1)), chooseArc.CreateReversed(), Line.CreateBound(chooseArc.GetEndPoint(0), arc.GetEndPoint(0)) });
            XYZ pointSecondOnPlane = GeometryLib.IntersectLineAndPlane(Line.CreateUnbound(pointsDitermineLengthCylinder[1], planeWithOrigin.Normal), planeWithOrigin);
            planeWithOrigin.Dispose();
            Line lineCylider = Line.CreateBound(pointSecondOnPlane, pointsDitermineLengthCylinder[1]);
            return GeometryCreationUtilities.CreateExtrusionGeometry(new List<CurveLoop>() { curveLoop }, lineCylider.Direction, Math.Round(lineCylider.Length, GeometryLib.N));
        }
        public static Solid GetOriginSolidFraming(FamilyInstance framing, Options options)
        {
            if (framing.Location is LocationCurve locationCurve)
            {
                Curve framingLocation = locationCurve.Curve;
                Solid solid = GetSolidsFromOriginalFamilyInstance(framing, options).Where(x => x.Volume > GeometryLib.EPSILON_VOLUME).ToList().ToUnionSolid();
                if (null == solid) return null;
                IEnumerable<PlanarFace> planarFaces = GetFacesFromSolid(solid).OfType<PlanarFace>();
                if (planarFaces.Count() > 0)
                {
                    foreach (PlanarFace planarFace in planarFaces)
                    {
                        if (GeometryLib.AngleBetweenTwoVectors(planarFace.FaceNormal, framingLocation.ComputeDerivatives(0, true).BasisX, false) < GeometryLib.DEGREE_ANGLE_MIN)
                        {
                            planarFace.Intersect(framingLocation, out IntersectionResultArray intersectionResultArray);
                            if (null != intersectionResultArray && !intersectionResultArray.IsEmpty)
                            {
                                return GeometryCreationUtilities.CreateSweptGeometry(CurveLoop.Create(new List<Curve>() { framingLocation }), 0, intersectionResultArray.get_Item(0).Parameter, planarFace.GetEdgesAsCurveLoops());
                            }
                            intersectionResultArray?.Dispose();
                        }
                    }
                    foreach (PlanarFace planarFace in planarFaces)
                    {
                        if (GeometryLib.AngleBetweenTwoVectors(planarFace.FaceNormal, framingLocation.ComputeDerivatives(1, true).BasisX, false) < GeometryLib.DEGREE_ANGLE_MIN)
                        {
                            planarFace.Intersect(framingLocation, out IntersectionResultArray intersectionResultArray);
                            if (null != intersectionResultArray && !intersectionResultArray.IsEmpty)
                            {
                                return GeometryCreationUtilities.CreateSweptGeometry(CurveLoop.Create(new List<Curve>() { framingLocation }), 0, intersectionResultArray.get_Item(0).Parameter, planarFace.GetEdgesAsCurveLoops());
                            }
                            intersectionResultArray?.Dispose();
                        }
                    }
                    return GetSolidsFromOriginalFamilyInstance(framing, options).ToUnionSolid();
                }
            }
            return null;
        }
        public static Curve GetCenterCurveFraming(FamilyInstance framing, Options options)
        {
            if (framing.Location is LocationCurve locationCurve)
            {
                List<Line> widthHeight = InforElementUtils.GetWidthHeightFraming(framing, options);
                if (null == widthHeight) return null;
                Curve framingLocation = locationCurve.Curve;
                if (framing.get_Parameter(BuiltInParameter.YZ_JUSTIFICATION).AsValueString() == "Uniform" && null != widthHeight && framing.get_Parameter(BuiltInParameter.STRUCTURAL_BEND_DIR_ANGLE).AsDouble() < GeometryLib.EPSILON)
                {
                    PlanarFace topPlanarFace = GetTopPlanarFacesFromInstanceElement(framing, options, true, GeometryLib.EPSILON_AREA).MaxBy(x => x.Origin.Z);
                    Parameter yJustification = framing.get_Parameter(BuiltInParameter.Y_JUSTIFICATION);
                    Parameter yOffsetValue = framing.get_Parameter(BuiltInParameter.Y_OFFSET_VALUE);
                    if (yJustification.AsValueString() == "Center" || yJustification.AsValueString() == "Origin")
                    {
                        Curve offsetCurve = framingLocation.CreateOffset(-yOffsetValue.AsDouble(), topPlanarFace.FaceNormal);
                        return AdjustCurveAllowZ(framing, widthHeight, offsetCurve);
                    }
                    else
                    {
                        if (null != topPlanarFace)
                        {
                            if (yJustification.AsValueString() == "Left")
                            {
                                Curve offsetCurve = framingLocation.CreateOffset(widthHeight[0].Length / 2 - yOffsetValue.AsDouble(), topPlanarFace.FaceNormal);
                                return AdjustCurveAllowZ(framing, widthHeight, offsetCurve);
                            }
                            else
                            {
                                Curve offsetCurve = framingLocation.CreateOffset(-widthHeight[0].Length / 2 - yOffsetValue.AsDouble(), topPlanarFace.FaceNormal);
                                return AdjustCurveAllowZ(framing, widthHeight, offsetCurve);
                            }
                        }
                    }
                }
                return framingLocation;
            }
            return null;
        }
        private static Curve AdjustCurveAllowZ(FamilyInstance framing, List<Line> widthHeight, Curve framingLocation)
        {
            if (!framing.Symbol.Family.IsInPlace && null != framingLocation)
            {
                Parameter zJustification = framing.get_Parameter(BuiltInParameter.Z_JUSTIFICATION);
                Parameter zOffsetValue = framing.get_Parameter(BuiltInParameter.Z_OFFSET_VALUE);
                Line line = widthHeight[1].GetEndPoint(0).Z < widthHeight[1].GetEndPoint(1).Z ? widthHeight[1].CreateReversed() as Line : widthHeight[1];

                if (zJustification.AsValueString() == "Origin" || zJustification.AsValueString() == "Center")
                {
                    return framingLocation;
                }
                if (zJustification.AsValueString() == "Top")
                {
                    Transform transform = Transform.CreateTranslation(line.Direction * (line.Length / 2 - zOffsetValue.AsDouble()));
                    return framingLocation.CreateTransformed(transform);
                }
                else
                {
                    Transform transform = Transform.CreateTranslation(-line.Direction * (line.Length / 2 + zOffsetValue.AsDouble()));
                    return framingLocation.CreateTransformed(transform);
                }
            }
            return null;
        }
        public static Solid ScaleSolid(Solid solid, double scale)
        {
            Transform transform = Transform.Identity.ScaleBasisAndOrigin(scale);
            Solid solidAfterTransform = SolidUtils.CreateTransformed(solid, transform);
            return SolidUtils.CreateTransformed(solidAfterTransform, Transform.CreateTranslation(solidAfterTransform.ComputeCentroid() - solid.ComputeCentroid()).Inverse);
        }
        public static BoundingBoxXYZ GetBoundingBoxXYZFromSolid(Solid solid)
        {
            if (null == solid && solid.Volume < GeometryLib.EPSILON_VOLUME) return null;
            BoundingBoxXYZ boundingBoxXYZ = solid.GetBoundingBox();
            Transform transform = boundingBoxXYZ.Transform;
            return new BoundingBoxXYZ() { Max = transform.Origin + boundingBoxXYZ.Max, Min = transform.Origin + boundingBoxXYZ.Min };
        }
        public static Solid CreateSolidFromPlanarFace(Document document, Solid elementSolid, PlanarFace planarFace, double thicknessFeet)
        {
            try
            {
                return GeometryCreationUtilities.CreateExtrusionGeometry(planarFace.GetEdgesAsCurveLoops().Where(x => x.NumberOfCurveOfCurveLoop() > 1).ToList(), planarFace.FaceNormal, thicknessFeet);
            }
            catch
            {
                try
                {
                    return GeometryCreationUtilities.CreateExtrusionGeometry(planarFace.GetEdgesAsCurveLoops().Where(x => x.NumberOfCurveOfCurveLoop() > 1).Select(x => x.ToAdjustCurveLoop(document)).ToList(), planarFace.FaceNormal, thicknessFeet);
                }
                catch
                {
                    try
                    {
                        return BooleanOperationsUtils.CutWithHalfSpace(SolidUtils.CreateTransformed(elementSolid, Transform.CreateTranslation(planarFace.FaceNormal * thicknessFeet)), Plane.CreateByNormalAndOrigin(planarFace.FaceNormal, planarFace.Origin));
                    }
                    catch
                    {
                        return null;
                    }
                }
            }
        }
        #endregion

    }
}
