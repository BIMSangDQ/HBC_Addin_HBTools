using System;
using System.Collections.Generic;
using System.Linq;
using Autodesk.Revit.DB;
using MoreLinq;
using Utils;

namespace HoaBinhTools.FramingInformation.Models
{
	public static class ContiuousBeamModels
	{

		public static bool TogetherIntersect(this Element Ele1, Element Ele2)
		{
			var IDs1 = Ele1.IntersectContiuousBeam(ActiveData.Document);

			IEnumerable<ElementId> IDs2 = Ele2.IntersectContiuousBeam(ActiveData.Document).Select(e => e.Id);

			foreach (Element e in IDs1)
			{
				if (IDs2.Contains(e.Id))
				{
					return true;
				}
			}

			return false;
		}


		public static List<List<Element>> GetGroupContiuousBeam(this List<Element> Eles)
		{
			// Lấy list level
			List<List<Element>> ListGroup = new List<List<Element>>();

			List<Element> Group;

			List<List<Element>> GroupComtiuousBeam = Eles.GroupRefLevelandStraight();

			foreach (List<Element> GroupBeam in GroupComtiuousBeam)
			{
				if (GroupBeam.Count == 0) continue;

				List<Element> Beams = GroupBeam.SortBeamContiuous();

				// giá trị đầu tiên luôn đúng
				Group = new List<Element>();

				if (!ListGroup.ContainsContiuousBeam(Beams[0]))
				{
					Group.Add(Beams[0]);
				}

				Element UnderReview = Beams[0];

				if (GroupBeam.Count <= 1)
				{
					ListGroup.Add(Group);
				}
				else
				{
					for (int i = 1; i <= Beams.Count - 1; i++)
					{
						if ((UnderReview.IsCollision(Beams[i]) || ((UnderReview.ClosestDistanceOf2Beam(Beams[i]) <= HoaBinhTools.Properties.Settings.Default.KhoangCach) && UnderReview.TogetherIntersect(Beams[i]))))
						{
							if (!ListGroup.ContainsContiuousBeam(Beams[i]))
							{
								Group.Add(Beams[i]);
							}

							UnderReview = Beams[i];
						}
						else
						{
							// nếu không phai liên tục với nhau thì add vào rồi xet lại từ đầu 
							if (Group.Count > 0)
							{
								ListGroup.Add(Group);
							}

							Group = new List<Element>();

							if (!ListGroup.ContainsContiuousBeam(Beams[i]))
							{
								Group.Add(Beams[i]);
							}
							// gán lại thông tin 

							UnderReview = Beams[i];
						}

						// Đên Đối tượng cuối cùng 
						if (i == Beams.Count - 1)
						{
							ListGroup.Add(Group);
						}
					}
				}

			}

			return ListGroup.Where(e => e.Count > 0).ToList();
		}


		public static bool ContainsContiuousBeam(this List<List<Element>> ListEles, Element e)
		{

			foreach (var ListEle in ListEles)
			{
				if (ListEle.Contains(e))
				{
					return true;
				}
			}

			return false;
		}


		public static List<Element> SortBeamContiuous(this List<Element> GroupBeam)
		{
			var Orgin = GroupBeam.OriginContinuousBeam();

			GroupBeam.Sort(new SortContiuousBeam(Orgin));

			return GroupBeam;
		}


		public static XYZ OriginContinuousBeam(this List<Element> Els)
		{
			List<XYZ> XYZs = new List<XYZ>();

			foreach (var e in Els)
			{
				var Loca = e.GetLocationCurve();

				XYZs.Add(Loca.GetEndPoint(0));

				XYZs.Add(Loca.GetEndPoint(1));
			}

			double length = 0;

			XYZ Origin = null;

			for (int i = 0; i < XYZs.Count; i++)
			{
				for (int j = i; j < XYZs.Count; j++)
				{
					double lengthBeam = XYZs[i].DistanceTo(XYZs[j]);

					if (lengthBeam > length)
					{
						length = lengthBeam;

						Origin = XYZs[i];
					}
				}
			}
			return Origin;
		}


		public static List<List<Element>> GroupRefLevelandStraight(this List<Element> Eles)
		{
			List<List<Element>> GroupComtiuousBeam = new List<List<Element>>();

			List<Element> DepotEle = new List<Element>();

			foreach (var Ele in Eles)
			{
				if (!DepotEle.Contains(Ele))
				{
					List<Element> Contiuous = Eles.GetListContiuousBeam(Ele, GroupComtiuousBeam);

					DepotEle = DepotEle.Concat(Contiuous).ToList();

					GroupComtiuousBeam.Add(Contiuous);
				}
			}

			DepotEle.Clear();

			return GroupComtiuousBeam;
		}


		public static List<Element> GetListContiuousBeam(this List<Element> Eles, Element Ele2, List<List<Element>> GroupComtiuousBeam)
		{
			List<Element> EleReturn = new List<Element>();

			var DepotGroupEle = GroupComtiuousBeam.Flatten().Cast<Element>();

			foreach (var Ele1 in Eles)
			{
				if (DepotGroupEle.Contains(Ele1)) continue;
				// thẳng hang // cùng cao độ // Cùng mặt cắt //

				var Eleva = Math.Abs(Math.Abs((double)Ele1.GetBeamElevationTop()) - Math.Abs((double)Ele2.GetBeamElevationTop()));

				if (Ele1.IsStraight(Ele2) && (Ele1.GetReferenceLevel() == Ele2.GetReferenceLevel()) && Ele1.EqualSection(Ele2) && (Eleva <= HoaBinhTools.Properties.Settings.Default.GiatCap))
				{
					EleReturn.Add(Ele1);
				}
			}
			return EleReturn;
		}


		// khoang cách ngán nhất 2 con dầm
		public static double ClosestDistanceOf2Beam(this Element Ele1, Element Ele2)
		{
			Curve cur1 = Ele1.GetLocationCurve();

			Curve cur2 = Ele2.GetLocationCurve();

			double a = cur1.GetEndPoint(0).DistanceTo(cur2.GetEndPoint(0)).FootToMm();

			double b = cur1.GetEndPoint(0).DistanceTo(cur2.GetEndPoint(1)).FootToMm();

			double c = cur1.GetEndPoint(1).DistanceTo(cur2.GetEndPoint(0)).FootToMm();

			double d = cur1.GetEndPoint(1).DistanceTo(cur2.GetEndPoint(1)).FootToMm();

			return (a < b & a < c && a < d) ? a : ((b < c) && (b < d)) ? b : (c < d) ? c : d;
		}


		public static bool EqualSection(this Element Ele1, Element Ele2)
		{
			var Fami1 = Ele1 as FamilyInstance;

			var Fami2 = Ele2 as FamilyInstance;

			if (Ele1 != null && Fami2 != null)
			{
				var WH1 = Fami1.GetWidthAndHight(ActiveData.Document);

				var WH2 = Fami2.GetWidthAndHight(ActiveData.Document);

				var Sec = (WH1.Item1 * WH1.Item2) - (WH2.Item1 * WH2.Item2);

				if (Sec <= HoaBinhTools.Properties.Settings.Default.ToleranceSection)
				{
					return true;
				}
				else
				{
					return false;
				}
			}

			return false;
		}


		// Cùng phương
		public static bool IsStraight(this Element Ele1, Element Ele2)
		{
			var plane = Plane.CreateByNormalAndOrigin(XYZ.BasisZ, XYZ.Zero);

			var Loca1 = Ele1.GetLocationCurve().ProjectCurveToPlane(plane);

			var Loca2 = Ele2.GetLocationCurve().ProjectCurveToPlane(plane);

			XYZ D1, D2 = null;

			try
			{
				var A = Loca1.GetEndPoint(0);

				var B = Loca1.GetEndPoint(1);

				var C = Loca2.GetEndPoint(0);

				var D = Loca2.GetEndPoint(1);

				// A làm goc
				if (A.DistanceTo(C) > B.DistanceTo(C))
				{
					D1 = Line.CreateBound(A, C).Direction;

					D2 = Line.CreateBound(A, D).Direction;
				}
				else
				{
					D1 = Line.CreateBound(B, C).Direction;

					D2 = Line.CreateBound(B, D).Direction;
				}
			}
			catch
			{
				D1 = Loca1.Direction();
				D2 = Loca2.Direction();
			}

			bool tam = ((Math.Abs(1 - Math.Abs(D1.DotProduct(D2)))).IsZero()) && ((D1.AngleBetweenTwoVectors(D2, false)) < HoaBinhTools.Properties.Settings.Default.GocLechGiahaiDam);

			return tam;
		}
	}
}
