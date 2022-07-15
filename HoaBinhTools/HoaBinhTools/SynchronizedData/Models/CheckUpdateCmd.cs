using System;
using Autodesk.Revit.ApplicationServices;
using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using HoaBinhTools.SynchronizedData.Db;
using HoaBinhTools.SynchronizedData.ViewModels;
using Utils;

namespace HoaBinhTools.SynchronizedData.Models
{
	[Transaction(TransactionMode.Manual)]
	class CheckUpdateCmd : IExternalCommand
	{
		public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
		{
			UIApplication uiapp = commandData.Application;

			UIDocument uidoc = uiapp.ActiveUIDocument;

			Application app = uiapp.Application;

			ActiveData.GetInformation(uidoc);

			ExternalEventHandler.Instance.Create();

			//CheckUpdate
			DbUtils db = new DbUtils();
			DbConnect dbConnect = new DbConnect();
			dbConnect.Get_DB_Connection();

			int i = db.countUpdate(db.GetIdCurrentFile());
			PutUpdate(uiapp, i);

			dbConnect.Close_DB_Connection();
			//
			return Result.Succeeded;
		}

		protected void PutUpdate(UIApplication application, int i)
		{
			RibbonPanel myPanel = application.GetRibbonPanels("BiMHoaBinh 2.5.7.0").Find(x => x.Name == "Sync Data");

			TextBox textBox;

			foreach (RibbonItem item in myPanel.GetItems())
			{
				if ("Count" == item.Name)
				{
					textBox = item as TextBox;
					string s = i > 1 ? i.ToString() + " Updates" : i.ToString() + " Update";
					textBox.Value = s;
					break;
				}
			}
		}
	}
}
