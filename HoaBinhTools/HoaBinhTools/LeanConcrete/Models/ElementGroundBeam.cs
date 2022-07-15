using System.Collections.Generic;
using System.Linq;
using Autodesk.Revit.DB;
using HoangBinhTools.LeanConcrete.ViewModels;
using MoreLinq;
using Utils;
//using Test;


namespace HoaBinhTools.LeanConcrete.Models
{
	class ElementGroundBeam
	{
		const double _eps = 1.0e-4;

		public Element Ele;

		public LeanConcreteViewModel LCVM;

		public ElementGroundBeam(Element Ele, LeanConcreteViewModel LCVM)
		{
			this.Ele = Ele;

			this.LCVM = LCVM;
		}

		public void Execute()
		{
			List<Solid> Solis = (Ele as FamilyInstance).GetAllSolids().Select(SolidUtils.SplitVolumes).Flatten().Cast<Solid>().ToList();

			Level Lev = Ele.GetLevel();

			foreach (var Soli in Solis)
			{
				var faces = Soli.Faces.Flatten().Where(x => x is PlanarFace).Cast<PlanarFace>().ToList();

				//var FaceBottom = XYZ.BasisZ.FirstFace(faces, false);

				var FaceBottom = faces.GetBotFaceOrigin();

				var cls = FaceBottom.GetEdgesAsCurveLoops();

				// lấy đương curve max nhất: more linq
				var cl = cls.MaxBy(x => x.GetExactLength()).FirstOrDefault();

				//  List<CurveArray> CursOp = FaceBottom.GetCurveLoop().ListCurverOpening();

				XYZ FaceNor = FaceBottom.FaceNormal;

				CurveLoop CursArray = cl.GetCurveLoopOffset(LCVM.Offset, FaceNor);

				SolidOptions options = new SolidOptions(ElementId.InvalidElementId, ElementId.InvalidElementId);

				Solid ExtrusionSoild = GeometryCreationUtilities.CreateExtrusionGeometry(new List<CurveLoop>() { CursArray }, FaceNor, MathUtils.MmToFoot(5), options);

				try
				{
					XYZ beamDir = (Ele.Location as LocationCurve).Curve.Direction();

					// điểm chính giữa mặt phẳng
					XYZ center = FaceBottom.GetCenterPointOfFace();

					foreach (Curve cur in cl)
					{
						// kiểm tra có song song không
						if (cur.Direction().IsParallel(beamDir) == false)
						{
							XYZ normal = cur.Direction().CrossProduct(XYZ.BasisZ);

							// vecto giữa đường giống dầm và 
							XYZ vector = center - cur.Midpoint();

							double dot = normal.DotProduct(vector);

							// nếu lớn hơn 90 độ và be hơn 270 độ thì ngược dấu đổi chiều lại
							if (dot < 0)
							{
								normal = -normal;
							}
							// có thể lấy bien vector
							var plane = Plane.CreateByNormalAndOrigin(normal, cur.Midpoint());

							BooleanOperationsUtils.CutWithHalfSpaceModifyingOriginalSolid(ExtrusionSoild, plane);
						}
					}

					var newFaces = ExtrusionSoild.Faces.FaceArryToPlanarFaces();

					var newBotFace = newFaces.GetTopFace();

					CurveLoop newCl = newBotFace.GetEdgesAsCurveLoops().MaxBy(x => x.GetExactLength()).FirstOrDefault();

					var Curarry = newCl.CurveLoopToCurveArray();

					var Para = Ele.LookupParameter("HB_ElementName");

					string HB_Ele;

					if (Para != null)
					{
						HB_Ele = Para.AsString();
					}
					else
					{
						HB_Ele = "N/N";
					}

					Floor BTL = ElementLeanConcrete.Create(Curarry, LCVM.Fltype, Lev, FaceNor, HB_Ele);

				}
				catch
				{

					var Para = Ele.LookupParameter("HB_ElementName");

					string HB_Ele;

					if (Para != null)
					{
						HB_Ele = Para.AsString();
					}
					else
					{
						HB_Ele = "N/N";
					}

					Floor BTL = ElementLeanConcrete.Create(CursArray.CurveLoopToCurveArray(), LCVM.Fltype, Lev, FaceNor, HB_Ele);


				}
			}
		}
	}
}
