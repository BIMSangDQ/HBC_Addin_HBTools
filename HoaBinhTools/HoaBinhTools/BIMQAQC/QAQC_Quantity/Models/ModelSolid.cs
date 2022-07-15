using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autodesk.Revit.DB;

namespace HoaBinhTools.BIMQAQC.QAQC_Quantity.Models
{
	public class ModelSolid
	{
		public List<GuiIdSolid> GuiIdSolids { get; set; }
		public string ModelName { get; set; }
		public string GuiIdModel { get; set; }
	}
	public class GuiIdSolid
	{
		public List<Solid> Solids { get; set; }
		public string GuiId { get; set; }
	}
}
