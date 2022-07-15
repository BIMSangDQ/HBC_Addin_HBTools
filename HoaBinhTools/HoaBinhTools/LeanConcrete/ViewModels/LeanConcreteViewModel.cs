using System;
using System.Collections.Generic;
using System.Linq;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI.Selection;
using HoaBinhTools.LeanConcrete.Models;
using HoaBinhTools.LeanConcrete.Views;
using Utils;

namespace HoangBinhTools.LeanConcrete.ViewModels
{
	public class LeanConcreteViewModel : ViewModelBase
	{
		public RelayCommand CancelButton { get; set; }

		public RelayCommand OkButton { get; set; }

		LeanConcreteView Fmain { get; set; }

		public void Execute()
		{
			Fmain = new LeanConcreteView(this);

			if (Fmain.ShowDialog() == true)
			{
				Run();
			}
		}

		public LeanConcreteViewModel()
		{
			CancelButton = new RelayCommand(Cancelbutton);

			OkButton = new RelayCommand(Okbutton);
		}


		public void Run()
		{

			List<ElementId> selected = ActiveData.Selection.GetElementIds().Where(e =>

				ActiveData.Document.GetElement(e).Category.Id.IntegerValue == (int)BuiltInCategory.OST_StructuralFoundation ||
				ActiveData.Document.GetElement(e).Category.Id.IntegerValue == (int)BuiltInCategory.OST_Floors ||
				ActiveData.Document.GetElement(e).Category.Id.IntegerValue == (int)BuiltInCategory.OST_StructuralFraming
									  ).ToList();

			if (selected.Count() < 1)
			{
				selected = ActiveData.Selection.PickObjects

				(ObjectType.Element, new FilterCategoryUtils


				{
					FuncElement = x =>
					x.Category.Id.IntegerValue == (int)BuiltInCategory.OST_StructuralFoundation ||
					x.Category.Id.IntegerValue == (int)BuiltInCategory.OST_Floors ||
					x.Category.Id.IntegerValue == (int)BuiltInCategory.OST_StructuralFraming
				},
					  "Select some foundations to model lining concrete"
				)

				.Select(e => ActiveData.Document.GetElement(e).Id).ToList();
			}

			var TotalElement = selected.IdsToElements();



			ProgressBarView prog = new ProgressBarView();

			prog.Show();
			foreach (Element Ele in TotalElement)
			{

				try
				{

					// nếu là móng
					if (Ele.Category.Id.IntegerValue == (int)BuiltInCategory.OST_StructuralFoundation && Foundation)
					{
						// nếu là slab Foubdation

						if (Ele is Floor)
						{
							ElementFoundation EFo = new ElementFoundation(Ele, this);

							EFo.Execute();

							if (!prog.Create(TotalElement.Count, "")) break;
						}

						// nêu là isolate Foundation
						if (Ele is FamilyInstance)
						{
							ElementIsolatedFoundation EI = new ElementIsolatedFoundation(Ele, this);

							EI.Execute();
						}
					}

					if (Ele.Category.Id.IntegerValue == (int)BuiltInCategory.OST_Floors && Floor)
					{
						ElementFloor EFL = new ElementFloor(Ele, this);

						EFL.Execute();

						if (!prog.Create(TotalElement.Count, "")) break;

					}

					if (Ele.Category.Id.IntegerValue == (int)BuiltInCategory.OST_StructuralFraming && Beam)
					{
						ElementGroundBeam EG = new ElementGroundBeam(Ele, this);

						EG.Execute();

						if (!prog.Create(TotalElement.Count, "")) break;

					}


				}
				catch (Exception ex)
				{

				}

			}
			prog.Close();
		}





		FloorType fltype;
		public FloorType Fltype
		{
			get
			{
				return fltype;
			}
			set
			{
				if (Fltype != value)
				{
					fltype = value;
					OnPropertyChanged(nameof(fltype));
				}
			}

		}




		public List<FloorType> foundationType;

		public List<FloorType> FoundationType
		{
			get
			{
				return foundationType = new FilteredElementCollector(ActiveData.Document).OfClass(typeof(FloorType)).OfCategory(BuiltInCategory.OST_StructuralFoundation).Cast<FloorType>().ToList();
			}
			set
			{
				this.foundationType = value;
			}
		}


		private double offset = 200;
		public double Offset
		{
			get
			{
				return offset;
			}
			set
			{
				offset = value;
			}
		}

		private bool beam = true;
		public bool Beam
		{
			get
			{
				return beam;
			}
			set
			{
				beam = value;
			}
		}

		private bool floor = true;
		public bool Floor
		{
			get
			{
				return floor;
			}
			set
			{
				floor = value;
			}
		}


		private bool foundation = true;
		public bool Foundation
		{
			get
			{
				return foundation;
			}
			set
			{
				foundation = value;
				OnPropertyChanged();

			}
		}



		public void Okbutton()
		{
			Fmain.DialogResult = true;
			Fmain.Close();
		}


		public void Cancelbutton()
		{
			Fmain.Close();
		}

	}
}
