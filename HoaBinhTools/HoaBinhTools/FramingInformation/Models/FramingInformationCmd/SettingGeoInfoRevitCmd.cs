
using Autodesk.Revit.ApplicationServices;
using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using HoaBinhTools.FramingInformation.ViewModels;
using Utils;

namespace HoaBinhTools.FramingInformation
{
	[Transaction(TransactionMode.Manual)]
	public class SettingGeoInfoRevitCmd : IExternalCommand
	{
		public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
		{

			UIApplication uiapp = commandData.Application;

			UIDocument uidoc = uiapp.ActiveUIDocument;

			Application app = uiapp.Application;

			ActiveData.GetInformation(uidoc);

			using (TransactionGroup transGroup = new TransactionGroup(ActiveData.Document))
			{
				transGroup.Start("Transaction Group");

				SettingGeoInfoRevitViewModels Fsv = new SettingGeoInfoRevitViewModels();

				transGroup.Assimilate();

			}

			return Result.Succeeded;
		}
	}
}
