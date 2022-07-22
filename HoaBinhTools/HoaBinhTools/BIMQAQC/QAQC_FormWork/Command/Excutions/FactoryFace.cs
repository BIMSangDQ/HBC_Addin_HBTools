using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autodesk.Revit.DB;
using HoaBinhTools.BIMQAQC.QAQC_FormWork.Models;

namespace HoaBinhTools.BIMQAQC.QAQC_FormWork.Command.Excutions
{
	public class FactoryFace
	{
		public static ResultFace GetFaceFilter(Element elementHost, List<Element> elementsInSelection, ParaUtil paraUtil)
		{
			switch (elementHost.Category.Name)
			{
				case "Structural Columns":
					if (paraUtil.SelectInclusion == "Selection")
					{
						return ColumnExcution.GetResultFace(elementHost as FamilyInstance, IntersectElement.GetElemenstIntersectWithHostElement(elementHost, elementsInSelection), paraUtil);
					}
					else
					{
						return ColumnExcution.GetResultFace(elementHost as FamilyInstance, IntersectElement.GetElemenstIntersectWithHostElement(elementHost, paraUtil.IsActiveView), paraUtil);
					}
				//case "Walls":

				//case "Structural Framing":

				//case "Floors":

				//case "Slab Edges":

				//case "Structural Foundations":

				//case "Generic Models":

				//case "Stairs":

				default:
					if (paraUtil.SelectInclusion == "Selection")
					{
						return PartExcution.GetResultFace(elementHost as Part, IntersectElement.GetElemenstIntersectWithHostElement(elementHost, elementsInSelection), paraUtil);
					}
					else
					{
						return PartExcution.GetResultFace(elementHost as Part, IntersectElement.GetElemenstIntersectWithHostElement(elementHost, paraUtil.IsActiveView), paraUtil);
					}

			}
		}
	}
}
