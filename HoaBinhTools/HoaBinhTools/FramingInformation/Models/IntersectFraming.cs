using System.Collections.Generic;
using System.Linq;
using Autodesk.Revit.DB;
using Utils;
using static HoaBinhTools.FramingInformation.Models.EnumSetting;


namespace HoaBinhTools.FramingInformation.Models
{
	public static class IntersectFraming
	{
		public static List<Element> GetElementIntersectFarming(this List<Element> eleFramings, Document doc)
		{
			List<Element> eleInters = new List<Element>();

			foreach (var eleFraming in eleFramings)
			{
				List<Element> eleIntersect;

				// Lay List A chac Chan Co

				// nếu chon project
				if (ViewSetting.project == (ViewSetting)HoaBinhTools.Properties.Settings.Default.ViewSetting)
				{
					eleIntersect = eleFraming.IntersectBounbingbox(doc);
				}
				else
				{
					eleIntersect = eleFraming.IntersectBounbingbox(doc, doc.ActiveView);
				}

				foreach (Element e in eleIntersect)
				{
					if (e.Category == null) continue;

					if (e.Category.Id.IntegerValue == (int)BuiltInCategory.OST_StructuralFoundation ||
						  e.Category.Id.IntegerValue == (int)BuiltInCategory.OST_Walls ||
						 ((e.Category.Id.IntegerValue == (int)BuiltInCategory.OST_StructuralFraming) && ((e is FamilyInstance) ? !((e as FamilyInstance).Symbol.Family.IsInPlace) : false)) ||
						  e.Category.Id.IntegerValue == (int)BuiltInCategory.OST_StructuralColumns)
					{
						if (e.IsCollision(eleFraming) == true && !eleFramings.Select(ex => ex.Id).Contains(e.Id))
						{
							eleInters.Add(e);
						}
					}
				}
			}
			return eleInters.Distinct(new ComparerElementID()).ToList();
		}



		public static List<Element> IntersectContiuousBeam(this Element eleFraming, Document doc)
		{
			List<Element> eleInters = new List<Element>();


			List<Element> eleIntersect;

			// Lay List A chac Chan Co

			// nếu chon project

			eleIntersect = eleFraming.IntersectBounbingbox(doc);


			foreach (Element e in eleIntersect)
			{
				if (e.Category == null) continue;

				if (e.Category.Id.IntegerValue == (int)BuiltInCategory.OST_StructuralFoundation ||
					  e.Category.Id.IntegerValue == (int)BuiltInCategory.OST_Walls ||
					   ((e.Category.Id.IntegerValue == (int)BuiltInCategory.OST_StructuralFraming) && ((e is FamilyInstance) ? !((e as FamilyInstance).Symbol.Family.IsInPlace) : false)) ||
					  e.Category.Id.IntegerValue == (int)BuiltInCategory.OST_StructuralColumns)
				{
					if (e.IsCollision(eleFraming) == true && eleFraming.Id != e.Id)
					{
						eleInters.Add(e);
					}
				}
			}

			return eleInters.Distinct(new ComparerElementID()).ToList();
		}





		public static XYZ GetPointIntersect(this List<Solid> S1, Solid S2)
		{
			List<XYZ> pointInterser = new List<XYZ>();

			XYZ centerPoint = XYZ.Zero;

			List<Face> Fs1 = S1.GetFacesToSoild();

			List<Edge> Ed2 = S2.GetEdgesToSoild();

			foreach (Face F in Fs1)
			{
				foreach (Edge e in Ed2)
				{

					if (F.Intersect(e.AsCurve(), out IntersectionResultArray results) == SetComparisonResult.Overlap)
					{
						pointInterser.Add(results.get_Item(0).XYZPoint);
					}
				}
			}

			List<Face> Fs2 = S2.GetFacesToSoild();

			List<Edge> Ed1 = S1.GetEdgesToSoild();

			foreach (Face F in Fs2)
			{
				foreach (Edge e in Ed1)
				{
					if (F.Intersect(e.AsCurve(), out IntersectionResultArray results) == SetComparisonResult.Overlap)
					{
						pointInterser.Add(results.get_Item(0).XYZPoint);
					}
				}
			}

			pointInterser.Distinct(new XYZEqualityComparer()).ToList();

			pointInterser.ForEach(e => centerPoint += e);

			if (pointInterser.Count > 0) centerPoint = centerPoint / pointInterser.Count;

			return centerPoint;

		}
	}
}
