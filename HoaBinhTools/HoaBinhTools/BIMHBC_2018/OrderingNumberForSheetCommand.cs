#region Namespaces
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using Autodesk.Revit.ApplicationServices;
using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using Autodesk.Revit.UI.Selection;
using Geometry;
//using Software_Key_Manager_Beta;
#endregion

namespace CreateRibbonTab
{
	[Transaction(TransactionMode.Manual)]
	public class OrderingNumberForSheetCommand : IExternalCommand
	{
		public Result Execute(
		  ExternalCommandData commandData,
		  ref string message,
		  ElementSet elements)
		{
			//String assemblyFilePath = System.Reflection.Assembly.GetExecutingAssembly().Location;
			//String assemplyDirPath = System.IO.Path.GetDirectoryName(assemblyFilePath);
			//frmInstallLicense validate = new frmInstallLicense();
			//validate.RegKeyPath = assemplyDirPath;
			//if (validate.checkFromRegFile() != frmInstallLicense.StatusKey.Matched)
			//{
			//    MainForm regForm = new MainForm();
			//    regForm.Show();
			//    return Result.Succeeded;
			//}
			//else
			//{

			UIApplication uiapp = commandData.Application;
			UIDocument uidoc = uiapp.ActiveUIDocument;
			Application app = uiapp.Application;
			Document doc = uidoc.Document;


			Selection sel = uidoc.Selection;

			if (sel.GetElementIds().Count == 0)
			{
				TaskDialog.Show(TAG, "Please select some Sheet");
				return Result.Failed;
			}
			List<ViewSheet> list = this.FilterSheet(doc, sel);

			ViewSheet sheet = list[0];
			Stopwatch watch;
			try
			{
				OrderingNumberForSheetForm f = new OrderingNumberForSheetForm();
				System.Windows.Forms.DialogResult dr = f.ShowDialog();
				if (dr != System.Windows.Forms.DialogResult.OK)
				{

					return Result.Cancelled;
				}
				Transaction trans = new Transaction(doc, "SET STT");
				trans.Start();
				int n = f.StartNumber;
				watch = System.Diagnostics.Stopwatch.StartNew();
				CategorySet cs = new CategorySet();
				Category sheets = Category.GetCategory(doc, BuiltInCategory.OST_Sheets);
				cs.Insert(sheets);
				ParameterUtil.AddParameter(doc, "HB_STT", AddParameterType.Instance, ParameterType.Text, BuiltInParameterGroup.PG_IDENTITY_DATA, cs);
				this.addParameterToSheetInstance(doc, app, sheet, "HB_STT");
				this.setOrderNumbering(list, n);
				trans.Commit();
			}

			catch
			{
				return Result.Cancelled;
			}
			watch.Stop();
			//int savingTime = (int)(2000 * list.Count  - watch.ElapsedMilliseconds) / 1000;
			//MainForm mainForm = new MainForm();
			//mainForm.insertRecord2DB("OrderingNumberForSheet", savingTime);
			return Result.Succeeded;
			//}
		}
		private const String TAG = "BimHoaBinh";

		private List<ViewSheet> FilterSheet(Document pDoc, Selection pSelection)
		{
			List<ViewSheet> list = new List<ViewSheet>();


			// FilteredElementCollector eleCollector = new FilteredElementCollector(pDoc).OfClass(typeof(ViewSheet));

			FilteredElementCollector eleCollector = new FilteredElementCollector(pDoc, pSelection.GetElementIds());
			eleCollector.OfClass(typeof(ViewSheet));
			list = eleCollector.Cast<ViewSheet>().ToList<ViewSheet>();

			var sql = from ele in list orderby ele.SheetNumber ascending select ele;
			return sql.ToList<ViewSheet>();

		}

		private void setOrderNumbering(List<ViewSheet> pList, int i)
		{

			foreach (ViewSheet item in pList)
			{
				Parameter p = item.LookupParameter("HB_STT");
				Debug.WriteLine(item.SheetNumber);
				if (p != null)
				{
					//p.Set((i++).ToString()); so 1-n
					//so 01-09-n
					if (pList.Count + i > 100)
					{
						string str_number = (i < 10) ? "00" + i : i.ToString();
						if (i < 10)
						{
							str_number = "00" + i.ToString();
						}
						else if (i < 100)
						{
							str_number = "0" + i.ToString();
						}
						else
						{
							str_number = i.ToString();
						}

						p.Set(str_number);
						i++;
					}
					else
					{
						string str_number = (i < 10) ? "0" + i : i.ToString();
						p.Set(str_number);
						i++;
					}

				}
				else
				{
					TaskDialog.Show("BIM", "Null Parameter STT");
				}
			}
		}

		private void pr(List<ViewSheet> list)
		{
			StringBuilder sb = new StringBuilder();
			foreach (ViewSheet vs in list)
			{
				sb.AppendLine(vs.SheetNumber);
			}

			TaskDialog.Show("TEST", sb.ToString());
		}
		private void addParameterToSheetInstance(Document pDoc, Application pApp, ViewSheet pSheet, String paramName)
		{

			Parameter IdParam = pSheet.LookupParameter(paramName);
			if (IdParam != null)
				return;

			//String strTplPath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
			//String strSharedParameterFilePath = Path.Combine(strTplPath, GlobalVar.RES_FOLDER, GlobalVar.SHARED_PARAM_FILE_NAME + GlobalVar.SHARED_PARAM_FILE_EXT);
			String strFileTemp = System.IO.Path.Combine(System.IO.Path.GetTempPath(), "temp" + "_AssemblyParameter.txt");
			using (File.Create(strFileTemp)) { }
			pApp.SharedParametersFilename = strFileTemp;

			DefinitionFile sharedFile = pApp.OpenSharedParameterFile();
			Definition myDefinition_ShapeID = null;


			// create a new group in the shared parameters file
			DefinitionGroups myGroups = sharedFile.Groups;
			DefinitionGroup myGroup = myGroups.Create(paramName);

			ExternalDefinitionCreationOptions options = new ExternalDefinitionCreationOptions(paramName, ParameterType.Text);
			options.Visible = true;
			options.UserModifiable = true; //true la cho user edit, false la khong cho edit

			options.Description = "Mapping Group/Assembly instance's ID";
			myDefinition_ShapeID = myGroup.Definitions.Create(options);
			// create a category set and insert category of TextNote to it
			CategorySet myCategories = pApp.Create.NewCategorySet();
			// use BuiltInCategory to get category of TextNote
			Category myCategory = Category.GetCategory(pDoc, BuiltInCategory.OST_Sheets);

			myCategories.Insert(myCategory);

			//Create an instance of InstanceBinding
			InstanceBinding instanceBinding = pApp.Create.NewInstanceBinding(myCategories);

			// Get the BingdingMap of current document.
			BindingMap bindingMap = pDoc.ParameterBindings;

			// Bind the definitions to the document
			bool instanceBindOK = bindingMap.Insert(myDefinition_ShapeID,
											instanceBinding, BuiltInParameterGroup.PG_IDENTITY_DATA);
			File.Delete(strFileTemp);
		}
	}
}
