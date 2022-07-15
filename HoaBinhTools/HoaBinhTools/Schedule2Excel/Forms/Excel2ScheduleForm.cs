using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;

using Autodesk.Revit.DB;

using Excel = Microsoft.Office.Interop.Excel;


namespace Schedule2Excel2k16.Forms
{
	public partial class Excel2ScheduleForm : System.Windows.Forms.Form
	{
		private const String TAG = "Excel2Schedule";
		private String mExcelFile = "";

		private List<HolderScheduleItem> mScheduleList;
		public List<HolderScheduleItem> ScheduleList
		{
			get { return mScheduleList; }
			set { mScheduleList = value; }
		}
		private BindingSource bsSchedule;

		private List<HolderFieldItem> mScheduleFieldList;
		public List<HolderFieldItem> ScheduleFieldList
		{
			get { return mScheduleFieldList; }
			set { mScheduleFieldList = value; }
		}
		private BindingSource bsScheduleField;

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

		public Excel2ScheduleForm()
		{
			InitializeComponent();

			this.mScheduleList = new List<HolderScheduleItem>();
			this.mScheduleFieldList = new List<HolderFieldItem>();

			this.bsSchedule = new BindingSource();
			this.bsScheduleField = null;

			this.mExcelFile = "";
		}

		private void Excel2ScheduleForm_Load(object sender, EventArgs e)
		{
			this.initScheduleListBox();
		}

		private void initScheduleListBox()
		{

			this.bsSchedule.DataSource = this.mScheduleList;
			this.lbScheduleList.DataSource = this.bsSchedule;
			this.lbScheduleList.DisplayMember = "ScheduleName";
			this.lbScheduleList.ValueMember = "Id";


			//this.bsScheduleField = new BindingSource();
			//this.bsExport.DataSource = this.mExportList;
			//this.lbExport2ExcelList.DataSource = this.bsExport;
			//this.lbExport2ExcelList.DisplayMember = "ScheduleName";
			//this.lbExport2ExcelList.ValueMember = "Id";
		}

		private void initExcelSheetListBox(String excelFile)
		{
			Excel.Application xlApp = new Excel.Application();

			if (xlApp == null)
			{
				MessageBox.Show("Microsoft Excel can't be found. Please install it before run this add-in.");
				return;
			}

			Excel.Workbook xlWorkBook;
			object misValue = System.Reflection.Missing.Value;

			xlWorkBook = xlApp.Workbooks.Open(excelFile,
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

			this.lbExcelSheet.Items.Clear();
			if (xlWorkBook.Worksheets.Count > 0)
			{
				foreach (Excel.Worksheet worksheet in xlWorkBook.Worksheets)
				{
					this.lbExcelSheet.Items.Add(worksheet.Name);
				}
				this.lbExcelSheet.SelectedIndex = 0;
			}

			xlWorkBook.Close(false, misValue, misValue);
			xlApp.Quit();

		}

		private void import2Schedule()
		{
			Excel.Application xlApp = new Excel.Application();

			if (xlApp == null)
			{
				MessageBox.Show("Microsoft Excel can't be found. Please install it before run this add-in.");
				return;
			}

			Excel.Workbook xlWorkBook;
			object misValue = System.Reflection.Missing.Value;

			xlWorkBook = xlApp.Workbooks.Open(this.mExcelFile,
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

			String sheetName = this.lbExcelSheet.GetItemText(this.lbExcelSheet.SelectedItem);
			Excel.Worksheet worksheet = xlWorkBook.Worksheets[sheetName] as Excel.Worksheet;

			int start_excel_row = GlobalVar.DEFAULT_DATA_ROW;
			int last_excel_row = worksheet.Cells.SpecialCells(Excel.XlCellType.xlCellTypeLastCell, misValue).Row;
			int last_excel_col = this.findLastColumnByVal(worksheet, GlobalVar.DEFAULT_HEADER_ROW, GlobalVar.ELEMENT_UNIQUE_ID_PARAM_NAME);

			StringBuilder sb = new StringBuilder();

			HolderScheduleItem scheduleItem = this.mScheduleList[this.lbScheduleList.SelectedIndex];
			ViewSchedule vs = this.mDoc.GetElement(new ElementId(scheduleItem.Id)) as ViewSchedule;
			TableData td = vs.GetTableData();
			TableSectionData header = td.GetSectionData(SectionType.Header);
			TableSectionData tsd = td.GetSectionData(SectionType.Body);



			int start_schedule_row = tsd.FirstRowNumber + 1;
			if (header.HideSection)
			{
				start_schedule_row += 1;
			}
			int offset_shchedule_row = start_excel_row - start_schedule_row;


			Transaction trans = new Transaction(this.mDoc, "EXCEL-2-SCHEDULE");
			trans.Start();

			IList<Element> eleList = this.getElementsOfSchedule(vs);
			//int lastColId = tsd.NumberOfColumns + 1;

			for (int j = 0; j < this.lbScheduleFieldList.SelectedIndices.Count; j++)
			{
				int id = this.lbScheduleFieldList.SelectedIndices[j];
				HolderFieldItem fieldItem = this.mScheduleFieldList[id];
				try
				{
					int colId = fieldItem.FieldOrder + 1;

					int eleIndex = 0;
					for (int i = start_excel_row; i <= last_excel_row; i++)
					{
						if (this.isBlankRow(worksheet, i, fieldItem.TotalColumn))
							continue;

						//Get Element Id Cell
						int lastColId = last_excel_col;// fieldItem.TotalColumn + 1;
						if (lastColId == -1)
							lastColId = fieldItem.TotalColumn + 1;

						Excel.Range rangeId = worksheet.Cells[i, lastColId] as Excel.Range;
						Element ele = this.getElementById(eleList, rangeId.Text);
						if (ele == null)
							continue;

						Excel.Range range = worksheet.Cells[i, colId] as Excel.Range;

						BuiltInParameter enumParam = (BuiltInParameter)fieldItem.ParamId;
						String fieldName = fieldItem.FieldName;
						//Parameter p = eleList[0].LookupParameter(fieldName);
						//Parameter p = eleList[eleIndex].get_Parameter(enumParam);
						Parameter p = ele.get_Parameter(enumParam);
						if (p == null)
						{
							ElementType type = this.mDoc.GetElement(ele.GetTypeId()) as ElementType;
							p = type.get_Parameter(enumParam);
							//p = type.LookupParameter(fieldName);
						}

						if (p != null)
						{
							switch (p.StorageType)
							{
								case StorageType.Double:
									double valD = range.Value2 == null ? 0 : Convert.ToDouble(range.Value2);
									//p.Set(valD);
									p.SetValueString(valD.ToString().Replace(',', '.'));
									break;

								case StorageType.Integer:
									int valI = range.Value2 == null ? 0 : Convert.ToInt32(range.Value2);
									//p.Set(valI);
									p.SetValueString(valI.ToString());
									break;

								case StorageType.String:
									String valS = range.Value2 == null ? "" : Convert.ToString(range.Value2);
									p.Set(valS);
									break;

								case StorageType.ElementId:
									break;
							}
						}

						eleIndex++;

						this.appendTextWithColor("Element Id[" + rangeId.Text + "]: ", System.Drawing.Color.Black);
						this.appendTextWithColor("[DONE]" + Environment.NewLine, System.Drawing.Color.Green);

					}

					this.appendTextWithColor("Field[" + fieldItem.FieldName + "]: ", System.Drawing.Color.Black);
					this.appendTextWithColor("[DONE]" + Environment.NewLine, System.Drawing.Color.Green);

					//Update Progress Bar
					int percent = (j + 1) * 100 / this.lbScheduleFieldList.SelectedIndices.Count;
					this.pgbImport.Value = percent;

					System.Threading.Thread.Sleep(100);

				}
				catch (Exception ex)
				{
					this.appendTextWithColor(fieldItem.FieldName + ": ", System.Drawing.Color.Black);
					this.appendTextWithColor("[FAIL]" + Environment.NewLine, System.Drawing.Color.Red);
					this.appendTextWithColor(ex.ToString() + Environment.NewLine, System.Drawing.Color.Red);
				}

			}

			trans.Commit();
		}

		private void getScheduleFields(int pos)
		{
			HolderScheduleItem item = this.mScheduleList[pos];
			ViewSchedule vs = this.mDoc.GetElement(new ElementId(item.Id)) as ViewSchedule;

			List<Element> eleList = this.getElementsOfSchedule(vs);

			if (eleList.Count == 0)
			{
				if (this.bsScheduleField != null)
				{
					this.bsScheduleField.ResetBindings(false);
				}

				return;
			}


			//IList<SchedulableField> fields = vs.Definition.GetSchedulableFields();
			IList<ScheduleFieldId> fields = vs.Definition.GetFieldOrder();

			StringBuilder sb = new StringBuilder();

			int total_visible_fields = 0;
			foreach (ScheduleFieldId id in fields)
			{
				ScheduleField scheduleField = vs.Definition.GetField(id);
				if (!scheduleField.IsHidden)
					total_visible_fields++;
			}

			int k = 0;
			foreach (ScheduleFieldId id in fields)
			{
				ScheduleField scheduleField = vs.Definition.GetField(id);

				if (scheduleField.IsHidden)
					continue;

				// parameter visible in the UI < 0
				// shared or project parameter > 0
				//if (scheduleField.ParameterId.IntegerValue < 0){}

				BuiltInParameter enumParam = (BuiltInParameter)scheduleField.ParameterId.IntegerValue;
				String fieldName = scheduleField.GetName();
				//Parameter p = eleList[0].LookupParameter(fieldName);
				Parameter p = eleList[0].get_Parameter(enumParam);
				if (p == null)
				{
					ElementType type = this.mDoc.GetElement(eleList[0].GetTypeId()) as ElementType;
					if (type != null)
					{
						p = type.get_Parameter(enumParam);
					}

					if (p == null)
					{
						Parameter p2 = eleList[0].get_Parameter(BuiltInParameter.MATERIAL_ID_PARAM);
						//Parameter p2 = eleList[0].LookupParameter("Material");
						if (p2 != null)
						{
							if (p2.AsElementId() != null)
							{
								Material material = this.mDoc.GetElement(p2.AsElementId()) as Material;
								if (material != null)
									p = material.get_Parameter(enumParam);
							}

						}
					}

					//p = type.LookupParameter(fieldName);
				}

				bool isEnabled = false;
				if (p != null)
				{
					isEnabled = !p.IsReadOnly;
				}

				sb.AppendLine(scheduleField.ParameterId.IntegerValue + " | " + scheduleField.GetName() + " | " + isEnabled.ToString() + " | " + enumParam.ToString() + " | " + k);
				if (isEnabled)
					this.mScheduleFieldList.Add(new HolderFieldItem(scheduleField.ParameterId.IntegerValue, scheduleField.GetName(), k, total_visible_fields, isEnabled));
				k++;

			}

			if (this.bsScheduleField == null)
			{
				this.bsScheduleField = new BindingSource();
				this.bsScheduleField.DataSource = this.mScheduleFieldList;
				this.lbScheduleFieldList.DataSource = this.bsScheduleField;
				this.lbScheduleFieldList.DisplayMember = "FieldName";
				this.lbScheduleFieldList.ValueMember = "ParamId";

			}

			this.bsScheduleField.ResetBindings(false);


			if (this.mScheduleFieldList.Count > 0)
			{
				for (int i = 0; i < this.mScheduleFieldList.Count; i++)
				{
					this.lbScheduleFieldList.SetSelected(i, false);
				}

				this.lbScheduleFieldList.SetSelected(this.mScheduleFieldList.Count - 1, true);
			}

			//MessageBox.Show(sb.ToString());

		}

		private List<Element> getElementsOfSchedule(ViewSchedule vs)
		{
			//Category of the elements in the schedule
			BuiltInCategory enumCategory = (BuiltInCategory)vs.Definition.CategoryId.IntegerValue;

			FilteredElementCollector eleCollector = new FilteredElementCollector(this.mDoc, vs.Id);
			eleCollector.WhereElementIsNotElementType();
			eleCollector.WhereElementIsViewIndependent();
			eleCollector.OfCategory(enumCategory);
			List<Element> list = eleCollector.ToElements() as List<Element>;

			StringBuilder sb = new StringBuilder();
			foreach (Element e in list)
			{
				sb.AppendLine(Util.ElementUtil.ElementInfo(e));
			}

			//MessageBox.Show(sb.ToString());

			return list;
		}

		private Element getElementById(IList<Element> pElementList, String pId)
		{
			if (pElementList.Count == 0 || pId == String.Empty)
				return null;

			foreach (Element ele in pElementList)
			{
				if (ele.Id.IntegerValue.ToString() == pId)
					return ele;
			}

			return null;
		}

		private int findLastColumnByVal(Excel.Worksheet ws, int row, String val)
		{
			for (int j = 0; j < 27; j++)
			{
				Excel.Range range = ws.Cells[row, j + 1] as Excel.Range;
				if (range.Value2 == val)
				{
					return j + 1;
				}
			}

			return -1;
		}

		private bool isBlankRow(Excel.Worksheet ws, int row, int totalCol)
		{
			bool blank = true;
			for (int j = 0; j < totalCol; j++)
			{
				Excel.Range range = ws.Cells[row, j + 1] as Excel.Range;
				if (range.Value2 != null)
				{
					blank = false;
					break;
				}
			}

			return blank;
		}

		private void appendTextWithColor(String text, System.Drawing.Color color)
		{
			this.rtbLog.Select(this.rtbLog.TextLength, 0);
			this.rtbLog.SelectionColor = color;
			this.rtbLog.AppendText(text);
		}

		private void btnSelectExcel_Click(object sender, EventArgs e)
		{
			MessageBox.Show("Please select the Excel file has a column named [Element_Unique_Id]. If not, the data may not be changed.", "Warning!");
			this.openFileDialogExcel.Filter = "Excel Files (*.xlsx)|*.xlsx";
			System.Windows.Forms.DialogResult dr = this.openFileDialogExcel.ShowDialog();
			if (dr == System.Windows.Forms.DialogResult.OK)
			{
				this.mExcelFile = this.openFileDialogExcel.FileName;

				this.initExcelSheetListBox(this.mExcelFile);
			}
		}

		private void btnFillData_Click(object sender, EventArgs e)
		{
			if (this.lbExcelSheet.Items.Count == 0)
			{
				MessageBox.Show("The Excel Sheet List can not be empty.");
				return;
			}

			if (this.mScheduleList.Count == 0)
			{
				MessageBox.Show("Can not find any schedule.");
				return;
			}

			if (this.mScheduleFieldList.Count == 0)
			{
				MessageBox.Show("Can not find any editable field.");
				return;
			}

			DialogResult dr = MessageBox.Show(this, "Are you ready?", "Confirm Dialog", MessageBoxButtons.YesNo);
			if (dr == System.Windows.Forms.DialogResult.No)
				return;

			this.grbExcel.Enabled = false;
			this.grbSchedule.Enabled = false;

			this.import2Schedule();

			this.grbExcel.Enabled = true;
			this.grbSchedule.Enabled = true;
		}

		private void lbScheduleList_SelectedIndexChanged(object sender, EventArgs e)
		{
			this.mScheduleFieldList.Clear();
			this.getScheduleFields(this.lbScheduleList.SelectedIndex);
		}

		private void lbScheduleFieldList_SelectedIndexChanged(object sender, EventArgs e)
		{

		}
	}

	public class ListBoxItem
	{
		public bool Enable { get; set; }
		public string Text { get; set; }
		public String Value { get; set; }

		public ListBoxItem()
		{
			Enable = true;
		}


		public override string ToString()
		{
			return Text;
		}
	}
}
