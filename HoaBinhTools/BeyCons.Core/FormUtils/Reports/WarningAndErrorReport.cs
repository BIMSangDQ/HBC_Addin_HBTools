using Autodesk.Revit.DB;
using BeyCons.Core.RevitUtils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BeyCons.Core.FormUtils.Reports
{
    public class WarningAndErrorReport
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
        public WarningAndErrorReport(ICollection<ElementId> elementIds, string descriptionText)
        {
            ElementIds = new List<ElementId>(elementIds);
            DescriptionText = descriptionText;
        }
        public string GetCategoryAndtype()
        {
            if (ElementIds.Count > 0)
            {
                List<string> categoryAndTypes = new List<string>();
                foreach (ElementId elementId in ElementIds)
                {
                    Element element = RevitData.Instance.Document.GetElement(elementId);
                    categoryAndTypes.Add($"[{elementId.IntegerValue} - {element.Category?.Name} - {element?.Name}]");
                }
                return string.Join("-", categoryAndTypes);
            }
            else
            {
                return "None";
            }
        }
    }
}