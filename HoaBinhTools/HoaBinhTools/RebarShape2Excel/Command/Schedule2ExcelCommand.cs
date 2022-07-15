using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using HoaBinhTools.QLUser.Models;
using SingleData;
using Utility;

namespace Schedule2Excel.Command
{
	[Transaction(TransactionMode.Manual)]
	public class Schedule2ExcelCommand : IExternalCommand
	{
		public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
		{
			//String assemblyFilePath = System.Reflection.Assembly.GetExecutingAssembly().Location;
			//String assemplyDirPath = System.IO.Path.GetDirectoryName(assemblyFilePath);
			//MainForm validate = new MainForm { RegKeyPath = assemplyDirPath };
			//if (validate.checkFromRegFile() != MainForm.StatusKey.Matched)
			//{
			//    MainForm regForm = new MainForm();
			//    regForm.ShowDialog();

			//    return Result.Succeeded;
			//}

			#region Initial
			Singleton.Instance = new Singleton();
			var revitData = RevitData.Instance;
			var revitModelData = RevitModelData.Instance;
			var templateData = TemplateData.Instance;
			var securityData = SecurityData.Instance;

			revitData.UIApplication = commandData.Application;

			var app = revitData.Application;
			FailureUtil.SetFailuresPreprocessorInTransaction();
			var tx = RevitData.Instance.Transaction;
			tx.Start();
			#endregion

			revitModelData.Form.ShowDialog();

			#region Ghi nhận tần suất
			string nameaddin = "RebarShapeSchedule2Excel";
			Registration r = new Registration();
			r.GhiNhanTanSuat(nameaddin);
			#endregion

			tx.Commit();
			return Result.Succeeded;
		}

	}
}
