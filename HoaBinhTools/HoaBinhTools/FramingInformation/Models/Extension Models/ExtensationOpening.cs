using System;
using Autodesk.Revit.DB;
using Utils;

namespace HoaBinhTools.FramingInformation.Models.Extension_Models
{
	public static class ExtensationOpening
	{
		public static bool IsArcBoundary(this Opening open)
		{
			foreach (var item in open.BoundaryCurves)
			{
				if (item is Arc)
				{
					return true;
				}
			}
			return false;
		}

		public static Tuple<double, double> GetBxHInRectangle(this Opening open)
		{
			Curve B = null;

			Curve H = null;

			var Boundary = open.BoundaryCurves;

			foreach (Curve C in Boundary)
			{
				if (C.Direction().DotProduct(XYZ.BasisZ) < 0.01)
				{
					B = C;
				}
			}

			foreach (Curve C in Boundary)
			{
				if (C.Direction().DotProduct(B.Direction()) < 0.01)
				{
					H = C;
				}
			}

			return Tuple.Create(B.Length.FootToMm(), H.Length.FootToMm());
		}


		public static double GetRadiusOnOpening(this Opening open)
		{
			double R = 0;

			var Boundary = open.BoundaryCurves;

			foreach (Curve C in Boundary)
			{
				if (C is Arc Ar)
				{
					R += Ar.Radius;
				}
			}

			R = (R / Boundary.Size).FootToMm();

			return R;
		}





		public static double GetDistanceX(this Opening open, Curve cur, XYZ Orgin)
		{
			XYZ Center = GetCenterPointInBoundaryCurves(open.BoundaryCurves);

			return cur.CutCurve(Center, Orgin).Length.FootToMm();
		}





		public static double GetDistanceY(this Opening open, Curve cur)
		{
			XYZ Center = GetCenterPointInBoundaryCurves(open.BoundaryCurves);

			Plane PlaneBeam = Plane.CreateByNormalAndOrigin(XYZ.BasisZ, cur.Midpoint());

			var Y = PlaneBeam.SignedDistanceTo(Center).FootToMm();

			if (Center.Z < PlaneBeam.Origin.Z)
			{
				Y = -Y;
			}

			return Y;
		}



		public static double GetDistanceY(this XYZ Center, Curve cur)
		{
			Plane PlaneBeam = Plane.CreateByNormalAndOrigin(XYZ.BasisZ, cur.Midpoint());

			var Y = PlaneBeam.SignedDistanceTo(Center).FootToMm();

			if (Center.Z < PlaneBeam.Origin.Z)
			{
				Y = -Y;
			}

			return Y;
		}


		public static double GetDistanceX(this XYZ Center, Curve cur, XYZ Orgin)
		{
			return cur.CutCurve(Center, Orgin).Length.FootToMm();
		}


		public static XYZ GetCenterPointInBoundaryCurves(CurveArray Cur)
		{
			XYZ Center = XYZ.Zero;

			foreach (Curve cu in Cur)
			{
				if (cu is Line L)
				{
					Center += L.Midpoint();
				}
				if (cu is Arc Ar)
				{
					Center += Ar.Center;
				}

			}
			return (Center / Cur.Size);
		}


		public static XYZ GetCenterGeneric(this Element Gene)
		{

			try
			{
				XYZ Center = XYZ.Zero;

				int Count = 0;

				var Fami = Gene as FamilyInstance;

				Options op = new Options();

				op.View = ActiveData.Document.ActiveView;

				op.IncludeNonVisibleObjects = true;

				var Geo = Fami.get_Geometry(op);

				foreach (var ge in Geo)
				{
					if (ge is GeometryInstance GeoIn)
					{
						var GeoI = GeoIn.GetInstanceGeometry();

						foreach (var cu in GeoI)
						{

							if (cu is Line L)
							{
								Center += L.Midpoint();

								Count++;
							}
							if (cu is Arc Ar)
							{
								Center += Ar.Center;
								Count++;
							}

						}
					}
				}
				if (Center != XYZ.Zero)
				{
					return Center / Count;
				}
				else
				{
					return Gene.GetElementCenter();
				}

			}
			catch
			{
				return Gene.GetElementCenter();
			}

		}

	}
}
