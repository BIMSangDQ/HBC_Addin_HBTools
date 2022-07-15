using System.Collections.Generic;
using System.Linq;
using Autodesk.Revit.DB;
using Utils;
//using Test;

namespace HoaBinhTools.LeanConcrete.Models
{
	public static class ExtensionMethod
	{
		public static PlanarFace FirstFace(this XYZ vector, IList<PlanarFace> listFace, bool trueReference = true)
		{
			PlanarFace planarFace = null;

			//Lấy các face 
			IList<PlanarFace> planarFaceList = listFace.PerpendicularFace(vector);
			double d1 = 0.0;
			foreach (PlanarFace face in planarFaceList)
			{
				if (face.Reference == null && trueReference)
				{
					continue;
				}

				XYZ source = face.GetCenterPointOfFace();

				if (d1.RevitEquals(0.0, 0.0001))
				{
					d1 = vector.DotProduct(source);

					planarFace = face;
				}

				else
				{
					double num = vector.DotProduct(source);
					if (num < d1)
					{
						planarFace = face;
						d1 = num;
					}
				}
			}
			return planarFace;
		}


		public static List<PlanarFace> FaceArryToPlanarFaces(this FaceArray FaceArr)
		{
			List<PlanarFace> Flanars = new List<PlanarFace>();
			foreach (var i in FaceArr)
			{
				var Plans = i as PlanarFace;

				if (Plans != null) Flanars.Add(Plans);

			}
			return Flanars;
		}

		public static List<CurveLoop> GetCurveLoop(this PlanarFace FaceBottom)
		{
			List<CurveLoop> List_CuveLoop = FaceBottom.GetEdgesAsCurveLoops().ToList();

			List_CuveLoop.Sort(new ElementSortMax());

			return List_CuveLoop;
		}

		public static CurveArray GetCurveArrayOffset(this CurveLoop CuLoop, double offset, XYZ nor)
		{

			CurveLoop CuLoop_Offset = CurveLoop.CreateViaOffset(CuLoop, MathUtils.MmToFoot(offset), nor);

			// nếu offset ngược 
			if (CuLoop_Offset.GetExactLength() < CuLoop.GetExactLength())
			{
				CuLoop_Offset = CurveLoop.CreateViaOffset(CuLoop, MathUtils.MmToFoot(offset), -nor);
			}

			return CuLoop_Offset.CurveLoopToCurveArray();
		}

		public static CurveArray CurveLoopToCurveArray(this CurveLoop CuLoop_Offset)
		{
			CurveArray ListCurves = new CurveArray();
			foreach (Curve item in CuLoop_Offset)
			{
				ListCurves.Append(item);
			}
			return ListCurves;
		}

		public static CurveLoop CurveArrayLoopToCurve(this CurveArray CuLoop_Offset)
		{
			CurveLoop ListCurves = new CurveLoop();
			foreach (Curve item in CuLoop_Offset)
			{
				ListCurves.Append(item);
			}
			return ListCurves;
		}

		public static CurveLoop GetCurveLoopOffset(this CurveLoop CuLoop, double offset, XYZ nor)
		{

			CurveLoop CuLoop_Offset = CurveLoop.CreateViaOffset(CuLoop, MathUtils.MmToFoot(offset), nor);

			// nếu offset ngược 
			if (CuLoop_Offset.GetExactLength() < CuLoop.GetExactLength())
			{
				CuLoop_Offset = CurveLoop.CreateViaOffset(CuLoop, MathUtils.MmToFoot(offset), -nor);
			}

			return CuLoop_Offset;
		}



		public static List<CurveArray> ListCurverOpening(this List<CurveLoop> List_CurveLoop)
		{
			List<CurveArray> tmp = new List<CurveArray>();

			for (int i = 1; i < List_CurveLoop.Count(); i++)
			{
				CurveArray CurArry = new CurveArray();

				foreach (Curve curve in List_CurveLoop[i])
				{
					CurArry.Append(curve);
				}

				tmp.Add(CurArry);
			}
			return tmp;
		}


		public static Level GetLevel(this Element Elem)
		{
			Level Lv = null;

			if (Elem.LevelId != ElementId.InvalidElementId)
			{
				Lv = ActiveData.Document.GetElement(Elem.LevelId) as Level;
			}
			else
			{
				var levelId = Elem.get_Parameter(BuiltInParameter.INSTANCE_REFERENCE_LEVEL_PARAM).AsElementId();

				Lv = ActiveData.Document.GetElement(levelId) as Level;
			}

			return Lv;
		}


	}
}
