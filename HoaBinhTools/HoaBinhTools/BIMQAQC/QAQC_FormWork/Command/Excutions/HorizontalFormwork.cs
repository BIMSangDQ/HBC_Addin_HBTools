using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autodesk.Revit.DB;
using HoaBinhTools.BIMQAQC.QAQC_FormWork.Models;

namespace HoaBinhTools.BIMQAQC.QAQC_FormWork.Command.Excutions
{
	public class HorizontalFormwork : FormworkUtils
    {
        public static CommonFaceFilter GetFaceFilterCommonFromHorizontalElementSolid(Element horizontalElement, GeometryExcution geometryExcution, Solid horizontalSolid, ParaUtil paraUtil)
        {
            IList<Solid> horizontalSolids = SolidUtils.SplitVolumes(horizontalSolid);
            FaceType facesFormworkHorizontalElement = new FaceType() { OriginFaces = new List<Face>(), RegainFaces = new List<Face>() };
            foreach (Solid solid in horizontalSolids)
            {
                FaceType faceType = geometryExcution.GetAllFaceFormworkElement(horizontalElement, solid);
                if (faceType.OriginFaces.Count > 0)
                {
                    facesFormworkHorizontalElement.OriginFaces.AddRange(faceType.OriginFaces);
                }
                if (faceType.RegainFaces.Count > 0)
                {
                    facesFormworkHorizontalElement.RegainFaces.AddRange(faceType.RegainFaces);
                }
            }
            CommonFaceFilter commonFaceFilter = GetFaceFilterFromFaces(facesFormworkHorizontalElement, paraUtil);
            //FilterTopFaces(ref commonFaceFilter, horizontalElement, paraUtil);
            return commonFaceFilter;
        }
        public static CommonFaceFilter GetFaceHorizontalElement(Element horizontalElement, GeometryExcution geometryExcution, Solid horizontalSolid, IList<Element> intersectElements, ParaUtil paraUtil, ref int numberOfFaceError)
        {
            IList<Solid> horizontalSolids = SolidUtils.SplitVolumes(horizontalSolid);
            FaceType facesFormworkHorizontalElement = new FaceType() { OriginFaces = new List<Face>(), RegainFaces = new List<Face>() };
            foreach (Solid solid in horizontalSolids)
            {
                FaceType faceType = geometryExcution.GetAllFaceFormworkElement(horizontalElement, solid, intersectElements, ref numberOfFaceError);
                if (faceType.OriginFaces.Count > 0)
                {
                    facesFormworkHorizontalElement.OriginFaces.AddRange(faceType.OriginFaces);
                }
                if (faceType.RegainFaces.Count > 0)
                {
                    facesFormworkHorizontalElement.RegainFaces.AddRange(faceType.RegainFaces);
                }
            }
            CommonFaceFilter commonFaceFilter = GetFaceFilterFromFaces(facesFormworkHorizontalElement, paraUtil);
            //FilterTopFaces(ref commonFaceFilter, horizontalElement, paraUtil);
            return commonFaceFilter;
        }
    }
}
