using System.Collections.Generic;
using System.Linq;
using Autodesk.Revit.DB;
using HoangBinhTools.LeanConcrete.ViewModels;
using MoreLinq;
using Utils;

namespace HoaBinhTools.LeanConcrete.Models
{
	class ElementIsolatedFoundation
	{


		public Element Ele;

		public LeanConcreteViewModel LCVM;

		public ElementIsolatedFoundation(Element Ele, LeanConcreteViewModel LCVM)
		{
			this.Ele = Ele;

			this.LCVM = LCVM;
		}


		public void Execute()
		{
			Level Lev = Ele.GetLevel();

			List<Solid> Solis = (Ele as FamilyInstance).GetAllSolids().Select(SolidUtils.SplitVolumes).Flatten().Cast<Solid>().ToList();

			foreach (var Soli in Solis)
			{
				PlanarFace FaceBottom = Soli.GetBotFaceVector();

				List<CurveLoop> Loops = FaceBottom.GetCurveLoop();

				List<CurveArray> CursOp = Loops.ListCurverOpening();

				XYZ FaceNor = FaceBottom.FaceNormal;

				CurveArray CursArray = Loops.FirstOrDefault().GetCurveArrayOffset(LCVM.Offset, FaceNor);

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

				Floor BTL = ElementLeanConcrete.Create(CursArray, LCVM.Fltype, Lev, FaceNor, HB_Ele);

				BTL.CreateOpening(CursOp);
			}
		}

	}
}
