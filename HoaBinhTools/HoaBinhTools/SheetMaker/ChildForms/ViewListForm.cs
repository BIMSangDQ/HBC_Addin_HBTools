using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Windows.Forms;

using Autodesk.Revit.DB;
using SheetDuplicateAndAlignView.Forms;

namespace SheetDuplicateAndAlignView.ChildForms
{
	public partial class ViewListForm : System.Windows.Forms.Form
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

		private FilterViewOption mViewOption;
		public FilterViewOption ViewOption
		{
			get { return this.mViewOption; }
			set { this.mViewOption = value; }
		}

		private List<ViewItem> mViewList;

		private List<SheetItem> mSelectedSheetItemList;
		public List<SheetItem> SelectedSheetItemList
		{
			set { this.mSelectedSheetItemList = value; }
		}

		private List<ViewItem> mSelectedViewList;

		public List<ViewItem> SelectedViewList
		{
			get
			{
				this.mSelectedViewList.Clear();
				foreach (ViewItem item in this.lbSelectedViewList.Items)
				{
					this.mSelectedViewList.Add(item);
				}

				return this.mSelectedViewList;
			}
		}

		private bool mIsApplyAll;
		public bool IsApplyAll
		{
			set { this.mIsApplyAll = value; }
			get { return this.mIsApplyAll; }
		}

		public ViewListForm()
		{
			InitializeComponent();

			this.mViewOption = FilterViewOption.VIEW;
			this.mViewList = new List<ViewItem>();
			this.mSelectedViewList = new List<ViewItem>();
			this.mSelectedSheetItemList = new List<SheetItem>();
			this.mIsApplyAll = false;
		}

		private void ViewListForm_Load(object sender, EventArgs e)
		{
			this.initFilterView();
			this.initSelectedViewList();
			this.initViewList();


			if (this.mSelectedSheetItemList.Count > 0)
			{
				this.Text = "Selected Sheet: " + this.mSelectedSheetItemList[0].ToString();
				this.toolStripStatusSelectedSheet.Text = this.Text;
			}
		}

		private void initFilterView()
		{

			switch (this.mViewOption)
			{
				case FilterViewOption.VIEW:

					this.Text = "View List";

					this.chkView.Checked = true;
					this.chkSchedule.Checked = false;
					this.chkLegend.Checked = false;

					this.chkView.Enabled = false;
					this.chkSchedule.Enabled = false;
					this.chkLegend.Enabled = false;

					break;

				case FilterViewOption.SCHEDULE:

					this.Text = "Schedule List";

					this.chkView.Checked = false;
					this.chkSchedule.Checked = true;
					this.chkLegend.Checked = false;

					this.chkView.Enabled = false;
					this.chkSchedule.Enabled = false;
					this.chkLegend.Enabled = false;
					break;

				case FilterViewOption.LEGEND:

					this.Text = "Legend List";

					this.chkView.Checked = false;
					this.chkSchedule.Checked = false;
					this.chkLegend.Checked = true;

					this.chkView.Enabled = false;
					this.chkSchedule.Enabled = false;
					this.chkLegend.Enabled = false;
					break;


			}


		}

		private void initSelectedViewList()
		{
			this.lbSelectedViewList.Items.Clear();
			this.lbSelectedViewList.Sorted = true;

			if (this.chkView.Checked)
			{
				foreach (ViewItem item in this.mSelectedSheetItemList[0].ViewList)
				{
					this.lbSelectedViewList.Items.Add(item);
				}
			}
			else if (this.chkLegend.Checked)
			{
				if (!this.mIsApplyAll)
				{
					foreach (ViewItem item in this.mSelectedSheetItemList[0].LegendList)
					{
						this.lbSelectedViewList.Items.Add(item);
					}
				}
			}
			else if (this.chkSchedule.Checked)
			{
				if (!this.mIsApplyAll)
				{
					foreach (ViewItem item in this.mSelectedSheetItemList[0].ScheduleList)
					{
						this.lbSelectedViewList.Items.Add(item);
					}
				}
			}

		}

		private void initViewList()
		{
			this.getAllViews();

			foreach (ViewItem item in this.mViewList)
			{
				if (this.checkViewExistInSelectedList(item))
					continue;

				this.lbViewList.Items.Add(item);
			}

			this.lbViewList.Sorted = true;


		}

		private bool checkViewExistInSelectedList(ViewItem pItem)
		{
			foreach (ViewItem ele in this.lbSelectedViewList.Items)
			{
				if (ele.Value.Id.Equals(pItem.Value.Id))
					return true;
			}

			return false;
		}

		private void getAllViews()
		{
			this.mViewList.Clear();
			if (chkView.Checked || chkLegend.Checked)
			{
				FilteredElementCollector col = new FilteredElementCollector(this.mDoc);
				col.OfCategory(BuiltInCategory.OST_Views);

				foreach (Element e in col.ToElements())
				{
					Autodesk.Revit.DB.View v = e as Autodesk.Revit.DB.View;
					if (v.IsTemplate)
						continue;

					ViewItem item = null;

					if (this.chkView.Checked)
					{
						if (v.ViewType == ViewType.Legend || v.ViewType == ViewType.Schedule)
							continue;
						item = new ViewItem();
						item.Text = v.Name;
						item.Value = v;

						this.mViewList.Add(item);
					}

					if (this.chkLegend.Checked)
					{
						if (v.ViewType == ViewType.Legend)
						{
							item = new ViewItem();
							item.Text = v.Name;
							item.Value = v;
							this.mViewList.Add(item);
						}
					}
				}
			}
			else if (chkSchedule.Checked)
			{
				FilteredElementCollector col = new FilteredElementCollector(this.mDoc);
				col.OfClass(typeof(ViewSchedule));

				foreach (Element e in col.ToElements())
				{
					Autodesk.Revit.DB.View v = e as Autodesk.Revit.DB.View;
					if (v.ViewType == ViewType.Schedule)
					{
						ViewItem item = new ViewItem();
						item.Text = v.Name;
						item.Value = v;

						this.mViewList.Add(item);
					}
				}
			}

		}

		private void addLeftToRight()
		{

			if (this.lbViewList.SelectedIndices.Count > 0)
			{

				List<Object> tmp = new List<Object>();
				for (int i = 0; i < this.lbViewList.SelectedIndices.Count; i++)
				{
					int idx = this.lbViewList.SelectedIndices[i];

					this.lbSelectedViewList.Items.Add(this.lbViewList.Items[idx]);
					tmp.Add(this.lbViewList.Items[idx]);
				}

				for (int j = 0; j < tmp.Count; j++)
				{
					this.lbViewList.Items.Remove(tmp[j]);
				}

			}
		}

		private void addRightToLeft()
		{

			if (this.lbSelectedViewList.SelectedIndices.Count > 0)
			{
				List<Object> tmp = new List<Object>();
				for (int i = 0; i < this.lbSelectedViewList.SelectedIndices.Count; i++)
				{
					int idx = this.lbSelectedViewList.SelectedIndices[i];
					this.lbViewList.Items.Add(this.lbSelectedViewList.Items[idx]);
					tmp.Add(this.lbSelectedViewList.Items[idx]);
				}

				for (int j = 0; j < tmp.Count; j++)
				{
					this.lbSelectedViewList.Items.Remove(tmp[j]);
				}
			}
		}


		private void txtSearch_TextChanged(object sender, EventArgs e)
		{
			this.lbViewList.Items.Clear();

			if (String.IsNullOrEmpty(this.txtSearch.Text))
			{
				foreach (ViewItem v in this.mViewList)
				{
					if (this.lbSelectedViewList.Items.Contains(v))
						continue;

					this.lbViewList.Items.Add(v);
				}
			}
			else
			{
				string q = this.txtSearch.Text.ToLower();
				var sql = from item in this.mViewList
						  where item.Text.ToLower().Contains(q) || item.Text.ToLower().Contains(q)
						  select item;

				foreach (ViewItem v in sql.ToList())
				{
					//Don't add the items are already in the selected list
					if (this.lbSelectedViewList.Items.Contains(v))
						continue;

					this.lbViewList.Items.Add(v);
				}
			}
		}

		private void lbViewList_MouseDoubleClick(object sender, MouseEventArgs e)
		{
			this.addLeftToRight();
		}

		private void lbSelectedViewList_MouseDoubleClick(object sender, MouseEventArgs e)
		{
			this.addRightToLeft();
		}

		private void btnAdd_Click(object sender, EventArgs e)
		{
			this.addLeftToRight();
		}

		private void btnRemove_Click(object sender, EventArgs e)
		{
			this.addRightToLeft();
		}

	}
}
