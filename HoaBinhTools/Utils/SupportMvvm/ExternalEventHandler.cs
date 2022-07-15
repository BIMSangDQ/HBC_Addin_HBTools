using System;
using Autodesk.Revit.UI;

namespace Utils
{
	public class ExternalEventHandler : IExternalEventHandler
	{
		protected static ExternalEventHandler instance { get; set; } = null;
		public static ExternalEventHandler Instance
		{
			get
			{
				if (instance == null)
				{
					instance = new ExternalEventHandler();
				}

				return instance;
			}
		}

		public async void Run()
		{
			create.Raise();

			while (create.IsPending)
			{
				await System.Threading.Tasks.Task.Delay(50);

			}

		}


		protected static ExternalEvent create { get; set; } = null;

		public ExternalEvent Create()
		{
			if (create == null)
			{
				create = ExternalEvent.Create(Instance);
			}

			return create;
		}


		protected static Action action;
		public void SetAction(Action Parameter)
		{
			action = Parameter;
		}
		public void Execute(UIApplication app)
		{
			UIDocument uidoc = app.ActiveUIDocument;
			if (null == uidoc)
			{
				TaskDialog.Show("Thông Báo", " no document, nothing to do");
				return;
			}
			action();
		}

		public string GetName()
		{
			return "HoaBinhTools";
		}
	}
}
