using System.Collections.Generic;
using System.Linq;
using Autodesk.Revit.DB;


namespace HoaBinhTools.ProjectWarnings.Models
{
	public class ObjectWaring
	{
		public int Stt { get; set; }

		public string Description { get; set; }

		public FailureSeverity TypeWarning { get; set; }

		public List<ElementId> ElementWarning { get; set; }

		public string GUIID { get; set; }

		public ObjectWaring(int i, FailureMessage Wn)
		{
			Stt = i;

			ElementWarning = Wn.GetFailingElements().ToList();

			TypeWarning = Wn.GetSeverity();

			Description = Wn.GetDescriptionText();

			GUIID = Wn.GetFailureDefinitionId().Guid.ToString();
		}

	}
}
