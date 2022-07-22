using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autodesk.Revit.DB;

namespace HoaBinhTools.BIMQAQC.QAQC_FormWork.Models
{
	public class VerticalFaceFilter
	{
		public CommonFaceFilter CommonFaceFilter { get; set; }
		public List<Face> FillFaces { get; set; }
	}
}
