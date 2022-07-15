#region Namespaces
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Threading;
using Autodesk.Revit.ApplicationServices;
using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
//using Vii.SecurityLib;
using Autodesk.Revit.DB.Structure;
using Autodesk.Revit.UI;
using Autodesk.Revit.UI.Selection;
using HoaBinhTools.QLUser.Models;
using Schedule2Excel2k16.Forms;
#endregion

namespace Schedule2Excel2k16
{
	[Transaction(TransactionMode.Manual)]
	public class Schedule2ExcelCommand : IExternalCommand
	{
		public static string path { get; set; } = @"C:\RebarShape";
		public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
		{
			UIApplication uiapp = commandData.Application;
			UIDocument uidoc = uiapp.ActiveUIDocument;
			Application app = uiapp.Application;
			Document doc = uidoc.Document;
			Selection sel = uidoc.Selection;

			if (Directory.Exists(path))
			{
				GetImage(doc, path);
			}
			else
			{
				DirectoryInfo folder = Directory.CreateDirectory(path);
				GetImage(doc, path);
			}

			List<HolderScheduleItem> holderList = this.filterScheduleNameList(doc);

			Schedule2ExcelForm fr = new Schedule2ExcelForm();
			fr.Doc = doc;
			fr.App = app;
			fr.ScheduleList = holderList;
			fr.ShowDialog();

			#region Ghi nhận tần suất 05/07/2021
			string nameaddin = "Schedule 2 Excel";
			Registration r = new Registration();

			Thread t = new Thread(() =>
			{
				r.GhiNhanTanSuat(nameaddin);
			});
			t.Start();
			#endregion

			return Result.Succeeded;
		}

		private List<HolderScheduleItem> filterScheduleNameList(Document pDoc)
		{
			FilteredElementCollector collector = new FilteredElementCollector(pDoc);
			collector.WhereElementIsNotElementType();
			collector.WhereElementIsViewIndependent();
			collector.OfClass(typeof(ViewSchedule));

			var sql = from e in collector orderby e.Name ascending select e;

			List<HolderScheduleItem> list = new List<HolderScheduleItem>();

			foreach (Element e in sql.ToList<Element>())
			{
				ViewSchedule vs = e as ViewSchedule;
				if (!vs.IsTemplate)
					list.Add(new HolderScheduleItem(vs.Id.IntegerValue, vs.Name));
			}

			return list;

		}

		public void GetImage(Document doc, string path)
		{
			List<RebarShape> rebars = new FilteredElementCollector(doc).OfClass(typeof(RebarShape)).WhereElementIsElementType().Cast<RebarShape>().ToList();
			foreach (RebarShape item in rebars)
			{
				Parameter parameter = item.LookupParameter("Shape Image");
				ImageType imageType = doc.GetElement(parameter.AsElementId()) as ImageType;
				if (null != imageType)
				{
					Bitmap bitmap = imageType.GetImage();

					if (bitmap.Width < bitmap.Height)
					{
						bitmap.RotateFlip(RotateFlipType.Rotate90FlipNone);
					}
					//str += "Rong: " + bitmap.Width.ToString() + " Cao: " + bitmap.Height.ToString() + "\n";
					if (File.Exists(path + @"\" + imageType.Name))
					{
						File.Delete(path + @"\" + imageType.Name);
						bitmap.Save(path + @"\" + imageType.Name);
					}
					else
					{
						bitmap.Save(path + @"\" + imageType.Name);
					}
				}
			}
		}

	}

	[Transaction(TransactionMode.Manual)]
	public class Excel2ScheduleCommand : IExternalCommand
	{
		public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
		{
			UIApplication uiapp = commandData.Application;
			UIDocument uidoc = uiapp.ActiveUIDocument;
			Application app = uiapp.Application;
			Document doc = uidoc.Document;

			// Access current selection

			Selection sel = uidoc.Selection;

			//List<ViewSchedule> list = this.filterViewScheduleList(doc);
			List<HolderScheduleItem> holderList = this.filterScheduleNameList(doc);

			Excel2ScheduleForm f = new Excel2ScheduleForm();

			f.Doc = doc;
			f.App = app;
			f.ScheduleList = holderList;
			f.ShowDialog();

			return Result.Succeeded;
		}

		private List<HolderScheduleItem> filterScheduleNameList(Document pDoc)
		{
			FilteredElementCollector collector = new FilteredElementCollector(pDoc);
			collector.WhereElementIsNotElementType();
			collector.WhereElementIsViewIndependent();
			collector.OfClass(typeof(ViewSchedule));

			var sql = from e in collector orderby e.Name ascending select e;

			List<HolderScheduleItem> list = new List<HolderScheduleItem>();

			foreach (Element e in sql.ToList<Element>())
			{
				ViewSchedule vs = e as ViewSchedule;
				//if(vs.CropBox != null)
				if (!vs.IsTemplate)
					list.Add(new HolderScheduleItem(vs.Id.IntegerValue, vs.Name));
			}


			return list;

		}
	}

	//[Transaction(TransactionMode.Manual)]
	//public class ReadMeCommand : IExternalCommand
	//{
	//    public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
	//    {
	//        UIApplication uiapp = commandData.Application;
	//        UIDocument uidoc = uiapp.ActiveUIDocument;
	//        Application app = uiapp.Application;
	//        Document doc = uidoc.Document;

	//        String assemblyFilePath = System.Reflection.Assembly.GetExecutingAssembly().Location;
	//        String assemplyDirPath = System.IO.Path.GetDirectoryName(assemblyFilePath);

	//        String fileName = System.IO.Path.Combine(assemplyDirPath, "changes.txt");
	//        Process.Start(fileName);

	//        return Result.Succeeded;
	//    }
	//}

	//[Transaction(TransactionMode.Manual)]
	//public class RegistrationCommand : IExternalCommand
	//{
	//    public Result Execute(
	//      ExternalCommandData commandData,
	//      ref string message,
	//      ElementSet elements)
	//    {
	//        UIApplication uiapp = commandData.Application;
	//        UIDocument uidoc = uiapp.ActiveUIDocument;
	//        Application app = uiapp.Application;
	//        Document doc = uidoc.Document;

	//        //String assemblyFilePath = System.Reflection.Assembly.GetExecutingAssembly().Location;
	//        //String assemplyDirPath = System.IO.Path.GetDirectoryName(assemblyFilePath);

	//        //SerialKeyValidation validate = new SerialKeyValidation(assemplyDirPath);

	//        //StatusKey sk = validate.checkSerialKey();

	//        //bool passed = true;
	//        //if (sk != StatusKey.Matched)
	//        //{
	//        //    if (!(passed = validate.runRegisterForm("Import/Export To Excel")))
	//        //        return Result.Cancelled;
	//        //}

	//        //if (passed)
	//        //{
	//        //    App.btnExport.Enabled = true;
	//        //    App.btnImport.Enabled = true;
	//        //    TaskDialog.Show("Registration", "This Add-in has been registered.");
	//        //}

	//        return Result.Succeeded;
	//    }
	//}

	public class HolderScheduleItem
	{
		public String ScheduleName
		{
			get;
			set;
		}

		public int Id
		{
			get;
			set;
		}

		public HolderScheduleItem(int id, String name)
		{
			Id = id;
			ScheduleName = name;

		}
	}

	public class HolderFieldItem
	{
		public String FieldName { get; set; }

		public int ParamId { get; set; }

		public bool IsEnable { get; set; }

		public int FieldOrder { get; set; }

		public int TotalColumn { get; set; }

		public HolderFieldItem(int id, String fieldName, int fieldOrder, int totalCol, bool isEnable = true)
		{
			ParamId = id;
			FieldName = fieldName;
			IsEnable = isEnable;
			FieldOrder = fieldOrder;
			TotalColumn = totalCol;
		}

		public override String ToString()
		{
			return ParamId + " | " + FieldName + " | " + IsEnable + " | " + FieldOrder;
		}
	}
}
