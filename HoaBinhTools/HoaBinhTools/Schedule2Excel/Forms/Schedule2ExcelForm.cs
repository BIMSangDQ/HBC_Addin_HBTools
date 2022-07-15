using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Windows.Forms;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using Excel = Microsoft.Office.Interop.Excel;

namespace Schedule2Excel2k16.Forms
{
	public partial class Schedule2ExcelForm : System.Windows.Forms.Form
	{
		private const String TAG = "Schedule2Excel";

		private string CCinGGDriver = "";

		private Excel.Application xlApp;

		private List<HolderScheduleItem> mScheduleList;
		public List<HolderScheduleItem> ScheduleList
		{
			get { return mScheduleList; }
			set { mScheduleList = value; }
		}
		private BindingSource bsSchedule;


		private List<HolderScheduleItem> mExportList;
		public List<HolderScheduleItem> ExportList
		{
			get { return mExportList; }
			set { mExportList = value; }
		}

		private BindingSource bsExport;


		private Autodesk.Revit.ApplicationServices.Application mApp;

		public Autodesk.Revit.ApplicationServices.Application App
		{
			set { mApp = value; }
		}
		private Document mDoc;

		public Document Doc
		{
			set { mDoc = value; }
		}

		ToolTip ttTemplateFile;
		ToolTip ttSavePath;

		//bool checkAllScheduleRunning;
		//bool checkAllExportRunning;

		String mAssemblyFilePath = "";
		String mAssemplyDirPath = "";

		int mTitleRow = 0;
		int mHeaderRow = 0;
		int mDataRow = 0;

		public Schedule2ExcelForm()
		{
			InitializeComponent();

			this.mScheduleList = new List<HolderScheduleItem>();
			this.mExportList = new List<HolderScheduleItem>();

			this.ttTemplateFile = new ToolTip();
			this.ttSavePath = new ToolTip();

			this.mAssemblyFilePath = System.Reflection.Assembly.GetExecutingAssembly().Location;
			this.mAssemplyDirPath = System.IO.Path.GetDirectoryName(this.mAssemblyFilePath);
			this.cbIsMap.Checked = true;
			this.ControlBox = false;
		}

		private void Schedule2ExcelForm_Load(object sender, EventArgs e)
		{
			this.initListBox();
			String desktopPath = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
			this.txtSavePath.Text = System.IO.Path.Combine(desktopPath, GlobalVar.DEFAULT_EXCEL_EXPORT_FILENAME);

			this.nudStartLine.Enabled = this.chkFromLine.Checked;

			this.lblPreviewColor.ForeColor = System.Drawing.Color.Black;
			this.lblPreviewColor.BackColor = System.Drawing.Color.FromArgb(0, 112, 192);

			this.myForeColorDialog.Color = this.lblPreviewColor.ForeColor;
			this.myBackColorDialog.Color = this.lblPreviewColor.BackColor;

			if (chkUseDefaultTpl.Checked)
			{
				this.txtTemplateFile.Text = System.IO.Path.Combine(this.mAssemplyDirPath, GlobalVar.EXCEL_TPL_FOLDER, GlobalVar.DEFAULT_EXCEL_TPL_FILENAME);

				this.chkFromLine.Checked = false;
				this.chkFromLine.Enabled = false;
				this.nudStartLine.Enabled = false;

				this.grbTitleOption.Enabled = false;
			}

			String assemblyFilePath = System.Reflection.Assembly.GetExecutingAssembly().Location;
			String assemplyDirPath = System.IO.Path.GetDirectoryName(assemblyFilePath);
			String dataFilePath = Path.Combine(assemplyDirPath, GlobalVar.DATA_FOLDER, GlobalVar.DATA_FILE_NAME);
			Dictionary<String, String> data = Util.JsonConfigParser.readJson(dataFilePath);

			if (data["show_warning"] == "on")
			{
				this.chkShowWarning.Checked = true;
			}
			else
			{
				this.chkShowWarning.Checked = false;
			}

			string fname = "";
			try
			{
				var modelPath = mDoc.GetWorksharingCentralModelPath();

				var centralServerPath = ModelPathUtils.ConvertModelPathToUserVisiblePath(modelPath);

				fname = centralServerPath.ToString();
			}
			catch
			{
				fname = mDoc.PathName;
			}

			CCinGGDriver = PermissionOfSchedule(fname).Replace("\0", "");

		}

		private void initListBox()
		{
			this.bsSchedule = new BindingSource();
			this.bsSchedule.DataSource = this.mScheduleList;
			this.lbScheduleList.DataSource = this.bsSchedule;
			this.lbScheduleList.DisplayMember = "ScheduleName";
			this.lbScheduleList.ValueMember = "Id";


			this.bsExport = new BindingSource();
			this.bsExport.DataSource = this.mExportList;
			this.lbExport2ExcelList.DataSource = this.bsExport;
			this.lbExport2ExcelList.DisplayMember = "ScheduleName";
			this.lbExport2ExcelList.ValueMember = "Id";
		}

		private void btnAdd_Click(object sender, EventArgs e)
		{
			this.addLeftToRight();
		}

		private void btnRemove_Click(object sender, EventArgs e)
		{
			this.addRightToLeft();
		}

		private void btnExport_Click(object sender, EventArgs e)
		{
			if (!System.IO.File.Exists(this.txtTemplateFile.Text))
			{
				TaskDialog.Show(TAG, "The template file is empty or does not exist.");
				return;
			}

			if (this.txtSavePath.Text == "")
			{
				TaskDialog.Show(TAG, "The save as path is empty or does not exist.");
				return;
			}
			else if (!System.IO.Directory.Exists(System.IO.Path.GetDirectoryName(this.txtSavePath.Text)))
			{
				TaskDialog.Show(TAG, "The save as path is empty or does not exist.");
				return;
			}

			if (this.lbExport2ExcelList.SelectedIndices.Count == 0)
			{
				TaskDialog.Show(TAG, "The exported list is empty.");
				return;
			}

			DialogResult dr = MessageBox.Show(this, "Are you ready?", "Confirm Dialog", MessageBoxButtons.YesNo);
			if (dr == System.Windows.Forms.DialogResult.No)
				return;

			if (this.isFileInUse(new System.IO.FileInfo(this.txtSavePath.Text)))
			{
				MessageBox.Show("The File: " + this.txtSavePath.Text + " is already in used by another process. Please close it, and try again.");
				return;
			}


			this.btnClose.Enabled = false;
			this.grbFileOption.Enabled = false;
			this.grbExportOption.Enabled = false;
			this.grbSchedule2Excel.Enabled = false;
			this.grbTitleOption.Enabled = false;

			this.rtbLog.Text = "";
			this.rtbLog.Focus();
			this.pgbExport.Value = 0;

			try
			{
				this.export2Excel();
			}
			catch (Exception ex)
			{
				MessageBox.Show(ex.ToString());

				if (this.xlApp != null)
				{
					this.xlApp.Quit();
					this.releaseObject(this.xlApp);
				}
			}
			finally
			{
				this.btnClose.Enabled = true;
				this.grbFileOption.Enabled = true;
				this.grbExportOption.Enabled = true;
				this.grbSchedule2Excel.Enabled = true;
				if (!chkUseDefaultTpl.Checked)
					this.grbTitleOption.Enabled = true;

			}

		}

		private void btnSelectFile_Click(object sender, EventArgs e)
		{
			this.myOpenFileDialog.Filter = "Excel Files (*.xlsx)|*.xlsx";
			System.Windows.Forms.DialogResult dr = this.myOpenFileDialog.ShowDialog();
			if (dr == System.Windows.Forms.DialogResult.OK)
			{
				this.txtTemplateFile.Text = this.myOpenFileDialog.FileName;

			}
		}

		private void btnBrowse_Click(object sender, EventArgs e)
		{
			//this.mySaveFileDialog.CheckFileExists = false;
			//this.mySaveFileDialog.CheckPathExists = true;
			this.mySaveFileDialog.Filter = "Excel Files (*.xlsx)|*.xlsx";
			if (this.txtSavePath.Text != "")
				this.mySaveFileDialog.FileName = System.IO.Path.GetFileName(this.txtSavePath.Text);
			else
				this.mySaveFileDialog.FileName = GlobalVar.DEFAULT_EXCEL_EXPORT_FILENAME;
			System.Windows.Forms.DialogResult dr = this.mySaveFileDialog.ShowDialog();
			if (dr == System.Windows.Forms.DialogResult.OK)
			{
				//string savePath = this.mySaveFileDialog.FileName;
				this.txtSavePath.Text = this.mySaveFileDialog.FileName;
			}
		}

		private void lbScheduleList_MouseDoubleClick(object sender, MouseEventArgs e)
		{
			this.addLeftToRight();
		}

		private void lbExport2ExcelList_MouseDoubleClick(object sender, MouseEventArgs e)
		{
			this.addRightToLeft();
		}

		private void chkCheckAllSchedule_Click(object sender, EventArgs e)
		{
			for (int i = 0; i < this.lbScheduleList.Items.Count; i++)
			{
				this.lbScheduleList.SetSelected(i, this.chkCheckAllSchedule.Checked);
			}
		}

		private void chkCheckAllExport_Click(object sender, EventArgs e)
		{
			for (int i = 0; i < this.lbExport2ExcelList.Items.Count; i++)
			{
				this.lbExport2ExcelList.SetSelected(i, this.chkCheckAllExport.Checked);
			}
		}

		/*=======================================================================*/

		private void addRightToLeft()
		{
			int n = this.lbExport2ExcelList.SelectedIndices.Count;
			if (n > 0)
			{
				List<HolderScheduleItem> tmp = new List<HolderScheduleItem>();

				for (int i = 0; i < n; i++)
				{
					int idx = this.lbExport2ExcelList.SelectedIndices[i];
					this.mScheduleList.Add(this.mExportList[idx]);
					tmp.Add(this.mExportList[idx]);
				}

				for (int i = 0; i < tmp.Count; i++)
				{
					this.mExportList.Remove(tmp[i]);
				}

				this.mScheduleList.Sort((lhs, rhs) => lhs.ScheduleName.CompareTo(rhs.ScheduleName));
				this.mExportList.Sort((lhs, rhs) => lhs.ScheduleName.CompareTo(rhs.ScheduleName));

				bsSchedule.ResetBindings(false);
				bsExport.ResetBindings(false);

				if (chkCheckAllExport.Checked)
					chkCheckAllExport.Checked = false;
			}
		}

		private void addLeftToRight()
		{
			int n = this.lbScheduleList.SelectedIndices.Count;
			if (n > 0)
			{
				List<HolderScheduleItem> tmp = new List<HolderScheduleItem>();

				for (int i = 0; i < n; i++)
				{
					int idx = this.lbScheduleList.SelectedIndices[i];
					// Check bảng tk

					HolderScheduleItem item = this.mScheduleList[idx];
					ViewSchedule OriVS = this.mDoc.GetElement(new ElementId(item.Id)) as ViewSchedule;

					ViewSchedule vs = OriVS;

					ElementId elementId = vs.Definition.CategoryId;

					try
					{
						Category categorySchedule = Category.GetCategory(mDoc, elementId);

						if (categorySchedule.Name == "Structural Columns" ||
							categorySchedule.Name == "Structural Framing" ||
							categorySchedule.Name == "Structural Foundations")
						{
							bool IsHaveVolume = false;
							//Check volume
							ScheduleDefinition sd = vs.Definition;
							int Fieldcount = sd.GetFieldCount();
							for (int j = 0; j < Fieldcount; j++)
							{
								ScheduleField field = sd.GetField(j);
								if (field.GetName() == "Volume")
								{
									IsHaveVolume = true;
									break;
								}
							}


							if (IsHaveVolume == true)
							{
								if (CCinGGDriver == "true")
								{
								}
								else
								{
									MessageBox.Show($"Vui lòng chạy addin BIM_QAQC trước nếu muốn export bảng thống kê với Category: {categorySchedule.Name}", "Revit");
									continue;
								}
							}
						}

					}
					catch
					{ }
					//
					this.mExportList.Add(this.mScheduleList[idx]);
					tmp.Add(this.mScheduleList[idx]);
				}

				for (int i = 0; i < tmp.Count; i++)
				{
					this.mScheduleList.Remove(tmp[i]);
				}

				this.mScheduleList.Sort((lhs, rhs) => lhs.ScheduleName.CompareTo(rhs.ScheduleName));
				this.mExportList.Sort((lhs, rhs) => lhs.ScheduleName.CompareTo(rhs.ScheduleName));

				bsSchedule.ResetBindings(false);
				bsExport.ResetBindings(false);

				if (chkCheckAllSchedule.Checked)
					chkCheckAllSchedule.Checked = false;
			}
		}

		private bool isFileInUse(System.IO.FileInfo file)
		{

			bool rs = false;
			System.IO.FileStream fs = null;
			try
			{
				fs = System.IO.File.OpenWrite(file.FullName);
			}
			catch (System.IO.IOException)
			{
				//the file is unavailable because it is:
				//still being written to
				//or being processed by another thread
				//or does not exist (has already been processed)
				rs = true;
			}
			finally
			{
				if (fs != null)
					fs.Close();
			}

			return rs;

		}

		private void export2Excel()
		{
			String assemblyFilePath = System.Reflection.Assembly.GetExecutingAssembly().Location;
			String assemplyDirPath = System.IO.Path.GetDirectoryName(assemblyFilePath);

			this.xlApp = new Excel.Application();

			if (xlApp == null)
			{
				MessageBox.Show("Microsoft Excel can't be found. Please install it before run this add-in.");
				return;
			}

			Excel.Workbook xlWorkBookTpl;
			Excel.Worksheet xlWorkSheetTpl;
			Excel.Workbook xlWorkBook;
			//Excel.Range range;
			object misValue = System.Reflection.Missing.Value;


			xlApp.DisplayAlerts = false;

			xlWorkBookTpl = xlApp.Workbooks.Open(this.txtTemplateFile.Text, 0, false, 5, "", "", true, Microsoft.Office.Interop.Excel.XlPlatform.xlWindows, "\t", false, false, 0, true, 1, 0);

			xlWorkSheetTpl = xlWorkBookTpl.Worksheets[1] as Excel.Worksheet;

			//New excel file for export
			xlWorkBook = xlApp.Workbooks.Add(misValue);
			xlWorkBook.SaveAs(this.txtSavePath.Text);

			List<String> tmpSheetName = new List<String>();

			if (chkUseDefaultTpl.Checked)
			{
				this.mTitleRow = GlobalVar.DEFAULT_TITLE_ROW;
				this.mHeaderRow = GlobalVar.DEFAULT_HEADER_ROW;
				this.mDataRow = this.mHeaderRow + 1;
			}
			else
			{
				this.mTitleRow = Convert.ToInt32(this.nudStartLine.Value);
				this.mHeaderRow = this.mTitleRow + 2;
				this.mDataRow = this.mHeaderRow + 1;
			}

			//int title_row = this.mTitleRow;
			//int start_row = this.mHeaderRow;
			int start_col = 1;

			StringBuilder sb = new StringBuilder();

			for (int k = 0; k < this.mExportList.Count; k++)
			{
				HolderScheduleItem item = this.mExportList[k];
				ViewSchedule OriVS = this.mDoc.GetElement(new ElementId(item.Id)) as ViewSchedule;

				ViewSchedule vs = OriVS;

				//Test lấy category schedule

				ElementId elementId = vs.Definition.CategoryId;

				Category categorySchedule = Category.GetCategory(mDoc, elementId);


				//
				bool isDeleteTmp = false;
				if (chkExportElementId.Checked)
				{
					bool canDuplicate = false;
					ViewSchedule tmpvs = this.createTemporarySchedule(OriVS, GlobalVar.ELEMENT_UNIQUE_ID_PARAM_NAME, ref canDuplicate);
					if (tmpvs != null)
					{
						vs = tmpvs;
						isDeleteTmp = true;
					}
					else
					{
						if (!canDuplicate)
						{
							this.appendTextWithColor(vs.Name + ": ", System.Drawing.Color.Black);
							this.appendTextWithColor("[WARNING]" + Environment.NewLine, System.Drawing.Color.OrangeRed);
							this.appendTextWithColor("Cannot duplicate Schedule to create the Element_Unique_Id parameter" + Environment.NewLine, System.Drawing.Color.OrangeRed);
						}
					}
				}

				TableData td = vs.GetTableData();
				TableSectionData header = td.GetSectionData(SectionType.Header);

				TableSectionData tsd = td.GetSectionData(SectionType.Body);

				String sheetName = "";

				try
				{
					if (tsd.NumberOfColumns > 0)
					{
						xlWorkSheetTpl.Copy(misValue, xlWorkBook.Worksheets[xlWorkBook.Worksheets.Count] as Excel.Worksheet);
						xlWorkBook.Save();

						Excel.Worksheet xlWorkSheetTmp = xlWorkBook.Worksheets[xlWorkBook.Worksheets.Count];// as Excel.Worksheet;

						if (OriVS.Name.Length < 32)
							sheetName = checkSheetNameExist(tmpSheetName, OriVS.Name);
						else
							sheetName = checkSheetNameExist(tmpSheetName, OriVS.Name.Substring(0, 30));

						xlWorkSheetTmp.Name = sheetName;

						tmpSheetName.Add(xlWorkSheetTmp.Name);

						//int title_row = start_row;
						//xlWorkSheetTmp.Cells[title_row, 1] = vs.Name;

						int start_row_data = tsd.FirstRowNumber;
						int offset_row = 0;
						if (header.HideSection == false)
						{
							xlWorkSheetTmp.Cells[this.mTitleRow, 1] = OriVS.Name;
						}
						else
						{
							xlWorkSheetTmp.Cells[this.mTitleRow, 1] = vs.GetCellText(SectionType.Body, 0, 0);
							//xlWorkSheetTmp.Cells[this.mTitleRow, 1] = tsd.GetCellText(0, 0);
							start_row_data = tsd.FirstRowNumber + 1;
							offset_row = 1;
						}



						int specCol = 0;
						int finalCol = tsd.NumberOfColumns;
						for (int i = start_row_data; i < tsd.NumberOfRows; i++)
						{
							for (int j = tsd.FirstColumnNumber; j < tsd.NumberOfColumns; j++)
							{
								//String temp = tsd.GetCellText(i, j);
								String temp = vs.GetCellText(SectionType.Body, i, j);
								if (chkExportElementId.Checked)
								{
									if (i == start_row_data && j == tsd.NumberOfColumns - 1)
									{
										temp = GlobalVar.ELEMENT_UNIQUE_ID_PARAM_NAME;
									}
								}

								int row = i + this.mHeaderRow - offset_row;
								int col = j + start_col;
								xlWorkSheetTmp.Cells[row, col] = temp;
								Excel.Range oRange = xlWorkSheetTmp.Cells[row, col];
								string link = Schedule2ExcelCommand.path + @"\" + temp;
								bool check = cbIsMap.Checked && File.Exists(link);
								bool extenImage = temp.EndsWith(".jpg") || temp.EndsWith(".png") || temp.EndsWith(".jpeg") || temp.EndsWith(".tiff") || temp.EndsWith(".gif") || temp.EndsWith(".bmp");
								if (extenImage && check)
								{
									//Image image = Image.FromFile(link);
									//float width = image.Width;
									//float height = image.Height;
									float Left = (float)((double)oRange.Left) + 1;
									float Top = (float)((double)oRange.Top) + 2;
									xlWorkSheetTmp.Rows[row].RowHeight = 75;
									xlWorkSheetTmp.Columns[col].ColumnWidth = 20;
									xlWorkSheetTmp.Shapes.AddPicture(link, Microsoft.Office.Core.MsoTriState.msoFalse, Microsoft.Office.Core.MsoTriState.msoCTrue, Left, Top, (float)144 / (float)(96.0 / 72.0), (float)96 / (float)(96.0 / 72.0));

									specCol = col;
								}
							}
						}
						if (specCol > 0)
						{
							xlWorkSheetTmp.Columns[specCol].HorizontalAlignment = Excel.XlHAlign.xlHAlignCenter;
						}

						//Title Range
						String cell1 = this.getCellNameByLetter(1, this.mTitleRow);
						String cell2 = this.getCellNameByLetter(tsd.NumberOfColumns, this.mTitleRow);
						Excel.Range rangeTitle = xlWorkSheetTmp.get_Range(cell1, cell2);
						rangeTitle.Font.Bold = true;
						rangeTitle.Merge();
						rangeTitle.HorizontalAlignment = Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignCenter;
						rangeTitle.VerticalAlignment = Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignCenter;
						rangeTitle.Font.Size = this.nudFontSize.Value;
						rangeTitle.Font.Color = System.Drawing.ColorTranslator.ToOle(this.lblPreviewColor.ForeColor);

						if (!chkUseDefaultTpl.Checked)
						{
							rangeTitle.Interior.Color = System.Drawing.ColorTranslator.ToOle(this.lblPreviewColor.BackColor);
							rangeTitle.RowHeight = 40;
						}

						//Header Range
						Excel.Range rangeHeader = xlWorkSheetTmp.get_Range("A" + this.mHeaderRow, "Z" + this.mHeaderRow);
						rangeHeader.Font.Bold = true;
						if (specCol == 0)
						{
							rangeHeader.EntireColumn.AutoFit();
						}
						else if (specCol != 1)
						{
							String C1 = this.getCellNameByLetter(1, this.mHeaderRow);
							String C2 = this.getCellNameByLetter(specCol - 1, this.mHeaderRow);
							Excel.Range range_1 = xlWorkSheetTmp.get_Range(C1, C2);
							range_1.EntireColumn.AutoFit();
							String C3 = this.getCellNameByLetter(specCol + 1, this.mHeaderRow);
							String C4 = this.getCellNameByLetter(finalCol, this.mHeaderRow);
							Excel.Range range_2 = xlWorkSheetTmp.get_Range(C3, C4);
							range_2.EntireColumn.AutoFit();
						}
						else
						{
							String C3 = this.getCellNameByLetter(specCol + 1, this.mHeaderRow);
							String C4 = this.getCellNameByLetter(finalCol, this.mHeaderRow);
							Excel.Range range_2 = xlWorkSheetTmp.get_Range(C3, C4);
							range_2.EntireColumn.AutoFit();
						}

						//Scale All Image to Original Size After The Colunm is AutoFit
						Excel.Pictures pics = xlWorkSheetTmp.Pictures(misValue) as Excel.Pictures;
						//scalePicturesToOriginal(pics);
						this.scalePictureToOriginal(pics, 1);

						//Draw Border
						String cell11 = this.getCellNameByLetter(1, this.mHeaderRow);
						String cell22 = this.getCellNameByLetter(tsd.NumberOfColumns, this.mHeaderRow + tsd.NumberOfRows - 1);
						Excel.Range rangeBorder = xlWorkSheetTmp.get_Range(cell11, cell22);

						rangeBorder.Borders.LineStyle = Excel.XlLineStyle.xlContinuous;
						rangeBorder.Borders.Color = System.Drawing.ColorTranslator.ToOle(System.Drawing.Color.Black);

						//xlWorkSheetTmp.Copy(misValue, xlWorkBook.Worksheets[xlWorkBook.Worksheets.Count] as Excel.Worksheet);
						xlWorkBook.Save();

						this.appendTextWithColor(sheetName + ": ", System.Drawing.Color.Black);
						this.appendTextWithColor("[DONE]" + Environment.NewLine, System.Drawing.Color.Green);
					}
					else
					{
						this.appendTextWithColor(sheetName + ": ", System.Drawing.Color.Black);
						this.appendTextWithColor("[EMPTY DATA]" + Environment.NewLine, System.Drawing.Color.OrangeRed);
					}

					//Update Progress Bar
					int percent = (k + 1) * 100 / this.mExportList.Count;
					this.pgbExport.Value = percent;

					Thread.Sleep(100);

				}
				catch (Exception ex)
				{
					this.appendTextWithColor(vs.Name + ": ", System.Drawing.Color.Black);
					this.appendTextWithColor("[FAIL]" + Environment.NewLine, System.Drawing.Color.Red);
					this.appendTextWithColor(ex.ToString() + Environment.NewLine, System.Drawing.Color.Red);
				}

				//sb.AppendLine("=======================");

				if (isDeleteTmp)
				{
					//Transaction trans = new Transaction(this.mDoc, "DELETE-TEMP-SCHEDULE");
					//trans.Start();
					//this.mDoc.Delete(vs.Id);
					//trans.Commit();
				}
			}

			try
			{
				if (xlWorkBook.Worksheets.Count > 1)
				{
					(xlWorkBook.Worksheets[1] as Excel.Worksheet).Delete();
					xlWorkBook.Save();
				}
			}
			catch (Exception ex1)
			{
				this.appendTextWithColor(ex1.ToString() + Environment.NewLine, System.Drawing.Color.Red);
			}
			finally
			{
				xlWorkBook.Close(true, misValue, misValue);
				xlWorkBookTpl.Close(false, misValue, misValue);
				xlApp.Quit();

				this.releaseObject(xlWorkSheetTpl);
				this.releaseObject(xlWorkBook);
				this.releaseObject(xlWorkBookTpl);
				this.releaseObject(xlApp);
			}
			//xlWorkBook.SaveAs(assemplyDirPath + @"\excel_tpl\file_01.xlsx");

			//TaskDialog.Show(TAG, sb.ToString());

			if (chkExportFolder.Checked)
			{
				String dir = System.IO.Path.GetDirectoryName(this.txtSavePath.Text);
				if (System.IO.Directory.Exists(dir))
					System.Diagnostics.Process.Start("explorer.exe", dir);
			}


			if (chkOpenExportFile.Checked)
			{
				if (System.IO.File.Exists(this.txtSavePath.Text))
					System.Diagnostics.Process.Start(this.txtSavePath.Text);
			}

			//MessageBox.Show(sb.ToString());

		}

		private string checkSheetNameExist(List<string> tmpSheetName, object Name)
		{
			throw new NotImplementedException();
		}

		private ViewSchedule createTemporarySchedule(ViewSchedule pOriSchedule, String pUniqueIdParamName, ref bool pCanDuplicate)
		{
			Transaction trans = new Transaction(this.mDoc, "CREATE-UNIQUE-PARAM-FIELD");
			trans.Start();

			ViewSchedule vs = null;
			pCanDuplicate = true;
			try
			{
				//duplicate the schedule so that the original is not modified
				ElementId tempViewScheduleId = pOriSchedule.Duplicate(ViewDuplicateOption.Duplicate);
				vs = this.mDoc.GetElement(tempViewScheduleId) as ViewSchedule;
			}
			catch (Exception ex)
			{
				trans.RollBack();
				pCanDuplicate = false;
				return null;
			}


			Guid paramGuid = Guid.Empty;

			FilteredElementCollector eleCollector = new FilteredElementCollector(this.mDoc, vs.Id);
			eleCollector.WhereElementIsNotElementType();
			eleCollector.OfCategory((BuiltInCategory)vs.Definition.CategoryId.IntegerValue);

			List<Element> eleList = eleCollector.ToList<Element>();

			if (eleList == null || (eleList != null && eleList.Count == 0))
			{
				trans.RollBack();
				return null;
			}


			Element firstEle = eleList[0];
			Util.ParameterUtil.createOrUpdateCustomParameter(this.mApp, this.mDoc, firstEle, pUniqueIdParamName, "Element Id", ParameterType.Text, BuiltInParameterGroup.PG_IDENTITY_DATA);

			//List<ElementId> listExcludedIds = new List<ElementId>();
			Parameter paramElementId = firstEle.LookupParameter(pUniqueIdParamName);
			if (paramElementId != null)
			{
				paramGuid = paramElementId.GUID;
				//listExcludedIds.Add(paramElementId.Id);
			}

			foreach (Element ele in eleCollector)
			{
				if (BuiltInCategory.OST_Views == (BuiltInCategory)ele.Category.Id.IntegerValue)
				{
					Autodesk.Revit.DB.View tmpView = ele as Autodesk.Revit.DB.View;
					if (tmpView.IsTemplate)
						continue;

					Autodesk.Revit.DB.View tplView = this.mDoc.GetElement(tmpView.ViewTemplateId) as Autodesk.Revit.DB.View;
					if (tplView != null)
					{
						//tplView.SetNonControlledTemplateParameterIds(listExcludedIds);

						ICollection<ElementId> nonIds = tplView.GetNonControlledTemplateParameterIds();
						if (!nonIds.Contains(paramElementId.Id))
						{
							nonIds.Add(paramElementId.Id);
							tplView.SetNonControlledTemplateParameterIds(nonIds);
						}
					}

					Parameter p = tmpView.get_Parameter(paramGuid);

					if (p != null)
					{
						if (!p.IsReadOnly)
						{
							p.Set(ele.Id.IntegerValue.ToString());
							//ele.LookupParameter(pUniqueIdParamName).Set(ele.Id.IntegerValue.ToString());
						}
					}
				}
				else
				{
					Parameter p2 = ele.get_Parameter(paramGuid);
					if (p2 != null)
					{
						if (!p2.IsReadOnly)
						{
							p2.Set(ele.Id.IntegerValue.ToString());
							//ele.LookupParameter(pUniqueIdParamName).Set(ele.Id.IntegerValue.ToString());
						}
					}

				}

			}

			//Create new field to mapping with UniqueId
			SchedulableField schedulableField = null;
			foreach (SchedulableField sf in vs.Definition.GetSchedulableFields())
			{
				if (sf.GetName(this.mDoc).Equals(pUniqueIdParamName))
				{
					schedulableField = sf;
					break;
				}
			}

			if (schedulableField == null)
			{
				TaskDialog.Show(TAG, "Didn't find Element's IdParam as Schedulable Field");
				trans.RollBack();
				return null;
			}

			try
			{
				vs.Definition.AddField(schedulableField);
				vs.RefreshData();

			}
			catch (Exception ex)
			{
				TaskDialog.Show(TAG, ex.ToString());
				trans.RollBack();
				return null;
			}

			trans.Commit();

			return vs;
		}


		private void releaseObject(object obj)
		{
			try
			{
				System.Runtime.InteropServices.Marshal.ReleaseComObject(obj);
				obj = null;
			}
			catch (Exception ex)
			{
				obj = null;
				//MessageBox.Show("Unable to release the Object " + ex.ToString());
			}
			finally
			{
				GC.Collect();
			}
		}

		private String checkSheetNameExist(List<String> pSrc, String pName)
		{
			pName.Replace(':', '_').Replace('/', '_');

			if (pSrc.Count == 0)
				return pName;

			String temp = pName;
			bool b = false;
			int k = 1;
			do
			{
				b = false;
				foreach (String str in pSrc)
				{
					if (str.Equals(temp, StringComparison.OrdinalIgnoreCase))
					{
						temp = (k++) + "_" + pName;
						temp = temp.Substring(0, 31);
						b = true;
						break;
					}
				}
			} while (b);

			return temp;
		}

		private void scalePicturesToOriginal(Excel.Pictures pics)
		{
			for (int i = 1; i <= pics.Count; i++)
			{
				Excel.Picture pic = pics.Item(i) as Excel.Picture;
				pic.ShapeRange.LockAspectRatio = Microsoft.Office.Core.MsoTriState.msoCTrue;
				pic.ShapeRange.ScaleWidth(1, Microsoft.Office.Core.MsoTriState.msoTrue);
				pic.ShapeRange.ScaleHeight(1, Microsoft.Office.Core.MsoTriState.msoTrue);
			}

		}

		private void scalePictureToOriginal(Excel.Pictures pics, int pos)
		{
			if (pics == null || pics.Count == 0 || pos < 1)
				return;

			Excel.Picture pic = pics.Item(pos) as Excel.Picture;
			if (pic == null)
				return;
			pic.ShapeRange.LockAspectRatio = Microsoft.Office.Core.MsoTriState.msoCTrue;
			pic.ShapeRange.ScaleWidth(1, Microsoft.Office.Core.MsoTriState.msoTrue);
			pic.ShapeRange.ScaleHeight(1, Microsoft.Office.Core.MsoTriState.msoTrue);


		}

		private String getCellNameByLetter(int col, int row)
		{
			String[] letters = new String[]{
				"A", "B", "C", "D", "E", "F", "G", "H", "I", "J", "K", "L",
				"M", "N", "O", "P", "Q", "R", "S", "T", "U", "V", "W", "X", "Y", "Z",
				"AA", "AB", "AC", "AD", "AE", "AF", "AG", "AH", "AI", "AJ", "AK", "AL",
				"AM", "AN", "AO", "AP", "AQ", "AR", "AS", "AT", "AU", "AV", "AW", "AX", "AY", "AZ"
			};

			return letters[col - 1] + row;

		}

		private void appendTextWithColor(String text, System.Drawing.Color color)
		{
			this.rtbLog.Select(this.rtbLog.TextLength, 0);
			this.rtbLog.SelectionColor = color;
			this.rtbLog.AppendText(text);
		}

		private void btnForeColor_Click(object sender, EventArgs e)
		{
			DialogResult dr = this.myForeColorDialog.ShowDialog();
			if (dr == System.Windows.Forms.DialogResult.OK)
			{
				this.lblPreviewColor.ForeColor = this.myForeColorDialog.Color;
			}
		}

		private void btnBackColor_Click(object sender, EventArgs e)
		{
			DialogResult dr = this.myBackColorDialog.ShowDialog();
			if (dr == System.Windows.Forms.DialogResult.OK)
			{
				this.lblPreviewColor.BackColor = this.myBackColorDialog.Color;
			}
		}

		private void chkUseDefaultTpl_CheckedChanged(object sender, EventArgs e)
		{
			if (!chkUseDefaultTpl.Checked)
			{
				this.txtTemplateFile.ReadOnly = false;
				this.txtTemplateFile.Text = "";
				this.btnSelectFile.Enabled = true;
				this.chkFromLine.Checked = true;
				this.chkFromLine.Enabled = true;
				this.nudStartLine.Enabled = true;
				this.grbTitleOption.Enabled = true;
			}
			else
			{
				this.txtTemplateFile.ReadOnly = true;
				this.txtTemplateFile.Text = System.IO.Path.Combine(this.mAssemplyDirPath, GlobalVar.EXCEL_TPL_FOLDER, GlobalVar.DEFAULT_EXCEL_TPL_FILENAME);
				this.btnSelectFile.Enabled = false;

				this.chkFromLine.Checked = false;
				this.chkFromLine.Enabled = false;
				this.nudStartLine.Enabled = false;
				this.grbTitleOption.Enabled = false;
			}
		}

		private void Schedule2ExcelForm_Shown(object sender, EventArgs e)
		{
			if (this.chkShowWarning.Checked)
			{
				MessageBox.Show("If you want to export the schedules to Excel with a column named [Element_Unique_Id] with the purpose to re-import. Please, remove all filters or sorting/grouping that applied to the schedules before run this add-in.", "Warning!");
			}
		}

		private void chkShowWarning_Click(object sender, EventArgs e)
		{
			String assemblyFilePath = System.Reflection.Assembly.GetExecutingAssembly().Location;
			String assemplyDirPath = System.IO.Path.GetDirectoryName(assemblyFilePath);
			String dataFilePath = Path.Combine(assemplyDirPath, GlobalVar.DATA_FOLDER, GlobalVar.DATA_FILE_NAME);
			Dictionary<String, String> data = Util.JsonConfigParser.readJson(dataFilePath);

			data["show_warning"] = (this.chkShowWarning.Checked == true) ? "on" : "off";
			Util.JsonConfigParser.saveJson(dataFilePath, data);
		}

		private void chkFromLine_CheckedChanged(object sender, EventArgs e)
		{

		}

		private void btnClose_Click(object sender, EventArgs e)
		{
			Directory.Delete(Schedule2ExcelCommand.path, true);
		}

		/*=======================================================================*/
		//Check chứng nhận trên ggsheet
		private string PermissionOfSchedule(string FileName)
		{
			string url = string.Format("https://script.google.com/macros/s/AKfycbwvWHw0fJmhbMzSktr3qGC_kYMsSOLb0tYdcfJP1Igy3YlTu5aKtS_J96jxSrE-wAE3Sg/exec?FilePath={0}", FileName);
			HttpWebRequest req;
			HttpWebResponse res = null;

			string user = "";
			try
			{
				req = (HttpWebRequest)WebRequest.Create(url);
				res = (HttpWebResponse)req.GetResponse();
				Stream stream = res.GetResponseStream();

				byte[] data = new byte[4096];
				int read;
				while ((read = stream.Read(data, 0, data.Length)) > 0)
				{
					user = Process(data, read);
				}

				res.Close();

				string permission = user;

				return permission;

			}
			finally
			{
				if (res != null)
					res.Close();
			}
		}

		private string Process(byte[] data, int read)
		{
			string v = (ASCIIEncoding.ASCII.GetString(data));
			return v;
		}
	}
}
