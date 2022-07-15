//using Test;
using Autodesk.Revit.DB;
using HoaBinhTools.AutocadToRevit.Column.ViewModels;
using HoaBinhTools.AutocadToRevit.Utils;

namespace HoaBinhTools.AutocadToRevit.Column.Models
{
	class ElementColumn
	{

		public ColumnViewModel CVM;

		public ElementColumn(ColumnViewModel CVM)
		{
			this.CVM = CVM;
		}

		public ElementId instanceId(ColumnData columnData, FamilySymbol familySymbol)
		{
			FamilyInstance instance = ColumnModels.Create(columnData, familySymbol, CVM.BaseLevel,
				CVM.TopLevel, CVM.BaseOffset, CVM.TopOffset, CVM.Doc);

			return instance.Id;
		}

	}
}
