using Autodesk.Revit.DB;
using Utils;

namespace HoaBinhTools.FramingInformation.Models
{
	public static class TransformCurve
	{
		public static Curve TransfCurve(this Curve cur, Curve curBeam)
		{
			XYZ p1 = cur.Midpoint();

			XYZ origin = curBeam.Midpoint();

			XYZ direction = curBeam.Direction();

			Plane planA = Plane.CreateByNormalAndOrigin(XYZ.BasisZ, origin);

			XYZ normal = direction.CrossProduct(XYZ.BasisZ);

			Plane planB = Plane.CreateByNormalAndOrigin(normal, origin);

			XYZ p2 = planA.ProjectOnto(p1);

			p2 = planB.ProjectOnto(p2);

			Transform transf = Transform.CreateTranslation(p2 - p1);

			Curve curtransform = cur.CreateTransformed(transf);

			return curtransform;
		}

	}
}
