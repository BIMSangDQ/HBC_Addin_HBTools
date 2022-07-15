#region Using
using Autodesk.Revit.DB;
using Autodesk.Revit.DB.Architecture;
using BeyCons.Core.RevitUtils.DataUtils;
using BeyCons.Core.Libraries.Geometries.Enums;
using BeyCons.Core.Libraries.Geometries.Models;
using BeyCons.Core.Libraries.Units;
using System;
using System.Collections.Generic;
using System.Linq;
using BeyCons.Core.RevitUtils;
#endregion

namespace BeyCons.Core.Libraries.Geometries
{
    public static class GeometryLib
    {
        /// <summary>
        /// Giá trị nhỏ nhất coi như bằng nhau
        /// </summary>
        public const double EPSILON = 0.0001;
        /// <summary>
        /// Giá trị nhỏ nhất của thể tích
        /// </summary>
        public const double EPSILON_VOLUME = 0.0000000001;
        /// <summary>
        /// 
        /// </summary>
        public const double EPSILON_AREA = 0.0000001;
        /// <summary>
        /// Giá trị nhỏ nhất chênh lệch giữa các Curve trong Room
        /// </summary>
        public const double EPSILON_ROOM = 0.001;
        /// <summary>
        /// Số chữ số sau dấu phẩy cần làm tròn
        /// </summary>
        public const int N = 6;
        /// <summary>
        /// Chiều dài lớn nhất đoạn thẳng
        /// </summary>
        private const double DISTANCE_MAX = 1000000;
        /// <summary>
        /// Góc nhỏ nhất giữa 2 tường
        /// </summary>
        public const double DEGREE_ANGLE_MIN = 5;
        /// <summary>
        /// Kiểm tra 2 điểm có trùng nhau hay không
        /// </summary>
        /// <param name="pointOne"></param>
        /// <param name="pointTwo"></param>
        /// <returns> true/false </returns>
        public static bool ArePointsOverlap(XYZ pointOne, XYZ pointTwo)
        {
            bool result = false;
            if (pointOne != null && pointTwo != null)
            {
                bool boolX = Math.Abs(pointOne.X - pointTwo.X) <= EPSILON;
                bool boolY = Math.Abs(pointOne.Y - pointTwo.Y) <= EPSILON;
                bool boolZ = Math.Abs(pointOne.Z - pointTwo.Z) <= EPSILON;
                result = boolX && boolY && boolZ;
            }
            return result;
        }
        /// <summary>
        /// Tính khoảng cách giữa 2 điểm trong không gian
        /// </summary>
        /// <param name="pointOne"></param>
        /// <param name="pointTwo"></param>
        /// <returns> khoảng cách </returns>
        public static double DistanceBetweenTwoPoints(XYZ pointOne, XYZ pointTwo)
        {
            return Math.Sqrt(Math.Pow(pointOne.X - pointTwo.X, 2) + Math.Pow(pointOne.Y - pointTwo.Y, 2) + Math.Pow(pointOne.Z - pointTwo.Z, 2));
        }
        /// <summary>
        /// Tính chiều dài vector
        /// </summary>
        /// <param name="vector"></param>
        /// <returns> chiều dài </returns>
        public static double VectorLength(XYZ vector)
        {
            return Math.Sqrt(Math.Pow(vector.X, 2) + Math.Pow(vector.Y, 2) + Math.Pow(vector.Z, 2));
        }
        /// <summary>
        /// Tìm vector đi qua 2 điểm
        /// </summary>
        /// <param name="pointOne"></param>
        /// <param name="pointTwo"></param>
        /// <returns> vector </returns>
        public static XYZ Vector(XYZ pointOne, XYZ pointTwo)
        {
            return new XYZ(pointTwo.X - pointOne.X, pointTwo.Y - pointOne.Y, pointTwo.Z - pointOne.Z);
        }
        /// <summary>
        /// Tìm vector đơn vị
        /// </summary>
        /// <param name="vector"></param>
        /// <returns> vector </returns>
        public static XYZ UnitVector(XYZ vector)
        {
            double denominator = Math.Sqrt(Math.Pow(vector.X, 2) + Math.Pow(vector.Y, 2) + Math.Pow(vector.Z, 2));
            return new XYZ(vector.X / denominator, vector.Y / denominator, vector.Z / denominator);
        }
        /// <summary>
        /// Đảo chiều vector ban đầu
        /// </summary>
        /// <param name="vector"></param>
        /// <returns> vector </returns>
        public static XYZ ReversingVector(XYZ vector)
        {
            return new XYZ(-vector.X, -vector.Y, -vector.Z);
        }
        /// <summary>
        /// Hai vector có bằng nhau hay không
        /// </summary>
        /// <param name="vectorOne"></param>
        /// <param name="vectorTwo"></param>
        /// <returns></returns>
        public static bool AreVectorsEqual(XYZ vectorOne, XYZ vectorTwo)
        {
            bool result = false;
            bool compareX = Math.Abs(vectorOne.X - vectorTwo.X) <= EPSILON;
            bool compareY = Math.Abs(vectorOne.Y - vectorTwo.Y) <= EPSILON;
            bool compareZ = Math.Abs(vectorOne.Z - vectorTwo.Z) <= EPSILON;
            if (compareX && compareY && compareZ)
            {
                result = true;
            }
            return result;
        }
        /// <summary>
        /// Tìm góc giữa 2 vector
        /// </summary>
        /// <param name="vectorOne"></param>
        /// <param name="vectorTwo"></param>
        /// <returns> Giá trị là độ </returns>
        public static double AngleBetweenTwoVectors(XYZ vectorOne, XYZ vectorTwo, bool absolute)
        {
            double numerator = vectorOne.X * vectorTwo.X + vectorOne.Y * vectorTwo.Y + vectorOne.Z * vectorTwo.Z;
            double denominator = VectorLength(vectorOne) * VectorLength(vectorTwo);
            if (absolute)
            {
                return (Math.Acos(Math.Round(numerator / denominator, N))).ToDegree();
            }
            else
            {
                return (Math.Acos(Math.Round(Math.Abs(numerator) / denominator, N))).ToDegree();
            }
        }
        /// <summary>
        /// Tính khoản cách từ một điểm đến đường trong không gian Oxyz
        /// </summary>
        /// <param name="point"></param>
        /// <param name="line"></param>
        /// <returns> khoảng cách </returns>
        public static double DistanceFromPointToLine(XYZ point, Line line)
        {
            XYZ vector = Vector(point, line.Origin);
            double denominator = VectorLength(line.Direction);
            XYZ dotProduct = CrossProduct(vector, line.Direction);
            double numerator = VectorLength(dotProduct);
            return numerator / denominator;
        }
        /// <summary>
        /// Tính khoảng cách từ một điểm đến mặt phẳng
        /// </summary>
        /// <param name="point"></param>
        /// <param name="plane"></param>
        /// <returns> khoảng cách </returns>
        public static double DistanceFromPointToPlane(XYZ point, Plane plane)
        {
            XYZ normal = plane.Normal;
            XYZ origin = plane.Origin;
            double d = -(normal.X * origin.X + normal.Y * origin.Y + normal.Z * origin.Z);
            double numerator = Math.Abs(normal.X * point.X + normal.Y * point.Y + normal.Z * point.Z + d);
            double denominator = Math.Sqrt(Math.Pow(normal.X, 2) + Math.Pow(normal.Y, 2) + Math.Pow(normal.Z, 2));
            return numerator / denominator;
        }
        /// <summary>
        /// Kiểm tra danh sách các điểm có thuộc mặt phẳng hay không
        /// </summary>
        /// <param name="points"></param>
        /// <param name="plane"></param>
        /// <returns> Có/Không </returns>
        public static bool ArePointsOnPlane(List<XYZ> points, Plane plane)
        {
            bool result = false;
            if (points.Count > 0)
            {
                XYZ normal = plane.Normal;
                XYZ origin = plane.Origin;
                foreach (XYZ point in points)
                {
                    double value = normal.X * (point.X - origin.X) + normal.Y * (point.Y - origin.Y) + normal.Z * (point.Z - origin.Z);
                    if (Math.Abs(value) <= EPSILON)
                    {
                        result = true;
                    }
                    else
                    {
                        result = false;
                        break;
                    }
                }
            }
            return result;
        }
        /// <summary>
        /// Kiểm tra 1 điểm có nằm trên đoạn thẳng trong không gian
        /// </summary>
        /// <param name="point"></param>
        /// <param name="line"></param>
        /// <returns> bool </returns>
        public static bool IsPointOnCurve(XYZ point, Curve curve)
        {
            try
            {
                IntersectionResult intersectionResult = curve.Project(point);
                if (null != intersectionResult && intersectionResult.Distance < EPSILON)
                {
                    double ratio = Math.Abs(intersectionResult.Parameter - curve.GetEndParameter(0)) / (curve.GetEndParameter(1) - curve.GetEndParameter(0));
                    if (ratio >= 0 && ratio <= 1)
                    {
                        intersectionResult?.Dispose();
                        return true;
                    }
                }
                intersectionResult?.Dispose();
            }
            catch { }
            return false;
        }
        /// <summary>
        /// Kiểm tra 1 điểm có thuộc Curve nhưng chưa chắc nằm trên Curve
        /// </summary>
        /// <param name="point"></param>
        /// <param name="curve"></param>
        /// <returns> Có/Không </returns>
        public static bool IsPointAtCurve(XYZ point, Curve curve)
        {
            try
            {
                if (curve.Distance(point) < EPSILON)
                {
                    return true;
                }
            }
            catch
            {
                return true;
            }
            return false;
        }
        /// <summary>
        /// Kiểm tra 2 đường thẳng trong không gian có đồng phẳng với nhau
        /// </summary>
        /// <param name="lineOne"></param>
        /// <param name="lineTwo"></param>
        /// <returns> Có/Không </returns>
        public static bool AreLinesOnPlane(Line lineOne, Line lineTwo)
        {
            bool result = false;
            XYZ pointOne = lineOne.Origin;
            XYZ vectorDirectionOne = lineOne.Direction;
            XYZ pointTwo = lineTwo.Origin;
            XYZ vectorDirectionTwo = lineTwo.Direction;
            XYZ m1m2 = Vector(pointOne, pointTwo);
            XYZ crossProductBetweenVecotrDirectionOneAndVectorDirectionTwo = CrossProduct(vectorDirectionOne, vectorDirectionTwo);
            //Kiểm tra có đông phẳng không
            if (Math.Abs(DotProduct(crossProductBetweenVecotrDirectionOneAndVectorDirectionTwo, m1m2)) < EPSILON)
            {
                result = true;
            }
            return result;
        }
        /// <summary>
        /// Xác định vị trí tương đối giữa 2 đường thẳng
        /// </summary>
        /// <param name="lineOne"></param>
        /// <param name="lineTwo"></param>
        /// <returns> Class ViTriTuongDoi </returns>
        public static BetweenLineAndLine RelativePositionBetweenLineAndLine(Line lineOne, Line lineTwo)
        {
            BetweenLineAndLine result = BetweenLineAndLine.None;
            XYZ pointOne = lineOne.Origin;
            XYZ vectorDirectionOne = lineOne.Direction;
            XYZ pointTwo = lineTwo.Origin;
            XYZ vectorDirectionTwo = lineTwo.Direction;
            XYZ m1m2 = Vector(pointOne, pointTwo);
            XYZ crossProductBetweenVecotrDirectionOneAndVectorDirectionTwo = CrossProduct(vectorDirectionOne, vectorDirectionTwo);
            XYZ crossProductBetweenVecotrDirectionOneAndVectorM1M2 = CrossProduct(vectorDirectionOne, m1m2);
            if (Math.Abs(DotProduct(crossProductBetweenVecotrDirectionOneAndVectorDirectionTwo, m1m2)) < EPSILON && !AreVectorsEqual(crossProductBetweenVecotrDirectionOneAndVectorDirectionTwo, XYZ.Zero))
            {
                result = BetweenLineAndLine.Intersecting;
                if (Math.Abs(AngleBetweenTwoVectors(vectorDirectionOne, vectorDirectionTwo, true) - 90) <= EPSILON)
                {
                    result = BetweenLineAndLine.IntersectPerpendicular;
                }
            }
            else if (AreVectorsEqual(crossProductBetweenVecotrDirectionOneAndVectorDirectionTwo, XYZ.Zero) && !AreVectorsEqual(crossProductBetweenVecotrDirectionOneAndVectorM1M2, XYZ.Zero))
            {
                result = BetweenLineAndLine.Parallel;
            }
            else if (AreVectorsEqual(crossProductBetweenVecotrDirectionOneAndVectorDirectionTwo, XYZ.Zero) && AreVectorsEqual(crossProductBetweenVecotrDirectionOneAndVectorM1M2, XYZ.Zero))
            {
                result = BetweenLineAndLine.Overlap;
            }
            else if (!AreVectorsEqual(crossProductBetweenVecotrDirectionOneAndVectorDirectionTwo, XYZ.Zero) && Math.Abs(DotProduct(crossProductBetweenVecotrDirectionOneAndVectorDirectionTwo, m1m2)) > EPSILON)
            {
                result = BetweenLineAndLine.CrossEachOther;
                if (Math.Abs(AngleBetweenTwoVectors(vectorDirectionOne, vectorDirectionTwo, true) - 90) <= EPSILON)
                {
                    result = BetweenLineAndLine.CrossEachOtherPerpendicular;
                }
            }
            return result;
        }
        /// <summary>
        /// Tìm khoảng cách giữa 2 đường thẳng chéo nhau
        /// </summary>
        /// <param name="lineOne"></param>
        /// <param name="lineTwo"></param>
        /// <returns> Khoảng cách </returns>
        public static double DistanceBetweenTwoLinesCrossed(Line lineOne, Line lineTwo)
        {
            if (RelativePositionBetweenLineAndLine(lineOne, lineTwo) == BetweenLineAndLine.CrossEachOther)
            {
                XYZ m1m2 = Vector(lineOne.Origin, lineTwo.Origin);
                XYZ directionOne = lineOne.Direction;
                XYZ directionTwo = lineTwo.Direction;
                XYZ crossProduct = CrossProduct(directionOne, directionTwo);
                double numerator = Math.Abs(DotProduct(m1m2, crossProduct));
                double denominator = VectorLength(crossProduct);
                return numerator / denominator;
            }
            else
            {
                return 0;
            }
        }
        /// <summary>
        /// Tìm giao điểm giữa 2 đường thẳng
        /// </summary>
        /// <param name="lineOne"></param>
        /// <param name="lineTwo"></param>
        /// <returns> ĐIểm </returns>
        public static XYZ PointBetweenTwoLine(Line lineOne, Line lineTwo)
        {
            if (RelativePositionBetweenLineAndLine(lineOne, lineTwo) == BetweenLineAndLine.Intersecting || RelativePositionBetweenLineAndLine(lineOne, lineTwo) == BetweenLineAndLine.IntersectPerpendicular)
            {
                XYZ M1 = lineOne.Origin;
                XYZ u1 = lineOne.Direction;
                XYZ M2 = lineTwo.Origin;
                XYZ u2 = lineTwo.Direction;
                double numerator = M1.X * u1.Y - M1.Y * u1.X + M2.Y * u1.X - M2.X * u1.Y;
                double denominator = u2.X * u1.Y - u2.Y * u1.X;
                double t = numerator / denominator;
                double x = M2.X + u2.X * t;
                double y = M2.Y + u2.Y * t;
                double z = M2.Z + u2.Z * t;
                return new XYZ(x, y, z);
            }
            return null;
        }
        /// <summary>
        /// Kiểm tra 2 đoạn thẳng có cắt nhau
        /// </summary>
        /// <param name="lineOne"></param>
        /// <param name="lineTwo"></param>
        /// <returns> Có/Không </returns>
        public static bool IsCrossTwoCurve(Curve curveOne, Curve curveTwo)
        {
            curveOne.Intersect(curveTwo, out IntersectionResultArray intersectionResultArray);
            if (null != intersectionResultArray)
            {
                intersectionResultArray?.Dispose();
                return true;
            }
            intersectionResultArray?.Dispose();
            return false;
        }
        /// <summary>
        /// Tìm góc giữa đường thẳng và mặt phẳng
        /// </summary>
        /// <param name="line"></param>
        /// <param name="planarFace"></param>
        /// <returns> Góc là độ </returns>
        public static double AngleBewteenLineAndPlane(Line line, PlanarFace planarFace)
        {
            XYZ U1 = line.Direction;
            XYZ N1 = planarFace.FaceNormal;
            double numerator = Math.Abs(DotProduct(U1, N1));
            double denominator = VectorLength(U1) * VectorLength(N1);
            return Math.Round((Math.Asin(numerator / denominator)).ToDegree(), N);
        }
        /// <summary>
        /// Kiểm tra một điểm có nằm trên mặt phẳng bị giới hạn hay không
        /// </summary>
        /// <param name="point"></param>
        /// <param name="planarFace"></param>
        /// <returns> Có/Không </returns>
        public static bool IsPointInsidePlanarFace(XYZ point, PlanarFace planarFace)
        {
            if (planarFace.HasRegions)
            {
                foreach (PlanarFace planarFaceSub in planarFace.GetRegions())
                {
                    if (IsPointInsideRegion(point, planarFaceSub, planarFace.FaceNormal))
                    {
                        return true;
                    }
                }
            }
            else
            {
                if (IsPointInsideRegion(point, planarFace, planarFace.FaceNormal))
                {
                    return true;
                }
            }
            return false;
        }
        /// <summary>
        /// Kiểm tra điểm có nằm trong vùng xem xét.
        /// </summary>
        /// <param name="point"></param>
        /// <param name="region"></param>
        /// <returns> Yes/No </returns>
        public static bool IsPointInsideRegion(XYZ point, PlanarFace region, XYZ normal)
        {
            Line line = Line.CreateBound(point, TranslatingPoint(point, region.XVector, DISTANCE_MAX));
            List<CurveLoop> curveLoops = region.GetEdgesAsCurveLoops().ToList();

            List<CurveLoop> curveLoopsOutside = curveLoops.Where(x => x.IsCounterclockwise(normal)).ToList();
            List<CurveLoop> curveLoopsInside = curveLoops.Where(x => !x.IsCounterclockwise(normal)).ToList();

            foreach (CurveLoop curveLoop in curveLoops)
            {
                foreach (Curve curve in curveLoop)
                {
                    if (curve.Distance(point) < EPSILON)
                    {
                        return true;
                    }
                }
            }

            List<XYZ> pointsOut = new List<XYZ>();
            foreach (CurveLoop curveLoop in curveLoopsOutside)
            {
                foreach (Curve curve in curveLoop)
                {
                    line.Intersect(curve, out IntersectionResultArray intersectionResultArray);
                    if (null != intersectionResultArray)
                    {
                        foreach (IntersectionResult intersectionResult in intersectionResultArray)
                        {
                            pointsOut.Add(intersectionResult.XYZPoint);
                            intersectionResult?.Dispose();
                        }
                    }
                    intersectionResultArray?.Dispose();
                }
            }
            if (pointsOut.Count % 2 == 0)
            {
                return false;
            }

            if (curveLoopsInside.Count > 0)
            {
                List<XYZ> pointsIn = new List<XYZ>();
                foreach (CurveLoop curveLoop in curveLoopsInside)
                {
                    foreach (Curve curve in curveLoop)
                    {
                        line.Intersect(curve, out IntersectionResultArray intersectionResultArray);
                        if (null != intersectionResultArray)
                        {
                            foreach (IntersectionResult intersectionResult in intersectionResultArray)
                            {
                                pointsIn.Add(intersectionResult.XYZPoint);
                            }
                        }
                        intersectionResultArray?.Dispose();
                    }
                }
                if (pointsIn.Count % 2 == 1)
                {
                    return false;
                }
            }
            line?.Dispose();

            return true;
        }
        /// <summary>
        /// Tính diện tích đa giác bất kì
        /// </summary>
        /// <param name="points"></param>
        /// <returns> double (area) </returns>
        public static double PolygonArea(List<XYZ> points)
        {
            double area = 0;
            int j = points.Count - 1;
            for (int i = 0; i < points.Count; i++)
            {
                area += (points[j].X + points[i].X) * (points[j].Y - points[i].Y);
                j = i;
            }
            return area / 2;
        }
        /// <summary>
        /// Kiểm tra 2 đường thẳng có overlap.
        /// </summary>
        /// <param name="lineOne"></param>
        /// <param name="lineTwo"></param>
        /// <returns> Có/Không </returns>
        public static bool AreTwoSegmentOverlap(Line lineOne, Line lineTwo)
        {
            BetweenLineAndLine relativePosition = RelativePositionBetweenLineAndLine(lineOne, lineTwo);
            if (relativePosition == BetweenLineAndLine.Overlap)
            {
                XYZ pointOne = lineOne.Evaluate(0.5, true);
                XYZ pointTwo = lineTwo.Evaluate(0.5, true);
                double distance = pointOne.DistanceTo(pointTwo);
                return distance <= (lineOne.Length + lineTwo.Length) / 2;
            }
            return false;
        }
        /// <summary>
        /// Di chuyển 1 điểm theo một vector với khoảng cách cho trước
        /// </summary>
        /// <param name="point"></param>
        /// <param name="vector"></param>
        /// <param name="distanceFeet"></param>
        /// <returns> điểm </returns>
        public static XYZ TranslatingPoint(XYZ point, XYZ vector, double distanceFeet)
        {
            XYZ normal = UnitVector(vector);
            return new XYZ(point.X + normal.X * distanceFeet, point.Y + normal.Y * distanceFeet, point.Z + normal.Z * distanceFeet);
        }
        /// <summary>
        /// Tìm tích có hướng giữa 2 vector
        /// </summary>
        /// <param name="vectorOne"></param>
        /// <param name="vectorTwo"></param>
        /// <returns> vector </returns>
        public static XYZ CrossProduct(XYZ vectorOne, XYZ vectorTwo)
        {
            double toaDoX = vectorOne.Y * vectorTwo.Z - vectorTwo.Y * vectorOne.Z;
            double toaDoY = vectorOne.Z * vectorTwo.X - vectorTwo.Z * vectorOne.X;
            double toaDoZ = vectorOne.X * vectorTwo.Y - vectorTwo.X * vectorOne.Y;
            return new XYZ(toaDoX, toaDoY, toaDoZ);
        }
        /// <summary>
        /// Xác định tích vô hướng giữa 2 vector
        /// </summary>
        /// <param name="vectorOne"></param>
        /// <param name="vectorTwo"></param>
        /// <returns> double </returns>
        public static double DotProduct(XYZ vectorOne, XYZ vectorTwo)
        {
            return vectorOne.X * vectorTwo.X + vectorOne.Y * vectorTwo.Y + vectorOne.Z * vectorTwo.Z;
        }
        /// <summary>
        /// Kiểm tra một danh sách các điểm có cùng phía với mặt phẳng
        /// </summary>
        /// <param name="plane"></param>
        /// <param name="points"></param>
        /// <returns> True/False </returns>
        public static bool IsSameSide(Plane plane, List<XYZ> points)
        {
            double equation(XYZ point) => plane.Normal.X * (point.X - plane.Origin.X) + plane.Normal.Y * (point.Y - plane.Origin.Y) + plane.Normal.Z * (point.Z - plane.Origin.Z);
            double result = equation(points[0]);
            for (int i = 1; i < points.Count; i++)
            {
                result *= equation(points[i]);
                if (result < 0)
                {
                    return false;
                }
            }
            return true;
        }
        /// <summary>
        /// Vị trí tương đối giữa đường và mặt phẳng
        /// </summary>
        /// <param name="line"></param>
        /// <param name="plane"></param>
        /// <returns> BetweenLineAndPlane </returns>
        public static BetweenLineAndPlane RelativePositionBetweenLineAndPlane(Line line, Plane plane)
        {
            double equation(XYZ point) => plane.Normal.X * (point.X - plane.Origin.X) + plane.Normal.Y * (point.Y - plane.Origin.Y) + plane.Normal.Z * (point.Z - plane.Origin.Z);

            if (Math.Abs(DotProduct(line.Direction, plane.Normal)) < EPSILON)
            {
                if (Math.Abs(equation(line.Origin)) < EPSILON)
                {
                    return BetweenLineAndPlane.Overlap;
                }
                else
                {
                    return BetweenLineAndPlane.Parallel;
                }
            }
            else
            {
                if (Math.Abs(AngleBetweenTwoVectors(line.Direction, plane.Normal, false)) < EPSILON)
                {
                    return BetweenLineAndPlane.IntersectPerpendicular;
                }
                else
                {
                    return BetweenLineAndPlane.Intersecting;
                }
            }
        }
        /// <summary>
        /// Tìm điểm giáo giữa đường thẳng và mặt phẳng
        /// </summary>
        /// <param name="line"></param>
        /// <param name="plane"></param>
        /// <returns> Điểm </returns>
        public static XYZ IntersectLineAndPlane(Line line, Plane plane)
        {
            if (RelativePositionBetweenLineAndPlane(line, plane) == BetweenLineAndPlane.IntersectPerpendicular || RelativePositionBetweenLineAndPlane(line, plane) == BetweenLineAndPlane.Intersecting)
            {
                XYZ n = plane.Normal;
                XYZ pP = plane.Origin;
                XYZ v = line.Direction;
                XYZ pL = line.Origin;
                double numerator = n.X * (pL.X - pP.X) + n.Y * (pL.Y - pP.Y) + n.Z * (pL.Z - pP.Z);
                double denominator = n.X * v.X + n.Y * v.Y + n.Z * v.Z;
                double t = -numerator / denominator;
                return new XYZ(pL.X + v.X * t, pL.Y + v.Y * t, pL.Z + v.Z * t);
            }
            else
            {
                return null;
            }
        }
        /// <summary>
        /// Lấy curvearray từ boundarysegment và hiệu chỉnh nếu hở
        /// </summary>
        /// <param name="boundarySegment"></param>
        /// <returns> CurveArray </returns>
        public static CurveArray GetCurveArrayFromBoundarySegment(IList<BoundarySegment> boundarySegment)
        {
            List<Curve> curves = new List<Curve>();
            for (int i = 0; i < boundarySegment.Count; i++)
            {
                if (i == 0)
                {
                    curves.Add(boundarySegment[i].GetCurve());
                }
                else
                {
                    int count = curves.Count;
                    Line newLine = boundarySegment[i].GetCurve() as Line;
                    Line lastLine = curves[count - 1] as Line;
                    if (null != newLine && null != lastLine)
                    {
                        double dotProduct = newLine.Direction.DotProduct(lastLine.Direction);
                        if ((1 - dotProduct) < EPSILON_ROOM)
                        {
                            Line line = Line.CreateBound(lastLine.GetEndPoint(0), newLine.GetEndPoint(1));
                            curves.Remove(curves.Last());
                            curves.Add(line);
                        }
                        else if ((1 + dotProduct) < EPSILON_ROOM)
                        {
                            Line line = Line.CreateBound(lastLine.GetEndPoint(0), newLine.GetEndPoint(0));
                            curves.Remove(curves.Last());
                            curves.Add(line);
                        }
                        else
                        {
                            curves.Add(boundarySegment[i].GetCurve());
                        }
                    }
                    else
                    {
                        curves.Add(boundarySegment[i].GetCurve());
                    }
                }
            }
            Line lineEnd = curves.Last() as Line;
            Line lineBegin = curves.First() as Line;
            if (null != lineEnd && null != lineBegin)
            {
                double dotProduct = lineEnd.Direction.DotProduct(lineBegin.Direction);
                if ((1 - dotProduct) < EPSILON_ROOM)
                {
                    Line line = Line.CreateBound(lineEnd.GetEndPoint(0), lineBegin.GetEndPoint(1));
                    curves.Remove(curves.Last());
                    curves.Remove(curves.First());
                    curves.Add(line);
                }
                else if ((1 + dotProduct) < EPSILON_ROOM)
                {
                    Line line = Line.CreateBound(lineEnd.GetEndPoint(0), lineBegin.GetEndPoint(0));
                    curves.Remove(curves.Last());
                    curves.Remove(curves.First());
                    curves.Add(line);
                }
            }
            CurveArray curveArray = new CurveArray();
            curves.ForEach(x => curveArray.Append(x));
            return curveArray;
        }
        /// <summary>
        /// Kiểm tra biên dạng của room có bị méo.
        /// </summary>
        /// <param name="room"></param>
        /// <returns> True/False </returns>
        public static bool CheckBoundaryRoom(Room room)
        {
            SpatialElementBoundaryOptions spatialElementBoundaryOptions = new SpatialElementBoundaryOptions();
            List<Curve> curves = room.GetBoundarySegments(spatialElementBoundaryOptions).FirstOrDefault().Select(x => x.GetCurve()).ToList();
            for (int i = 0; i < curves.Count - 1; i++)
            {
                if (curves[i] is Line lineOneFor && curves[i + 1] is Line lineTwoFor)
                {
                    double angle = AngleBetweenTwoVectors(lineOneFor.Direction, lineTwoFor.Direction, true);
                    if (angle > 180 - DEGREE_ANGLE_MIN)
                    {
                        return false;
                    }
                }
            }
            if (curves[curves.Count - 1] is Line lineOne && curves[0] is Line lineTwo)
            {
                double angle = AngleBetweenTwoVectors(lineOne.Direction, lineTwo.Direction, true);
                if (angle > 180 - DEGREE_ANGLE_MIN)
                {
                    return false;
                }
            }
            spatialElementBoundaryOptions?.Dispose();
            return true;
        }
        /// <summary>
        /// Lấy tâm curve array
        /// </summary>
        /// <param name="curveArray"></param>
        /// <returns> Điểm </returns>
        public static XYZ GetCenterPolygon(CurveArray curveArray)
        {
            List<XYZ> points = new List<XYZ>();
            foreach (Curve curve in curveArray)
            {
                points.Add(curve.Evaluate(0.5, true));
            }
            return new XYZ(points.Select(x => x.X).Sum() / points.Count, points.Select(y => y.Y).Sum() / points.Count, points.Select(z => z.Z).Sum() / points.Count);
        }
        /// <summary>
        /// Kiểm tra point có nằm trong CurveArray
        /// </summary>
        /// <param name="curves"></param>
        /// <param name="point"></param>
        /// <returns> True/False </returns>
        public static bool IsPointInsidePolygon(CurveArray curveArray, XYZ point)
        {
            CurveLoop curveLoop = curveArray.ToCurveLoop();
            if (curveLoop.ToList().Count > 0)
            {
                Plane plane = curveLoop.GetPlane();
                Line line = Line.CreateBound(point, TranslatingPoint(point, plane.Normal, DISTANCE_MAX));
                XYZ pointIntersect = IntersectLineAndPlane(line, plane);
                foreach (Curve curve in curveArray)
                {
                    if (IsPointOnCurve(pointIntersect, curve))
                    {
                        return true;
                    }
                }
                line?.Dispose();
                Line ray = Line.CreateBound(pointIntersect, TranslatingPoint(pointIntersect, plane.XVec, DISTANCE_MAX));
                int intersect = 0;
                foreach (Curve curve in curveArray)
                {
                    ray.Intersect(curve, out IntersectionResultArray intersectionResultArray);
                    if (null != intersectionResultArray)
                    {
                        intersect++;
                    }
                    intersectionResultArray?.Dispose();
                }
                if (intersect % 2 == 1)
                {
                    return true;
                }
                ray?.Dispose();
            }
            curveLoop.Dispose();
            return false;
        }
        /// <summary>
        /// Kiểm tra point có nằm trên face
        /// </summary>
        /// <param name="face"></param>
        /// <param name="point"></param>
        /// <returns> True/False </returns>
        public static bool ArePointsOnFace(Face face, List<XYZ> points)
        {
            bool result = true;
            foreach (XYZ point in points)
            {
                IntersectionResult intersectionResult = face.Project(point);
                if (null != intersectionResult)
                {
                    result = result && intersectionResult.Distance < EPSILON;
                }
                else
                {
                    result = false;
                }
                intersectionResult?.Dispose();
            }
            return result;
        }
        public static List<Line> GetLinesFromPoints(List<XYZ> points)
        {
            List<Line> lines = new List<Line>();
            if (points.Count > 1)
            {
                for (int i = 0; i < points.Count - 1; i++)
                {
                    for (int j = i + 1; j < points.Count; j++)
                    {
                        if (DistanceBetweenTwoPoints(points[i], points[j]) >= 1.0.ToFeet())
                        {
                            lines.Add(Line.CreateBound(points[i], points[j]));
                        }
                    }
                }
            }
            return lines;
        }
        /// <summary>
        /// Lấy số lượng curve trong CurveLoop dành cho Revit 2018
        /// </summary>
        /// <param name="curveLoop"></param>
        /// <returns></returns>
        public static int NumberOfCurveOfCurveLoop(this CurveLoop curveLoop)
        {
            int numberOfCurve = 0;
            foreach (Curve curve in curveLoop)
            {
                numberOfCurve++;
            }
            return numberOfCurve;
        }
        /// <summary>
        /// Vị trí tương đối giữa các cung tròn trên cùng mặt phẳng
        /// </summary>
        /// <param name="arcOne"></param>
        /// <param name="arcTwo"></param>
        /// <returns> BetweenArcAndArc </returns>
        public static BetweenArcAndArc RelativePositionBetweenArcAndArc(Arc arcOne, Arc arcTwo)
        {
            if (!ArePointsOverlap(arcOne.Center, arcTwo.Center))
            {
                arcOne.Intersect(arcTwo, out IntersectionResultArray intersectionResultArray);
                if (null != intersectionResultArray)
                {
                    intersectionResultArray?.Dispose();
                    return BetweenArcAndArc.Intersecting;
                }
                else
                {
                    intersectionResultArray?.Dispose();
                    return BetweenArcAndArc.None;
                }
            }
            else if (ArePointsOverlap(arcOne.Center, arcTwo.Center) && Math.Abs(arcOne.Radius - arcTwo.Radius) >= EPSILON)
            {
                return BetweenArcAndArc.Parallel;
            }
            else
            {
                XYZ pointOne = arcOne.Evaluate(0.5, true);
                XYZ pointTwo = arcTwo.Evaluate(0.5, true);
                XYZ pointTemp = Line.CreateBound(pointOne, pointTwo).Evaluate(0.5, true);
                XYZ pointOnArc = TranslatingPoint(arcOne.Center, Vector(arcOne.Center, pointTemp), arcOne.Radius);
                Arc arcSub = Arc.Create(pointOne, pointTwo, pointOnArc);
                if (arcSub.Length <= (arcOne.Length + arcTwo.Length) / 2)
                {
                    return BetweenArcAndArc.OnCircleOverlap;
                }
                else
                {
                    return BetweenArcAndArc.OnCircleNoOverlap;
                }
            }
        }
        /// <summary>
        /// Lấy mặt phẳng có vector pháp tuyến song song với mặt phẳng cho trước
        /// </summary>
        /// <param name="planarFaces"></param>
        /// <param name="vector"></param>
        /// <returns></returns>
        public static List<PlanarFace> GetPlanarFaceHaveNormalVectorParallelVector(Solid solid, XYZ vector)
        {
            List<PlanarFace> filterPlanarFaces = new List<PlanarFace>();
            foreach (Face face in solid.Faces)
            {
                if (face is PlanarFace planarFace)
                {
                    if (Math.Abs(AngleBetweenTwoVectors(planarFace.FaceNormal, vector, true)) < EPSILON)
                    {
                        filterPlanarFaces.Add(planarFace);
                    }
                }
            }
            return filterPlanarFaces;
        }
        /// <summary>
        /// Kiểm tra 2 điểm có nằm khác phía đối với mặt phẳng
        /// </summary>
        /// <param name="plane"></param>
        /// <param name="pointOne"></param>
        /// <param name="pointTwo"></param>
        /// <returns> True/False </returns>
        public static bool AreTwoSidePlane(Plane plane, XYZ pointOne, XYZ pointTwo)
        {
            double equation(XYZ point) => plane.Normal.X * (point.X - plane.Origin.X) + plane.Normal.Y * (point.Y - plane.Origin.Y) + plane.Normal.Z * (point.Z - plane.Origin.Z);
            if (Math.Round(equation(pointOne) * equation(pointTwo), N) < 0)
            {
                return true;
            }
            return false;
        }
        /// <summary>
        /// Nhóm 2 danh sách điểm nằm về 2 phía đường thẳng và cùng nằm trên mặt phẳng
        /// </summary>
        /// <param name="plane"></param>
        /// <param name="line"></param>
        /// <param name="points"></param>
        /// <returns> Trả về 2 danh sách </returns>
        public static PointsTwoSideOfLine FilterPointsTwoSideOfLine(Plane plane, Line line, List<XYZ> points)
        {
            PointsTwoSideOfLine pointsTwoSideOfLine = new PointsTwoSideOfLine() { FirstPoints = new List<XYZ>(), SecondPoints = new List<XYZ>() };
            if (points.Count > 1)
            {
                if (ArePointsOnPlane(points, plane))
                {
                    XYZ nomalNewPlane = line.Direction.CrossProduct(plane.Normal);
                    double equation(XYZ point) => nomalNewPlane.X * (point.X - line.Origin.X) + nomalNewPlane.Y * (point.Y - line.Origin.Y) + nomalNewPlane.Z * (point.Z - line.Origin.Z);
                    XYZ pointCheck = points[0];
                    pointsTwoSideOfLine.FirstPoints.Add(pointCheck);
                    points.RemoveAt(0);
                    for (int i = 0; i < points.Count; i++)
                    {
                        if (Math.Round(equation(pointCheck) * equation(points[i]), N) < 0)
                        {
                            pointsTwoSideOfLine.SecondPoints.Add(points[i]);
                        }
                        else
                        {
                            pointsTwoSideOfLine.FirstPoints.Add(points[i]);
                        }
                    }
                }
            }
            return pointsTwoSideOfLine;
        }
        /// <summary>
        /// Phân loại points bởi mặt phẳng
        /// </summary>
        /// <param name="plane"></param>
        /// <param name="points"></param>
        /// <returns> 2 danh sách point hoặc null </returns>
        public static List<List<XYZ>> DividePointsByPlane(Plane plane, List<XYZ> points)
        {
            if (points.Count > 0)
            {
                double equation(XYZ point) => plane.Normal.X * (point.X - plane.Origin.X) + plane.Normal.Y * (point.Y - plane.Origin.Y) + plane.Normal.Z * (point.Z - plane.Origin.Z);
                List<XYZ> firstPoints = new List<XYZ>();
                List<XYZ> secondPoints = new List<XYZ>();
                XYZ pointCheck = points[0];
                firstPoints.Add(pointCheck);
                points.RemoveAt(0);
                for (int i = 0; i < points.Count; i++)
                {
                    if (Math.Round(equation(pointCheck) * equation(points[i]), N) < 0)
                    {
                        secondPoints.Add(points[i]);
                    }
                    else
                    {
                        firstPoints.Add(points[i]);
                    }
                }
                return new List<List<XYZ>>() { firstPoints, secondPoints };
            }
            return null;
        }
        /// <summary>
        /// Lấy những đoạn thẳng nằm trên đường thẳng
        /// </summary>
        /// <param name="lines"></param>
        /// <param name="lineHost"></param>
        /// <returns> List<Line> </returns>
        public static List<Line> GetLineOverlapWithLine(List<Line> lines, Line lineHost)
        {
            List<Line> linesFilter = new List<Line>();
            foreach (Line line in lines)
            {
                if (DistanceFromPointToLine(line.GetEndPoint(0), lineHost) < EPSILON && DistanceFromPointToLine(line.GetEndPoint(1), lineHost) < EPSILON)
                {
                    linesFilter.Add(line);
                }
            }
            return linesFilter;
        }
        /// <summary>
        /// Kiểm tra face có nằm trên solid
        /// </summary>
        /// <param name="face"></param>
        /// <param name="solid"></param>
        /// <returns></returns>
        public static bool IsFaceTouchSolid(Face face, Solid solid)
        {
            List<Curve> curves = new List<Curve>();
            foreach (EdgeArray edgeArray in face.EdgeLoops)
            {
                foreach (Edge edge in edgeArray)
                {
                    curves.Add(edge.AsCurveFollowingFace(face));
                }
            }
            SolidCurveIntersectionOptions solidCurveIntersectionOptions = new SolidCurveIntersectionOptions();
            foreach (Curve curve in curves)
            {
                try
                {
                    SolidCurveIntersection solidCurveIntersection = solid.IntersectWithCurve(curve, solidCurveIntersectionOptions);
                    if (null == solidCurveIntersection || solidCurveIntersection.ResultType == SolidCurveIntersectionMode.CurveSegmentsOutside || solidCurveIntersection.SegmentCount == 0)
                    {
                        return false;
                    }
                    solidCurveIntersection?.Dispose();
                }
                catch
                {
                    return false;
                }
            }
            solidCurveIntersectionOptions?.Dispose();
            return true;
        }
        /// <summary>
        /// Get BoundingBoxXYZ of solid in project
        /// </summary>
        /// <param name="solid"></param>
        /// <returns> BoundingBoxXYZ </returns>
        public static BoundingBoxXYZ GetBoundingBoxXYZSolidInProject(Solid solid)
        {
            BoundingBoxXYZ boundingBoxXYZ = solid.GetBoundingBox();
            Transform transform = boundingBoxXYZ.Transform;
            XYZ min = new XYZ(boundingBoxXYZ.Min.X + transform.Origin.X, boundingBoxXYZ.Min.Y + transform.Origin.Y, boundingBoxXYZ.Min.Z + transform.Origin.Z);
            XYZ max = new XYZ(boundingBoxXYZ.Max.X + transform.Origin.X, boundingBoxXYZ.Max.Y + transform.Origin.Y, boundingBoxXYZ.Max.Z + transform.Origin.Z);
            return new BoundingBoxXYZ() { Max = max, Min = min };
        }
        /// <summary>
        /// Chỉnh sửa CurveLoop khi có một cạnh nhỏ hơn chiều dài cho phép của Revit
        /// </summary>
        /// <param name="curveLoop"></param>
        /// <returns></returns>
        public static CurveLoop ToAdjustCurveLoop(this CurveLoop curveLoop, Document document)
        {
            List<Curve> curves = curveLoop.OfType<Curve>().ToRemoveDuplicateCurves();
            List<Curve> editCurves = new List<Curve>();
            foreach (Curve cure in curves)
            {
                if (cure.ApproximateLength > document.Application.ShortCurveTolerance)
                {
                    editCurves.Add(cure);
                    curves.Remove(cure);
                    break;
                }
            }
            foreach (Curve curve in curves)
            {
                if (curve.ApproximateLength > document.Application.ShortCurveTolerance)
                {
                    if (editCurves.Last().GetEndPoint(1).DistanceTo(curve.GetEndPoint(0)) < EPSILON.ToFeet())
                    {
                        editCurves.Add(curve);
                    }
                    else
                    {
                        if (curve is Line line)
                        {
                            editCurves.Add(Line.CreateBound(editCurves.Last().GetEndPoint(1), line.GetEndPoint(1)));
                        }
                        else
                        {
                            editCurves.Add(Arc.Create(editCurves.Last().GetEndPoint(1), curve.GetEndPoint(1), curve.Evaluate(0.5, true)));
                        }
                    }
                }
            }
            if (editCurves.Last().GetEndPoint(1).DistanceTo(editCurves.First().GetEndPoint(0)) > EPSILON.ToFeet())
            {
                if (editCurves.Last() is Line line)
                {
                    Line editLine = Line.CreateBound(line.GetEndPoint(0), editCurves.First().GetEndPoint(0));
                    editCurves.RemoveAt(editCurves.Count - 1);
                    editCurves.Add(editLine);
                }
                else
                {
                    Arc editArc = Arc.Create(editCurves.Last().GetEndPoint(0), editCurves.First().GetEndPoint(0), editCurves.Last().Evaluate(0.5, true));
                    editCurves.RemoveAt(editCurves.Count - 1);
                    editCurves.Add(editArc);
                }
            }
            return CurveLoop.Create(editCurves);
        }
        /// <summary>
        /// Xóa curve duplicate
        /// </summary>
        /// <param name="curves"></param>
        /// <returns></returns>
        public static List<Curve> ToRemoveDuplicateCurves(this IEnumerable<Curve> curves)
        {
            List<Curve> curvesClone = curves.ToList();
            List<Curve> curvesDuplicate = new List<Curve>();
            for (int i = 0; i < curvesClone.Count - 1; i++)
            {
                for (int j = i + 1; j < curvesClone.Count; j++)
                {
                    if (curvesClone[i].Intersect(curvesClone[j]) == SetComparisonResult.Equal)
                    {
                        curvesDuplicate.Add(curvesClone[j]);
                    }
                }
            }
            return curvesClone.Except(curvesDuplicate).ToList();
        }
        /// <summary>
        /// Tìm góc hợp giữa mặt phẳng và phương bất kì
        /// </summary>
        /// <param name="face"></param>
        /// <param name="direction"></param>
        /// <param name="abs"></param>
        /// <returns> Góc </returns>
        public static double AngleFaceWithDirection(Face face, XYZ direction, bool abs = true)
        {
            XYZ dir = face.ComputeNormal(new UV((face.GetBoundingBox().Min.U + face.GetBoundingBox().Max.U) / 2, (face.GetBoundingBox().Min.V + face.GetBoundingBox().Max.V) / 2));
            if (abs)
            {
                return Math.Abs(AngleBetweenTwoVectors(dir, direction, true));
            }
            else
            {
                return AngleBetweenTwoVectors(dir, direction, true);
            }
        }
    }
}