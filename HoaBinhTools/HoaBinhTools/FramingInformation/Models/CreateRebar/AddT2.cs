using System;
using System.Collections.Generic;
using System.Linq;
using Autodesk.Revit.DB;
using Autodesk.Revit.DB.Structure;
using HoaBinhTools.FramingInformation.Db;
using Utils;

namespace HoaBinhTools.FramingInformation.Models.CreateRebar
{
	public class AddT2
	{
		public void DrawAddRebar(List<MovableRestBeam> ListMovable, Document doc)
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
				new XYZ(0, 0, 0),
				new XYZ(1, 1, 1)
			};
			IList<Curve> curves = new List<Curve>();

			double Ele_old = 0;
			double dkthepchu = rebarBarType.BarDiameter;

			double Lneo = double.Parse(ListMovable[0].FraInfoViewModels.Top_Anchorage) * dkthepchu;
			double abvtop = MathUtils.MmToFoot(double.Parse(Save.Default.ConcreteCover) + 5) + dkthepchu / 2;
			double Khoanglui = MathUtils.MmToFoot(100);

			int slthepchu = ListMovable[0].FraInfoViewModels.SL_T2; //Số lượng thép chủ
																	//Đếm số nhịp
			int SoluongNhip = 0;
			foreach (MovableRestBeam movable in ListMovable)
			{
				if (movable.Type.ToString() == "ND") SoluongNhip++;
			}

			double tl = double.Parse(Save.Default.TL_top);  // Thay bằng tỉ lệ setting
			int Nhip = 0;

			int sl1 = 0;
			int sl2 = 0;
			int sl3 = 0;
			string dk1 = "";
			string dk2 = "";
			string dk3 = "";
			double kc = 0;

			int slcu = 0;
			RebarBarType cu = rebarBarType;
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
						double elTop = MathUtils.MmToFoot((double)movable.TopElevation);


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
						origin = origin - vectorZ * (abvtop + MathUtils.MmToFoot(50)); // Đưa về đúng cao độ đặt thép rồi

						#endregion

						#region Reset Biến
						sl1 = 0;
						sl2 = 0;
						sl3 = 0;
						dk1 = "";
						dk2 = "";
						dk3 = "";

						foreach (DbAddT2 span in movable.FraInfoViewModels.ListAddT2)
						{
							if (span.Vitri.ToString() == "1" && span.EleID == id)
							{
								sl1 = int.Parse(span.Count.ToString());
								dk1 = span.Diameter.ToString();
							}

							if (span.Vitri.ToString() == "2" && span.EleID == id)
							{
								sl2 = int.Parse(span.Count.ToString());
								dk2 = span.Diameter.ToString();
							}

							if (span.Vitri.ToString() == "3" && span.EleID == id)
							{
								sl3 = int.Parse(span.Count.ToString());
								dk3 = span.Diameter.ToString();
							}
						} //Lấy thông tin thép
						#endregion

						#region ĐẦU NHỊP
						//So sánh với số lượng thép chủ và số lượng thép tăng cường để có vị trí dặt đúng
						if (sl1 != 0) // ĐẦu nhịp có thép thì tạo curve
						{
							origin = pnt - vectorY * b / 2 - vectorZ * h * x + vectorX * dis; // vị trí đầu nhịp
							origin = origin - vectorZ * (abvtop + MathUtils.MmToFoot(50)); // Đưa về đúng cao độ đặt thép rồi

							double kc_TinhTuMepdenThanhThep = (b - 2 * abvtop) / (slthepchu + sl1 - 1);
							if (slthepchu == 0) kc_TinhTuMepdenThanhThep = 0;
							origin = origin + vectorY * (kc_TinhTuMepdenThanhThep + abvtop);

							//Tính khoảng cách các thanh thép
							if (slthepchu == 2)
							{
								kc = kc_TinhTuMepdenThanhThep;
							}
							else
							{
								kc = 2 * kc_TinhTuMepdenThanhThep;
							}

							rebarBarType = new FilteredElementCollector(doc)
								.OfClass(typeof(RebarBarType))
								.Cast<RebarBarType>()
								.First(xx => xx.Name == dk1);

							Lneo = double.Parse(ListMovable[0].FraInfoViewModels.Top_Anchorage) * rebarBarType.BarDiameter;

							//nhịp đầu tiên
							if (Nhip == 0) // Nhịp đầu tiên
							{
								double ChieuDaiGoi = MathUtils.MmToFoot((ListMovable[i - 1].Length));

								if (ChieuDaiGoi < MathUtils.MmToFoot(300)) // Nếu gối nhỏ hơn 300 thì lùi 50 thôi và chắc chắn phải bẻ giò gà
								{
									Khoanglui = MathUtils.MmToFoot(150);
								}
								else // Nếu gối neo thẳng được thì neo thẳng ko thì tính doạn giò gà tối thiểu 10d -> 
								{
									Khoanglui = MathUtils.MmToFoot(200);
								}

								if (ChieuDaiGoi > Lneo) // Neo thẳng
								{
									sectionPnts[0] = origin - vectorX * Lneo;
									sectionPnts[1] = origin;
									Line ln = Line.CreateBound(sectionPnts[0], sectionPnts[1]);
									curves.Add(ln); // Đoạn neo vào gối

									sectionPnts[0] = origin;
									sectionPnts[1] = origin + vectorX * nhipdam * tl;
									ln = Line.CreateBound(sectionPnts[0], sectionPnts[1]);
									curves.Add(ln); // Đoạn trong dầm

									//CreateRebar(curves, abvtop, sl1, vectorY, rebarBarType, doc, elem, b, kc);
									//curves = new List<Curve>();
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

									sectionPnts[0] =
									origin - vectorX * (ChieuDaiGoi - Khoanglui) - vectorZ * doangioga;
									sectionPnts[1] = origin - vectorX * (ChieuDaiGoi - Khoanglui);
									Line ln = Line.CreateBound(sectionPnts[0], sectionPnts[1]);
									curves.Add(ln); // Đoạn neo gio ga

									sectionPnts[0] = origin - vectorX * (ChieuDaiGoi - Khoanglui);
									sectionPnts[1] = origin;
									ln = Line.CreateBound(sectionPnts[0], sectionPnts[1]);
									curves.Add(ln); // Đoạn neo

									sectionPnts[0] = origin;
									sectionPnts[1] = origin + vectorX * nhipdam * tl;
									ln = Line.CreateBound(sectionPnts[0], sectionPnts[1]);
									curves.Add(ln); // Đoạn trong dầm
													//CreateRebar(curves, abvtop, sl1, vectorY, rebarBarType, doc, elem, b, kc);
													//curves = new List<Curve>();
								}
							}
							//Các nhịp sau
							else //Không phải nhịp đầu tiên vẽ curve ra
							{
								if (curves.Count != 0) //Nếu đã tồn tại curve
								{
									//Chỉ cần nhấn thép
									if (Math.Abs(elTop - Ele_old) <= MathUtils.MmToFoot(50)) // Trường hợp nhấn thép không cắt thép hoặc chạy thawnrr đc
									{
										sectionPnts[0] = sectionPnts[1];
										sectionPnts[1] = origin;
										Line lr = Line.CreateBound(sectionPnts[0], sectionPnts[1]);
										curves.Add(lr); // Đoạn trong gối

										sectionPnts[0] = origin;
										sectionPnts[1] = origin + vectorX * nhipdam * tl;
										lr = Line.CreateBound(sectionPnts[0], sectionPnts[1]);
										curves.Add(lr); // Đoạn trong dầm
									}
									//Cắt thép vì chiều cao vượt giới hạn
									else
									{
										//Giật cấp lên thanh cũ neo thẳng
										if (elTop - Ele_old > 0)
										{
											Lneo = double.Parse(ListMovable[0].FraInfoViewModels.Top_Anchorage) * rebarBarType.BarDiameter;
											sectionPnts[0] = sectionPnts[1];
											sectionPnts[1] = sectionPnts[0] + vectorX * Lneo;
											Line lr = Line.CreateBound(sectionPnts[0], sectionPnts[1]);
											curves.Add(lr); // Đoạn neo từ bên trái qua
											CreateRebar(curves, abvtop, sl1, vectorY,
												rebarBarType, doc, elem, b, kc, slthepchu);

											curves = new List<Curve>(); //Reset rồi tạo đoạn neo cho nhịp tiếp theo

											#region Đoạn neo 

											double ChieuDaiGoi = MathUtils.MmToFoot((ListMovable[i - 1].Length));
											if (ChieuDaiGoi < MathUtils.MmToFoot(300)
											) // Nếu gối nhỏ hơn 300 thì lùi 50 thôi và chắc chắn phải bẻ giò gà
											{
												Khoanglui = MathUtils.MmToFoot(150);
											}
											else // Nếu gối neo thẳng được thì neo thẳng ko thì tính doạn giò gà tối thiểu 10d -> 
											{
												Khoanglui = MathUtils.MmToFoot(200);
											}

											Lneo = double.Parse(ListMovable[0].FraInfoViewModels.Top_Anchorage) * rebarBarType.BarDiameter;

											if (ChieuDaiGoi > Lneo) // Neo thẳng
											{
												sectionPnts[0] = origin - vectorX * Lneo;
												sectionPnts[1] = origin;
												Line ln = Line.CreateBound(sectionPnts[0], sectionPnts[1]);
												curves.Add(ln); // Đoạn neo vào gối

												sectionPnts[0] = origin;
												sectionPnts[1] = origin + vectorX * nhipdam * tl;
												ln = Line.CreateBound(sectionPnts[0], sectionPnts[1]);
												curves.Add(ln); // Đoạn trong dầm

												//CreateRebar(curves,abv,movable.FraInfoViewModels.SL_T1,vectorY,rebarBarType,doc,elem,b);
											}
											else // Có bẻ giò gà
											{
												Lneo = double.Parse(ListMovable[0].FraInfoViewModels.Top_Anchorage) * rebarBarType.BarDiameter;
												// Tính lại khoảng neo vào để đảm bảo giò gà
												double doangioga = Lneo - (ChieuDaiGoi - Khoanglui);
												if (doangioga < 10 * dkthepchu)
												{
													doangioga = 10 * dkthepchu;
													Khoanglui = ChieuDaiGoi - (Lneo - doangioga);
												}

												sectionPnts[0] =
													origin - vectorX * (ChieuDaiGoi - Khoanglui) - vectorZ * doangioga;
												sectionPnts[1] = origin - vectorX * (ChieuDaiGoi - Khoanglui);
												Line ln = Line.CreateBound(sectionPnts[0], sectionPnts[1]);
												curves.Add(ln); // Đoạn neo gio ga

												sectionPnts[0] = origin - vectorX * (ChieuDaiGoi - Khoanglui);
												sectionPnts[1] = origin;
												ln = Line.CreateBound(sectionPnts[0], sectionPnts[1]);
												curves.Add(ln); // Đoạn neo

												sectionPnts[0] = origin;
												sectionPnts[1] = origin + vectorX * nhipdam * tl;
												ln = Line.CreateBound(sectionPnts[0], sectionPnts[1]);
												curves.Add(ln); // Đoạn trong dầm
																//CreateRebar(curves,abv,movable.FraInfoViewModels.SL_T1,vectorY,rebarBarType,doc,elem,b);
											}

											#endregion
										}
										// Giật cấp xuống thanh cũ tính neo, thanh mới neo thẳng
										else
										{
											#region  Đoạn neo thanh cũ

											double ChieuDaiGoi = MathUtils.MmToFoot((ListMovable[i + 1].Length));
											if (ChieuDaiGoi < MathUtils.MmToFoot(300)
											) // Nếu gối nhỏ hơn 300 thì lùi 50 thôi và chắc chắn phải bẻ giò gà
											{
												Khoanglui = MathUtils.MmToFoot(150);
											}
											else // Nếu gối neo thẳng được thì neo thẳng ko thì tính doạn giò gà tối thiểu 10d -> 
											{
												Khoanglui = MathUtils.MmToFoot(200);
											}

											Lneo = double.Parse(ListMovable[0].FraInfoViewModels.Top_Anchorage) * rebarBarType.BarDiameter;

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

											CreateRebar(curves, abvtop, movable.FraInfoViewModels.SL_T1, vectorY,
												rebarBarType, doc, elem, b, kc, slthepchu); // VẼ  thanh cũ

											curves = new List<Curve>(); //Reset rồi tạo đoạn neo cho nhịp tiếp theo

											sectionPnts[0] = origin - vectorX * Lneo;
											sectionPnts[1] = origin;
											Line lr = Line.CreateBound(sectionPnts[0], sectionPnts[1]);
											curves.Add(lr); // Đoạn trong gối

											sectionPnts[0] = origin;
											sectionPnts[1] = origin + vectorX * nhipdam * tl;

											lr = Line.CreateBound(sectionPnts[0], sectionPnts[1]);
											curves.Add(lr); // Đoạn trong dầm

										}
									}
								}
								//Chưa có curve thì tính đoạn neo r vẽ đoạn 1/4
								else
								{
									double ChieuDaiGoi = MathUtils.MmToFoot((ListMovable[i - 1].Length));

									Lneo = double.Parse(ListMovable[0].FraInfoViewModels.Top_Anchorage) * rebarBarType.BarDiameter;

									if (Ele_old - elTop < 0)
									{
										if (ChieuDaiGoi < MathUtils.MmToFoot(300)
										) // Nếu gối nhỏ hơn 300 thì lùi 50 thôi và chắc chắn phải bẻ giò gà
										{
											Khoanglui = MathUtils.MmToFoot(150);
										}
										else // Nếu gối neo thẳng được thì neo thẳng ko thì tính doạn giò gà tối thiểu 10d -> 
										{
											Khoanglui = MathUtils.MmToFoot(200);
										}

										if (ChieuDaiGoi > Lneo) // Neo thẳng
										{
											sectionPnts[0] = origin - vectorX * Lneo;
											sectionPnts[1] = origin;
											Line ln = Line.CreateBound(sectionPnts[0], sectionPnts[1]);
											curves.Add(ln); // Đoạn neo vào gối

											sectionPnts[0] = origin;
											sectionPnts[1] = origin + vectorX * nhipdam * tl;
											ln = Line.CreateBound(sectionPnts[0], sectionPnts[1]);
											curves.Add(ln); // Đoạn trong dầm
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

											sectionPnts[0] =
												origin - vectorX * (ChieuDaiGoi - Khoanglui) - vectorZ * doangioga;
											sectionPnts[1] = origin - vectorX * (ChieuDaiGoi - Khoanglui);
											Line ln = Line.CreateBound(sectionPnts[0], sectionPnts[1]);
											curves.Add(ln); // Đoạn neo gio ga

											sectionPnts[0] = origin - vectorX * (ChieuDaiGoi - Khoanglui);
											sectionPnts[1] = origin;
											ln = Line.CreateBound(sectionPnts[0], sectionPnts[1]);
											curves.Add(ln); // Đoạn neo

											sectionPnts[0] = origin;
											sectionPnts[1] = origin + vectorX * nhipdam * tl;
											ln = Line.CreateBound(sectionPnts[0], sectionPnts[1]);
											curves.Add(ln); // Đoạn trong dầm
											CreateRebar(curves, abvtop, sl1, vectorY, rebarBarType, doc, elem, b, kc, slthepchu);
											curves = new List<Curve>();
										}
									}
									else
									{
										sectionPnts[0] = origin - vectorX * Lneo;
										sectionPnts[1] = origin;
										Line ln = Line.CreateBound(sectionPnts[0], sectionPnts[1]);
										curves.Add(ln); // Đoạn neo vào gối

										sectionPnts[0] = origin;
										sectionPnts[1] = origin + vectorX * nhipdam * tl;
										ln = Line.CreateBound(sectionPnts[0], sectionPnts[1]);
										curves.Add(ln); // Đoạn trong dầm
									}
								}
							}
						}
						// Đầu dầm mà không có thép, nếu đã tồn tại curve thì neo vào. Nếu chưa có thì bỏ  qua luôn
						else
						{
							Lneo = double.Parse(ListMovable[0].FraInfoViewModels.Top_Anchorage) * cu.BarDiameter;

							if (curves.Count != 0) // Nếu đã tồn tại curve thì tạo neo rồi vẽ thép
							{
								#region  Đoạn neo cuối cùng
								double ChieuDaiGoi = MathUtils.MmToFoot((ListMovable[i + 1].Length));

								if (ChieuDaiGoi < MathUtils.MmToFoot(300)) // Nếu gối nhỏ hơn 300 thì lùi 50 thôi và chắc chắn phải bẻ giò gà
								{
									Khoanglui = MathUtils.MmToFoot(150);
								}
								else // Nếu gối neo thẳng được thì neo thẳng ko thì tính doạn giò gà tối thiểu 10d -> 
								{
									Khoanglui = MathUtils.MmToFoot(200);
								}

								if (ChieuDaiGoi > Lneo) // Neo thẳng qua
								{
									sectionPnts[0] = sectionPnts[1];
									sectionPnts[1] = sectionPnts[1] + vectorX * Lneo;
									Line ln = Line.CreateBound(sectionPnts[0], sectionPnts[1]);
									curves.Add(ln); // Đoạn neo  thẳng vào gối

									CreateRebar(curves, abvtop, slcu, vectorY, cu, doc, elem, b, kc, slthepchu);
									curves = new List<Curve>();
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

									CreateRebar(curves, abvtop, slcu, vectorY, cu, doc, elem, b, kc, slthepchu);
									curves = new List<Curve>();
								}
								#endregion
							}
						}

						if (sl1 != 0)
						{
							slcu = sl1;
							cu = rebarBarType;
						}

						#endregion

						#region GIỮA NHỊP

						if (sl2 != 0) //Giữa nhịp
						{
							origin = pnt - vectorY * b / 2 - vectorZ * h * x + vectorX * dis; // vị trí đầu nhịp
							origin = origin - vectorZ * (abvtop + MathUtils.MmToFoot(50)); // Đưa về đúng cao độ đặt thép rồi

							double kc_TinhTuMepdenThanhThep = (b - 2 * abvtop) / (slthepchu + sl2 - 1);
							if (slthepchu == 0) kc_TinhTuMepdenThanhThep = 0;
							origin = origin + vectorY * (kc_TinhTuMepdenThanhThep + abvtop);

							rebarBarType = new FilteredElementCollector(doc)
								.OfClass(typeof(RebarBarType))
								.Cast<RebarBarType>()
								.First(xx => xx.Name == dk2);

							//Tính khoảng cách các thanh thép
							if (slthepchu == 2)
							{
								kc = kc_TinhTuMepdenThanhThep;
							}
							else
							{
								kc = 2 * kc_TinhTuMepdenThanhThep;
							}

							//XÁC ĐỊNH VỊ TRÍ
							//ĐÃ có curve thì vẽ tiếp
							if (curves.Count != 0)
							{
								sectionPnts[0] = sectionPnts[1];
								sectionPnts[1] = sectionPnts[0] + vectorX * (1 - 2 * tl) * nhipdam;
								Line lr = Line.CreateBound(sectionPnts[0], sectionPnts[1]);
								curves.Add(lr);
							}
							//Chưa có curve thì tạo curve
							else
							{
								sectionPnts[0] = origin + vectorX * (tl * nhipdam);
								sectionPnts[1] = sectionPnts[0] + vectorX * (1 - 2 * tl) * nhipdam;
								Line lr = Line.CreateBound(sectionPnts[0], sectionPnts[1]);
								curves.Add(lr);
							}

						}
						//Giữa nhịp không có thép
						else
						{
							if (curves.Count != 0)
							{
								CreateRebar(curves, abvtop, slcu, vectorY, cu, doc, elem, b, kc, slthepchu);
								curves = new List<Curve>();
							}
						}

						if (sl2 != 0)
						{
							slcu = sl2;
							cu = rebarBarType;
						}
						#endregion

						#region CUỐI NHỊP
						if (sl3 != 0) //Cuối nhịp
						{
							origin = pnt - vectorY * b / 2 - vectorZ * h * x + vectorX * dis; // vị trí đầu nhịp
							origin = origin - vectorZ * (abvtop + MathUtils.MmToFoot(50)); // Đưa về đúng cao độ đặt thép rồi

							double kc_TinhTuMepdenThanhThep = (b - 2 * abvtop) / (slthepchu + sl3 - 1);
							if (slthepchu == 0) kc_TinhTuMepdenThanhThep = 0;
							origin = origin + vectorY * (kc_TinhTuMepdenThanhThep + abvtop);

							rebarBarType = new FilteredElementCollector(doc)
								.OfClass(typeof(RebarBarType))
								.Cast<RebarBarType>()
								.First(xx => xx.Name == dk3);

							Lneo = double.Parse(ListMovable[0].FraInfoViewModels.Top_Anchorage) * rebarBarType.BarDiameter;

							//Tính khoảng cách các thanh thép
							if (slthepchu == 2)
							{
								kc = kc_TinhTuMepdenThanhThep;
							}
							else
							{
								kc = 2 * kc_TinhTuMepdenThanhThep;
							}

							if (Nhip != SoluongNhip - 1)
							{
								sectionPnts[0] = origin + vectorX * (1 - tl) * nhipdam;
								sectionPnts[1] = origin + vectorX * nhipdam;
								Line lr = Line.CreateBound(sectionPnts[0], sectionPnts[1]);
								curves.Add(lr);

								//CreateRebar(curves, abvtop, slcu, vectorY, cu, doc, elem, b, kc);
								//curves = new List<Curve>();

								slcu = sl3;
								cu = rebarBarType;
							}
							//Nhịp cuối cùng thì tính neo r vẽ luôn
							else
							{
								#region  Đoạn neo cuối cùng
								double ChieuDaiGoi = MathUtils.MmToFoot((ListMovable[i + 1].Length));

								sectionPnts[0] = origin + vectorX * (1 - tl) * nhipdam;
								sectionPnts[1] = origin + vectorX * nhipdam;
								Line lr = Line.CreateBound(sectionPnts[0], sectionPnts[1]);
								curves.Add(lr);

								if (ChieuDaiGoi < MathUtils.MmToFoot(300)) // Nếu gối nhỏ hơn 300 thì lùi 50 thôi và chắc chắn phải bẻ giò gà
								{
									Khoanglui = MathUtils.MmToFoot(150);
								}
								else // Nếu gối neo thẳng được thì neo thẳng ko thì tính doạn giò gà tối thiểu 10d -> 
								{
									Khoanglui = MathUtils.MmToFoot(200);
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

									CreateRebar(curves, abvtop, sl3, vectorY, rebarBarType, doc, elem, b, kc, slthepchu);
								}
								#endregion
							}
						}
						//
						else
						{
							if (curves.Count != 0)
							{
								CreateRebar(curves, abvtop, slcu, vectorY, cu, doc, elem, b, kc, slthepchu);
								curves = new List<Curve>();
							}
						}


						#endregion

						Ele_old = elTop;
						Nhip++;
					}

					//
				}
			}
			catch (Exception e)
			{
				throw;
			}
		}

		public void CreateRebar(IList<Curve> curves1, double abv, int sl, XYZ vector, RebarBarType rebarBarType, Document doc, Element elem,
			double b, double kc, int slthepchu)
		{
			try
			{
				double KC_2thanhthepchu = (b - 2 * abv) / (slthepchu - 1);
				double kc_2thanhgiacuong = 0;
				int sl1 = sl;

				int solap = 1;
				if (slthepchu == 2)
				{
					kc_2thanhgiacuong = (b - 2 * abv) / (slthepchu + sl - 1);
				}
				else if (slthepchu > 2 && sl == slthepchu - 1)
				{
					kc_2thanhgiacuong = KC_2thanhthepchu;
				}
				else if ((sl % (slthepchu - 1) == 0) && slthepchu > 0)
				{
					solap = sl / (slthepchu - 1);
					sl1 = sl / solap;
					kc_2thanhgiacuong = KC_2thanhthepchu / (sl1 + 1);
				}
				else
				{
					kc_2thanhgiacuong = (b - 2 * abv) / (slthepchu + sl - 1);
				}

				for (int j = 0; j < solap; j++)
				{
					IList<Curve> curves = new List<Curve>();

					List<XYZ> abc = new List<XYZ>();
					for (int i = 0; i < curves1.Count; i++)
					{
						if (i == 0)
						{
							abc.Add(curves1[i].GetEndPoint(0) + vector * KC_2thanhthepchu * j);
							abc.Add(curves1[i].GetEndPoint(1) + vector * KC_2thanhthepchu * j);
						}
						else
						{
							abc.Add(curves1[i].GetEndPoint(1) + vector * KC_2thanhthepchu * j);
						}
					}


					for (int i = 0; i < abc.Count - 1; i++)
					{
						Line ln = Line.CreateBound(abc[i], abc[i + 1]);
						curves.Add(ln);
					}

					IList<Curve> curvesphu = new List<Curve>();
					curvesphu = RutGonCurves(curves); // Rút gọn loại bỏ các segment

					RebarHookOrientation ho = RebarHookOrientation.Right;

					RebarHookOrientation hi = RebarHookOrientation.Right;

					Rebar stirrup = Rebar.CreateFromCurves(doc, RebarStyle.Standard, rebarBarType, null, null, elem,
						vector, curvesphu, ho, hi, true, true);

					RebarShapeDrivenAccessor rebarShapeDrivenAccessor = stirrup.GetShapeDrivenAccessor();
					if (sl == 1)
					{
						rebarShapeDrivenAccessor.SetLayoutAsFixedNumber(sl, (sl - 1) * kc, true, true, true);
					}
					else
					{
						rebarShapeDrivenAccessor.SetLayoutAsFixedNumber(sl1, kc_2thanhgiacuong * (sl1 - 1), true, true, true);
					}
				}
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
