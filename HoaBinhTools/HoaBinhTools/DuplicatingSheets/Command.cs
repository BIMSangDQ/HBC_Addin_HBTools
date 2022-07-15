using System;
using System.IO;
using System.Reflection;
using System.Threading;
using Autodesk.Revit.ApplicationServices;
using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using Autodesk.Revit.UI.Selection;
using HoaBinhTools.QLUser.Models;

namespace DuplicatingSheets
{
	[Transaction(TransactionMode.Manual)]
	public class Command : IExternalCommand
	{
		public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
		{
			string location = Assembly.GetExecutingAssembly().Location;
			string directoryName = Path.GetDirectoryName(location);
			bool flag = false;
			Result result = Result.Cancelled;

			UIApplication application = commandData.Application;
			UIDocument activeUIDocument = application.ActiveUIDocument;
			Application application2 = application.Application;
			Document document = activeUIDocument.Document;
			Selection selection = activeUIDocument.Selection;
			bool flag2 = selection.GetElementIds().Count == 0;
			if (flag2)
			{
				TaskDialog.Show("BIM", "Please Select Sheet");
				result = Result.Cancelled;
			}
			else
			{
				try
				{
					#region Ghi nhận tần suất 05/07/2021

					string nameaddin = "DuplicatingSheets";
					Registration registration = new Registration();

					Thread t = new Thread(() => { registration.GhiNhanTanSuat(nameaddin); });
					t.Start();

					#endregion

					new DuplicatingSheets.DuplicateSheetsForm
					{
						Doc = document,
						App = application2,
						Sel = selection
					}.ShowDialog();
				}
				catch (Exception ex)
				{
					return Result.Cancelled;
				}

				result = 0;
			}

			return result;
		}
	}
}