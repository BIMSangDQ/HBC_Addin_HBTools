using SingleData;

namespace Utility
{
	public static class RevitDataUtil
	{
		private static RevitData revitData;
		public static void ClearData()
		{
			GetSingleData();

			revitData.Transaction = null;
			revitData.InstanceElements = null;
			revitData.TypeElements = null;
			revitData.Families = null;
			revitData.FamilySymbols = null;
			revitData.FamilyInstances = null;
			revitData.Views = null;
			revitData.UserWorksets = null;
			revitData.WorksetDefaultVisibilitySettings = null;
			revitData.Rebars = null;
			revitData.RebarBarTypes = null;
			revitData.RebarShapes = null;
			revitData.Categories = null;
			revitData.TextNoteTypes = null;
			revitData.Levels = null;
			revitData.WallTypes = null;
			revitData.ParameterBindings = null;
			revitData.FillPatternElements = null;
			revitData.RebarCoverTypes = null;
			revitData.ViewSchedules = null;
			revitData.ReinforcementSettings = null;
			revitData.RebarRoundingManager = null;
			revitData.RevitLinkInstances = null;
			revitData.LinkDocuments = null;
			revitData.AllInstances = null;
			revitData.ProjectInfo = null;
			revitData.BindingMap = null;
		}
		private static void GetSingleData()
		{
			if (Singleton.Instance != null)
			{
				revitData = RevitData.Instance;
			}
		}
	}
}
