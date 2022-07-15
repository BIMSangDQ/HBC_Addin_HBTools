using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Windows.Forms;

using Autodesk.Revit.DB;

namespace SheetDuplicateAndAlignView.Forms
{
	public partial class AlignViewForm : System.Windows.Forms.Form
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

		private List<SheetViewPortItem> mAlignViewList;
		private List<SheetViewPortItem> mSelectedAlignViewList;

		public AlignViewForm()
		{
			InitializeComponent();

			this.mAlignViewList = new List<SheetViewPortItem>();
			this.mSelectedAlignViewList = new List<SheetViewPortItem>();
		}

		private void AlignViewForm_Load(object sender, EventArgs e)
		{
			this.initComboBoxPrimaryView();
			this.initComboBoxAlignPosition();

			this.initAlignViewData();
		}

		private void initComboBoxPrimaryView()
		{
			this.cboPrimaryView.Items.Clear();
			this.lbAlignedView.Items.Clear();

			List<Viewport> vpList = this.getAllViewports();
			List<ScheduleSheetInstance> scheduleList = this.getAllViewSchedules();

			foreach (Viewport vp in vpList)
			{
				ViewSheet vs = this.mDoc.GetElement(vp.SheetId) as ViewSheet;
				Autodesk.Revit.DB.View v = this.mDoc.GetElement(vp.ViewId) as Autodesk.Revit.DB.View;

				SheetViewPortItem item = new SheetViewPortItem();
				item.Value = vp;
				item.ViewName = v.Name;
				item.SheetNumber = vs.SheetNumber;
				item.isViewPort = true;

				this.cboPrimaryView.Items.Add(item);
				this.mAlignViewList.Add(item);
			}

			foreach (ScheduleSheetInstance si in scheduleList)
			{
				ViewSheet vs = this.mDoc.GetElement(si.OwnerViewId) as ViewSheet;
				ViewSchedule v = this.mDoc.GetElement(si.ScheduleId) as ViewSchedule;

				SheetViewPortItem item = new SheetViewPortItem();
				item.Value = si;
				item.ViewName = v.Name;
				item.SheetNumber = vs.SheetNumber;
				item.isViewPort = false;

				this.cboPrimaryView.Items.Add(item);
				this.mAlignViewList.Add(item);
			}



			this.cboPrimaryView.Sorted = true;
			this.cboPrimaryView.AutoCompleteMode = AutoCompleteMode.SuggestAppend;
			this.cboPrimaryView.AutoCompleteSource = AutoCompleteSource.ListItems;

		}

		private void initAlignViewData()
		{
			foreach (SheetViewPortItem ele in this.mAlignViewList)
			{
				this.lbAlignedView.Items.Add(ele);
			}

			this.lbAlignedView.Sorted = true;
			this.lbSelectedAlignedView.Sorted = true;
		}

		private void initComboBoxAlignPosition()
		{
			this.cboAlignPosition.Items.Clear();
			AlignTypeItem item1 = new AlignTypeItem();
			item1.Text = "Top Left";
			item1.Value = AlignType.TOP_LEFT;
			this.cboAlignPosition.Items.Add(item1);

			AlignTypeItem item2 = new AlignTypeItem();
			item2.Text = "Top Right";
			item2.Value = AlignType.TOP_RIGHT;
			this.cboAlignPosition.Items.Add(item2);

			AlignTypeItem item3 = new AlignTypeItem();
			item3.Text = "Center";
			item3.Value = AlignType.CENTER;
			this.cboAlignPosition.Items.Add(item3);

			AlignTypeItem item4 = new AlignTypeItem();
			item4.Text = "Bottom Left";
			item4.Value = AlignType.BOTTOM_LEFT;
			this.cboAlignPosition.Items.Add(item4);

			AlignTypeItem item5 = new AlignTypeItem();
			item5.Text = "Bottom Right";
			item5.Value = AlignType.BOTTOM_RIGHT;
			this.cboAlignPosition.Items.Add(item5);

			this.cboAlignPosition.SelectedIndex = 2;

			this.cboAlignPosition.AutoCompleteMode = AutoCompleteMode.SuggestAppend;
			this.cboAlignPosition.AutoCompleteSource = AutoCompleteSource.ListItems;
		}

		private List<ViewSheet> getAllSheets()
		{
			FilteredElementCollector col = new FilteredElementCollector(this.mDoc);
			col.OfCategory(BuiltInCategory.OST_Sheets);

			List<ViewSheet> list = new List<ViewSheet>();

			foreach (Element e in col.ToElements())
			{
				list.Add((e as ViewSheet));
			}

			return list;
		}

		private List<Viewport> getAllViewports()
		{
			FilteredElementCollector col = new FilteredElementCollector(this.mDoc);
			col.OfCategory(BuiltInCategory.OST_Viewports);

			List<Viewport> list = new List<Viewport>();

			foreach (Element e in col.ToElements())
			{
				list.Add((e as Viewport));
			}

			return list;
		}

		private List<ScheduleSheetInstance> getAllViewSchedules()
		{
			FilteredElementCollector col = new FilteredElementCollector(this.mDoc);
			col.OfClass(typeof(ScheduleSheetInstance));

			List<ScheduleSheetInstance> list = new List<ScheduleSheetInstance>();

			foreach (Element e in col.ToElements())
			{
				list.Add((e as ScheduleSheetInstance));
			}

			return list;
		}

		private void addLeftToRight()
		{
			if (this.cboPrimaryView.SelectedIndex == -1)
			{
				MessageBox.Show("Please select the primary view first.");
				return;
			}

			if (this.lbAlignedView.SelectedIndices.Count > 0)
			{
				SheetViewPortItem primaryItem = this.cboPrimaryView.SelectedItem as SheetViewPortItem;

				List<Object> tmp = new List<Object>();
				for (int i = 0; i < this.lbAlignedView.SelectedIndices.Count; i++)
				{
					int idx = this.lbAlignedView.SelectedIndices[i];
					if (primaryItem.Value.Id.Equals((this.lbAlignedView.Items[idx] as SheetViewPortItem).Value.Id))
						continue;

					this.lbSelectedAlignedView.Items.Add(this.lbAlignedView.Items[idx]);
					tmp.Add(this.lbAlignedView.Items[idx]);
				}

				for (int j = 0; j < tmp.Count; j++)
				{
					this.lbAlignedView.Items.Remove(tmp[j]);
				}

			}
		}

		private void addRightToLeft()
		{

			if (this.lbSelectedAlignedView.SelectedIndices.Count > 0)
			{
				List<Object> tmp = new List<Object>();
				for (int i = 0; i < this.lbSelectedAlignedView.SelectedIndices.Count; i++)
				{
					int idx = this.lbSelectedAlignedView.SelectedIndices[i];
					this.lbAlignedView.Items.Add(this.lbSelectedAlignedView.Items[idx]);
					tmp.Add(this.lbSelectedAlignedView.Items[idx]);
				}

				for (int j = 0; j < tmp.Count; j++)
				{
					this.lbSelectedAlignedView.Items.Remove(tmp[j]);
				}
			}
		}

		private void appendTextWithColor(String text, System.Drawing.Color color)
		{
			this.rtbLog.Select(this.rtbLog.TextLength, 0);
			this.rtbLog.SelectionColor = color;
			this.rtbLog.AppendText(text);
		}

		private void applyAlignView()
		{
			//Get the selected primary Viewport
			SheetViewPortItem primaryItem = this.cboPrimaryView.SelectedItem as SheetViewPortItem;
			AlignTypeItem typeItem = this.cboAlignPosition.SelectedItem as AlignTypeItem;

			if (primaryItem.isViewPort)
			{
				Viewport primaryView = primaryItem.Value as Viewport;

				XYZ primaryCenterPoint = primaryView.GetBoxCenter();
				Outline primaryOutline = primaryView.GetBoxOutline();

				Transaction trans = new Transaction(this.mDoc, "ALIGN VIEWPORT");
				string itemTextName = "";
				try
				{
					for (int i = 0; i < this.lbSelectedAlignedView.Items.Count; i++)
					{
						SheetViewPortItem item = this.lbSelectedAlignedView.Items[i] as SheetViewPortItem;
						if (!item.isViewPort)
							continue;

						Viewport curViewport = item.Value as Viewport;
						if (curViewport.Id.Equals(primaryView.Id))
							continue;

						itemTextName = item.ToString();
						trans.Start();

						Outline curViewportOutline = curViewport.GetBoxOutline();
						XYZ alignedPoint = primaryCenterPoint;
						XYZ p1 = null;
						XYZ p2 = null;
						switch (typeItem.Value)
						{
							case AlignType.CENTER:
								alignedPoint = primaryCenterPoint;
								break;

							case AlignType.TOP_LEFT:
								p1 = new XYZ(primaryOutline.MinimumPoint.X, primaryOutline.MaximumPoint.Y, primaryOutline.MaximumPoint.Z);
								p2 = new XYZ(curViewportOutline.MinimumPoint.X, curViewportOutline.MaximumPoint.Y, curViewportOutline.MaximumPoint.Z);
								alignedPoint = curViewport.GetBoxCenter().Subtract(p2.Subtract(p1));
								break;

							case AlignType.TOP_RIGHT:
								p1 = primaryOutline.MaximumPoint;
								p2 = curViewportOutline.MaximumPoint;
								alignedPoint = curViewport.GetBoxCenter().Add(p1.Subtract(p2));
								break;

							case AlignType.BOTTOM_LEFT:
								p1 = primaryOutline.MinimumPoint;
								p2 = curViewportOutline.MinimumPoint;
								alignedPoint = curViewport.GetBoxCenter().Subtract(p2.Subtract(p1));
								break;

							case AlignType.BOTTOM_RIGHT:
								p1 = new XYZ(primaryOutline.MaximumPoint.X, primaryOutline.MinimumPoint.Y, primaryOutline.MaximumPoint.Z);
								p2 = new XYZ(curViewportOutline.MaximumPoint.X, curViewportOutline.MinimumPoint.Y, curViewportOutline.MaximumPoint.Z);
								alignedPoint = curViewport.GetBoxCenter().Add(p1.Subtract(p2));
								break;
						}

						//Set the new position
						curViewport.SetBoxCenter(alignedPoint);
						trans.Commit();

						this.appendTextWithColor(itemTextName + ": ", System.Drawing.Color.Black);
						this.appendTextWithColor("[DONE]" + Environment.NewLine, System.Drawing.Color.Green);
					}
				}
				catch (Exception ex)
				{
					this.appendTextWithColor(itemTextName + ": ", System.Drawing.Color.Black);
					this.appendTextWithColor("[FAILED]" + Environment.NewLine, System.Drawing.Color.Red);
					this.appendTextWithColor(ex.ToString() + Environment.NewLine, System.Drawing.Color.Red);
					trans.RollBack();
				}
			}
			else
			{
				ScheduleSheetInstance primaryView = primaryItem.Value as ScheduleSheetInstance;
				XYZ primaryCenterPoint = primaryView.Point;
				BoundingBoxXYZ primaryBB = primaryView.get_BoundingBox(null);

				Transaction trans = new Transaction(this.mDoc, "ALIGN SCHEDULE");
				string itemTextName = "";

				try
				{
					for (int i = 0; i < this.lbSelectedAlignedView.Items.Count; i++)
					{
						SheetViewPortItem item = this.lbSelectedAlignedView.Items[i] as SheetViewPortItem;
						if (item.isViewPort)
							continue;

						ScheduleSheetInstance curSchedule = item.Value as ScheduleSheetInstance;
						if (curSchedule.Id.Equals(primaryView.Id))
							continue;

						itemTextName = item.ToString();
						trans.Start();

						BoundingBoxXYZ curScheduleBB = curSchedule.get_BoundingBox(null);
						XYZ alignedPoint = primaryCenterPoint;
						XYZ p1 = null;
						XYZ p2 = null;
						switch (typeItem.Value)
						{
							case AlignType.CENTER:
								alignedPoint = primaryCenterPoint;
								break;

							case AlignType.TOP_LEFT:
								p1 = new XYZ(primaryBB.Min.X, primaryBB.Max.Y, primaryBB.Max.Z);
								p2 = new XYZ(curScheduleBB.Min.X, curScheduleBB.Max.Y, curScheduleBB.Max.Z);
								alignedPoint = curSchedule.Point.Subtract(p2.Subtract(p1));
								break;

							case AlignType.TOP_RIGHT:
								p1 = primaryBB.Max;
								p2 = curScheduleBB.Max;
								alignedPoint = curSchedule.Point.Add(p1.Subtract(p2));
								break;

							case AlignType.BOTTOM_LEFT:
								p1 = primaryBB.Min;
								p2 = curScheduleBB.Min;
								alignedPoint = curSchedule.Point.Subtract(p2.Subtract(p1));
								break;

							case AlignType.BOTTOM_RIGHT:
								p1 = new XYZ(primaryBB.Max.X, primaryBB.Min.Y, primaryBB.Max.Z);
								p2 = new XYZ(curScheduleBB.Max.X, curScheduleBB.Min.Y, curScheduleBB.Max.Z);
								alignedPoint = curSchedule.Point.Add(p1.Subtract(p2));
								break;
						}
						curSchedule.Point = alignedPoint;
						trans.Commit();

						this.appendTextWithColor(itemTextName + ": ", System.Drawing.Color.Black);
						this.appendTextWithColor("[DONE]" + Environment.NewLine, System.Drawing.Color.Green);
					}
				}
				catch (Exception ex)
				{
					this.appendTextWithColor(itemTextName + ": ", System.Drawing.Color.Black);
					this.appendTextWithColor("[FAILED]" + Environment.NewLine, System.Drawing.Color.Red);
					this.appendTextWithColor(ex.ToString() + Environment.NewLine, System.Drawing.Color.Red);
					trans.RollBack();
				}
			}
		}

		private void resetSelectedAlignedView()
		{
			this.lbSelectedAlignedView.Items.Clear();
			this.lbAlignedView.Items.Clear();

			foreach (SheetViewPortItem ele in this.mAlignViewList)
			{
				if (this.cboPrimaryView.SelectedIndex > -1 && this.cboPrimaryView.SelectedItem.Equals(ele))
					continue;

				this.lbAlignedView.Items.Add(ele);
			}


		}

		private void btnRemoveSelected_Click(object sender, EventArgs e)
		{
			this.addRightToLeft();
		}

		private void btnAddSelected_Click(object sender, EventArgs e)
		{
			this.addLeftToRight();
		}

		private void cboPrimaryView_SelectedIndexChanged(object sender, EventArgs e)
		{
			this.txtAlignViewSearch.Text = string.Empty;
			this.resetSelectedAlignedView();
		}

		private void txtAlignViewSearch_TextChanged(object sender, EventArgs e)
		{
			if (this.cboPrimaryView.SelectedIndex == -1)
			{
				MessageBox.Show("Please select the primary view first.");
				return;
			}

			this.lbAlignedView.Items.Clear();

			//If search box is empty, load all item to listbox
			if (String.IsNullOrEmpty(this.txtAlignViewSearch.Text))
			{
				foreach (SheetViewPortItem ele in this.mAlignViewList)
				{
					if (this.lbSelectedAlignedView.Items.Contains(ele) || this.cboPrimaryView.SelectedItem.Equals(ele))
						continue;

					this.lbAlignedView.Items.Add(ele);
				}
			}
			else
			{
				//Matching the search text with the items in source list
				string q = this.txtAlignViewSearch.Text.ToLower();
				var sql = from item in this.mAlignViewList
						  where item.SheetNumber.ToLower().Contains(q) || item.ViewName.ToLower().Contains(q)
						  select item;

				//and add them to the listbox
				foreach (SheetViewPortItem ele in sql.ToList())
				{
					//Don't add the items are already in the selected list and the selected primary view
					if (this.lbSelectedAlignedView.Items.Contains(ele) || this.cboPrimaryView.SelectedItem.Equals(ele))
						continue;

					this.lbAlignedView.Items.Add(ele);
				}
			}
		}

		private void lbAlignedView_MouseDoubleClick(object sender, MouseEventArgs e)
		{
			this.addLeftToRight();
		}

		private void lbSelectedAlignedView_MouseDoubleClick(object sender, MouseEventArgs e)
		{
			this.addRightToLeft();
		}

		private void btnApply_Click(object sender, EventArgs e)
		{
			if (this.cboPrimaryView.SelectedIndex == -1)
			{
				MessageBox.Show("Please select the primary view.");
				return;
			}

			if (this.lbSelectedAlignedView.Items.Count == 0)
			{
				MessageBox.Show("Please add the views to be aligned from the left to the right list.");
				return;
			}

			if (this.cboAlignPosition.SelectedIndex == -1)
			{
				MessageBox.Show("Please select the align position.");
				return;
			}

			if (MessageBox.Show("Are you ready?", "Confirm", MessageBoxButtons.YesNo) == System.Windows.Forms.DialogResult.No)
			{
				return;
			}

			try
			{
				this.cboPrimaryView.Enabled = false;

				this.lbAlignedView.Enabled = false;
				this.lbSelectedAlignedView.Enabled = false;
				this.btnAddSelected.Enabled = false;
				this.btnRemoveSelected.Enabled = false;

				this.cboAlignPosition.Enabled = false;

				this.btnClose.Enabled = false;
				this.btnApply.Enabled = false;

				this.rtbLog.Text = "";
				//Apply the aligned
				this.applyAlignView();
			}
			catch (Exception ex)
			{
				this.appendTextWithColor("[ERROR]" + Environment.NewLine, System.Drawing.Color.Red);
				this.appendTextWithColor(ex.ToString() + Environment.NewLine, System.Drawing.Color.Red);
			}
			finally
			{
				this.cboPrimaryView.Enabled = true;

				this.lbAlignedView.Enabled = true;
				this.lbSelectedAlignedView.Enabled = true;
				this.btnAddSelected.Enabled = true;
				this.btnRemoveSelected.Enabled = true;

				this.cboAlignPosition.Enabled = true;

				this.btnClose.Enabled = true;
				this.btnApply.Enabled = true;
			}
		}
	}

	public class SheetViewPortItem
	{
		public string SheetNumber { get; set; }
		public String ViewName { get; set; }
		public Element Value { get; set; }
		public bool isViewPort { get; set; }

		public SheetViewPortItem() { }

		public override string ToString()
		{
			return ViewName + ", " + SheetNumber;
		}
	}

	public class AlignTypeItem
	{

		public string Text { get; set; }

		public AlignType Value { get; set; }

		public AlignTypeItem() { }

		public override string ToString()
		{
			return Text;
		}
	}

	public enum AlignType
	{
		TOP_LEFT = 1,
		TOP_RIGHT = 2,
		CENTER = 3,
		BOTTOM_LEFT = 4,
		BOTTOM_RIGHT = 5
	}
}
