#region Namespaces
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Autodesk.Revit.ApplicationServices;
using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.DB.Structure;
using Autodesk.Revit.UI;
using Autodesk.Revit.UI.Selection;
//using Software_Key_Manager_Beta;
#endregion

namespace CreateRibbonTab
{
	[Transaction(TransactionMode.Manual)]
	public class filterRebar : IExternalCommand
	{
		private const String TAG = "BimHoaBinh";
		public Result Execute(
		  ExternalCommandData commandData,
		  ref string message,
		  ElementSet elements)
		{
			UIApplication uiapp = commandData.Application;
			UIDocument uidoc = uiapp.ActiveUIDocument;
			Application app = uiapp.Application;
			Document doc = uidoc.Document;
			Selection sel = uidoc.Selection;

			try
			{
				if (sel.GetElementIds().Count == 0)
				{
					List<Element> elems = sel.PickElementsByRectangle(new RebarSelectionFilter()) as List<Element>;
					sel.SetElementIds(elems.Select(x => x.Id).ToList());
					//sel.SetElementIds(sel.PickElementsByRectangle(new RebarSelectionFilter()).Select(x => x.Id).ToList());
				}
				var watch = System.Diagnostics.Stopwatch.StartNew();
				////List<Element> listRebar = this.FilterRebar(doc, sel);
				//FilteredElementCollector eleCollector = new FilteredElementCollector(doc, sel.GetElementIds());
				//eleCollector.OfClass(typeof(Group));
				//List<Element> listRebarGroup = this.FilterRebarFromGroup(doc, eleCollector);




				List<ElementId> listId = new List<ElementId>();

				listId.AddRange(this.getIds(this.FilterRebar(doc, sel)));
				listId.AddRange(this.getIds(this.FilterRebarFromGroup(doc, sel)));
				listId.AddRange(this.getIds(this.FilterRebarFromAssembly(doc, sel)));
				Transaction trans = new Transaction(doc, "SELECT REBAR");
				trans.Start();
				sel.SetElementIds(listId);
				trans.Commit();

				watch.Stop();
				//int savingTime = (int)(5000 - watch.ElapsedMilliseconds) / 1000;
				//MainForm mainForm = new MainForm();
				//mainForm.insertRecord2DB("FilterRebar", savingTime);
				return Result.Succeeded;
			}
			catch
			{
				return Result.Cancelled;
			}
		}

		private void pr(List<Element> list)
		{
			StringBuilder sb = new StringBuilder();

			foreach (Element e in list)
			{
				sb.AppendLine(e.Id.IntegerValue.ToString());
			}
			TaskDialog.Show(TAG, sb.ToString());
		}

		// Filter Group
		private List<Element> FilterRebarFromGroup(Document pDoc, Selection sel)
		{
			List<Element> list = new List<Element>();

			FilteredElementCollector eleCollector = new FilteredElementCollector(pDoc, sel.GetElementIds());
			eleCollector.OfClass(typeof(Group));

			FilteredElementCollector eleCollector2 = new FilteredElementCollector(pDoc);
			eleCollector2.OfClass(typeof(Rebar));
			foreach (Element e in eleCollector)
			{
				var query = from ele in eleCollector2
							where (
								ele.GroupId.Equals(e.Id)
							)
							select ele;
				list.AddRange(query.ToList<Element>());
			}

			StringBuilder sb = new StringBuilder();

			//this.pr(list);

			return list;
		}
		private List<Element> FilterRebarFromAssembly(Document pDoc, Selection sel)
		{
			List<Element> list = new List<Element>();

			FilteredElementCollector eleCollector = new FilteredElementCollector(pDoc, sel.GetElementIds());
			eleCollector.OfClass(typeof(AssemblyInstance));

			FilteredElementCollector eleCollector2 = new FilteredElementCollector(pDoc);
			eleCollector2.OfClass(typeof(Rebar));
			foreach (Element e in eleCollector)
			{
				var query = from ele in eleCollector2
							where (
								ele.AssemblyInstanceId.Equals(e.Id)
							)
							select ele;
				list.AddRange(query.ToList<Element>());
			}

			StringBuilder sb = new StringBuilder();

			//this.pr(list);

			return list;
		}


		private List<Element> FilterElement(Document pDoc, Selection pSelection)
		{
			List<Element> list = new List<Element>();

			FilteredElementCollector eleCollector = new FilteredElementCollector(pDoc, pSelection.GetElementIds());
			eleCollector.WhereElementIsNotElementType();
			eleCollector.WhereElementIsViewIndependent();
			list = eleCollector.ToElements() as List<Element>;
			return list;
		}

		private List<Element> FilterRebar(Document pDoc, Selection pSelection)
		{
			List<Element> list = new List<Element>();

			FilteredElementCollector eleCollector = new FilteredElementCollector(pDoc, pSelection.GetElementIds());
			eleCollector.OfCategory(BuiltInCategory.OST_Rebar);
			eleCollector.OfClass(typeof(Rebar));

			list = eleCollector.ToElements() as List<Element>;

			//this.pr(list);

			return list;
		}

		private List<ElementId> getIds(List<Element> pList)
		{
			List<ElementId> ids = new List<ElementId>();
			foreach (Element e in pList)
			{
				ids.Add(e.Id);
			}

			return ids;
		}
	}
	public class RebarSelectionFilter : ISelectionFilter
	{
		public bool AllowElement(Element elem)
		{
			if (elem is Rebar) return true;
			return false;
		}
		public bool AllowReference(Reference reference, XYZ position)
		{
			return false;
		}
	}
}

