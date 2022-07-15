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
    public class JoinReport
    {
        public int Index { get; set; }
        public string DescriptionText { get; set; }
        public List<ElementId> ElementIds { get; set; } = new List<ElementId>();
        public string CategoryAndType
        {
            get
            {
                return GetCategoryAndtype();
            }
        }
        public JoinReport(List<ElementId> elementIds)
        {
            ElementIds = new List<ElementId>(elementIds);
        }
        public string GetCategoryAndtype()
        {
            if (ElementIds.Count > 0)
            {
                string categoryAndType = "";
                foreach (ElementId elementId in ElementIds)
                {
                    Element element = RevitData.Instance.Document.GetElement(elementId);
                    categoryAndType += $"[{elementId.IntegerValue} - {element.Category?.Name} - {element?.Name}]";
                }
                return categoryAndType;
            }
            else
            {
                return "None";
            }
        }
    }
}