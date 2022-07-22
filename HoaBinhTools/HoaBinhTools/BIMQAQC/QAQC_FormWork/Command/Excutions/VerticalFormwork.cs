using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autodesk.Revit.DB;
using BeyCons.Core.Extensions;
using BeyCons.Core.Libraries.Geometries;
using BeyCons.Core.RevitUtils;
using BeyCons.Core.RevitUtils.DataUtils;
using HoaBinhTools.BIMQAQC.QAQC_FormWork.Models;
using HoaBinhTools.BIMQAQC.QAQC_FormWork.ViewModels;

namespace HoaBinhTools.BIMQAQC.QAQC_FormWork.Command.Excutions
{
	public class VerticalFormwork : FormworkUtils
    {
        public static VerticalFaceFilter GetFaceFilterVerticalFromSolids(Element verticalElement, GeometryExcution geometryExcution, List<Solid> solids, IList<Element> intersectElements, ParaUtil paraUtil, ref int numberOfFaceError)
        {
            VerticalFaceFilter verticalFaceFilter = new VerticalFaceFilter() 
                { 
                    CommonFaceFilter = new CommonFaceFilter(),
                    FillFaces = new List<Face>()
                };

            CommonFaceFilter commonFaceFilter;
            if (solids.Count > 1)
            {
                if (null != solids[0])
                {
                    commonFaceFilter = ClassifyFaceVerticalElement(verticalElement, geometryExcution, solids[0], intersectElements, paraUtil, ref numberOfFaceError);
                    if (null != commonFaceFilter)
                    {
                        verticalFaceFilter.CommonFaceFilter.TopFaces = commonFaceFilter.TopFaces;
                        verticalFaceFilter.CommonFaceFilter.SideFaces = commonFaceFilter.SideFaces;
                        verticalFaceFilter.CommonFaceFilter.BottomFaces = commonFaceFilter.BottomFaces;
                    }
                }

                if (null != solids[1])
                {
                    List<Face> faces = GetFacesFromCommonFaceFilter(ClassifyFaceVerticalElement(verticalElement, geometryExcution, solids[1], intersectElements, paraUtil, ref numberOfFaceError));
                    if (faces.Count > 0)
                    {
                        verticalFaceFilter.FillFaces = faces;
                    }
                }

                try
                {
                    if (null != solids[2])
                    {
                        List<Face> faces = GetFacesFromCommonFaceFilter(ClassifyFaceVerticalElement(verticalElement, geometryExcution, solids[1], intersectElements, paraUtil, ref numberOfFaceError));
                        if (faces.Count > 0)
                        {
                            verticalFaceFilter.FillFaces = faces;
                        }
                    }
                }
                catch { }
            }
            else
            {
                if (null != solids[0])
                {
                    commonFaceFilter = ClassifyFaceVerticalElement(verticalElement, geometryExcution, solids[0], intersectElements, paraUtil, ref numberOfFaceError);
                    if (null != commonFaceFilter)
                    {
                        verticalFaceFilter.CommonFaceFilter.TopFaces = commonFaceFilter.TopFaces;
                        verticalFaceFilter.CommonFaceFilter.SideFaces = commonFaceFilter.SideFaces;
                        verticalFaceFilter.CommonFaceFilter.BottomFaces = commonFaceFilter.BottomFaces;
                    }
                }
            }
            return verticalFaceFilter;
        }
        public static CommonFaceFilter GetFaceFilterCommonFromVerticalElementSolid(Element verticalElement, GeometryExcution geometryExcution, Solid verticalSolid, ParaUtil paraUtil)
        {
            FaceType facesFormworkVerticalElement = geometryExcution.GetAllFaceFormworkElement(verticalElement, verticalSolid);
            CommonFaceFilter commonFaceFilter = GetFaceFilterFromFaces(facesFormworkVerticalElement, paraUtil);
            //FilterTopFaces(ref commonFaceFilter, verticalElement, paraUtil);
            FilterBottomFaces(ref commonFaceFilter, verticalElement, paraUtil);
            return commonFaceFilter;
        }
        public static CommonFaceFilter ClassifyFaceVerticalElement(Element verticalElement, GeometryExcution geometryExcute, Solid verticalSolid, IList<Element> intersectElements, ParaUtil paraUtil, ref int numberOfFaceError)
        {
            FaceType facesFormworkColumn = geometryExcute.GetAllFaceFormworkElement(verticalElement, verticalSolid, intersectElements, ref numberOfFaceError);
            CommonFaceFilter commonFaceFilter = GetFaceFilterFromFaces(facesFormworkColumn, paraUtil);
            //FilterTopFaces(ref commonFaceFilter, verticalElement, paraUtil);
            FilterBottomFaces(ref commonFaceFilter, verticalElement, paraUtil);
            return commonFaceFilter;
        }
        public static PlanarFace GetPlanarFaceBottomOfHeadVerticalElement(Level topLevel, IList<Element> elementsIntersect)
        {
            List<PlanarFace> filterPlanarFaces = new List<PlanarFace>();
            if (elementsIntersect.Count > 0)
            {
                foreach (Element elementIntersect in elementsIntersect)
                {
                    if (elementIntersect is Floor floor && (floor.Document.GetElement(floor.LevelId) as Level).Name == topLevel.Name)
                    {
                        List<PlanarFace> planarFaces = GeometryUtils.GetBottomPlanarFacesFromInstanceElement(elementIntersect, FormworkViewModels.Options, true, FormworkViewModels.EpsilonArea);
                        if (null != planarFaces)
                        {
                            filterPlanarFaces.Add(planarFaces.MinBy(l => l.Origin.Z));
                        }
                    }
                    else if (elementIntersect.Category.Name == FormworkViewModels.Categories[2].Name)
                    {
                        if (elementIntersect.Document.GetElement(elementIntersect.get_Parameter(BuiltInParameter.INSTANCE_REFERENCE_LEVEL_PARAM).AsElementId()) is Level referenceLevel && referenceLevel.Name == topLevel.Name)
                        {
                            List<PlanarFace> planarFaces = GeometryUtils.GetBottomPlanarFacesFromInstanceElement(elementIntersect, FormworkViewModels.Options, true, FormworkViewModels.EpsilonArea);
                            if (null != planarFaces)
                            {
                                filterPlanarFaces.Add(planarFaces.MinBy(l => l.Origin.Z));
                            }
                        }
                    }
                }
            }
            return filterPlanarFaces.Count > 0 ? filterPlanarFaces.MinBy(x => x.Origin.Z) : null;
        }
        public static PlanarFace GetBottomPlanarFaceVerticalElement(Solid verticalSolid)
        {
            if (null != verticalSolid)
            {
                List<PlanarFace> basePlanarFaces = GeometryUtils.GetBottomPlanarFacesFromSolid(verticalSolid, FormworkViewModels.EpsilonArea);
                PlanarFace basePlanarFace = null != basePlanarFaces ? (basePlanarFaces.Count > 0 ? basePlanarFaces.OrderBy(x => x.Origin.Z).LastOrDefault() : null) : null;
                if (null != basePlanarFace)
                {
                    try
                    {
                        Solid splitSolid = BooleanOperationsUtils.CutWithHalfSpace(verticalSolid, Plane.CreateByNormalAndOrigin(XYZ.BasisZ, basePlanarFace.Origin));
                        List<PlanarFace> planarFaces = GeometryUtils.GetBottomPlanarFacesFromSolid(splitSolid, FormworkViewModels.EpsilonArea);
                        return null != planarFaces ? (planarFaces.Count > 0 ? planarFaces.OrderBy(x => x.Origin.Z).FirstOrDefault() : null) : null;
                    }
                    catch { }
                }
            }
            return null;
        }
        //Tìm mặt trên thấp nhất.
        public static PlanarFace GetPlanarFaceTopOfHeadVerticalElement(Level topLevel, IList<Element> elementsIntersect)
        {
            List<PlanarFace> filterPlanarFaces = new List<PlanarFace>();
            if (elementsIntersect.Count > 0)
            {
                foreach (Element elementIntersect in elementsIntersect)
                {
                    if (elementIntersect is Floor floor 
                        && (floor.Document.GetElement(floor.LevelId) as Level).Name == topLevel.Name)
                    {
                        List<PlanarFace> planarFaces = 
                            GeometryUtils.GetTopPlanarFacesFromInstanceElement(elementIntersect, FormworkViewModels.Options, true, FormworkViewModels.EpsilonArea);
                        
                        if (null != planarFaces)
                        {
                            filterPlanarFaces.Add(planarFaces.MinBy(l => l.Origin.Z));
                        }
                    }
                    //else if (elementIntersect.Category.Name == FormworkViewModels.Categories[2].Name)
                    //{
                    //    if (elementIntersect.Document.GetElement(elementIntersect.get_Parameter(BuiltInParameter.INSTANCE_REFERENCE_LEVEL_PARAM).AsElementId()) is Level referenceLevel 
                    //        && referenceLevel.Name == topLevel.Name)
                    //    {
                    //        List<PlanarFace> planarFaces = GeometryUtils.GetTopPlanarFacesFromInstanceElement(elementIntersect, FormworkViewModels.Options, true, FormworkViewModels.EpsilonArea);
                    //        if (null != planarFaces)
                    //        {
                    //            filterPlanarFaces.Add(planarFaces.MinBy(l => l.Origin.Z));
                    //        }
                    //    }
                    //}
                }
            }
            return filterPlanarFaces.Count > 0 ? filterPlanarFaces.MinBy(x => x.Origin.Z) : null;
        }
        public static List<Curve> GetBoundaryVerticalElement(PlanarFace basePlanarFace, Solid verticalSolid)
        {
            List<Curve> curves = new List<Curve>();
            //Outsize Lấy đoạn cp
            //Inside Lấy đoạn bị giao

            SolidCurveIntersectionOptions solidCurveIntersectionOptions = new SolidCurveIntersectionOptions() 
            {
                ResultType = SolidCurveIntersectionMode.CurveSegmentsOutside
            };

            foreach (Curve curve in basePlanarFace.GetEdgesAsCurveLoops().MaxBy(x => x.GetExactLength()))
            {
                try
                {
                    SolidCurveIntersection solidCurveIntersection = verticalSolid.IntersectWithCurve(curve, solidCurveIntersectionOptions);
                    if (solidCurveIntersection.SegmentCount > 0)
                    {
                        for (int i = 0; i < solidCurveIntersection.SegmentCount; i++)
                        {
                            if (solidCurveIntersection.GetCurveSegment(i).Length > RevitData.Instance.Application.ShortCurveTolerance)
                            {
                                curves.Add(solidCurveIntersection.GetCurveSegment(i));
                            }
                        }
                    }
                }
                catch
                {
                    curves.Add(curve);
                }
            }
            solidCurveIntersectionOptions?.Dispose();
            return curves.Count > 0 ? curves : null;
        }
        public static PlanarFace GetBottomPlanarFaceFloors(List<Floor> floors)
        {
            List<PlanarFace> planarFaces = new List<PlanarFace>();
            foreach (Floor floor in floors)
            {
                planarFaces.Add(GeometryUtils.GetBottomPlanarFacesFromInstanceElement(floor, FormworkViewModels.Options, true, FormworkViewModels.EpsilonArea).MinBy(x => x.Origin.Z));
            }
            return planarFaces.Count > 0 ? planarFaces.MinBy(x => x.Origin.Z) : null;
        }
        public static PlanarFace GetPlanarFaceTopFloors(List<Floor> floors)
        {
            List<PlanarFace> planarFaces = new List<PlanarFace>();
            foreach (Floor floor in floors)
            {
                planarFaces.Add(GeometryUtils.GetBottomPlanarFacesFromInstanceElement(floor, FormworkViewModels.Options, true, FormworkViewModels.EpsilonArea).MaxBy(x => x.Origin.Z));
            }
            return planarFaces.Count > 0 ? planarFaces.MaxBy(x => x.Origin.Z) : null;

        }
        public static Solid CreateExtrusionFromCurves(List<Curve> perimeters, double heightFeet)
        {
            List<Solid> solids = new List<Solid>();
            foreach (Curve curve in perimeters)
            {
                try
                {
                    CurveLoop curveLoop = CurveLoop.CreateViaThicken(curve, FormworkViewModels.Thickness, XYZ.BasisZ);
                    Solid solid = GeometryCreationUtilities.CreateExtrusionGeometry(new List<CurveLoop>() { curveLoop }, XYZ.BasisZ, heightFeet);
                    solids.Add(solid);
                }
                catch { }
            }
            return solids.Count > 0 ? solids.ToUnionSolid() : null;
        }
        public static bool IsOriginFramingIntersectFloor(FamilyInstance framing, List<Floor> floors)
        {
            Solid framingSolid = GeometryUtils.GetSolidsFromOriginalFamilyInstance(framing, FormworkViewModels.Options).ToUnionSolid();
            if (null != framingSolid)
            {
                foreach (Floor floor in floors)
                {
                    Solid floorSolid = GeometryUtils.GetSolidsFromInstanceElement(floor, FormworkViewModels.Options, true).ToUnionSolid();
                    if (null != floorSolid)
                    {
                        try
                        {
                            Solid intersectSolid = BooleanOperationsUtils.ExecuteBooleanOperation(framingSolid, floorSolid, BooleanOperationsType.Intersect);
                            if (intersectSolid.Volume > GeometryLib.EPSILON_VOLUME)
                            {
                                return true;
                            }
                        }
                        catch { }
                    }
                }
            }
            return false;
        }
        public static bool IsFramingIntersectFloor(Solid framingSolid, List<Floor> floors)
        {
            foreach (Floor floor in floors)
            {
                Solid floorSolid = GeometryUtils.GetSolidsFromInstanceElement(floor, FormworkViewModels.Options, true).ToUnionSolid();
                if (IntersectUtils.DoesIntersect(floorSolid, framingSolid))
                {
                    return true;
                }
            }
            return false;
        }
        public static bool CheckSubtractFormulaFloors(Element verticalElement, List<Floor> floors,double minZ,double maxZ)
        {
            //PlanarFace minTopPlanarFaceWall = GeometryUtils.GetTopPlanarFacesFromInstanceElement(verticalElement, FormworkViewModels.Options, true, FormworkViewModels.EpsilonArea).Where(x => !IntersectRay.IsInsidePlanarFace(x, verticalElement, true)).MinBy(x => x.Origin.Z);
            PlanarFace minBottomPlanarFaceFloor = floors.Select(x => GeometryUtils.GetBottomPlanarFacesFromInstanceElement(x, FormworkViewModels.Options, true, FormworkViewModels.EpsilonArea).MinBy(y => y.Origin.Z)).MinBy(z => z.Origin.Z);
            if (maxZ >= minBottomPlanarFaceFloor.Origin.Z && minBottomPlanarFaceFloor.Origin.Z >= minZ)
            {
                return true;
            }

            return false;
        }
        public static bool CheckSubtractFormulaFramings(Element verticalElement, List<FamilyInstance> framings, double minZ, double maxZ)
        {
            //PlanarFace minTopPlanarFaceWall = GeometryUtils.GetTopPlanarFacesFromInstanceElement(verticalElement, FormworkViewModels.Options, true, FormworkViewModels.EpsilonArea).Where(x => !IntersectRay.IsInsidePlanarFace(x, verticalElement, true)).MinBy(x => x.Origin.Z);
            PlanarFace maxBottomPlanarFaceFraming = framings.Select(x => GeometryUtils.GetBottomPlanarFacesFromInstanceElement(x, FormworkViewModels.Options, true, FormworkViewModels.EpsilonArea).MinBy(y => y.Origin.Z)).MaxBy(z => z.Origin.Z);
            if (maxZ >= maxBottomPlanarFaceFraming.Origin.Z && maxBottomPlanarFaceFraming.Origin.Z >= minZ)
            {
                return true;
            }
            return false;
        }
        public static string FomularPerimeters(List<string> perimeters, double height)
        {
            string fomular = string.Empty;

            List<LineOfPerimeter> units = new List<LineOfPerimeter>();
            foreach (string line in perimeters)
            {
                if (units.Where(x => x.lenght == double.Parse(line)).Count() > 0)
                {
                    units.Where(x => x.lenght == double.Parse(line)).FirstOrDefault().count += 1; 
                }
                else
                {
                    units.Add(new LineOfPerimeter()
                    {
                        lenght = double.Parse(line),
                        count =1,
                    });
                }
            }

            foreach (LineOfPerimeter u in units)
            {
                if (u.count > 1)
                {
                    u.fomular = $"{u.count}*{u.lenght}";
                }
                else
                {
                    u.fomular = $"{u.lenght}";
                }
            }

            fomular = $"({string.Join("+", units.Select(x => x.fomular).ToArray())})*{height}";

            return fomular;
        }
    }
}
