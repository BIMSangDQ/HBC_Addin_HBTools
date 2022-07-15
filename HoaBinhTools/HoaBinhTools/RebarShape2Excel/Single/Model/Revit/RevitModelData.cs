using System.Collections.Generic;
using OfficeOpenXml;

namespace SingleData
{
	public partial class RevitModelData : Utility.NotifyClass
	{
		public static RevitModelData Instance { get { return Singleton.Instance.ModelData.RevitModelData; } }
		#region Variables
		private Model.Form.Schedule2ExcelForm form;
		private List<Model.Entity.RebarShapeInfo> rebarShapeInfos;
		#endregion

		#region Properties
		public bool IsCreateNewParameter { get; set; } = false;
		public Autodesk.Revit.DB.Definition Definition { get; set; }
		public Model.Form.Schedule2ExcelForm Form
		{
			get
			{
				if (form == null) form = new Model.Form.Schedule2ExcelForm();
				return form;
			}
		}
		public List<Model.Entity.RebarShapeInfo> RebarShapeInfos
		{
			get
			{
				if (rebarShapeInfos == null) rebarShapeInfos = new List<Model.Entity.RebarShapeInfo>();
				return rebarShapeInfos;
			}
		}

		private List<Model.Entity.Element> elements;
		public virtual List<Model.Entity.Element> Elements
		{
			get { if (elements == null) elements = new List<Model.Entity.Element>(); return elements; }
		}
		public virtual List<Model.Entity.CategoryExtension> CategoryExtensions { get; set; } = new List<Model.Entity.CategoryExtension>();

		public List<string> ExcludeSquareStirrupShapeNames { get; set; }
		   = new List<string> { "TD_02", "TD_03", "TD_04" };

		public List<string> ExcludeAntiTwistingSquareStirrupShapeNames { get; set; }
		   = new List<string> { "TD_CX_03" };

		public string EditValueElementInGroupWarningName { get; set; }
			= "A group has been changed outside group edit mode";

		public List<string> DeleteGuids { get; set; } = new List<string>();

		private List<Model.Entity.ScheduleColumn> scheduleColumns;
		public List<Model.Entity.ScheduleColumn> ScheduleColumns
		{
			get
			{
				if (scheduleColumns == null) scheduleColumns = new List<Model.Entity.ScheduleColumn>();
				return scheduleColumns;
			}
			set
			{
				scheduleColumns = value;
			}
		}

		public int ProjectRow { get; set; } = 3;
		public int TitleRow { get; set; } = 5;
		public int HeaderRow { get; set; } = 7;
		public int BodyLastRow { get; set; }
		public ExcelWorksheet CurrentExcelWorksheet { get; set; }
		public int ShapeColumn { get; set; }

		public int LongNameWorkSheetIndex { get; set; } = 1;
		#endregion
	}
}