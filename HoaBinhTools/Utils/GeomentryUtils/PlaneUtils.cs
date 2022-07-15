using System;
using Autodesk.Revit.DB;

namespace Utils
{
	public static class PlaneUtils
	{

		public static double SignedDistanceTo(this Plane plane, XYZ p)
		{
			XYZ v = p - plane.Origin;

			return Math.Abs(plane.Normal.DotProduct(v));
		}

		public static double SignedDistanceTo(this Plane plane, XYZ p, bool minus)
		{
			XYZ v = p - plane.Origin;
			if (minus)
			{
				return plane.Normal.DotProduct(v);
			}
			return Math.Abs(plane.Normal.DotProduct(v));
		}

		public static XYZ ProjectOnto(this Plane plane, XYZ p)
		{
			XYZ v = p - plane.Origin;
			double d = plane.Normal.DotProduct(v);
			XYZ q = p - d * plane.Normal;
			return q;
		}
		public static XYZ ProjectOnto(this XYZ p, Plane plane)
		{
			XYZ v = p - plane.Origin;
			double d = plane.Normal.DotProduct(v);
			XYZ q = p - d * plane.Normal;
			return q;
		}


		public static bool IsPointOnPlane(this Plane plane, XYZ point)
		{
			return Math.Abs(plane.SignedDistanceTo(point)) < 0.0001;
		}


		public static Plane ToPlane(this PlanarFace planarFace, Transform transform = null)
		{
			if (transform == null)
			{
				transform = Transform.Identity;
			}
			return Plane.CreateByNormalAndOrigin(transform.OfVector(planarFace.FaceNormal), transform.OfPoint(planarFace.Origin));
		}


		public static Plane ToPlane(this View view)
		{
			return Plane.CreateByNormalAndOrigin(view.ViewDirection, view.Origin);
		}


	}
}
