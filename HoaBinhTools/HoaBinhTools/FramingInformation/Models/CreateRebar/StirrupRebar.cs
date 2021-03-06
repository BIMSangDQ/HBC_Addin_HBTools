using System;
using System.Collections.Generic;
using System.Linq;
using Autodesk.Revit.DB;
using Autodesk.Revit.DB.Structure;
using HoaBinhTools.FramingInformation.Db;
using Utils;

namespace HoaBinhTools.FramingInformation.Models.CreateRebar
{
	public class StirrupRebar
	{
		public void DrawMainStirrup(List<MovableRestBeam> ListMovable, Document doc)
		{

			View view = doc.ActiveView;
			string kc1 = "";
			string kc2 = "";
			string kc3 = "";
			string rebarDia = "";

			try
			{

				//Vẽ thép đai vào nhịp
				foreach (MovableRestBeam movable in ListMovable)
				{
					if (movable.Type.ToString() == "ND")
					{
						double tl = Double.Parse(Save.Default.TL_top); // Tỉ lệ rải đai
						double abv = MathUtils.MmToFoot(double.Parse(Save.Default.ConcreteCover));
						double vitribatdau = MathUtils.MmToFoot(50);

						//Đi vào db ktr có khoảng cách vs dk không
						//Nếu mà nhịp đó không có thì quay lại lấy theo nhịp đầu tiên

						#region Db space

						kc1 = Save.Default.StirrupSup;
						kc2 = Save.Default.StirrupSpan;
						kc3 = Save.Default.StirrupSup;
						rebarDia = Save.Default.StirrupDia;

						foreach (Stirrup span in movable.FraInfoViewModels.ListStirrup)
						{
							if (span.Vitri.ToString() == "1" && span.EleID == movable.Id.ToString())
							{
								kc1 = span.Space.ToString();
								rebarDia = span.Diameter.ToString().Replace(" ", "");
							}
							else if (span.Vitri.ToString() == "2" && span.EleID == movable.Id.ToString())
							{
								kc2 = span.Space.ToString();
								rebarDia = span.Diameter.ToString().Replace(" ", "");
							}
							else if (span.Vitri.ToString() == "3" && span.EleID == movable.Id.ToString())
							{
								kc3 = span.Space.ToString();
								rebarDia = span.Diameter.ToString().Replace(" ", "");
							}

						}
						RebarBarType rebarBarType = new FilteredElementCollector(doc)
							.OfClass(typeof(RebarBarType))
							.Cast<RebarBarType>()
							.First(xx => xx.Name.Replace(" ", "") == rebarDia);
						#endregion




						double sp1 = MathUtils.MmToFoot(double.Parse(kc1));
						double sp2 = MathUtils.MmToFoot(double.Parse(kc2));
						double sp3 = MathUtils.MmToFoot(double.Parse(kc3));

						double b = MathUtils.MmToFoot((double)movable.Width);
						double h = MathUtils.MmToFoot((double)movable.Hight);
						double elTop = MathUtils.MmToFoot((double)movable.TopElevation);
						double elBot = MathUtils.MmToFoot((double)movable.BottomElevation);

						Line Linekhoangrai = movable.Curve as Line;
						double nhipdam = Linekhoangrai.Length;

						Element elem = doc.GetElement(movable.Id);
						#region tìm vị trí rải đai đầu dầm
						Location loc = elem.Location;
						LocationCurve locCur = loc as LocationCurve;
						Curve curve = locCur.Curve;
						Line line = curve as Line;

						XYZ vectorX = line.Direction;
						XYZ vectorZ = XYZ.BasisZ;
						XYZ vectorY = vectorZ.CrossProduct(vectorX);

						//Tìm điểm bắt đầu rải đai
						//Tạo mặt phẳng => tìm kc từ điểm bắt đầu curve tới mp
						XYZ pnt1 = Linekhoangrai.GetEndPoint(0) + vitribatdau * vectorX;
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
						XYZ origin = pnt - vectorY * b / 2 - vectorZ * h * x + vectorX * dis;
						List<XYZ> sectionPnts = new List<XYZ>
						{
							origin + vectorY*abv - vectorZ * abv,
							origin + vectorY * (b-abv)- vectorZ * abv,
							origin + vectorY * (b-abv) - vectorZ * (h-abv),
							origin + vectorY*abv- vectorZ * (h-abv),
						};
						#endregion

						Line lr1 = Line.CreateBound(sectionPnts[0], sectionPnts[1]);
						Line lr2 = Line.CreateBound(sectionPnts[1], sectionPnts[2]);
						Line lr3 = Line.CreateBound(sectionPnts[2], sectionPnts[3]);
						Line lr4 = Line.CreateBound(sectionPnts[3], sectionPnts[0]);

						IList<Curve> curves = new List<Curve>();

						curves.Add(lr1);
						curves.Add(lr2);
						curves.Add(lr3);
						curves.Add(lr4);

						//Kiểm tra hook sau này cho nhập
						RebarHookType HookStart = (from abcd in new FilteredElementCollector(doc).OfClass(typeof(RebarHookType)).Cast<RebarHookType>()
												   where abcd.Name == Save.Default.Stirrup_HookStart
												   select abcd).First();

						RebarHookType HookEnd = (from abcd in new FilteredElementCollector(doc).OfClass(typeof(RebarHookType)).Cast<RebarHookType>()
												 where abcd.Name == Save.Default.Stirrup_HookEnd
												 select abcd).First();

						RebarHookOrientation ho = RebarHookOrientation.Right;

						RebarHookOrientation hi = RebarHookOrientation.Right;

						//Vẽ thép đai từng đoạn dầm tỉ lệ trước mắt 0.25-0.5-0.25 sau này fix tỉ lệ nhập sau

						//List<Autodesk.Revit.DB.Structure.Rebar> newRebars = new List<Autodesk.Revit.DB.Structure.Rebar>();

						Rebar stirrup = Rebar.CreateFromCurves(doc, RebarStyle.StirrupTie, rebarBarType, HookStart, HookEnd, elem, vectorX, curves, ho, hi, true, true);

						//Tính toán số lượng
						if (sp1 == sp2 && sp2 == sp3) // Cả nhịp cùng rải 1 khoảng cách
						{
							double Khoangdaudam = nhipdam;
							double sl1 = Math.Round((Khoangdaudam - 2 * vitribatdau) / sp1, 0);
							//Check lại thử có vượt qua ko.
							sl1 = (sl1 * sp1 > (nhipdam - 2 * vitribatdau)) ? sl1 - 1 : sl1;

							RebarShapeDrivenAccessor rebarShapeDrivenAccessor = stirrup.GetShapeDrivenAccessor();
							rebarShapeDrivenAccessor.SetLayoutAsFixedNumber((int)sl1, sp1 * (sl1), true, true, true);

						}
						else if (sp1 == sp2 && sp2 != sp3) // Khoảng rải cuối dầm khác
						{
							double Khoangdaudam = nhipdam * (1 - tl);
							double sl1 = Math.Round((Khoangdaudam - vitribatdau) / sp1, 0);

							RebarShapeDrivenAccessor rebarShapeDrivenAccessor = stirrup.GetShapeDrivenAccessor();
							rebarShapeDrivenAccessor.SetLayoutAsFixedNumber((int)sl1, sp1 * (sl1 - 1), true, true, true);
							// Tính toán vẽ đoạn cuối dầm 

							ho = RebarHookOrientation.Left;

							hi = RebarHookOrientation.Left;

							double Khoangcuoidam = nhipdam - vitribatdau - sp1 * (sl1 - 1) - sp1;
							double sl2 = Math.Round((Khoangcuoidam) / sp3, 0);
							// Tính toán khoảng cách giữa 2 loại thép để tránh thiếu  1 đai
							double KcConLai = (nhipdam - 2 * vitribatdau) - sp1 * (sl1 - 1) - sp3 * (sl2 - 1);
							sl2 = (KcConLai > sp3 + vitribatdau) ? sl2 = sl2 + 1 : sl2 = sl2;

							origin = origin + vectorX * (nhipdam - 2 * vitribatdau);
							sectionPnts = new List<XYZ>
							{
							origin + vectorY*abv - vectorZ * abv,
							origin + vectorY * (b-abv)- vectorZ * abv,
							origin + vectorY * (b-abv) - vectorZ * (h-abv),
							origin + vectorY*abv- vectorZ * (h-abv),
							};
							lr1 = Line.CreateBound(sectionPnts[0], sectionPnts[1]);
							lr2 = Line.CreateBound(sectionPnts[1], sectionPnts[2]);
							lr3 = Line.CreateBound(sectionPnts[2], sectionPnts[3]);
							lr4 = Line.CreateBound(sectionPnts[3], sectionPnts[0]);

							IList<Curve> curves1 = new List<Curve>();

							curves1.Add(lr1);
							curves1.Add(lr2);
							curves1.Add(lr3);
							curves1.Add(lr4);
							stirrup = Rebar.CreateFromCurves(doc, RebarStyle.StirrupTie, rebarBarType, HookStart, HookEnd, elem, -vectorX, curves1, ho, hi, true, true);
							rebarShapeDrivenAccessor = stirrup.GetShapeDrivenAccessor();
							rebarShapeDrivenAccessor.SetLayoutAsFixedNumber((int)sl2, sp3 * (sl2 - 1), true, true, true);


						}
						else // Nếu mà khoảng rải các đoạn liên tục khác nhau
						{
							double Khoangdaudam = nhipdam * tl;
							double sl1 = 1 + Math.Round((Khoangdaudam - vitribatdau) / sp1, 0);

							RebarShapeDrivenAccessor rebarShapeDrivenAccessor = stirrup.GetShapeDrivenAccessor();
							rebarShapeDrivenAccessor.SetLayoutAsFixedNumber((int)sl1, sp1 * (sl1 - 1), true, true, true);

							// Tính toán vẽ phần cuối dầm
							if (sp2 == sp3) // Nếu 2 đoạn cuối cùng khoảng rải
							{
								ho = RebarHookOrientation.Left;

								hi = RebarHookOrientation.Left;

								double Khoangcuoidam = nhipdam - Khoangdaudam - vitribatdau;
								double sl2 = Math.Round((Khoangcuoidam) / sp3, 0);

								// Tính toán khoảng cách giữa 2 loại thép để tránh thiếu  1 đai
								double KcConLai = (nhipdam - 2 * vitribatdau) - sp1 * (sl1) - sp3 * (sl2 - 1);
								sl2 = (KcConLai > sp3 + vitribatdau) ? sl2 = sl2 + 1 : sl2 = sl2;

								origin = origin + vectorX * (nhipdam - 2 * vitribatdau);
								sectionPnts = new List<XYZ>
								{
								origin + vectorY*abv - vectorZ * abv,
								origin + vectorY * (b-abv)- vectorZ * abv,
								origin + vectorY * (b-abv) - vectorZ * (h-abv),
								origin + vectorY*abv- vectorZ * (h-abv),
								};
								lr1 = Line.CreateBound(sectionPnts[0], sectionPnts[1]);
								lr2 = Line.CreateBound(sectionPnts[1], sectionPnts[2]);
								lr3 = Line.CreateBound(sectionPnts[2], sectionPnts[3]);
								lr4 = Line.CreateBound(sectionPnts[3], sectionPnts[0]);

								IList<Curve> curves1 = new List<Curve>();

								curves1.Add(lr1);
								curves1.Add(lr2);
								curves1.Add(lr3);
								curves1.Add(lr4);
								stirrup = Rebar.CreateFromCurves(doc, RebarStyle.StirrupTie, rebarBarType, HookStart, HookEnd, elem, -vectorX, curves1, ho, hi, true, true);
								rebarShapeDrivenAccessor = stirrup.GetShapeDrivenAccessor();
								rebarShapeDrivenAccessor.SetLayoutAsFixedNumber((int)sl2, sp3 * (sl2), true, true, true);
							}
							else // Nếu 2 đoạn cuối khác đoạn rải
							{
								#region Vẽ đai đoạn cuối dầm
								double sl3 = 1 + Math.Round((Khoangdaudam - vitribatdau) / sp3, 0);

								ho = RebarHookOrientation.Left;

								hi = RebarHookOrientation.Left;

								origin = origin + vectorX * (nhipdam - 2 * vitribatdau);
								sectionPnts = new List<XYZ>
								{
								origin + vectorY*abv - vectorZ * abv,
								origin + vectorY * (b-abv)- vectorZ * abv,
								origin + vectorY * (b-abv) - vectorZ * (h-abv),
								origin + vectorY*abv- vectorZ * (h-abv),
								};
								lr1 = Line.CreateBound(sectionPnts[0], sectionPnts[1]);
								lr2 = Line.CreateBound(sectionPnts[1], sectionPnts[2]);
								lr3 = Line.CreateBound(sectionPnts[2], sectionPnts[3]);
								lr4 = Line.CreateBound(sectionPnts[3], sectionPnts[0]);

								IList<Curve> curves1 = new List<Curve>();

								curves1.Add(lr1);
								curves1.Add(lr2);
								curves1.Add(lr3);
								curves1.Add(lr4);
								stirrup = Rebar.CreateFromCurves(doc, RebarStyle.StirrupTie, rebarBarType, HookStart, HookEnd, elem, -vectorX, curves1, ho, hi, true, true);
								rebarShapeDrivenAccessor = stirrup.GetShapeDrivenAccessor();
								rebarShapeDrivenAccessor.SetLayoutAsFixedNumber((int)sl3, sp3 * (sl3), true, true, true);
								#endregion
								#region Vẽ đai đoạn giữa dầm
								//Tính số lượng đai giữa dầm
								double sl2 = 1 + (nhipdam - 2 * vitribatdau - sp2 - sl1 * sp1 - sl3 * sp3) / sp2;
								double KcConLai = (nhipdam - 2 * vitribatdau - sl1 * sp1 - sl2 * sp2 - sl3 * sp3) / 2;
								sl2 = (KcConLai > sp2) ? sl2 + 1 : sl2;
								KcConLai = (nhipdam - 2 * vitribatdau - sl1 * sp1 - sl2 * sp2 - sl3 * sp3) / 2;

								origin = pnt - vectorY * b / 2 - vectorZ * h * x + vectorX * (dis + sl1 * sp1 + KcConLai);
								sectionPnts = new List<XYZ>
								{
									origin + vectorY*abv - vectorZ * abv,
									origin + vectorY * (b-abv)- vectorZ * abv,
									origin + vectorY * (b-abv) - vectorZ * (h-abv),
									origin + vectorY*abv- vectorZ * (h-abv),
								};
								#endregion

								lr1 = Line.CreateBound(sectionPnts[0], sectionPnts[1]);
								lr2 = Line.CreateBound(sectionPnts[1], sectionPnts[2]);
								lr3 = Line.CreateBound(sectionPnts[2], sectionPnts[3]);
								lr4 = Line.CreateBound(sectionPnts[3], sectionPnts[0]);

								IList<Curve> curves2 = new List<Curve>();

								curves2.Add(lr1);
								curves2.Add(lr2);
								curves2.Add(lr3);
								curves2.Add(lr4);

								ho = RebarHookOrientation.Right;

								hi = RebarHookOrientation.Right;

								stirrup = Rebar.CreateFromCurves(doc, RebarStyle.StirrupTie, rebarBarType, HookStart, HookEnd, elem, vectorX, curves2, ho, hi, true, true);
								rebarShapeDrivenAccessor = stirrup.GetShapeDrivenAccessor();
								rebarShapeDrivenAccessor.SetLayoutAsFixedNumber((int)sl2, sp2 * (sl2), true, true, true);
							}
						}
						doc.Regenerate();
						//newRebars.Add(stirrup);

					}
				}
			}
			catch (Exception ex)
			{ }
		}
	}
}

