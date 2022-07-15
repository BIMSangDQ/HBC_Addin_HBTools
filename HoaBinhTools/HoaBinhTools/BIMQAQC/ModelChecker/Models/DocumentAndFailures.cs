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
    public class DocumentAndFailures
    {
        public Document Document { get; set; }
        public List<FailureMessage> FailureMessages { get; set; }
    }
}