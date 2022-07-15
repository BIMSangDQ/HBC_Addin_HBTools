using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using HoaBinhTools.BIMQAQC.ModelChecker.Models;
using HoaBinhTools.BIMQAQC.QAQC_Quantity.Models;
using Microsoft.Win32;
using Excel = Microsoft.Office.Interop.Excel;

namespace HoaBinhTools.BIMQAQC.ModelChecker.ViewModels
{
	public class ToExcel
	{
		public static void ExportToExcel(ObservableCollection<FileInfor> ListFileInfors)
		{
			try
			{
				SaveFileDialog saveFileDialog1 = new SaveFileDialog();
				saveFileDialog1.Filter = "Excel|*.xlsx";
				saveFileDialog1.Title = "Save Excel File";
				saveFileDialog1.ShowDialog();

				if (saveFileDialog1.FileName == "")
				{
					return;
				}

				Excel.Application xlApp = new Excel.Application();
				Excel.Workbook xlWorkbook = xlApp.Workbooks.Add();
				Excel._Worksheet xlWorksheet = xlWorkbook.Sheets[1];
				xlWorkbook.Sheets.Add(xlWorksheet, Type.Missing, Type.Missing, Type.Missing);
				xlWorkbook.Sheets.Add(xlWorksheet, Type.Missing, Type.Missing, Type.Missing);
				xlWorksheet = xlWorkbook.Sheets[1];
				xlWorksheet.Name = "Files";
				Excel._Worksheet xlWorksheet2 = xlWorkbook.Sheets[2];
				xlWorksheet2.Name = "Checks";
				Excel._Worksheet xlWorksheet3 = xlWorkbook.Sheets[3];
				xlWorksheet3.Name = "Elements";
				Excel.Range xlRange = xlWorksheet.UsedRange;

				///Nội dung ở đây
				#region Sheet Files
				xlWorksheet.Cells[1, 1] = "Report Date";
				xlWorksheet.Cells[1, 2] = "Revit File";
				xlWorksheet.Cells[1, 3] = "Checkset Title";
				xlWorksheet.Cells[1, 4] = "Pass Count";
				xlWorksheet.Cells[1, 5] = "Fail Count";
				int startRow = 2;
				foreach (FileInfor FI in ListFileInfors)
				{
					xlWorksheet.Cells[startRow, 1] = DateTime.Now.ToString();
					xlWorksheet.Cells[startRow, 2] = FI.FileName;
					xlWorksheet.Cells[startRow, 3] = GetListFile.GetDisciplineCheckName(FI.FileName);
					xlWorksheet.Cells[startRow, 4] = FI.FileCheckResults.Where(x => x.Result).Count().ToString();
					xlWorksheet.Cells[startRow, 5] = FI.FileCheckResults.Where(x => x.Result == false).Count().ToString();
					startRow = startRow + 1;
				}
				#endregion

				#region Sheet Checks
				xlWorksheet2.Cells[1, 1] = "Revit File";
				xlWorksheet2.Cells[1, 2] = "Check ID";
				xlWorksheet2.Cells[1, 3] = "Name";
				xlWorksheet2.Cells[1, 4] = "Description";
				xlWorksheet2.Cells[1, 5] = "Result";
				xlWorksheet2.Cells[1, 6] = "Result Message";
				xlWorksheet2.Cells[1, 7] = "Count";
				startRow = 2;
				foreach (FileInfor FI in ListFileInfors)
				{
					ObservableCollection<FileCheckResult> FileCheckResults = FI.FileCheckResults;
					string Name = FI.FileName;
					foreach (FileCheckResult FileCheckResult in FileCheckResults)
					{
						xlWorksheet2.Cells[startRow, 1] = Name;
						xlWorksheet2.Cells[startRow, 2] = FileCheckResult.Id;
						xlWorksheet2.Cells[startRow, 3] = FileCheckResult.Name;
						xlWorksheet2.Cells[startRow, 4] = FileCheckResult.Description;
						xlWorksheet2.Cells[startRow, 5] = FileCheckResult.Result;
						xlWorksheet2.Cells[startRow, 6] = FileCheckResult.Failure_Message;
						xlWorksheet2.Cells[startRow, 7] = FileCheckResult.Elements.Count.ToString();
						startRow = startRow + 1;
					}
				}
				#endregion

				#region Sheet Elements
				xlWorksheet3.Cells[1, 1] = "Check ID";
				xlWorksheet3.Cells[1, 2] = "Element ID";
				xlWorksheet3.Cells[1, 3] = "Category";
				xlWorksheet3.Cells[1, 4] = "Family";
				xlWorksheet3.Cells[1, 5] = "Type";
				xlWorksheet3.Cells[1, 6] = "Para1";
				xlWorksheet3.Cells[1, 7] = "Para2";
				xlWorksheet3.Cells[1, 8] = "Para3";
				xlWorksheet3.Cells[1, 9] = "Revit File";
				startRow = 2;
				foreach (FileInfor FI in ListFileInfors)
				{
					ObservableCollection<FileCheckResult> FileCheckResults = FI.FileCheckResults;
					string Name = FI.FileName;
					foreach (FileCheckResult FileCheckResult in FileCheckResults)
					{
						int Id = FileCheckResult.Id;
						ObservableCollection<ListElementResult> Elements = FileCheckResult.Elements;
						foreach (ListElementResult Element in Elements)
						{
							xlWorksheet3.Cells[startRow, 1] = Id;
							xlWorksheet3.Cells[startRow, 2] = Element.ElementId;
							xlWorksheet3.Cells[startRow, 3] = Element.Category;
							xlWorksheet3.Cells[startRow, 4] = Element.Name;
							xlWorksheet3.Cells[startRow, 5] = Element.Type;
							xlWorksheet3.Cells[startRow, 6] = Element.Para1!=""?Element.Para1 + " : " + Element.Para1Value:"";
							xlWorksheet3.Cells[startRow, 7] = Element.Para2 != "" ? Element.Para2 + " : " + Element.Para2Value : "";
							xlWorksheet3.Cells[startRow, 8] = Element.Para3 != "" ? Element.Para3 + " : " + Element.Para3Value : "";
							xlWorksheet3.Cells[startRow, 9] = Name;
							startRow = startRow + 1;
						}
					}
				}
				#endregion

				//Kết thúc nội dung

				GC.Collect();
				GC.WaitForPendingFinalizers();
				Marshal.ReleaseComObject(xlRange);
				Marshal.ReleaseComObject(xlWorksheet);
				//close and release
				xlWorkbook.SaveAs(saveFileDialog1.FileName);
				Marshal.ReleaseComObject(xlWorkbook);
				xlApp.Quit();
				Marshal.ReleaseComObject(xlApp);
			}
			catch (Exception e) { }

		}

		public static void ExportToExcelOverLap(ObservableCollection<DetectOverlap> DetectOverlaps)
		{
			try
			{
				SaveFileDialog saveFileDialog1 = new SaveFileDialog();
				saveFileDialog1.Filter = "Excel|*.xlsx";
				saveFileDialog1.Title = "Save Excel File";
				saveFileDialog1.ShowDialog();

				if (saveFileDialog1.FileName == "")
				{
					return;
				}

				Excel.Application xlApp = new Excel.Application();
				Excel.Workbook xlWorkbook = xlApp.Workbooks.Add();
				Excel._Worksheet xlWorksheet = xlWorkbook.Sheets[1];
				xlWorkbook.Sheets.Add(xlWorksheet, Type.Missing, Type.Missing, Type.Missing);
				xlWorkbook.Sheets.Add(xlWorksheet, Type.Missing, Type.Missing, Type.Missing);
				xlWorksheet = xlWorkbook.Sheets[1];
				xlWorksheet.Name = "OverLap";
				
				Excel.Range xlRange = xlWorksheet.UsedRange;

				///Nội dung ở đây
				#region Sheet Files
				xlWorksheet.Cells[1, 1] = "Element Id";
				xlWorksheet.Cells[1, 2] = "Description";
				xlWorksheet.Cells[1, 3] = "Element Id";
				xlWorksheet.Cells[1, 4] = "Description";
				xlWorksheet.Cells[1, 5] = "Overlap Volumne";
				int startRow = 2;
				foreach (DetectOverlap DT in DetectOverlaps)
				{
					xlWorksheet.Cells[startRow, 1] = DT.Element1;
					xlWorksheet.Cells[startRow, 2] = DT.Element1CateGory;
					try
					{
						xlWorksheet.Cells[startRow, 3] = DT.Element2;
						xlWorksheet.Cells[startRow, 4] = DT.Element1CateGory;
					}
					catch { }
					xlWorksheet.Cells[startRow, 5] = DT.OverlapVolume;
					startRow = startRow + 1;
				}
				#endregion

				//Kết thúc nội dung

				GC.Collect();
				GC.WaitForPendingFinalizers();
				Marshal.ReleaseComObject(xlRange);
				Marshal.ReleaseComObject(xlWorksheet);
				//close and release
				xlWorkbook.SaveAs(saveFileDialog1.FileName);
				Marshal.ReleaseComObject(xlWorkbook);
				xlApp.Quit();
				Marshal.ReleaseComObject(xlApp);
			}
			catch (Exception e) { }

		}

		public static void ExportExcelWithAutorun(ObservableCollection<FileInfor> ListFileInfors)
		{
			Excel.Application xlApp = new Excel.Application();
			Excel.Workbook xlWorkbook = xlApp.Workbooks.Add();
			Excel._Worksheet xlWorksheet = xlWorkbook.Sheets[1];
			xlWorkbook.Sheets.Add(xlWorksheet, Type.Missing, Type.Missing, Type.Missing);
			xlWorkbook.Sheets.Add(xlWorksheet, Type.Missing, Type.Missing, Type.Missing);
			xlWorksheet = xlWorkbook.Sheets[1];
			xlWorksheet.Name = "Files";
			Excel._Worksheet xlWorksheet2 = xlWorkbook.Sheets[2];
			xlWorksheet2.Name = "Checks";
			Excel._Worksheet xlWorksheet3 = xlWorkbook.Sheets[3];
			xlWorksheet3.Name = "Elements";
			Excel.Range xlRange = xlWorksheet.UsedRange;

			//Bắt đầu nội dung Sheet File 
			#region Sheet Files
			xlWorksheet.Cells[1, 1] = "Report Date";
			xlWorksheet.Cells[1, 2] = "Revit File";
			xlWorksheet.Cells[1, 3] = "Checkset Title";
			xlWorksheet.Cells[1, 4] = "Pass Count";
			xlWorksheet.Cells[1, 5] = "Fail Count";
			int startRow = 2;
			foreach (FileInfor FI in ListFileInfors)
			{
				xlWorksheet.Cells[startRow, 1] = DateTime.Now.ToString();
				xlWorksheet.Cells[startRow, 2] = FI.FileName;
				xlWorksheet.Cells[startRow, 3] = GetListFile.GetDisciplineCheckName(FI.FileName);
				xlWorksheet.Cells[startRow, 4] = FI.FileCheckResults.Where(x => x.Result).Count().ToString();
				xlWorksheet.Cells[startRow, 5] = FI.FileCheckResults.Where(x => x.Result == false).Count().ToString();
				startRow = startRow + 1;
			}
			#endregion

			#region Sheet Checks
			xlWorksheet2.Cells[1, 1] = "Revit File";
			xlWorksheet2.Cells[1, 2] = "Check ID";
			xlWorksheet2.Cells[1, 3] = "Name";
			xlWorksheet2.Cells[1, 4] = "Description";
			xlWorksheet2.Cells[1, 5] = "Result";
			xlWorksheet2.Cells[1, 6] = "Result Message";
			xlWorksheet2.Cells[1, 7] = "Count";
			startRow = 2;
			foreach (FileInfor FI in ListFileInfors)
			{
				ObservableCollection<FileCheckResult> FileCheckResults = FI.FileCheckResults;
				string Name = FI.FileName;
				foreach (FileCheckResult FileCheckResult in FileCheckResults)
				{
					xlWorksheet2.Cells[startRow, 1] = Name;
					xlWorksheet2.Cells[startRow, 2] = FileCheckResult.Id;
					xlWorksheet2.Cells[startRow, 3] = FileCheckResult.Name;
					xlWorksheet2.Cells[startRow, 4] = FileCheckResult.Description;
					xlWorksheet2.Cells[startRow, 5] = FileCheckResult.Result;
					xlWorksheet2.Cells[startRow, 6] = FileCheckResult.Failure_Message;
					xlWorksheet2.Cells[startRow, 7] = FileCheckResult.Elements.Count.ToString();
					startRow = startRow + 1;
				}
			}
			#endregion

			#region Sheet Elements
			xlWorksheet3.Cells[1, 1] = "Check ID";
			xlWorksheet3.Cells[1, 2] = "Element ID";
			xlWorksheet3.Cells[1, 3] = "Category";
			xlWorksheet3.Cells[1, 4] = "Family";
			xlWorksheet3.Cells[1, 5] = "Type";
			xlWorksheet3.Cells[1, 6] = "Para1";
			xlWorksheet3.Cells[1, 7] = "Para2";
			xlWorksheet3.Cells[1, 8] = "Para3";
			xlWorksheet3.Cells[1, 9] = "Revit File";
			startRow = 2;
			foreach (FileInfor FI in ListFileInfors)
			{
				ObservableCollection<FileCheckResult> FileCheckResults = FI.FileCheckResults;
				string Name = FI.FileName;
				foreach (FileCheckResult FileCheckResult in FileCheckResults)
				{
					int Id = FileCheckResult.Id;
					ObservableCollection<ListElementResult> Elements = FileCheckResult.Elements;
					foreach (ListElementResult Element in Elements)
					{
						xlWorksheet3.Cells[startRow, 1] = Id;
						xlWorksheet3.Cells[startRow, 2] = Element.ElementId;
						xlWorksheet3.Cells[startRow, 3] = Element.Category;
						xlWorksheet3.Cells[startRow, 4] = Element.Name;
						xlWorksheet3.Cells[startRow, 5] = Element.Type;
						xlWorksheet3.Cells[startRow, 6] = Element.Para1 != "" ? Element.Para1 + " : " + Element.Para1Value : "";
						xlWorksheet3.Cells[startRow, 7] = Element.Para2 != "" ? Element.Para2 + " : " + Element.Para2Value : "";
						xlWorksheet3.Cells[startRow, 8] = Element.Para3 != "" ? Element.Para3 + " : " + Element.Para3Value : "";
						xlWorksheet3.Cells[startRow, 9] = Name;
						startRow = startRow + 1;
					}
				}
			}
			#endregion
			//Kết thúc nội dung

			GC.Collect();
			GC.WaitForPendingFinalizers();
			Marshal.ReleaseComObject(xlRange);
			Marshal.ReleaseComObject(xlWorksheet);
			//close and release
			string folder = QAQCSetting.Default.FolderResult;
			DateTime d1 = DateTime.Now;
			
			string path = folder + $@"\QAQC_{d1.Day.ToString()+d1.Month.ToString()+d1.Year.ToString()}.xlsx";
			xlWorkbook.SaveAs(path);
			Marshal.ReleaseComObject(xlWorkbook);
			xlApp.Quit();
			Marshal.ReleaseComObject(xlApp);
		}
	}
}
