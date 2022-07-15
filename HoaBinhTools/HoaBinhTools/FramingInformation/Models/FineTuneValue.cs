using System;
using System.Collections.Generic;
using System.Linq;
using Autodesk.Revit.DB;
using HoaBinhTools.FramingInformation.Models.FramingInformationCmd;
using HoaBinhTools.FramingInformation.ViewModels;
using Utils;

namespace HoaBinhTools.FramingInformation.Models
{
	public static class FineTuneValue
	{
		public static List<MovableRestBeam> SetOverlapCurve(this List<MovableRestBeam> Movable, FramingInfoViewModels MainFramingViewModel, BeamSystemsGroup GroupBeam)
		{
			List<MovableRestBeam> ReturnValue = new List<MovableRestBeam>();

			MovableRestBeam TPM;

			double? Hig = 0;

			//double Wid= 0;

			List<XYZ> ListCurveOverlap = new List<XYZ>();


			for (int i = 0; i < Movable.Count; i++)
			{

				try
				{

					var p1 = Movable[i].Curve.GetEndPoint(0);

					var p5 = Movable[i].Curve.GetEndPoint(1);

					var p3 = (p1 + p5) / 2;
					p3 = Movable[i].Curve.Project(p3).XYZPoint;

					var p2 = (p1 + p3) / 2;
					p2 = Movable[i].Curve.Project(p2).XYZPoint;

					var p4 = (p3 + p5) / 2;
					p4 = Movable[i].Curve.Project(p4).XYZPoint;

					ListCurveOverlap.Add(p1);

					ListCurveOverlap.Add(p2);

					ListCurveOverlap.Add(p3);

					ListCurveOverlap.Add(p4);

					ListCurveOverlap.Add(p5);

					// Lấy chiều cao và chiều rộng max nhất
					if (Hig < Movable[i].Hight)
					{
						Hig = Movable[i].Hight;
					}

					if (i == Movable.Count - 1)
					{
						if (ListCurveOverlap.Count > 5)
						{
							TPM = new MovableRestBeam();

							Curve cur = GetMaxLength(ListCurveOverlap, GroupBeam);

							// LÀM TRÒN 
							if (Movable[i].Type == TypeAdjSupport.ND)
							{
								TPM.Length = ((int)cur.Length.FootToMm()).Round10(); ;
							}
							else
							{
								TPM.Length = ((int)cur.Length.FootToMm()).Round5();
							}
							// XET CHIỀU CAO KHI SÁT NHAU
							if (Hig > 0)
							{
								TPM.Hight = Hig;
							}
							else
							{
								TPM.Hight = Movable[i].Hight;
							}

							Hig = 0;

							TPM.Type = Movable[i].Type;

							TPM.Category = Movable[i].Category;

							TPM.Id = Movable[i].Id;

							TPM.Curve = cur;

							TPM.TopElevation = Movable[i].TopElevation;

							TPM.BottomElevation = Movable[i].BottomElevation;

							ReturnValue.Add(TPM);

						}
						else
						{
							ReturnValue.Add(Movable[i]);
						}

						ListCurveOverlap.Clear();
					}

					else
					{
						if (Movable[i].Type != Movable[i + 1].Type)
						{
							// tại vị trí đang xét nó có trùng nhau hoặc là đơn

							// nếu ở giá trị đơn thì không xử lý gì 

							if (ListCurveOverlap.Count > 5)
							{

								TPM = new MovableRestBeam();

								Curve cur = GetMaxLength(ListCurveOverlap, GroupBeam);

								// nếu là nhịp dầm thì nghĩa là dầm chủ lấy luôn H max và B max
								if (Movable[i].Type == TypeAdjSupport.ND)
								{

									// Lấy giá trị H max của dầm liên tục
									if (Hig > 0)
									{
										TPM.Hight = Hig;
									}
									else
									{
										TPM.Hight = Movable[i].Hight;
									}

									Hig = 0;

									TPM.Length = ((int)cur.Length.FootToMm()).Round10();
								}


								// không phải nhíp dầm
								else
								{
									// nếu là nó lớn hơn không
									if (Hig > 0)
									{
										TPM.Hight = Hig;
									}
									else
									{
										TPM.Hight = Movable[i].Hight;
									}

									//Wid = 0;
									Hig = 0;

									// ở chổ này length đã thay with rồi nên ko xét wid
									TPM.Length = ((int)cur.Length.FootToMm()).Round5();
								}


								TPM.Type = Movable[i].Type;

								TPM.Category = Movable[i].Category;

								TPM.Id = Movable[i].Id;

								TPM.TopElevation = Movable[i].TopElevation;

								TPM.BottomElevation = Movable[i].BottomElevation;

								TPM.Curve = cur;

								ReturnValue.Add(TPM);

							}

							// nếu là gần kề khác nhau thì xet lại
							else
							{
								ReturnValue.Add(Movable[i]);

								//Wid = 0;
								Hig = 0;
							}

							ListCurveOverlap.Clear();
						}
					}
				}

				catch (Exception ex)
				{
					MainFramingViewModel.ListErro.Add(new ErrorModels() { ErrorID = Movable[i].Id, Category = Movable[i].Category, InfoErro = ex.ToString() });

				}

			}
			return ReturnValue;

		}

		private static Curve GetMaxLength(List<XYZ> ListXYZ, BeamSystemsGroup GroupBeam)
		{

			var ListPoint = ListXYZ.Distinct(new XYZEqualityComparer()).ToList();

			XYZ sortXYZ = GroupBeam.Host.OrginPoint;

			ListPoint.Sort(new SortPoint(sortXYZ));


			Curve Cur = NurbSpline.CreateCurve(HermiteSpline.Create(ListPoint, false));
			//double length = 0;

			//Curve Tpmcur = null;

			//for (int i = 0; i < Cur.Count-1; i++)
			//{
			//    for (int j = i+1; j < Cur.Count; j++)
			//    {
			//        if(Cur[i].DistanceTo(Cur[j]) > length)
			//        {
			//            Tpmcur = Line.CreateBound(Cur[i], Cur[j]);
			//            length = Tpmcur.Length;
			//        }
			//    }
			//}
			return Cur;
		}
	}
}
