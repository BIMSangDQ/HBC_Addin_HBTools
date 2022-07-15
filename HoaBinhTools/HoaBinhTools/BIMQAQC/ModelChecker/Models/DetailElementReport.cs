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
    public class DetailElementReport
    {
        public int Index { get; set; }
        public string ElementName { get; set; }
        public int Id { get; set; }
        public Document Document { get; set; }
    }
}