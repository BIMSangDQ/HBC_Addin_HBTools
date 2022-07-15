using System;
using System.Text;
using Utility;

namespace Constant
{
	public static class ConstantValue
	{
		public static string AddinManagerDb { get; set; } = "AddinManager";
		public static string DEFAULT_NAMESPACE { get; set; } = "Schedule2Excel2k16";

		public static string IMAGES_FOLDER { get; set; } = "images";

		public static string EXCEL_TPL_FOLDER { get; set; } = "excel_tpl";

		public static string DEFAULT_EXCEL_TPL_FILENAME { get; set; } = "template.xlsx";

		public static string DEFAULT_EXCEL_EXPORT_FILENAME { get; set; } = "export_schedule.xlsx";

		public static string EXCEL_EXT { get; set; } = ".xlsx";

		public static int DEFAULT_TITLE_ROW { get; set; } = 5;

		public static int DEFAULT_HEADER_ROW { get; set; } = 7;

		public static int DEFAULT_DATA_ROW { get; set; } = 8;

		public static string SHARED_PARAM_FILE_EXT { get; set; } = ".txt";

		public static string SHARED_PARAM_FILE_NAME { get; set; } = "BIMScheduleSharedParameter";

		public static string ELEMENT_UNIQUE_ID_PARAM_NAME { get; set; } = "Element_Unique_Id";

		public static string DATA_FOLDER { get; set; } = "data";
		public static string DATA_FILE_NAME { get; set; } = "options.json";

		public static double BitmapTextHeightMM { get; set; } = 10;
		public static double BitmapOffsetValueMM { get; set; } = 25;
		public static double BitmapOffsetValueFeet
		{
			get { return BitmapOffsetValueMM.milimeter2Feet(); }
		}
		public static double BitmapMaxControlLengthMM { get; set; } = 90;
		public static double BitmapMaxControlLengthFeet
		{
			get { return BitmapMaxControlLengthMM.milimeter2Feet(); }
		}

		public static double BitmapMaxControlRadiusMM { get; set; } = 20;
		public static double BitmapMaxControlRadiusFeet
		{
			get { return BitmapMaxControlRadiusMM.milimeter2Feet(); }
		}

		public static double BitMapMaxWidthMM
		{
			get { return BitmapOffsetValueMM * 2 + BitmapMaxControlLengthMM * 2; }
		}

		public static StringBuilder Output { get; set; } = new StringBuilder();

		public static string RebarShapePath { get; set; } =
			System.IO.Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Desktop),
				"RebarShape.png");

		public static string AddinName { get; set; } = "RebarShapeSchedule2Excel";
	}
}
