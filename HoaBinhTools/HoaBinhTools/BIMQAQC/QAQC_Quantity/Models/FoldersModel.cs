using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HoaBinhTools.BIMQAQC.QAQC_Quantity.Models
{
	public class Folders
	{
		public static string Setting { get; set; } = "Settings";
		public static string Picture { get; set; } = "Pictures";
		public static string RevitTemp { get; set; } = "Revit_temp";
		public static string BackupModel { get; set; } = "Backup Model";
		public static string Family { get; set; } = "Families";
		public static string Excel { get; set; } = "Excels";
	}

	public class SaveModel
	{
		public string GuiIdDocument { get; set; }
		public string GuiIDFamilySymbol { get; set; }
		public List<string> GuiIdElements { get; set; }
	}
}
