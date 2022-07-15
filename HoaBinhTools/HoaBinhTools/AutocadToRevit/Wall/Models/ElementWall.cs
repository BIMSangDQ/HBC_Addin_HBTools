using System;
using System.Collections.Generic;
using Autodesk.Revit.DB;
using HoaBinhTools.AutocadToRevit.Wall.ViewModels;
using Utils;

namespace HoaBinhTools.AutocadToRevit.Wall.Models
{
	class ElementWall
	{
		public WallViewModel WVM;

		public ElementWall(WallViewModel WVM)
		{
			this.WVM = WVM;
		}

		public ElementId instanceId(List<Curve> listCurves)
		{
			try
			{
				if (listCurves.Count == 2)
				{
					Line line1 = listCurves[0] as Line;
					Line line2 = listCurves[1] as Line;

					WallType wt = WVM.ListWallTypes[0].WallTypeSelect;
					foreach (ItemWallType item in WVM.ListWallTypes)
					{
						if (Math.Round((item.With + 2).MmToFoot(), 8) >
							Math.Round(CurveUtils.Distance2Lines(line1, line2), 8) &&
							Math.Round((item.With - 2).MmToFoot(), 8) <
							Math.Round(CurveUtils.Distance2Lines(line1, line2), 8))
						{
							wt = item.WallTypeSelect;
						}
					}

					//IList<Curve> listCV = new List<Curve>();
					//listCV.Add(line1);

					XYZ SPline1 = line1.GetEndPoint(0);
					XYZ EPline1 = line1.GetEndPoint(1);
					XYZ SPline2 = line2.GetEndPoint(0);
					XYZ EPline2 = line2.GetEndPoint(1);

					Line line = Line.CreateBound(SPline1, EPline1);

					if (XYZUtils.Distance2Point(SPline1, SPline2) < XYZUtils.Distance2Point(SPline1, EPline2))
					{
						line = Line.CreateBound(XYZUtils.Midpoint(SPline1, SPline2),
							XYZUtils.Midpoint(EPline1, EPline2));
					}
					else
					{
						line = Line.CreateBound(XYZUtils.Midpoint(SPline1, EPline2),
							XYZUtils.Midpoint(EPline1, SPline2));
					}

					Curve cv = line as Curve;
					Autodesk.Revit.DB.Wall instance = WallModel.CreateWall(WVM.Doc, cv, wt, WVM.BaseLevel.Id,
						WVM.TopLevel.Id, WVM.BaseOffset, WVM.TopOffset);
					return instance.Id;
				}
				else // Nếu tồn tại lớn hơn 2 line
				{
					Line line1 = maxLenght(listCurves);
					XYZ SPline1 = line1.GetEndPoint(0);
					XYZ EPline1 = line1.GetEndPoint(1);

					Line line2 = listCurves[0] as Line;

					if (CurveUtils.Distance2Lines(line1, line2) == 0)
					{
						line2 = listCurves[1] as Line;
					}

					WallType wt = WVM.ListWallTypes[0].WallTypeSelect; // Lấy walltype
					foreach (ItemWallType item in WVM.ListWallTypes)
					{
						if (Math.Round((item.With + 2).MmToFoot(), 8) >
							Math.Round(CurveUtils.Distance2Lines(line1, line2), 8) &&
							Math.Round((item.With - 2).MmToFoot(), 8) <
							Math.Round(CurveUtils.Distance2Lines(line1, line2), 8))
						{
							wt = item.WallTypeSelect;
						}
					}

					// Tìm điểm đầu
					XYZ SPline = new XYZ(0, 0, 0);
					double disStartpoint = 10000;
					foreach (Curve cv in listCurves)
					{
						Line line = cv as Line;
						XYZ SPline2 = line.GetEndPoint(0);
						XYZ EPline2 = line.GetEndPoint(1);

						if (disStartpoint > XYZUtils.Distance2Point(SPline1, SPline2) &&
							XYZUtils.Distance2Point(SPline1, SPline2) != 0)
						{
							disStartpoint = XYZUtils.Distance2Point(SPline1, SPline2);
							SPline = XYZUtils.Midpoint(SPline1, SPline2);
						}

						if (disStartpoint > XYZUtils.Distance2Point(SPline1, EPline2) &&
							XYZUtils.Distance2Point(SPline1, EPline2) != 0)
						{
							disStartpoint = XYZUtils.Distance2Point(SPline1, EPline2);
							SPline = XYZUtils.Midpoint(SPline1, EPline2);
						}
					}

					XYZ EPline = new XYZ(0, 0, 0);
					double disEndpoint = 10000;
					foreach (Curve cv in listCurves)
					{
						Line line = cv as Line;
						XYZ SPline2 = line.GetEndPoint(0);
						XYZ EPline2 = line.GetEndPoint(1);

						if (disEndpoint > XYZUtils.Distance2Point(EPline1, SPline2) &&
							XYZUtils.Distance2Point(EPline1, SPline2) != 0)
						{
							disEndpoint = XYZUtils.Distance2Point(EPline1, SPline2);
							EPline = XYZUtils.Midpoint(EPline1, SPline2);
						}

						if (disEndpoint > XYZUtils.Distance2Point(EPline1, EPline2) &&
							XYZUtils.Distance2Point(EPline1, EPline2) != 0)
						{
							disEndpoint = XYZUtils.Distance2Point(EPline1, EPline2);
							EPline = XYZUtils.Midpoint(EPline1, EPline2);
						}
					}

					Line lineDraw = Line.CreateBound(SPline, EPline);

					Curve cv1 = lineDraw as Curve;
					Autodesk.Revit.DB.Wall instance = WallModel.CreateWall(WVM.Doc, cv1, wt, WVM.BaseLevel.Id,
						WVM.TopLevel.Id, WVM.BaseOffset, WVM.TopOffset);
					return instance.Id;
				}
			}
			catch (Exception e)
			{
				throw;
			}
		}

		public Line maxLenght(List<Curve> listCurves)
		{
			Line line1;
			double lenght = 0;
			foreach (Curve cv in listCurves) // Lấy line dài nhất
			{
				if (cv.Length > lenght)
				{
					lenght = cv.Length;
				}
			}

			foreach (Curve cv in listCurves)
			{
				if (cv.Length == lenght)
				{
					line1 = cv as Line;
					return line1;
				}
			}

			return line1 = Line.CreateBound(new XYZ(0, 0, 0), new XYZ(0, 1, 0));
		}

		//public static double Distance2Point(XYZ p1, XYZ p2)
		//      {
		//       double d = Math.Sqrt((p1.X - p2.X) * (p1.X - p2.X) + (p1.Y - p2.Y) * (p1.Y - p2.Y));
		//       return d;
		//      }
	}
}