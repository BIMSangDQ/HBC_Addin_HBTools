using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI.Selection;

namespace DuplicatingSheets
{
	public partial class DuplicateSheetsForm : System.Windows.Forms.Form
	{
		public Document Doc
		{
			set
			{
				this.mdoc = value;
			}
		}

		public Autodesk.Revit.ApplicationServices.Application App
		{
			set
			{
				this.mApp = value;
			}
		}

		public Selection Sel
		{
			set
			{
				this.mSel = value;
			}
		}

		public DuplicateSheetsForm()
		{
			this.InitializeComponent();
		}

		private void btnOK_Click(object sender, EventArgs e)
		{
			decimal value = this.nudQuantity.Value;
			List<ViewSheet> list;
			using (Transaction transaction = new Transaction(this.mdoc, "Duplicate Sheet"))
			{
				transaction.Start();
				list = this.FilterSheet(this.mdoc, this.mSel);
				int num = 1;
				while (num <= value)
				{
					switch (this.checkSelectType())
					{
						case SelectType.VIEW_LEGEND:
							foreach (ViewSheet viewSheet in list)
							{
								try
								{
									ViewSheet newSheet = this.DuplicateSheet(this.mdoc, viewSheet, num);
									this.InsertLegends(this.mdoc, viewSheet, newSheet);
									this.InsertViews(this.mdoc, viewSheet, newSheet, num);
								}
								catch (Exception ex)
								{
								}
							}
							break;
						case SelectType.VIEW_LEGEND_SCHEDULE:
							foreach (ViewSheet viewSheet2 in list)
							{
								try
								{
									ViewSheet newSheet2 = this.DuplicateSheet(this.mdoc, viewSheet2, num);
									this.InsertLegends(this.mdoc, viewSheet2, newSheet2);
									this.InsertViews(this.mdoc, viewSheet2, newSheet2, num);
									this.InsertSchedules(this.mdoc, viewSheet2, newSheet2);
								}
								catch (Exception ex2)
								{
									MessageBox.Show(ex2.ToString());
								}
							}
							break;
						case SelectType.LEGEND_SCHEDULE:
							foreach (ViewSheet viewSheet3 in list)
							{
								try
								{
									ViewSheet newSheet3 = this.DuplicateSheet(this.mdoc, viewSheet3, num);
									this.InsertLegends(this.mdoc, viewSheet3, newSheet3);
									this.InsertSchedules(this.mdoc, viewSheet3, newSheet3);
								}
								catch (Exception ex3)
								{
								}
							}
							break;
						case SelectType.VIEW_SCHEDULE:
							foreach (ViewSheet viewSheet4 in list)
							{
								try
								{
									ViewSheet newSheet4 = this.DuplicateSheet(this.mdoc, viewSheet4, num);
									this.InsertSchedules(this.mdoc, viewSheet4, newSheet4);
									this.InsertViews(this.mdoc, viewSheet4, newSheet4, num);
								}
								catch (Exception ex4)
								{
								}
							}
							break;
						case SelectType.VIEW:
							foreach (ViewSheet viewSheet5 in list)
							{
								try
								{
									ViewSheet newSheet5 = this.DuplicateSheet(this.mdoc, viewSheet5, num);
									this.InsertViews(this.mdoc, viewSheet5, newSheet5, num);
								}
								catch (Exception ex5)
								{
								}
							}
							break;
						case SelectType.LEGEND:
							foreach (ViewSheet viewSheet6 in list)
							{
								try
								{
									ViewSheet newSheet6 = this.DuplicateSheet(this.mdoc, viewSheet6, num);
									this.InsertLegends(this.mdoc, viewSheet6, newSheet6);
								}
								catch (Exception ex6)
								{
								}
							}
							break;
						case SelectType.SCHEDULE:
							foreach (ViewSheet viewSheet7 in list)
							{
								try
								{
									ViewSheet newSheet7 = this.DuplicateSheet(this.mdoc, viewSheet7, num);
									this.InsertSchedules(this.mdoc, viewSheet7, newSheet7);
								}
								catch (Exception ex7)
								{
								}
							}
							break;
					}
					num++;
				}
				transaction.Commit();
			}
			int est_Saving_Time = (int)(20000m * value * list.Count) / 1000;
		}

		private SelectType checkSelectType()
		{
			bool flag = this.chbViews.Checked && this.chbLegends.Checked && this.chbSchedules.Checked;
			SelectType result;
			if (flag)
			{
				result = SelectType.VIEW_LEGEND_SCHEDULE;
			}
			else
			{
				bool flag2 = !this.chbViews.Checked && this.chbLegends.Checked && this.chbSchedules.Checked;
				if (flag2)
				{
					result = SelectType.LEGEND_SCHEDULE;
				}
				else
				{
					bool flag3 = !this.chbViews.Checked && !this.chbLegends.Checked && this.chbSchedules.Checked;
					if (flag3)
					{
						result = SelectType.SCHEDULE;
					}
					else
					{
						bool flag4 = !this.chbViews.Checked && this.chbLegends.Checked && !this.chbSchedules.Checked;
						if (flag4)
						{
							result = SelectType.LEGEND;
						}
						else
						{
							bool flag5 = this.chbViews.Checked && !this.chbLegends.Checked && this.chbSchedules.Checked;
							if (flag5)
							{
								result = SelectType.VIEW_SCHEDULE;
							}
							else
							{
								bool flag6 = this.chbViews.Checked && this.chbLegends.Checked && !this.chbSchedules.Checked;
								if (flag6)
								{
									result = SelectType.VIEW_LEGEND;
								}
								else
								{
									bool flag7 = this.chbViews.Checked && !this.chbLegends.Checked && !this.chbSchedules.Checked;
									if (flag7)
									{
										result = SelectType.VIEW;
									}
									else
									{
										result = SelectType.NONE;
									}
								}
							}
						}
					}
				}
			}
			return result;
		}

		private void InsertSchedules(Document mdoc, ViewSheet OldSheet, ViewSheet NewSheet)
		{
			foreach (Element element in new FilteredElementCollector(mdoc).OfClass(typeof(ScheduleSheetInstance)))
			{
				ScheduleSheetInstance scheduleSheetInstance = (ScheduleSheetInstance)element;
				bool flag = scheduleSheetInstance.OwnerViewId == OldSheet.Id;
				if (flag)
				{
					bool flag2 = !scheduleSheetInstance.IsTitleblockRevisionSchedule;
					if (flag2)
					{
						foreach (Element element2 in new FilteredElementCollector(mdoc).OfClass(typeof(ViewSchedule)))
						{
							ViewSchedule viewSchedule = (ViewSchedule)element2;
							bool flag3 = scheduleSheetInstance.ScheduleId == viewSchedule.Id;
							if (flag3)
							{
								try
								{
									BoundingBoxXYZ boundingBoxXYZ = scheduleSheetInstance.get_BoundingBox(OldSheet);
									XYZ xyz = (boundingBoxXYZ.Max + boundingBoxXYZ.Min) / 2.0;
									ScheduleSheetInstance scheduleSheetInstance2 = ScheduleSheetInstance.Create(mdoc, NewSheet.Id, viewSchedule.Id, XYZ.Zero);
									BoundingBoxXYZ boundingBoxXYZ2 = scheduleSheetInstance2.get_BoundingBox(NewSheet);
									XYZ xyz2 = (boundingBoxXYZ2.Max + boundingBoxXYZ2.Min) / 2.0;
									ElementTransformUtils.MoveElement(mdoc, scheduleSheetInstance2.Id, new XYZ(xyz.X - xyz2.X, xyz.Y - xyz2.Y, 0.0));
								}
								catch (Exception ex)
								{
								}
							}
						}
					}
				}
			}
		}

		private void InsertLegends(Document mdoc, ViewSheet OldSheet, ViewSheet NewSheet)
		{
			foreach (ElementId elementId in OldSheet.GetAllPlacedViews())
			{
				Autodesk.Revit.DB.View view = mdoc.GetElement(elementId) as Autodesk.Revit.DB.View;
				Autodesk.Revit.DB.View view2 = null;
				bool flag = view.ViewType == ViewType.Legend;
				if (flag)
				{
					view2 = view;
					foreach (Element element in new FilteredElementCollector(mdoc).OfClass(typeof(Viewport)))
					{
						Viewport viewport = (Viewport)element;
						bool flag2 = viewport.SheetId == OldSheet.Id && viewport.ViewId == view.Id;
						if (flag2)
						{
							try
							{
								BoundingBoxXYZ boundingBoxXYZ = viewport.get_BoundingBox(OldSheet);
								XYZ xyz = (boundingBoxXYZ.Max + boundingBoxXYZ.Min) / 2.0;
								ElementId typeId = viewport.GetTypeId();
								Viewport viewport2 = Viewport.Create(mdoc, NewSheet.Id, view2.Id, XYZ.Zero);
								viewport2.ChangeTypeId(typeId);
								BoundingBoxXYZ boundingBoxXYZ2 = viewport2.get_BoundingBox(NewSheet);
								XYZ xyz2 = (boundingBoxXYZ2.Max + boundingBoxXYZ2.Min) / 2.0;
								ElementTransformUtils.MoveElement(mdoc, viewport2.Id, new XYZ(xyz.X - xyz2.X, xyz.Y - xyz2.Y, 0.0));
							}
							catch (Exception ex)
							{
							}
						}
					}
				}
			}
		}

		private void InsertViews(Document mdoc, ViewSheet OldSheet, ViewSheet NewSheet, int num)
		{
			int num2 = 0;
			foreach (ElementId elementId in OldSheet.GetAllPlacedViews())
			{
				Autodesk.Revit.DB.View view = mdoc.GetElement(elementId) as Autodesk.Revit.DB.View;
				Autodesk.Revit.DB.View view2 = null;
				bool flag = view.ViewType != ViewType.Legend;
				if (flag)
				{
					bool flag2 = !view.CanViewBeDuplicated(ViewDuplicateOption.WithDetailing);
					if (!flag2)
					{
						bool flag3 = this.cmbDuplicate.SelectedIndex == 0;
						ElementId elementId2;
						if (flag3)
						{
							elementId2 = view.Duplicate(0);
						}
						else
						{
							bool flag4 = this.cmbDuplicate.SelectedIndex == 2;
							if (flag4)
							{
								ViewType viewType = view.ViewType;
								bool flag5 = viewType == ViewType.Detail || viewType == ViewType.ThreeD;
								if (flag5)
								{
									elementId2 = view.Duplicate(ViewDuplicateOption.WithDetailing);
								}
								else
								{
									elementId2 = view.Duplicate(ViewDuplicateOption.AsDependent);
								}
							}
							else
							{
								elementId2 = view.Duplicate(ViewDuplicateOption.WithDetailing);
							}
						}
						view2 = (mdoc.GetElement(elementId2) as Autodesk.Revit.DB.View);
						string arg = view.Name.Replace("{", "").Replace("}", "") + "_DUP_";
						num2++;
						bool flag6 = false;
						while (!flag6)
						{
							flag6 = true;
							try
							{
								view2.Name = arg + num2;
							}
							catch
							{
								flag6 = false;
								num2++;
							}
						}
						foreach (Element element in new FilteredElementCollector(mdoc).OfClass(typeof(Viewport)))
						{
							Viewport viewport = (Viewport)element;
							bool flag7 = viewport.SheetId == OldSheet.Id && viewport.ViewId == view.Id;
							if (flag7)
							{
								BoundingBoxXYZ boundingBoxXYZ = viewport.get_BoundingBox(OldSheet);
								XYZ xyz = (boundingBoxXYZ.Max + boundingBoxXYZ.Min) / 2.0;
								ElementId typeId = viewport.GetTypeId();
								Viewport viewport2 = Viewport.Create(mdoc, NewSheet.Id, view2.Id, XYZ.Zero);
								viewport2.ChangeTypeId(typeId);
								mdoc.Regenerate();
								viewport2.LookupParameter("Detail Number").Set(viewport.LookupParameter("Detail Number").AsString());
								BoundingBoxXYZ boundingBoxXYZ2 = viewport2.get_BoundingBox(NewSheet);
								XYZ xyz2 = (boundingBoxXYZ2.Max + boundingBoxXYZ2.Min) / 2.0;
								ElementTransformUtils.MoveElement(mdoc, viewport2.Id, new XYZ(xyz.X - xyz2.X, xyz.Y - xyz2.Y, 0.0));
							}
						}
					}
				}
			}
		}

		private ViewSheet DuplicateSheet(Document mdoc, ViewSheet vs, int i = 1)
		{
			ViewSheet viewSheet = null;
			FamilyInstance familyInstance = new FilteredElementCollector(mdoc).OfClass(typeof(FamilyInstance)).OfCategory(BuiltInCategory.OST_TitleBlocks).Cast<FamilyInstance>().First((FamilyInstance q) => q.OwnerViewId == vs.Id);
			viewSheet = ViewSheet.Create(mdoc, familyInstance.GetTypeId());
			bool flag = false;
			while (!flag)
			{
				flag = true;
				try
				{
					viewSheet.SheetNumber = vs.SheetNumber + "_DUP_" + i;
				}
				catch
				{
					flag = false;
					i++;
				}
			}
			viewSheet.Name = vs.Name;
			foreach (object obj in viewSheet.Parameters)
			{
				Parameter parameter = (Parameter)obj;
				bool flag2 = parameter.Definition.Name == "Sheet Number" || parameter.Definition.Name == "SheetName" || parameter.IsReadOnly;
				if (!flag2)
				{
					Definition definition = parameter.Definition;
					Parameter parameter2 = vs.get_Parameter(definition);
					bool flag3 = parameter2 == null;
					if (!flag3)
					{
						bool flag4 = parameter2.StorageType == StorageType.ElementId;
						if (!flag4)
						{
							bool flag5 = parameter2.StorageType == StorageType.Double;
							if (flag5)
							{
								parameter.Set(parameter2.AsDouble());
							}
							else
							{
								bool flag6 = parameter2.StorageType == StorageType.String;
								if (flag6)
								{
									parameter.Set(parameter2.AsString());
								}
								else
								{
									bool flag7 = parameter2.StorageType == StorageType.Integer;
									if (flag7)
									{
										parameter.Set(parameter2.AsInteger());
									}
								}
							}
						}
					}
				}
			}
			return viewSheet;
		}

		private List<ViewSheet> FilterSheet(Document pDoc, Selection pSelection)
		{
			List<ViewSheet> source = new List<ViewSheet>();
			FilteredElementCollector filteredElementCollector = new FilteredElementCollector(pDoc, pSelection.GetElementIds());
			filteredElementCollector.OfClass(typeof(ViewSheet));
			source = filteredElementCollector.Cast<ViewSheet>().ToList<ViewSheet>();
			IOrderedEnumerable<ViewSheet> source2 = from ele in source
													orderby ele.SheetNumber
													select ele;
			return source2.ToList<ViewSheet>();
		}

		private List<Element> GetAllTitleBlocks(Document doc)
		{
			FilteredElementCollector filteredElementCollector = new FilteredElementCollector(doc).OfClass(typeof(FamilySymbol)).OfCategory(BuiltInCategory.OST_TitleBlocks);
			return filteredElementCollector.ToElements() as List<Element>;
		}

		private void DuplicateSheetsForm_Load(object sender, EventArgs e)
		{
			this.cmbDuplicate.SelectedIndex = 1;
		}

		private Document mdoc;

		private Autodesk.Revit.ApplicationServices.Application mApp;

		private Selection mSel;
	}
}
