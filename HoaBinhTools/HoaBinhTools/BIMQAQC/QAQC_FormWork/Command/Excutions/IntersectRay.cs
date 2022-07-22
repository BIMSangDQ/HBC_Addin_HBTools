using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autodesk.Revit.DB;
using BeyCons.Core.Libraries.Geometries;
using BeyCons.Core.Libraries.Units;
using BeyCons.Core.RevitUtils;
using HoaBinhTools.BIMQAQC.QAQC_FormWork.ViewModels;

namespace HoaBinhTools.BIMQAQC.QAQC_FormWork.Command.Excutions
{
	public class IntersectRay
	{

        #region Field
        private static LogicalOrFilter logicalOrFilter;
        private static ReferenceIntersector referenceIntersector;
        #endregion

        #region Properties
        private static LogicalOrFilter LogicalOrFilter
        {
            get
            {
                if (null == logicalOrFilter)
                {
                    logicalOrFilter = new LogicalOrFilter(FormworkViewModels.Categories.Select(x => new ElementCategoryFilter((BuiltInCategory)(x.Id.IntegerValue))).Cast<ElementFilter>().ToList());
                }
                return logicalOrFilter;
            }
        }
        private static ReferenceIntersector ReferenceIntersector
        {
            get
            {
                if (null == referenceIntersector)
                {
                    referenceIntersector = new ReferenceIntersector(LogicalOrFilter, FindReferenceTarget.Face, RevitData.Instance.ActiveView as View3D);
                }
                return referenceIntersector;
            }
        }
        #endregion

        #region Method
        public static bool IsInsidePlanarFace(PlanarFace planarFace, Element elementHost, bool isPlanarFaceOriginal)
        {
            XYZ point = planarFace.Evaluate(new UV((planarFace.GetBoundingBox().Min.U + planarFace.GetBoundingBox().Max.U) / 2, (planarFace.GetBoundingBox().Min.V + planarFace.GetBoundingBox().Max.V) / 2));
            XYZ direction = isPlanarFaceOriginal ? planarFace.FaceNormal : planarFace.FaceNormal.Negate();
            IList<ReferenceWithContext> referenceWithContexts = ReferenceIntersector.Find(GeometryLib.TranslatingPoint(point, direction, 5.0.ToFeet()), direction);
            if (referenceWithContexts.Count > 0)
            {
                int result = 0;
                foreach (ReferenceWithContext referenceWithContext in referenceWithContexts)
                {
                    if (RevitData.Instance.Document.GetElement(referenceWithContext.GetReference()).Id == elementHost.Id)
                    {
                        referenceWithContext.Dispose();
                        result++;
                    }
                }
                if (result != 0 && result % 2 == 0)
                {
                    return true;
                }
            }
            return false;
        }
        #endregion

    }
}
