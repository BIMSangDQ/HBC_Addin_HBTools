#region Using
using Autodesk.Revit.DB;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks; 
#endregion

namespace BeyCons.Core.Utils
{
    public class ElementComparer : IEqualityComparer<Element>
    {
        public bool Equals(Element x, Element y)
        {
            if (ReferenceEquals(x, y))
            {
                return true;
            }

            if (x is null || y is null)
            {
                return false;
            }

            return x.Id.IntegerValue == y.Id.IntegerValue;
        }

        public int GetHashCode(Element element)
        {
            if (element is null)
            {
                return 0;
            }

            return element.Id.IntegerValue.GetHashCode();
        }
    }
}