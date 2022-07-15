using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using Autodesk.Revit.DB;
using Model.Entity;
using OfficeOpenXml;
using SingleData;
using Utility;

namespace Property.WPF
{
	public partial class BaseAttachedProperty
	{
		private static RevitModelData revitModelData = RevitModelData.Instance;
		private static RevitData revitData = RevitData.Instance;

		public static readonly DependencyProperty ExportClickedProperty = DependencyProperty.RegisterAttached(
			"ExportClicked", typeof(string), typeof(BaseAttachedProperty), new PropertyMetadata(null, new PropertyChangedCallback(OnExportClickedPropertyChanged)));

		public static string GetExportClickedProperty(DependencyObject obj)
		{
			return (string)obj.GetValue(ExportClickedProperty);
		}
		public static void SetExportClickedProperty(DependencyObject obj, string value)
		{
			obj.SetValue(ExportClickedProperty, value);
		}
		private static void OnExportClickedPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			Button btn = d as Button;
			if (btn == null) return;

			btn.Click -= ExportClicked;
			if ((string)e.NewValue != null)
			{
				btn.Click += ExportClicked;
			}
		}

		private static void ExportClicked(object sender, RoutedEventArgs e)
		{
			var wpfData = WPFData.Instance;
			var revitModelData = RevitModelData.Instance;

			if (!File.Exists(wpfData.TemplatePath))
			{
				MessageBox.Show("The template file is empty or does not exist.");
				return;
			}

			if (wpfData.SavePath == "")
			{
				MessageBox.Show("The save as path is empty or does not exist.");
				return;
			}
			else if (!Directory.Exists(Path.GetDirectoryName(wpfData.SavePath)))
			{
				MessageBox.Show("The save as path is empty or does not exist.");
				return;
			}
			if (wpfData.ExportViewSchedules.Count == 0)
			{
				MessageBox.Show("The exported list is empty.");
				return;
			}

			if (Utility.IOUtil.IsFileInUse(wpfData.SavePath))
			{
				MessageBox.Show("The file: " + wpfData.SavePath + " is already in used by another process. Please close it, and try again.");
				return;
			}

			Export2Excel();
		}

		public static void Export2Excel()
		{
			var wpfData = WPFData.Instance;
			var revitModelData = RevitModelData.Instance;
			var uidoc = revitData.UIDocument;
			var sel = revitData.Selection;

			var titleRow = revitModelData.TitleRow;
			var headerRow = revitModelData.HeaderRow;

			ExcelPackage excelPackage = new ExcelPackage(new FileInfo(wpfData.TemplatePath));
			ExcelWorkbook newWb = excelPackage.Workbook;
			ExcelWorksheet newWs = newWb.Worksheets[1];

			int start_col = 1;
			StringBuilder sb = new StringBuilder();

			for (int k = 0; k < wpfData.ExportViewSchedules.Count; k++)
			{
				newWb.Worksheets.Copy($"Sheet{k + 1}", $"Sheet{k + 2}");
				newWs = revitModelData.CurrentExcelWorksheet = newWb.Worksheets[$"Sheet{k + 1}"];

				bool isDeleteTempVs = false;
				ViewSchedule vs = wpfData.ExportViewSchedules[k];
				ViewSchedule tempVs = null;
				ScheduleDefinition sd = vs.Definition;

				{
					var name = vs.Name;
					if (name.Length > 31)
					{
						name = name.Substring(0, 25) + $"...[{revitModelData.LongNameWorkSheetIndex}]";
						revitModelData.LongNameWorkSheetIndex++;
					}
					newWs.Name = name;
				}

				var scheduleColumns = revitModelData.ScheduleColumns;

				if (sd.CategoryId.IntegerValue == (int)BuiltInCategory.OST_Rebar)
				{
					isDeleteTempVs = true;
					vs = vs.CreateTemporaryScheduleWithElementGuid();
					if (vs == null)
					{
						RevitModelData.Instance.Form.Close();
						return;
					}
					tempVs = vs;
					sd = vs.Definition;
					//return;
				}

				TableData td = vs.GetTableData();
				TableSectionData header = td.GetSectionData(Autodesk.Revit.DB.SectionType.Header);
				TableSectionData tsd = td.GetSectionData(Autodesk.Revit.DB.SectionType.Body);

				List<string> fieldNames = new List<string>();
				List<int> fieldIndexs = new List<int>();
				int shapeImageColIndex = -1;
				List<UnitSymbolType> fieldSymbolTypes = new List<UnitSymbolType>();

				List<ScheduleFieldId> sfIds = sd.GetFieldOrder() as List<ScheduleFieldId>;
				int t = 0;

				foreach (ScheduleFieldId sfId in sfIds)
				{
					ScheduleField sf = sd.GetField(sfId);

					string name = sf.GetName();
					if (sf.IsHidden)
					{
						continue;
					}

					scheduleColumns.Add(new ScheduleColumn
					{
						Name = name,
						Index = t
					});

					if (sf.GetName().Length == 1 && sf.UnitType == UnitType.UT_Reinforcement_Length)
					{
						fieldNames.Add(name);
						fieldIndexs.Add(t);
						FormatOptions fo = sf.GetFormatOptions();

						if (fo.UseDefault)
						{
							fieldSymbolTypes.Add(UnitSymbolType.UST_MM);
						}
						else
						{
							fieldSymbolTypes.Add(fo.UnitSymbol);
						}
					}
					if (sf.GetName() == "Shape Image")
					{
						shapeImageColIndex = t;
					}
					t++;
				}

				int start_row_data = tsd.FirstRowNumber;
				int offset_row = 0;
				int dataRow = headerRow + 1;

				if (header.HideSection == false)
				{
					newWs.Cells[titleRow, 1].Value = vs.Name;
				}
				else
				{
					newWs.Cells[titleRow, 1].Value = vs.GetCellText(SectionType.Body, 0, 0);
					start_row_data = tsd.FirstRowNumber + 1;
					offset_row = 1;
				}

				int specCol = 0;
				int finalCol = tsd.NumberOfColumns;
				int shapeCol = 0;
				for (int i = start_row_data; i < tsd.NumberOfRows; i++)
				{
					#region Get Dimension Parameters
					List<int> fieldValues = new List<int>();
					bool isValidate = true;
					for (int j = tsd.FirstColumnNumber; j < tsd.NumberOfColumns - (isDeleteTempVs ? 1 : 0); j++)
					{
						if (fieldValues.Count == fieldNames.Count) break;

						string temp = vs.GetCellText(SectionType.Body, i, j);
						if (fieldIndexs.Contains(j))
						{
							t = fieldIndexs.IndexOf(j);
							string value = null;
							switch (fieldSymbolTypes[t])
							{
								case UnitSymbolType.UST_MM:
									{
										if (temp.IndexOf(' ') == -1)
										{
											goto L1;
										}
										value = temp.Substring(0, temp.IndexOf(' '));
									}
									break;
								case UnitSymbolType.UST_NONE:
									value = temp;
									break;
							}
							double d = 0;
							if (double.TryParse(value, out d))
							{
								fieldValues.Add((int)Math.Round(d, 0));
							}
							else
							{
								continue;
							}
						}
					}

					if (fieldValues.Count == 0)
					{
						isValidate = false;
					}
					#endregion

					L1:
					int row = 0;
					for (int j = tsd.FirstColumnNumber; j < tsd.NumberOfColumns - (isDeleteTempVs ? 1 : 0); j++)
					{
						row = i + headerRow - offset_row;
						int col = j + start_col;

						var value = vs.GetCellText(SectionType.Body, i, j);
						if (j == shapeImageColIndex)
						{
							if (isValidate)
							{
								string guid = vs.GetCellText(SectionType.Body, i, tsd.NumberOfColumns - 1);

								var lis2t = revitModelData.RebarShapeInfos;
								var rsi = revitModelData.RebarShapeInfos.SingleOrDefault(x => x.GUID == guid);
								if (rsi == null)
								{
									newWs.Cells[row, col].Value = "";
									continue;
								}

								newWs.Row(row).Height = ((double)140) / 185 * rsi.Height;
								if (shapeCol == 0) shapeCol = col;

								if (!rsi.SetValuesBaseOnFields(fieldNames, fieldValues))
								{
									newWs.Cells[row, col].Value = "Thiếu tham biến chiều dài để hiển thị hình dạng thép!";
									continue;
								}

								try
								{
									rsi.CreateBitmap();
								}
								catch (System.ArgumentException)
								{
									newWs.Cells[row, col].Value = "Một số tham biến chiều dài thép không có trong bảng thống kê. Hãy cập nhập đủ các tham biến vào bảng và chạy lại add-in.";
									continue;
								}

								OfficeOpenXml.Drawing.ExcelPicture picture = newWs.Drawings.AddPicture($"RebarShapeImage{i}", rsi.Bitmap);
								picture.SetPosition(row - 1, 0, col - 1, 0);

								specCol = revitModelData.ShapeColumn = col;
							}
							else
							{
								double d = 0;
								if (double.TryParse(value, out d))
								{
									newWs.Cells[row, col].Value = d;
								}
								else
								{
									newWs.Cells[row, col].Value = value;
								}
							}
						}
						else
						{
							double d = 0;
							if (double.TryParse(value, out d))
							{
								newWs.Cells[row, col].Value = d;
							}
							else
							{
								newWs.Cells[row, col].Value = value;
							}
						}
					}
					revitModelData.BodyLastRow = row;
				}

				ScheduleSheetUtil.AddProjectInfo();
				ScheduleSheetUtil.ModifyTitleRows();
				ScheduleSheetUtil.ModifyColumnWidths();
				ScheduleSheetUtil.ModifyHeaderRows();
				ScheduleSheetUtil.ModifyBodyRows();

				if (shapeCol != 0)
				{
					newWs.Column(shapeCol).Width = ((double)110) / 1000 * revitModelData.RebarShapeInfos.Max(x => x.Width);
				}

				try
				{
					revitData.Document.Delete(tempVs.Id);
				}
				catch
				{

				}

				revitModelData.ScheduleColumns = null;
			}

			excelPackage.SaveAs(new FileInfo(wpfData.SavePath));
			MessageBox.Show("Finish Export!");

			ScheduleSheetUtil.OpenExcelFile();
		}
	}
}
