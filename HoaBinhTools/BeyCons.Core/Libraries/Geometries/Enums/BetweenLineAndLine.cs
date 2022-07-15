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
    /// Enum dùng để kiểm tra vị trí tương đối giữa 2 đường thẳng
    /// </summary>
    public enum BetweenLineAndLine
    {
        None,
        Intersecting,
        Parallel,
        Overlap,
        CrossEachOther,
        IntersectPerpendicular,
        CrossEachOtherPerpendicular
    }
}