using System;
using Autodesk.Revit.UI;
using Constant;

namespace Model.Event
{
	public class ExternalEventHandler : IExternalEventHandler
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
			return ConstantValue.AddinName;
		}
	}
}
