using System;

namespace Utility
{
	public static class UnitUtil
	{
		const double FEET_TO_METERS = 0.3048;
		const double FEET_TO_CENTIMETERS = FEET_TO_METERS * 100;
		const double FEET_TO_MILIMETERS = FEET_TO_METERS * 1000;
		public static double feet2Meter(this double feet)
		{
			return feet * FEET_TO_METERS;
		}
		public static double feet2MeterSquare(this double feet)
		{
			return feet * FEET_TO_METERS * FEET_TO_METERS;
		}
		public static double feet2MeterCubic(this double feet)
		{
			return feet * FEET_TO_METERS * FEET_TO_METERS * FEET_TO_METERS;
		}
		public static double feet2Meter(this int feet)
		{
			return feet * FEET_TO_METERS;
		}
		public static double feet2MeterSquare(this int feet)
		{
			return feet * FEET_TO_METERS * FEET_TO_METERS;
		}
		public static double feet2Centimeter(this double feet)
		{
			return feet * FEET_TO_CENTIMETERS;
		}
		public static double feet2CentimeterSquare(this double feet)
		{
			return feet * FEET_TO_CENTIMETERS * FEET_TO_CENTIMETERS;
		}
		public static double feet2CentimeterCubic(this double feet)
		{
			return feet * FEET_TO_CENTIMETERS * FEET_TO_CENTIMETERS * FEET_TO_CENTIMETERS;
		}
		public static double feet2Milimeter(this double feet)
		{
			return feet * FEET_TO_MILIMETERS;
		}
		public static double feet2MilimeterSquare(this double feet)
		{
			return feet * FEET_TO_MILIMETERS * FEET_TO_MILIMETERS;
		}
		public static double feet2MilimeterCubic(this double feet)
		{
			return feet * FEET_TO_MILIMETERS * FEET_TO_MILIMETERS * FEET_TO_CENTIMETERS;
		}
		public static double feet2Milimeter(this int feet)
		{
			return feet * FEET_TO_MILIMETERS;
		}
		public static double feet2MilimeterSquare(this int feet)
		{
			return feet * FEET_TO_MILIMETERS * FEET_TO_MILIMETERS;
		}
		public static double meter2Feet(this double meter)
		{
			return meter / FEET_TO_METERS;
		}
		public static double meter2FeetSquare(this double meter)
		{
			return meter / (FEET_TO_METERS * FEET_TO_METERS);
		}
		public static double meter2Feet(this int meter)
		{
			return meter / FEET_TO_METERS;
		}
		public static double meter2FeetSquare(this int meter)
		{
			return meter / (FEET_TO_METERS * FEET_TO_METERS);
		}
		public static double milimeter2Feet(this double milimeter)
		{
			return milimeter / FEET_TO_MILIMETERS;
		}
		public static double milimeter2FeetSquare(this double milimeter)
		{
			return milimeter / (FEET_TO_MILIMETERS * FEET_TO_MILIMETERS);
		}
		public static double milimeter2Feet(this int milimeter)
		{
			return milimeter / FEET_TO_MILIMETERS;
		}
		public static double milimeter2FeetSquare(this int milimeter)
		{
			return milimeter / (FEET_TO_MILIMETERS * FEET_TO_MILIMETERS);
		}
		public static double radian2Degree(this double rad)
		{
			return rad * 180 / Math.PI;
		}
		public static double radian2Degree(this int rad)
		{
			return rad * 180 / Math.PI;
		}
		public static double degree2Radian(this double deg)
		{
			return deg * Math.PI / 180;
		}
		public static double degree2Radian(this int deg)
		{
			return deg * Math.PI / 180;
		}

		public static int RoundUp(this double d)
		{
			return Math.Round(d, 0) < d ? (int)(Math.Round(d, 0) + 1) : (int)(Math.Round(d, 0));
		}
		public static int RoundDown(this double d)
		{
			return Math.Round(d, 0) < d ? (int)(Math.Round(d, 0)) : (int)(Math.Round(d, 0) - 1);
		}
	}
}
