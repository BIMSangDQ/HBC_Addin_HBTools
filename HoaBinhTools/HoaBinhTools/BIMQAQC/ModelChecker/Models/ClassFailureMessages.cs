using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autodesk.Revit.DB;

namespace HoaBinhTools.BIMQAQC.ModelChecker.Models
{
	public class ClassFailureMessages
	{
		public string Document {get;set;}
		public string DesiredRule { get;set;}
		public List<ElementFailure> ListElement { get; set; }
	}

	public class ElementFailure
	{
		public ElementId ElementId { get; set; }

		public Category Category { get; set; }
	}
}
