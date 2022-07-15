using Autodesk.Revit.DB;
using HoaBinhTools.AutocadToRevit.Beam.ViewModels;
using HoaBinhTools.AutocadToRevit.Utils;

namespace HoaBinhTools.AutocadToRevit.Beam.Models
{
	class ElementBeam
	{

		public BeamViewModel CVM;

		public ElementBeam(BeamViewModel CVM)
		{
			this.CVM = CVM;
		}

		public ElementId instanceId(BeamData beamData, FamilySymbol familySymbol)
		{
			FamilyInstance instance = BeamModels.Create(beamData, familySymbol, CVM.BaseLevel,
				 CVM.BaseOffset, CVM.Doc, CVM.ParameterName, CVM.IsSetName);

			return instance.Id;
		}
	}
}
