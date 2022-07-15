#region Using
using Autodesk.Revit.UI;
using System;
using System.Collections.Generic;
#endregion

namespace BeyCons.Core.RevitUtils.Events
{
    public class ExternalEventHandlerMultiple : IExternalEventHandler
    {
        private readonly List<Action> actions = new List<Action>();
        public void SetAction(Action action)
        {
            actions.Add(action);
        }
        public void Execute(UIApplication app)
        {
            foreach (Action action in actions)
            {
                action();
            }
        }
        public string GetName()
        {
            return "Action Revit Multiple";
        }
    }
}