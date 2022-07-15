#region Using
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks; 
#endregion

namespace BeyCons.Core.Libraries.Geometries.Enums
{
    /// <summary>
    /// Enum dùng để kiểm tra vị trí tương đối giữa đường thẳng và mặt phẳng
    /// </summary>
    public enum BetweenArcAndArc
    {
        None,
        Parallel,
        OnCircleOverlap,
        OnCircleNoOverlap,
        Intersecting,
    }
}