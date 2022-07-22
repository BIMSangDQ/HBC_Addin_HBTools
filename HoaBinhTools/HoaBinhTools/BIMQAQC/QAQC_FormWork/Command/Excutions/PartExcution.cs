using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autodesk.Revit.DB;
using BeyCons.Core.RevitUtils.DataUtils;
using HoaBinhTools.BIMQAQC.QAQC_FormWork.Models;
using HoaBinhTools.BIMQAQC.QAQC_FormWork.ViewModels;

namespace HoaBinhTools.BIMQAQC.QAQC_FormWork.Command.Excutions
{
	public class PartExcution : HorizontalFormwork
	{
		public static ResultFace GetResultFace(Part elementPart, IList<Element> intersectElements, ParaUtil paraUtils)
		{
			CommonFaceFilter commonFaceFilter = new CommonFaceFilter();
			GeometryExcution geometryExcution = new GeometryExcution();
			int numberOfFaceError = 0;
			Solid partSolid = GeometryUtils.GetSolidsFromInstanceElement(elementPart, FormworkViewModels.Options, true).ToUnionSolid();
			if (null != partSolid)
			{
				if (!paraUtils.IsAccurateExplanation)
				{
					if (null != intersectElements)
					{
						commonFaceFilter = GetFaceHorizontalElement(elementPart, geometryExcution, partSolid, intersectElements, paraUtils, ref numberOfFaceError);
					}
					else
					{
						commonFaceFilter = GetFaceFilterCommonFromHorizontalElementSolid(elementPart, geometryExcution, partSolid, paraUtils);
					}
				}
				else
				{
					commonFaceFilter = GetFaceFilterCommonFromHorizontalElementSolid(elementPart, geometryExcution, partSolid, paraUtils);
				}
			}
			else
			{
				//geometryExcution.DataReports.Add(new DataReport(elementPart, null, "Unknow", $"Element has more than one solid") { Index = FormworkViewModels.Index });
			}
			return new ResultFace()
			{
				CommonFaceFilter = commonFaceFilter,
				//DataReports = geometryExcution.DataReports,
				FillFaces = new List<Face>(),
				//NumberOfFaceError = numberOfFaceError,
				ApproxSide = string.Empty,
				ApproxBottom = string.Empty
			};
		}
	}
}
