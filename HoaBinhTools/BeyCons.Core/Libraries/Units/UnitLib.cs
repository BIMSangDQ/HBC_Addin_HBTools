#region Using
using System;
#endregion

namespace BeyCons.Core.Libraries.Units
{
    /// <summary>
    /// Chuyển đổi đơn vị
    /// </summary>
    public static class UnitLib
    {
        /// <summary>
        /// 1 feet bằng bao nhiêu mm
        /// </summary>
        private const double feetMeter = 0.3048;
        /// <summary>
        /// Chuyển đổi feet đến mm
        /// </summary>
        /// <param name="feet"></param>
        /// <returns></returns>
        public static double ToMilimeter(this double feet)
        {
            return feet * feetMeter * 1000;
        }
        /// <summary>
        /// Chuyển đổi từ feet đến cm
        /// </summary>
        /// <param name="feet"></param>
        /// <returns></returns>
        public static double ToCentimeter(this double feet)
        {
            return feet * feetMeter * 100;
        }
        /// <summary>
        /// Chuyển đổi từ feet đến m
        /// </summary>
        /// <param name="feet"></param>
        /// <returns></returns>
        public static double ToMeter(this double feet)
        {
            return feet * feetMeter;
        }
        /// <summary>
        /// Chuyển đổi mm đến feet
        /// </summary>
        /// <param name="milimeter"></param>
        /// <returns></returns>
        public static double ToFeet(this double milimeter)
        {
            return milimeter / (feetMeter * 1000);
        }
        /// <summary>
        /// Chuyển đổi m2 đến f2
        /// </summary>
        /// <param name="squareMeter"></param>
        /// <returns></returns>
        public static double ToSquareFeet(this double squareMeter)
        {
            return squareMeter / (feetMeter * feetMeter);
        }
        /// <summary>
        /// Chuyển đổi m3 đến v3
        /// </summary>
        /// <param name="volumeMeter"></param>
        /// <returns></returns>
        public static double ToCubicFeet(this double volumeMeter)
        {
            return volumeMeter / (feetMeter * feetMeter * feetMeter);
        }
        /// <summary>
        /// Chuyển đổi f2 đến m2
        /// </summary>
        /// <param name="squareFeet"></param>
        /// <returns></returns>
        public static double ToSquareMeter(this double squareFeet)
        {
            return squareFeet * feetMeter * feetMeter;
        }
        /// <summary>
        /// Chuyển đổi f3 đến m3
        /// </summary>
        /// <param name="cubicFeet"></param>
        /// <returns></returns>
        public static double ToVolumeMeter(this double cubicFeet)
        {
            return cubicFeet * feetMeter * feetMeter * feetMeter;
        }
        /// <summary>
        /// Chuyển đổi độ sang radian
        /// </summary>
        /// <param name="degree"></param>
        /// <returns></returns>
        public static double ToRadian(this double degree)
        {
            return degree * (Math.PI / 180);
        }
        /// <summary>
        /// Chuyển đổi radian đến độ
        /// </summary>
        /// <param name="radian"></param>
        /// <returns></returns>
        public static double ToDegree(this double radian)
        {
            return radian * (180 / (Math.PI));
        }
    }
}