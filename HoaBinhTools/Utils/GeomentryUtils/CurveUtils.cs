using System;
using System.Collections.Generic;
using System.Linq;
using Autodesk.Revit.DB;
using MoreLinq;

namespace Utils
{
	public static class CurveUtils
	{

		public static List<Line> GetWidthHeightFraming(FamilyInstance framing, Document doc)
		{
			Curve framingLocation = (framing.Location as LocationCurve).Curve;

			var planarFaces = framing.GetSolidsFromOriginalFamilyInstance().Faces;

			List<PlanarFace> pF = new List<PlanarFace>();

			foreach (var item in planarFaces)
			{
				pF.Add(item as PlanarFace);

			}

			pF = pF.Where(e => e.FaceNormal.AngleBetweenTwoVectors(XYZ.BasisZ, false) > 45).ToList();



			foreach (PlanarFace planarFace in pF)
			{
				Transform transform = framingLocation.ComputeDerivatives(0, true);

				if (transform.BasisX.AngleBetweenTwoVectors(planarFace.FaceNormal, false) < 5)
				{


					CurveLoop curveLoop = planarFace.GetEdgesAsCurveLoops().MaxBy(x => x.GetExactLength()).FirstOrDefault();

					if (curveLoop.IsRectangular(curveLoop.GetPlane()))
					{
						IEnumerable<Line> lines = curveLoop.Select(x => x).OfType<Line>();

						Line width = lines.Where(y => y.Direction.AngleBetweenTwoVectors(XYZ.BasisZ, false) > 45).FirstOrDefault();

						Line height = lines.Where(y => y.Direction.AngleBetweenTwoVectors(XYZ.BasisZ, false) < 45).FirstOrDefault();

						if (null != width && null != height)
						{
							return new List<Line>() { width, height };
						}
					}
				}
			}
			return null;
		}

		// Loại bỏ các curve song song trùng nhau để vẽ thép
		//List Curve dạng polyline trong cad
		public static IList<Curve> RutGonCurves(IList<Curve> cv)
		{
			IList<Curve> curves = new List<Curve>();

			int Xp = 0;

			XYZ spnt = new XYZ(0, 0, 0);
			Line linexp = Line.CreateBound(new XYZ(0, 0, 0), new XYZ(1, 1, 1));

			for (int i = 0; i < cv.Count; i++)
			{
				if (Xp == i && i == 0)
				{
					spnt = cv[i].GetEndPoint(0);
					linexp = cv[i] as Line;
				}
				else if (Xp == i && i != 0)// Vẽ Line đoạn trước
				{

				}
				else // Các nhịp sau nhịp xp
				{
					//Check nếu thẳng thì bỏ qua
					if (DistancePoint2Line(cv[i].GetEndPoint(0), linexp) == 0 && DistancePoint2Line(cv[i].GetEndPoint(1), linexp) == 0) //Nếu mà 2 điểm của curve sau đều  nằm trên đường thẳng thì cứ bỏ qua đoạn này
					{

					}
					else // Nếu mà đoạn thẳng sau có dấu hiện bẻ thì vẽ đoạn trước 
					{
						Line line = Line.CreateBound(spnt, cv[i].GetEndPoint(0));
						curves.Add(line);

						Xp = i;
						spnt = cv[i].GetEndPoint(0);
					}
				}
			}

			//Vẽ segment cuối cùng
			Line line1 = Line.CreateBound(spnt, cv[cv.Count - 1].GetEndPoint(1));
			curves.Add(line1);

			return curves;
		}

		public static Curve ProjectCurveToPlane(this Curve curve, Plane plane)
		{

			var p1 = curve.GetEndPoint(0).ProjectOnto(plane);

			var p5 = curve.GetEndPoint(1).ProjectOnto(plane);


			var p3 = (p1 + p5) / 2;
			p3 = curve.Project(p3).XYZPoint.ProjectOnto(plane);


			var p2 = (p3 + p1) / 2;
			p2 = curve.Project(p2).XYZPoint.ProjectOnto(plane);

			var p4 = (p3 + p5) / 2;
			p4 = curve.Project(p4).XYZPoint.ProjectOnto(plane);



			var Points = new List<XYZ>() { p1, p2, p3, p4, p5 };

			return NurbSpline.CreateCurve(HermiteSpline.Create(Points, false));
		}



		//public static Curve ProjectLineToPlane(this Curve curve, Plane plane)
		//{
		//    var p1 = curve.GetEndPoint(0).ProjectOnto(plane);

		//    var p2 = curve.GetEndPoint(1).ProjectOnto(plane);

		//    return Line.CreateBound(p1, p2);
		//}



		public static Line CreateLine(this XYZ p1, XYZ p2)
		{
			return Line.CreateBound(p1, p2);
		}

		public static Line CreateLinePointAndDirection(this XYZ p, XYZ direction)
		{
			return Line.CreateBound(p, p.Add(direction));
		}

		public static XYZ SP(this Curve curve)
		{
			return curve.Tessellate()[0];
		}

		public static XYZ EditZ(this XYZ p, double z)
		{
			return new XYZ(p.X, p.Y, z);
		}

		public static XYZ EP(this Curve curve)
		{
			IList<XYZ> xyzList = curve.Tessellate();
			return xyzList[xyzList.Count - 1];
		}

		public static XYZ GetIntersectPoint(this Curve curve1, Curve curve2)
		{
			XYZ endPoint1 = curve1.GetEndPoint(0);
			XYZ endPoint2 = curve1.GetEndPoint(1);
			XYZ endPoint3 = curve2.GetEndPoint(0);
			XYZ endPoint4 = curve2.GetEndPoint(1);
			XYZ source1 = endPoint2 - endPoint1;
			XYZ source2 = endPoint3 - endPoint1;
			double num1 = source1.DotProduct(source2) / (source1.GetLength() * source2.GetLength());
			XYZ xyz1 = endPoint1 + source2.GetLength() * num1 * source1.Normalize() - endPoint3;
			double num2 = xyz1.DotProduct(source1) / (xyz1.GetLength() * source1.GetLength());
			double length = xyz1.GetLength();
			XYZ source3 = endPoint4 - endPoint3;
			double num3 = xyz1.DotProduct(source3) / (xyz1.GetLength() * source3.GetLength());
			XYZ xyz2 = endPoint3 + length / num3 * source3.Normalize();
			XYZ xyz3 = xyz2 - endPoint1;
			(xyz3.CrossProduct(source1) / (xyz3.GetLength() * source1.GetLength())).GetLength();
			return xyz2;
		}

		public static double Distance2Lines(this Line l1, Line l2)
		{
			return l1.GetEndPoint(0).DistancePoint2Line(l2);
		}

		public static double DistancePoint2Line(this XYZ p, Line line)
		{
			var endPoint = line.GetEndPoint(0);
			var direction = line.Direction.Normalize();
			var d = Math.Abs((p - endPoint).DotProduct(direction));
			return Math.Sqrt(p.DistanceTo(endPoint) * p.DistanceTo(endPoint) - d * d);
		}
		public static XYZ ProjectPoint2Line(this XYZ p, Line line)
		{
			var endPoint = line.GetEndPoint(0);
			var vector1 = p - endPoint;
			var direction = line.Direction.Normalize();
			return endPoint.Add(vector1.DotProduct(direction) * direction);
		}
		public static bool IsAlmostInside(this Line l1, Line l2, double tol)
		{
			var flag = false;
			var p0 = l1.GetEndPoint(0).ProjectPoint2Line(l2);
			var p1 = l1.GetEndPoint(1).ProjectPoint2Line(l2);
			if (p0.IsPointInsideLine(l2, tol) && p1.IsPointInsideLine(l2, tol))
			{
				flag = true;
			}
			var p00 = l2.GetEndPoint(0).ProjectPoint2Line(l1);
			var p11 = l2.GetEndPoint(1).ProjectPoint2Line(l1);
			if (p00.IsPointInsideLine(l1, tol) && p11.IsPointInsideLine(l1, tol))
			{
				flag = true;
			}
			return flag;
		}
		public static bool IsPointInsideLine(this XYZ C, Line line, double tol)
		{
			var A = line.GetEndPoint(0);
			var B = line.GetEndPoint(1);
			var AC = C - A;
			var AB = B - A;
			var BC = C - B;
			if (AC.IsAlmostEqualTo(XYZ.Zero, 0.001))
			{
				return true;
			}
			else
			{
				var cross = AC.CrossProduct(AB);
				if (cross.GetLength() < 0.001)
				{
					if ((AC.GetLength() + BC.GetLength()).RevitEquals(AB.GetLength(), tol))
					{
						return true;
					}
				}
				else
				{
					return false;
				}
			}
			return false;
		}
		public static Line ProjectLine2Line(this Line l1, Line l2)
		{
			var a = l1.GetEndPoint(0);
			var b = l1.GetEndPoint(1);
			a = a.ProjectPoint2Line(l2);
			b = b.ProjectPoint2Line(l2);
			return Line.CreateBound(a, b);
		}
		public static bool IsEqual(this Curve curve1, Curve curve2)
		{
			var u1 = curve1.Direction();
			var u2 = curve2.Direction();
			var l1 = curve1.Length;
			var l2 = curve2.Length;
			var mid1 = curve1.Midpoint();
			var mid2 = curve2.Midpoint();
			if (u1.IsParallel(u2) && l1 == l2 && mid1.IsAlmostEqualTo(mid2))
			{
				return true;
			}
			return false;
		}
		public static XYZ GetProjectPointOnCurve(this Curve curve, XYZ point)
		{
			XYZ xyz1 = new XYZ();
			XYZ endPoint = curve.GetEndPoint(0);
			XYZ xyz2 = curve.GetEndPoint(1) - endPoint;
			XYZ source = point - endPoint;
			if (source.GetLength() < 1E-05)
				return endPoint;
			if (source.GetLength() > 1E-05)
			{
				double num = xyz2.DotProduct(source) / (xyz2.GetLength() * source.GetLength());
				xyz1 = endPoint + source.GetLength() * num * xyz2.Normalize();
			}
			return xyz1;
		}
		public static double DistanceFromPointToCurve(XYZ point, Curve curve)
		{
			XYZ projectPointOnCurve = GetProjectPointOnCurve(curve, point);
			return (point - projectPointOnCurve).GetLength();
		}
		public static bool IsPointInsideCurve(this Curve curve, XYZ point, double epsilon)
		{
			bool flag = false;
			XYZ endPoint1 = curve.GetEndPoint(0);
			XYZ endPoint2 = curve.GetEndPoint(1);
			XYZ xyz1 = point - endPoint1;
			XYZ xyz2 = point - endPoint2;
			if (xyz1.GetLength() < epsilon || xyz2.GetLength() < epsilon)
				return true;
			if (MathUtils.RevitEquals(curve.Length, xyz1.GetLength() + xyz2.GetLength(), epsilon))
				flag = true;
			return flag;
		}
		public static bool IsEndPointOfCurve(this XYZ p, Curve curve, double tol)
		{
			var p0 = curve.SP();
			var p1 = curve.EP();
			if (p0.IsAlmostEqualTo(p, tol) || p1.IsAlmostEqualTo(p, tol))
			{
				return true;
			}
			return false;
		}
		public static bool IsSameSide(Curve curve, XYZ firstPoint, XYZ secondPoint)
		{
			bool flag = false;
			XYZ projectPointOnCurve1 = GetProjectPointOnCurve(curve, firstPoint);
			XYZ projectPointOnCurve2 = GetProjectPointOnCurve(curve, secondPoint);
			double num = (projectPointOnCurve1 - firstPoint).DotProduct(projectPointOnCurve2 - secondPoint);
			if (num < 0.0)
				flag = false;
			if (num > 0.0)
				flag = true;
			return flag;
		}
		public static Curve Join2Curve(this Curve c1, Curve c2)
		{

			var a = c1.SP();
			var b = c1.EP();
			var c = c2.SP();
			var d = c2.EP();
			var m1 = (a - c).GetLength();
			var m2 = (a - b).GetLength();
			var m3 = (b - c).GetLength();
			var m4 = (b - d).GetLength();
			var list = new List<double>() { m1, m2, m3, m4 };
			var max = list.Max();
			if (m1 == max)
			{
				c1 = Line.CreateBound(a, c);
			}
			if (m2 == max)
			{
				c1 = Line.CreateBound(a, d);
			}
			if (m3 == max)
			{
				c1 = Line.CreateBound(b, c);
			}
			if (m4 == max)
			{
				c1 = Line.CreateBound(b, d);
			}
			return c1;
		}
		public static List<Curve> RemoveCurve(this List<Curve> curves, Curve curve)
		{
			var list = new List<Curve>();
			foreach (var c in curves)
			{
				if (c.IsEqual(curve)) continue;
				list.Add(c);
			}
			return list;
		}

		public static Arc CircleByPointAndRadius(this XYZ p, Plane plane, double r)
		{
			p = p.ProjectOnto(plane);
			return Arc.Create(p, r, 0, Math.PI * 2, plane.XVec, plane.YVec);
		}

		public static Line CreateLine(this XYZ sp, XYZ direction, double? length = null)
		{
			if (length == null)
			{
				length = 1;
			}
			var ep = sp.Add(direction * (double)length);
			return Line.CreateBound(sp, ep);
		}

		public static XYZ IntersectLines(XYZ ps1, XYZ pe1, XYZ ps2, XYZ pe2)
		{
			double num = pe1.Y - ps1.Y;
			double num2 = ps1.X - pe1.X;
			double num3 = num * ps1.X + num2 * ps1.Y;
			double num4 = pe2.Y - ps2.Y;
			double num5 = ps2.X - pe2.X;
			double num6 = num4 * ps2.X + num5 * ps2.Y;
			double num7 = num * num5 - num4 * num2;
			if (num7.RevitEquals(0.0))
			{
				return null;
			}
			return new XYZ((num5 * num3 - num2 * num6) / num7, (num * num6 - num4 * num3) / num7, ps1.Z);
		}
	}
}
