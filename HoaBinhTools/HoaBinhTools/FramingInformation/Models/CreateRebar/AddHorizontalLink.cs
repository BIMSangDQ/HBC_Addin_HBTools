using System.Collections.Generic;
using System.Linq;
using Autodesk.Revit.DB;
using Autodesk.Revit.DB.Structure;
using HoaBinhTools.FramingInformation.Db;
using Utils;

namespace HoaBinhTools.FramingInformation.Models.CreateRebar
{
	public class AddHorizontalLink
	{
		public void DrawHorizontalLine(List<MovableRestBeam> ListMovable, Document doc)
		{
			#region variable
			string rebarType = Save.Default.AddStirup_Dia;
			RebarBarType RebarBarType = new FilteredElementCollector(doc)
				.OfClass(typeof(RebarBarType))
				.Cast<RebarBarType>()
				.First(x => x.Name == rebarType);

			XYZ vectorX = XYZ.BasisX;
			XYZ vectorZ = XYZ.BasisZ;
			XYZ vectorY = XYZ.BasisY;

			RebarHookType HookStart = (from abcd in new FilteredElementCollector(doc).OfClass(typeof(RebarHookType)).Cast<RebarHookType>()
									   where abcd.Name == Save.Default.Link_HookStart
									   select abcd).First();

			RebarHookType HookEnd = (from abcd in new FilteredElementCollector(doc).OfClass(typeof(RebarHookType)).Cast<RebarHookType>()
									 where abcd.Name == Save.Default.Link_HookEnd
									 select abcd).First();

			RebarHookOrientation ho = RebarHookOrientation.Left;

			RebarHookOrientation hi = RebarHookOrientation.Left;

			int slT2 = ListMovable[0].FraInfoViewModels.SL_T2;

			int sl1 = 0;
			int sl2 = 0;
			int sl3 = 0;

			double tl = double.Parse(Save.Default.TL_top);

			double kc = MathUtils.MmToFoot(double.Parse(Save.Default.AddStirrup_Spc));

			double abvtop = MathUtils.MmToFoot(double.Parse(Save.Default.ConcreteCover));

			List<Line> LL = new List<Line>();

			List<XYZ> sectionPnts = new List<XYZ>
			{
				new XYZ( 0,0,0),
				new XYZ(1,1,1)
			};

			IList<Curve> curves = new List<Curve>();
			#endregion

			try
			{
				for (int i = 0; i < ListMovable.Count; i++)
				{
					MovableRestBeam movable = ListMovable[i];
					string id = movable.Id.ToString(); // Lấy id để truy xuất thông tin thép

					if (movable.Type.ToString() == "ND")
					{

						#region Thông tin cơ bản

						double b = MathUtils.MmToFoot((double)movable.Width);
						double h = MathUtils.MmToFoot((double)movable.Hight);

						Line Linenhipdam = movable.Curve as Line;
						double nhipdam = Linenhipdam.Length; //Chiều dài nhịp

						Element elem = doc.GetElement(movable.Id);
						Location loc = elem.Location;
						LocationCurve locCur = loc as LocationCurve;
						Curve curve = locCur.Curve;
						Line line = curve as Line;

						vectorX = line.Direction;
						vectorZ = XYZ.BasisZ;
						vectorY = vectorZ.CrossProduct(vectorX);

						XYZ pnt1 = Linenhipdam.GetEndPoint(0);
						XYZ pnt2 = pnt1 + b * vectorY;
						XYZ pnt3 = pnt1 + b * vectorY + h * vectorZ;

						Plane pl = Plane.CreateByThreePoints(pnt1, pnt2, pnt3);

						XYZ pnt = line.GetEndPoint(0);

						double dis = PlaneUtils.SignedDistanceTo(pl, pnt); // Kc từ đầu curve tới mp rải
						Parameter zJus = elem.LookupParameter("z Justification");
						int zPos = zJus.AsInteger();
						double x = 0;
						switch (zPos)
						{
							case 0: // Top   
								x = 0;
								break;
							case 1: // Center  
							case 2: // Origin  
								x = 0.5;
								break;
							case 3: // Bottom  
								x = h;
								break;
						}

						XYZ origin = pnt - vectorY * b / 2 - vectorZ * h * x + vectorX * dis; // vị trí đầu nhịp
																							  //origin = origin - vectorZ *
																							  //         (h - abvtop - MathUtils.MmToFoot(65)); // Đưa về đúng cao độ đặt thép rồi

						#endregion

						#region Reset Biến

						sl1 = 0;
						sl2 = 0;
						sl3 = 0;

						foreach (DbAddT2 span in movable.FraInfoViewModels.ListAddT2)
						{
							if (span.Vitri.ToString() == "1" && span.EleID == id)
							{
								sl1 = int.Parse(span.Count.ToString());
							}

							if (span.Vitri.ToString() == "2" && span.EleID == id)
							{
								sl2 = int.Parse(span.Count.ToString());
							}

							if (span.Vitri.ToString() == "3" && span.EleID == id)
							{
								sl3 = int.Parse(span.Count.ToString());
							}
						} //Lấy thông tin thép

						#endregion

						#region vẽ đoạn rải
						if (sl1 > 2 && sl2 > 2 && sl3 > 2)  // Vẽ đai C full nhịp
						{
							sectionPnts[0] = origin;
							sectionPnts[1] = origin + vectorX * nhipdam;
							Line line1 = Line.CreateBound(sectionPnts[0], sectionPnts[1]);
							LL.Add(line1);
						}
						else if (sl1 > 2 && sl2 > 2)
						{
							sectionPnts[0] = origin;
							sectionPnts[1] = origin + vectorX * nhipdam * (1 - tl);
							Line line1 = Line.CreateBound(sectionPnts[0], sectionPnts[1]);
							LL.Add(line1);
						}
						else if (sl1 > 2 && sl2 < 3)
						{
							sectionPnts[0] = origin;
							sectionPnts[1] = origin + vectorX * nhipdam * tl;
							Line line1 = Line.CreateBound(sectionPnts[0], sectionPnts[1]);
							LL.Add(line1);

							if (sl3 > 2)
							{
								sectionPnts[0] = origin + vectorX * nhipdam * (1 - tl);
								sectionPnts[1] = origin + vectorX * nhipdam;
								Line line2 = Line.CreateBound(sectionPnts[0], sectionPnts[1]);
								LL.Add(line2);
							}
						}
						else if (sl1 < 3)
						{
							if (sl2 > 2)
							{
								if (sl3 > 2)
								{
									sectionPnts[0] = origin + vectorX * nhipdam * tl;
									sectionPnts[1] = origin + vectorX * nhipdam * tl;
									Line line1 = Line.CreateBound(sectionPnts[0], sectionPnts[1]);
									LL.Add(line1);
								}
								else
								{
									sectionPnts[0] = origin + vectorX * nhipdam * tl;
									sectionPnts[1] = origin + vectorX * nhipdam * (1 - tl);
									Line line1 = Line.CreateBound(sectionPnts[0], sectionPnts[1]);
									LL.Add(line1);
								}
							}
							else if (sl3 > 2 && sl2 < 3)
							{
								sectionPnts[0] = origin + vectorX * nhipdam * (1 - tl);
								sectionPnts[1] = origin + vectorX * nhipdam;
								Line line2 = Line.CreateBound(sectionPnts[0], sectionPnts[1]);
								LL.Add(line2);
							}
						}

						#endregion

						#region Vẽ đai C

						foreach (Line V in LL)
						{
							double len = V.Length;

							int sl = (int)(len / kc) - 1;

							double khoanglui = (len - sl * kc) / 2;

							sectionPnts[0] = V.GetEndPoint(0) + vectorY * abvtop + vectorX * khoanglui - vectorZ * (abvtop + MathUtils.MmToFoot(85));
							sectionPnts[1] = V.GetEndPoint(0) + vectorY * (b - abvtop) +
											 vectorX * khoanglui - vectorZ * (abvtop + MathUtils.MmToFoot(85));
							Line li = Line.CreateBound(sectionPnts[0], sectionPnts[1]);

							curves.Add(li);


							Rebar stirrup = Rebar.CreateFromCurves(doc, RebarStyle.StirrupTie, RebarBarType, HookStart,
								HookEnd, elem, vectorX, curves, ho, hi, true, true);

							RebarShapeDrivenAccessor rebarShapeDrivenAccessor = stirrup.GetShapeDrivenAccessor();
							rebarShapeDrivenAccessor.SetLayoutAsFixedNumber(sl, kc * sl, true, true, true);
							curves = new List<Curve>();
						}

						#endregion
					}
				}
			}
			catch
			{

			}
		}
	}
}
