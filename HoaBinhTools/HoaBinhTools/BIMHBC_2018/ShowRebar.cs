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

namespace CreateRibbonTab
{
	[Transaction(TransactionMode.Manual)]
	public class ShowRebar : IExternalCommand
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

			var watch = System.Diagnostics.Stopwatch.StartNew();
			if (sel.GetElementIds().Count == 0)
			{
				TaskDialog.Show(TAG, "Please select some Rebar");
				return Result.Failed;
			}
			List<Element> listRebar = new List<Element>();

			List<Element> list1 = this.FilterRebar(doc, sel);
			listRebar.AddRange(list1);

			List<Element> list2 = this.FilterRebarFromAssembly(doc, sel);
			foreach (Element e in list2)
			{
				if (this.isContainElement(listRebar, e))
					continue;

				listRebar.Add(e);
			}

			List<Element> list3 = this.FilterRebarFromGroup(doc, sel);
			listRebar.AddRange(list3);


			//this.pr(listRebar);

			using (Transaction tran = new Transaction(doc))
			{
				tran.Start("Begin Set");
				int k = 0;
				foreach (Element e in listRebar)
				{
					Rebar r = e as Rebar;
					if (doc.ActiveView.GetType().Equals(typeof(View3D)))
					{
						//View3D v3d = doc.ActiveView as View3D;
						r.SetSolidInView(doc.ActiveView as View3D, true);
						//r.SetUnobscuredInView(doc.ActiveView, !r.IsUnobscuredInView(doc.ActiveView));
						r.SetUnobscuredInView(doc.ActiveView, true);
						k++;
					}
					else
					{
						r.SetUnobscuredInView(doc.ActiveView, !r.IsUnobscuredInView(doc.ActiveView));

						k++;
					}

				}

				tran.Commit();
				watch.Stop();
				int savingTime = (int)(6000 - watch.ElapsedMilliseconds) / 1000;
				//MainForm mainForm = new MainForm();
				//mainForm.insertRecord2DB("ShowRebar", savingTime);
				//TaskDialog.Show(TAG, "Total Rebar: " + k.ToString());
				return Result.Succeeded;
			}
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
			eleCollector.OfClass(typeof(Rebar));
			list = eleCollector.ToElements() as List<Element>;
			return list;
		}
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


			//this.pr(list);

			return list;
		}

		private void pr(List<Element> list)
		{
			StringBuilder sb = new StringBuilder();
			foreach (Element e in list)
			{
				sb.AppendLine(e.Id.IntegerValue.ToString());
			}

			TaskDialog.Show("TEST", sb.ToString());
		}

		private bool isContainElement(List<Element> list, Element item)
		{
			bool b = false;
			foreach (Element e in list)
			{
				if (e.Id.Equals(item.Id))
				{
					b = true;
					break;
				}
			}
			return b;
		}
	}
}
