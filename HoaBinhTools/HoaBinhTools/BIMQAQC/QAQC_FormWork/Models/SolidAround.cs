using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autodesk.Revit.DB;

namespace HoaBinhTools.BIMQAQC.QAQC_FormWork.Models
{
	public class SolidAround
	{
		public List<Solid> BottomSolids { get; set; }
		public List<Solid> SideSolids { get; set; }
	}
}
