using Autodesk.Revit.ApplicationServices;
using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using HoaBinhTools.BIMQAQC.QAQC_FormWork.ViewModels;
using Utils;

namespace HoaBinhTools.BIMQAQC.QAQC_FormWork.Command
{
	[Transaction(TransactionMode.Manual)]
	public class QAQCFormWorkCmd : IExternalCommand
	{
		public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
		{
			//UIApplication uiapp = commandData.Application;

			//UIDocument uidoc = uiapp.ActiveUIDocument;

			//Application app = uiapp.Application;

			//ActiveData.GetInformation(uidoc);

			ExternalEventHandler.Instance.Create();

			FormworkViewModels LaunchViewModels = new FormworkViewModels();

			LaunchViewModels.Excute();

			return Result.Succeeded;
		}
	}
}
