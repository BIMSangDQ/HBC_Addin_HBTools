using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using HoaBinhTools.AutocadToRevit.Column.Models;
using HoaBinhTools.AutocadToRevit.Column.Views;
using HoaBinhTools.AutocadToRevit.Utils;
using Utils;

namespace HoaBinhTools.AutocadToRevit.Column.ViewModels
{
	public class ColumnViewModel : ViewModelBase
	{
		public RelayCommand btnOk { get; set; }
		ColumnView CloumnMainView { get; set; }

		public ColumnViewModel(ImportInstance rr)
		{
			btnOk = new RelayCommand(BtnOk);

			SelectedCadLink = rr;
		}

		public UIDocument UIdoc;
		public Document Doc;
		public ImportInstance SelectedCadLink = null;

		private double _percent;

		public double Percent
		{
			get => _percent;
			set
			{
				_percent = value;
				OnPropertyChanged();
			}
		}

		public void Execute(UIDocument uidoc)
		{
			UIdoc = uidoc;
			Doc = UIdoc.Document;
			btnOk = new RelayCommand(BtnOk);
			try
			{

				SelectedLayer = AllLayers[0];

				//Get Family column
				AllFamiliesColumn = new FilteredElementCollector(Doc)
					.OfClass(typeof(Family))
					.Cast<Family>()
					.Where(f => f.FamilyCategory.Name.Equals("Structural Columns")
								|| f.FamilyCategory.Name.Equals("Columns")
					)
					.ToList();

				SelectedFamilyColumn = AllFamiliesColumn[0];

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

				AllParameters = new List<string>();
				foreach (Element e in allElements)
				{
					AllParameters
						.AddRange(ParameterUtils.GetAllParameters(e));
				}

				AllParameters
					= AllParameters.Distinct().ToList();
				AllParameters.Sort();

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

				CloumnMainView = new ColumnView(this);
				if (CloumnMainView.ShowDialog() == true)
				{
					run();
				}
			}
			catch (Exception e)
			{
				throw;
			}
		}

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

		public string SelectedLayer
		{
			get { return selectedLayer; }
			set { this.selectedLayer = value; }
		}

		public List<Family> AllFamiliesColumn { get; set; }

		private Family selectedFamilyColumn = null;

		public Family SelectedFamilyColumn
		{
			get { return selectedFamilyColumn; }
			set { this.selectedFamilyColumn = value; }
		}

		public List<string> AllParameters { get; set; }

		private string bParam = "b";

		public string BParam
		{
			get { return bParam; }
			set { this.bParam = value; }
		}

		private string hParam = "h";

		public string HParam
		{
			get { return hParam; }
			set { this.hParam = value; }
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

		private int maximumObj = 100;

		public int MaximumObj
		{
			get { return maximumObj; }
			set { this.maximumObj = value; }
		}

		public double ValueObj { get; set; }

		public void run()
		{
			#region Lấy về maximum những element cần chạy

			List<PlanarFace> hatchToCreateColumn = AcadUtils.GetHatchHaveName(this.SelectedCadLink,
				this.SelectedLayer);

			List<ColumnData> allColumnsData
				= new List<ColumnData>();

			foreach (PlanarFace hatch in hatchToCreateColumn)
			{
				List<Curve> allCurves = new List<Curve>();
				EdgeArrayArray edgeLoops = hatch.EdgeLoops;
				foreach (EdgeArray edgeArray in edgeLoops)
				{
					foreach (Edge edge in edgeArray)
					{
						allCurves.Add(edge.AsCurve());
					}
				}

				if (allCurves.Count == 4)
				{
					ColumnData columnData = new ColumnData(hatch);
					allColumnsData.Add(columnData);
				}
			}

			maximumObj = allColumnsData.Count;

			#endregion

			#region Code

			ProgressBarView prog = new ProgressBarView();

			prog.Show();

			List<ElementId> newColumn = new List<ElementId>();

			int count = 0;
			foreach (ColumnData columnData in allColumnsData)
			{
				#region Code

				FamilySymbol familySymbol
					= Utils.FamilyUtils.GetFamilySymbolColumn(this.SelectedFamilyColumn,
						columnData.CanhNgan,
						columnData.CanhDai,
						bParam, hParam);

				if (familySymbol == null) continue;
				{
					try
					{
						ElementColumn EI = new ElementColumn(this);
						newColumn.Add(EI.instanceId(columnData, familySymbol));
						if (!prog.Create(allColumnsData.Count, "")) break;
					}
					catch (Exception e)
					{
						throw;
					}
				}

				#endregion
			}

			prog.Close();

			//CloumnMainView.Close();

			MessageBox.Show(string.Concat("Success: ", newColumn.Count, " elements!"),
				"Success", MessageBoxButtons.OK, MessageBoxIcon.Information);

			this.UIdoc.RefreshActiveView();
			this.UIdoc.Selection.SetElementIds(newColumn);
			#endregion
		}

		public void BtnOk()
		{
			CloumnMainView.DialogResult = true;
			//CloumnMainView.Close();
		}
	}
}