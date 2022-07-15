using System.Threading;
using Autodesk.Revit.ApplicationServices;
using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using Autodesk.Revit.UI.Selection;
using HoaBinhTools.AutocadToRevit.FoudationSlab.ViewModels;
using HoaBinhTools.AutocadToRevit.Utils;
using HoaBinhTools.QLUser.Models;
using Utils;

namespace HoaBinhTools.AutocadToRevit.FoudationSlab.Models.FoundationSlabCmd
{
	[Transaction(TransactionMode.Manual)]
	class FoundationSlabCmd : IExternalCommand
	{
		public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
		{
			UIApplication uiapp = commandData.Application;

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

				FoundationSlabViewModel ViewControl = new FoundationSlabViewModel(importInstance);

				ViewControl.Execute(uidoc);

				transGroup.Commit();

			}
			return Result.Succeeded;
		}
	}
}
