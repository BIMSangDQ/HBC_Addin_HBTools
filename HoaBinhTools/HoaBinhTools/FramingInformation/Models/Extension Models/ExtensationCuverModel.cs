using System.Collections.Generic;
using Autodesk.Revit.DB;
using Utils;
namespace HoaBinhTools.FramingInformation.Models
{
	public static class CuverModel
	{
		public static Curve GetLocationCurve(this Element ele)
		{
			if (ele is Wall w)
			{
				return (w.Location as LocationCurve).Curve;
			}
			else
			{
				FamilyInstance beam = ele as FamilyInstance;

				try
				{
					if (HoaBinhTools.Properties.Settings.Default.MaxLength)
					{
						return beam.GetMaxCurve();
					}
					// lấy min
					else
					{
						return beam.GetCenterCurve();
					}
				}
				catch
				{
					return (beam.Location as LocationCurve).Curve.Clone();
				}

			}
		}


		public static Curve GetCenterCurve(this FamilyInstance Fami)
		{
			var solid = Fami.GetOriginSolidFramingSweptGeometry(ActiveData.Document);

			double Maxlength = double.MaxValue;

			double Minlength = double.MinValue;

			Curve Cur = null;

			foreach (Edge Ed in solid.GetEdgesToSoild())
			{
				Curve C = Ed.AsCurve();

				if (C.Length > Minlength)
				{
					Cur = C;

					Minlength = C.Length;
				}
			}

			Curve CurTop = null;

			var Topfaces = solid.GetFacesToSoild().GetTopFace();

			foreach (EdgeArray Edg in Topfaces.EdgeLoops)
			{

				foreach (Edge Ed in Edg)
				{
					Curve cr = Ed.AsCurve();

					if (cr.Length < Maxlength)
					{
						CurTop = cr;

						Maxlength = cr.Length;
					}

				}

			}

			var XYZTop = CurTop.Evaluate(0.5, true);

			var XYZCur = Cur.Project(XYZTop).XYZPoint;

			var Trans = Transform.CreateTranslation(XYZTop - XYZCur);

			return Cur.CreateTransformed(Trans);
		}





		public static Curve GetMaxCurve(this FamilyInstance Fami)
		{
			var solid = Fami.GetOriginSolidFramingSweptGeometry(ActiveData.Document);

			double Maxlength = 0;

			Curve Cur = null;

			foreach (Edge Ed in solid.GetEdgesToSoild())
			{
				Curve C = Ed.AsCurve();

				if (C.Length > Maxlength)
				{
					Cur = C;

					Maxlength = C.Length;
				}
			}

			var CurveCurrent = Cur.Evaluate(0.5, true);

			var XYZTopFace = solid.GetFacesToSoild().GetTopFace().Project(CurveCurrent).XYZPoint;

			var Trans = Transform.CreateTranslation(XYZTopFace - CurveCurrent);

			return Cur.CreateTransformed(Trans);
		}




		public static Curve GetTrimLocationCurve(this Element ele, Document doc, List<Element> ListEle)
		{
			FamilyInstance beam = ele as FamilyInstance;

			Curve c = (beam.Location as LocationCurve).Curve.Clone();

			try
			{
				var SolidFilter = c.GetEndPoint(0).CreateGlobular(1).UnionSoild(c.GetEndPoint(1).CreateGlobular(1));

				var List = SolidFilter.GetElementIntersectsSolid(ListEle, doc);

				Solid SolidCut = UtilsSolid.GetSolidZero();

				foreach (var e in List)
				{
					SolidCut = SolidCut.UnionSoild(e.GetAllSolids().UnionSoilds());
				}

				return c.CurveTrim(SolidCut);
			}
			catch
			{
				return c;
			}
		}


		public static Curve GetcurveAdijusIntersect(this Curve locaCurve, XYZ p1, Solid soild, Plane planeWork)
		{
			var p2 = locaCurve.Evaluate(0.5, true);

			// dich leen 1 ti lo sat mep
			var trans = Transform.CreateTranslation((p1 - p2) * 1.15);

			var CurveTransf = locaCurve.CreateTransformed(trans);

			Curve curveInside = null;

			SolidCurveIntersectionOptions opSoild = new SolidCurveIntersectionOptions();

			opSoild.ResultType = SolidCurveIntersectionMode.CurveSegmentsInside;

			SolidCurveIntersection intersection1 = soild.IntersectWithCurve(CurveTransf, opSoild);

			// gối

			double length = 0;

			Curve Curvereturn = null;

			for (int n = 0; n < intersection1.SegmentCount; n++)
			{
				// calculate length of rebar that is inside the column
				curveInside = intersection1.GetCurveSegment(n);

				if (curveInside.Length > length)
				{
					Curvereturn = curveInside;

					length = curveInside.Length;
				}
			}

			if (Curvereturn != null)
			{
				// chiếu lên trùng đương location curve của dầm
				Curvereturn = Curvereturn.TransfCurve(locaCurve);



				//return Curvereturn.ProjectCurveToPlane(planeWork);
				intersection1.Dispose();

				return Curvereturn.ProjectCurveToPlane(planeWork);
			}
			else
			{
				intersection1.Dispose();

				return null;
			}
		}




		public static List<Curve> GetListBeamsSpanIntersect(this Curve locaCurve, Solid soild, XYZ Original, Plane planeWork)
		{
			List<Curve> CursBeamSpan = new List<Curve>();

			SolidCurveIntersectionOptions opSoild = new SolidCurveIntersectionOptions();

			opSoild.ResultType = SolidCurveIntersectionMode.CurveSegmentsOutside;

			locaCurve = locaCurve.ProjectCurveToPlane(planeWork);

			SolidCurveIntersection intersection1 = soild.IntersectWithCurve(locaCurve, opSoild);

			// gối

			for (int n = 0; n < intersection1.SegmentCount; n++)
			{
				// calculate length of rebar that is inside the column

				CursBeamSpan.Add(intersection1.GetCurveSegment(n));
			}

			intersection1.Dispose();

			return CursBeamSpan;
		}



		public static Curve CurveTrim(this Curve locaCurve, Solid soild)
		{
			Curve CursBeamSpan = null;

			double leght = 0;

			SolidCurveIntersectionOptions opSoild = new SolidCurveIntersectionOptions();

			opSoild.ResultType = SolidCurveIntersectionMode.CurveSegmentsOutside;

			SolidCurveIntersection intersection1 = soild.IntersectWithCurve(locaCurve, opSoild);

			// gối

			for (int n = 0; n < intersection1.SegmentCount; n++)
			{
				Curve c = intersection1.GetCurveSegment(n);

				// calculate length of rebar that is inside the column
				if (c.Length > leght)
				{
					CursBeamSpan = c;

					leght = c.Length;
				}
			}

			intersection1.Dispose();

			return CursBeamSpan;
		}




		public static Curve CutCurve(this Curve Cur, XYZ Point, XYZ Origin)
		{
			double ParaIntersCurve = (Cur.Project(Point).Parameter);

			Curve CurClone = Cur.Clone();

			double GetEndParameter;

			if (CurClone.GetEndPoint(0).DistanceTo(Origin) < CurClone.GetEndPoint(1).DistanceTo(Origin))
			{
				GetEndParameter = (CurClone.GetEndParameter(0));
			}
			else
			{
				GetEndParameter = (CurClone.GetEndParameter(1));
			}

			CurClone.MakeBound(GetEndParameter, ParaIntersCurve);

			return CurClone;

		}
	}
}
