using System.Collections.Generic;
using System.Linq;
using Autodesk.Revit.DB;
using HoangBinhTools.LeanConcrete.ViewModels;
using MoreLinq;
using Utils;

namespace HoaBinhTools.LeanConcrete.Models
{
	class ElementFoundation
	{


		public Element Ele;



		public LeanConcreteViewModel LCVM;

		public ElementFoundation(Element Ele, LeanConcreteViewModel LCVM)
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

				List<CurveLoop> Loops = FaceBottom.GetCurveLoop();

				List<CurveArray> CursOp = Loops.ListCurverOpening();

				XYZ FaceNor = FaceBottom.FaceNormal;

				CurveArray CursArray = Loops.FirstOrDefault().GetCurveArrayOffset(LCVM.Offset, FaceNor);

				Floor BTL = ElementLeanConcrete.Create(CursArray, LCVM.Fltype, Lev, FaceNor, Ele.Name);

				BTL.CreateOpening(CursOp);


			}

		}

	}
}
