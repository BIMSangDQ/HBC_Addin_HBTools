using System.Collections.Generic;
//using Test;
using Autodesk.Revit.DB;
using HoaBinhTools.AutocadToRevit.FoudationSlab.ViewModels;

namespace HoaBinhTools.AutocadToRevit.FoudationSlab.Models
{
	class ElementFoudation
	{
		public FoundationSlabViewModel FVM;

		public ElementFoudation(FoundationSlabViewModel FVM)
		{
			this.FVM = FVM;
		}

		public ElementId instanceId(IList<XYZ> ListXYZ, FloorType familySymbol)
		{
			Floor instance = FoudationModels.Create(ListXYZ, familySymbol, FVM.BaseLevel, FVM.BaseOffset, FVM.Doc, FVM.FloorT, FVM.IsPolyline);

			return instance.Id;
		}
	}
}
