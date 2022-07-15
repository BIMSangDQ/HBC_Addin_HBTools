#region Namespaces
using System;
using System.Collections.Generic;
using Autodesk.Revit.ApplicationServices;
using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using Autodesk.Revit.UI.Selection;
#endregion

namespace CreateRibbonTab
{

	[Transaction(TransactionMode.Manual)]
	public class SetMarkForBeam : IExternalCommand
	{
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
				TaskDialog.Show(TAG, "No select Beam");
				return Result.Failed;
			}
			List<Element> filter = this.FilterFamilyInstance(doc, sel);

			Transaction tran = new Transaction(doc, "SET MARK");
			tran.Start();
			BeamLevelNumberForm f = new BeamLevelNumberForm();
			f.ShowDialog();

			if (f.ButtonVal)
				this.setMark(doc, filter, f.LevelText);

			tran.Commit();

			return Result.Succeeded;
		}
		private const String TAG = "BimHoaBinh";
		private List<Element> FilterFamilyInstance(Document pDoc, Selection pSelection)
		{
			List<Element> list = new List<Element>();

			FilteredElementCollector eleCollector = new FilteredElementCollector(pDoc, pSelection.GetElementIds());
			eleCollector.OfClass(typeof(FamilyInstance));
			list = eleCollector.ToElements() as List<Element>;
			return list;
		}

		private void setMark(Document pDoc, List<Element> pList, String levelText)
		{

			foreach (Element p in pList)
			{
				FamilyInstance fi = p as FamilyInstance;
				if (fi.StructuralType == Autodesk.Revit.DB.Structure.StructuralType.Beam)
				{
					Parameter para = fi.LookupParameter("Mark");
					if (para != null && !para.IsReadOnly)
					{
						string typename = (pDoc.GetElement(fi.GetTypeId()) as ElementType).Name;
						string strNewName = changeFirstNumber(typename, levelText);
						para.Set(strNewName);
					}

				}
			}
		}
		private String changeFirstNumber(String typename, String number)
		{
			int pos = 0;
			for (int i = 0; i < typename.Length; i++)
			{
				if (char.IsDigit(typename, i))
					continue;

				pos = i;
				break;
			}

			return String.Concat(number, typename.Substring(pos));
		}
	}
}
