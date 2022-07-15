using Autodesk.Revit.DB;
using HoaBinhTools.AutocadToRevit.Utils;
using Utils;


namespace HoaBinhTools.AutocadToRevit.Wall.Models
{
	class WallModel
	{
		public static Autodesk.Revit.DB.Wall CreateWall(Document doc, Curve curve, WallType wallType, ElementId levelId, ElementId topLevelId, double Baseoffset, double Topoffset)
		{
			using (Transaction tx = new Transaction(doc, "CreateColumn"))
			{
				tx.Start();

				DeleteWarningSuper warningSuper = new DeleteWarningSuper();
				FailureHandlingOptions failOpt = tx.GetFailureHandlingOptions();
				failOpt.SetFailuresPreprocessor(warningSuper);
				tx.SetFailureHandlingOptions(failOpt);

				Autodesk.Revit.DB.Wall instance = Autodesk.Revit.DB.Wall.Create(doc, curve, levelId, true);
				instance.WallType = wallType;

				instance.get_Parameter(BuiltInParameter.WALL_BASE_OFFSET)
					.Set(Baseoffset.MmToFoot());

				instance.get_Parameter(BuiltInParameter.WALL_HEIGHT_TYPE)
					.Set(topLevelId);

				instance.get_Parameter(BuiltInParameter.WALL_TOP_OFFSET)
					.Set(Topoffset.MmToFoot());


				tx.Commit();
				return instance;
			}
		}
	}
}
