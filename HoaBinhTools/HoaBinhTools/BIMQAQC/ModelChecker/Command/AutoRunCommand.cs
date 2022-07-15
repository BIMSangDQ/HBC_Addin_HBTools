using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using Autodesk.Revit.UI.Events;

namespace HoaBinhTools.BIMQAQC.ModelChecker.Command
{
    [Transaction (TransactionMode.Manual)]
	public class AutoRunCommand: IExternalCommand
	{
        TextNote textNote = null;
        String oldDateTime = null;
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            UIApplication uiApp = new UIApplication(commandData.Application.Application);
            Document doc = commandData.Application.ActiveUIDocument.Document;
            using (Transaction t = new Transaction(doc, "Text Note Creation"))
            {
                t.Start();
                oldDateTime = DateTime.Now.ToString();
                ElementId defaultTextTypeId = doc.GetDefaultElementTypeId(ElementTypeGroup.TextNoteType);
                textNote = TextNote.Create(doc, doc.ActiveView.Id, XYZ.Zero, oldDateTime, defaultTextTypeId);
                t.Commit();
            }
            uiApp.Idling += new EventHandler<IdlingEventArgs>(idleUpdate);
            return Result.Succeeded;
        }
        public void idleUpdate(object sender, IdlingEventArgs e)
        {
            UIApplication uiApp = sender as UIApplication;
            UIDocument uidoc = uiApp.ActiveUIDocument;
            Document doc = uiApp.ActiveUIDocument.Document;
            if (oldDateTime != DateTime.Now.ToString())
            {
                using (Transaction transaction = new Transaction(doc, "Text Note Update"))
                {
                    transaction.Start();
                    textNote.Text = DateTime.Now.ToString();
                    uidoc.RefreshActiveView();
                    transaction.Commit();
                }
                oldDateTime = DateTime.Now.ToString();
            }
        }
    }
}
