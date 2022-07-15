#region Using
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks; 
#endregion

namespace BeyCons.Core.RevitUtils.DataUtils.Models
{
    public class ParameterInformation
    {
        public string Name { get; set; }
        public bool ReadOnly { get; set; }
        public string Description { get; set; }
        public bool Visible { get; set; } = true;
    }
}