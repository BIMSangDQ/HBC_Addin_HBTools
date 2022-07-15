using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Windows.Forms;

using Autodesk.Revit.DB;

using Excel = Microsoft.Office.Interop.Excel;

namespace SheetDuplicateAndAlignView.Forms
{
	public partial class SheetMakerForm : System.Windows.Forms.Form
	{

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

		private List<SheetItem> mSheetItemList;

		private Excel.Application xlApp;

		public SheetMakerForm()
		{
			InitializeComponent();

			this.mSheetItemList = new List<SheetItem>();
		}

		private void SheetMakerForm_Load(object sender, EventArgs e)
		{
			this.initComboBoxTitleBlock();
			this.initComboboxViewportType();
			this.initListBoxSheets();
		}

		private void initComboboxViewportType()
		{
			FilteredElementCollector col = new FilteredElementCollector(this.mDoc);
			IList<ElementType> viewportTypes = col.OfClass(typeof(ElementType)).Cast<ElementType>().Where(q => q.FamilyName == "Viewport").ToList();

			foreach (ElementType ele in viewportTypes)
			{
				ViewportTypeItem item = new ViewportTypeItem();
				item.Text = ele.Name;
				item.Value = ele;
				this.cboViewportType.Items.Add(item);
			}

			this.cboViewportType.Sorted = true;
			this.cboViewportType.AutoCompleteMode = AutoCompleteMode.SuggestAppend;
			this.cboViewportType.AutoCompleteSource = AutoCompleteSource.ListItems;

		}

		private void initComboBoxTitleBlock()
		{
			FilteredElementCollector col = new FilteredElementCollector(this.mDoc);
			col.WhereElementIsElementType();
			col.OfCategory(BuiltInCategory.OST_TitleBlocks);

			this.cboTitleBlock.Items.Clear();
			foreach (Element e in col.ToElements())
			{
				TitleBlockItem item = new TitleBlockItem();
				item.Text = e.Name;
				item.Value = e;
				this.cboTitleBlock.Items.Add(item);
			}

		}

		private void initListBoxSheets()
		{
			List<ViewSheet> list = this.getAllViewSheets();

			this.mSheetItemList.Clear();
			this.lbViewSheet.Items.Clear();

			foreach (ViewSheet sheet in list)
			{
				SheetItem item = new SheetItem();
				item.SheetName = sheet.Name;
				item.SheetNumber = sheet.SheetNumber;
				item.SheetId = sheet.Id;

				this.lbViewSheet.Items.Add(item);
				this.mSheetItemList.Add(item);
			}

			this.lbViewSheet.Sorted = true;
		}

		private List<ViewSheet> getAllViewSheets()
		{
			FilteredElementCollector col = new FilteredElementCollector(this.mDoc);
			col.OfCategory(BuiltInCategory.OST_Sheets);

			List<ViewSheet> list = new List<ViewSheet>();
			//foreach (ViewSheet item in col.Cast<ViewSheet>().ToList())
			foreach (ViewSheet item in col.ToElements())
			{
				list.Add(item);
			}

			return list;
		}

		private bool isSheetExisted(String pSheetNumber)
		{
			FilteredElementCollector col = new FilteredElementCollector(this.mDoc);
			col.OfCategory(BuiltInCategory.OST_Sheets);

			foreach (ViewSheet item in col.ToElements())
			{
				if (item.SheetNumber.Equals(pSheetNumber))
					return true;
			}

			return false;


		}

		private void parseSheetsFromExcel()
		{
			this.xlApp = new Excel.Application();

			if (xlApp == null)
			{
				MessageBox.Show("Microsoft Excel can't be found. Please install it before run this add-in.");
				return;
			}

			Excel.Workbook xlWorkBook;
			object misValue = System.Reflection.Missing.Value;

			xlWorkBook = xlApp.Workbooks.Open(this.txtExcelFile.Text,
				0,
				true,
				5,
				"",
				"",
				true,
				Microsoft.Office.Interop.Excel.XlPlatform.xlWindows,
				"\t",
				false,
				false,
				0,
				true,
				1,
				0);

			Excel.Worksheet worksheet = xlWorkBook.Worksheets[1] as Excel.Worksheet;

			int start_excel_row = 2;
			int last_excel_row = worksheet.Cells.SpecialCells(Excel.XlCellType.xlCellTypeLastCell, misValue).Row;


			try
			{
				this.mSheetItemList.Clear();
				this.lbViewSheet.Items.Clear();
				for (int i = start_excel_row; i <= last_excel_row; i++)
				{
					SheetItem item = new SheetItem();

					Excel.Range rangeSheetNumber = worksheet.Cells[i, 1] as Excel.Range;
					Excel.Range rangeSheetName = worksheet.Cells[i, 2] as Excel.Range;
					item.SheetNumber = rangeSheetNumber.Value2;
					item.SheetName = rangeSheetName.Value2;

					this.mSheetItemList.Add(item);
					this.lbViewSheet.Items.Add(item);
				}
			}
			catch (Exception ex)
			{
				MessageBox.Show(ex.ToString(), "Parsing Excel");
			}
			finally
			{

			}

		}

		private void createNewSheets()
		{
			this.xlApp = new Excel.Application();

			if (xlApp == null)
			{
				MessageBox.Show("Microsoft Excel can't be found. Please install it before run this add-in.");
				return;
			}

			Excel.Workbook xlWorkBook;
			object misValue = System.Reflection.Missing.Value;

			xlWorkBook = xlApp.Workbooks.Open(this.txtExcelFile.Text,
				0,
				true,
				5,
				"",
				"",
				true,
				Microsoft.Office.Interop.Excel.XlPlatform.xlWindows,
				"\t",
				false,
				false,
				0,
				true,
				1,
				0);

			Excel.Worksheet worksheet = xlWorkBook.Worksheets[1] as Excel.Worksheet;

			int start_excel_row = 2;
			int last_excel_row = worksheet.Cells.SpecialCells(Excel.XlCellType.xlCellTypeLastCell, misValue).Row;

			Transaction trans = new Transaction(this.mDoc, "SHEET MAKER");

			ViewSheet curViewSheet = null;
			TitleBlockItem titleBlock = this.cboTitleBlock.SelectedItem as TitleBlockItem;

			String viewSheetText = "";
			String viewSheetNumber = "";
			//List<ViewSheet> sheetList = this.getAllViewSheets();

			for (int i = start_excel_row; i <= last_excel_row; i++)
			{
				try
				{
					Excel.Range rangeSheetNumber = worksheet.Cells[i, 1] as Excel.Range;
					Excel.Range rangePostfixNumber = worksheet.Cells[i, 2] as Excel.Range;
					Excel.Range rangeSheetName = worksheet.Cells[i, 3] as Excel.Range;

					viewSheetNumber = rangeSheetNumber.Value2;

					if (!String.IsNullOrEmpty(Convert.ToString(rangePostfixNumber.Value2)))
					{
						int n = 0;
						if (Int32.TryParse(Convert.ToString(rangePostfixNumber.Value2), out n))
						{
							String sn = "";
							if (n < 10)
							{
								sn = "00" + n;
							}
							else if (n > 9 && n < 100)
							{
								sn = "0" + n;
							}
							else
							{
								sn = n.ToString();
							}

							viewSheetNumber += "-" + sn;
						}
						else
						{
							viewSheetNumber += "-" + rangePostfixNumber.Value2;
						}

					}

					viewSheetText = viewSheetNumber;
					if (!String.IsNullOrEmpty(Convert.ToString(rangeSheetName.Value2)))
					{
						viewSheetText += " - " + rangeSheetName.Value2;
					}

					//Ignore if the sheet number is already existed
					if (this.isSheetExisted(viewSheetNumber))
					{
						this.appendTextWithColor(viewSheetText + ": ", System.Drawing.Color.Black);
						this.appendTextWithColor("[ALREADY EXISTED]" + Environment.NewLine, System.Drawing.Color.Orange);
						continue;
					}

					trans.Start();
					curViewSheet = ViewSheet.Create(this.mDoc, titleBlock.Value.Id);
					curViewSheet.SheetNumber = viewSheetNumber;
					if (!String.IsNullOrEmpty(Convert.ToString(rangeSheetName.Value2)))
						curViewSheet.Name = rangeSheetName.Value2;

					trans.Commit();

					this.appendTextWithColor(viewSheetText + ": ", System.Drawing.Color.Black);
					this.appendTextWithColor("[CREATED]" + Environment.NewLine, System.Drawing.Color.Green);
				}
				catch (Exception ex)
				{
					this.appendTextWithColor(viewSheetText + ": ", System.Drawing.Color.Black);
					this.appendTextWithColor("[FAILED]" + Environment.NewLine, System.Drawing.Color.Red);
					this.appendTextWithColor(ex.ToString() + Environment.NewLine, System.Drawing.Color.Red);
					if (trans.HasStarted())
						trans.RollBack();
				}
				finally
				{

				}
			}
		}

		private void insertViews()
		{

			Transaction trans = new Transaction(this.mDoc, "INSERT VIEWS");

			ViewSheet curViewSheet = null;
			TitleBlockItem titleBlock = this.cboTitleBlock.SelectedItem as TitleBlockItem;

			String viewSheetText = "";
			//List<ViewSheet> sheetList = this.getAllViewSheets();

			foreach (SheetItem sheet in this.lbViewSheet.Items)
			{
				try
				{
					viewSheetText = sheet.ToString();
					if (!sheet.hasViews())
					{
						this.appendTextWithColor(viewSheetText + ": ", System.Drawing.Color.Black);
						this.appendTextWithColor("[NO VIEW INSERTED]" + Environment.NewLine, System.Drawing.Color.Orange);
						continue;
					}

					//Create new Sheet
					trans.Start();
					this.appendTextWithColor(viewSheetText + ": " + Environment.NewLine, System.Drawing.Color.Black);
					curViewSheet = this.mDoc.GetElement(sheet.SheetId) as ViewSheet;

					//Insert view to current new sheet
					this.insertViewsToSheet(curViewSheet, sheet);
					trans.Commit();

					this.appendTextWithColor(viewSheetText + ": ", System.Drawing.Color.Black);
					this.appendTextWithColor("[DONE]" + Environment.NewLine, System.Drawing.Color.Green);


				}
				catch (Exception ex)
				{
					this.appendTextWithColor(viewSheetText + ": ", System.Drawing.Color.Black);
					this.appendTextWithColor("[FAILED]" + Environment.NewLine, System.Drawing.Color.Red);
					this.appendTextWithColor(ex.ToString() + Environment.NewLine, System.Drawing.Color.Red);

					if (trans.HasStarted())
						trans.RollBack();
				}
				finally
				{

				}

			}

		}


		private bool insertViewsToSheet(ViewSheet pSheet, SheetItem pSheetItem)
		{
			String viewName = "";
			int k = 1;
			UV previousPoint = UV.Zero;

			//Insert Views
			foreach (ViewItem view in pSheetItem.ViewList)
			{
				try
				{
					viewName = view.Text;
					if (Viewport.CanAddViewToSheet(this.mDoc, pSheet.Id, view.Value.Id))
					{
						//Add viewport
						Viewport curViewport = Viewport.Create(this.mDoc, pSheet.Id, view.Value.Id, XYZ.Zero);
						previousPoint = this.setCenterViewPosition(pSheet, curViewport, previousPoint, k++);

						//Change viewport type
						if (this.cboViewportType.SelectedIndex > -1)
						{
							ViewportTypeItem item = this.cboViewportType.SelectedItem as ViewportTypeItem;
							curViewport.ChangeTypeId(item.Value.Id);
						}

						this.appendTextWithColor(viewName + ": ", System.Drawing.Color.Black);
						this.appendTextWithColor("[DONE]" + Environment.NewLine, System.Drawing.Color.Green);
					}
					else
					{
						this.appendTextWithColor(viewName + ": ", System.Drawing.Color.Black);
						this.appendTextWithColor("[WARNING: Cannot insert view to sheet]" + Environment.NewLine, System.Drawing.Color.Orange);
					}
				}
				catch (Exception ex)
				{
					this.appendTextWithColor(viewName + ": ", System.Drawing.Color.Black);
					this.appendTextWithColor("[FAILED]" + Environment.NewLine, System.Drawing.Color.Red);
					this.appendTextWithColor(ex.ToString() + Environment.NewLine, System.Drawing.Color.Red);
				}
				finally
				{

				}

			}

			//Insert Legend
			foreach (ViewItem view in pSheetItem.LegendList)
			{
				try
				{
					viewName = view.Text;
					if (Viewport.CanAddViewToSheet(this.mDoc, pSheet.Id, view.Value.Id))
					{
						//Add viewport
						Viewport curViewport = Viewport.Create(this.mDoc, pSheet.Id, view.Value.Id, XYZ.Zero);
						previousPoint = this.setCenterViewPosition(pSheet, curViewport, previousPoint, k++);

						//Change viewport type
						if (this.cboViewportType.SelectedIndex > -1)
						{
							ViewportTypeItem item = this.cboViewportType.SelectedItem as ViewportTypeItem;
							curViewport.ChangeTypeId(item.Value.Id);
						}

						this.appendTextWithColor(viewName + ": ", System.Drawing.Color.Black);
						this.appendTextWithColor("[DONE]" + Environment.NewLine, System.Drawing.Color.Green);
					}
					else
					{
						this.appendTextWithColor(viewName + ": ", System.Drawing.Color.Black);
						this.appendTextWithColor("[WARNING: Cannot insert view to sheet]" + Environment.NewLine, System.Drawing.Color.Orange);
					}
				}
				catch (Exception ex)
				{
					this.appendTextWithColor(viewName + ": ", System.Drawing.Color.Black);
					this.appendTextWithColor("[FAILED]" + Environment.NewLine, System.Drawing.Color.Red);
					this.appendTextWithColor(ex.ToString() + Environment.NewLine, System.Drawing.Color.Red);
				}
				finally
				{

				}

			}

			//Insert Schedule
			foreach (ViewItem view in pSheetItem.ScheduleList)
			{
				try
				{
					viewName = view.Text;

					ScheduleSheetInstance curSchedule = ScheduleSheetInstance.Create(this.mDoc, pSheet.Id, view.Value.Id, XYZ.Zero);
					previousPoint = this.setCenterSchedulePosition(pSheet, curSchedule, previousPoint, k++);

					//Change viewport type
					if (this.cboViewportType.SelectedIndex > -1)
					{
						ViewportTypeItem item = this.cboViewportType.SelectedItem as ViewportTypeItem;
						curSchedule.ChangeTypeId(item.Value.Id);
					}

					this.appendTextWithColor(viewName + ": ", System.Drawing.Color.Black);
					this.appendTextWithColor("[DONE]" + Environment.NewLine, System.Drawing.Color.Green);

				}
				catch (Exception ex)
				{
					this.appendTextWithColor(viewName + ": ", System.Drawing.Color.Black);
					this.appendTextWithColor("[FAILED]" + Environment.NewLine, System.Drawing.Color.Red);
					this.appendTextWithColor(ex.ToString() + Environment.NewLine, System.Drawing.Color.Red);
				}
				finally
				{

				}

			}

			return (k > 1);

		}

		private UV setCenterViewPosition(ViewSheet pSheet, Viewport pViewport, UV pPreviousPoint, int k)
		{
			//Get center point of viewport
			//XYZ vpCenter = pViewport.GetBoxCenter();
			//Outline vpOutline = pViewport.GetBoxOutline();

			BoundingBoxUV sheetOutline = pSheet.Outline;
			UV offsetPoint = (sheetOutline.Min + sheetOutline.Max) * 0.25;

			UV newPoint = UV.Zero;

			//if (k == 1 || k == 4 || k == 7 || k == 10 || k == 13 || k == 16)
			if ((k - 1) % 3 == 0)
			{
				newPoint = new UV(offsetPoint.U, pPreviousPoint.V + offsetPoint.V);
			}
			else
			{
				newPoint = new UV(pPreviousPoint.U + offsetPoint.U, pPreviousPoint.V);
			}

			XYZ sheetPointXYZ = new XYZ(newPoint.U, newPoint.V, 0);

			pViewport.SetBoxCenter(sheetPointXYZ);

			return newPoint;
		}

		private UV setCenterSchedulePosition(ViewSheet pSheet, ScheduleSheetInstance pScheduleSheet, UV pPreviousPoint, int k)
		{

			BoundingBoxUV sheetOutline = pSheet.Outline;
			UV offsetPoint = (sheetOutline.Min + sheetOutline.Max) * 0.25;

			UV newPoint = UV.Zero;

			if ((k - 1) % 3 == 0)
			{
				newPoint = new UV(offsetPoint.U, pPreviousPoint.V + offsetPoint.V);
			}
			else
			{
				newPoint = new UV(pPreviousPoint.U + offsetPoint.U, pPreviousPoint.V);
			}

			XYZ sheetPointXYZ = new XYZ(newPoint.U, newPoint.V, 0);

			ElementTransformUtils.MoveElement(this.mDoc, pScheduleSheet.Id, sheetPointXYZ);

			return newPoint;
		}

		private void appendTextWithColor(String text, System.Drawing.Color color)
		{
			this.rtbLog.Select(this.rtbLog.TextLength, 0);
			this.rtbLog.SelectionColor = color;
			this.rtbLog.AppendText(text);
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

		private void addEditViews()
		{
			int idx = this.lbViewSheet.SelectedIndex;
			if (idx != ListBox.NoMatches)
			{
				if (this.lbViewSheet.Items.Count > 0 && idx > -1)
				{
					SheetItem selectedSheet = this.lbViewSheet.SelectedItem as SheetItem;
					List<SheetItem> selectedSheetItemList = new List<SheetItem>();
					selectedSheetItemList.Add(selectedSheet);
					ChildForms.ViewListForm fView = new ChildForms.ViewListForm();
					fView.App = this.mApp;
					fView.Doc = this.mDoc;
					fView.ViewOption = FilterViewOption.VIEW;
					fView.SelectedSheetItemList = selectedSheetItemList;
					System.Windows.Forms.DialogResult dr = fView.ShowDialog();

					if (dr == System.Windows.Forms.DialogResult.OK)
					{
						selectedSheet.ViewList = fView.SelectedViewList;
					}
				}
			}
		}

		private void addEditLegends()
		{
			int idx = this.lbViewSheet.SelectedIndex;
			if (idx != ListBox.NoMatches)
			{
				if (this.lbViewSheet.Items.Count > 0 && idx > -1)
				{
					SheetItem selectedSheet = this.lbViewSheet.SelectedItem as SheetItem;
					List<SheetItem> selectedSheetItemList = new List<SheetItem>();
					selectedSheetItemList.Add(selectedSheet);
					ChildForms.ViewListForm fView = new ChildForms.ViewListForm();
					fView.App = this.mApp;
					fView.Doc = this.mDoc;
					fView.ViewOption = FilterViewOption.LEGEND;
					fView.SelectedSheetItemList = selectedSheetItemList;
					System.Windows.Forms.DialogResult dr = fView.ShowDialog();

					if (dr == System.Windows.Forms.DialogResult.OK)
					{
						selectedSheet.LegendList = fView.SelectedViewList;
					}
				}
			}


		}

		private void addEditSchedules()
		{
			int idx = this.lbViewSheet.SelectedIndex;
			if (idx != ListBox.NoMatches)
			{
				if (this.lbViewSheet.Items.Count > 0 && idx > -1)
				{
					SheetItem selectedSheet = this.lbViewSheet.SelectedItem as SheetItem;
					List<SheetItem> selectedSheetItemList = new List<SheetItem>();
					selectedSheetItemList.Add(selectedSheet);
					ChildForms.ViewListForm fView = new ChildForms.ViewListForm();
					fView.App = this.mApp;
					fView.Doc = this.mDoc;
					fView.ViewOption = FilterViewOption.SCHEDULE;
					fView.SelectedSheetItemList = selectedSheetItemList;
					System.Windows.Forms.DialogResult dr = fView.ShowDialog();

					if (dr == System.Windows.Forms.DialogResult.OK)
					{
						selectedSheet.ScheduleList = fView.SelectedViewList;
					}
				}
			}
		}

		private bool checkViewExistInList(List<ViewItem> pList, ViewItem pItem)
		{
			foreach (ViewItem ele in pList)
			{
				if (ele.Value.Id.Equals(pItem.Value.Id))
					return true;
			}

			return false;
		}

		private void btnSelectExcelFile_Click(object sender, EventArgs e)
		{
			this.myOpenFileDialog.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
			this.myOpenFileDialog.RestoreDirectory = true;
			this.myOpenFileDialog.DefaultExt = "xlsx";
			this.myOpenFileDialog.Filter = "Excel Files (*.xlsx)|*.xlsx";
			this.myOpenFileDialog.FileName = "";
			System.Windows.Forms.DialogResult dr = this.myOpenFileDialog.ShowDialog();
			if (dr == System.Windows.Forms.DialogResult.OK)
			{
				this.txtExcelFile.Text = this.myOpenFileDialog.FileName;

			}
		}

		private void btnCreateSheet_Click(object sender, EventArgs e)
		{
			if (this.txtExcelFile.Text == "")
			{
				MessageBox.Show("Please select a Excel file.", "Excel File");
				return;
			}

			if (this.cboTitleBlock.SelectedIndex == -1)
			{
				MessageBox.Show("Please select a Title Block.", "Title Block");
				return;
			}

			try
			{
				this.grbData.Enabled = false;
				this.grbSheet.Enabled = false;
				this.btnClose.Enabled = false;
				this.btnInsertView.Enabled = false;

				this.rtbLog.Text = "";
				this.createNewSheets();
				this.initListBoxSheets();
			}
			catch (Exception ex)
			{
				MessageBox.Show(ex.ToString());
			}
			finally
			{
				this.grbData.Enabled = true;
				this.grbSheet.Enabled = true;
				this.btnClose.Enabled = true;
				this.btnInsertView.Enabled = true;

				if (this.xlApp != null)
				{
					this.xlApp.Quit();
					this.releaseObject(this.xlApp);
				}
			}
		}

		private void txtSearch_TextChanged(object sender, EventArgs e)
		{
			this.lbViewSheet.Items.Clear();
			if (String.IsNullOrEmpty(this.txtSearch.Text))
			{
				foreach (SheetItem ele in this.mSheetItemList)
				{
					this.lbViewSheet.Items.Add(ele);
				}
			}
			else
			{
				//Matching the search text with the items in source list
				string q = this.txtSearch.Text.ToLower();
				var sql = from item in this.mSheetItemList
						  where item.SheetNumber.ToLower().Contains(q) || item.SheetName.ToLower().Contains(q)
						  select item;

				//and add them to the listbox
				foreach (SheetItem ele in sql.ToList())
				{
					this.lbViewSheet.Items.Add(ele);
				}
			}
		}

		private void lbViewSheet_MouseDoubleClick(object sender, MouseEventArgs e)
		{
			this.addEditViews();
		}

		private void lbViewSheet_MouseDown(object sender, MouseEventArgs e)
		{
			if (e.Button == System.Windows.Forms.MouseButtons.Right)
			{
				int idx = this.lbViewSheet.IndexFromPoint(e.Location);

				if (idx != ListBox.NoMatches)
				{
					this.lbViewSheet.ClearSelected();
					this.lbViewSheet.SelectedIndex = idx;
				}

			}
		}

		private void lbViewSheet_MouseUp(object sender, MouseEventArgs e)
		{
			if (e.Button == System.Windows.Forms.MouseButtons.Right)
			{
				int idx = this.lbViewSheet.IndexFromPoint(e.Location);

				if (idx != ListBox.NoMatches)
				{
					this.cmsViewSheet.Visible = true;
				}
				else
				{
					this.cmsViewSheet.Visible = false;
				}
			}

		}

		private void btnView_Click(object sender, EventArgs e)
		{
			if (this.lbViewSheet.SelectedIndices.Count == 0)
			{
				MessageBox.Show("Please, select the sheet to add views.");
				return;
			}

			this.addEditViews();
		}

		private void btnLegend_Click(object sender, EventArgs e)
		{
			if (this.lbViewSheet.SelectedIndices.Count == 0)
			{
				MessageBox.Show("Please, select the sheets.");
				return;
			}


			ChildForms.ViewListForm fView = new ChildForms.ViewListForm();
			fView.App = this.mApp;
			fView.Doc = this.mDoc;
			fView.IsApplyAll = true;
			fView.ViewOption = FilterViewOption.LEGEND;
			System.Windows.Forms.DialogResult dr = fView.ShowDialog();
			if (dr == System.Windows.Forms.DialogResult.OK)
			{
				for (int i = 0; i < this.lbViewSheet.SelectedIndices.Count; i++)
				{
					int id = this.lbViewSheet.SelectedIndices[i];
					SheetItem item = this.lbViewSheet.Items[id] as SheetItem;
					if (fView.SelectedViewList.Count > 0)
					{
						foreach (ViewItem ele in fView.SelectedViewList)
						{
							if (this.checkViewExistInList(item.LegendList, ele))
								continue;
							item.LegendList.Add(ele);
						}
					}

				}
			}

		}

		private void btnSchedule_Click(object sender, EventArgs e)
		{
			if (this.lbViewSheet.SelectedIndices.Count == 0)
			{
				MessageBox.Show("Please, select the sheets.");
				return;
			}

			ChildForms.ViewListForm fView = new ChildForms.ViewListForm();
			fView.App = this.mApp;
			fView.Doc = this.mDoc;
			fView.IsApplyAll = true;
			fView.ViewOption = FilterViewOption.SCHEDULE;
			System.Windows.Forms.DialogResult dr = fView.ShowDialog();
			if (dr == System.Windows.Forms.DialogResult.OK)
			{
				for (int i = 0; i < this.lbViewSheet.SelectedIndices.Count; i++)
				{
					int id = this.lbViewSheet.SelectedIndices[i];
					SheetItem item = this.lbViewSheet.Items[id] as SheetItem;
					if (fView.SelectedViewList.Count > 0)
					{
						foreach (ViewItem ele in fView.SelectedViewList)
						{
							if (this.checkViewExistInList(item.ScheduleList, ele))
								continue;
							item.ScheduleList.Add(ele);
						}
					}
				}
			}
		}

		private void btnClearAllView_Click(object sender, EventArgs e)
		{
			if (this.lbViewSheet.SelectedIndices.Count == 0)
			{
				MessageBox.Show("Please, select the sheets want to clear.");
				return;
			}

			if (MessageBox.Show("Are you sure?", "Confirm", MessageBoxButtons.YesNo) == System.Windows.Forms.DialogResult.No)
			{
				return;
			}

			for (int i = 0; i < this.lbViewSheet.SelectedIndices.Count; i++)
			{
				int id = this.lbViewSheet.SelectedIndices[i];
				SheetItem item = this.lbViewSheet.Items[id] as SheetItem;
				item.ViewList.Clear();
			}
		}

		private void btnClearAllLegend_Click(object sender, EventArgs e)
		{
			if (this.lbViewSheet.SelectedIndices.Count == 0)
			{
				MessageBox.Show("Please, select the sheets want to clear.");
				return;
			}

			if (MessageBox.Show("Are you sure?", "Confirm", MessageBoxButtons.YesNo) == System.Windows.Forms.DialogResult.No)
			{
				return;
			}

			for (int i = 0; i < this.lbViewSheet.SelectedIndices.Count; i++)
			{
				int id = this.lbViewSheet.SelectedIndices[i];
				SheetItem item = this.lbViewSheet.Items[id] as SheetItem;
				item.LegendList.Clear();
			}
		}

		private void btnClearAllSchedule_Click(object sender, EventArgs e)
		{
			if (this.lbViewSheet.SelectedIndices.Count == 0)
			{
				MessageBox.Show("Please, select the sheets want to clear.");
				return;

			}

			if (MessageBox.Show("Are you sure?", "Confirm", MessageBoxButtons.YesNo) == System.Windows.Forms.DialogResult.No)
			{
				return;
			}

			for (int i = 0; i < this.lbViewSheet.SelectedIndices.Count; i++)
			{
				int id = this.lbViewSheet.SelectedIndices[i];
				SheetItem item = this.lbViewSheet.Items[id] as SheetItem;
				item.ScheduleList.Clear();
			}
		}

		private void btnInsertView_Click(object sender, EventArgs e)
		{
			if (this.lbViewSheet.Items.Count == 0)
			{
				MessageBox.Show("Cannot create sheets because the list of sheets is empty.");
				return;
			}

			if (MessageBox.Show("Are you ready?", "Confirm", MessageBoxButtons.YesNo) == System.Windows.Forms.DialogResult.No)
			{
				return;
			}


			try
			{
				this.grbData.Enabled = false;
				this.grbSheet.Enabled = false;
				this.btnClose.Enabled = false;
				this.btnInsertView.Enabled = false;

				this.rtbLog.Text = "";
				this.insertViews();
			}
			catch (Exception ex)
			{
				this.appendTextWithColor("[ERROR]" + Environment.NewLine, System.Drawing.Color.Red);
				this.appendTextWithColor(ex.ToString() + Environment.NewLine, System.Drawing.Color.Red);
			}
			finally
			{
				this.grbData.Enabled = true;
				this.grbSheet.Enabled = true;
				this.btnClose.Enabled = true;
				this.btnInsertView.Enabled = true;
			}
		}










	}

	public enum FilterViewOption
	{
		VIEW = 1,
		SCHEDULE = 2,
		LEGEND = 3,
		SCHEDULE_OR_LENGEND = 4
	}

	public class SheetItem
	{
		public String SheetNumber
		{
			get;
			set;
		}

		public String SheetName
		{
			get;
			set;
		}

		public ElementId SheetId
		{
			get;
			set;
		}

		private List<ViewItem> mLegendList;
		public List<ViewItem> LegendList
		{
			get { return this.mLegendList; }
			set { this.mLegendList = value; }
		}

		private List<ViewItem> mViewList;
		public List<ViewItem> ViewList
		{
			get { return this.mViewList; }
			set { this.mViewList = value; }
		}

		private List<ViewItem> mScheduleList;
		public List<ViewItem> ScheduleList
		{
			get { return this.mScheduleList; }
			set { this.mScheduleList = value; }
		}

		public SheetItem()
		{
			this.mLegendList = new List<ViewItem>();
			this.mViewList = new List<ViewItem>();
			this.ScheduleList = new List<ViewItem>();
		}

		public bool hasViews()
		{
			int n = this.mViewList.Count + this.mLegendList.Count + this.mScheduleList.Count;

			return (n > 0);
		}

		public override string ToString()
		{
			return SheetNumber + " - " + SheetName;
		}
	}

	public class ViewItem
	{
		public String Text
		{
			set;
			get;
		}

		public Autodesk.Revit.DB.View Value
		{
			set;
			get;
		}

		public ViewItem() { }

		public override string ToString()
		{
			return Text;
		}
	}

	public class TitleBlockItem
	{
		public String Text
		{
			set;
			get;
		}

		public Element Value
		{
			set;
			get;
		}

		public TitleBlockItem() { }

		public override string ToString()
		{
			return Text;
		}
	}

	public class ViewportTypeItem
	{
		public String Text
		{
			set;
			get;
		}

		public Element Value
		{
			set;
			get;
		}
		public ViewportTypeItem() { }

		public override string ToString()
		{
			return Text;
		}
	}
}
