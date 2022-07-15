using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Threading;
using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.DB.Structure;
using Autodesk.Revit.UI;
using Autodesk.Revit.UI.Selection;
using Constant;
using HoaBinhTools.QLUser.Models;
using SingleData;
using Utility;

namespace Schedule2Excel.Command
{
	[Transaction(TransactionMode.Manual)]
	public class Command : IExternalCommand
	{
		public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
		{
			Singleton.Instance = new Singleton();
			var revitData = RevitData.Instance;
			var revitModelData = RevitModelData.Instance;

			#region Ghi nhận tần suất 05/07/2021
			string nameaddin = "New Rebar Detailing";
			Registration r = new Registration();

			Thread t = new Thread(() =>
			{
				r.GhiNhanTanSuat(nameaddin);
			});
			t.Start();
			#endregion

			revitData.UIApplication = commandData.Application;
			FailureUtil.SetFailuresPreprocessorInTransaction();
			revitData.Transaction.Start();

			var doc = revitData.Document;
			var sel = revitData.Selection;
			Rebar rebar = doc.GetElement(sel.PickObject(ObjectType.Element,
				new FuncSelectionFilter { FuncElement = x => x is Rebar })) as Rebar;

			Model.Entity.RebarShapeInfo rebarShapeInfo = null;
			try
			{
				//rebarShapeInfo = ModelUtil.GetCurve2DsFromRebar(rebar);

				rebarShapeInfo = rebar.GetCurve2DsFromRebar();
			}
			catch
			{
				throw;
				//revitData.Transaction.Commit(); return Result.Succeeded;
			}

			List<string> fieldNames = new List<string> { "A", "B", "C", "D", "E", "F", "G", "H", "J" };
			List<int> fieldValues = new List<int> { 100, 200, 300, 400, 500, 600, 700, 800 };

			try
			{
				rebarShapeInfo.Initial();
				rebarShapeInfo.Edit();
				rebarShapeInfo.SetValuesBaseOnFields(fieldNames, fieldValues);
				rebarShapeInfo.CreateBitmap();
				rebarShapeInfo.Bitmap.Save(ConstantValue.RebarShapePath);
			}
			catch (Model.Exception.CaseNotCheckException)
			{
				revitData.Transaction.Commit();
				return Result.Succeeded;
			}

			var desktopPath = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
			string textPath = Path.Combine(desktopPath, "output.txt");
			/*if (!File.Exists(textPath)) */
			File.Create(textPath).Dispose();
			using (var tw = new StreamWriter(textPath, true))
			{
				tw.WriteLine(ConstantValue.Output.ToString());
			}

			Process.Start(textPath);
			Process.Start(ConstantValue.RebarShapePath);

			revitData.Transaction.Commit();
			return Result.Succeeded;
		}
	}
}
