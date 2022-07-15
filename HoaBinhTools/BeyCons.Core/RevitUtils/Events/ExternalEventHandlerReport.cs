#region Using
using Autodesk.Revit.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks; 
#endregion

namespace BeyCons.Core.RevitUtils.Events
{
    public class ExternalEventHandlerReport : IExternalEventHandler
    {
        private Action action;
        public void SetAction(Action action)
        {
            this.action = action;
        }
        public void Execute(UIApplication app)
        {
            action();
        }
        public string GetName()
        {
            return "Action Revit Report";
        }
    }
}
