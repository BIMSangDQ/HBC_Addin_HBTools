using Autodesk.Revit.DB;
using System.Collections.Generic;

namespace HoaBinhTools.BIMQAQC.ModelChecker.Models
{
    public struct Informations
    {
        public string TitleDocument;
        public string FileSize;
        public List<DocumentAndElements> Instances;
        public List<DocumentAndElements> Types;
        public List<RevitLinkInstance> RevitLinks;
        public List<DocumentAndElements> ImportsCAD;
        public List<DocumentAndElements> LinksCAD;
        public List<DocumentAndFailures> Warnings;
        public string Status;
    }
}
