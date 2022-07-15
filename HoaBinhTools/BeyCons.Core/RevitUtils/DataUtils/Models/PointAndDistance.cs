#region Using
using Autodesk.Revit.DB;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks; 
#endregion

namespace BeyCons.Core.RevitUtils.DataUtils.Models
{
    public class PointAndDistance
    {
        public XYZ Point { get; set; }
        public double Distance { get; set; }
    }
}