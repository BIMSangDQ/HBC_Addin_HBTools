using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using HoaBinhTools.AutocadToRevit.Beam.Models;
using HoaBinhTools.AutocadToRevit.Beam.Views;
using HoaBinhTools.AutocadToRevit.Utils;
using Utils;

namespace HoaBinhTools.AutocadToRevit.Beam.ViewModels
{
	public class BeamViewModel : ViewModelBase
	{
		public RelayCommand btnOk { get; set; }
		BeamView BeamMainView { get; set; }

		public BeamViewModel(ImportInstance rr)
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
				AllFamiliesBeam = new FilteredElementCollector(Doc)
					.OfClass(typeof(Family))
					.Cast<Family>()
					.Where(f => f.FamilyCategory.Name.Equals("Structural Framing"))
					.ToList();

				SelectedFamilyBeam = AllFamiliesBeam[0];

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
				//
				//SetName
				isSetName = false;
				//
				BeamMainView = new BeamView(this);
				if (BeamMainView.ShowDialog() == true)
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

		public List<Family> AllFamiliesBeam { get; set; }

		private Family selectedFamilyBeam = null;

		public Family SelectedFamilyBeam
		{
			get { return selectedFamilyBeam; }
			set { this.selectedFamilyBeam = value; }
		}

		private bool isSetName;
		public bool IsSetName
		{
			get { return isSetName; }
			set { this.isSetName = value; }
		}

		private string parameterName = null;

		public string ParameterName
		{
			get { return parameterName; }
			set { this.parameterName = value; }
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

		private double baseOffset = 0;

		public double BaseOffset
		{
			get { return baseOffset; }
			set { this.baseOffset = value; }
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

			List<Line> LineToCreateBeam = AcadUtils.GetLineHaveName(this.SelectedCadLink);

			List<BeamData> allBeamsData = new List<BeamData>();

			foreach (Line acadLine in LineToCreateBeam)
			{
				List<Curve> allCurves = new List<Curve>();
				BeamData beamData = new BeamData(this.SelectedCadLink, acadLine);
				allBeamsData.Add(beamData);
			}

			maximumObj = allBeamsData.Count;

			#endregion

			#region Code

			ProgressBarView prog = new ProgressBarView();

			prog.Show();

			List<ElementId> newColumn = new List<ElementId>();

			int count = 0;
			foreach (BeamData beamData in allBeamsData)
			{
				#region Code

				FamilySymbol familySymbol
					= Utils.FamilyUtils.GetFamilySymbolBeam(this.SelectedFamilyBeam,
						MathUtils.MmToFoot(beamData.BeRong),
						MathUtils.MmToFoot(beamData.ChieuCao),
						bParam, hParam);

				if (familySymbol == null) continue;
				{
					try
					{
						ElementBeam EI = new ElementBeam(this);
						newColumn.Add(EI.instanceId(beamData, familySymbol));
						if (!prog.Create(allBeamsData.Count, "")) break;
					}
					catch (Exception e)
					{
						throw;
					}
				}

				#endregion
			}

			prog.Close();

			BeamMainView.Close();

			MessageBox.Show(string.Concat("Success: ", newColumn.Count, " elements!"),
				"Success", MessageBoxButtons.OK, MessageBoxIcon.Information);

			this.UIdoc.RefreshActiveView();
			this.UIdoc.Selection.SetElementIds(newColumn);
			#endregion
		}

		public void BtnOk()
		{
			BeamMainView.DialogResult = true;
		}
	}
}
