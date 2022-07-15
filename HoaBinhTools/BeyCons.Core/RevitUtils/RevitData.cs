#region Using
using Autodesk.Revit.DB;
using Autodesk.Revit.DB.Structure;
using Autodesk.Revit.UI;
using Autodesk.Revit.UI.Selection;
using BeyCons.Core.Extensions;
using BeyCons.Core.RevitUtils.Events;
using OfficeOpenXml;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
#endregion

namespace BeyCons.Core.RevitUtils
{
    public class RevitData
    {
        public static RevitData Instance { get; set; }

        #region Variable
        private Autodesk.Revit.ApplicationServices.Application application;
        private UIDocument uIDocument;
        private Document document;
        private Selection selection;
        private List<RebarBarType> rebarTypes;
        private Level level;
        private List<Level> levels;
        private ExternalEventHandlerSingle externalEventHandlerSingle;        
        private ExternalEventHandlerReport externalEventHandlerReport;
        private ExternalEventHandlerMultiple externalEventHandlerMultiple;
        private List<RevitLinkInstance> revitLinkInstances;
        private WorksetTable worksetTable;
        private Transaction transaction;
        private SubTransaction subTransaction;
        private TransactionGroup transactionGroup;
        #endregion

        #region Property
        public UIApplication UIApplication { get; set; }
        public string AddInKey { get; set; }
        public Autodesk.Revit.ApplicationServices.Application Application
        {
            get
            {
                if (application == null) application = UIApplication.Application;
                return application;
            }
        }
        public UIDocument UIDocument
        {
            get
            {
                if (uIDocument == null) uIDocument = UIApplication.ActiveUIDocument;
                return uIDocument;
            }
        }
        public Document Document
        {
            get
            {
                if (document == null) document = UIDocument.Document;
                return document;
            }
        }
        public virtual Transaction Transaction
        {
            get
            {
                if (transaction == null)
                {
                    transaction = new Transaction(Document);
                }
                return transaction;
            }
            set
            {
                transaction = value;
            }
        }
        public virtual SubTransaction SubTransaction
        {
            get
            {
                if(subTransaction == null)
                {
                    subTransaction = new SubTransaction(Document);
                }
                return subTransaction;
            }
            set
            {
                subTransaction = value;
            }
        }
        public virtual TransactionGroup TransactionGroup
        {
            get
            {
                if (transactionGroup == null)
                {
                    transactionGroup = new TransactionGroup(Document);
                }
                return transactionGroup;
            }
            set
            {
                transactionGroup = value;
            }
        }
        public WorksetTable WorksetTable
        {
            get
            {
                if (worksetTable == null) worksetTable = Document.GetWorksetTable();
                return worksetTable;
            }
        }
        public View ActiveView
        {
            get
            {
                return UIDocument.ActiveView;
            }
        }
        public Selection Selection
        {
            get
            {
                if (selection == null) selection = UIDocument.Selection;
                return selection;
            }
        }
        public List<Rebar> Rebars
        {
            get
            {
                return new FilteredElementCollector(document).OfClass(typeof(Rebar)).Cast<Rebar>().Where(x => x.DistributionType == DistributionType.Uniform).ToList();
            }
        }
        public List<RebarBarType> RebarTypes
        {
            get
            {
                if (rebarTypes == null) rebarTypes = new FilteredElementCollector(Document).OfClass(typeof(RebarBarType)).Cast<RebarBarType>().ToList();
                return rebarTypes;
            }
        }
        public List<RevitLinkInstance> RevitLinkInstances
        {
            get
            {
                if (revitLinkInstances == null) revitLinkInstances = new FilteredElementCollector(Document).OfClass(typeof(RevitLinkInstance)).Cast<RevitLinkInstance>().ToList();
                return revitLinkInstances;
            }
        }
        public Level Level
        {
            get
            {
                if (level == null) level = Document.ActiveView.GenLevel;
                return level;
            }
        }
        public List<Level> Levels
        {
            get
            {
                if (levels == null) levels = new FilteredElementCollector(Document).OfClass(typeof(Level)).Cast<Level>().OrderBy(x => x.Elevation).ToList();
                return levels;
            }
        }        
        public ExternalEventHandlerSingle ExternalEventHandlerSingle
        {
            get
            {
                if (externalEventHandlerSingle == null) externalEventHandlerSingle = new ExternalEventHandlerSingle();
                return externalEventHandlerSingle;
            }
            set
            {
                externalEventHandlerSingle = value;
            }
        }        
        public ExternalEventHandlerMultiple ExternalEventHandlerMultiple
        {
            get
            {
                if (externalEventHandlerMultiple == null) externalEventHandlerMultiple = new ExternalEventHandlerMultiple();
                return externalEventHandlerMultiple;
            }
            set
            {
                externalEventHandlerMultiple = value;
            }
        }
        public ExternalEvent ExternalEventSingle { get; set; }
        public ExternalEvent ExternalEventMultiple { get; set; }
        public ExternalEvent ExternalEventReport { get; set; }
        public ExternalEventHandlerReport ExternalEventHandlerReport
        {
            get
            {
                if (externalEventHandlerReport == null) externalEventHandlerReport = new ExternalEventHandlerReport();
                return externalEventHandlerReport;
            }
            set
            {
                externalEventHandlerReport = value;
            }
        }  
        public List<GraphicsStyle> GraphicsStyles { get; set; }
        public ExcelPackage ExcelPackage { get; set; }
        #endregion

        #region Method
        public string GetFullPathSaveProject(string fileName)
        {
            string fullPath = string.Empty;
            try
            {
                ModelPath modelPath = Instance.Document.GetWorksharingCentralModelPath();
                if (modelPath != null && modelPath.Empty == false)
                {
                    fullPath = Path.Combine(Path.GetDirectoryName(ModelPathUtils.ConvertModelPathToUserVisiblePath(modelPath)), Folders.RevitTemp, $"{fileName}.json");
                }
                else if (!string.IsNullOrEmpty(Instance.Document.PathName))
                {
                    fullPath = Path.Combine(Path.GetDirectoryName(Instance.Document.PathName), Folders.BackupModel, $"{fileName}.json");
                }
            }
            catch (Autodesk.Revit.Exceptions.InvalidOperationException)
            {
                if (!string.IsNullOrEmpty(Instance.Document.PathName))
                {
                    fullPath = Path.Combine(Path.GetDirectoryName(Instance.Document.PathName), Folders.BackupModel, $"{fileName}.json");
                }
            }
            return fullPath;
        }
        #endregion

    }
}