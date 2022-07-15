using System.Diagnostics;
using OfficeOpenXml.Style;
using SingleData;

namespace Utility
{
	public static class ScheduleSheetUtil
	{
		private static RevitData revitData
		{
			get
			{
				return RevitData.Instance;
			}
		}

		private static RevitModelData revitModelData
		{
			get
			{
				return RevitModelData.Instance;
			}
		}
		public static void AddProjectInfo()
		{
			var workSheet = revitModelData.CurrentExcelWorksheet;
			var projectRow = revitModelData.ProjectRow;

			var projectInfo = revitData.ProjectInfo;
			if (projectInfo.Name != null)
			{
				var cell = workSheet.Cells[projectRow, 4];
				cell.Value = projectInfo.Name;

				var style = cell.Style;
				style.Font.SetFromFont(new System.Drawing.Font("Arial", 11));
				style.Font.Bold = true;
			}

			if (projectInfo.Number != null && projectInfo.Number != "Project Number")
			{
				var cell = workSheet.Cells[projectRow, 13];
				cell.Value = projectInfo.Number;

				var style = cell.Style;
				style.Font.SetFromFont(new System.Drawing.Font("Arial", 11));
				style.Font.Bold = true;
			}
		}
		public static void ModifyColumnWidths()
		{
			var workSheet = revitModelData.CurrentExcelWorksheet;
			var scheduleColumns = revitModelData.ScheduleColumns;
			var headerRow = revitModelData.HeaderRow;

			foreach (var scheduleColumn in scheduleColumns)
			{
				if (scheduleColumn.Width != null)
				{
					workSheet.Column(scheduleColumn.Index + 1).Width = scheduleColumn.Width.Value;
				}
			}
			workSheet.Row(headerRow).Height = 51;
		}
		public static void ModifyTitleRows()
		{
			var workSheet = revitModelData.CurrentExcelWorksheet;
			var titleRow = revitModelData.TitleRow;
			var scheduleColumns = revitModelData.ScheduleColumns;

			var cell = workSheet.Cells[titleRow, 1, titleRow, scheduleColumns.Count - 1];
			cell.Merge = true;

			var style = cell.Style;
			style.Font.SetFromFont(new System.Drawing.Font("Arial", 10));
			style.Font.Bold = true;
			style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
			style.VerticalAlignment = ExcelVerticalAlignment.Center;

			var border = style.Border;
			border.Top.Style = ExcelBorderStyle.Thin;
			border.Left.Style = ExcelBorderStyle.Thin;
			border.Right.Style = ExcelBorderStyle.Thin;
			border.Bottom.Style = ExcelBorderStyle.Thin;

			for (int i = 0; i < scheduleColumns.Count - 1; i++)
			{
				cell = workSheet.Cells[titleRow + 1, i + 1];
				border = cell.Style.Border;
				border.Top.Style = ExcelBorderStyle.Thin;
				border.Left.Style = ExcelBorderStyle.Thin;
				border.Right.Style = ExcelBorderStyle.Thin;
				border.Bottom.Style = ExcelBorderStyle.Thin;
			}
		}
		public static void ModifyHeaderRows()
		{
			var workSheet = revitModelData.CurrentExcelWorksheet;
			var headerRow = revitModelData.HeaderRow;
			var shapeColumn = revitModelData.ShapeColumn;
			var scheduleColumns = revitModelData.ScheduleColumns;

			for (int i = 0; i < scheduleColumns.Count - 1; i++)
			{
				var cell = workSheet.Cells[headerRow, i + 1];
				var style = cell.Style;
				style.Font.SetFromFont(new System.Drawing.Font("Arial", 10));
				style.Font.Bold = true;
				style.WrapText = true;
				style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
				style.VerticalAlignment = ExcelVerticalAlignment.Center;

				var border = style.Border;
				border.Top.Style = ExcelBorderStyle.Thin;
				border.Left.Style = ExcelBorderStyle.Thin;
				border.Right.Style = ExcelBorderStyle.Thin;
				border.Bottom.Style = ExcelBorderStyle.Thin;
			}
		}

		public static void ModifyBodyRows()
		{
			var workSheet = revitModelData.CurrentExcelWorksheet;
			var headerRow = revitModelData.HeaderRow;
			var shapeColumn = revitModelData.ShapeColumn;
			var scheduleColumns = revitModelData.ScheduleColumns;
			var bodyLastRow = revitModelData.BodyLastRow;

			for (int i = 0; i < scheduleColumns.Count - 1; i++)
			{
				for (int j = headerRow + 1; j <= bodyLastRow; j++)
				{
					var cell = workSheet.Cells[j, i + 1];
					var style = cell.Style;
					style.Font.SetFromFont(new System.Drawing.Font("Arial", 10));
					if (scheduleColumns[i].Name != "Partition")
					{
						style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
					}
					style.VerticalAlignment = ExcelVerticalAlignment.Center;

					var border = style.Border;
					border.Top.Style = ExcelBorderStyle.Thin;
					border.Left.Style = ExcelBorderStyle.Thin;
					border.Right.Style = ExcelBorderStyle.Thin;
					border.Bottom.Style = ExcelBorderStyle.Thin;
				}
			}
		}

		public static void OpenExcelFile()
		{
			var wpfData = WPFData.Instance;

			if (wpfData.IsOpenExcel.Value)
			{
				Process.Start(wpfData.SavePath);
			}
		}
	}
}
