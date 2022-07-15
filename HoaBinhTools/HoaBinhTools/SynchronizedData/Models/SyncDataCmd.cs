using System;
using Autodesk.Revit.ApplicationServices;
using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using HoaBinhTools.SynchronizedData.ViewModels;
using Utils;

namespace HoaBinhTools.SynchronizedData.Models
{
	[Transaction(TransactionMode.Manual)]
	class SyncDataCmd : IExternalCommand
	{
		public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
		{
			UIApplication uiapp = commandData.Application;

			UIDocument uidoc = uiapp.ActiveUIDocument;

			Application app = uiapp.Application;

			ActiveData.GetInformation(uidoc);

			ExternalEventHandler.Instance.Create();

			PutTime(uiapp);
			//using (Transaction trans = new Transaction(ActiveData.Document))
			//{
			//	trans.Start("Transaction Group");
			SynchronizedDataViewModels ViewControl = new SynchronizedDataViewModels();
			ViewControl.Execute();
			//	trans.Commit();
			//}

			return Result.Succeeded;
		}

		protected void PutTime(UIApplication application)
		{
			RibbonPanel myPanel = application.GetRibbonPanels("BiMHoaBinh 2.5.7.0").Find(x => x.Name == "Sync Data");

			TextBox textBox;

			foreach (RibbonItem item in myPanel.GetItems())
			{
				if ("TimeCheck" == item.Name)
				{
					textBox = item as TextBox;
					textBox.Value = DateTime.Now;
					break;
				}
			}
		}
	}
}
