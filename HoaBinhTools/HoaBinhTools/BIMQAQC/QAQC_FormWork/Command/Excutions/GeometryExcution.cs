//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;
//using Autodesk.Revit.DB;

//namespace HoaBinhTools.BIMQAQC.QAQC_FormWork.Command.Excutions
//{
//	public class GeometryExcution
//	{
//        #region Field
//        private List<DataReport> dataReports;
//        #endregion

//        #region Properties
//        public List<DataReport> DataReports
//        {
//            get
//            {
//                if (dataReports == null)
//                {
//                    dataReports = new List<DataReport>();
//                }
//                return dataReports;
//            }
//            set
//            {
//                dataReports = value;
//            }
//        }
//        private double Length { get; set; } = 5000.0.ToFeet();
//        #endregion

//        #region Method      
//        public FaceType GetAllFaceFormworkElement(Element element, Solid elementSolid, IList<Element> intersectElements, ref int numberOfFaceError)
//        {
//            FaceType facesFormwork = new FaceType() { OriginFaces = new List<Face>(), RegainFaces = new List<Face>() };

//            IEnumerable<Face> originalFaces = GeometryUtils.GetFacesFromSolid(elementSolid).Where(f => f.Area > FormworkControl.EpsilonArea);
//            IEnumerable<Face> originalPlanarAndCylindricalFaces = originalFaces.Where(x => x.GetType() == typeof(PlanarFace) || x.GetType() == typeof(CylindricalFace));
//            IEnumerable<Face> originalOtherFaces = originalFaces.Where(x => x.GetType() != typeof(PlanarFace) && x.GetType() != typeof(CylindricalFace));

//            if (originalOtherFaces.Count() > 0)
//            {
//                List<Face> otherFaces = GetOtherFaces(element, elementSolid, intersectElements);
//                if (null != otherFaces)
//                {
//                    if (otherFaces.Count > 0)
//                    {
//                        facesFormwork.OriginFaces.AddRange(otherFaces);
//                    }
//                    else
//                    {
//                        numberOfFaceError += originalOtherFaces.Count();
//                    }
//                }
//                else
//                {
//                    numberOfFaceError += originalOtherFaces.Count();
//                    facesFormwork.OriginFaces.AddRange(originalOtherFaces);
//                }
//            }

//            if (originalPlanarAndCylindricalFaces.Count() > 0)
//            {
//                Solid solidOpening = GetOpeningSolid(element);
//                foreach (Face planarAndCylindricalFace in originalPlanarAndCylindricalFaces)
//                {
//                    if (planarAndCylindricalFace is PlanarFace planarFace)
//                    {
//                        try
//                        {
//                            Solid solidFormwork = GeometryUtils.CreateSolidFromPlanarFace(element.Document, elementSolid, planarFace, FormworkControl.Thickness);
//                            if (null == solidFormwork)
//                            {
//                                numberOfFaceError++;
//                                facesFormwork.OriginFaces.Add(planarFace);
//                            }
//                            else
//                            {
//                                IList<Element> elementIntersectSolidsFormwork = IntersectElement.GetElemenstIntersectWithSolid(solidFormwork, intersectElements);
//                                if (null != elementIntersectSolidsFormwork)
//                                {
//                                    if (IntersectFormworkSolid(ref solidFormwork, elementIntersectSolidsFormwork))
//                                    {
//                                        numberOfFaceError++;
//                                    }
//                                }
//                                if (solidFormwork.Volume > FormworkControl.EpsilonVolume)
//                                {
//                                    PlanarFace planarFaceOrigin = FormworkUtils.GetPlanarFace(planarFace, GeometryUtils.GetFacesFromSolid(solidFormwork).OfType<PlanarFace>(), FormworkControl.EpsilonArea);
//                                    if (null != planarFaceOrigin)
//                                    {
//                                        if (planarFaceOrigin.Area > FormworkControl.EpsilonArea)
//                                        {
//                                            facesFormwork.RegainFaces.Add(planarFaceOrigin);
//                                        }
//                                    }
//                                }
//                            }
//                        }
//                        catch
//                        {
//                            numberOfFaceError++;
//                            facesFormwork.OriginFaces.Add(planarFace);
//                        }
//                    }
//                    else
//                    {
//                        try
//                        {
//                            Solid solidFormwork = GeometryUtils.CreateSolidFromCylindricalFace(planarAndCylindricalFace as CylindricalFace, FormworkControl.Thickness);
//                            IList<Element> elementIntersectSolidsFormwork = IntersectElement.GetElemenstIntersectWithSolid(solidFormwork, intersectElements);
//                            List<Solid> solidIntersectSolidsFormwork = elementIntersectSolidsFormwork.Select(e => GeometryUtils.GetSolidsFromInstanceElement(e, FormworkControl.Options, true).ToUnionSolids()).SelectMany(x => x).ToList().ToUnionSolids();
//                            if (null != solidIntersectSolidsFormwork)
//                            {
//                                if (IntersectFormworkSolid(ref solidFormwork, elementIntersectSolidsFormwork))
//                                {
//                                    numberOfFaceError++;
//                                    facesFormwork.OriginFaces.Add(planarAndCylindricalFace);
//                                }
//                            }
//                            if (null != solidOpening)
//                            {
//                                solidFormwork = BooleanOperationsUtils.ExecuteBooleanOperation(solidFormwork, solidOpening, BooleanOperationsType.Difference);
//                            }
//                            if (solidFormwork.Volume > FormworkControl.EpsilonVolume)
//                            {
//                                CylindricalFace cylindricalFaceOrigin = FormworkUtils.GetCylindricalFace(GeometryUtils.GetFacesFromSolid(solidFormwork).OfType<CylindricalFace>(), elementSolid, FormworkControl.EpsilonArea);
//                                if (null != cylindricalFaceOrigin)
//                                {
//                                    if (cylindricalFaceOrigin.Area > FormworkControl.EpsilonArea)
//                                    {
//                                        if (cylindricalFaceOrigin.Area < planarAndCylindricalFace.Area + FormworkControl.EpsilonArea)
//                                        {
//                                            facesFormwork.RegainFaces.Add(cylindricalFaceOrigin);
//                                        }
//                                        else
//                                        {
//                                            numberOfFaceError++;
//                                            facesFormwork.OriginFaces.Add(planarAndCylindricalFace);
//                                        }
//                                    }
//                                }
//                                else
//                                {
//                                    numberOfFaceError++;
//                                    facesFormwork.OriginFaces.Add(planarAndCylindricalFace);
//                                }
//                            }
//                        }
//                        catch
//                        {
//                            numberOfFaceError++;
//                            facesFormwork.OriginFaces.Add(planarAndCylindricalFace);
//                        }
//                    }
//                }
//            }
//            return facesFormwork;
//        }
//        public FaceType GetAllFaceFormworkElement(Element element, Solid elementSolid)
//        {
//            Solid solidOpening = GetOpeningSolid(element);
//            if (null != solidOpening)
//            {
//                try
//                {
//                    elementSolid = BooleanOperationsUtils.ExecuteBooleanOperation(elementSolid, solidOpening, BooleanOperationsType.Difference);
//                    return new FaceType() { OriginFaces = GeometryUtils.GetFacesFromSolid(elementSolid).Where(a => a.Area > FormworkControl.EpsilonArea).ToList(), RegainFaces = new List<Face>() };
//                }
//                catch
//                {
//                    DataReports.Add(new DataReport(element, null, "Unknow", $"Formwork area was ignored the opening") { Index = FormworkControl.Index });
//                    return new FaceType() { OriginFaces = GeometryUtils.GetFacesFromSolid(elementSolid).Where(a => a.Area > FormworkControl.EpsilonArea).ToList(), RegainFaces = new List<Face>() };
//                }
//            }
//            else
//            {
//                return new FaceType() { OriginFaces = GeometryUtils.GetFacesFromSolid(elementSolid).Where(a => a.Area > FormworkControl.EpsilonArea).ToList(), RegainFaces = new List<Face>() };
//            }
//        }
//        private bool IntersectFormworkSolid(ref Solid formworkSolid, IList<Element> intersectElements)
//        {
//            bool result = false;
//            foreach (Element intersectElement in intersectElements)
//            {
//                Solid solid = GeometryUtils.GetSolidsFromInstanceElement(intersectElement, FormworkControl.Options, true).ToUnionSolid();
//                if (null != solid)
//                {
//                    try
//                    {
//                        BooleanOperationsUtils.ExecuteBooleanOperationModifyingOriginalSolid(formworkSolid, solid, BooleanOperationsType.Difference);
//                    }
//                    catch
//                    {
//                        try
//                        {
//                            Solid unionSolids = new List<Solid> { solid, formworkSolid }.ToUnionSolid();

//                            if (null != unionSolids)
//                            {
//                                BooleanOperationsUtils.ExecuteBooleanOperationModifyingOriginalSolid(unionSolids, solid, BooleanOperationsType.Difference);

//                                formworkSolid = unionSolids;
//                            }
//                            else
//                            {

//                            }
//                        }
//                        catch (Exception e)
//                        {
//                            result = true;
//                        }
//                    }
//                }
//                else
//                {
//                    result = true;
//                }
//            }
//            return result;
//        }
//        private List<Face> GetOtherFaces(Element element, Solid elementSolid, IList<Element> intersectElements)
//        {
//            List<Solid> unionSolids = UnionSolidsIntersectElement(element, elementSolid, intersectElements);
//            if (null != unionSolids && unionSolids.Count == 1)
//            {
//                try
//                {
//                    List<Face> otherFaces = new List<Face>();
//                    Solid solidUnionAll = BooleanOperationsUtils.ExecuteBooleanOperation(elementSolid, unionSolids[0], BooleanOperationsType.Union);
//                    IEnumerable<Face> otherFacesSolidUnionAll = GeometryUtils.GetFacesFromSolid(solidUnionAll).Where(x => x.GetType() != typeof(PlanarFace) && x.GetType() != typeof(CylindricalFace));
//                    foreach (Face otherFaceSolidUnionAll in otherFacesSolidUnionAll)
//                    {
//                        if (GeometryLib.IsFaceTouchSolid(otherFaceSolidUnionAll, elementSolid))
//                        {
//                            if (otherFaceSolidUnionAll.Area > FormworkControl.EpsilonArea)
//                            {
//                                otherFaces.Add(otherFaceSolidUnionAll);
//                            }
//                        }
//                    }
//                    return otherFaces;
//                }
//                catch { }
//            }
//            return null;
//        }
//        public List<Solid> UnionSolidsIntersectElement(Element element, Solid elementSolid, IList<Element> intersectElements)
//        {
//            List<Solid> solids = new List<Solid>() { elementSolid };
//            Solid solid;
//            foreach (Element intersectElement in intersectElements)
//            {
//                solid = GeometryUtils.GetSolidsFromInstanceElement(intersectElement, FormworkControl.Options, true).ToUnionSolid();
//                if (null != solid)
//                {
//                    solids.Add(solid);
//                }
//                else
//                {
//                    DataReports.Add(new DataReport(element, null, "Unknow", $"Element has more than one solid") { Index = FormworkControl.Index });
//                    return null;
//                }
//            }
//            solids = solids.ToUnionSolids();
//            return solids.Count > 0 ? solids : null;
//        }
//        private Solid GetOpeningSolid(Element element)
//        {
//            if (element.Category.Name == FormworkControl.Categories[0].Name)
//            {
//                return GetOpeningsSolidFromColumn(element);
//            }
//            else if (element.Category.Name == FormworkControl.Categories[1].Name)
//            {
//                return GetOpeningsSolidFromWall(element);
//            }
//            else if (element.Category.Name == FormworkControl.Categories[2].Name)
//            {
//                return GetOpeningsSolidFromFraming(element);
//            }
//            return null;
//        }
//        private Solid GetOpeningsSolidFromColumn(Element column)
//        {
//            List<Opening> structuralOpeningCuts = IntersectElement.GetOpeningColumns(column);
//            if (structuralOpeningCuts.Count > 0)
//            {
//                List<Solid> solids = new List<Solid>();
//                foreach (Opening opening in structuralOpeningCuts)
//                {
//                    List<Type> types = opening.BoundaryCurves.ToCurves().Select(x => x.GetType()).Where(y => y.Name != "Line").ToList();
//                    if (types.Count == 0)
//                    {
//                        CurveLoop curveLoop = opening.BoundaryCurves.ToCurveLoop();
//                        Plane plane = curveLoop.GetPlane();
//                        Transform transform = Transform.CreateTranslation(GeometryLib.TranslatingPoint(plane.Origin, plane.Normal.Negate(), Length / 2).Subtract(plane.Origin));
//                        curveLoop = CurveLoop.CreateViaTransform(curveLoop, transform);
//                        solids.Add(GeometryCreationUtilities.CreateExtrusionGeometry(new List<CurveLoop>() { curveLoop }, plane.Normal, Length));
//                    }
//                }
//                return solids.ToUnionSolid();
//            }
//            return null;
//        }
//        private Solid GetOpeningsSolidFromWall(Element wall)
//        {
//            List<Opening> rectangularArcWallOpenings = IntersectElement.GetOpeningWalls(wall);
//            List<Solid> solidOpeningWalls = new List<Solid>();
//            if (rectangularArcWallOpenings.Count > 0)
//            {
//                foreach (Opening opening in rectangularArcWallOpenings)
//                {
//                    Solid solidOpening = GeometryUtils.GetSolidsFromInstanceElement(opening, new Options() { DetailLevel = ViewDetailLevel.Fine, IncludeNonVisibleObjects = true }, true).ToUnionSolid();
//                    if (null != solidOpening)
//                    {
//                        List<CylindricalFace> cylindricalFaces = GeometryUtils.GetFacesFromSolid(solidOpening).OfType<CylindricalFace>().ToList();
//                        if (cylindricalFaces.Count > 0)
//                        {
//                            foreach (CylindricalFace cylindricalFace in cylindricalFaces)
//                            {
//                                solidOpeningWalls.Add(GeometryUtils.CreateSolidFromCylindricalFace(cylindricalFace, FormworkControl.Thickness));
//                            }
//                        }
//                    }
//                    else
//                    {
//                        DataReports.Add(new DataReport(wall, null, "Unknow", $"Formwork area was ignored the opening") { Index = FormworkControl.Index });
//                    }
//                }
//            }
//            return solidOpeningWalls.ToUnionSolid();
//        }
//        private Solid GetOpeningsSolidFromFraming(Element framing)
//        {
//            List<Opening> structuralOpeningCuts = IntersectElement.GetOpeningFramings(framing);
//            if (structuralOpeningCuts.Count > 0)
//            {
//                List<Solid> solids = new List<Solid>();
//                foreach (Opening opening in structuralOpeningCuts)
//                {
//                    List<Type> types = opening.BoundaryCurves.ToCurves().Select(x => x.GetType()).Where(y => y.Name != "Line").ToList();
//                    if (types.Count == 0)
//                    {
//                        CurveLoop curveLoop = opening.BoundaryCurves.ToCurveLoop();
//                        Plane plane = curveLoop.GetPlane();
//                        Transform transform = Transform.CreateTranslation(GeometryLib.TranslatingPoint(plane.Origin, plane.Normal.Negate(), Length / 2).Subtract(plane.Origin));
//                        curveLoop = CurveLoop.CreateViaTransform(curveLoop, transform);
//                        solids.Add(GeometryCreationUtilities.CreateExtrusionGeometry(new List<CurveLoop>() { curveLoop }, plane.Normal, Length));
//                    }
//                }
//                return solids.ToUnionSolid();
//            }
//            return null;
//        }
//        #endregion
//    }
//}
