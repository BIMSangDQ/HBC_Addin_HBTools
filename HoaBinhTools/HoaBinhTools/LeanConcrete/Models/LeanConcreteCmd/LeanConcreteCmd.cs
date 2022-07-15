using System.Threading;
using Autodesk.Revit.ApplicationServices;
using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using HoaBinhTools.QLUser.Models;
using HoangBinhTools.LeanConcrete.ViewModels;
using Utils;

namespace HoaBinhTools.LeanConcrete.Models
{
	[Transaction(TransactionMode.Manual)]
	public class LeanConcreteCmd : IExternalCommand
	{
		public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
		{
			UIApplication uiapp = commandData.Application;

			//Lấy Đối Tượng Tài Liệu Hiện Hành
			UIDocument uidoc = uiapp.ActiveUIDocument;

			Application app = uiapp.Application;

			ActiveData.GetInformation(uidoc);

			#region Ghi nhận tần suất 05/07/2021
			string nameaddin = "Lean Concrete";
			Registration r = new Registration();

			Thread t = new Thread(() =>
			{
				r.GhiNhanTanSuat(nameaddin);
			});
			t.Start();
			#endregion

			using (TransactionGroup transGroup = new TransactionGroup(ActiveData.Document))
			{
				transGroup.Start("Transaction Group");

				LeanConcreteViewModel ViewControl = new LeanConcreteViewModel();

				ViewControl.Execute();

				transGroup.Assimilate();

			}
			return Result.Succeeded;

		}


	}
}
