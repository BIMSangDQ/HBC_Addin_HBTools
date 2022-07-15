using System.Threading;
using Autodesk.Revit.ApplicationServices;
using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using Autodesk.Revit.UI.Selection;
using HoaBinhTools.AutocadToRevit.Beam.ViewModels;
using HoaBinhTools.AutocadToRevit.Utils;
using HoaBinhTools.QLUser.Models;
using Utils;

namespace HoaBinhTools.AutocadToRevit.Beam.Models.BeamCmd
{
	[Transaction(TransactionMode.Manual)]
	class BeamCmd : IExternalCommand
	{
		public Result Execute(ExternalCommandData commandData, ref string message, Autodesk.Revit.DB.ElementSet elements)
		{
			UIApplication uiapp = commandData.Application;

			//Lấy Đối Tượng Tài Liệu Hiện Hành
			UIDocument uidoc = uiapp.ActiveUIDocument;

			Application app = uiapp.Application;

			ActiveData.GetInformation(uidoc);

			#region Ghi nhận tần suất 05/07/2021
			string nameaddin = "Autocad2Revit";
			Registration registration = new Registration();

			Thread t = new Thread(() =>
			{
				registration.GhiNhanTanSuat(nameaddin);
			});
			t.Start();
			#endregion

			using (TransactionGroup transGroup = new TransactionGroup(ActiveData.Document))
			{
				transGroup.Start("Transaction Group");

				Reference r = uidoc.Selection.PickObject(ObjectType.Element, new ImportInstanceSelectionFilter(),
					"Chọn Link Cad");
				ImportInstance importInstance = ActiveData.Document.GetElement(r) as ImportInstance;

				BeamViewModel ViewControl = new BeamViewModel(importInstance);

				ViewControl.Execute(uidoc);

				transGroup.Commit();

			}
			return Result.Succeeded;
		}
	}
}
