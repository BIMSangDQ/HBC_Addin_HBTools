using Autodesk.Revit.ApplicationServices;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;

namespace Utils
{

	public class ActiveData
	{
		public static UIDocument UIDoc;
		public static Document Document;
		public static Application Application;
		public static UIApplication UIApplication;
		public static Autodesk.Revit.UI.Selection.Selection Selection;
		public static View ActiveView;
		public static string Username;
		public static Plane ViewPlane;

		public static void GetInformation(UIDocument uidoc)
		{
			UIDoc = uidoc;
			Document = uidoc.Document;
			Application = uidoc.Application.Application;
			UIApplication = uidoc.Application;
			Selection = uidoc.Selection;
			Username = Application.Username;
			ActiveView = Document.ActiveView;
			ViewPlane = Plane.CreateByNormalAndOrigin(ActiveView.ViewDirection, ActiveView.Origin);

		}

		public static void GetInformation(Document doc)
		{
			UIDoc = null;
			Document = doc;
			Application = doc.Application;
			UIApplication = null;
			Username = Application.Username;
			ActiveView = Document.ActiveView;

		}

	}
}
