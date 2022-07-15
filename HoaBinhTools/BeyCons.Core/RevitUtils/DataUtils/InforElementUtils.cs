#region Using
using Autodesk.Revit.DB;
using BeyCons.Core.Extensions;
using BeyCons.Core.Libraries.Geometries;
using System;
using System.Collections.Generic;
using System.Linq;
#endregion

namespace BeyCons.Core.RevitUtils.DataUtils
{
    public class InforElementUtils
    {
        public static List<Line> GetWidthHeightFraming(FamilyInstance framing, Options options)
        {
            Curve framingLocation = (framing.Location as LocationCurve).Curve;
            IEnumerable<PlanarFace> planarFaces = GeometryUtils.GetFacesFromSolid(GeometryUtils.GetSolidsFromOriginalFamilyInstance(framing, options)[0]).OfType<PlanarFace>().Where(x => Math.Abs(GeometryLib.AngleBetweenTwoVectors(x.FaceNormal, XYZ.BasisZ, false)) > 45);
            foreach (PlanarFace planarFace in planarFaces)
            {
                Transform transform = framingLocation.ComputeDerivatives(0, true);
                if(GeometryLib.AngleBetweenTwoVectors(transform.BasisX, planarFace.FaceNormal, false) < 5)
                {
                    CurveLoop curveLoop = planarFace.GetEdgesAsCurveLoops().MaxBy(x => x.GetExactLength());
                    if (curveLoop.IsRectangular(curveLoop.GetPlane()))
                    {
                        IEnumerable<Line> lines = curveLoop.Select(x => x).OfType<Line>();
                        Line width = lines.Where(y => GeometryLib.AngleBetweenTwoVectors(y.Direction, XYZ.BasisZ, false) > 45).FirstOrDefault();
                        Line height = lines.Where(y => GeometryLib.AngleBetweenTwoVectors(y.Direction, XYZ.BasisZ, false) < 45).FirstOrDefault();
                        if (null != width && null != height)
                        {
                            return new List<Line>() { width, height };
                        }
                    }
                }
            }
            return null;
        }
        public static List<Curve> GetDimentionColumn(FamilyInstance rectangularColumn, Options options)
        {
            XYZ columnLocation = (rectangularColumn.Location as LocationPoint).Point;
            PlanarFace planarFace = GeometryUtils.GetFacesFromSolid(GeometryUtils.GetSolidsFromOriginalFamilyInstance(rectangularColumn, options)[0]).OfType<PlanarFace>().Where(x => GeometryLib.ArePointsOnFace(x, new List<XYZ>() { columnLocation })).FirstOrDefault();
            if (null != planarFace)
            {
                CurveLoop curveLoop = planarFace.GetEdgesAsCurveLoops().MaxBy(x => x.GetExactLength());
                if (curveLoop.ToCurves().Count == 2 || curveLoop.ToCurves().Count == 4)
                {
                    List<Curve> lines = curveLoop.Select(x => x).ToList();
                    return new List<Curve>() { lines[0], lines[1] };
                }
            }
            return null;
        }
    }
}
