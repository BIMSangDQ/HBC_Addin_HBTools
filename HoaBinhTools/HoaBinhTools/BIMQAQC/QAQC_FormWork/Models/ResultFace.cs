using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autodesk.Revit.DB;

namespace HoaBinhTools.BIMQAQC.QAQC_FormWork.Models
{
	public class ResultFace
	{
        public CommonFaceFilter CommonFaceFilter { get; set; }
        public List<Face> FillFaces { get; set; }
        public string ApproxBSide { get; set; }
        public string ApproxESide { get; set; }
        public string ApproxSide { get; set; }
        public string ApproxMide { get; set; }
        public string ApproxBottom { get; set; }
    }
}
