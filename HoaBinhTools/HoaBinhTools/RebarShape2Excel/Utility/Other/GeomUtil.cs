#region Namespaces
using System;
using System.Collections.Generic;
using Autodesk.Revit.DB;
#endregion

namespace Utility
{
	public static class GeometryUtil
	{
		public static bool IsSameDirection(this Autodesk.Revit.DB.XYZ firstVec, Autodesk.Revit.DB.XYZ secondVec)
		{
			Autodesk.Revit.DB.XYZ first = firstVec.Normalize();
			Autodesk.Revit.DB.XYZ second = secondVec.Normalize();
			double dot = first.DotProduct(second);

			var check = dot.IsEqual(1);
			return dot.IsEqual(1);
		}
		public static bool IsSameDirection(this UV firstVec, UV secondVec)
		{
			Autodesk.Revit.DB.UV first = firstVec.Normalize();
			Autodesk.Revit.DB.UV second = secondVec.Normalize();
			double dot = first.DotProduct(second);
			return dot.IsEqual(1);
		}
		public static bool IsSameOrOppositeDirection(this XYZ firstVec, XYZ secondVec)
		{
			Autodesk.Revit.DB.XYZ first = firstVec.Normalize();
			Autodesk.Revit.DB.XYZ second = secondVec.Normalize();

			// if the dot product of two unit vectors is equal to 1, return true
			double dot = first.DotProduct(second);
			return dot.IsEqual(1) || dot.IsEqual(-1);
		}
		public static bool IsSameOrOppositeDirection(this UV firstVec, UV secondVec)
		{
			Autodesk.Revit.DB.UV first = firstVec.Normalize();
			Autodesk.Revit.DB.UV second = secondVec.Normalize();

			// if the dot product of two unit vectors is equal to 1, return true
			double dot = first.DotProduct(second);
			return dot.IsEqual(1) || dot.IsEqual(-1);
		}
		public static bool IsOppositeDirection(this Autodesk.Revit.DB.XYZ firstVec, Autodesk.Revit.DB.XYZ secondVec)
		{
			// get the unit vector for two vectors
			Autodesk.Revit.DB.XYZ first = firstVec.Normalize();
			Autodesk.Revit.DB.XYZ second = secondVec.Normalize();
			// if the dot product of two unit vectors is equal to -1, return true
			double dot = first.DotProduct(second);
			return dot.IsEqual(-1);
		}
		public static bool IsOppositeDirection(this Autodesk.Revit.DB.UV firstVec, Autodesk.Revit.DB.UV secondVec)
		{
			// get the unit vector for two vectors
			Autodesk.Revit.DB.UV first = firstVec.Normalize();
			Autodesk.Revit.DB.UV second = secondVec.Normalize();

			// if the dot product of two unit vectors is equal to -1, return true
			double dot = first.DotProduct(second);
			return dot.IsEqual(-1);
		}
		public static bool IsPerpendicularDirection(this Autodesk.Revit.DB.XYZ firstVec, Autodesk.Revit.DB.XYZ secondVec)
		{
			double dot = firstVec.DotProduct(secondVec);
			return dot.IsEqual(0);
		}
		public static XYZ GetMiddlePoint(XYZ first, XYZ second)
		{
			return (first + second) / 2;
		}

		public static Autodesk.Revit.DB.XYZ TransformPoint(Autodesk.Revit.DB.XYZ point, Transform transform)
		{
			//get the coordinate value in X, Y, Z axis
			double x = point.X;
			double y = point.Y;
			double z = point.Z;

			//transform basis of the old coordinate system in the new coordinate system
			Autodesk.Revit.DB.XYZ b0 = transform.get_Basis(0);
			Autodesk.Revit.DB.XYZ b1 = transform.get_Basis(1);
			Autodesk.Revit.DB.XYZ b2 = transform.get_Basis(2);
			Autodesk.Revit.DB.XYZ origin = transform.Origin;

			//transform the origin of the old coordinate system in the new coordinate system
			double xTemp = x * b0.X + y * b1.X + z * b2.X + origin.X;
			double yTemp = x * b0.Y + y * b1.Y + z * b2.Y + origin.Y;
			double zTemp = x * b0.Z + y * b1.Z + z * b2.Z + origin.Z;

			return new Autodesk.Revit.DB.XYZ(xTemp, yTemp, zTemp);
		}
		public static Autodesk.Revit.DB.Curve TransformCurve(Autodesk.Revit.DB.Curve curve, Transform transform)
		{
			return Line.CreateBound(TransformPoint(curve.GetEndPoint(0), transform), TransformPoint(curve.GetEndPoint(1), transform));
		}

		public static Autodesk.Revit.DB.Curve OffsetCurve(Autodesk.Revit.DB.Curve c, Autodesk.Revit.DB.XYZ direction, double offset)
		{
			c = Line.CreateBound(c.GetEndPoint(0) + direction * offset,
				c.GetEndPoint(1) + direction * offset);
			return c;
		}

		public static List<Curve> OffsetListCurve(List<Curve> cs,
			Autodesk.Revit.DB.XYZ direction, double offset)
		{
			for (int i = 0; i < cs.Count; i++)
			{
				cs[i] = OffsetCurve(cs[i], direction, offset);
			}
			return cs;
		}

		public static XYZ GetIntersectionPoint2D(Line l1, Line l2)
		{
			XYZ p1 = l1.GetEndPoint(0), p2 = l1.GetEndPoint(1);
			XYZ p3 = l2.GetEndPoint(0), p4 = l2.GetEndPoint(1);

			// Get the segments' parameters.
			double dx12 = p2.X - p1.X;
			double dy12 = p2.Y - p1.Y;
			double dx34 = p4.X - p3.X;
			double dy34 = p4.Y - p3.Y;

			// Solve for t1 and t2
			double denominator = (dy12 * dx34 - dx12 * dy34);

			double t1 =
				((p1.X - p3.X) * dy34 + (p3.Y - p1.Y) * dx34)
					/ denominator;
			if (double.IsInfinity(t1))
			{
				// Parallel
				return null;
			}

			double t2 =
				((p3.X - p1.X) * dy12 + (p1.Y - p3.Y) * dx12)
					/ -denominator;

			// Find the point of intersection.
			return new XYZ(p1.X + dx12 * t1, p1.Y + dy12 * t1, p1.Z);
		}
		public static bool IsPointInLine2D(XYZ point, Line line)
		{
			XYZ p1 = line.GetEndPoint(0), p2 = line.GetEndPoint(1);
			if (point.IsEqual(p1) || point.IsEqual(p2)) return true;
			XYZ vec1 = point - p1, vec2 = point - p2;
			XYZ dir = line.Direction;
			if (!IsSameOrOppositeDirection(vec1, dir)) return false;
			if (!IsOppositeDirection(vec1, vec2)) return false;
			return true;
		}
		public static double GetAngle(this XYZ vec1, XYZ vec2)
		{
			var cross = vec1.CrossProduct(vec2);
			var dot = vec1.DotProduct(vec2);

			var angle = Math.Atan2(cross.GetLength(), dot);
			return angle;
		}

		public static double DistanceFromPointToLine(this Line line, XYZ point)
		{
			XYZ vector = line.Origin - point;
			double Denominator = vector.GetLength();
			XYZ crossProduct = vector.CrossProduct(line.Direction);
			double Numerator = crossProduct.GetLength();
			return Numerator / Denominator;
		}
		public static double GetAngle(this XYZ targetVec, XYZ vecX, XYZ vecY)
		{
			vecX = vecX.Normalize(); vecY = vecY.Normalize();
			XYZ vecZ = vecX.CrossProduct(vecY);
			double dot = vecZ.DotProduct(targetVec);

			if (!vecX.DotProduct(vecY).IsEqual(0))
				throw new Exception("Two basis is not perpecular!");
			if (!vecX.CrossProduct(vecY).DotProduct(targetVec).IsEqual(0))
			{
				throw new Exception("TargetVector is not planar with two basis!");
			}
			double angle = 0;
			double angle1 = vecX.AngleTo(targetVec);
			double angle2 = Math.PI - angle1;
			XYZ assVec = vecX * Math.Cos(angle1) + vecY * Math.Sin(angle1);
			if (IsSameDirection(assVec, targetVec))
			{
				angle = angle1;
			}
			else if (IsOppositeDirection(assVec, targetVec))
			{
				angle = -angle2;
			}
			else
			{
				assVec = vecX * Math.Cos(angle2) + vecY * Math.Sin(angle2);
				if (IsSameDirection(assVec, targetVec))
				{
					angle = angle2;
				}
				else if (IsOppositeDirection(assVec, targetVec))
				{
					angle = -angle1;
				}
				else throw new Exception("The code should never go here!");
			}
			return angle;
		}
		public static XYZ GetVector(this double angle, XYZ vecX, XYZ vecY)
		{
			return (vecX + vecY * Math.Tan(angle)).Normalize();
		}
		public static XYZ GetPositionVector(this XYZ targetVec, XYZ vecX, XYZ vecY)
		{
			XYZ vec = null;
			double ang = GetAngle(targetVec, vecX, vecY);
			if (Math.Abs(ang).IsEqual(Math.PI / 2) || ang < Math.PI / 2)
			{
				vec = targetVec;
			}
			else if (ang > Math.PI / 2)
			{
				vec = -targetVec;
			}
			return vec;
		}
		/// <summary>
		/// Lấy góc của Vector tham biến so với mặt phẳng VecX và VecY
		/// </summary>
		/// <param name="targetVec"></param>
		/// <param name="vecX"></param>
		/// <param name="vecY"></param>
		/// <param name="vecZ"></param>
		/// <returns></returns>
		public static double GetAngle2(this XYZ targetVec, XYZ vecX, XYZ vecY, XYZ vecZ)
		{
			vecX = vecX.Normalize(); vecY = vecY.Normalize(); vecZ = vecZ.Normalize();
			if (!vecX.DotProduct(vecY).IsEqual(0)) throw new Exception("Two basis X and Y is not perpecular!");
			if (!vecX.CrossProduct(vecY).IsEqual(vecZ)) throw new Exception("Three basises are not a coordiante!");
			Plane pl = Plane.CreateByOriginAndBasis(XYZ.Zero, vecX, vecY);
			XYZ pjPnt = pl.GetProjectPoint(targetVec);
			return GetAngle(targetVec, pjPnt, vecZ);
		}

		/// <summary>
		/// Lấy góc của Vector tham biến so với VecZ
		/// </summary>
		/// <param name="targetVec"></param>
		/// <param name="vecX"></param>
		/// <param name="vecY"></param>
		/// <param name="vecZ"></param>
		/// <returns></returns>
		public static double GetAngle3(this XYZ targetVec, XYZ vecX, XYZ vecY, XYZ vecZ)
		{
			vecX = vecX.Normalize(); vecY = vecY.Normalize(); vecZ = vecZ.Normalize();
			if (!vecX.DotProduct(vecY).IsEqual(0)) throw new Exception("Two basis X and Y is not perpecular!");
			if (!vecX.CrossProduct(vecY).IsEqual(vecZ)) throw new Exception("Three basises are not a coordiante!");
			Plane pl = Plane.CreateByOriginAndBasis(XYZ.Zero, vecX, vecY);
			XYZ pjPnt = pl.GetProjectPoint(targetVec);
			return GetAngle(targetVec, vecZ, pjPnt);
		}

		/// <summary>
		/// Lấy mặt phẳng của mặt đang xét
		/// </summary>
		/// <param name="f">Mặt đang xét</param>
		/// <returns>Mặt phẳng trả về</returns>
		public static Plane GetPlane(this PlanarFace f)
		{
			return Plane.CreateByOriginAndBasis(f.Origin, f.XVector, f.YVector);
		}

		/// <summary>
		/// Lấy mặt phẳng của mặt đang xét với trục Ox
		/// </summary>
		/// <param name="f">Mặt đang xét</param>
		/// <param name="vecX">Trục Ox của mặt phẳng</param>
		/// <returns></returns>
		public static Plane GetPlaneWithBasisX(PlanarFace f, XYZ vecX)
		{
			if (!vecX.DotProduct(f.FaceNormal).IsEqual(0))
				throw new Exception("VecX is not perpendicular with Normal!");
			return Plane.CreateByOriginAndBasis
				(f.Origin, vecX, vecX.CrossProduct(f.FaceNormal).Normalize());
		}

		/// <summary>
		/// Lấy mặt phẳng của mặt đang xét với trục Oy
		/// </summary>
		/// <param name="f">Mặt đang xét</param>
		/// <param name="vecY">Trục Oy của mặt phẳng</param>
		/// <returns></returns>
		public static Plane GetPlaneWithBasisY(PlanarFace f, XYZ vecY)
		{
			if (!vecY.DotProduct(f.FaceNormal).IsEqual(0))
				throw new Exception("VecY is not perpendicular with Normal!");
			return Plane.CreateByOriginAndBasis
				(f.Origin, vecY.CrossProduct(f.FaceNormal), vecY);
		}

		/// <summary>
		/// Lấy tất cả các đường thẳng trong xác định mặt đang xét
		/// </summary>
		/// <param name="f">Mặt đang xét</param>
		/// <returns></returns>
		public static List<Curve> GetCurves(PlanarFace f)
		{
			List<Curve> curves = new List<Curve>();
			IList<CurveLoop> curveLoops = f.GetEdgesAsCurveLoops();
			foreach (CurveLoop cl in curveLoops)
			{
				foreach (Curve c in cl)
				{
					curves.Add(c);
				}
				break;
			}
			return curves;
		}

		/// <summary>
		/// Lấy khoảng cách từ môt điểm đến một mặt phẳng
		/// </summary>
		/// <param name="plane">Mặt phẳng đang xét</param>
		/// <param name="point">Điểm đang xét</param>
		/// <returns></returns>
		public static double GetSignedDistance(this Plane plane, XYZ point)
		{
			XYZ v = point - plane.Origin;
			return Math.Abs(plane.Normal.DotProduct(v));
		}

		/// <summary>
		/// Lấy khoảng cách từ một điểm đến một đường thẳng
		/// </summary>
		/// <param name="line">Đường thằng đang xét</param>
		/// <param name="point">Điểm đang xét</param>
		/// <returns></returns>
		public static double GetSignedDistance(this Line line, XYZ point)
		{
			if (line.IsPointInLineOrExtend(point)) return 0;
			return (point - line.GetProjectPoint(point)).GetLength();
		}

		/// <summary>
		/// Lấy khoảng cách từ một điểm đến một đường thẳng
		/// </summary>
		/// <param name="line">Đường thẳng đang xét</param>
		/// <param name="point">Điểm đang xét</param>
		/// <returns></returns>
		public static double GetSignedDistance(this Curve line, XYZ point)
		{
			if (line.Convert2Line().IsPointInLineOrExtend(point)) return 0;
			return (point - line.GetProjectPoint(point)).GetLength();
		}

		/// <summary>
		/// Kiểm tra một điểm nằm trên đường thẳng hay không
		/// </summary>
		/// <param name="line">Đường thằng đang xét</param>
		/// <param name="point">Điểm đang xét</param>
		/// <returns></returns>
		public static bool IsPointInLineOrExtend(this Line line, XYZ point)
		{
			if (point.IsEqual(line.GetEndPoint(0)) || point.IsEqual(line.GetEndPoint(1)))
				return true;
			if ((point - line.GetEndPoint(0)).IsSameOrOppositeDirection(point - line.GetEndPoint(1)))
				return true;
			return false;
		}

		/// <summary>
		/// Chuyển đổi từ kiểu dữ liệu curve đang line
		/// </summary>
		/// <param name="c">Kiểu dữ liệu Curve đang xét</param>
		/// <returns></returns>
		public static Line Convert2Line(this Curve c)
		{
			return Line.CreateBound(c.GetEndPoint(0), c.GetEndPoint(1));
		}

		/// <summary>
		/// Kiểm tra một điểm nằm trên mặt phẳng hay không
		/// </summary>
		/// <param name="plane">Mặt phẳng đang xét</param>
		/// <param name="point">Điểm đang xét</param>
		/// <returns></returns>
		public static bool IsPointInPlane(this Plane plane, XYZ point)
		{
			return plane.GetSignedDistance(point).IsEqual(0);
		}

		/// <summary>
		/// Kiểm tra một điểm nằm trên đoạn thẳng hay không
		/// </summary>
		/// <param name="line">Đoạn thẳng đang xét</param>
		/// <param name="point">Điểm đang xét</param>
		/// <returns></returns>
		public static bool IsPointInLine(Line line, XYZ point)
		{
			if (point.IsEqual(line.GetEndPoint(0)) || point.IsEqual(line.GetEndPoint(1)))
				return true;
			if ((point - line.GetEndPoint(0)).IsOppositeDirection(point - line.GetEndPoint(1)))
				return true;
			return false;
		}

		/// <summary>
		/// Chuyển đổi kiểu dữ liệu từ string sang XYZ
		/// Giá trị nhập vào phải thỏa các điều kiện nhất định
		/// </summary>
		/// <param name="pointString">Giá trị string đang xét</param>
		/// <returns></returns>
		public static XYZ ConvertStringToXYZ(string pointString)
		{
			List<double> nums = new List<double>();
			foreach (string s in pointString.Split('(', ',', ' ', ')'))
			{
				double x = 0;
				if (double.TryParse(s, out x)) { nums.Add(x); }
			}
			return new XYZ(nums[0], nums[1], nums[2]);
		}

		/// <summary>
		/// Chuyển đổi từ kiểu dữ liệu string sang kiểu dữ liệu BoundingBoxXYZ
		/// </summary>
		/// <param name="bbString">Giá trị string đang xét phải thỏa các điều kiện nhất định</param>
		/// <returns></returns>
		public static BoundingBoxXYZ ConvertStringToBoundingBox(string bbString)
		{
			BoundingBoxXYZ bb = new BoundingBoxXYZ();
			string[] ss = bbString.Split(';');
			ss[0] = ss[0].Substring(1, ss[0].Length - 1);
			ss[1] = ss[1].Substring(0, ss[1].Length - 1);
			bb.Min = ConvertStringToXYZ(ss[0]);
			bb.Max = ConvertStringToXYZ(ss[1]);
			return bb;
		}

		/// <summary>
		/// Chuyển đổi từ kiểu dữ liệu BoundingBoxXYZ sang kiểu dữ liệu string
		/// </summary>
		/// <param name="bb">BoundingBoxXYZ đang xét</param>
		/// <returns></returns>
		public static string ConvertBoundingBoxToString(BoundingBoxXYZ bb)
		{
			return "{" + bb.Min.ToString() + ";" + bb.Max.ToString() + "}";
		}

		/// <summary>
		/// Chuyển đổi từ kiểu dữ liệu CurveLoop sang List<Curve>
		/// </summary>
		/// <param name="cl">CurveLoop đang xét</param>
		/// <returns></returns>
		public static List<Curve> ConvertCurveLoopToCurveList(this CurveLoop cl)
		{
			List<Curve> cs = new List<Curve>();
			foreach (Curve c in cl)
			{
				cs.Add(c);
			}
			return cs;
		}

		public static UV GetUVCoordinate(this XYZ targetVec, XYZ vecX, XYZ vecY)
		{
			vecX = vecX.Normalize(); vecY = vecY.Normalize();
			double len = targetVec.GetLength();
			double angle = GetAngle(targetVec, vecX, vecY);
			return new UV(Math.Cos(angle) * len, Math.Sin(angle) * len);
		}
	}
}
