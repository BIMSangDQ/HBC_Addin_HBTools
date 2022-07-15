using System;
using System.Collections.Generic;
using System.Linq;
using Autodesk.Revit.DB;
using Autodesk.Revit.DB.Structure;
using Utils;

namespace HoaBinhTools.FramingInformation.Models.CreateRebar
{
	public class SideBar
	{
		public void DrawSideBar(List<MovableRestBeam> ListMovable, Document doc, string rebarDia,
			int layer, int sl)
		{
			RebarBarType rebarBarType = new FilteredElementCollector(doc)
				.OfClass(typeof(RebarBarType))
				.Cast<RebarBarType>()
				.First(x => x.Name == rebarDia);

			XYZ vectorX = XYZ.BasisX;
			XYZ vectorZ = XYZ.BasisZ;
			XYZ vectorY = XYZ.BasisY;

			double dkthepgia = rebarBarType.BarDiameter;
			double abv = MathUtils.MmToFoot(double.Parse(Save.Default.ConcreteCover) + 5) + dkthepgia;

			double b_old = 0;
			double h_old = 0;
			XYZ old;
			double hesoChenhLechTietDien = MathUtils.MmToFoot(50);

			List<XYZ> sectionPnts = new List<XYZ> // Tạo để vào if gán thôi không quan trọng
			{
				new XYZ(0,0,0),
				new XYZ(0,0,0)
			};

			//Đếm số nhịp
			int SoluongNhip = 0;
			foreach (MovableRestBeam movable in ListMovable)
			{
				if (movable.Type.ToString() == "ND") SoluongNhip++;
			}

			try
			{
				for (int i = 0; i < layer; i++)
				{
					int VitriDangXet = 0;
					int Nhip = 0;
					IList<Curve> curves = new List<Curve>();
					foreach (MovableRestBeam movable in ListMovable)
					{
						if (VitriDangXet == 0) //Reset lại tiết diện đầu tiên
						{
							b_old = 0;
							h_old = 0;
						}

						string a = ListMovable[0].Type.ToString();

						if (movable.Type.ToString() == "ND")
						{
							double b = MathUtils.MmToFoot((double)movable.Width);
							double h = MathUtils.MmToFoot((double)movable.Hight);
							double elTop = MathUtils.MmToFoot((double)movable.TopElevation);
							double elBot = MathUtils.MmToFoot((double)movable.BottomElevation);

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
							XYZ origin = pnt - vectorY * b / 2 - vectorZ * h * x + vectorX * dis;

							//Check tiết diện và lệch ngang so với nhịp truóc xem có vẽ liên tục được không
							#region check để vẽ thép giá
							if (b_old == 0)// Đầu nhịp bỏ qua
							{

								if (VitriDangXet == 0)//Dầm consol
								{
									origin = origin + vectorX * MathUtils.MmToFoot(100);
								}
								else // Dầm có gối neo vào 10d
								{
									origin = origin - vectorX * 10 * dkthepgia;
								}

								sectionPnts = new List<XYZ>
								{
									origin + vectorY*abv - vectorZ*((i+1)*((h-2*abv)/(layer+1))+abv),
									origin + vectorY*abv - vectorZ*((i+1)*((h-2*abv)/(layer+1))+abv) + vectorX*(nhipdam+10*dkthepgia) ,
								};
							}
							else if (Math.Abs(h - h_old) > hesoChenhLechTietDien) // Chiều cao lệch lớn hơn thì phải tách thép.
							{
								//Tạo đoạn neo 10d cho nhịp cũ và vẽ ra
								sectionPnts[0] = sectionPnts[1];

								sectionPnts[1] = sectionPnts[1] + vectorX * (10 * dkthepgia);
								Line lr = Line.CreateBound(sectionPnts[0], sectionPnts[1]);

								curves.Add(lr);
								CreateRebar(curves, abv, sl, vectorY, rebarBarType, doc, elem, b); // vẽ thép nhịp cũ

								curves = new List<Curve>(); // Vẽ xong xoá các curve cũ
															//Tạo cho nhịp mới
								sectionPnts = new List<XYZ>
								{
								origin + vectorY*abv - vectorZ*((i+1)*((h-2*abv)/(layer+1))+abv) - vectorX*(10*dkthepgia),
								origin + vectorY*abv - vectorZ*((i+1)*((h-2*abv)/(layer+1))+abv) + vectorX*(nhipdam+10*dkthepgia) ,
								};
							}
							else
							{
								sectionPnts = new List<XYZ>
								{
								origin + vectorY*abv - vectorZ*((i+1)*((h-2*abv)/(layer+1))+abv),
								origin + vectorY*abv - vectorZ*((i+1)*((h-2*abv)/(layer+1))+abv) + vectorX*nhipdam ,
								};
							}

							#endregion

							Line lr1 = Line.CreateBound(sectionPnts[0], sectionPnts[1]);
							curves.Add(lr1);

							// Đếm xem nếu là nhịp cuối cùng thì vẽ

							if (SoluongNhip - 1 == Nhip)
							{
								sectionPnts[0] = sectionPnts[1];
								sectionPnts[1] = sectionPnts[1] + vectorX * 10 * dkthepgia;

								Line lr = Line.CreateBound(sectionPnts[0], sectionPnts[1]);
								curves.Add(lr);

								CreateRebar(curves, abv, sl, vectorY, rebarBarType, doc, elem, b); // vẽ thép nhịp cuối cùng
							}
							b_old = b;
							h_old = h;
							VitriDangXet++;
							Nhip++;

							//Vẽ đai C ngang nếu có.
							if (movable.FraInfoViewModels.IsCheckHorizontalC == true)
							{
								XYZ origin_SP = origin + vectorY * abv -
												vectorZ * ((i + 1) * ((h - 2 * abv) / (layer + 1)) + abv);
								XYZ origin_EP = origin + vectorX * nhipdam;
								Line Lr = Line.CreateBound(origin_SP, origin_EP);



								HorizontalLink horizontalLink = new HorizontalLink();
								horizontalLink.DrawHorizontalLine(doc, elem, b, movable.FraInfoViewModels.AddStirrupDiameter, Lr,
									vectorY, movable.FraInfoViewModels.KC_AddStirrup, dkthepgia);
							}
						}
						else if (movable.Type.ToString() == "GT")
						{
							VitriDangXet++;
						}
					}
				}

			}
			catch (Exception ex)
			{

			}
		}
		public void CreateRebar(IList<Curve> curves1, double abv, int sl, XYZ vector, RebarBarType rebarBarType, Document doc, Element elem, double b)
		{
			try
			{
				if (sl < 2) sl = 2;

				IList<Curve> curves = new List<Curve>();
				int i = 0;
				Curve cv_old = Line.CreateBound(new XYZ(0, 0, 0), new XYZ(1, 1, 1)) as Curve;
				Curve cv_old1 = Line.CreateBound(new XYZ(0, 0, 0), new XYZ(1, 1, 1)) as Curve;
				foreach (Curve cv in curves1)//Check và nối với đoạn cv của nhịp trước 
				{
					if (i == 0)
					{
						cv_old = cv;
						cv_old1 = cv;
					}
					else
					{
						if ((cv.GetEndPoint(0).X != cv_old1.GetEndPoint(1).X) || (cv.GetEndPoint(0).Y != cv_old1.GetEndPoint(1).Y) || (cv.GetEndPoint(0).Z != cv_old1.GetEndPoint(1).Z)) // Nếu nhịp tiếp theo khác thì vẽ lại
						{
							Line c = Line.CreateBound(cv_old.GetEndPoint(0), cv_old.GetEndPoint(1));
							curves.Add(c);

							c = Line.CreateBound(cv_old.GetEndPoint(1), cv.GetEndPoint(0));
							curves.Add(c);

							c = cv as Line;
							curves.Add(c);

							cv_old = cv;
							cv_old1 = cv;
						}
						else
						{

							Line c = Line.CreateBound(cv_old.GetEndPoint(0), cv.GetEndPoint(1));
							curves.Add(c);

							cv_old = cv;
							cv_old1 = cv;
						}
						cv_old1 = cv;
					}
					i++;
				}

				//curves = new List<Curve>();

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
