using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autodesk.Revit.DB;

namespace HoaBinhTools.BIMQAQC.QAQC_FormWork.Models
{
	public class FaceFilter
	{
		public List<Face> TopFaces { get; set; }
		public List<Face> SideFaces { get; set; }
		public List<Face> BottomFaces { get; set; }
	}
}
