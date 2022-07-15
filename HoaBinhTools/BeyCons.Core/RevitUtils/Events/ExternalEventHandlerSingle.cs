#region Using
using Autodesk.Revit.UI;
using BeyCons.Core.FormUtils;
using BeyCons.Core.FormUtils.Reports;
using System;
#endregion

namespace BeyCons.Core.RevitUtils.Events
{
    public class ExternalEventHandlerSingle : IExternalEventHandler
    {
        private Action action;
        public void SetAction(Action action)
        {
            this.action = action;
        }
        public void Execute(UIApplication app)
        {
            action();

            //Bold tab control
            ReportData.Instance.HighLight();
        }
        public string GetName()
        {
            return "Action Revit Single";
        }
    }
}
