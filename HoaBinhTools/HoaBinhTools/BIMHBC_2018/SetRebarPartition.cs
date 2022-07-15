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
namespace CreateRibbonTab
{
	[Transaction(TransactionMode.Manual)]
	public class SetRebarPartition : IExternalCommand
	{
		private const String TAG = "Study";
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
			if (sel.GetElementIds().Count == 0)
			{
				TaskDialog.Show(TAG, "Please select some Elements");
				return Result.Failed;
			}
			try
			{
				List<Element> eleSelection = new List<Element>();
				List<Element> list1 = this.FilterRebar(doc, sel);
				eleSelection.AddRange(list1);
				List<Element> list2 = this.FilterRebarFromAssembly(doc, sel);
				foreach (Element e in list2)
				{
					if (this.isContainElement(eleSelection, e))
						continue;

					eleSelection.Add(e);
				}
				List<Element> list3 = this.FilterRebarFromGroup(doc, sel);
				eleSelection.AddRange(list3);

				using (Transaction tran = new Transaction(doc))
				{
					tran.Start("Begin Set");
					if (eleSelection.Count == 0)
					{
						TaskDialog.Show(TAG, "No selected Rebar");
						return Result.Failed;
					}

					//Element host = doc.GetElement((eleSelection[0] as Rebar).GetHostId());

					PartitionRebarForm f = new PartitionRebarForm();
					f.BeamName = this.getFirstStirrupRebarHostName(eleSelection, doc);

					System.Windows.Forms.DialogResult dr = f.ShowDialog();

					if (dr == System.Windows.Forms.DialogResult.OK)
					{
						String new_beam_name = f.BeamName;

						StringBuilder sb = new StringBuilder();
						foreach (Rebar r in eleSelection)
						{
							setPartitionParameter(r, new_beam_name, ref sb);
						}
					}

					tran.Commit();
				}

			}
			catch
			{
				return Result.Cancelled;
			}



			return Result.Succeeded;
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

		private string getFirstStirrupRebarHostName(List<Element> pList, Document pDoc)
		{
			ElementId hostId = null;

			foreach (Element e in pList)
			{
				Rebar r = e as Rebar;
				Parameter p = r.LookupParameter("Style");

				if (p != null && p.AsInteger() > 0)
				{
					hostId = r.GetHostId();
					break;
				}
				else
				{
					hostId = r.GetHostId();
				}

			}

			if (hostId != null)
			{
				Element eleHost = pDoc.GetElement(hostId);
				if (eleHost != null)
					return eleHost.Name;
			}

			return "";
		}

		private List<Element> FilterRebar(Document pDoc, Selection pSelection)
		{
			List<Element> list = new List<Element>();

			FilteredElementCollector eleCollector = new FilteredElementCollector(pDoc, pSelection.GetElementIds());
			eleCollector.OfClass(typeof(Rebar));
			list = eleCollector.ToElements() as List<Element>;

			return list;
		}
		//private List<Element> FilterSRebar(Document pDoc, Selection pSelection) { }
		private bool setPartitionParameter(Rebar ele, String pContent, ref StringBuilder sb)
		{
			if (ele == null)
				return false;

			Parameter p = ele.LookupParameter("Partition");
			if (p != null && p.IsReadOnly == false)
			{
				p.Set(pContent);
				String strResult = ele.Id.ToString();
				strResult += " | " + ele.Name;
				strResult += " | " + ele.GetType().Name;
				strResult += (ele.Category != null) ? "[" + ele.Category.Name + "]" : "";
				strResult += " | " + p.AsString();

				sb.AppendLine(strResult);
				return true;
			}
			return false;
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
