#region Using
using Autodesk.Revit.UI;
using BeyCons.Core.Extensions;
using BeyCons.Core.RevitUtils;
using SoftwareKeyManager;
using System.IO;
#endregion

namespace BeyCons.Core
{
    public static class KeyHBC
    {
        public static bool Debug { get; set; } = false;
        public static Result GetKey()
        {
			//string assemblyFilePath = System.Reflection.Assembly.GetExecutingAssembly().Location;
			//string assemplyDirPath = Path.GetDirectoryName(assemblyFilePath);
			//MainForm validate = new MainForm { RegKeyPath = assemplyDirPath };
			//if (validate.checkFromRegFile() != MainForm.StatusKey.Matched)
			//{
			//	//MainForm regForm = new MainForm();
			//	//regForm.ShowDialog();
			//	return Result.Succeeded;
			//}
			//else
			//{
			//	return Result.Cancelled;
			//}
			return Result.Cancelled;
        }
    }
}