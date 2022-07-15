using System.Collections.Generic;
using Autodesk.Revit.DB;
using HoaBinhTools.AutocadToRevit.Utils;
using Utils;

namespace HoaBinhTools.AutocadToRevit.FoudationSlab.Models
{
	class FoudationModels
	{
		public static Floor Create(IList<XYZ> ListXYZ, FloorType familySymbol, Level Level, double offset, Document doc, string type, bool isPolyline)
		{
			using (Transaction tx = new Transaction(doc, "CreateColumn"))
			{
				tx.Start();

				CurveArray cv = new CurveArray();

				for (int i = 1; i < ListXYZ.Count; i++)
				{
					Line line = Line.CreateBound(ListXYZ[i - 1], ListXYZ[i]);
					cv.Append(line);
				}

				XYZ normal = new Autodesk.Revit.DB.XYZ(0, 0, 1);

				DeleteWarningSuper warningSuper = new DeleteWarningSuper();
				FailureHandlingOptions failOpt = tx.GetFailureHandlingOptions();
				failOpt.SetFailuresPreprocessor(warningSuper);
				tx.SetFailureHandlingOptions(failOpt);

				if (type != "Floor")
				{
					Floor instance = doc.Create.NewFoundationSlab(cv, familySymbol, Level, true, normal);
					instance.get_Parameter(BuiltInParameter.FLOOR_HEIGHTABOVELEVEL_PARAM)
						.SetValue(offset.MmToFoot());

					instance.Pinned = true;

					tx.Commit();
					return instance;
				}
				else
				{
					Floor instance = doc.Create.NewFloor(cv, familySymbol, Level, true);
					instance.get_Parameter(BuiltInParameter.FLOOR_HEIGHTABOVELEVEL_PARAM)
						.SetValue(offset.MmToFoot());

					instance.Pinned = true;

					tx.Commit();
					return instance;
				}

			}
		}
	}
}
