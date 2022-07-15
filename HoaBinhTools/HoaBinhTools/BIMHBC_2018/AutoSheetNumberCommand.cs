using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using Autodesk.Revit.ApplicationServices;
using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using Autodesk.Revit.UI.Selection;
//using Software_Key_Manager_Beta;

namespace CreateRibbonTab
{
	[Transaction(TransactionMode.Manual)]
	public class AutoSheetNumberCommand : IExternalCommand
	{
		Stopwatch watch = null;
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
				TaskDialog.Show("BIM", "Please select some Sheet");
				return Result.Cancelled;

			}
			List<ViewSheet> list = this.FilterSheet(doc, sel);
			//this.pr(list);
			Transaction trans = new Transaction(doc, "Set SheetNumber");
			trans.Start();
			this.setOrderNumbering(list);
			trans.Commit();

			//if(watch != null)
			//{
			//    watch.Stop();
			//    int savingTime = (int)(5000 * list.Count - watch.ElapsedMilliseconds) / 1000;
			//    MainForm mainForm = new MainForm();
			//    mainForm.insertRecord2DB("AutoSheetNumber", savingTime);
			//}
			return Result.Succeeded;
		}

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

		private void setOrderNumbering(List<ViewSheet> pList)
		{
			Parameter p = pList[0].LookupParameter("Sheet Number");

			if (p == null)
				return;

			AutoSheetNumberForm f = new AutoSheetNumberForm();
			f.SheetNumber = p.AsString();
			System.Windows.Forms.DialogResult dr = f.ShowDialog();

			if (dr == System.Windows.Forms.DialogResult.Cancel)
				return;

			if (f.PostNumber == -1)
				return;
			int k = f.PostNumber;
			watch = System.Diagnostics.Stopwatch.StartNew();
			for (int i = 0; i < pList.Count; i++)
			{
				ViewSheet item = pList[i];
				Parameter p1 = item.LookupParameter("Sheet Number");

				if (p1 != null)
				{
					String strk = "";
					if (pList.Count < 100)
					{
						strk = k < 10 ? "0" + k.ToString() : k.ToString();
					}
					else
					{
						if (k < 10)
						{
							strk = "00" + k.ToString();
						}
						else if (k < 100)
						{
							strk = "0" + k.ToString();
						}
						else
						{
							strk= k.ToString();
						}
					}


					for (int j = i + 1; j < pList.Count; j++)
					{
						Parameter p2 = pList[j].LookupParameter("Sheet Number");
						if (p2 != null)
						{
							String temp = f.PrefixSheet + strk;
							if (temp.Equals(p2.AsString()))
							{
								p2.Set(f.PrefixSheet + strk + "00");
								break;
							}
						}

					}

					p1.Set(f.PrefixSheet + strk);
					k++;
				}
				else
				{
					TaskDialog.Show("BIM", "No Selection Sheet");
					break;
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
		private String changeEndNumber(String sheetnumber)
		{
			int pos = 0;
			for (int i = sheetnumber.Length; i > 0; i--)
			{

				if (char.IsDigit(sheetnumber, i))
					continue;

				pos = i;
				break;
			}
			return String.Concat(sheetnumber.Substring(pos));
		}
	}
}
