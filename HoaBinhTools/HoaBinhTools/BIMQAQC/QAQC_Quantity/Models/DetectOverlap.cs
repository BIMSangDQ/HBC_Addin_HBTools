using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autodesk.Revit.DB;

namespace HoaBinhTools.BIMQAQC.QAQC_Quantity.Models
{
	public class DetectOverlap
	{
		public ElementId Element1 { get; set; }

		public string Element1CateGory {get; set; }

		public ElementId Element2 { get; set; }

		public string Element2CateGory { get; set; }

		public double OverlapVolume { get; set; }
	}
}
