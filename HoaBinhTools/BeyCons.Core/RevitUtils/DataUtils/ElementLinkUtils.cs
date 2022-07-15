#region Using
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using BeyCons.Core.RevitUtils.DataUtils.Models;
using System.Collections.Generic;
using System.Linq;
#endregion

namespace BeyCons.Core.RevitUtils.DataUtils
{
    public static class ElementLinkUtils
    {

        #region Field
        private static List<ElementLink> instances;
        private static List<ElementLink> types;
        private static List<RevitLinkInstance> revitLinkInstances;
        private static List<ElementLink> importCADInstances;
        private static List<ElementLink> linkCADInstances;
        private static List<FailureLink> failureLinks;
        #endregion

        #region Properties
        private static List<ElementLink> Instances
        {
            get
            {
                if (instances == null)
                {
                    instances = new List<ElementLink>();
                }
                return instances;
            }
            set
            {
                instances = value;
            }
        }
        private static List<ElementLink> Types
        {
            get
            {
                if (types == null)
                {
                    types = new List<ElementLink>();
                }
                return types;
            }
            set
            {
                types = value;
            }
        }
        private static List<RevitLinkInstance> RevitLinkInstances
        {
            get
            {
                if (revitLinkInstances == null)
                {
                    revitLinkInstances = new List<RevitLinkInstance>();
                }
                return revitLinkInstances;
            }
            set
            {
                revitLinkInstances = value;
            }
        }
        private static List<ElementLink> ImportCADInstances
        {
            get
            {
                if (importCADInstances == null)
                {
                    importCADInstances = new List<ElementLink>();
                }
                return importCADInstances;
            }
            set
            {
                importCADInstances = value;
            }
        }
        private static List<ElementLink> LinkCADInstances
        {
            get
            {
                if (linkCADInstances == null)
                {
                    linkCADInstances = new List<ElementLink>();
                }
                return linkCADInstances;
            }
            set
            {
                linkCADInstances = value;
            }
        }
        private static List<FailureLink> FailureLinks
        {
            get
            {
                if (failureLinks == null)
                {
                    failureLinks = new List<FailureLink>();
                }
                return failureLinks;
            }
            set
            {
                failureLinks = value;
            }
        }
        #endregion

        public static List<ElementLink> GetInstances(UIApplication uIApplication, bool recursive)
        {
            Instances = new List<ElementLink>();
            IEnumerable<RevitLinkInstance> revitLinkInstances = new FilteredElementCollector(uIApplication.ActiveUIDocument.Document).OfCategory(BuiltInCategory.OST_RvtLinks).WhereElementIsNotElementType().Cast<RevitLinkInstance>();
            if (revitLinkInstances.Count() > 0)
            {
                RecursiveInstances(revitLinkInstances, recursive);
            }
            return Instances;
        }
        private static void RecursiveInstances(IEnumerable<RevitLinkInstance> revitLinkInstances, bool recursive)
        {
            foreach (RevitLinkInstance revitLinkInstance in revitLinkInstances)
            {
                Document documentLink = revitLinkInstance.GetLinkDocument();
                if (documentLink != null)
                {
                    List<Element> instanceElements = new FilteredElementCollector(documentLink).WhereElementIsNotElementType().ToElements().ToList();
                    if (instanceElements.Count > 0)
                    {
                        Instances.Add(new ElementLink() { Document = documentLink, Elements = instanceElements });
                    }
                    if (recursive)
                    {
                        IEnumerable<RevitLinkInstance> subRevitLinkInstances = new FilteredElementCollector(documentLink).OfCategory(BuiltInCategory.OST_RvtLinks).WhereElementIsNotElementType().Cast<RevitLinkInstance>();
                        if (subRevitLinkInstances.Count() > 0)
                        {
                            RecursiveInstances(subRevitLinkInstances, recursive);
                        }
                    }
                }
            }
        }
        public static List<ElementLink> GetTypes(UIApplication uIApplication, bool recursive)
        {
            Types = new List<ElementLink>();
            IEnumerable<RevitLinkInstance> revitLinkInstances = new FilteredElementCollector(uIApplication.ActiveUIDocument.Document).OfCategory(BuiltInCategory.OST_RvtLinks).WhereElementIsNotElementType().Cast<RevitLinkInstance>();
            if (revitLinkInstances.Count() > 0)
            {
                RecursiveTypes(revitLinkInstances, recursive);
            }
            return Types;
        }
        private static void RecursiveTypes(IEnumerable<RevitLinkInstance> revitLinkInstances, bool recursive)
        {
            foreach (RevitLinkInstance revitLinkInstance in revitLinkInstances)
            {
                Document documentLink = revitLinkInstance.GetLinkDocument();
                if (documentLink != null)
                {
                    List<Element> typeElements = new FilteredElementCollector(documentLink).WhereElementIsElementType().ToElements().ToList();
                    if (typeElements.Count > 0)
                    {
                        Types.Add(new ElementLink() { Document = documentLink, Elements = typeElements });
                    }
                    if (recursive)
                    {
                        IEnumerable<RevitLinkInstance> subRevitLinkInstances = new FilteredElementCollector(documentLink).OfCategory(BuiltInCategory.OST_RvtLinks).WhereElementIsNotElementType().Cast<RevitLinkInstance>();
                        if (subRevitLinkInstances.Count() > 0)
                        {
                            RecursiveTypes(subRevitLinkInstances, recursive);
                        }
                    }
                }
            }
        }
        public static List<RevitLinkInstance> GetRevitLinkInstances(UIApplication uIApplication, bool recursive)
        {
            RevitLinkInstances = new List<RevitLinkInstance>();
            IEnumerable<RevitLinkInstance> revitLinkInstances = new FilteredElementCollector(uIApplication.ActiveUIDocument.Document).OfCategory(BuiltInCategory.OST_RvtLinks).WhereElementIsNotElementType().Cast<RevitLinkInstance>();
            if (revitLinkInstances.Count() > 0)
            {
                RecursiveRevitLinkInstance(revitLinkInstances, recursive);
            }
            return RevitLinkInstances;
        }
        private static void RecursiveRevitLinkInstance(IEnumerable<RevitLinkInstance> revitLinkInstances, bool recursive)
        {
            foreach (RevitLinkInstance revitLinkInstance in revitLinkInstances)
            {
                Document documentLink = revitLinkInstance.GetLinkDocument();
                if (documentLink != null)
                {
                    IEnumerable<RevitLinkInstance> subRevitLinkInstances = new FilteredElementCollector(documentLink).OfCategory(BuiltInCategory.OST_RvtLinks).WhereElementIsNotElementType().Cast<RevitLinkInstance>();
                    if (subRevitLinkInstances.Count() > 0)
                    {
                        RevitLinkInstances.AddRange(subRevitLinkInstances);
                        if (recursive)
                        {
                            RecursiveRevitLinkInstance(subRevitLinkInstances, recursive);
                        }
                    }
                }
            }
        }
        public static List<ElementLink> GetImportCADInstances(UIApplication uIApplication, bool recursive)
        {
            ImportCADInstances = new List<ElementLink>();
            IEnumerable<RevitLinkInstance> revitLinkInstances = new FilteredElementCollector(uIApplication.ActiveUIDocument.Document).OfCategory(BuiltInCategory.OST_RvtLinks).WhereElementIsNotElementType().Cast<RevitLinkInstance>();
            if (revitLinkInstances.Count() > 0)
            {
                RecursiveImportCADInstances(revitLinkInstances, recursive);
            }
            return ImportCADInstances;
        }
        private static void RecursiveImportCADInstances(IEnumerable<RevitLinkInstance> revitLinkInstances, bool recursive)
        {
            foreach (RevitLinkInstance revitLinkInstance in revitLinkInstances)
            {
                Document documentLink = revitLinkInstance.GetLinkDocument();
                if (documentLink != null)
                {
                    IEnumerable<Element> subImportCADInstances = new FilteredElementCollector(documentLink).OfClass(typeof(ImportInstance)).WhereElementIsNotElementType().Cast<ImportInstance>().Where(x => x != null && !x.IsLinked);
                    if (subImportCADInstances.Count() > 0)
                    {
                        ImportCADInstances.Add(new ElementLink() { Document = documentLink, Elements = subImportCADInstances.ToList() });
                    }
                    if (recursive)
                    {
                        IEnumerable<RevitLinkInstance> subRevitLinkInstances = new FilteredElementCollector(documentLink).OfCategory(BuiltInCategory.OST_RvtLinks).WhereElementIsNotElementType().Cast<RevitLinkInstance>();
                        if (subRevitLinkInstances.Count() > 0)
                        {
                            RecursiveImportCADInstances(subRevitLinkInstances, recursive);
                        }
                    }
                }
            }
        }
        public static List<ElementLink> GetLinkCADInstances(UIApplication uIApplication, bool recursive)
        {
            LinkCADInstances = new List<ElementLink>();
            IEnumerable<RevitLinkInstance> revitLinkInstances = new FilteredElementCollector(uIApplication.ActiveUIDocument.Document).OfCategory(BuiltInCategory.OST_RvtLinks).WhereElementIsNotElementType().Cast<RevitLinkInstance>();
            if (revitLinkInstances.Count() > 0)
            {
                RecursiveLinkCADInstances(revitLinkInstances, recursive);
            }
            return LinkCADInstances;
        }
        private static void RecursiveLinkCADInstances(IEnumerable<RevitLinkInstance> revitLinkInstances, bool recursive)
        {
            foreach (RevitLinkInstance revitLinkInstance in revitLinkInstances)
            {
                Document documentLink = revitLinkInstance.GetLinkDocument();
                if (documentLink != null)
                {
                    IEnumerable<Element> subLinkCADInstances = new FilteredElementCollector(documentLink).OfClass(typeof(ImportInstance)).WhereElementIsNotElementType().Cast<ImportInstance>().Where(x => x != null && x.IsLinked);
                    if (subLinkCADInstances.Count() > 0)
                    {
                        LinkCADInstances.Add(new ElementLink() { Document = documentLink, Elements = subLinkCADInstances.ToList() });
                    }
                    if (recursive)
                    {
                        IEnumerable<RevitLinkInstance> subRevitLinkInstances = new FilteredElementCollector(documentLink).OfCategory(BuiltInCategory.OST_RvtLinks).WhereElementIsNotElementType().Cast<RevitLinkInstance>();
                        if (subRevitLinkInstances.Count() > 0)
                        {
                            RecursiveLinkCADInstances(subRevitLinkInstances, recursive);
                        }
                    }
                }
            }
        }
        public static List<FailureLink> GetFailureMessages(UIApplication uIApplication, bool recursive)
        {
            FailureLinks = new List<FailureLink>();
            IEnumerable<RevitLinkInstance> revitLinkInstances = new FilteredElementCollector(uIApplication.ActiveUIDocument.Document).OfCategory(BuiltInCategory.OST_RvtLinks).WhereElementIsNotElementType().Cast<RevitLinkInstance>();
            if (revitLinkInstances.Count() > 0)
            {
                RecursiveFailureMessages(revitLinkInstances, recursive);
            }
            return FailureLinks;
        }
        private static void RecursiveFailureMessages(IEnumerable<RevitLinkInstance> revitLinkInstances, bool recursive)
        {
            foreach (RevitLinkInstance revitLinkInstance in revitLinkInstances)
            {
                Document documentLink = revitLinkInstance.GetLinkDocument();
                if (documentLink != null)
                {
                    IEnumerable<FailureMessage> subFailureMessages = documentLink.GetWarnings();
                    if (subFailureMessages.Count() > 0)
                    {
                        FailureLinks.Add(new FailureLink() { Document = documentLink, FailureMessages = subFailureMessages.ToList() });
                    }
                    if (recursive)
                    {
                        IEnumerable<RevitLinkInstance> subRevitLinkInstances = new FilteredElementCollector(documentLink).OfCategory(BuiltInCategory.OST_RvtLinks).WhereElementIsNotElementType().Cast<RevitLinkInstance>();
                        if (subRevitLinkInstances.Count() > 0)
                        {
                            RecursiveFailureMessages(subRevitLinkInstances, recursive);
                        }
                    }
                }
            }
        }
    }
}