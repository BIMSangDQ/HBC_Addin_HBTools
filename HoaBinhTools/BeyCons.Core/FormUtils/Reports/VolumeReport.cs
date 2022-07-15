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
    public class VolumeReport
    {
        public int Index { get; set; }
        public List<ElementId> ElementIds { get; set; } = new List<ElementId>();
        public string CategoryAndType
        {
            get
            {
                return GetCategoryAndtype();
            }
        }
        public VolumeReport(ElementId elementId)
        {
            ElementIds = new List<ElementId>() { elementId };
        }
        public string GetCategoryAndtype()
        {
            Element element = RevitData.Instance.Document.GetElement(ElementIds.First());
            return $"[{element?.Id.IntegerValue} - {element.Category?.Name} - {element?.Name}]"; ;
        }
    }
}