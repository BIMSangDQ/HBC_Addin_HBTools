#region Using
using Autodesk.Revit.DB;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks; 
#endregion

namespace BeyCons.Core.Libraries.Geometries.Models
{
    /// <summary>
    /// Kiểm tra điểm có nằm về 2 phía của đường trên mặt phẳng
    /// </summary>
    public class PointsTwoSideOfLine
    {
        public List<XYZ> FirstPoints { get; set; }
        public List<XYZ> SecondPoints { get; set; }
    }
}