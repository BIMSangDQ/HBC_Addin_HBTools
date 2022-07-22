using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autodesk.Revit.DB;
using BeyCons.Core.Libraries.Geometries;
using HoaBinhTools.BIMQAQC.QAQC_FormWork.Models;

namespace HoaBinhTools.BIMQAQC.QAQC_FormWork.Command.Excutions
{
	public class FormworkUtils
    {
        public static CommonFaceFilter GetFaceFilterFromFaces(FaceType faceType, ParaUtil paraUtil)
        {
            CommonFaceFilter commonFaceFilter = new CommonFaceFilter()
            {
                TopFaces = new FaceType() { OriginFaces = new List<Face>(), RegainFaces = new List<Face>() },
                SideFaces = new FaceType() { OriginFaces = new List<Face>(), RegainFaces = new List<Face>() },
                BottomFaces = new FaceType() { OriginFaces = new List<Face>(), RegainFaces = new List<Face>() }
            };

            if (faceType.OriginFaces.Count > 0)
            {
                foreach (Face face in faceType.OriginFaces)
                {
                    XYZ direction = face.ComputeNormal(new UV((face.GetBoundingBox().Min.U + face.GetBoundingBox().Max.U) / 2, (face.GetBoundingBox().Min.V + face.GetBoundingBox().Max.V) / 2));
                    double angleWithDirectionZ = Math.Abs(GeometryLib.AngleBetweenTwoVectors(direction, XYZ.BasisZ, true));
                    if (angleWithDirectionZ <= paraUtil.AngleTopAreaTo)
                    {
                        commonFaceFilter.TopFaces.OriginFaces.Add(face);
                    }

                    if (angleWithDirectionZ >= 180 - paraUtil.AngleBottomAreaTo)
                    {
                        commonFaceFilter.BottomFaces.OriginFaces.Add(face);
                    }

                    if (180 - paraUtil.AngleBottomAreaTo > angleWithDirectionZ && angleWithDirectionZ > paraUtil.AngleTopAreaTo)
                    {
                        commonFaceFilter.SideFaces.OriginFaces.Add(face);
                    }
                }
            }
            if (faceType.RegainFaces.Count > 0)
            {
                foreach (Face face in faceType.RegainFaces)
                {
                    XYZ direction = face.ComputeNormal(new UV((face.GetBoundingBox().Min.U + face.GetBoundingBox().Max.U) / 2, (face.GetBoundingBox().Min.V + face.GetBoundingBox().Max.V) / 2));
                    double angleWithDirectionZ = Math.Abs(GeometryLib.AngleBetweenTwoVectors(-direction, XYZ.BasisZ, true));
                    if (angleWithDirectionZ <= paraUtil.AngleTopAreaTo)
                    {
                        commonFaceFilter.TopFaces.RegainFaces.Add(face);
                    }

                    if (angleWithDirectionZ >= 180 - paraUtil.AngleBottomAreaTo)
                    {
                        commonFaceFilter.BottomFaces.RegainFaces.Add(face);
                    }

                    if (180 - paraUtil.AngleBottomAreaTo > angleWithDirectionZ && angleWithDirectionZ > paraUtil.AngleTopAreaTo)
                    {
                        commonFaceFilter.SideFaces.RegainFaces.Add(face);
                    }
                }
            }
            return commonFaceFilter;
        }
        public static List<Face> GetFacesFromCommonFaceFilter(CommonFaceFilter faceFilterCommon)
        {
            List<Face> faces = new List<Face>();
            if (null != faceFilterCommon)
            {
                if (null != faceFilterCommon.TopFaces)
                {
                    if (faceFilterCommon.TopFaces.OriginFaces.Count > 0)
                    {
                        faceFilterCommon.TopFaces.OriginFaces.ForEach(x => faces.Add(x));
                    }
                    if (faceFilterCommon.TopFaces.RegainFaces.Count > 0)
                    {
                        faceFilterCommon.TopFaces.RegainFaces.ForEach(x => faces.Add(x));
                    }
                }
                if (null != faceFilterCommon.SideFaces)
                {
                    if (faceFilterCommon.SideFaces.OriginFaces.Count > 0)
                    {
                        faceFilterCommon.SideFaces.OriginFaces.ForEach(x => faces.Add(x));
                    }
                    if (faceFilterCommon.SideFaces.RegainFaces.Count > 0)
                    {
                        faceFilterCommon.SideFaces.RegainFaces.ForEach(x => faces.Add(x));
                    }
                }
                if (null != faceFilterCommon.BottomFaces)
                {
                    if (faceFilterCommon.BottomFaces.OriginFaces.Count > 0)
                    {
                        faceFilterCommon.BottomFaces.OriginFaces.ForEach(x => faces.Add(x));
                    }
                    if (faceFilterCommon.BottomFaces.RegainFaces.Count > 0)
                    {
                        faceFilterCommon.BottomFaces.RegainFaces.ForEach(x => faces.Add(x));
                    }
                }
            }
            return faces;
        }
        public static List<Face> GetFacesFromFaceResult(ResultFace faceResult)
        {
            List<Face> faces = GetFacesFromCommonFaceFilter(faceResult.CommonFaceFilter);
            if (null != faceResult.FillFaces && faceResult.FillFaces.Count > 0)
            {
                faces.AddRange(faceResult.FillFaces);
            }
            return faces;
        }
        public static PlanarFace GetPlanarFace(PlanarFace planarFaceHost, IEnumerable<PlanarFace> planarFaces, double epsilonArea)
        {
            if (planarFaces.Count() > 0)
            {
                foreach (PlanarFace planarFace in planarFaces)
                {
                    if (Math.Abs(planarFaceHost.FaceNormal.DotProduct(planarFace.FaceNormal) + 1) < GeometryLib.EPSILON)
                    {
                        if (planarFace.Area > epsilonArea)
                        {
                            return planarFace;
                        }
                    }
                }
            }
            return null;
        }

        public static CylindricalFace GetCylindricalFace(IEnumerable<CylindricalFace> cylindricalFaces, Solid solidHost, double epsilonArea)
        {
            if (cylindricalFaces.Count() > 0)
            {
                foreach (CylindricalFace cylindricalFace in cylindricalFaces)
                {
                    if (GeometryLib.IsFaceTouchSolid(cylindricalFace, solidHost))
                    {
                        if (cylindricalFace.Area > epsilonArea)
                        {
                            return cylindricalFace;
                        }
                    }
                }
            }
            return null;
        }

        //public static void FilterTopFaces(ref CommonFaceFilter commonFaceFilter, Element element, ParaUtil paraUtil)
        //{
        //    if (commonFaceFilter.TopFaces.OriginFaces.Count > 0)
        //    {
        //        List<Face> topFacesOrigin = new List<Face>();
        //        foreach (Face face in commonFaceFilter.TopFaces.OriginFaces)
        //        {
        //            if (face is PlanarFace planarFace)
        //            {
        //                double angleWithDirectionZ = Math.Abs(GeometryLib.AngleBetweenTwoVectors(planarFace.FaceNormal, XYZ.BasisZ, true));
        //                if (angleWithDirectionZ >= paraUtil.AngleTopAreaFrom || IntersectRay.IsInsidePlanarFace(planarFace, element, true))
        //                {
        //                    topFacesOrigin.Add(face);
        //                }
        //            }
        //            else
        //            {
        //                topFacesOrigin.Add(face);
        //            }
        //        }
        //        commonFaceFilter.TopFaces.OriginFaces = topFacesOrigin;
        //    }
        //    if (commonFaceFilter.TopFaces.RegainFaces.Count > 0)
        //    {
        //        List<Face> topFacesRegain = new List<Face>();
        //        foreach (Face face in commonFaceFilter.TopFaces.RegainFaces)
        //        {
        //            if (face is PlanarFace planarFace)
        //            {
        //                double angleWithDirectionZ = Math.Abs(GeometryLib.AngleBetweenTwoVectors(-planarFace.FaceNormal, XYZ.BasisZ, true));
        //                if (angleWithDirectionZ >= paraUtil.AngleTopAreaFrom || IntersectRay.IsInsidePlanarFace(planarFace, element, false))
        //                {
        //                    topFacesRegain.Add(face);
        //                }
        //            }
        //            else
        //            {
        //                topFacesRegain.Add(face);
        //            }
        //        }
        //        commonFaceFilter.TopFaces.RegainFaces = topFacesRegain;
        //    }
        //}
        public static void FilterBottomFaces(ref CommonFaceFilter commonFaceFilter, Element element, ParaUtil paraUtil)
        {
            if (commonFaceFilter.BottomFaces.OriginFaces.Count > 0)
            {
                List<Face> bottomFaces = new List<Face>();
                foreach (Face face in commonFaceFilter.BottomFaces.OriginFaces)
                {
                    if (face is PlanarFace planarFace)
                    {
                        if (IntersectRay.IsInsidePlanarFace(planarFace, element, true))
                        {
                            bottomFaces.Add(face);
                        }
                    }
                    else
                    {
                        bottomFaces.Add(face);
                    }
                }
                commonFaceFilter.BottomFaces.OriginFaces = bottomFaces;
            }
            if (commonFaceFilter.BottomFaces.RegainFaces.Count > 0)
            {
                List<Face> bottomFaces = new List<Face>();
                foreach (Face face in commonFaceFilter.BottomFaces.RegainFaces)
                {
                    if (face is PlanarFace planarFace)
                    {
                        if (IntersectRay.IsInsidePlanarFace(planarFace, element, false))
                        {
                            bottomFaces.Add(face);
                        }
                    }
                    else
                    {
                        bottomFaces.Add(face);
                    }
                }
                commonFaceFilter.BottomFaces.RegainFaces = bottomFaces;
            }
        }
    }
}
