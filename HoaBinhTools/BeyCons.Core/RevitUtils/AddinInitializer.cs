#region Using
using Autodesk.Revit.UI;
using BeyCons.Core.Extensions;
using OfficeOpenXml;
using System.IO;
#endregion

namespace BeyCons.Core.RevitUtils
{
    public class AddinInitializer
    {
        public static void CreateInitialize(UIApplication uIApplication, string addinKey)
        {
            RevitData.Instance = new RevitData
            {
                UIApplication = uIApplication,
                AddInKey = addinKey
        };
            RevitData.Instance.ExternalEventReport = ExternalEvent.Create(RevitData.Instance.ExternalEventHandlerReport);
            RevitData.Instance.ExternalEventSingle = ExternalEvent.Create(RevitData.Instance.ExternalEventHandlerSingle);
            RevitData.Instance.ExternalEventMultiple = ExternalEvent.Create(RevitData.Instance.ExternalEventHandlerMultiple);
        }
    }
}