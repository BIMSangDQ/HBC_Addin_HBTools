using System;
using System.Collections.Generic;
using Autodesk.Revit.DB;

namespace Utils
{
	public static class XYZUtils
	{
		private const double PrecisionComparison = 1E-06;

		public static XYZ GetCenterPointOfFace(this PlanarFace face)
		{
			XYZ xyz = new XYZ();
			Mesh mesh = face.Triangulate();
			int count = 0;
			foreach (XYZ vertex in mesh.Vertices)
			{
				xyz += vertex;

				count++;
			}

			XYZ centerpoint = xyz / count;

			return centerpoint;
		}

		public static XYZ GetCenterPointOfFace(this Face face)
		{
			XYZ xyz = new XYZ();
			Mesh mesh = face.Triangulate();
			int count = 0;
			foreach (XYZ vertex in mesh.Vertices)
			{
				xyz += vertex;

				count++;
			}

			XYZ centerpoint = xyz / count;

			return centerpoint;
		}




		public static bool IsPerpendicular(this XYZ v, XYZ w)
		{
			return 1E-09 < v.GetLength() && 1E-09 < w.GetLength() && 1E-09 > Math.Abs(v.DotProduct(w));
		}



		/// <summary>
		/// Kiểm tra 2 vecto có song song hay không đúng trả về true sai trả về false
		/// </summary>
		public static bool IsParallel(this XYZ p, XYZ q)
		{
			return p.CrossProduct(q).IsZeroLength();
		}


		private static double AngleBetweenTwoVectors(XYZ p, XYZ q, bool v)
		{
			throw new NotImplementedException();
		}

		public static bool IsVertical(this XYZ v, double tolerance)
		{
			if (MathUtils.RevitEquals(v.X, 0, tolerance))
				return MathUtils.RevitEquals(v.Y, 0, tolerance);
			return false;
		}
		public static XYZ GetClosestPoint(this XYZ pt, List<XYZ> pts)
		{
			XYZ xyz = new XYZ();
			double num1 = 0.0;
			foreach (XYZ pt1 in pts)
			{
				if (!pt.Equals((object)pt1))
				{
					double num2 = Math.Sqrt(Math.Pow(pt.X - pt1.X, 2.0) + Math.Pow(pt.Y - pt1.Y, 2.0) + Math.Pow(pt.Z - pt1.Z, 2.0));
					if (xyz.IsZeroLength())
					{
						num1 = num2;
						xyz = pt1;
					}
					else if (num2 < num1)
					{
						num1 = num2;
						xyz = pt1;
					}
				}
			}
			return xyz;
		}


		public static bool Iscontains(this XYZ point, List<XYZ> listPoint)
		{
			bool result = false;
			foreach (XYZ item in listPoint)
			{
				if (item.IsAlmostEqualTo(point))
				{
					result = true;
				}
			}
			return result;
		}

		public static bool IsOppositeDirectionTo(this XYZ vecThis, XYZ vecTo)
		{
			return MathUtils.Equals(-1.0, vecThis.Normalize().DotProduct(vecTo.Normalize()));
		}

		public static bool IsOrthogonalTo(this XYZ vecThis, XYZ vecTo)
		{
			return MathUtils.Equals(0.0, vecThis.Normalize().DotProduct(vecTo.Normalize()));
		}

		public static bool IsHorizontal(this XYZ vecThis)
		{
			return vecThis.IsPerpendicular(XYZ.BasisZ);
		}

		public static bool IsHorizontal(this XYZ vecThis, View view)
		{
			return vecThis.IsPerpendicular(view.UpDirection);
		}

		public static bool IsVertical(this XYZ vecThis)
		{
			return vecThis.IsPerpendicular(XYZ.BasisX);
		}
		public static bool IsVertical(this XYZ vecThis, View view)
		{
			return vecThis.IsPerpendicular(view.RightDirection);
		}

		public static XYZ GetElementCenter(this Element element)
		{
			BoundingBoxXYZ boundingBoxXYZ = element.get_BoundingBox((View)null);

			XYZ xYZ = boundingBoxXYZ.Max - boundingBoxXYZ.Min;

			return new XYZ(boundingBoxXYZ.Min.X + xYZ.X / 2.0, boundingBoxXYZ.Min.Y + xYZ.Y / 2.0, boundingBoxXYZ.Min.Z + xYZ.Z / 2.0);
		}



		public static XYZ ConvertCoordinateFromAPIUnitToCentimeterUnit(XYZ pointToConvert)
		{
			double x = UnitUtils.Convert(pointToConvert.X, DisplayUnitType.DUT_DECIMAL_FEET, DisplayUnitType.DUT_CENTIMETERS);
			double y = UnitUtils.Convert(pointToConvert.Y, DisplayUnitType.DUT_DECIMAL_FEET, DisplayUnitType.DUT_CENTIMETERS);
			double z = UnitUtils.Convert(pointToConvert.Z, DisplayUnitType.DUT_DECIMAL_FEET, DisplayUnitType.DUT_CENTIMETERS);
			return new XYZ(x, y, z);
		}

		public static XYZ ConvertCoordinateFromAPIUnitToMeterUnit(XYZ pointToConvert)
		{
			double x = UnitUtils.Convert(pointToConvert.X, DisplayUnitType.DUT_DECIMAL_FEET, DisplayUnitType.DUT_METERS);
			double y = UnitUtils.Convert(pointToConvert.Y, DisplayUnitType.DUT_DECIMAL_FEET, DisplayUnitType.DUT_METERS);
			double z = UnitUtils.Convert(pointToConvert.Z, DisplayUnitType.DUT_DECIMAL_FEET, DisplayUnitType.DUT_METERS);
			return new XYZ(x, y, z);
		}
		public static XYZ RotateRadians(this XYZ v, double radians)
		{
			var ca = Math.Cos(radians);
			var sa = Math.Sin(radians);
			return new XYZ(ca * v.X - sa * v.Y, sa * v.X + ca * v.Y, v.Z);
		}
		public static XYZ RotateDegree(this XYZ v, double degrees)
		{
			return v.RotateRadians(degrees * 0.0174532925);
		}


		/// <summary>
		/// Lấy vecto chỉ phương của đường cong
		/// </summary>
		public static XYZ Direction(this Curve curve)
		{
			return (curve.GetEndPoint(0) - curve.GetEndPoint(1)).Normalize();
		}

		/// <summary>
		/// Lấy vecto chỉ phương của cạnh
		/// </summary>
		public static XYZ Direction(this Edge edge)
		{
			var curve = edge.AsCurve();
			return curve.Direction();
		}

		public static XYZ GetClosestPt(XYZ pt, List<XYZ> pts)
		{
			XYZ xYZ = new XYZ();
			double num = 0.0;
			foreach (XYZ pt2 in pts)
			{
				if (!pt.Equals(pt2))
				{
					double num2 = Math.Sqrt(Math.Pow(pt.X - pt2.X, 2.0) + Math.Pow(pt.Y - pt2.Y, 2.0) + Math.Pow(pt.Z - pt2.Z, 2.0));
					if (xYZ.IsZeroLength())
					{
						num = num2;
						xYZ = pt2;
					}
					else if (num2 < num)
					{
						num = num2;
						xYZ = pt2;
					}
				}
			}
			return xYZ;
		}

		public static XYZ Intersection(Curve c1, Curve c2)
		{
			var l = c1 as Line;
			var ll = c2 as Line;
			IntersectionResultArray resultArray;
			if (Line.CreateBound(l.Origin + 10000.0 * l.Direction, l.Origin - 10000.0 * l.Direction).Intersect((Curve)Line.CreateBound(ll.Origin + 10000.0 * ll.Direction, ll.Origin - 10000.0 * ll.Direction), out resultArray) != SetComparisonResult.Overlap)
				throw new InvalidOperationException("Input lines did not intersect.");
			if (resultArray == null || resultArray.Size != 1)
				throw new InvalidOperationException("Could not extract line intersection point.");
			return resultArray.get_Item(0).XYZPoint;
		}

		/// <summary>
		/// Lấy điểm chính giữa 2 điểm
		/// </summary>
		public static XYZ Midpoint(this XYZ p, XYZ q)
		{
			return 0.5 * (p + q);
		}


		/// <summary>
		/// Lấy điểm giữa  đường Line
		/// </summary>
		public static XYZ Midpoint(this Line line)
		{
			return Midpoint(line.GetEndPoint(0), line.EP());
		}


		/// <summary>
		/// Lấy điểm chính giữa đường cong
		/// </summary>
		public static XYZ Midpoint(this Curve curve)
		{

			return Midpoint(curve.GetEndPoint(0), curve.GetEndPoint(1));
		}


		/// <summary>
		/// Lấy điểm chính giữa các cạnh
		/// </summary>
		public static XYZ Midpoint(this Edge edge)
		{
			var curve = edge.AsCurve();
			return Midpoint(curve.GetEndPoint(0), curve.EP());
		}



		public static List<XYZ> GetBottomCorners(BoundingBoxXYZ b)
		{
			double z = b.Min.Z;
			return new List<XYZ>
			{
				new XYZ(b.Min.X, b.Min.Y, z),
				new XYZ(b.Max.X, b.Min.Y, z),
				new XYZ(b.Max.X, b.Max.Y, z),
				new XYZ(b.Min.X, b.Max.Y, z)
			};
		}



		public static XYZ VectorHasZZero(this XYZ vector)
		{
			return new XYZ(vector.X, vector.Y, 0);
		}
		public static XYZ ModifyVector(this XYZ vector, double num, XyzEnum e)
		{
			var x = vector.X;
			var y = vector.Y;
			var z = vector.Z;
			if (e == XyzEnum.X)
			{
				x = num;
			}
			if (e == XyzEnum.Y)
			{
				y = num;
			}
			if (e == XyzEnum.Z)
			{
				z = num;
			}
			return new XYZ(x, y, z);
		}

		public static double Distance2Point(XYZ p1, XYZ p2)
		{
			double d = Math.Sqrt((p1.X - p2.X) * (p1.X - p2.X) + (p1.Y - p2.Y) * (p1.Y - p2.Y));
			return d;
		}

		public enum XyzEnum
		{
			X,
			Y,
			Z
		}


	}
}
