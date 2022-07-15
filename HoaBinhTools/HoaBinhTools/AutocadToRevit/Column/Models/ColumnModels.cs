using Autodesk.Revit.DB;
using Autodesk.Revit.DB.Structure;
using HoaBinhTools.AutocadToRevit.Utils;
using Utils;

namespace HoaBinhTools.AutocadToRevit.Column.Models
{
	class ColumnModels
	{
		public static FamilyInstance Create(ColumnData columnData, FamilySymbol familySymbol,
			Level BaseLevel, Level TopLevel,
			double BaseOffset, double TopOffset, Document doc)
		{
			using (Transaction tx = new Transaction(doc, "CreateColumn"))
			{
				tx.Start();

				DeleteWarningSuper warningSuper = new DeleteWarningSuper();
				FailureHandlingOptions failOpt = tx.GetFailureHandlingOptions();
				failOpt.SetFailuresPreprocessor(warningSuper);
				tx.SetFailureHandlingOptions(failOpt);

				FamilyInstance instance = doc.Create
					.NewFamilyInstance(columnData.TamCot,
						familySymbol, BaseLevel, StructuralType.Column);

				instance.get_Parameter(BuiltInParameter.FAMILY_BASE_LEVEL_PARAM)
					.SetValue(BaseLevel.Id);
				instance.get_Parameter(BuiltInParameter.FAMILY_TOP_LEVEL_PARAM)
					.Set(TopLevel.Id);

				instance.get_Parameter(BuiltInParameter.FAMILY_BASE_LEVEL_OFFSET_PARAM)
					.Set(BaseOffset.MmToFoot());

				instance.get_Parameter(BuiltInParameter.FAMILY_TOP_LEVEL_OFFSET_PARAM)
					.Set(TopOffset.MmToFoot());

				var axis = Line.CreateUnbound(columnData.TamCot, XYZ.BasisZ);
				ElementTransformUtils.RotateElement(doc,
					instance.Id, axis,
					columnData.GocXoay);

				ActiveData.Document.Regenerate();

				//newColumns.Add(instance.Id);
				instance.Pinned = true;

				tx.Commit();
				return instance;
			}
		}
	}
}
