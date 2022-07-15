using Autodesk.Revit.DB;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HoaBinhTools.FramingInformation.Models
{
    class MovableRestBeamSort : IComparer<MovableRestBeam>
    {
        public XYZ original { get; set; }
        public MovableRestBeamSort(XYZ original)
        {
            this.original = original;
        }

        int IComparer<MovableRestBeam>.Compare(MovableRestBeam poly1, MovableRestBeam poly2)
        {
            MovableRestBeam Poly1 = poly1 as MovableRestBeam;

            MovableRestBeam Poly2 = poly2 as MovableRestBeam;

            if (GetSmallestDistance(Poly1) < GetSmallestDistance(Poly2))
            {
                return -1;
            }

            else if (GetSmallestDistance(Poly1) == GetSmallestDistance(Poly2))
            {
                return 0;
            }

            else
            {
                return 1;
            }
        } 

        public double GetSmallestDistance(MovableRestBeam Cur )
        {
            var p0 = Cur.Curve.GetEndPoint(0).DistanceTo(original);

            var p1 = Cur.Curve.GetEndPoint(1).DistanceTo(original);

            return p0 < p1 ? p0 : p1;

        }
    }
}
