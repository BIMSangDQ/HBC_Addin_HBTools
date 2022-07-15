using System;

namespace Utility
{
	public static class EqualUtil
	{
		const double Precision = 0.00001;    //precision when judge whether two doubles are equal
		public static bool IsEqual(this double d1, double d2)
		{
			//get the absolute value;
			double diff = Math.Abs(d1 - d2);
			return diff < Precision;
		}
		public static bool IsEqual(this Autodesk.Revit.DB.XYZ first, Autodesk.Revit.DB.XYZ second)
		{
			bool flag = true;
			flag = flag && IsEqual(first.X, second.X);
			flag = flag && IsEqual(first.Y, second.Y);
			flag = flag && IsEqual(first.Z, second.Z);
			return flag;
		}
		public static bool IsEqualOrBigger(this double first, double second)
		{
			if (IsEqual(first, second)) return true;
			return first > second;
		}
		public static bool IsEqualOrBigger
			(this Autodesk.Revit.DB.XYZ first, Autodesk.Revit.DB.XYZ second)
		{
			if (IsEqual(first, second)) return true;
			if (IsEqual(first.Z, second.Z))
			{
				if (IsEqual(first.Y, second.Y))
				{
					return (first.X > second.X);
				}
				return (first.Y > second.Y);
			}
			return (first.Z > second.Z);
		}
		public static bool IsEqualOrSmaller(this double first, double second)
		{
			if (IsEqual(first, second)) return true;
			return first < second;
		}
		public static bool IsEqualOrSmaller
			(this Autodesk.Revit.DB.XYZ first, Autodesk.Revit.DB.XYZ second)
		{
			if (IsEqual(first, second)) return true;
			if (IsEqual(first.Z, second.Z))
			{
				if (IsEqual(first.Y, second.Y))
				{
					return (first.X < second.X);
				}
				return (first.Y < second.Y);
			}
			return (first.Z < second.Z);
		}
		public static bool IsEqual(this Autodesk.Revit.DB.UV first, Autodesk.Revit.DB.UV second)
		{
			bool flag = true;
			flag = flag && IsEqual(first.U, second.U);
			flag = flag && IsEqual(first.V, second.V);
			return flag;
		}
		public static bool IsEqual
			(this Autodesk.Revit.DB.Curve first, Autodesk.Revit.DB.Curve second)
		{
			if (IsEqual(first.GetEndPoint(0), second.GetEndPoint(0)))
			{
				return IsEqual(first.GetEndPoint(1), second.GetEndPoint(1));
			}
			if (IsEqual(first.GetEndPoint(1), second.GetEndPoint(0)))
			{
				return IsEqual(first.GetEndPoint(0), second.GetEndPoint(1));
			}
			return false;
		}
		public static bool IsSmaller
			(this Autodesk.Revit.DB.XYZ first, Autodesk.Revit.DB.XYZ second)
		{
			if (IsEqual(first, second)) return false;
			if (IsEqual(first.Z, second.Z))
			{
				if (IsEqual(first.Y, second.Y))
				{
					return (first.X < second.X);
				}
				return (first.Y < second.Y);
			}
			return (first.Z < second.Z);
		}
		public static bool IsSmaller(this double x, double y)
		{
			if (IsEqual(x, y)) return false;
			return x < y;
		}
		public static bool IsBigger
			(this Autodesk.Revit.DB.XYZ first, Autodesk.Revit.DB.XYZ second)
		{
			if (IsEqual(first, second)) return false;
			if (IsEqual(first.Z, second.Z))
			{
				if (IsEqual(first.Y, second.Y))
				{
					return (first.X > second.X);
				}
				return (first.Y > second.Y);
			}
			return (first.Z > second.Z);
		}
		public static bool IsBigger(this double first, double second)
		{
			if (IsEqual(first, second)) return false;
			return first > second;
		}
	}
}
