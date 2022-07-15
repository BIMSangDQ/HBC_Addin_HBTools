#region Using
using Autodesk.Revit.DB;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks; 
#endregion

namespace HoaBinhTools.BIMQAQC.ModelChecker.Models
{
    public class DocumentAndElements
    {
        public Document Document { get; set; }
        public List<Element> Elements { get; set; }
    }
}