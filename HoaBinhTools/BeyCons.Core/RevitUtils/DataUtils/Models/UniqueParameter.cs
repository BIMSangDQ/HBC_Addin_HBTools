#region Using
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks; 
#endregion

namespace BeyCons.Core.RevitUtils.DataUtils.Models
{
    public class UniqueParameter
    {
        public ParameterInformation ParameterInformation { get; set; }
        public Guid Guid { get; set; }
    }
}