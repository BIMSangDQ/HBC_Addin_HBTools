using System;
using System.Collections.Generic;
using System.Linq;
using Autodesk.Revit.DB;
using Autodesk.Revit.DB.Structure;
using HoaBinhTools.FramingInformation.Db;
using Utils;

namespace HoaBinhTools.FramingInformation.Models.CreateRebar
{
	public class Ver_CStirrup
	{
		public void DrawCStirrup(List<MovableRestBeam> ListMovable, Document doc)
		{
			#region Variable
			string kc1 = "";
			string kc2 = "";
			string kc3 = "";
			string rebarDia = "";

			//Kiểm tra hook sau này cho nhập
			RebarHookType HookStart = (from abcd in new FilteredElementCollector(doc).OfClass(typeof(RebarHookType)).Cast<RebarHookType>()
									   where abcd.Name == Save.Default.Link_HookStart
									   select abcd).First();

			RebarHookType HookEnd = (from abcd in new FilteredElementCollector(doc).OfClass(typeof(RebarHookType)).Cast<RebarHookType>()
									 where abcd.Name == Save.Default.Link_HookEnd
									 select abcd).First();

			int slthepchu = ListMovable[0].FraInfoViewModels.SL_T1;

			RebarBarType rebarBarTypethepchu = new FilteredElementCollector(doc)
				.OfClass(typeof(RebarBarType))
				.Cast<RebarBarType>()
				.First(x => x.Name == ListMovable[0].FraInfoViewModels.TopMainBarDia);

			double dkthepchu = rebarBarTypethepchu.BarDiameter;

			RebarHookOrientation ho = RebarHookOrientation.Right;

			RebarHookOrientation hi = RebarHookOrientation.Right;
			#endregion

			try
			{
				//Vẽ thép đai vào nhịp
				foreach (MovableRestBeam movable in ListMovable)
				{
					if (movable.Type.ToString() == "ND")
					{

						double tl = Double.Parse(Save.Default.TL_top); // Tỉ lệ rải đai
						double abvtop = MathUtils.MmToFoot(double.Parse(Save.Default.ConcreteCover));
						double abv = MathUtils.MmToFoot(double.Parse(Save.Default.ConcreteCover)) + MathUtils.MmToFoot(5) + dkthepchu / 2;
						double vitribatdau = MathUtils.MmToFoot(50);

						if (movable.FraInfoViewModels.IsCheckMainStirrup == true)
						{
							#region Db space
							//string SQL_String = string.Format("SELECT * FROM Stirrup WHERE ElementID = '{0}' ORDER BY Vitri",
							//	movable.Id);
							//System.Data.DataTable SID = LocalDb.Get_DataTable(SQL_String);

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
							#endregion
						}
						else
						{
							kc1 = movable.FraInfoViewModels.KC_AddStirrup.ToString();
							kc2 = movable.FraInfoViewModels.KC_AddStirrup.ToString();
							kc3 = movable.FraInfoViewModels.KC_AddStirrup.ToString();
							rebarDia = movable.FraInfoViewModels.AddStirrupDiameter;
						}
						RebarBarType rebarBarType = new FilteredElementCollector(doc)
							.OfClass(typeof(RebarBarType))
							.Cast<RebarBarType>()
							.First(xx => xx.Name.Replace(" ", "") == rebarDia);

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
						#endregion
						//string SQLString = string.Format("SELECT * FROM SStirrup WHERE ElementID = '{0}' AND Type = 'DaiC'",
						//	movable.Id);
						//System.Data.DataTable SID1 = LocalDb.Get_DataTable(SQLString);
						foreach (SStirrup span in movable.FraInfoViewModels.ListSStirrup)
						{
							if (span.EleID == movable.Id.ToString()
								&& span.Type == "DaiC")
							{
								origin = pnt - vectorY * b / 2 - vectorZ * h * x + vectorX * dis;
								int vitri = int.Parse(span.Vitri1.ToString());
								double kcthanh = (b - 2 * abv) / (slthepchu - 1);
								List<XYZ> sectionPnts = new List<XYZ>
								{
								origin + vectorY*(abv+kcthanh*(vitri-1)+MathUtils.MmToFoot(15)) - vectorZ * abvtop,
								origin + vectorY*(abv+kcthanh*(vitri-1)+MathUtils.MmToFoot(15)) - vectorZ * (h-abvtop),
								};

								Line lr1 = Line.CreateBound(sectionPnts[0], sectionPnts[1]);

								IList<Curve> curves = new List<Curve>();

								curves.Add(lr1);

								#region Vẽ thép
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

									ho = RebarHookOrientation.Right;

									hi = RebarHookOrientation.Right;

									double Khoangcuoidam = nhipdam - vitribatdau - sp1 * (sl1 - 1) - sp1;
									double sl2 = Math.Round((Khoangcuoidam) / sp3, 0);
									// Tính toán khoảng cách giữa 2 loại thép để tránh thiếu  1 đai
									double KcConLai = (nhipdam - 2 * vitribatdau) - sp1 * (sl1 - 1) - sp3 * (sl2 - 1);
									sl2 = (KcConLai > sp3 + vitribatdau) ? sl2 = sl2 + 1 : sl2 = sl2;

									origin = origin + vectorX * (nhipdam - 2 * vitribatdau);
									sectionPnts = new List<XYZ>
								{
									origin + vectorY*(abv+kcthanh*(vitri-1)+MathUtils.MmToFoot(15)) - vectorZ * abvtop,
									origin + vectorY*(abv+kcthanh*(vitri-1)+MathUtils.MmToFoot(15)) - vectorZ * (h-abvtop),
								};
									lr1 = Line.CreateBound(sectionPnts[0], sectionPnts[1]);

									IList<Curve> curves1 = new List<Curve>();

									curves1.Add(lr1);

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
										ho = RebarHookOrientation.Right;

										hi = RebarHookOrientation.Right;

										double Khoangcuoidam = nhipdam - Khoangdaudam - vitribatdau;
										double sl2 = Math.Round((Khoangcuoidam) / sp3, 0);

										// Tính toán khoảng cách giữa 2 loại thép để tránh thiếu  1 đai
										double KcConLai = (nhipdam - 2 * vitribatdau) - sp1 * (sl1) - sp3 * (sl2 - 1);
										sl2 = (KcConLai > sp3 + vitribatdau) ? sl2 = sl2 + 1 : sl2 = sl2;

										origin = origin + vectorX * (nhipdam - 2 * vitribatdau);
										sectionPnts = new List<XYZ>
								{
									origin + vectorY*(abv+kcthanh*(vitri-1)+MathUtils.MmToFoot(15)) - vectorZ * abvtop,
									origin + vectorY*(abv+kcthanh*(vitri-1)+MathUtils.MmToFoot(15)) - vectorZ * (h-abvtop),
								};
										lr1 = Line.CreateBound(sectionPnts[0], sectionPnts[1]);


										IList<Curve> curves1 = new List<Curve>();

										curves1.Add(lr1);

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
									origin + vectorY*(abv+kcthanh*(vitri-1)+MathUtils.MmToFoot(15)) - vectorZ * abvtop,
									origin + vectorY*(abv+kcthanh*(vitri-1)+MathUtils.MmToFoot(15)) - vectorZ * (h-abvtop),
								};
										lr1 = Line.CreateBound(sectionPnts[0], sectionPnts[1]);

										IList<Curve> curves1 = new List<Curve>();

										curves1.Add(lr1);

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
									origin + vectorY*(abv+kcthanh*(vitri-1)+MathUtils.MmToFoot(15)) - vectorZ * abvtop,
									origin + vectorY*(abv+kcthanh*(vitri-1)+MathUtils.MmToFoot(15)) - vectorZ * (h-abvtop),
								};
										#endregion

										lr1 = Line.CreateBound(sectionPnts[0], sectionPnts[1]);


										IList<Curve> curves2 = new List<Curve>();

										curves2.Add(lr1);

										ho = RebarHookOrientation.Right;

										hi = RebarHookOrientation.Right;

										stirrup = Rebar.CreateFromCurves(doc, RebarStyle.StirrupTie, rebarBarType, HookStart, HookEnd, elem, vectorX, curves2, ho, hi, true, true);
										rebarShapeDrivenAccessor = stirrup.GetShapeDrivenAccessor();
										rebarShapeDrivenAccessor.SetLayoutAsFixedNumber((int)sl2, sp2 * (sl2), true, true, true);
									}
								}
								doc.Regenerate();

								#endregion
							}
						}
					}
				}
			}
			catch (Exception e)
			{
				throw;
			}
		}
	}
}
