using Autodesk.Revit.ApplicationServices;
using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using HoaBinhTools.BIMQAQC.QAQC_Quantity.ViewModels;
using Utils;

namespace HoaBinhTools.BIMQAQC.QAQC_Quantity.Command
{
	[Transaction(TransactionMode.Manual)]
	public class QAQC_QuantityCommand : IExternalCommand
	{

		public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
		{
			UIApplication uiapp = commandData.Application;

			//Lấy Đối Tượng Tài Liệu Hiện Hành
			UIDocument uidoc = uiapp.ActiveUIDocument;

			Application app = uiapp.Application;

			ActiveData.GetInformation(uidoc);

			ExternalEventHandler.Instance.Create();

			using (TransactionGroup transGroup = new TransactionGroup(ActiveData.Document))
			{
				transGroup.Start("Transaction Group");

				QuantityStrViewModel ViewControl = new QuantityStrViewModel();

				transGroup.Commit();
			}
			return Result.Succeeded;
		}
	}
}
