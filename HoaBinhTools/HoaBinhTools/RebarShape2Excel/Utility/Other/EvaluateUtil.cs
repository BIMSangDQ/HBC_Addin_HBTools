using System;
using Autodesk.Revit.DB;

namespace Utility
{
	public static class EvaluateUtil
	{
		/// <summary>
		/// Mô phỏng một tọa độ 3d lên hệ trục địa phương của mặt phẳng
		/// </summary>
		/// <param name="plane">Mặt phẳng đang xét</param>
		/// <param name="point">Điểm 3d đang xét</param>
		/// <returns></returns>
		public static UV Evaluate(Plane plane, XYZ point)
		{
			if (!plane.IsPointInPlane(point)) point = plane.GetProjectPoint(point);
			Plane planeOx = Plane.CreateByOriginAndBasis(plane.Origin, plane.XVec, plane.Normal);
			Plane planeOy = Plane.CreateByOriginAndBasis(plane.Origin, plane.YVec, plane.Normal);
			double lenX = planeOy.GetSignedDistance(point);
			double lenY = planeOx.GetSignedDistance(point);
			for (int i = 0; i < 2; i++)
			{
				for (int j = 0; j < 2; j++)
				{
					double tLenX = lenX * Math.Pow(-1, i + 1);
					double tLenY = lenY * Math.Pow(-1, j + 1);
					XYZ tPoint = plane.Origin + plane.XVec * tLenX + plane.YVec * tLenY;
					if (tPoint.IsEqual(point)) return new UV(tLenX, tLenY);
				}
			}
			throw new Exception("Code complier should never be here!");
		}

		/// <summary>
		/// Mô phỏng một tọa độ 2d trên mặt phẳng đang xét thành tọa độ 3d
		/// </summary>
		/// <param name="p">Mặt phẳng đang xét</param>
		/// <param name="point">Điểm 2d đang xét</param>
		/// <returns></returns>
		public static XYZ Evaluate(Plane p, UV point)
		{
			XYZ pnt = p.Origin;
			pnt = pnt + p.XVec * point.U;
			pnt = pnt + p.YVec * point.V;
			return pnt;
		}

		/// <summary>
		/// Mô phỏng một tọa độ 3d lên hệ trục địa phương của mặt đang xét
		/// </summary>
		/// <param name="f">Mặt đang xét</param>
		/// <param name="point">Điểm 3d đang xét</param>
		/// <returns></returns>
		public static UV Evaluate(PlanarFace f, XYZ point)
		{
			return Evaluate(f.GetPlane(), point);
		}

		/// <summary>
		/// Mô phỏng một tọa độ 2d trên mặt đang xét thành tọa độ 3d
		/// </summary>
		/// <param name="f">Mặt đang xét</param>
		/// <param name="point">Điểm 2d đang xét</param>
		/// <returns></returns>
		public static XYZ Evaluate(PlanarFace f, UV point)
		{
			return f.Evaluate(point);
		}
	}
}
