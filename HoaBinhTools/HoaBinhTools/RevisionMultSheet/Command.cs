#region Namespaces
using System;
using System.Threading;
using System.Windows;
using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using HoaBinhTools.QLUser.Models;
//using Software_Key_Manager_Beta;
#endregion

namespace RevisionMultiSheet
{
	[Transaction(TransactionMode.Manual)]
	public class RevisionCommand : IExternalCommand
	{
		public Result Execute(
		  ExternalCommandData commandData,
		  ref string message,
		  ElementSet elements)
		{
			String assemblyFilePath = System.Reflection.Assembly.GetExecutingAssembly().Location;
			String assemplyDirPath = System.IO.Path.GetDirectoryName(assemblyFilePath);
			//MainForm validate = new MainForm();
			//validate.RegKeyPath = assemplyDirPath;
			//if (validate.checkFromRegFile() != MainForm.StatusKey.Matched)
			//{
			//    MainForm regForm = new MainForm();
			//    regForm.ShowDialog();
			//    return Result.Succeeded;
			//}
			UIApplication uiapp = commandData.Application;
			UIDocument uidoc = uiapp.ActiveUIDocument;
			Autodesk.Revit.ApplicationServices.Application app = uiapp.Application;
			Document doc = uidoc.Document;

			// Access current selection
			#region Ghi nhận tần suất 05/07/2021
			string nameaddin = "Revision MultiSheets";
			Registration r = new Registration();

			Thread t = new Thread(() =>
			{
				r.GhiNhanTanSuat(nameaddin);
			});
			t.Start();
			#endregion

			//Selection sel = uidoc.Selection;            
			Window window = new Window();
			window.WindowStartupLocation = WindowStartupLocation.CenterScreen;
			window.Height = 785; window.Width = 890;
			window.Title = "Revision To MultiSheet";
			window.ResizeMode = ResizeMode.NoResize;
			FormMaster formMaster = new FormMaster();

			formMaster.App = app;
			formMaster.Doc = doc;
			formMaster.UIDoc = uidoc;
			window.Content = formMaster;
			window.ShowDialog();

			return Result.Succeeded;
		}
	}
}
