using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Forms;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using HoaBinhTools.AutocadToRevit.FoudationSlab.Models;
using HoaBinhTools.AutocadToRevit.FoudationSlab.Views;
using HoaBinhTools.AutocadToRevit.Utils;
using Utility;
using Utils;

namespace HoaBinhTools.AutocadToRevit.FoudationSlab.ViewModels
{
	public class FoundationSlabViewModel : ViewModelBase
	{
		public RelayCommand btnOk { get; set; }
		FoundationSlabView FoundationSlabView { get; set; }

		public FoundationSlabViewModel(ImportInstance rr)
		{
			btnOk = new RelayCommand(BtnOk);

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

		public string SelectedLayer
		{
			get { return selectedLayer; }
			set { this.selectedLayer = value; }
		}

		public ObservableCollection<FloorType> AllFamiliesFoundation { get; set; }

		private FloorType selectedFamilyFoundation = null;

		public FloorType SelectedFamilyFoundation
		{
			get { return selectedFamilyFoundation; }
			set
			{
				if (selectedFamilyFoundation != value)
				{
					selectedFamilyFoundation = value;
					//OnPropertyChanged(nameof(selectedFamilyFoundation));
				}

				//this.selectedFamilyFoundation = value;
			}
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

		public List<string> FloorOrFoundation { get; set; }

		private string floorT;

		public string FloorT
		{
			get { return floorT; }
			set
			{
				this.floorT = value;
				ObservableCollection<FloorType> floorType = new ObservableCollection<FloorType>();

				if (floorT != "Floor")
				{
					floorType = new FilteredElementCollector(Doc)
						.OfClass(typeof(FloorType))
						.Cast<FloorType>()
						.Where(type => type.Category.Name.ToString().Equals("Structural Foundations"))
						.ToObservableCollection();
				}
				else
				{
					floorType = new FilteredElementCollector(Doc)
						.OfClass(typeof(FloorType))
						.Cast<FloorType>()
						.ToObservableCollection();
				}

				AllFamiliesFoundation = floorType;
				OnPropertyChanged(nameof(AllFamiliesFoundation));
			}
		}

		private bool isPolyline = true;

		public bool IsPolyline
		{
			get { return isPolyline; }
			set { this.isPolyline = value; }
		}

		public void BtnOk()
		{
			FoundationSlabView.DialogResult = true;
			FoundationSlabView.Close();
		}

		public void Execute(UIDocument uidoc)
		{
			UIdoc = uidoc;
			Doc = UIdoc.Document;
			btnOk = new RelayCommand(BtnOk);
			try
			{
				SelectedLayer = AllLayers[0];

				ObservableCollection<FloorType> floorType = new ObservableCollection<FloorType>();

				if (FloorT != "Slab")
				{
					floorType = new FilteredElementCollector(Doc)
						.OfClass(typeof(FloorType))
						.Cast<FloorType>()
						.Where(type => type.Category.Name.ToString().Equals("Structural Foundations"))
						.ToObservableCollection();
				}
				else
				{
					floorType = new FilteredElementCollector(Doc)
						.OfClass(typeof(FloorType))
						.Cast<FloorType>()
						.ToObservableCollection();
				}

				AllFamiliesFoundation = floorType;

				SelectedFamilyFoundation = AllFamiliesFoundation[0];

				//Get List Level -- Set base level -- Set top level
				AllLevel = new FilteredElementCollector(Doc)
					.OfClass(typeof(Level))
					.Cast<Level>().ToList();
				AllLevel = AllLevel.OrderBy(l => l.Elevation)
					.ToList();

				baseLevel = AllLevel[0];
				//
				FloorOrFoundation = new List<string>();
				FloorOrFoundation.Add("Foundation Slab");
				FloorOrFoundation.Add("Floor");

				FloorT = FloorOrFoundation[0];

				FoundationSlabView = new FoundationSlabView(this);
				if (FoundationSlabView.ShowDialog() == true)
				{
					run();
				}
			}
			catch
			{
			}
		}

		public void run()
		{
			List<PolyLine> polylineToFoundation = AcadUtils.GetPolylineHaveName(this.SelectedCadLink,
				this.SelectedLayer);

			List<PlanarFace> hatchToCreateSlab = AcadUtils.GetHatchHaveName(this.SelectedCadLink,
				this.SelectedLayer);

			List<ElementId> newFoudation = new List<ElementId>();

			ProgressBarView prog = new ProgressBarView();

			prog.Show();

			if (this.IsPolyline == true)
			{
				foreach (PolyLine polyline in polylineToFoundation)
				{
					IList<XYZ> xyz = polyline.GetCoordinates();

					if (Math.Round(xyz[0].X - xyz[xyz.Count - 1].X, 0) == 0 &&
						Math.Round(xyz[0].Y - xyz[xyz.Count - 1].Y, 0) == 0)
					{
						try
						{
							ElementFoudation EI = new ElementFoudation(this);
							newFoudation.Add(EI.instanceId(xyz, SelectedFamilyFoundation));

							if (!prog.Create(polylineToFoundation.Count, "")) break;
						}
						catch (Exception e)
						{
							throw;
						}
					}
				}
			}
			else
			{
				foreach (PlanarFace hatch in hatchToCreateSlab)
				{
					List<Curve> allCurves = new List<Curve>();
					EdgeArrayArray edgeLoops = hatch.EdgeLoops;
					foreach (EdgeArray edgeArray in edgeLoops)
					{
						foreach (Edge edge in edgeArray)
						{
							allCurves.Add(edge.AsCurve());
						}

						try
						{
							IList<XYZ> xyz = new List<XYZ>();
							for (int i = 0; i < allCurves.Count; i++)
							{
								if (i == 0)
								{
									xyz.Add(allCurves[i].GetEndPoint(0));
									xyz.Add(allCurves[i].GetEndPoint(1));
								}
								else
								{
									xyz.Add(allCurves[i].GetEndPoint(1));
								}
							}

							ElementFoudation EI = new ElementFoudation(this);
							newFoudation.Add(EI.instanceId(xyz, SelectedFamilyFoundation));

							if (!prog.Create(polylineToFoundation.Count, "")) break;
						}
						catch (Exception e)
						{
							throw;
						}
					}
				}
			}

			prog.Close();

			FoundationSlabView.Close();

			MessageBox.Show(string.Concat("Success: ", newFoudation.Count, " elements!"),
				"Success", MessageBoxButtons.OK, MessageBoxIcon.Information);

			this.UIdoc.RefreshActiveView();
			this.UIdoc.Selection.SetElementIds(newFoudation);
		}
	}
}