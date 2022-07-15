#region Using
using Autodesk.Revit.DB;
using BeyCons.Core.Extensions;
using System.Collections.Generic;
using System.Linq;
#endregion

namespace BeyCons.Core.RevitUtils.DataUtils
{
    public class BaseRevitLink
    {
        public List<Element> ElementLinkInstances { get; set; } = new List<Element>();
        public List<Reference> References { get; set; }
        public Dictionary<string, List<Element>> GroupFollowDocument
        {
            get { return ElementLinkInstances.GroupBy(x => x.Document.Title).ToDictionary(k => k.Key, v => v.ToList()); }
        }
        public Dictionary<int, List<Reference>> GroupFollowRevitInstance
        {
            get { return References.GroupBy(x => (RevitData.Instance.Document.GetElement(x) as RevitLinkInstance).Id.IntegerValue).ToDictionary(k => k.Key, v => v.DistinctBy(y => y.LinkedElementId).ToList()); }
        }
        public BaseRevitLink(List<Reference> references)
        {
            References = references;
            GetElementsRevitLink(references);
        }
        private void GetElementsRevitLink(List<Reference> references)
        {
            foreach (Reference reference in references)
            {
                Document document = (RevitData.Instance.Document.GetElement(reference) as RevitLinkInstance).GetLinkDocument();
                ElementLinkInstances.Add(document.GetElement(reference.LinkedElementId));
            }
        }
    }
}