using Autodesk.AutoCAD.Interop;
using Autodesk.AutoCAD.Interop.Common;
using System;
using System.Runtime.InteropServices;

namespace UtilsCad
{
	public class AutocadConnection
	{
		private AcadApplication application;
		private AcadDocument activeDocument;
		private AcadModelSpace activeModelSpace;

		public AcadApplication Application
		{

			get { return application; }
			set { }
		}

		public AcadDocument ActiveDocument
		{
			get { return activeDocument; }
			set { }
		}
		public AcadModelSpace ActiveModelspace
		{
			get { return activeModelSpace; }
			set { }
		}
		/**
		 * Creating the live connection with Autocad
		 */
		public AutocadConnection(string progID = "AutoCAD.Application.23.1")
		{
			Object acApp = null;

			try
			{
				acApp =
					(AcadApplication)Marshal.GetActiveObject(progID);
			}
			catch
			{
				try
				{
					Type acType =Type.GetTypeFromProgID(progID);
					acApp =(AcadApplication)Activator.CreateInstance(acType,true);
				}
				catch
				{
					Console.WriteLine("Cannot create object of type \"" +progID + "\"");
				}
			}
			application = (acApp == null) ? null : (AcadApplication)acApp;
			activeDocument = (acApp == null) ? null : application.ActiveDocument;
			activeModelSpace = (acApp == null) ? null : application.ActiveDocument.ModelSpace;
		}
	}
}
