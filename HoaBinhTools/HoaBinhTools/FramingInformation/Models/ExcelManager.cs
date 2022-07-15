using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Autodesk.Revit.DB;
using HoaBinhTools.FramingInformation.Models.CreateRebar;
using HoaBinhTools.FramingInformation.Models.Extension_Models;
using HoaBinhTools.FramingInformation.Models.FramingInformationCmd;
using HoaBinhTools.FramingInformation.ViewModels;
using OfficeOpenXml;
using Utils;

namespace HoaBinhTools.FramingInformation.Models
{
	public class ExcelManager
	{
		Plane planeWork = Plane.CreateByNormalAndOrigin(XYZ.BasisZ, XYZ.Zero);

		public FramingInfoViewModels MainFramingViewModel;

		public void Exprot(FramingInfoViewModels MainFramingViewModel, Document doc)
		{
			try
			{
				this.MainFramingViewModel = MainFramingViewModel;

				var SystemFraming = ConverterCollection<BeamSystemsGroup>.ToList(MainFramingViewModel.SystemFramings);

				int SumGroupBeam = MainFramingViewModel.CountBeamSystemsGroup;

				ProgressBarView BIMExport = new ProgressBarView();

				BIMExport.Show();

				for (int Index = 0; Index < SumGroupBeam; Index++)
				{
					//SystemFraming[Index] đối tượng đặc trung trong 1 hệ Group system thường shop thì chỉ 1 cái

					if (!BIMExport.Create(MainFramingViewModel.SystemFramings.Count, " Drawing Rebar ")) break;

					Tuple<XYZ, Curve> OrginAndCurve = GetOrginAndCurveBeam(SystemFraming[Index]); // Lấy curve tổng

					List<MovableRestBeam> ValueUP =
						RunSpanAndSupport(ConverterCollection<MovableRestBeam>.ToList(SystemFraming[Index].Movable),
							SystemFraming[Index].Host, doc).SetOverlapCurve(MainFramingViewModel, SystemFraming[Index]);

					List<MovableRestBeam> ValueDown = BeamIntersectBeam(SystemFraming[Index], OrginAndCurve.Item2)
						.SetOverlapCurve(MainFramingViewModel, SystemFraming[Index]);

					//Vẽ thép đai của các nhịp dầm
					StirrupRebar stirrupRebar = new StirrupRebar();
					stirrupRebar.DrawMainStirrup(ValueUP, doc);

					//Vẽ thép giá và đai thép giá nếu có
					if (MainFramingViewModel.LayerSide > 0) // Nếu số lớp > 0
					{
						SideBar sideBar = new SideBar();
						sideBar.DrawSideBar(ValueUP, doc, MainFramingViewModel.SideBar_Dia,
							MainFramingViewModel.LayerSide, MainFramingViewModel.CountSideBar);
					}

					//Vẽ thép chủ T1 
					if (MainFramingViewModel.SL_T1 < 2) MainFramingViewModel.SL_T1 = 2;
					TopMainRebar topMainRebar = new TopMainRebar();
					topMainRebar.DrawTopMainBar(ValueUP, doc);

					//Vẽ thép gia cường T1
					AddT1 addT1 = new AddT1();
					addT1.DrawAddRebar(ValueUP, doc);

					//Vẽ thép chủ T2
					if (MainFramingViewModel.SL_T2 >= 2)
					{
						TopMainRebarT2 topMainRebarT2 = new TopMainRebarT2();
						topMainRebarT2.DrawTopMainBar(ValueUP, doc);
					}

					//Vẽ thép gia cường T2
					AddT2 addT2 = new AddT2();
					addT2.DrawAddRebar(ValueUP, doc);

					try
					{
						//Check vẽ đai C lớp T2
						if (MainFramingViewModel.IsCheckCstirrup == true)
						{
							AddHorizontalLink addHorizontal = new AddHorizontalLink();
							addHorizontal.DrawHorizontalLine(ValueUP, doc);
							AddhorizontalLinkBot addHorizontalbot = new AddhorizontalLinkBot();
							addHorizontalbot.DrawHorizontalLine(ValueUP, doc);
						}
					}
					catch { }
					//Vẽ thép chủ B1
					if (MainFramingViewModel.SL_B1 < 2) MainFramingViewModel.SL_B1 = 2;
					BotMainRebar botMainRebar = new BotMainRebar();
					botMainRebar.DrawBotMainBar(ValueUP, doc);

					//Vẽ thép gia cường B1
					AddB1 addB1 = new AddB1();
					addB1.DrawAddRebar(ValueUP, doc);

					//Vẽ thép chủ B2
					if (MainFramingViewModel.SL_B2 >= 2)
					{
						BotMainRebarB2 botMainRebarB2 = new BotMainRebarB2();
						botMainRebarB2.DrawBotMainBar(ValueUP, doc);
					}

					//Vẽ thép gia cường B2
					AddB2 addB2 = new AddB2();
					addB2.DrawAddRebar(ValueUP, doc);

					//Vẽ thép đai phụ
					Ver_CStirrup verC = new Ver_CStirrup();
					verC.DrawCStirrup(ValueUP, doc);

					Ver_Stirrup verStirrup = new Ver_Stirrup();
					verStirrup.DrawStirrup(ValueUP, doc);
				}

				BIMExport.Close();

				//FramingInfoViewModels.excelPackage.Save();

				//Process.Start(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "\\Autodesk\\Revit\\Addins" + "\\" + ActiveData.Application.VersionNumber + "\\BIM_HBC\\DirectorySupport\\Library\\HB_Main.xlsm");
			}
			catch (Exception ex)
			{
				MainFramingViewModel.ListErro.Add(new ErrorModels()
				{ ErrorID = ElementId.InvalidElementId, Category = "Loi toan cuc", InfoErro = ex.ToString() });
			}
		}

		public void WriteOpening(ExcelWorksheet excelWorksheet, List<OpeningModels> OpN, int Index)
		{
			if (OpN == null) return;

			int IndexColu = 3;


			foreach (var p in OpN)
			{
				if (p.Shape == OpeningModels.ShapeOpening.HCN)
				{
					excelWorksheet.Cells[(Index * 24) + 22, IndexColu].Value = string.Format("{0}/{1}/{2}/{3}/{4}",
						p.Shape, Math.Round(p.DistanceX), Math.Round(p.DistanceY), Math.Round(p.Width),
						Math.Round(p.Height));
				}
				else
				{
					excelWorksheet.Cells[(Index * 24) + 22, IndexColu].Value = string.Format("{0}/{1}/{2}/{3}", p.Shape,
						Math.Round(p.DistanceX), Math.Round(p.DistanceY), Math.Round(p.Width));
				}

				IndexColu++;
			}
		}


		public List<OpeningModels> GetInfoOpening(List<Element> ops, Curve curX, XYZ orgin, Curve curY)
		{
			List<OpeningModels> Opens = new List<OpeningModels>();

			foreach (var e in ops)
			{
				if (e is Opening op)
				{
					if (!op.IsArcBoundary())
					{
						var OpModel = new OpeningModels();

						OpModel.Shape = OpeningModels.ShapeOpening.HCN;

						var BxH = op.GetBxHInRectangle();

						OpModel.Width = BxH.Item1;

						OpModel.Height = BxH.Item2;

						OpModel.DistanceX = op.GetDistanceX(curX, orgin);

						OpModel.DistanceY = op.GetDistanceY(curY);

						Opens.Add(OpModel);
					}
					else
					{
						var OpModel = new OpeningModels();

						OpModel.Shape = OpeningModels.ShapeOpening.HT;

						var Ar = op.GetRadiusOnOpening();

						OpModel.Width = Ar;

						OpModel.DistanceX = op.GetDistanceX(curX, orgin);

						OpModel.DistanceY = op.GetDistanceY(curY);

						Opens.Add(OpModel);
					}
				}
				else
				{
					Element Type = ActiveData.Document.GetElement(e.GetTypeId());

					if (Type.LookupParameter("HB_Void").AsString() == "C")
					{
						var OpModel = new OpeningModels();

						OpModel.Shape = OpeningModels.ShapeOpening.HT;

						OpModel.Width = Type.LookupParameter("R").AsDouble().FootToMm();

						OpModel.DistanceX = e.GetCenterGeneric().GetDistanceX(curX, orgin);

						OpModel.DistanceY = e.GetCenterGeneric().GetDistanceY(curY);

						Opens.Add(OpModel);
					}

					if (Type.LookupParameter("HB_Void").AsString() == "R")
					{
						var OpModel = new OpeningModels();

						OpModel.Shape = OpeningModels.ShapeOpening.HCN;

						OpModel.Width = e.LookupParameter("b").AsDouble().FootToMm();

						OpModel.Height = e.LookupParameter("h").AsDouble().FootToMm();

						OpModel.DistanceX = e.GetCenterGeneric().GetDistanceX(curX, orgin);

						OpModel.DistanceY = e.GetCenterGeneric().GetDistanceY(curY);

						Opens.Add(OpModel);
					}
				}
			}

			return Opens;
		}


		public Tuple<XYZ, Curve> GetOrginAndCurveBeam(BeamSystemsGroup GroupBeam)
		{
			IList<XYZ> ListXYZ = new List<XYZ>();

			List<double> weights = new List<double>();

			XYZ sortXYZ = GroupBeam.Host.OrginPoint;

			foreach (MovableRestBeam CurBeam in GroupBeam.Movable)
			{
				// A B C D E

				var curve = CurBeam.Curve;

				var p1 = curve.GetEndPoint(0);

				var p5 = curve.GetEndPoint(1);

				var p3 = (p1 + p5) / 2;
				p3 = curve.Project(p3).XYZPoint;

				var p2 = (p1 + p3) / 2;
				p2 = curve.Project(p2).XYZPoint;

				var p4 = (p3 + p5) / 2;
				p4 = curve.Project(p4).XYZPoint;

				ListXYZ.Add(p1);

				ListXYZ.Add(p2);

				ListXYZ.Add(p3);

				ListXYZ.Add(p4);

				ListXYZ.Add(p5);
			}


			var ListPoint = ListXYZ.Distinct(new XYZEqualityComparer()).ToList();

			ListPoint.Sort(new SortPoint(sortXYZ));

			Curve Cur = NurbSpline.CreateCurve(HermiteSpline.Create(ListPoint, false));


			var pA = Cur.GetEndPoint(0);

			var pB = Cur.GetEndPoint(1);

			return Tuple.Create(pA.DistanceTo(sortXYZ) < pB.DistanceTo(sortXYZ) ? pA : pB, Cur);
		}


		public void WriteElevationTop(ExcelWorksheet excelWorksheet, List<MovableRestBeam> ValueUP, int Index)
		{
			// viết gia trị đầu tiên 
			double TemporaryVariable = 0;

			int cout = 3;

			foreach (var Movab in ValueUP)
			{
				if (Movab.Type != TypeAdjSupport.ND) continue;

				var Elevation = Movab.TopElevation;

				if (cout == 3)
				{
					excelWorksheet.Cells[(Index * 24) + 8, 2].Value = Elevation;

					TemporaryVariable = (double)Elevation;

					cout += 2;
				}
				else
				{
					if (Math.Abs((double)TemporaryVariable - (double)Elevation) > 0.01)
					{
						excelWorksheet.Cells[(Index * 24) + 10, cout].Value = Elevation - TemporaryVariable;

						TemporaryVariable = (double)Elevation;
					}

					cout += 2;
				}
			}
		}


		public void WriteElevationBottom(ExcelWorksheet excelWorksheet, List<MovableRestBeam> ValueUP, int Index)
		{
			// viết gia trị đầu tiên 

			double TemporaryVariable = 0;

			int cout = 3;

			foreach (var Movab in ValueUP)
			{
				if (Movab.Type != TypeAdjSupport.ND) continue;

				var Elevation = Movab.BottomElevation ?? 0;

				if (cout == 3)
				{
					//excelWorksheet.Cells[8, 2].Value = Elevation;

					TemporaryVariable = (double)Elevation;

					cout += 2;
				}
				else
				{
					if (Math.Abs((double)TemporaryVariable - (double)Elevation) > 0.01)
					{
						excelWorksheet.Cells[(Index * 24) + 17, cout].Value = Elevation - TemporaryVariable;

						TemporaryVariable = (double)Elevation;
					}

					cout += 2;
				}
			}
		}


		public List<MovableRestBeam> RunSpanAndSupport(List<MovableRestBeam> Values, HostFraming Host, Document doc)
		{
			List<MovableRestBeam> ReturnSystemFraming = new List<MovableRestBeam>();

			Solid SolidAdj = null;

			foreach (var Fra in Values)
			{
				try
				{
					if (Fra.Type == TypeAdjSupport.GT)
					{
						ReturnSystemFraming.Add(Fra);

						var curloop = CurveLoop.CreateViaThicken(Fra.Curve, 100, XYZ.BasisZ);

						var soild = curloop.GetSolidToCurveloop();

						if (SolidAdj == null)
						{
							SolidAdj = soild;
						}
						else
						{
							BooleanOperationsUtils.ExecuteBooleanOperationModifyingOriginalSolid(SolidAdj, soild,
								BooleanOperationsType.Union);
						}
					}
				}
				catch (Exception ex)
				{
					MainFramingViewModel.ListErro.Add(new ErrorModels()
					{ ErrorID = Host.ID, Category = Host.Name, InfoErro = ex.ToString() });
				}
			}

			// nhip 
			foreach (var Beam in Host.ListHost)
			{
				FamilyInstance FamiInstane = (Beam as FamilyInstance);

				Curve SpanBeam = FamiInstane.GetLocationCurve();

				var With_Hight = FamiInstane.GetWidthAndHight(doc);

				List<Curve> Curs = null;

				if (SolidAdj != null)
				{
					Curs = SpanBeam.GetListBeamsSpanIntersect(SolidAdj, Host.OrginPoint, planeWork);
				}
				else
				{
					Curs = new List<Curve> { SpanBeam };
				}

				foreach (var curBemaPan in Curs)
				{
					try
					{
						MovableRestBeam Adj = new MovableRestBeam(MainFramingViewModel);

						Adj.Id = Beam.Id;

						Adj.Length = ((int)Math.Round(curBemaPan.Length.FootToMm(), 0)).Round5();

						Adj.Curve = curBemaPan;

						Adj.Category = string.Format("Host: {0}", Beam.Name);

						Adj.Width = With_Hight.Item1;

						Adj.Hight = With_Hight.Item2;

						Adj.TopElevation = Beam.GetBeamElevationTop();

						Adj.BottomElevation = Beam.GetBeamElevationBottom();

						Adj.Type = TypeAdjSupport.ND;

						ReturnSystemFraming.Add(Adj);
					}
					catch (Exception ex)
					{
						MainFramingViewModel.ListErro.Add(new ErrorModels()
						{ ErrorID = Host.ID, Category = Host.Name, InfoErro = ex.ToString() });
					}
				}
			}

			ReturnSystemFraming.Sort(new MovableRestBeamSort(Host.OrginPoint));

			return ReturnSystemFraming;
		}


		public List<MovableRestBeam> BeamIntersectBeam(BeamSystemsGroup GroupBeam, Curve CurLink)
		{
			List<MovableRestBeam> ReturnSystemFraming = new List<MovableRestBeam>();

			Solid SolidAdj = null;

			List<Element> EleGT = new List<Element>();

			HostFraming Host = GroupBeam.Host;

			List<MovableRestBeam> Values = ConverterCollection<MovableRestBeam>.ToList(GroupBeam.Movable);

			foreach (var Fra in Values)
			{
				try
				{
					if (Fra.Type == TypeAdjSupport.DP ||
						(Fra.Type == TypeAdjSupport.GT &&
						 ActiveData.Document.GetElement(Fra.Id).Category.Id.IntegerValue ==
						 (int)BuiltInCategory.OST_StructuralFraming))
					{
						ReturnSystemFraming.Add(Fra);

						var curloop = CurveLoop.CreateViaThicken(Fra.Curve, 100, XYZ.BasisZ);

						var soild = curloop.GetSolidToCurveloop();

						if (SolidAdj == null)
						{
							SolidAdj = soild;
						}
						else
						{
							BooleanOperationsUtils.ExecuteBooleanOperationModifyingOriginalSolid(SolidAdj, soild,
								BooleanOperationsType.Union);
						}
					}
				}
				catch (Exception ex)
				{
					MainFramingViewModel.ListErro.Add(new ErrorModels()
					{ ErrorID = Host.ID, Category = Host.Name, InfoErro = ex.ToString() });
				}
			}


			if (SolidAdj != null)
			{
				List<Curve> Curs = CurLink.GetListBeamsSpanIntersect(SolidAdj, Host.OrginPoint, planeWork);

				foreach (var curBemaPan in Curs)
				{
					try
					{
						MovableRestBeam Adj = new MovableRestBeam(MainFramingViewModel);

						Adj.Id = Host.ID;

						Adj.Length = ((int)Math.Round(curBemaPan.Length.FootToMm(), 0)).Round5();

						Adj.Curve = curBemaPan;

						Adj.Category = string.Format("Host: {0}", Host.Name);

						Adj.Width = Host.Width;

						Adj.Hight = Host.Hight;

						Adj.Type = TypeAdjSupport.ND;

						ReturnSystemFraming.Add(Adj);
					}
					catch (Exception ex)
					{
						MainFramingViewModel.ListErro.Add(new ErrorModels()
						{ ErrorID = Host.ID, Category = Host.Name, InfoErro = ex.ToString() });
					}
				}

				ReturnSystemFraming.Sort(new MovableRestBeamSort(Host.OrginPoint));
			}

			// nêu không có cái nào thuộc dòng dưới thì thôi tra về lại nguyên trạng
			else
			{
				foreach (var Fra in Values)
				{
					try
					{
						MovableRestBeam Adj = new MovableRestBeam(MainFramingViewModel);

						Adj.Id = Fra.Id;

						Adj.Length = ((int)Math.Round(Fra.Length.FootToMm(), 0)).Round5();

						Adj.Curve = Fra.Curve;

						Adj.Category = Fra.Category;

						Adj.Width = Host.Width;

						Adj.Hight = Host.Hight;

						Adj.Type = TypeAdjSupport.ND;

						ReturnSystemFraming.Add(Adj);
					}
					catch (Exception ex)
					{
						MainFramingViewModel.ListErro.Add(new ErrorModels()
						{ ErrorID = Host.ID, Category = Host.Name, InfoErro = ex.ToString() });
					}
				}
			}

			return ReturnSystemFraming;
		}


		public void WriteGrid(ExcelWorksheet excelWorksheet, List<Grid> Grids, XYZ orgin, int Index)
		{
			if (Grids == null) return;

			int IndexColu = 3;


			orgin = orgin.ProjectOnto(planeWork);

			double TPM = 0;

			foreach (var grid in Grids)
			{
				var cur = grid.Curve.ProjectCurveToPlane(planeWork);

				var DistanGrid = cur.Project(orgin).Distance.FootToMm();

				excelWorksheet.Cells[(Index * 24) + 20, IndexColu].Value =
					((int)Math.Round((DistanGrid - TPM), 0)).Round5();

				excelWorksheet.Cells[(Index * 24) + 21, IndexColu].Value = grid.Name;

				TPM = DistanGrid;

				IndexColu++;
			}
		}


		public void WriteInfoHost(ExcelWorksheet excelWorksheet, HostFraming Host, int Index)
		{
			//name
			excelWorksheet.Cells[(Index * 24) + 3, 2].Value = Host.Name;


			//Hight
			excelWorksheet.Cells[(Index * 24) + 5, 2].Value = Host.Hight;

			// with
			excelWorksheet.Cells[(Index * 24) + 6, 2].Value = Host.Width;
		}


		public void WriteDownGeomentry(List<MovableRestBeam> ListMovable, ExcelWorksheet excelWorksheet, int Index)
		{
			int IndexColu = 3;

			foreach (var movable in ListMovable)
			{
				// nếu giá trị đầu tiên mà cột 
				if (ListMovable.First().Type == TypeAdjSupport.ND && IndexColu == 3)
				{
					excelWorksheet.Cells[(Index * 24) + 18, IndexColu].Value = 0;
					excelWorksheet.Cells[(Index * 24) + 19, IndexColu].Formula = "=b7";

					IndexColu++;
				}

				excelWorksheet.Cells[(Index * 24) + 18, IndexColu].Value = movable.Length;

				// nếu trung ô lẽ là ô dầm trực giao thì 
				if (IndexColu % 2 != 0)
				{
					excelWorksheet.Cells[(Index * 24) + 19, IndexColu].Value = movable.Hight;
				}

				IndexColu++;
			}
		}


		public void WriteUpGeomentry(List<MovableRestBeam> ListMovable, int Index)
		{
			int IndexColu = 3;

			foreach (MovableRestBeam movable in ListMovable)
			{
				//if (ListMovable.First().Type == TypeAdjSupport.ND && IndexColu == 3)
				//{
				//    excelWorksheet.Cells[(Index * 24) + 9, IndexColu].Value = 0;

				//    excelWorksheet.Cells[(Index * 24) + 8, IndexColu].Value = "CL";

				//    IndexColu++;
				//}

				//excelWorksheet.Cells[(Index * 24) + 9, IndexColu].Value = movable.Length;

				//if (movable.Category.Contains("Framing"))
				//{
				//    excelWorksheet.Cells[(Index * 24) + 8, IndexColu].Value = "BE";
				//}
				//else if (movable.Category.Contains("Foundations"))
				//{
				//    excelWorksheet.Cells[(Index * 24) + 8, IndexColu].Value = "FO";
				//}
				//else if (movable.Category.Contains("Walls"))
				//{
				//    excelWorksheet.Cells[(Index * 24) + 8, IndexColu].Value = "WL";
				//}
				//else if (movable.Category.Contains("Host"))
				//{
				//    excelWorksheet.Cells[(Index * 24) + 8, IndexColu].Value = "SB";
				//}
				//else if (movable.Category.Contains("Columns"))
				//{
				//    excelWorksheet.Cells[(Index * 24) + 8, IndexColu].Value = "CL";
				//}
				//else
				//{
				//    excelWorksheet.Cells[(Index * 24) + 8, IndexColu].Value = "Error";
				//}


				IndexColu++;
			}
		}


		public void RemoveData(ExcelWorksheet excelWorksheet, int Index)
		{
			for (int i = 3; i < excelWorksheet.Dimension.End.Column; i++)
			{
				excelWorksheet.Cells[(Index * 24) + 9, i].Value = string.Empty;
				excelWorksheet.Cells[(Index * 24) + 10, i].Value = string.Empty;
				excelWorksheet.Cells[(Index * 24) + 11, i].Value = string.Empty;
				excelWorksheet.Cells[(Index * 24) + 12, i].Value = string.Empty;
				excelWorksheet.Cells[(Index * 24) + 13, i].Value = string.Empty;
				excelWorksheet.Cells[(Index * 24) + 14, i].Value = string.Empty;
				excelWorksheet.Cells[(Index * 24) + 15, i].Value = string.Empty;
				excelWorksheet.Cells[(Index * 24) + 16, i].Value = string.Empty;
				excelWorksheet.Cells[(Index * 24) + 17, i].Value = string.Empty;
				excelWorksheet.Cells[(Index * 24) + 18, i].Value = string.Empty;
				excelWorksheet.Cells[(Index * 24) + 19, i].Value = string.Empty;
				excelWorksheet.Cells[(Index * 24) + 20, i].Value = string.Empty;
				excelWorksheet.Cells[(Index * 24) + 21, i].Value = string.Empty;
				excelWorksheet.Cells[(Index * 24) + 22, i].Value = string.Empty;
			}
		}


		public bool IsRevitRunning()
		{
			return (uint)Process.GetProcessesByName("Excel").Length > 0U;
		}

		public void CloseRevitApp()
		{
			Process.GetProcesses();
			foreach (Process process in Process.GetProcesses())
			{
				string upper = process.ProcessName.ToUpper();
				if ((upper.Contains(".xlsm") || process.MainWindowTitle.Contains("HB_Main")))
				{
					process.Kill();

					process.WaitForExit();
				}
			}
		}


		public bool CheckTypeSpanUp(MovableRestBeam Movable1, MovableRestBeam Movable2)
		{
			// giá trị tiếp theo mà khác
			if ((Movable1.Type == TypeAdjSupport.DP || Movable1.Type == TypeAdjSupport.ND) &&
				(Movable2.Type == TypeAdjSupport.GT))
			{
				return true;
			}

			return false;
		}

		public bool CheckTypeSupportUp(MovableRestBeam Movable1, MovableRestBeam Movable2)
		{
			// giá trị tiếp theo mà khác
			if (Movable1.Type == TypeAdjSupport.GT &&
				(Movable2.Type == TypeAdjSupport.ND || Movable2.Type == TypeAdjSupport.DP))
			{
				return true;
			}

			return false;
		}
	}
}