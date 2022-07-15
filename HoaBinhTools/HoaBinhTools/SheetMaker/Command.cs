#region Namespaces
using System;
using System.Collections.Generic;
using System.Linq;
using Autodesk.Revit.ApplicationServices;
using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using Autodesk.Revit.UI.Selection;
//using Vii.SecurityLib;
#endregion

namespace SheetDuplicateAndAlignView
{
	[Transaction(TransactionMode.Manual)]
	public class DuplicateViewCommand : IExternalCommand
	{
		private static String TAG = "Duplicate View";
		public Result Execute(
		  ExternalCommandData commandData,
		  ref string message,
		  ElementSet elements)
		{
			UIApplication uiapp = commandData.Application;
			UIDocument uidoc = uiapp.ActiveUIDocument;
			Application app = uiapp.Application;
			Document doc = uidoc.Document;


			// Access current selection

			Selection sel = uidoc.Selection;

			if (sel.GetElementIds().Count == 0)
			{
				TaskDialog.Show(TAG, "Please, select a view.");
				return Result.Cancelled;
			}

			// Retrieve elements from database
			FilteredElementCollector eleCollector = new FilteredElementCollector(doc, sel.GetElementIds())
				.WhereElementIsNotElementType()
				.OfClass(typeof(View));
			//.OfCategory(BuiltInCategory.OST_Views);


			List<View> selectedViews = new List<View>();
			selectedViews = eleCollector.Cast<View>().ToList<View>();

			if (selectedViews.Count == 0)
			{
				TaskDialog.Show(TAG, "Please, select a view.");
				return Result.Cancelled;
			}


			Forms.ViewDuplicaterForm f = new Forms.ViewDuplicaterForm();
			f.App = app;
			f.Doc = doc;
			f.SelectedViews = selectedViews;
			f.ShowDialog();

			return Result.Succeeded;
		}
	}

	[Transaction(TransactionMode.Manual)]
	public class AlignViewCommand : IExternalCommand
	{
		private static String TAG = "Align Views";
		public Result Execute(
		  ExternalCommandData commandData,
		  ref string message,
		  ElementSet elements)
		{
			UIApplication uiapp = commandData.Application;
			UIDocument uidoc = uiapp.ActiveUIDocument;
			Application app = uiapp.Application;
			Document doc = uidoc.Document;


			// Access current selection

			Selection sel = uidoc.Selection;


			Forms.AlignViewForm f = new Forms.AlignViewForm();
			f.App = app;
			f.Doc = doc;
			f.ShowDialog();

			return Result.Succeeded;
		}
	}

	[Transaction(TransactionMode.Manual)]
	public class SheetMakerCommand : IExternalCommand
	{
		private static String TAG = "Sheet Maker";
		public Result Execute(
		  ExternalCommandData commandData,
		  ref string message,
		  ElementSet elements)
		{
			UIApplication uiapp = commandData.Application;
			UIDocument uidoc = uiapp.ActiveUIDocument;
			Application app = uiapp.Application;
			Document doc = uidoc.Document;


			// Access current selection

			Selection sel = uidoc.Selection;

			Forms.SheetMakerForm f = new Forms.SheetMakerForm();
			f.App = app;
			f.Doc = doc;
			f.ShowDialog();

			return Result.Succeeded;
		}
	}

	//[Transaction(TransactionMode.Manual)]
	//public class ReadMeCommand : IExternalCommand
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

	//        String assemblyFilePath = System.Reflection.Assembly.GetExecutingAssembly().Location;
	//        String assemplyDirPath = System.IO.Path.GetDirectoryName(assemblyFilePath);

	//        SerialKeyValidation validate = new SerialKeyValidation(assemplyDirPath);
	//        StatusKey sk = validate.checkSerialKey();

	//        bool passed = true;
	//        if (sk != StatusKey.Matched)
	//        {
	//            if (!(passed = validate.runRegisterForm("Sheet Maker - Duplicate & Align Views")))
	//                return Result.Cancelled;
	//        }

	//        if (passed)
	//        {

	//            App.Instance.btnSheetMaker.Enabled = true;
	//            App.Instance.btnDuplicateView.Enabled = true;
	//            App.Instance.btnAlignView.Enabled = true;
	//            TaskDialog.Show("Registration", "This Add-in has been registered.");
	//        }

	//        return Result.Succeeded;
	//    }
	//}
}
