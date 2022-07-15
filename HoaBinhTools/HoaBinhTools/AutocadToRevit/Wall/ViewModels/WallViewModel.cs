using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Forms;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using HoaBinhTools.AutocadToRevit.Utils;
using HoaBinhTools.AutocadToRevit.Wall.Models;
using HoaBinhTools.AutocadToRevit.Wall.Views;
using Utils;


namespace HoaBinhTools.AutocadToRevit.Wall.ViewModels
{
	public class WallViewModel : ViewModelBase
	{
		#region variable
		public RelayCommand btnOk { get; set; }

		public RelayCommand btnAdd { get; set; }

		public RelayCommand btnDel { get; set; }

		WallView WallMainView { get; set; }

		public WallViewModel(ImportInstance rr)
		{
			btnOk = new RelayCommand(BtnOk);

			btnDel = new RelayCommand(BtnDel);

			btnAdd = new RelayCommand(BtnAdd);

			SelectedCadLink = rr;
		}

		public UIDocument UIdoc;
		public Document Doc;
		public ImportInstance SelectedCadLink = null;

		private List<string> allLayers()
		{
			List<string> all = AcadUtils.GetAllLayer(SelectedCadLink);
			return all;
		}

		public List<string> AllLayers
		{
			get { return allLayers(); }
		}

		private string selectedLayer = null;

		public List<WallType> AllWallTypes { get; set; }

		private WallType selectedWallTypes = null;

		public WallType SelectedWallTypes
		{
			get { return selectedWallTypes; }
			set { this.selectedWallTypes = value; }
		}

		public string SelectedLayer
		{
			get { return selectedLayer; }
			set { this.selectedLayer = value; }
		}

		public List<Level> AllLevel { get; set; }

		private Level baseLevel = null;

		public Level BaseLevel
		{
			get { return baseLevel; }
			set { this.baseLevel = value; }
		}

		private Level topLevel = null;

		public Level TopLevel
		{
			get { return topLevel; }
			set { this.topLevel = value; }
		}

		private double baseOffset = 0;

		public double BaseOffset
		{
			get { return baseOffset; }
			set { this.baseOffset = value; }
		}

		private double topOffset = 0;

		public double TopOffset
		{
			get { return topOffset; }
			set { this.topOffset = value; }
		}

		private ObservableCollection<ItemWallType> listWallTypes = new ObservableCollection<ItemWallType>();
		public ObservableCollection<ItemWallType> ListWallTypes
		{
			get { return listWallTypes; }
		}

		public ItemWallType SelectWallTypeItem { set; get; }

		#endregion
		public void Execute(UIDocument uidoc)
		{
			UIdoc = uidoc;
			Doc = UIdoc.Document;
			btnOk = new RelayCommand(BtnOk);

			try
			{

				SelectedLayer = AllLayers[0];

				//Get Family WallType
				AllWallTypes = new FilteredElementCollector(Doc)
					.OfClass(typeof(WallType))
					.Cast<WallType>()
					.ToList();

				//SelectedWallTypes = AllWallTypes[0];

				#region Lấy về all parameter có trong project

				IList<Element> allElements
					= new FilteredElementCollector(Doc)
						.WhereElementIsNotElementType()
						.Where(e => e.Category != null)
						.ToList();

				allElements =
					allElements
						.Distinct(new IEqualityComparerCategory())
						.ToList();

				#endregion

				//
				//Get List Level -- Set base level -- Set top level
				AllLevel = new FilteredElementCollector(Doc)
					.OfClass(typeof(Level))
					.Cast<Level>().ToList();
				AllLevel = AllLevel.OrderBy(l => l.Elevation)
					.ToList();

				baseLevel = AllLevel[0];
				topLevel = AllLevel[1];
				//

				WallMainView = new WallView(this);
				if (WallMainView.ShowDialog() == true)
				{
					run();
				}
			}
			catch (Exception e)
			{
				throw;
			}
		}
		public void BtnOk()
		{
			WallMainView.DialogResult = true;
		}

		public void BtnDel()
		{
			listWallTypes.Remove(SelectWallTypeItem);
		}
		public void BtnAdd()
		{
			ItemWallType lwt = new ItemWallType(AllWallTypes[0], AllWallTypes);
			listWallTypes.Add(lwt);
		}

		public void run()
		{
			List<PlanarFace> hatchToCreateWall = AcadUtils.GetHatchHaveName(this.SelectedCadLink,
				this.SelectedLayer);

			List<ElementId> newWall = new List<ElementId>();

			ProgressBarView prog = new ProgressBarView();

			prog.Show();

			List<ElementId> wallElementIds = new List<ElementId>();

			foreach (PlanarFace hatch in hatchToCreateWall)
			{

				EdgeArrayArray edgeLoops = hatch.EdgeLoops;
				foreach (EdgeArray edgeArray in edgeLoops)
				{
					List<Curve> allCurves = new List<Curve>();
					foreach (Edge edge in edgeArray)
					{
						allCurves.Add(edge.AsCurve());
					}

					#region Xử lý Curve

					#region nháp
					List<Curve> ListCurvesRemove = new List<Curve>();
					foreach (Curve cv in allCurves)
					{
						Line line = cv as Line;
						foreach (ItemWallType itemwt in listWallTypes)
						{
							if (Math.Round(itemwt.With.MmToFoot(), 8) == Math.Round(line.Length, 8))
							{
								ListCurvesRemove.Add(cv);
								break;
							}
						}
					}

					//Remove các curve theo bề rộng tường vd tuong 
					foreach (Curve cv in ListCurvesRemove)
					{
						allCurves.Remove(cv);
					}
					#endregion

					// Nếu còn nhiều hơn 2 line thì mới vẽ
					if (allCurves.Count > 1)
					{
						do
						{
							for (int i = 0; i < allCurves.Count - 1; i++)
							{
								List<Curve> ListCurvesDraw = new List<Curve>();
								Curve cv = allCurves[i];
								ListCurvesDraw.Add(cv);
								for (int j = i + 1; j < allCurves.Count; j++) //Lấy tất cả các curve song song và có kc = rộng tường
								{
									if (cv.Direction().IsParallel(allCurves[j].Direction())// || (IsParallelSs(cv.Direction(), allCurves[j].Direction())) 
										&& CheckDis(cv, allCurves[j]) == true) // Song song và có kc phù hợp
									{
										ListCurvesDraw.Add(allCurves[j]);
										double distance = Dis2Curve(cv, allCurves[j]);
										ListCurvesDraw = CurvesDraw(cv, allCurves[j], allCurves, distance);
										break;
									}
								}

								if (ListCurvesDraw.Count > 1)
								{
									//Dùng các curve trong list để vẽ
									ElementWall elementWall = new ElementWall(this);
									ElementId elementId = elementWall.instanceId(ListCurvesDraw);
									this.UIdoc.RefreshActiveView();
									wallElementIds.Add(elementId);
								}

								//Remove các curve thỏa điều kiện
								foreach (Curve v in ListCurvesDraw)
								{
									allCurves.Remove(v);
								}

								//Vẽ đoạn tường đó

								break;
							}

						} while (allCurves.Count > 1);
						if (!prog.Create(hatchToCreateWall.Count, "")) break;
					}
					#endregion

				}
			}

			MessageBox.Show(string.Concat("Success: ", wallElementIds.Count, " elements!"),
				"Success", MessageBoxButtons.OK, MessageBoxIcon.Information);


			this.UIdoc.Selection.SetElementIds(wallElementIds);
			prog.Close();
		}

		public bool CheckDis(Curve cv, Curve cv2)
		{
			bool TF = false;

			Line line1 = cv as Line;

			Line line2 = cv2 as Line;

			foreach (ItemWallType itemwt in listWallTypes)
			{
				if (Math.Round((itemwt.With + 2).MmToFoot(), 8) > Math.Round(CurveUtils.Distance2Lines(line1, line2), 8) &&
					Math.Round((itemwt.With - 2).MmToFoot(), 8) < Math.Round(CurveUtils.Distance2Lines(line1, line2), 8))   // 2 line song song và có kc = 1 bề rộng tường đã nhập vào thì xem xét
				{
					XYZ a1 = XYZUtils.Midpoint(line1);
					XYZ a2 = XYZUtils.Midpoint(line2);

					double dis = Math.Sqrt((a1.X - a2.X) * (a1.X - a2.X) + (a1.Y - a2.Y) * (a1.Y - a2.Y));

					if (dis < line1.Length / 2 || dis < line2.Length / 2)   // khoảng cách tâm 2 line nhỏ hơn 1 nửa chiều dài line dài
					{
						TF = true;
						return TF;
					}
				}
			}

			return TF;
		}

		public double Dis2Curve(Curve cv, Curve cv2)
		{
			Line line1 = cv as Line;

			Line line2 = cv2 as Line;

			double distance = CurveUtils.Distance2Lines(line1, line2);
			return distance;
		}

		/// <summary>
		/// Kiểm tra 2 vecto có gần song song hay không đúng trả về true sai trả về false
		/// </summary>
		public bool IsParallelSs(XYZ p, XYZ q)
		{
			bool s = p.CrossProduct(q).IsZeroLength();
			if (s == true)
			{
				return true;
			}
			else
			{
				double angle = p.AngleTo(q);
				if (angle < 0.0872665 || Math.PI - angle < 0.0872665)
				{
					return true;
				}
				else
				{
					return false;
				}
			}
		}

		public List<Curve> CurvesDraw(Curve cv1, Curve cv2, List<Curve> allCurves, double distance)
		{
			Curve cv;
			if (cv1.Length >= cv2.Length)
			{
				cv = cv1;
			}
			else
			{
				cv = cv2;
			}

			XYZ mpCv = XYZUtils.Midpoint(cv);

			List<Curve> ListCurvesDraw = new List<Curve>();
			ListCurvesDraw.Add(cv);

			for (int j = 0; j < allCurves.Count; j++) //Lấy tất cả các curve song song và có kc = rộng tường
			{
				if ((cv.Direction().IsParallel(allCurves[j].Direction())) //|| IsParallelSs(cv.Direction(),allCurves[j].Direction()))
					&& CheckDis(cv, allCurves[j])) //&&
												   //(Math.Round(distance, 8) == Math.Round(Dis2Curve(cv, allCurves[j]), 8)))
												   //(Math.Round(distance+2.MmToFoot(), 8) > Math.Round(Dis2Curve(cv, allCurves[j]), 8) &&
												   // (Math.Round(distance - 2.MmToFoot(), 8) < Math.Round(Dis2Curve(cv, allCurves[j]), 8)))) // Song song và có kc phù hợp
				{
					XYZ mpoint = XYZUtils.Midpoint(allCurves[j]);

					double dis = Math.Sqrt((mpCv.X - mpoint.X) * (mpCv.X - mpoint.X) + (mpCv.Y - mpoint.Y) * (mpCv.Y - mpoint.Y));

					if (dis < cv.Length / 2)
					{
						ListCurvesDraw.Add(allCurves[j]);
					}
				}
			}

			return ListCurvesDraw;
		}
	}
}
