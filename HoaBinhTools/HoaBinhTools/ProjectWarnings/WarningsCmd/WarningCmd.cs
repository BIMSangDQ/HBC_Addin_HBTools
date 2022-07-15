using Autodesk.Revit.ApplicationServices;
using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using HoaBinhTools.ProjectWarnings.Models;
using Utils;

namespace HoaBinhTools.ProjectWarnings.WarningsCmd
{
	[Transaction(TransactionMode.Manual)]
	public class WarningCmd : IExternalCommand
	{
		public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
		{
			UIApplication uiapp = commandData.Application;

			UIDocument uidoc = uiapp.ActiveUIDocument;

			Application app = uiapp.Application;

			ActiveData.GetInformation(uidoc);


			ExternalEventHandler.Instance.Create();

			FailureController FailController = new FailureController();

			FailController.Excute();

			return Result.Succeeded;

		}
	}
}
