using System;
using Autodesk.Revit.DB;

namespace Utils
{
	public static class MathUtils
	{
		public static double EPSILON_VOLUME = 1E-09;

		public static double EPSILON = 0.001;

		public const double PI = Math.PI;


		// góc giữa 2 vecto này  luôn từ 0 -> 180 
		public static double AngleBetweenTwoVectors(this XYZ vectorOne, XYZ vectorTwo, bool absolute)
		{
			// nhan 2 vecto
			double Numerator = vectorOne.X * vectorTwo.X + vectorOne.Y * vectorTwo.Y + vectorOne.Z * vectorTwo.Z;

			// lấy góc hợp giữa chiều 2 vec tơ điểm đầu cùng nhau
			double Denominator = vectorOne.GetLength() * vectorTwo.GetLength();

			if (absolute)
			{

				return ((Math.Acos(Math.Round(Numerator / Denominator, 3))) * 180) / 3.14;
			}

			// luôn lấy goc nhỏ hơn 90
			else
			{
				return ((Math.Acos(Math.Round(Math.Abs(Numerator) / Denominator, 3))) * 180) / 3.14;
			}
		}



		public static bool RevitEquals(this double A, double B, double tolerance)
		{
			return Math.Abs(B - A) < tolerance;
		}

		public static bool RevitEquals(this double A, double B)
		{
			return Math.Abs(B - A) < 1E-06;
		}


		public static bool IsZero(this double A)
		{
			return Equals(0.0, A);
		}

		public static bool IsZeroTol(this double A, double tolerance)
		{
			return RevitEquals(0.0, A, tolerance);
		}

		public static bool IsSmallerTol(this double A, double B, double tolerance)
		{
			return A + tolerance < B;
		}

		public static bool IsSmallerEqualTol(this double A, double B, double tolerance)
		{
			if (A + tolerance >= B)
			{
				return Math.Abs(B - A) < tolerance;
			}
			return true;
		}

		public static bool IsSmaller(this double A, double B)
		{
			return A + 1E-06 < B;
		}

		public static bool IsSmallerEqual(this double A, double B)
		{
			if (A + 1E-06 >= B)
			{
				return Math.Abs(B - A) < 1E-06;
			}
			return true;
		}

		public static bool IsGreaterTol(this double A, double B, double tolerance)
		{
			return A > B + tolerance;
		}

		public static bool IsGreaterEqualTol(this double A, double B, double tolerance)
		{
			if (Math.Abs(B - A) >= tolerance)
			{
				return A > B + tolerance;
			}
			return true;
		}

		public static bool IsGreater(this double A, double B)
		{
			return A > B + 1E-06;
		}

		public static bool IsGreaterEqual(this double A, double B)
		{
			if (A <= B + 1E-06)
			{
				return Math.Abs(B - A) < 1E-06;
			}
			return true;
		}

		public static double Min(this double A, double B)
		{
			if (!IsSmaller(A, B))
			{
				return B;
			}
			return A;
		}

		public static double MinTol(this double A, double B, double tolerance)
		{
			if (!IsSmallerTol(A, B, tolerance))
			{
				return B;
			}
			return A;
		}

		public static double Max(this double A, double B)
		{
			if (!IsGreater(A, B))
			{
				return B;
			}
			return A;
		}

		public static double MaxTol(double A, double B, double tolerance)
		{
			if (!IsGreaterTol(A, B, tolerance))
			{
				return B;
			}
			return A;
		}

		public static double MmToFoot(this double mm)
		{
			return mm / 304.8;
		}

		public static double MmToFoot(this int mm)
		{
			return mm / 304.79999999999995;
		}

		public static double MeterToFoot(this double metter)
		{
			return metter / 0.30479999999999996;
		}

		public static double FootToMm(this double feet)
		{
			return feet * 304.79999999999995;
		}
		public static double FootToMet(this double feet)
		{
			return feet * 304.79999999999995 / 1000;
		}

		public static double CubicFootToCubicMeter(this double cubicFoot)
		{
			return cubicFoot * 0.02831684659199999;
		}

		public static double SquareFootToSquareMeter(this double squareFoot)
		{
			return squareFoot * 0.092903039999999978;
		}

		public static double RadiansToDegrees(double rads)
		{
			return rads * 57.295779513082323;
		}

		public static double DegreesToRadians(double degrees)
		{
			return degrees * 0.017453292519943295;
		}
	}
}
