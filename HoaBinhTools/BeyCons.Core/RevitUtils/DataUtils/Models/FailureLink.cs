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
    public class FailureLink
    {
        public Document Document { get; set; }
        public List<FailureMessage> FailureMessages { get; set; }
    }
}