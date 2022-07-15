using System;
using System.Collections.Generic;
using System.Linq;
using Autodesk.Revit.DB;
using Autodesk.Revit.DB.Structure;
using Utils;

namespace HoaBinhTools.FramingInformation.Models.CreateRebar
{
	public class TopMainRebar
	{
		public void DrawTopMainBar(List<MovableRestBeam> ListMovable, Document doc)
		{
			#region Variable
			RebarBarType rebarBarType = new FilteredElementCollector(doc)
				.OfClass(typeof(RebarBarType))
				.Cast<RebarBarType>()
				.First(x => x.Name == ListMovable[0].FraInfoViewModels.TopMainBarDia);

			XYZ vectorX = XYZ.BasisX;
			XYZ vectorZ = XYZ.BasisZ;
			XYZ vectorY = XYZ.BasisY;

			List<XYZ> sectionPnts = new List<XYZ>
			{
				new XYZ( 0,0,0),
				new XYZ(1,1,1)
			};
			IList<Curve> curves = new List<Curve>();

			double Ele_old = 0;
			double dkthepchu = rebarBarType.BarDiameter;

			double Lneo = double.Parse(ListMovable[0].FraInfoViewModels.Top_Anchorage) * dkthepchu;
			double abv = MathUtils.MmToFoot(double.Parse(Save.Default.ConcreteCover) + 5) + dkthepchu / 2;
			double Khoanglui = MathUtils.MmToFoot(100);
			//Đếm số nhịp
			int SoluongNhip = 0;
			foreach (MovableRestBeam movable in ListMovable)
			{
				if (movable.Type.ToString() == "ND") SoluongNhip++;
			}

			int Nhip = 0;
			#endregion

			try
			{
				for (int i = 0; i < ListMovable.Count; i++) // Lặp qua từng nhịp
				{

					MovableRestBeam movable = ListMovable[i];

					if (movable.Type.ToString() == "ND")
					{
						#region Thông tin cơ bản
						double b = MathUtils.MmToFoot((double)movable.Width);
						double h = MathUtils.MmToFoot((double)movable.Hight);
						double elTop = MathUtils.MmToFoot((double)movable.TopElevation);


						Line Linenhipdam = movable.Curve as Line;
						double nhipdam = Linenhipdam.Length;

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
						#endregion

						XYZ origin = pnt - vectorY * b / 2 - vectorZ * h * x + vectorX * dis; // vị trí đầu nhịp

						origin = origin + vectorY * abv - vectorZ * abv;

						// Nhịp đầu tiên thì tính đoạn neo luôn.
						if (Nhip == 0)
						{
							if (i == 0) //Đầu consol
							{
								Khoanglui = MathUtils.MmToFoot(50);
								sectionPnts[0] = origin + vectorX * Khoanglui - vectorZ * (h - 2 * Khoanglui);
								sectionPnts[1] = origin + vectorX * Khoanglui;
								Line lr = Line.CreateBound(sectionPnts[0], sectionPnts[1]);
								curves.Add(lr); // Đoạn bẻ thép consol

								sectionPnts[0] = origin + vectorX * Khoanglui;
								sectionPnts[1] = origin + vectorX * nhipdam;
								lr = Line.CreateBound(sectionPnts[0], sectionPnts[1]);
								curves.Add(lr); // Đoạn trong dầm
							}
							else// Có gối phía trước
							{
								double ChieuDaiGoi = MathUtils.MmToFoot((ListMovable[i - 1].Length));
								if (ChieuDaiGoi < MathUtils.MmToFoot(300)) // Nếu gối nhỏ hơn 300 thì lùi 50 thôi và chắc chắn phải bẻ giò gà
								{
									Khoanglui = MathUtils.MmToFoot(50);
								}
								else // Nếu gối neo thẳng được thì neo thẳng ko thì tính doạn giò gà tối thiểu 10d -> 
								{
									Khoanglui = MathUtils.MmToFoot(100);
								}

								if (ChieuDaiGoi > Lneo) // Neo thẳng
								{
									sectionPnts[0] = origin - vectorX * Lneo;
									sectionPnts[1] = origin;
									Line ln = Line.CreateBound(sectionPnts[0], sectionPnts[1]);
									curves.Add(ln); // Đoạn neo vào gối

									sectionPnts[0] = origin;
									sectionPnts[1] = origin + vectorX * nhipdam;
									ln = Line.CreateBound(sectionPnts[0], sectionPnts[1]);
									curves.Add(ln); // Đoạn trong dầm

									//CreateRebar(curves,abv,movable.FraInfoViewModels.SL_T1,vectorY,rebarBarType,doc,elem,b);
								}
								else // Có bẻ giò gà
								{
									// Tính lại khoảng neo vào để đảm bảo giò gà
									double doangioga = Lneo - (ChieuDaiGoi - Khoanglui);
									if (doangioga < 10 * dkthepchu)
									{
										doangioga = 10 * dkthepchu;
										Khoanglui = ChieuDaiGoi - (Lneo - doangioga);
									}

									sectionPnts[0] = origin - vectorX * (ChieuDaiGoi - Khoanglui) - vectorZ * doangioga;
									sectionPnts[1] = origin - vectorX * (ChieuDaiGoi - Khoanglui);
									Line ln = Line.CreateBound(sectionPnts[0], sectionPnts[1]);
									curves.Add(ln); // Đoạn neo gio ga

									sectionPnts[0] = origin - vectorX * (ChieuDaiGoi - Khoanglui);
									sectionPnts[1] = origin;
									ln = Line.CreateBound(sectionPnts[0], sectionPnts[1]);
									curves.Add(ln); // Đoạn neo

									sectionPnts[0] = origin;
									sectionPnts[1] = origin + vectorX * nhipdam;
									ln = Line.CreateBound(sectionPnts[0], sectionPnts[1]);
									curves.Add(ln); // Đoạn trong dầm
													//CreateRebar(curves,abv,movable.FraInfoViewModels.SL_T1,vectorY,rebarBarType,doc,elem,b);
								}
							}
						}
						else  //Các nhịp sau check giật cấp r tính neo
						{
							if (Math.Abs(elTop - Ele_old) <= MathUtils.MmToFoot(50)) // Trường hợp nhấn thép không cắt thép hoặc chạy thawnrr đc
							{
								sectionPnts[0] = sectionPnts[1];
								sectionPnts[1] = origin;
								Line lr = Line.CreateBound(sectionPnts[0], sectionPnts[1]);
								curves.Add(lr); // Đoạn trong gối

								sectionPnts[0] = origin;
								sectionPnts[1] = origin + vectorX * nhipdam;
								lr = Line.CreateBound(sectionPnts[0], sectionPnts[1]);
								curves.Add(lr); // Đoạn trong dầm
							}
							else //Có cắt thép tính đoạn neo và vẽ thép
							{
								if (elTop - Ele_old > 0) // Nhịp hiện tại giật cấp lên bên kia neo thẳng ko quan tâm tới gối
								{
									sectionPnts[0] = sectionPnts[1];
									sectionPnts[1] = sectionPnts[0] + vectorX * Lneo;
									Line lr = Line.CreateBound(sectionPnts[0], sectionPnts[1]);
									curves.Add(lr); // Đoạn neo từ bên trái qua
									CreateRebar(curves, abv, movable.FraInfoViewModels.SL_T1, vectorY, rebarBarType, doc, elem, b);

									curves = new List<Curve>();//Reset rồi tạo đoạn neo cho nhịp tiếp theo

									#region Đoạn neo 
									double ChieuDaiGoi = MathUtils.MmToFoot((ListMovable[i - 1].Length));
									if (ChieuDaiGoi < MathUtils.MmToFoot(300)) // Nếu gối nhỏ hơn 300 thì lùi 50 thôi và chắc chắn phải bẻ giò gà
									{
										Khoanglui = MathUtils.MmToFoot(50);
									}
									else // Nếu gối neo thẳng được thì neo thẳng ko thì tính doạn giò gà tối thiểu 10d -> 
									{
										Khoanglui = MathUtils.MmToFoot(100);
									}

									if (ChieuDaiGoi > Lneo) // Neo thẳng
									{
										sectionPnts[0] = origin - vectorX * Lneo;
										sectionPnts[1] = origin;
										Line ln = Line.CreateBound(sectionPnts[0], sectionPnts[1]);
										curves.Add(ln); // Đoạn neo vào gối

										sectionPnts[0] = origin;
										sectionPnts[1] = origin + vectorX * nhipdam;
										ln = Line.CreateBound(sectionPnts[0], sectionPnts[1]);
										curves.Add(ln); // Đoạn trong dầm

										//CreateRebar(curves,abv,movable.FraInfoViewModels.SL_T1,vectorY,rebarBarType,doc,elem,b);
									}
									else // Có bẻ giò gà
									{
										// Tính lại khoảng neo vào để đảm bảo giò gà
										double doangioga = Lneo - (ChieuDaiGoi - Khoanglui);
										if (doangioga < 10 * dkthepchu)
										{
											doangioga = 10 * dkthepchu;
											Khoanglui = ChieuDaiGoi - (Lneo - doangioga);
										}

										sectionPnts[0] = origin - vectorX * (ChieuDaiGoi - Khoanglui) - vectorZ * doangioga;
										sectionPnts[1] = origin - vectorX * (ChieuDaiGoi - Khoanglui);
										Line ln = Line.CreateBound(sectionPnts[0], sectionPnts[1]);
										curves.Add(ln); // Đoạn neo gio ga

										sectionPnts[0] = origin - vectorX * (ChieuDaiGoi - Khoanglui);
										sectionPnts[1] = origin;
										ln = Line.CreateBound(sectionPnts[0], sectionPnts[1]);
										curves.Add(ln); // Đoạn neo

										sectionPnts[0] = origin;
										sectionPnts[1] = origin + vectorX * nhipdam;
										ln = Line.CreateBound(sectionPnts[0], sectionPnts[1]);
										curves.Add(ln); // Đoạn trong dầm
														//CreateRebar(curves,abv,movable.FraInfoViewModels.SL_T1,vectorY,rebarBarType,doc,elem,b);
									}
									#endregion
								}
								else // Giật cấp xuống thanh cũ tính neo, thanh mới neo thẳng
								{
									#region  Đoạn neo thanh cũ
									double ChieuDaiGoi = MathUtils.MmToFoot((ListMovable[i + 1].Length));
									if (ChieuDaiGoi < MathUtils.MmToFoot(300)) // Nếu gối nhỏ hơn 300 thì lùi 50 thôi và chắc chắn phải bẻ giò gà
									{
										Khoanglui = MathUtils.MmToFoot(50);
									}
									else // Nếu gối neo thẳng được thì neo thẳng ko thì tính doạn giò gà tối thiểu 10d -> 
									{
										Khoanglui = MathUtils.MmToFoot(100);
									}

									if (ChieuDaiGoi > Lneo) // Neo thẳng qua
									{
										sectionPnts[0] = sectionPnts[1];
										sectionPnts[1] = sectionPnts[1] + vectorX * Lneo;
										Line ln = Line.CreateBound(sectionPnts[0], sectionPnts[1]);
										curves.Add(ln); // Đoạn neo  thẳng vào gối
									}
									else // Có bẻ giò gà
									{
										// Tính lại khoảng neo vào để đảm bảo giò gà
										double doangioga = Lneo - (ChieuDaiGoi - Khoanglui);
										if (doangioga < 10 * dkthepchu)
										{
											doangioga = 10 * dkthepchu;
											Khoanglui = ChieuDaiGoi - (Lneo - doangioga);
										}

										sectionPnts[0] = sectionPnts[1];
										sectionPnts[1] = sectionPnts[1] + vectorX * (ChieuDaiGoi - Khoanglui);
										Line ln = Line.CreateBound(sectionPnts[0], sectionPnts[1]);
										curves.Add(ln); // Đoạn neo gio ga

										sectionPnts[0] = sectionPnts[1];
										sectionPnts[1] = sectionPnts[1] - vectorZ * doangioga;
										ln = Line.CreateBound(sectionPnts[0], sectionPnts[1]);
										curves.Add(ln); // Đoạn neo
									}
									#endregion
									CreateRebar(curves, abv, movable.FraInfoViewModels.SL_T1, vectorY, rebarBarType, doc, elem, b); // VẼ  thanh cũ

									curves = new List<Curve>();//Reset rồi tạo đoạn neo cho nhịp tiếp theo
									sectionPnts[0] = origin - vectorX * Lneo;
									sectionPnts[1] = origin;
									Line lr = Line.CreateBound(sectionPnts[0], sectionPnts[1]);
									curves.Add(lr); // Đoạn trong gối


									sectionPnts[0] = origin;
									if (i == ListMovable.Count - 1)
									{
										sectionPnts[1] = origin + vectorX * (nhipdam - MathUtils.MmToFoot(50));
									}
									else
									{
										sectionPnts[1] = origin + vectorX * nhipdam;
									}

									lr = Line.CreateBound(sectionPnts[0], sectionPnts[1]);
									curves.Add(lr); // Đoạn trong dầm

								}
							}
						}


						//Vẽ đoạn neo cuối cùng rối tạo thép
						if (i >= ListMovable.Count - 2)
						{
							if (i == ListMovable.Count - 1) // Đầu cuối consol
							{
								sectionPnts[0] = sectionPnts[1];
								sectionPnts[1] = sectionPnts[1] - vectorZ * (h - MathUtils.MmToFoot(100));
								Line lr = Line.CreateBound(sectionPnts[0], sectionPnts[1]);
								curves.Add(lr); // Đoạn trong gối
							}
							else // Neo vào gối cuối
							{
								#region  Đoạn neo cuối cùng
								double ChieuDaiGoi = MathUtils.MmToFoot((ListMovable[i + 1].Length));

								if (ChieuDaiGoi < MathUtils.MmToFoot(300)) // Nếu gối nhỏ hơn 300 thì lùi 50 thôi và chắc chắn phải bẻ giò gà
								{
									Khoanglui = MathUtils.MmToFoot(50);
								}
								else // Nếu gối neo thẳng được thì neo thẳng ko thì tính doạn giò gà tối thiểu 10d -> 
								{
									Khoanglui = MathUtils.MmToFoot(100);
								}

								if (ChieuDaiGoi > Lneo) // Neo thẳng qua
								{
									sectionPnts[0] = sectionPnts[1];
									sectionPnts[1] = sectionPnts[1] + vectorX * Lneo;
									Line ln = Line.CreateBound(sectionPnts[0], sectionPnts[1]);
									curves.Add(ln); // Đoạn neo  thẳng vào gối
								}
								else // Có bẻ giò gà
								{
									// Tính lại khoảng neo vào để đảm bảo giò gà
									double doangioga = Lneo - (ChieuDaiGoi - Khoanglui);
									if (doangioga < 10 * dkthepchu)
									{
										doangioga = 10 * dkthepchu;
										Khoanglui = ChieuDaiGoi - (Lneo - doangioga);
									}

									sectionPnts[0] = sectionPnts[1];
									sectionPnts[1] = sectionPnts[1] + vectorX * (ChieuDaiGoi - Khoanglui);
									Line ln = Line.CreateBound(sectionPnts[0], sectionPnts[1]);
									curves.Add(ln); // Đoạn neo gio ga

									sectionPnts[0] = sectionPnts[1];
									sectionPnts[1] = sectionPnts[1] - vectorZ * doangioga;
									ln = Line.CreateBound(sectionPnts[0], sectionPnts[1]);
									curves.Add(ln); // Đoạn neo
								}
								#endregion
							}
							CreateRebar(curves, abv, movable.FraInfoViewModels.SL_T1, vectorY, rebarBarType, doc, elem, b);
						}

						Ele_old = elTop;
						Nhip++;
					}

				}
			}
			catch (Exception e)
			{
				throw;
			}
		}

		public void CreateRebar(IList<Curve> curves1, double abv, int sl, XYZ vector, RebarBarType rebarBarType, Document doc, Element elem, double b)
		{
			try
			{
				if (sl < 2) sl = 2;

				IList<Curve> curves = new List<Curve>();

				List<XYZ> abc = new List<XYZ>();
				for (int i = 0; i < curves1.Count; i++)
				{
					if (i == 0)
					{
						abc.Add(curves1[i].GetEndPoint(0));
						abc.Add(curves1[i].GetEndPoint(1));
					}
					else
					{
						abc.Add(curves1[i].GetEndPoint(1));
					}
				}


				for (int i = 0; i < abc.Count - 1; i++)
				{
					Line ln = Line.CreateBound(abc[i], abc[i + 1]);
					curves.Add(ln);
				}

				IList<Curve> curvesphu = new List<Curve>();
				curvesphu = RutGonCurves(curves);// Rút gọn loại bỏ các segment

				RebarHookOrientation ho = RebarHookOrientation.Right;

				RebarHookOrientation hi = RebarHookOrientation.Right;

				Rebar stirrup = Rebar.CreateFromCurves(doc, RebarStyle.Standard, rebarBarType, null, null, elem, vector, curvesphu, ho, hi, true, true);

				RebarShapeDrivenAccessor rebarShapeDrivenAccessor = stirrup.GetShapeDrivenAccessor();
				rebarShapeDrivenAccessor.SetLayoutAsFixedNumber(sl, (b - 2 * abv), true, true, true);


			}
			catch (Exception ex)
			{ }
		}

		public IList<Curve> RutGonCurves(IList<Curve> cv)
		{
			IList<Curve> curves = new List<Curve>();

			int Xp = 0;

			XYZ spnt = new XYZ(0, 0, 0);
			Line linexp = Line.CreateBound(new XYZ(0, 0, 0), new XYZ(1, 1, 1));

			for (int i = 0; i < cv.Count; i++)
			{
				if (Xp == i && i == 0)
				{
					spnt = cv[i].GetEndPoint(0);
					linexp = cv[i] as Line;
				}
				else if (Xp == i && i != 0)// Vẽ Line đoạn trước
				{

				}
				else // Các nhịp sau nhịp xp
				{
					//Check nếu thẳng thì bỏ qua
					if (CurveUtils.DistancePoint2Line(cv[i].GetEndPoint(0), linexp) == 0 && CurveUtils.DistancePoint2Line(cv[i].GetEndPoint(1), linexp) == 0) //Nếu mà 2 điểm của curve sau đều  nằm trên đường thẳng thì cứ bỏ qua đoạn này
					{

					}
					else // Nếu mà đoạn thẳng sau có dấu hiện bẻ thì vẽ đoạn trước 
					{
						Line line = Line.CreateBound(spnt, cv[i].GetEndPoint(0));
						curves.Add(line);

						linexp = Line.CreateBound(cv[i].GetEndPoint(0), cv[i].GetEndPoint(1));

						Xp = i;
						spnt = cv[i].GetEndPoint(0);
					}
				}
			}

			//Vẽ segment cuối cùng
			Line line1 = Line.CreateBound(spnt, cv[cv.Count - 1].GetEndPoint(1));
			curves.Add(line1);

			return curves;
		}
	}
}
