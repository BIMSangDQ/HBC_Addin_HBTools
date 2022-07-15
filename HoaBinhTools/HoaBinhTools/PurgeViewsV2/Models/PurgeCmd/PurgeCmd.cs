using System.Threading;
using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using HoaBinhTools.PurgeViewsV2.ViewModels;
using HoaBinhTools.QLUser.Models;
using Utils;

namespace HoaBinhTools.PurgeViewsV2.Models.PurgeCmd
{

	[Transaction(TransactionMode.Manual)]
	public class PurgeCmd : IExternalCommand
	{
		public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
		{
			// Show Form
			UIApplication uiapp = commandData.Application;

			UIDocument uidoc = uiapp.ActiveUIDocument;

			//String assemblyFilePath = System.Reflection.Assembly.GetExecutingAssembly().Location;
			//String assemplyDirPath = System.IO.Path.GetDirectoryName(assemblyFilePath);
			//MainForm validate = new MainForm();
			//validate.RegKeyPath = assemplyDirPath;
			//if (validate.checkFromRegFile() != MainForm.StatusKey.Matched)
			//{
			//    MainForm regForm = new MainForm();
			//    regForm.ShowDialog();
			//    return Result.Succeeded;
			//}


			ActiveData.GetInformation(uidoc);

			ExternalEventHandler.Instance.Create();

			PugreViewModels vm = new PugreViewModels();

			vm.Execute();

			#region Ghi nhận tần suất 05/07/2021
			string nameaddin = "Purge View";
			Registration r = new Registration();

			Thread t = new Thread(() =>
			{
				r.GhiNhanTanSuat(nameaddin);
			});
			t.Start();
			#endregion

			return Result.Succeeded;
		}



	}
}
