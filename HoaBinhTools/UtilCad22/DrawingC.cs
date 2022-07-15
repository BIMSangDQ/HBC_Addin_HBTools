using System;
using Autodesk.AutoCAD.Runtime;
using Autodesk.AutoCAD.ApplicationServices;
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.Geometry;
using Autodesk.AutoCAD.EditorInput;
using Autodesk.AutoCAD.Interop;
using Autodesk.AutoCAD.Interop.Common;

namespace UtilCad22
{
    public class DrawingC
    {
	    public void veLine()
	    {
		    AutocadConnection acad = new AutocadConnection();
		    acad.ActiveDocument.PostCommand("line ");
	    }
    }
}
