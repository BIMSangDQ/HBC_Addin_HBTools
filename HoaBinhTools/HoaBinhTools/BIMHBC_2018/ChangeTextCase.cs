using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Autodesk.Revit.ApplicationServices;
using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using Autodesk.Revit.UI.Selection;
using ChangeTextCase;

namespace CreateRibbonTab
{
	[Transaction(TransactionMode.Manual)]
	public class ChangeTextCase : IExternalCommand
	{
		const String TAG = "Change Text Case";


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
			List<TextNote> list = this.FilterTextNote(doc, sel);
			if (list == null)
			{
				TaskDialog.Show(TAG, "You haven't selected any TextNote.");
				return Result.Failed;
			}

			//Form
			ChangeTextCaseForm f = new ChangeTextCaseForm();
			f.ShowDialog();

			if (f.Case == TextCase.NOTHING)
				return Result.Cancelled;

			this.convertText(doc, list, f.Case);
			//doc.Save();

			return Result.Succeeded;
		}
		private List<TextNote> FilterTextNote(Document pDoc, Selection pSelection)
		{
			List<TextNote> list = null;

			IList<Reference> refs = pSelection.PickObjects(ObjectType.Element, new TextNoteSelectionFilter(), "Select TextNote");

			if (refs.Count > 0)
			{
				List<ElementId> ids = new List<ElementId>(refs.Select<Reference, ElementId>(r => r.ElementId));

				list = new List<TextNote>();
				foreach (ElementId id in ids)
				{
					list.Add(pDoc.GetElement(id) as TextNote);
				}
			}

			return list;

		}


		private void convertText(Document pDoc, List<TextNote> noteList, TextCase convertType)
		{

			int counter = 0;

			using (Transaction curTrans = new Transaction(pDoc, "Current Transaction"))
			{
				if (curTrans.Start() == TransactionStatus.Started)
				{

					foreach (TextNote curNote in noteList)
					{
						switch (convertType)
						{
							case TextCase.UPPER:
								//curNote.Text = curNote.Text.ToUpper();
								curNote.Text = CultureInfo.CurrentCulture.TextInfo.ToUpper(curNote.Text);
								break;

							case TextCase.LOWER:
								//curNote.Text = curNote.Text.ToLower();
								curNote.Text = CultureInfo.CurrentCulture.TextInfo.ToLower(curNote.Text);
								break;

							case TextCase.CAPITALIZE:
								curNote.Text = CultureInfo.CurrentCulture.TextInfo.ToTitleCase(curNote.Text);
								break;
						}

						//increment counter
						counter = counter + 1;
					}
				}

				//commit changes
				curTrans.Commit();
			}

			//alert user
			TaskDialog.Show(TAG, "Converted " + counter + " text notes.");

		}



	}

	class TextNoteSelectionFilter : ISelectionFilter
	{
		public bool AllowElement(Element e)
		{
			return e is TextNote;
		}

		public bool AllowReference(Reference r, XYZ p)
		{
			return true;
		}
	}

	public enum TextCase
	{
		NOTHING = 0, UPPER = 1, LOWER = 2, CAPITALIZE = 3
	};
}
