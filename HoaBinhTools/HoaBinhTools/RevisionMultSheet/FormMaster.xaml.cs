using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media.Imaging;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;


namespace RevisionMultiSheet
{
	/// <summary>
	/// Interaction logic for FormMaster.xaml
	/// </summary>
	/// 



	public partial class FormMaster : UserControl
	{
		private Document mDoc;
		private Autodesk.Revit.ApplicationServices.Application mApp;
		private UIDocument mUIDoc;
		List<string> listParaName;
		System.Data.DataTable dt, dtSelected;
		public UIDocument UIDoc
		{
			set { mUIDoc = value; }
		}
		public Document Doc
		{
			set { mDoc = value; }
		}
		public Autodesk.Revit.ApplicationServices.Application App
		{
			set { mApp = value; }
		}
		List<Revision> listRevision;
		List<ViewSheet> listAlreadySheet;
		//List<SheetItem> listSheetItem;
		List<RevisionItem> listRevisionItem;
		List<ViewSheet> listGridSelected;
		public FormMaster()
		{
			InitializeComponent();

		}
		private void Init()
		{
			listGridSelected = new List<ViewSheet>();
			listParaName = new List<string>();
			listRevision = getAllRevision();

			//listAlreadySheet = new List<ViewSheet>();
			listAlreadySheet = getAlreadySheet();


			listRevisionItem = new List<RevisionItem>();
			foreach (Revision rev in listRevision)
			{
				RevisionItem revisionItem = new RevisionItem();
				revisionItem.Sequence = rev.Name;
				revisionItem.Date = rev.RevisionDate;
				revisionItem.Issuedto = rev.IssuedTo;
				revisionItem.Issuedby = rev.IssuedBy;

				listRevisionItem.Add(revisionItem);
				//gridRevision.Items.Add(new RevisionItem() { Sequence = revisionItem.Sequence, Date = rev.RevisionDate, Isuedto = rev.IssuedTo, Isuedby = rev.IssuedBy });
			}
			gridRevision.Items.Clear();
			gridRevision.ItemsSource = listRevisionItem;




			dt = new DataTable();

			dt.Columns.Add("Sheet Number");
			dt.Columns.Add("Sheet Name");
			dt.Columns.Add("Current Revision");

			dtSelected = new DataTable();


			//alreadySheet.Items.Clear();
			alreadySheet.ItemsSource = convertAlreadySheet2DataTable(listAlreadySheet).DefaultView;
			alreadySheet.Columns[0].Visibility = System.Windows.Visibility.Visible;
			alreadySheet.Columns[1].Visibility = System.Windows.Visibility.Visible;
			alreadySheet.Columns[2].Visibility = System.Windows.Visibility.Visible;

			dtSelected = dt.Clone();
			selectedSheet.ItemsSource = dtSelected.DefaultView;
			//alreadySheet.ItemsSource = listAlreadySheet;


			//alreadySheet.Items.Clear();


		}
		private void UserControl_Loaded(object sender, RoutedEventArgs e)
		{
			string bitmapPath = "image/Logo_BIM_72.png";
			BitmapImage bitmapImage = new BitmapImage(new Uri(bitmapPath, UriKind.Relative));
			iconBIM.Source = bitmapImage;

			Dispatcher.BeginInvoke(System.Windows.Threading.DispatcherPriority.Loaded, new Action(Init));

		}
		private DataTable convertAlreadySheet2DataTable(List<ViewSheet> listSheet)
		{
			dt.Clear();
			listSheet = listSheet.OrderBy(x => x.SheetNumber).ToList();
			if (listParaName.Count == 0)
			{
				foreach (ViewSheet vs in listSheet)
				{
					ParameterSet parameterSet = vs.Parameters;
					foreach (Parameter para in parameterSet)
					{
						//if (!para.IsShared)
						//    continue;
						//if (para.IsReadOnly)
						//    continue;
						if (para.Definition.Name == "Sheet Number" || para.Definition.Name == "Sheet Name" || para.Definition.Name == "Current Revision")
							continue;

						if (!listParaName.Contains(para.Definition.Name))
						{

							if (!dt.Columns.Contains(para.Definition.Name))
							{
								listParaName.Add(para.Definition.Name);
								dt.Columns.Add(para.Definition.Name);
							}
							//dtSelected.Columns.Add(para.Definition.Name);
						}
						break;
					}
				}
				listCheckbox.ItemsSource = listParaName.OrderBy(x => x).ToDictionary(x => x, x => (object)x);
			}

			string[] arr2 = new string[listParaName.Count + 3];

			int ivs = 0;
			foreach (ViewSheet vs in listSheet)
			{
				arr2[0] = vs.SheetNumber;
				arr2[1] = vs.Name;
				Revision rvs = mDoc.GetElement(vs.GetCurrentRevision()) as Revision;
				if (rvs == null)
				{
					arr2[2] = "";
				}
				else
				{
					arr2[2] = rvs.Name;
				}
				ParameterSet vsPara = vs.Parameters;
				int i = 3;
				foreach (string pa in listParaName)
				{
					foreach (Parameter p in vsPara)
					{
						//if (!p.IsShared)
						//    continue;
						//if (p.IsReadOnly)
						//    continue;
						if (p.Definition.Name == pa)
						{
							arr2[i] = getValueParameter(p);
						}
					}
					i++;
				}
				dt.Rows.Add(arr2);

				int percent = (ivs + 1) * 100 / listSheet.Count;
				progressBar.Dispatcher.Invoke(() => progressBar.Value = percent, System.Windows.Threading.DispatcherPriority.Background);
				ivs++;
			}
			//MessageBox.Show(listSheet[0].SheetNumber);
			AddorRemoveCol();
			return dt;
		}
		private DataTable convertAlreadySheet2DataTableSelected(List<ViewSheet> listSheet)
		{
			//dtSelected.Clear();
			dtSelected = new DataTable();
			dtSelected.Columns.Add("Sheet Number");
			dtSelected.Columns.Add("Sheet Name");
			dtSelected.Columns.Add("Current Revision");
			listSheet = listSheet.OrderBy(x => x.SheetNumber).ToList();
			//foreach (ViewSheet vs in listSheet)
			//{
			//    ParameterSet parameterSet = vs.Parameters;
			//    foreach (Parameter para in parameterSet)
			//    {
			//        if (para.Definition.Name == "Sheet Number" || para.Definition.Name == "Sheet Name" || para.Definition.Name == "Current Revision")
			//            continue;
			//        if (!dtSelected.Columns.Contains(para.Definition.Name))
			//            dtSelected.Columns.Add(para.Definition.Name);
			//    }
			//}

			string[] arr2 = new string[3];
			foreach (ViewSheet vs in listSheet)
			{
				arr2[0] = vs.SheetNumber;
				arr2[1] = vs.Name;
				Revision rvs = mDoc.GetElement(vs.GetCurrentRevision()) as Revision;
				if (rvs == null)
				{
					arr2[2] = "";
				}
				else
				{
					arr2[2] = rvs.Name;
				}
				//ParameterSet vsPara = vs.Parameters;
				//int i = 3;
				//foreach (string pa in listParaName)
				//{
				//    foreach (Parameter p in vsPara)
				//    {
				//        if (p.Definition.Name == pa)
				//        {
				//            arr2[i] = getValueParameter(p);
				//        }
				//    }
				//    i++;
				//}
				dtSelected.Rows.Add(arr2);
			}
			//MessageBox.Show(listSheet[0].SheetNumber);
			//AddorRemoveCol();
			return dtSelected;
		}
		private string getValueParameter(Parameter param)
		{
			switch (param.StorageType)
			{
				case StorageType.Double:
					//covert the number into Metric
					//value = param.AsValueString();
					//listValueString.Add(param.AsValueString());
					//listPairValueParameter.Add(param.Definition.Name, param.AsValueString());
					return param.AsValueString();
				case StorageType.ElementId:
					//find out the name of the element
					ElementId id = param.AsElementId();
					if (id.IntegerValue >= 0)
					{
						//value = doc.GetElement(id).Name;
						//listValueString.Add(doc.GetElement(id).Name);
						//listPairValueParameter.Add(param.Definition.Name, doc.GetElement(id).Name);
						return mDoc.GetElement(id).Name;
					}
					else
					{
						//value = id.IntegerValue.ToString();
						//listValueString.Add(id.IntegerValue.ToString());
						//listPairValueParameter.Add(param.Definition.Name, id.IntegerValue.ToString());
						return id.IntegerValue.ToString();
					}
				case StorageType.Integer:
					if (ParameterType.YesNo == param.Definition.ParameterType)
					{
						if (param.AsInteger() == 0)
						{
							//value = "False";
							//listValueString.Add("False");
							//listPairValueParameter.Add(param.Definition.Name, "False");
							return "False";
						}
						else
						{
							//value = "True";
							//listValueString.Add("True");
							//listPairValueParameter.Add(param.Definition.Name, "True");
							return "True";
						}
					}
					else
					{
						//value = param.AsInteger().ToString();
						//listValueString.Add(param.AsInteger().ToString());
						//listPairValueParameter.Add(param.Definition.Name, param.AsInteger().ToString());
						return param.AsInteger().ToString();
					}
				case StorageType.String:
					//value = param.AsString();
					//listValueString.Add(param.AsString());
					//listPairValueParameter.Add(param.Definition.Name, param.AsString());
					return param.AsString();
				default:
					//value = "Unexposed parameter.";
					//listValueString.Add("Unexposed parameter.");
					//listPairValueParameter.Add(param.Definition.Name, "Unexposed parameter.");
					return "Unexposed parameter.";

			}
		}

		private List<Revision> getAllRevision()
		{
			FilteredElementCollector colector2 = new FilteredElementCollector(mDoc).OfCategory(BuiltInCategory.OST_Revisions);
			List<Revision> revisionList = colector2.ToElements().Cast<Revision>().OrderBy(x => x.IssuedTo == String.Empty).ThenBy(x => x.IssuedTo).ToList();

			return revisionList;
		}

		private List<ViewSheet> getAlreadySheet()
		{
			ElementCategoryFilter ecf = new ElementCategoryFilter(BuiltInCategory.OST_Sheets);
			FilteredElementCollector collector = new FilteredElementCollector(mDoc).WherePasses(ecf);
			List<ViewSheet> sheetlist = collector.ToElements().Cast<ViewSheet>().ToList().OrderBy(x => x.SheetNumber).ToList();
			return sheetlist;
		}

		private void findSheet_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
		{
			findSheet.Text = String.Empty;
		}

		System.Windows.Forms.Timer _tmrDelaySearch;
		int indexTextChange = 0;
		void _tmrDelaySearch_Tick(object sender, EventArgs e)
		{
			////System.Windows.Controls.TextBox textbox = sender as System.Windows.Controls.TextBox;
			//string strText = findSheet.Text;

			//if (string.IsNullOrEmpty(strText))
			//{
			//    if(indexTextChange != 0)
			//    {
			//        for (int i = alreadySheet.Items.Count - 1; i >= 0; i--)
			//        {
			//            DataGridRow row = (DataGridRow)alreadySheet.ItemContainerGenerator.ContainerFromIndex(i);
			//            if (row == null)
			//            {
			//                var item = alreadySheet.Items[i];
			//                /* bring the data item (Product object) into view
			//                 * in case it has been virtualized away */
			//                alreadySheet.ScrollIntoView(item);
			//                row = alreadySheet.ItemContainerGenerator.ContainerFromIndex(i) as DataGridRow;
			//            }
			//            //row.Visibility = System.Windows.Visibility.Visible;
			//            row.Dispatcher.Invoke(() => row.Visibility = System.Windows.Visibility.Visible, System.Windows.Threading.DispatcherPriority.Send);
			//        }
			//    }
			//    indexTextChange++;
			//}
			//else
			//{
			//    //Dispatcher.BeginInvoke(System.Windows.Threading.DispatcherPriority.Loaded, new Action(() => findingText(strText)));
			//    alreadySheet.Items.Dispatcher.BeginInvoke(System.Windows.Threading.DispatcherPriority.Send, new Action(findingText));
			//    //findingText(strText);
			//}

			//if (_tmrDelaySearch != null)
			//    _tmrDelaySearch.Stop();
		}
		private void findSheet_TextChanged(object sender, TextChangedEventArgs e)
		{
			string strText = findSheet.Text;
			if (string.IsNullOrEmpty(strText))
			{

				for (int i = alreadySheet.Items.Count - 1; i >= 0; i--)
				{
					DataGridRow row = (DataGridRow)alreadySheet.ItemContainerGenerator.ContainerFromIndex(i);
					if (row == null)
					{
						var item = alreadySheet.Items[i];
						/* bring the data item (Product object) into view
                         * in case it has been virtualized away */
						alreadySheet.ScrollIntoView(item);
						row = alreadySheet.ItemContainerGenerator.ContainerFromIndex(i) as DataGridRow;
					}
					//row.Visibility = System.Windows.Visibility.Visible;

					row.Dispatcher.Invoke(() => row.Visibility = System.Windows.Visibility.Visible, System.Windows.Threading.DispatcherPriority.Send);
				}

			}
			//int DelayedTextChangedTimeout = 500;
			//if (_tmrDelaySearch != null)
			//    _tmrDelaySearch.Stop();

			//if (_tmrDelaySearch == null)
			//{
			//    _tmrDelaySearch = new System.Windows.Forms.Timer();
			//    _tmrDelaySearch.Tick += _tmrDelaySearch_Tick;
			//    _tmrDelaySearch.Interval = DelayedTextChangedTimeout;
			//}

			//_tmrDelaySearch.Start();
		}

		//private void findingText()
		//{
		//    if (listAlreadySheet != null)
		//    {
		//        string strText = findSheet.Text;
		//        for (int i = 0; i < alreadySheet.Items.Count; i++)
		//        {
		//            DataRowView dataRowView = alreadySheet.Items[i] as DataRowView;
		//            string sheetnumber = dataRowView[0].ToString();
		//            //if (!sheetnumber.ToLower().Contains(strText.ToLower()))
		//            //    MatchCollection matches = Regex.Matches(sheetnumber.ToLower(), strText.ToLower());
		//            //if (matches.Count == 0)
		//            //DataGridRow row1 = (DataGridRow)alreadySheet.ItemContainerGenerator.ContainerFromIndex(i);
		//            //row1.Visibility = System.Windows.Visibility.Hidden;
		//            //row1.Visibility = System.Windows.Visibility.Collapsed;
		//            if (!sheetnumber.ToLower().Contains(strText.ToLower()))
		//            {
		//                DataGridRow row = (DataGridRow)alreadySheet.ItemContainerGenerator.ContainerFromIndex(i);
		//                //row.Visibility = System.Windows.Visibility.Visible;
		//                if (row == null)
		//                {
		//                    var item = alreadySheet.Items[i];
		//                    /* bring the data item (Product object) into view
		//                     * in case it has been virtualized away */
		//                    alreadySheet.ScrollIntoView(item);
		//                    row = alreadySheet.ItemContainerGenerator.ContainerFromIndex(i) as DataGridRow;
		//                }
		//                row.Visibility = System.Windows.Visibility.Hidden;
		//                row.Visibility = System.Windows.Visibility.Collapsed;

		//                //row.Dispatcher.Invoke(() => row.Visibility = System.Windows.Visibility.Collapsed, System.Windows.Threading.DispatcherPriority.Send);
		//            }
		//        }
		//        if (alreadySheet.Items.Count != 0)
		//            alreadySheet.ScrollIntoView(alreadySheet.Items[0]);

		//    }

		//}

		private void alreadySheet_MouseEnter(object sender, MouseEventArgs e)
		{
			alreadySheet.Width = 810;
			alreadySheet.SetValue(System.Windows.Controls.Grid.ColumnSpanProperty, 2);
			selectedSheet.Visibility = System.Windows.Visibility.Hidden;


			//System.Windows.Controls.Grid.SetColumn(alreadySheet, 1);
			//System.Windows.Controls.Grid.SetRow(alreadySheet, 3);
		}

		private void alreadySheet_MouseLeave(object sender, MouseEventArgs e)
		{
			alreadySheet.Width = 400;
			alreadySheet.SetValue(System.Windows.Controls.Grid.ColumnProperty, 0);
			selectedSheet.Visibility = System.Windows.Visibility.Visible;
			//System.Windows.Controls.Grid.SetColumn(alreadySheet, 1);
			//System.Windows.Controls.Grid.SetRow(alreadySheet, 3);
		}
		private string ConvertTiengVietCoDau(string s)
		{
			Regex regex = new Regex("\\p{IsCombiningDiacriticalMarks}+");
			string temp = s.Normalize(NormalizationForm.FormD);
			return regex.Replace(temp, String.Empty).Replace('\u0111', 'd').Replace('\u0110', 'D');

		}
		//private List<SheetItem> ConvertViewSheet2SheetItem(List<ViewSheet> listVS)
		//{
		//    List<SheetItem> listSI = new List<SheetItem>();
		//    List<string> parameterlist = new List<string>();
		//    Items = new Dictionary<string, object>();
		//    foreach (ViewSheet vs in listVS)
		//    {
		//        Revision rvs = mDoc.GetElement(vs.GetCurrentRevision()) as Revision;
		//        ParameterSet parameterSet = vs.Parameters;
		//        string hangmuc = null;
		//        foreach (Parameter para in parameterSet)
		//        {
		//            //para.Definition.Name == "HẠNG MỤC" ? para.As
		//            if (ConvertTiengVietCoDau(para.Definition.Name).ToLower().Replace(" ", String.Empty) == "hangmuc")
		//            {
		//                hangmuc = para.AsString();
		//                //break;
		//            }
		//            if (!Items.ContainsKey(para.Definition.Name))
		//                Items.Add(para.Definition.Name, para.Definition.Name);


		//        }
		//        SheetItem sheetItem = new SheetItem();
		//        if (rvs == null)
		//        {
		//            //alreadySheet.Items.Add(new SheetItem() { Number = vs.SheetNumber, Name = vs.Name, RevSeq = "" });

		//            sheetItem.SheetNumber = vs.SheetNumber;
		//            sheetItem.SheetName = vs.Name;
		//            sheetItem.CurrentRevision = "";
		//            sheetItem.HangMuc = hangmuc;
		//            sheetItem.Para = vs.Parameters;
		//            listSI.Add(sheetItem);
		//        }
		//        else
		//        {
		//            //alreadySheet.Items.Add(new SheetItem() { Number = vs.SheetNumber, Name = vs.Name, RevSeq = rvs.Name });                    
		//            sheetItem.SheetNumber = vs.SheetNumber;
		//            sheetItem.SheetName = vs.Name;
		//            sheetItem.CurrentRevision = rvs.Name;
		//            sheetItem.HangMuc = hangmuc;
		//            sheetItem.Para = vs.Parameters;
		//            listSI.Add(sheetItem);
		//        }
		//    }

		//    var item = from pair in Items orderby pair.Key ascending select pair;
		//    listCheckbox.ItemsSource = item.ToDictionary(x => x.Key, x => x.Value);
		//    return listSI;
		//}
		private Dictionary<string, object> _items;
		public Dictionary<string, object> Items
		{
			get
			{
				return _items;
			}
			set
			{
				_items = value;

			}
		}
		private void btnAdd_Click(object sender, RoutedEventArgs e)
		{
			var listSelected = alreadySheet.SelectedItems;
			foreach (var o in listSelected)
			{
				var indexSelected = alreadySheet.Items.IndexOf(o);
				DataGridRow row = (DataGridRow)alreadySheet.ItemContainerGenerator.ContainerFromIndex(indexSelected);
				if (row.Visibility == System.Windows.Visibility.Collapsed)
					continue;
				listGridSelected.Add(listAlreadySheet[indexSelected]);
				//listAlreadySheet.RemoveAt(indexSelected);

				var z = dt.Rows[indexSelected];
				dtSelected.Rows.Add(z.ItemArray);
				//dt.Rows[indexSelected].Delete();
			}
			for (int i = listSelected.Count - 1; i >= 0; i--)
			{
				var indexSelected = alreadySheet.Items.IndexOf(listSelected[i]);
				DataGridRow row = (DataGridRow)alreadySheet.ItemContainerGenerator.ContainerFromIndex(indexSelected);
				if (row.Visibility == System.Windows.Visibility.Collapsed)
					continue;
				listAlreadySheet.RemoveAt(indexSelected);
				dt.Rows[indexSelected].Delete();
			}

			alreadySheet.ItemsSource = dt.DefaultView;
			selectedSheet.ItemsSource = dtSelected.DefaultView;
			alreadySheet.Columns[0].Visibility = System.Windows.Visibility.Visible;
			alreadySheet.Columns[1].Visibility = System.Windows.Visibility.Visible;
			alreadySheet.Columns[2].Visibility = System.Windows.Visibility.Visible;
		}

		private void btnRemove_Click(object sender, RoutedEventArgs e)
		{
			var listSelected = selectedSheet.SelectedItems;
			foreach (var o in listSelected)
			{
				var indexSelected = selectedSheet.Items.IndexOf(o);
				listAlreadySheet.Add(listGridSelected[indexSelected]);
				//listAlreadySheet.RemoveAt(indexSelected);

				var z = dtSelected.Rows[indexSelected];
				dt.Rows.Add(z.ItemArray);
				//dt.Rows[indexSelected].Delete();
			}
			for (int i = listSelected.Count - 1; i >= 0; i--)
			{
				var indexSelected = selectedSheet.Items.IndexOf(listSelected[i]);
				listGridSelected.RemoveAt(indexSelected);
				dtSelected.Rows[indexSelected].Delete();
			}

			alreadySheet.ItemsSource = dt.DefaultView;
			selectedSheet.ItemsSource = dtSelected.DefaultView;
			alreadySheet.Columns[0].Visibility = System.Windows.Visibility.Visible;
			alreadySheet.Columns[1].Visibility = System.Windows.Visibility.Visible;
			alreadySheet.Columns[2].Visibility = System.Windows.Visibility.Visible;
		}

		private void btnCancel_Click(object sender, RoutedEventArgs e)
		{
			for (int i = 0; i < selectedSheet.Items.Count; i++)
			{
				var z = dtSelected.Rows[i];
				dt.Rows.Add(z.ItemArray);
				listAlreadySheet.Add(listGridSelected[i]);
			}

			for (int i = selectedSheet.Items.Count - 1; i >= 0; i--)
			{
				listGridSelected.RemoveAt(i);
				dtSelected.Rows[i].Delete();
			}


			//var listSelected = selectedSheet.SelectedItems;
			//foreach (var o in listSelected)
			//{
			//    var indexSelected = selectedSheet.Items.IndexOf(o);
			//    listAlreadySheet.Add(listGridSelected[indexSelected]);
			//    //listAlreadySheet.RemoveAt(indexSelected);

			//    var z = dtSelected.Rows[indexSelected];
			//    dt.Rows.Add(z.ItemArray);
			//    //dt.Rows[indexSelected].Delete();
			//}
			//for (int i = listSelected.Count - 1; i >= 0; i--)
			//{
			//    var indexSelected = selectedSheet.Items.IndexOf(listSelected[i]);
			//    listGridSelected.RemoveAt(indexSelected);
			//    dtSelected.Rows[indexSelected].Delete();
			//}

			alreadySheet.ItemsSource = dt.DefaultView;
			selectedSheet.ItemsSource = dtSelected.DefaultView;
			alreadySheet.Columns[0].Visibility = System.Windows.Visibility.Visible;
			alreadySheet.Columns[1].Visibility = System.Windows.Visibility.Visible;
			alreadySheet.Columns[2].Visibility = System.Windows.Visibility.Visible;
		}

		private void btnAccept_Click(object sender, RoutedEventArgs e)
		{
			//MessageBox.Show("Total Sheet Selected: " + listGridSelected.Count.ToString());
			Object o = gridRevision.SelectedItem;
			if (o != null)
			{
				var watch = System.Diagnostics.Stopwatch.StartNew();

				RevisionItem revSelected = o as RevisionItem;

				Revision rev = null;
				foreach (Revision rv in listRevision)
				{
					if (revSelected.Sequence == rv.Name)
					{
						rev = rv;
						break;
					}
				}
				//MessageBox.Show(listGridSelected.Count.ToString());
				int i = 0;
				foreach (ViewSheet vs in listGridSelected)
				{
					List<ElementId> listRev = new List<ElementId>();
					listRev.Add(rev.Id);
					using (Transaction t = new Transaction(mDoc))
					{
						t.Start("Apply Revision");
						List<ElementId> lvs = vs.GetAdditionalRevisionIds() as List<ElementId>;
						//List<ElementId> lvs = vs.GetAllRevisionIds() as List<ElementId>;
						listRev.AddRange(lvs);
						vs.SetAdditionalRevisionIds(listRev);

						t.Commit();
					}

					int percent = (i + 1) * 100 / listGridSelected.Count;
					progressBar.Dispatcher.Invoke(() => progressBar.Value = percent, System.Windows.Threading.DispatcherPriority.Background);
					//System.Threading.Thread.Sleep(1000);
					i++;
				}
				watch.Stop();
				resultBox.Content = "Complete in " + watch.ElapsedMilliseconds + " milisecond" + "\nEstimate you just saving " + (3000 * listGridSelected.Count - watch.ElapsedMilliseconds) / 1000 + " seconds to set Revision to " + listGridSelected.Count + " sheets"
					+ "\nNote: Average we take 3s to set a Sheet to Revision. \nDon't forget give us your feedback to improve quality better.";
				//progressBar.Value = 100;

				//Software_Key_Manager_Beta.MainForm mainForm = new Software_Key_Manager_Beta.MainForm();
				//mainForm.insertRecord2DB(System.Reflection.Assembly.GetExecutingAssembly().GetName().Name, (int)(3000 * listGridSelected.Count - watch.ElapsedMilliseconds) / 1000);
				MessageBox.Show("Operation Complete Success. ");
			}
			else
			{
				MessageBox.Show("Please select Revision to set.");
			}
		}

		private void alreadySheet_MouseDoubleClick(object sender, MouseButtonEventArgs e)
		{
			try
			{
				int indexSelected = alreadySheet.SelectedIndex;

				listGridSelected.Add(listAlreadySheet[indexSelected]);
				listAlreadySheet.RemoveAt(indexSelected);


				var z = dt.Rows[indexSelected];
				dtSelected.Rows.Add(z.ItemArray);
				dt.Rows[indexSelected].Delete();

				//alreadySheet.Items.Refresh();
				//selectedSheet.Items.Refresh();

				alreadySheet.ItemsSource = dt.DefaultView;
				selectedSheet.ItemsSource = dtSelected.DefaultView;
				alreadySheet.Columns[0].Visibility = System.Windows.Visibility.Visible;
				alreadySheet.Columns[1].Visibility = System.Windows.Visibility.Visible;
				alreadySheet.Columns[2].Visibility = System.Windows.Visibility.Visible;
				//MessageBox.Show("dt còn số dòng: " + dt.Rows.Count);
			}
			catch { /*MessageBox.Show(ex.ToString()); */};
		}
		private void MenuItem_Click(object sender, RoutedEventArgs e)
		{
			//MessageBox.Show("This delete revision");
			Object o = gridRevision.SelectedItem;
			RevisionItem revSelected = o as RevisionItem;

			Revision rev = null;
			foreach (Revision rv in listRevision)
			{
				if (revSelected.Sequence == rv.Name)
				{
					rev = rv;
					break;
				}
			}
			using (Transaction t = new Transaction(mDoc))
			{
				t.Start("Delete Revision");
				mDoc.Delete(rev.Id);
				t.Commit();
			}
			listRevision = getAllRevision();
			listRevisionItem = new List<RevisionItem>();
			foreach (Revision revi in listRevision)
			{
				RevisionItem revisionItem = new RevisionItem();
				revisionItem.Sequence = revi.Name;
				revisionItem.Date = revi.RevisionDate;
				revisionItem.Issuedto = revi.IssuedTo;
				revisionItem.Issuedby = revi.IssuedBy;

				listRevisionItem.Add(revisionItem);
				//gridRevision.Items.Add(new RevisionItem() { Sequence = revisionItem.Sequence, Date = rev.RevisionDate, Isuedto = rev.IssuedTo, Isuedby = rev.IssuedBy });
			}
			//gridRevision.Items.Clear();
			gridRevision.ItemsSource = listRevisionItem;
		}

		private void selectedSheet_MouseDoubleClick(object sender, MouseButtonEventArgs e)
		{
			try
			{
				int indexSelected = selectedSheet.SelectedIndex;

				listAlreadySheet.Add(listGridSelected[indexSelected]);
				listGridSelected.RemoveAt(indexSelected);



				var z = dtSelected.Rows[indexSelected];
				dt.Rows.Add(z.ItemArray);
				dtSelected.Rows[indexSelected].Delete();

				//alreadySheet.Items.Refresh();
				//selectedSheet.Items.Refresh();
				alreadySheet.ItemsSource = dt.DefaultView;
				selectedSheet.ItemsSource = dtSelected.DefaultView;
				alreadySheet.Columns[0].Visibility = System.Windows.Visibility.Visible;
				alreadySheet.Columns[1].Visibility = System.Windows.Visibility.Visible;
				alreadySheet.Columns[2].Visibility = System.Windows.Visibility.Visible;
			}
			catch { };

		}

		private void alreadySheet_RightClick_Click(object sender, RoutedEventArgs e)
		{
			var listSelected = alreadySheet.SelectedItems;
			int i = 0;
			foreach (var o in listSelected)
			{
				var indexSelected = alreadySheet.Items.IndexOf(o);
				using (Transaction t = new Transaction(mDoc))
				{
					t.Start("Remove All Revision, View Sheet");
					//List<ElementId> lvs = vs.GetAdditionalRevisionIds() as List<ElementId>; 
					List<ElementId> lvc = new List<ElementId>();
					listAlreadySheet[indexSelected].SetAdditionalRevisionIds(lvc);

					dt.Rows[indexSelected][2] = String.Empty;
					t.Commit();
				}

				//int percent = (i + 1) * 100 / listGridSelected.Count;
				//progressBar.Dispatcher.Invoke(() => progressBar.Value = percent, System.Windows.Threading.DispatcherPriority.Background);
				//i++;
			}

			alreadySheet.ItemsSource = dt.DefaultView;//ConvertViewSheet2SheetItem(listAlreadySheet).OrderBy(x => x.SheetNumber);
			alreadySheet.Columns[0].Visibility = System.Windows.Visibility.Visible;
			alreadySheet.Columns[1].Visibility = System.Windows.Visibility.Visible;
			alreadySheet.Columns[2].Visibility = System.Windows.Visibility.Visible;

		}
		private void alreadySheet_AutoGeneratingColumn(object sender, DataGridAutoGeneratingColumnEventArgs e)
		{
			e.Column.Visibility = System.Windows.Visibility.Hidden;
		}

		private void listCheckbox_MouseMove(object sender, MouseEventArgs e)
		{
			AddorRemoveCol();
		}

		private void selectedSheet_AutoGeneratingColumn(object sender, DataGridAutoGeneratingColumnEventArgs e)
		{
			//e.Column.Visibility = System.Windows.Visibility.Hidden;
		}

		private void MenuItem_Click_1(object sender, RoutedEventArgs e)
		{
			//int i = 0;
			//foreach (Revision rv in listRevision)
			//{
			//    if (i == 0)
			//        continue;
			//    try
			//    {
			//        using (Transaction t = new Transaction(mDoc))
			//        {
			//            t.Start("Delete All Revision");
			//            try
			//            {
			//                mDoc.Delete(rv.Id);
			//            }
			//            catch { continue; }
			//            t.Commit();
			//        }
			//        i++;
			//    }
			//    catch (Exception ex) { /*continue;*/ MessageBox.Show(ex.ToString()); }

			//}
			if (listRevision.Count == 0)
				return;
			using (Transaction t = new Transaction(mDoc, "Delete All Revision"))
			{
				t.Start();
				for (int i = 0; i < listRevision.Count; i++)
				{
					if (i == 0)
						continue;
					mDoc.Delete(listRevision[i].Id);
					int percent = (i + 1) * 100 / listRevision.Count;
					progressBar.Dispatcher.Invoke(() => progressBar.Value = percent, System.Windows.Threading.DispatcherPriority.Send);
				}
				t.Commit();
			}

			listRevisionItem = new List<RevisionItem>();
			listRevision = new List<Revision>();
			listRevision = getAllRevision();
			foreach (Revision revi in listRevision)
			{
				RevisionItem revisionItem = new RevisionItem();
				revisionItem.Sequence = revi.Name;
				revisionItem.Date = revi.RevisionDate;
				revisionItem.Issuedto = revi.IssuedTo;
				revisionItem.Issuedby = revi.IssuedBy;

				listRevisionItem.Add(revisionItem);
				//gridRevision.Items.Add(new RevisionItem() { Sequence = revisionItem.Sequence, Date = rev.RevisionDate, Isuedto = rev.IssuedTo, Isuedby = rev.IssuedBy });
			}
			gridRevision.ItemsSource = listRevisionItem;
		}

		private void findSheet_KeyDown(object sender, KeyEventArgs e)
		{
			if (e.Key != System.Windows.Input.Key.Enter) return;

			string strText = findSheet.Text;
			if (!string.IsNullOrEmpty(strText))
			{
				if (listAlreadySheet != null)
				{
					//string strText = findSheet.Text;
					for (int i = 0; i < alreadySheet.Items.Count; i++)
					{
						DataRowView dataRowView = alreadySheet.Items[i] as DataRowView;
						string sheetnumber = dataRowView[0].ToString();
						//if (!sheetnumber.ToLower().Contains(strText.ToLower()))
						//if (!sheetnumber.ToLower().Contains(strText.ToLower()))
						//MatchCollection matches = Regex.Matches(sheetnumber.ToLower(), strText.ToLower());
						//if (matches.Count == 0)
						//StringComparison comp = StringComparison.OrdinalIgnoreCase;
						int index = sheetnumber.ToLower().IndexOf(strText.ToLower());
						if (index < 0)
						{
							DataGridRow row = (DataGridRow)alreadySheet.ItemContainerGenerator.ContainerFromIndex(i);
							//row.Visibility = System.Windows.Visibility.Visible;
							if (row == null)
							{
								var item = alreadySheet.Items[i];
								/* bring the data item (Product object) into view
                                 * in case it has been virtualized away */
								alreadySheet.ScrollIntoView(item);
								row = alreadySheet.ItemContainerGenerator.ContainerFromIndex(i) as DataGridRow;
							}

							row.Visibility = System.Windows.Visibility.Hidden;
							row.Visibility = System.Windows.Visibility.Collapsed;

							//dt.Rows[i].Delete();
							//dt.Rows[i].Delete();
							//row.Dispatcher.Invoke(() => row.Visibility = System.Windows.Visibility.Collapsed, System.Windows.Threading.DispatcherPriority.Send);
						}
						else
						{
							//MessageBox.Show(sheetnumber + " " + index + " " + alreadySheet.Items.Count);


						}
						alreadySheet.ScrollIntoView(alreadySheet.Items[0]);

						int percent = (i + 1) * 100 / alreadySheet.Items.Count;
						progressBar.Dispatcher.Invoke(() => progressBar.Value = percent, System.Windows.Threading.DispatcherPriority.Render);
					}
					if (alreadySheet.Items.Count != 0)
						alreadySheet.ScrollIntoView(alreadySheet.Items[0]);
					alreadySheet.UpdateLayout();

				}
			}
		}
		private void alreadySheet_Sorting(object sender, DataGridSortingEventArgs e)
		{
			this.Dispatcher.BeginInvoke((Action)delegate ()
			{
				//do format here.
				List<ViewSheet> tempView = new List<ViewSheet>();
				for (int i = 0; i < alreadySheet.Items.Count; i++)
				{
					DataRowView dataRowView = alreadySheet.Items[i] as DataRowView;
					string sheetnumber = dataRowView[0].ToString();
					foreach (ViewSheet vs in listAlreadySheet)
					{
						if (vs.SheetNumber == sheetnumber)
						{
							tempView.Add(vs);
						}
					}
					//MessageBox.Show(sheetnumber);
				}
				listAlreadySheet = new List<ViewSheet>();
				listAlreadySheet = tempView;
				StringBuilder sb = new StringBuilder();
				foreach (ViewSheet vs in listAlreadySheet)
				{
					sb.AppendFormat("{0}", vs.SheetNumber).AppendLine();
				}
				dt = new DataTable();
				dt = ((DataView)alreadySheet.ItemsSource).ToTable();

				alreadySheet.Items.Refresh();
				//MessageBox.Show(sb.ToString());
			}, null);
		}

		private void selectedSheet_Sorting(object sender, DataGridSortingEventArgs e)
		{
			this.Dispatcher.BeginInvoke((Action)delegate ()
			{
				//do format here.
				List<ViewSheet> tempView = new List<ViewSheet>();
				for (int i = 0; i < selectedSheet.Items.Count; i++)
				{
					DataRowView dataRowView = selectedSheet.Items[i] as DataRowView;
					string sheetnumber = dataRowView[0].ToString();
					foreach (ViewSheet vs in listGridSelected)
					{
						if (vs.SheetNumber == sheetnumber)
						{
							tempView.Add(vs);
						}
					}
					//MessageBox.Show(sheetnumber);
				}
				listGridSelected = new List<ViewSheet>();
				listGridSelected = tempView;
				StringBuilder sb = new StringBuilder();
				foreach (ViewSheet vs in listGridSelected)
				{
					sb.AppendFormat("{0}", vs.SheetNumber).AppendLine();
				}
				dtSelected = new DataTable();
				dtSelected = ((DataView)selectedSheet.ItemsSource).ToTable();

				selectedSheet.Items.Refresh();
				MessageBox.Show(sb.ToString());
			}, null);
		}

		private void alreadySheet_Add_RightClick_Click(object sender, RoutedEventArgs e)
		{
			//MessageBox.Show("Add");
			var listSelected = alreadySheet.SelectedItems;
			foreach (var o in listSelected)
			{
				var indexSelected = alreadySheet.Items.IndexOf(o);
				listGridSelected.Add(listAlreadySheet[indexSelected]);
				//listAlreadySheet.RemoveAt(indexSelected);

				var z = dt.Rows[indexSelected];
				dtSelected.Rows.Add(z.ItemArray);
				//dt.Rows[indexSelected].Delete();
			}
			for (int i = listSelected.Count - 1; i >= 0; i--)
			{
				var indexSelected = alreadySheet.Items.IndexOf(listSelected[i]);
				listAlreadySheet.RemoveAt(indexSelected);
				dt.Rows[indexSelected].Delete();
			}

			alreadySheet.ItemsSource = dt.DefaultView;
			selectedSheet.ItemsSource = dtSelected.DefaultView;
			alreadySheet.Columns[0].Visibility = System.Windows.Visibility.Visible;
			alreadySheet.Columns[1].Visibility = System.Windows.Visibility.Visible;
			alreadySheet.Columns[2].Visibility = System.Windows.Visibility.Visible;
		}

		private void btnReload_Click(object sender, RoutedEventArgs e)
		{
			listGridSelected = new List<ViewSheet>();
			listParaName = new List<string>();
			//listRevision = getAllRevision();


			listAlreadySheet = new List<ViewSheet>();
			listAlreadySheet = getAlreadySheet();

			//listParaName = new List<string>();
			//dt.Reset();
			dtSelected.Reset();
			//dt.Columns.Add("Sheet Number");
			//dt.Columns.Add("Sheet Name");
			//dt.Columns.Add("Current Revision");

			dt = convertAlreadySheet2DataTable(listAlreadySheet);
			dtSelected = dt.Clone();
			//alreadySheet.Items.Clear();
			alreadySheet.ItemsSource = dt.DefaultView;
			selectedSheet.ItemsSource = dtSelected.DefaultView;

			//alreadySheet.Columns[0].Visibility = System.Windows.Visibility.Visible;
			//alreadySheet.Columns[1].Visibility = System.Windows.Visibility.Visible;
			//alreadySheet.Columns[2].Visibility = System.Windows.Visibility.Visible;
			//alreadySheet.Items.Refresh();
			//MessageBox.Show(alreadySheet.Items.Count.ToString());          
		}

		private void alreadySheet_RemoveCurrentRev_RightClick_Click(object sender, RoutedEventArgs e)
		{
			var listSelected = alreadySheet.SelectedItems;
			int i = 0;
			foreach (var o in listSelected)
			{
				var indexSelected = alreadySheet.Items.IndexOf(o);
				using (Transaction t = new Transaction(mDoc))
				{
					t.Start("Remove Current Revision, View Sheet");
					//List<ElementId> lvs = vs.GetAdditionalRevisionIds() as List<ElementId>; 
					IList<ElementId> lvc = new List<ElementId>();
					lvc = listAlreadySheet[indexSelected].GetAllRevisionIds();
					ElementId currentRev = listAlreadySheet[indexSelected].GetCurrentRevision();

					lvc.Remove(currentRev);
					listAlreadySheet[indexSelected].SetAdditionalRevisionIds(lvc);
					t.Commit();
				}
				Revision rvs = mDoc.GetElement(listAlreadySheet[indexSelected].GetCurrentRevision()) as Revision;
				if (rvs == null)
				{
					dt.Rows[indexSelected][2] = String.Empty;
				}
				else
				{
					dt.Rows[indexSelected][2] = rvs.Name;
				}
				//int percent = (i + 1) * 100 / listGridSelected.Count;
				//progressBar.Dispatcher.Invoke(() => progressBar.Value = percent, System.Windows.Threading.DispatcherPriority.Background);
				//i++;
			}

			alreadySheet.ItemsSource = dt.DefaultView;//ConvertViewSheet2SheetItem(listAlreadySheet).OrderBy(x => x.SheetNumber);
			alreadySheet.Columns[0].Visibility = System.Windows.Visibility.Visible;
			alreadySheet.Columns[1].Visibility = System.Windows.Visibility.Visible;
			alreadySheet.Columns[2].Visibility = System.Windows.Visibility.Visible;
		}

		private void MenuItem_Click_2(object sender, RoutedEventArgs e)
		{
			var o = alreadySheet.SelectedItem;
			var indexSelected = alreadySheet.Items.IndexOf(o);
			mUIDoc.ActiveView = listAlreadySheet[indexSelected];
		}

		private void AddorRemoveCol()
		{
			List<int> indexChecked = new List<int>();
			if (listCheckbox.SelectedItems != null)
			{
				Dictionary<string, object> listChecked = new Dictionary<string, object>();
				listChecked = listCheckbox.SelectedItems;


				if (listChecked.Count != 0)
				{
					foreach (string showcol in listChecked.Keys)
					{
						int index = alreadySheet.Columns.Single(c => c.Header.ToString() == showcol).DisplayIndex;
						alreadySheet.Columns[index].Visibility = System.Windows.Visibility.Visible;
						indexChecked.Add(index);
					}
				}
			}

			for (int i = 3; i < alreadySheet.Columns.Count; i++)
			{
				if (!indexChecked.Contains(i))
					alreadySheet.Columns[i].Visibility = System.Windows.Visibility.Hidden;
			}
		}
	}
	public class RevisionItem
	{
		public string Sequence { get; set; }
		public string Date { get; set; }
		public string Issuedto { get; set; }
		public string Issuedby { get; set; }
	}

	//public class SheetItem
	//{
	//    public string SheetNumber { get; set; }
	//    public string SheetName { get; set; }
	//    public string CurrentRevision { get; set; }        
	//    public string HangMuc { get; set; }    
	//    public ParameterSet Para { get; set; }

	//    public SheetItem(ViewSheet vs)
	//    {
	//        SheetNumber = vs.SheetNumber;            
	//    }
	//    public SheetItem() { }
	//}

}
