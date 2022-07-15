using Autodesk.Revit.ApplicationServices;
using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using HoaBinhTools.QLUser.ViewModels;
using Utils;

namespace HoaBinhTools.QLUser.Models
{
	[Transaction(TransactionMode.Manual)]
	class QLUserCmd : IExternalCommand
	{
		public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
		{
			UIApplication uiapp = commandData.Application;

			//Lấy Đối Tượng Tài Liệu Hiện Hành
			UIDocument uidoc = uiapp.ActiveUIDocument;

			Application app = uiapp.Application;

			ActiveData.GetInformation(uidoc);

			using (TransactionGroup transGroup = new TransactionGroup(ActiveData.Document))
			{
				transGroup.Start("Transaction Group");

				QLUserViewModel ViewControl = new QLUserViewModel();

				ViewControl.Execute();

				transGroup.Assimilate();
			}
			return Result.Succeeded;
		}
	}
}
