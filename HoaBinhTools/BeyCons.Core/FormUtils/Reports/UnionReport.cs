#region Using
using Autodesk.Revit.DB;
using BeyCons.Core.RevitUtils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks; 
#endregion

namespace BeyCons.Core.FormUtils.Reports
{
    public class UnionReport
    {
        public int Index { get; set; }
        public List<ElementId> ElementIds { get; set; }
        public string CategoryAndType
        {
            get
            {
                return GetCategoryAndtype(ElementIds);
            }
        }
        public UnionReport(ElementId elementIdOne, ElementId elementIdTwo)
        {
            ElementIds = new List<ElementId>() { elementIdOne, elementIdTwo };
        }
        public string GetCategoryAndtype(List<ElementId> elementIds)
        {
            Element elementOne = RevitData.Instance.Document.GetElement(elementIds.First());
            Element elementTwo = RevitData.Instance.Document.GetElement(elementIds.Last());
            return $"[{elementOne?.Id.IntegerValue} - {elementOne.Category?.Name} - {elementOne?.Name}]-[{elementTwo?.Id.IntegerValue} - {elementTwo.Category?.Name} - {elementTwo?.Name}]";
        }
    }
}