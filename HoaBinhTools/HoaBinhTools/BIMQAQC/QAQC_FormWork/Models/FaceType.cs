using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autodesk.Revit.DB;

namespace HoaBinhTools.BIMQAQC.QAQC_FormWork.Models
{
	public class FaceType
	{
		public List<Face> OriginFaces { get; set; }
		public List<Face> RegainFaces { get; set; }
	}
}
