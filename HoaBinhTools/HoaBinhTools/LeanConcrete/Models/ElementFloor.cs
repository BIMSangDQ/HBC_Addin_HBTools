using System.Collections.Generic;
using System.Linq;
using Autodesk.Revit.DB;
using HoangBinhTools.LeanConcrete.ViewModels;
//using Test;
using MoreLinq;
using Utils;

namespace HoaBinhTools.LeanConcrete.Models
{
	class ElementFloor
	{
		const double _eps = 1.0e-4;

		public Element Ele;

		public LeanConcreteViewModel LCVM;


		public ElementFloor(Element Ele, LeanConcreteViewModel LCVM)
		{
			this.Ele = Ele;

			this.LCVM = LCVM;


		}

		public void Execute()
		{
			List<Solid> Solis = (Ele).GetAllSolids().Select(SolidUtils.SplitVolumes).Flatten().Cast<Solid>().ToList();

			foreach (var Soli in Solis)
			{
				Level Lev = Ele.GetLevel();

				PlanarFace FaceBottom = Soli.GetBotFaceVector();

				List<CurveLoop> Loops = FaceBottom.GetEdgesAsCurveLoops().ToList();

				XYZ FaceNor = FaceBottom.FaceNormal;

				Loops.Sort(new ElementSortMax());

				var Cur = Loops.FirstOrDefault().GetCurveArrayOffset(LCVM.Offset, FaceNor);

				Floor BTL = ElementLeanConcrete.Create(Cur, LCVM.Fltype, Lev, FaceNor, Ele.Name);

				List<CurveArray> CursOp = Loops.ListCurverOpening();

				BTL.CreateOpening(CursOp);

			}

			//Solid Soli = Ele.GetAllSolids().First();

			//Level Lev = Ele.GetLevel();

			//PlanarFace FaceBottom = Soli.GetBotFace();

			//List<CurveLoop> Loops = FaceBottom.GetEdgesAsCurveLoops().ToList();

			//XYZ FaceNor = FaceBottom.FaceNormal;

			//Loops.Sort(new ElementSortMin());

			//foreach (var Loop in Loops)
			//{
			//    try
			//    {
			//        CurveArray CursArray = Loop.GetCurveArrayOffset(LCVM.Offset, FaceNor);

			//        var with = LCVM.Fltype.GetCompoundStructure().GetWidth();

			//        Solid ExtrusionSoild = GeometryCreationUtilities.CreateExtrusionGeometry(new List<CurveLoop>() { CursArray.CurveArrayLoopToCurve() }, FaceNor, with);

			//        ExtrusionSoild = ExtrusionSoild.GetSoildIntersect();

			//        ExtrusionSoild.DrawingSolid(ActiveData.Document);

			//        if (ExtrusionSoild.Volume < 0.01) continue;
			//    }
			//    catch
			//    {
			//    }

			//    if (!prog.Create(Loops.Count)) break;
			//

			//List<PlanarFace> newFaces = ExtrusionSoild.Faces.FaceArryToPlanarFaces();

			//var newBotFace = newFaces;

			//foreach (var i in newBotFace)
			//{

			//    if (i.FaceNormal.Z - (-1) < 0.01)

			//    {

			//        CurveLoop newCl = i.GetEdgesAsCurveLoops().MaxBy(x => x.GetExactLength()).FirstOrDefault();

			//        var Cu = newCl.CurveLoopToCurveArray();

			//        Floor BTL = ElementLeanConcrete.Create(Cu, LCVM.Fltype, Lev, FaceNor, Ele.Name);
			//    }
			//}

			//CurveLoop newCl = newBotFace.GetEdgesAsCurveLoops().MaxBy(x => x.GetExactLength()).FirstOrDefault();

			//var Cu = newCl.CurveLoopToCurveArray();

			//}

		}
	}
}
