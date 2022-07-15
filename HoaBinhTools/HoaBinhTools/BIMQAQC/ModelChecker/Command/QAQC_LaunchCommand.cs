using Autodesk.Revit.ApplicationServices;
using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using HoaBinhTools.BIMQAQC.ModelChecker.ViewModels;
using Utils;

namespace HoaBinhTools.BIMQAQC.ModelChecker.Command
{
	[Transaction(TransactionMode.Manual)]
	public class QAQC_LaunchCommand : IExternalCommand
	{
		public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
		{
			UIApplication uiapp = commandData.Application;

			UIDocument uidoc = uiapp.ActiveUIDocument;

			Application app = uiapp.Application;

			ActiveData.GetInformation(uidoc);

			ExternalEventHandler.Instance.Create();

			LaunchViewModels LaunchViewModels = new LaunchViewModels(uiapp);

			LaunchViewModels.Excute();

			return Result.Succeeded;
		}
	}
}
