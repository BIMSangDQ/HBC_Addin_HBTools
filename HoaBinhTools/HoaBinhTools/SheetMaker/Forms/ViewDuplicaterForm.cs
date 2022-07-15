using System;
using System.Collections.Generic;
using System.Windows.Forms;

using Autodesk.Revit.DB;

namespace SheetDuplicateAndAlignView.Forms
{
	public partial class ViewDuplicaterForm : System.Windows.Forms.Form
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

		private List<Autodesk.Revit.DB.View> mSelectedViews;
		public List<Autodesk.Revit.DB.View> SelectedViews
		{
			set { mSelectedViews = value; }
		}


		public ViewDuplicaterForm()
		{
			InitializeComponent();
		}

		private void ViewDuplicateForm_Load(object sender, EventArgs e)
		{
			this.toolStripStatusSelectedView.Text += Util.ElementUtil.ViewElementInfo(this.mSelectedViews[0]);
			this.initDuplicateViewComboBox();
			this.initViewTemplateComboBox();
			this.initScopeBoxList();

			this.nudViewQuantity.Enabled = false;
			this.nudStartNumber.Enabled = false;
		}

		private void initDuplicateViewComboBox()
		{
			this.cboDuplicateView.Items.Clear();
			ComboBoxDuplicateItem item1 = new ComboBoxDuplicateItem();
			item1.Text = "Duplicate";
			item1.Value = DuplicateViewType.DUPLICATE;
			this.cboDuplicateView.Items.Add(item1);

			ComboBoxDuplicateItem item2 = new ComboBoxDuplicateItem();
			item2.Text = "Duplicate With Detailing";
			item2.Value = DuplicateViewType.DUPLICATE_WITH_DETAILING;
			this.cboDuplicateView.Items.Add(item2);

			ComboBoxDuplicateItem item3 = new ComboBoxDuplicateItem();
			item3.Text = "Duplicate As a Dependent";
			item3.Value = DuplicateViewType.DUPLICATE_AS_DEPENDENT;
			this.cboDuplicateView.Items.Add(item3);

			this.cboDuplicateView.SelectedIndex = 0;
			this.cboDuplicateView.AutoCompleteMode = AutoCompleteMode.SuggestAppend;
			this.cboDuplicateView.AutoCompleteSource = AutoCompleteSource.ListItems;
		}

		private void initViewTemplateComboBox()
		{
			this.cboViewTemplate.Items.Clear();

			List<Autodesk.Revit.DB.View> templateList = this.getAllViewTemplate();

			foreach (Autodesk.Revit.DB.View tv in templateList)
			{
				ComboBoxViewTemplateItem item = new ComboBoxViewTemplateItem();
				item.Text = tv.Name;
				item.Value = tv.Id;

				this.cboViewTemplate.Items.Add(item);
			}

			this.cboViewTemplate.AutoCompleteMode = AutoCompleteMode.SuggestAppend;
			this.cboViewTemplate.AutoCompleteSource = AutoCompleteSource.ListItems;
		}

		private void initScopeBoxList()
		{
			this.lbScopeBox.Items.Clear();

			List<Element> scopeboxList = this.getAllScopeBox();

			foreach (Element e in scopeboxList)
			{
				ListBoxScopeBoxItem item = new ListBoxScopeBoxItem();
				item.Text = e.Name;
				item.Value = e.Id;
				this.lbScopeBox.Items.Add(item);
			}

			this.lbScopeBox.Sorted = true;
			this.lbSelectedScopeBox.Sorted = true;
		}

		private List<Autodesk.Revit.DB.View> getAllViewTemplate()
		{
			FilteredElementCollector col = new FilteredElementCollector(this.mDoc);
			col.OfCategory(BuiltInCategory.OST_Views);

			List<Autodesk.Revit.DB.View> list = new List<Autodesk.Revit.DB.View>();

			foreach (Element e in col.ToElements())
			{
				Autodesk.Revit.DB.View v = e as Autodesk.Revit.DB.View;
				if (v.IsTemplate)
					list.Add(v);
			}

			return list;

		}

		private List<Element> getAllScopeBox()
		{
			FilteredElementCollector col = new FilteredElementCollector(this.mDoc);
			col.OfCategory(BuiltInCategory.OST_VolumeOfInterest);

			List<Element> list = new List<Element>();
			foreach (Element e in col.ToElements())
			{
				list.Add(e);
			}

			return list;
		}

		private bool checkViewCanBeDuplicated(Autodesk.Revit.DB.View view)
		{

			ComboBoxDuplicateItem item = this.cboDuplicateView.SelectedItem as ComboBoxDuplicateItem;
			bool canDuplicated = true;
			switch (item.Value)
			{
				case DuplicateViewType.DUPLICATE:
					if (!view.CanViewBeDuplicated(ViewDuplicateOption.Duplicate))
					{
						MessageBox.Show(Util.ElementUtil.ViewElementInfo(view) + " cannot be duplicated.");
						canDuplicated = false;
					}
					break;

				case DuplicateViewType.DUPLICATE_WITH_DETAILING:
					if (!view.CanViewBeDuplicated(ViewDuplicateOption.WithDetailing))
					{
						MessageBox.Show(Util.ElementUtil.ViewElementInfo(view) + " cannot be duplicated with detailing.");
						canDuplicated = false;
					}
					break;

				case DuplicateViewType.DUPLICATE_AS_DEPENDENT:
					if (!view.CanViewBeDuplicated(ViewDuplicateOption.AsDependent))
					{
						MessageBox.Show(Util.ElementUtil.ViewElementInfo(view) + " cannot be duplicated as dependent.");
						canDuplicated = false;
					}
					break;
			}

			return canDuplicated;
		}

		private void addLeftToRight()
		{

			if (this.lbScopeBox.SelectedIndices.Count > 0)
			{
				List<Object> tmp = new List<Object>();
				for (int i = 0; i < this.lbScopeBox.SelectedIndices.Count; i++)
				{
					int idx = this.lbScopeBox.SelectedIndices[i];
					this.lbSelectedScopeBox.Items.Add(this.lbScopeBox.Items[idx]);
					tmp.Add(this.lbScopeBox.Items[idx]);
				}

				for (int j = 0; j < tmp.Count; j++)
				{
					this.lbScopeBox.Items.Remove(tmp[j]);
				}

			}
		}

		private void addRightToLeft()
		{
			if (this.lbSelectedScopeBox.SelectedIndices.Count > 0)
			{
				List<Object> tmp = new List<Object>();
				for (int i = 0; i < this.lbSelectedScopeBox.SelectedIndices.Count; i++)
				{
					int idx = this.lbSelectedScopeBox.SelectedIndices[i];
					this.lbScopeBox.Items.Add(this.lbSelectedScopeBox.Items[idx]);
					tmp.Add(this.lbSelectedScopeBox.Items[idx]);
				}

				for (int j = 0; j < tmp.Count; j++)
				{
					this.lbSelectedScopeBox.Items.Remove(tmp[j]);
				}
			}
		}

		private void appendTextWithColor(String text, System.Drawing.Color color)
		{
			this.rtbLog.Select(this.rtbLog.TextLength, 0);
			this.rtbLog.SelectionColor = color;
			this.rtbLog.AppendText(text);
		}

		private void duplicateView()
		{
			foreach (Autodesk.Revit.DB.View view in this.mSelectedViews)
			{
				if (!this.checkViewCanBeDuplicated(view))
					continue;

				//Get the duplicate option is corresponding to the selected item from duplicated combobox
				ComboBoxDuplicateItem item = this.cboDuplicateView.SelectedItem as ComboBoxDuplicateItem;
				ViewDuplicateOption duplicateOpt = ViewDuplicateOption.Duplicate;
				switch (item.Value)
				{
					case DuplicateViewType.DUPLICATE:
						duplicateOpt = ViewDuplicateOption.Duplicate;
						break;

					case DuplicateViewType.DUPLICATE_WITH_DETAILING:
						duplicateOpt = ViewDuplicateOption.WithDetailing;
						break;

					case DuplicateViewType.DUPLICATE_AS_DEPENDENT:
						duplicateOpt = ViewDuplicateOption.AsDependent;
						break;
				}

				//Start to duplicate
				Autodesk.Revit.DB.View dupView = null;
				ElementId dupViewId = null;

				int n = this.lbSelectedScopeBox.Items.Count;
				if (chkViewQuantity.Checked)
					n = Convert.ToInt32(this.nudViewQuantity.Value);

				Transaction trans = new Transaction(this.mDoc, "DUPLICATE-VIEW");
				for (int i = 0; i < n; i++)
				{

					trans.Start();
					try
					{
						//Duplicate the view
						dupViewId = view.Duplicate(duplicateOpt);
						dupView = this.mDoc.GetElement(dupViewId) as Autodesk.Revit.DB.View;

						//Set the view template id for the duplicated view
						if (this.cboViewTemplate.SelectedIndex > -1)
						{
							ComboBoxViewTemplateItem templateItem = this.cboViewTemplate.SelectedItem as ComboBoxViewTemplateItem;
							dupView.ViewTemplateId = templateItem.Value;
						}

						if (!chkViewQuantity.Checked)
						{
							//Set the scope box id for the duplicated view
							ListBoxScopeBoxItem sbItem = this.lbSelectedScopeBox.Items[i] as ListBoxScopeBoxItem;
							Parameter p = dupView.get_Parameter(BuiltInParameter.VIEWER_VOLUME_OF_INTEREST_CROP);
							if (p != null)
							{
								p.Set(sbItem.Value);
							}
							dupView.Name = view.Name + "-" + sbItem.Text;
						}
						else
						{
							int k = Convert.ToInt32(this.nudStartNumber.Value) + i;
							string post_number = (k < 10) ? "0" + k : k.ToString();
							dupView.Name = view.Name + "-" + post_number;
						}


						//Set the new name for the duplicated view

						trans.Commit();

						this.appendTextWithColor(dupView.Name + ": ", System.Drawing.Color.Black);
						this.appendTextWithColor("[DONE]" + Environment.NewLine, System.Drawing.Color.Green);
					}
					catch (Exception ex)
					{
						this.appendTextWithColor(dupView.Name + ": ", System.Drawing.Color.Black);
						this.appendTextWithColor("[FAILED]" + Environment.NewLine, System.Drawing.Color.Red);
						this.appendTextWithColor(ex.ToString() + Environment.NewLine, System.Drawing.Color.Red);
						trans.RollBack();
					}

				}

			}
		}

		private void btnAddSelected_Click(object sender, EventArgs e)
		{
			this.addLeftToRight();
		}

		private void btnRemoveSelected_Click(object sender, EventArgs e)
		{
			this.addRightToLeft();
		}

		private void lbScopeBox_MouseDoubleClick(object sender, MouseEventArgs e)
		{
			this.addLeftToRight();
		}

		private void lbSelectedScopeBox_MouseDoubleClick(object sender, MouseEventArgs e)
		{
			this.addRightToLeft();
		}

		private void chkViewQuantity_CheckedChanged(object sender, EventArgs e)
		{
			this.lbScopeBox.Enabled = !this.chkViewQuantity.Checked;
			this.lbSelectedScopeBox.Enabled = !this.chkViewQuantity.Checked;
			this.btnAddSelected.Enabled = !this.chkViewQuantity.Checked;
			this.btnRemoveSelected.Enabled = !this.chkViewQuantity.Checked;

			this.nudViewQuantity.Enabled = this.chkViewQuantity.Checked;
			this.nudStartNumber.Enabled = this.chkViewQuantity.Checked;
		}

		private void btnDuplicateView_Click(object sender, EventArgs e)
		{
			if (this.cboDuplicateView.SelectedIndex == -1)
			{
				MessageBox.Show("Please, select a duplicate view option.");
				return;
			}


			if (!chkViewQuantity.Checked)
			{
				if (this.lbSelectedScopeBox.Items.Count == 0)
				{
					MessageBox.Show("Please, add the some scope box to the right list.");
					return;
				}
			}

			if (MessageBox.Show("Are you ready?", "Confirm", MessageBoxButtons.YesNo) == System.Windows.Forms.DialogResult.No)
			{
				return;
			}

			try
			{
				//Disabled the control before duplicate the view
				this.cboDuplicateView.Enabled = false;
				this.cboViewTemplate.Enabled = false;

				this.lbScopeBox.Enabled = false;
				this.lbSelectedScopeBox.Enabled = false;
				this.btnAddSelected.Enabled = false;
				this.btnRemoveSelected.Enabled = false;

				this.btnClose.Enabled = false;
				this.btnDuplicateView.Enabled = false;

				this.chkViewQuantity.Enabled = false;
				this.nudStartNumber.Enabled = false;
				this.nudViewQuantity.Enabled = false;

				this.rtbLog.Text = "";
				//Run the duplicate views
				this.duplicateView();
			}
			catch (Exception ex)
			{
				this.appendTextWithColor("[ERROR]" + Environment.NewLine, System.Drawing.Color.Red);
				this.appendTextWithColor(ex.ToString() + Environment.NewLine, System.Drawing.Color.Red);
			}
			finally
			{
				//Enabled the control after duplicate the view
				this.cboDuplicateView.Enabled = true;
				this.cboViewTemplate.Enabled = true;

				this.lbScopeBox.Enabled = !this.chkViewQuantity.Checked;
				this.lbSelectedScopeBox.Enabled = !this.chkViewQuantity.Checked;
				this.btnAddSelected.Enabled = !this.chkViewQuantity.Checked;
				this.btnRemoveSelected.Enabled = !this.chkViewQuantity.Checked;

				this.btnClose.Enabled = true;
				this.btnDuplicateView.Enabled = true;

				this.chkViewQuantity.Enabled = true;

				this.nudStartNumber.Enabled = this.chkViewQuantity.Checked;
				this.nudViewQuantity.Enabled = this.chkViewQuantity.Checked;
			}
		}




	}

	public enum DuplicateViewType
	{
		NONE = 0,
		DUPLICATE = 1,
		DUPLICATE_WITH_DETAILING = 2,
		DUPLICATE_AS_DEPENDENT = 3
	}

	public class ComboBoxDuplicateItem
	{
		public string Text { get; set; }
		public DuplicateViewType Value { get; set; }

		public ComboBoxDuplicateItem() { }

		public override string ToString()
		{
			return Text;
		}
	}

	public class ComboBoxViewTemplateItem
	{
		public string Text { get; set; }

		public ElementId Value { get; set; }

		public ComboBoxViewTemplateItem() { }

		public override string ToString()
		{
			return Text + " (" + Value.IntegerValue.ToString() + ")";
		}
	}

	public class ListBoxScopeBoxItem
	{
		public string Text { get; set; }

		public ElementId Value { get; set; }
		public ListBoxScopeBoxItem() { }

		public override string ToString()
		{
			return Text;
		}
	}
}
