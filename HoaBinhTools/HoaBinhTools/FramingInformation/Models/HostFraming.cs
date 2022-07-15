using System;
using System.Collections.Generic;
using System.Linq;
using Autodesk.Revit.DB;
using Utils;
//using Test;
using static HoaBinhTools.FramingInformation.Models.EnumSetting;

namespace HoaBinhTools.FramingInformation.Models
{
	public class HostFraming
	{
		public string Name { get; set; }

		public double Section { get; set; }

		public ElementId ID { get; set; }

		public List<Solid> SolidOrgin { get; set; }

		public List<Element> ListHost { get; set; }

		public double Width { get; set; }

		public double Hight { get; set; }

		public XYZ OrginPoint { get; set; }

		public Curve TraightCurve { get; set; }

		public List<Element> Openings { get; set; }

		public double WithFloor { get; set; }

		public HostFraming(List<Element> eles, Document doc)
		{

			SolidOrgin = GetSoilContiuousBeam(eles, doc);

			TraightCurve = GetTraightCurve(eles);

			this.OrginPoint = GetSortPoint(TraightCurve);

			ListHost = eles;

			// sắp sếp tăng dần 

			ListHost.Sort(new SortContiuousBeam(OrginPoint));

			// lấy chiều cao diện tích cái dầm đầu tiên edit lại 
			var ObjectContiuousBeam = Get_Width_Hight_ID_CrossSectional(ListHost[0], doc);

			Width = ObjectContiuousBeam.Item1;

			Hight = ObjectContiuousBeam.Item2;

			ID = ObjectContiuousBeam.Item3;

			Section = ObjectContiuousBeam.Item4;

			Name = GetNameBeamMain(ListHost);

			Openings = GetOpenings(ListHost);
		}


		public List<Element> GetOpenings(List<Element> Eles)
		{
			var IDs = Eles.Select(e => e.Id);

			List<Element> OP1 = new List<Element>();

			foreach (var e in new FilteredElementCollector(ActiveData.Document).OfClass(typeof(Opening)).Where(e => e.Category.Id.IntegerValue != (int)BuiltInCategory.OST_ShaftOpening))
			{
				var OP = (e as Opening);

				if (OP.Host.Id == null) continue;


				if (IDs.Contains(OP.Host.Id))
				{
					if (e is Opening Op)

						OP1.Add(Op);
				}
			}

			foreach (Element Gener in new FilteredElementCollector(ActiveData.Document).OfClass(typeof(FamilyInstance)).OfCategory(BuiltInCategory.OST_GenericModel).ToList())
			{

				var EleType = ActiveData.Document.GetElement(Gener.GetTypeId());

				var Look = EleType.LookupParameter("HB_Void");

				if (Look == null) continue;

				if (Look.AsString() == "C" || Look.AsString() == "R")
				{
					OP1.Add(Gener);
				}

			}

			return OP1;
		}

		public List<Solid> GetSoilContiuousBeam(List<Element> ListHost, Document doc)
		{
			List<Solid> Solids = new List<Solid>();

			Solid union = null;

			foreach (var host in ListHost)
			{
				Solid solid = host.GetQuickSolidOrigin(doc);

				if (null != solid && 0 < solid.Faces.Size)
				{
					if (null == union)
					{
						union = solid;
					}
					else
					{
						try
						{
							union = BooleanOperationsUtils.ExecuteBooleanOperation(union, solid, BooleanOperationsType.Union);
						}
						catch
						{
							Solids.Add(solid);
						}
					}
				}
			}
			Solids.Add(union);
			return Solids;
		}


		public static Curve GetTraightCurve(List<Element> ListHost)
		{
			var plane = Plane.CreateByNormalAndOrigin(XYZ.BasisZ, ListHost[0].GetLocationCurve().GetEndPoint(0));

			List<XYZ> PointCurve = new List<XYZ>();

			foreach (var cur in ListHost)
			{
				var loca = cur.GetLocationCurve();

				PointCurve.Add(plane.ProjectOnto(loca.GetEndPoint(0)));

				PointCurve.Add(plane.ProjectOnto(loca.GetEndPoint(1)));
			}

			PointCurve = PointCurve.Distinct(new XYZEqualityComparer()).ToList();

			XYZ Vec = PointCurve.FirstOrDefault() - PointCurve.LastOrDefault();

			var Trans = Transform.CreateTranslation(Vec * 500);

			var orgin = Trans.OfPoint(PointCurve.FirstOrDefault());

			PointCurve.Sort(new SortPoint(orgin));

			return Line.CreateBound(PointCurve.FirstOrDefault(), PointCurve.LastOrDefault());

		}


		public Tuple<double, double, ElementId, double> Get_Width_Hight_ID_CrossSectional(Element ele, Document doc)
		{
			var TupeWithAndHight = (ele as FamilyInstance).GetWidthAndHight(doc);

			var section = TupeWithAndHight.Item1 * TupeWithAndHight.Item2;

			var Width = TupeWithAndHight.Item1;

			var Hight = TupeWithAndHight.Item2;

			var CrossSection = section;

			var ID = ele.Id;

			return Tuple.Create(Width, Hight, ID, CrossSection);
		}


		public string GetNameBeamMain(List<Element> HostBeamMain)
		{


			List<ElementId> ListID = new List<ElementId>();

			Dictionary<string, int> ListName = new Dictionary<string, int>();

			for (int i = 0; i < HostBeamMain.Count; i++)

			{
				Element element = ActiveData.Document.GetElement(HostBeamMain[i].Id);

				var Para = element.LookupParameter(HoaBinhTools.Properties.Settings.Default.NameHost);

				string ParaString;

				if (Para != null)
				{
					if (Para.AsString() == null)
					{
						ParaString = "Khong Xac Dinh";
					}
					else
					{
						ParaString = Para.AsString();
					}


					if (!ListName.ContainsKey(ParaString))
					{
						ListName.Add(ParaString, i);
					}


				}
				else
				{

					ListName.Add("Chua Gan Ten", i);

					break;
				}
			}

			string NameBem = "";
			foreach (var i in from pair in ListName orderby pair.Value ascending select pair)
			{
				if (NameBem == "")
				{
					NameBem = NameBem + i.Key;
				}
				else
				{
					NameBem = NameBem + "-" + i.Key;
				}
			}

			return NameBem;
		}




		public XYZ GetSortPoint(Curve Cur)
		{
			var planeX = Plane.CreateByNormalAndOrigin(XYZ.BasisY, XYZ.Zero);

			var planeY = Plane.CreateByNormalAndOrigin(XYZ.BasisX, XYZ.Zero);

			var CurXLenght = Cur.GetEndPoint(0).ProjectOnto(planeX).DistanceTo(Cur.GetEndPoint(1).ProjectOnto(planeX));

			var CurYLenght = Cur.GetEndPoint(0).ProjectOnto(planeY).DistanceTo(Cur.GetEndPoint(1).ProjectOnto(planeY));

			XYZ DirectionDetermination = null;

			// xét theo phương X
			if (CurXLenght > CurYLenght)
			{
				if ((Direction)HoaBinhTools.Properties.Settings.Default.Horizonl == Direction.Right)
				{
					DirectionDetermination = XYZ.BasisX;
				}

				// Phải
				if ((Direction)HoaBinhTools.Properties.Settings.Default.Horizonl == Direction.Left)
				{
					DirectionDetermination = -XYZ.BasisX;
				}

				XYZ P1 = Cur.GetEndPoint(0);

				XYZ P2 = Cur.GetEndPoint(1);

				// vecto p1p2
				XYZ P1P2 = P2 - P1;

				// nếu là dương thì cùng chiều
				if (P1P2.DotProduct(DirectionDetermination) > 0.01)
				{
					return Transform.CreateTranslation(-P1P2 * 50).OfPoint(P1);
				}
				// nếu được chiều
				else
				{
					return Transform.CreateTranslation(P1P2 * 50).OfPoint(P2);
				}

			}
			// xet theo phương Y
			else
			{

				if ((Direction)HoaBinhTools.Properties.Settings.Default.Vertical == Direction.Up)
				{
					DirectionDetermination = XYZ.BasisY;
				}

				// Phải
				if ((Direction)HoaBinhTools.Properties.Settings.Default.Vertical == Direction.Down)
				{
					DirectionDetermination = -XYZ.BasisY;
				}

				XYZ P1 = Cur.GetEndPoint(0);

				XYZ P2 = Cur.GetEndPoint(1);

				// vecto p1p2
				XYZ P1P2 = P2 - P1;

				// nếu là dương thì cùng chiều
				if (P1P2.DotProduct(DirectionDetermination) > 0.01)
				{
					return Transform.CreateTranslation(-P1P2 * 50).OfPoint(P1);
				}
				// nếu được chiều
				else
				{
					return Transform.CreateTranslation(P1P2 * 50).OfPoint(P2);

				}

			}
		}


	}
}
