using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Autodesk.Revit.DB;
using HoaBinhTools.PurgeViewsV2.Models;
using HoaBinhTools.PurgeViewsV2.Models.PurgeCmd;
using MoreLinq;
using Utils;

namespace HoaBinhTools.PurgeViewsV2.ViewModels
{
	public class PugreViewModels : ViewModelBase
	{
		#region 
		public PurgeViews WinDowsMain { get; set; }

		public RelayCommand btnDelete { get; set; }

		public RelayCommand btnCheckAllLinePattern { get; set; }
		public RelayCommand btnCheckNoneLinePattern { get; set; }

		public RelayCommand btnCheckAllFillPattern { get; set; }
		public RelayCommand btnCheckNoneFillPattern { get; set; }

		public RelayCommand btnCheckAllFillPara { get; set; }
		public RelayCommand btnCheckNoneFillPara { get; set; }

		public RelayCommand btnNotOnSheet { get; set; }

		public RelayCommand btnOnSheet { get; set; }

		public RelayCommand<bool> btnCheckLinePar { get; set; }

		public ObservableCollection<InfoView> allViews = ViewExtensation.GetInfoView();

		public ObservableCollection<InfoView> AllViews
		{
			get
			{
				return allViews;
			}
			set
			{
				allViews = value;
			}
		}


		public ObservableCollection<InfoView> allAllViewTemplate = ViewExtensation.GetInfoViewTemplate();

		public ObservableCollection<InfoView> AllViewTemplate
		{
			get
			{
				return allAllViewTemplate;
			}
			set
			{
				allAllViewTemplate = value;
			}
		}


		public ObservableCollection<InfoView> allViewSheet = ViewExtensation.GetInfoViewSheet();

		public ObservableCollection<InfoView> AllViewSheet
		{
			get
			{
				return allViewSheet;
			}
			set
			{
				allViewSheet = value;
			}
		}


		public ObservableCollection<InfoLink> allLinkFile = ViewExtensation.GetInfoFileLink();
		public ObservableCollection<InfoLink> AllLinkFile
		{
			get
			{
				return allLinkFile;
			}
			set
			{
				allLinkFile = value;
			}
		}


		public ObservableCollection<LinePatternModels> allLinePattern = PugreExtensation.GetLineParrtation();

		public ObservableCollection<LinePatternModels> AllLinePattern
		{
			get
			{
				return allLinePattern;
			}
			set
			{
				allLinePattern = value;

				OnPropertyChanged(nameof(AllLinePattern));
			}
		}

		public ObservableCollection<FillPatternsModel> allFillPattern = PugreExtensation.GetFillPattern();

		public ObservableCollection<FillPatternsModel> AllFillPattern
		{
			get
			{
				return allFillPattern;
			}
			set
			{
				allFillPattern = value;

				OnPropertyChanged(nameof(AllFillPattern));
			}
		}


		public ObservableCollection<ParameterModel> allParameter = PugreExtensation.GetAllParameter();

		public ObservableCollection<ParameterModel> AllParameter
		{
			get
			{
				return allParameter;
			}
			set
			{
				allParameter = value;

				OnPropertyChanged(nameof(AllParameter));
			}
		}


		public bool isPurgeUnused;

		public bool IsPurgeUnused
		{
			get
			{
				return isPurgeUnused;
			}
			set
			{
				isPurgeUnused = value;
			}
		}


		public bool isPurgeFamily;
		public bool IsPurgeFamily
		{
			get
			{
				return isPurgeFamily;
			}
			set
			{
				isPurgeFamily = value;
			}
		}

		#endregion

		public PugreViewModels()
		{



		}


		public void EventCheckPatter(bool check)
		{
			//MessageBox.Show(check.ToString());

			//var Libra = WinDowsMain.ListViewPart.SelectedItems.Cast<LinePatternModels>().ToList();

			//for (int i = 0; i < AllLinePattern.Count; i++)
			//{
			//    if (Libra.Contains(AllLinePattern[i]))
			//    {
			//        AllLinePattern[i].IsCheck = !check;
			//    }

			//}
		}


		public void OnSheet()
		{
			var Viewports = new FilteredElementCollector(ActiveData.Document).OfClass(typeof(Viewport)).Cast<Viewport>().ToList();

			if (Viewports.Count() > 0)
			{
				foreach (var Listinfview in allViews)
				{

					foreach (var infview in Listinfview.Models)
					{
						foreach (var myview in infview.Views)
						{
							View Vs = myview.view;

							if (!Viewports.OnSheet(Vs))
							{
								myview.IsCheck = false;
							}
							else
							{
								myview.IsCheck = true;
							}

						}
					}


				}
			}
			else
			{
				foreach (InfoView Listinfview in allViews)
				{
					foreach (var infview in Listinfview.Models)
					{
						foreach (var myview in infview.Views)
						{
							View Vs = myview.view;

							myview.IsCheck = false;

						}
					}


				}
			}
		}



		public void NotOnSheet()
		{
			var Viewports = new FilteredElementCollector(ActiveData.Document).OfClass(typeof(Viewport)).Cast<Viewport>().ToList();

			if (Viewports.Count() > 0)
			{
				foreach (var Listinfview in allViews)
				{

					foreach (var infview in Listinfview.Models)
					{
						foreach (var myview in infview.Views)
						{
							View Vs = myview.view;

							if (!Viewports.OnSheet(Vs))
							{
								myview.IsCheck = true;
							}
							else
							{
								myview.IsCheck = false;
							}

						}
					}


				}
			}
			else
			{
				foreach (InfoView Listinfview in allViews)
				{
					foreach (var infview in Listinfview.Models)
					{
						foreach (var myview in infview.Views)
						{
							View Vs = myview.view;

							myview.IsCheck = true;

						}
					}


				}
			}
		}


		protected void ButtonDelete()
		{
			Action action = new Action(() =>
			{
				DeleteView();

				DeleteSheet();

				DeleteSheetTemplate();

				DeleteFileLink();

				DeletePurgeUnused();

				DeleteTypeLinePattern();

				DeleteFillPattern();

				DeleteParameter();

				WinDowsMain.Close();

				PugreViewModels dt = new PugreViewModels();

				dt.Execute();


			});

			ExternalEventHandler.Instance.SetAction(action);

			ExternalEventHandler.Instance.Run();
		}

		public void DeleteParameter()
		{
			if (AllParameter.Where(e => e.IsCheck).Count() > 0)
			{
				ProgressBarView ProgressParameter = new ProgressBarView();

				Transaction transGroup = new Transaction(ActiveData.Document);

				ProgressParameter.Show();

				foreach (ParameterModel Para in AllParameter)
				{
					if (Para.IsCheck)
					{
						transGroup.Start("Delete Parameter");

						transGroup.SetFailuresPreprocessorInTransaction();

						try
						{
							ActiveData.Document.Delete(Para.Id);
						}
						catch
						{
						}

						if (!ProgressParameter.Create(AllParameter.Count, " Đang Xóa Parameter ")) break;

						transGroup.Commit();

					}
				}
				ProgressParameter.Close();
			}
		}

		public void DeleteFillPattern()
		{
			if (AllFillPattern.Where(e => e.IsCheck).Count() > 0)
			{

				using (Transaction transGroupPattern = new Transaction(ActiveData.Document))
				{

					ProgressBarView ProgressFillPatter = new ProgressBarView();

					ProgressFillPatter.Show();

					foreach (var fillpatter in AllFillPattern)
					{
						if (fillpatter.IsCheck)
						{
							if (fillpatter.Id != ElementId.InvalidElementId)
							{
								transGroupPattern.Start("Delete Fill Pattern");

								transGroupPattern.SetFailuresPreprocessorInTransaction();

								try
								{
									ActiveData.Document.Delete(fillpatter.Id);
								}
								catch
								{

								}

								if (!ProgressFillPatter.Create(AllFillPattern.Count, " Đang Xóa Fill Pattern ")) break;

								transGroupPattern.Commit();

							}

						}

					}

					ProgressFillPatter.Close();
				}


			}


		}

		public void DeleteTypeLinePattern()
		{
			if (AllLinePattern.Where(e => e.IsCheck).Count() > 0)
			{
				ProgressBarView ProgressFillPatter = new ProgressBarView();

				ProgressFillPatter.Show();


				List<bool> TF = AllLinePattern.Select(e => e.IsCheck).ToList();
				foreach (var linepatter in AllLinePattern)
				{

					if (linepatter.IsCheck)
					{

						if (linepatter.Id != ElementId.InvalidElementId)
						{

							Transaction transGroup = new Transaction(ActiveData.Document);
							transGroup.Start("Delete Line  Pattern");

							transGroup.SetFailuresPreprocessorInTransaction();
							if (!ProgressFillPatter.Create(AllLinePattern.Count, " Đang Xóa Line Pattern ")) break;

							try
							{
								ActiveData.Document.Delete(linepatter.Id);
							}
							catch (Exception ex)
							{

							}
							transGroup.Commit();

						}

					}
				}
				
				ProgressFillPatter.Close();
			}

		}

		public void DeletePurgeUnused()
		{
			if (IsPurgeUnused)
			{
				using (Transaction transGroup = new Transaction(ActiveData.Document))
				{
					transGroup.Start("Purge Unused");

					transGroup.SetFailuresPreprocessorInTransaction();

					PurgeModels.PurgeUnused();

					transGroup.Commit();
				}
			}

			if (IsPurgeFamily)
			{
				PurgeModels.PurgeFamily();
			}

		}

		public void DeleteView()
		{
			using (Transaction transGroup = new Transaction(ActiveData.Document))
			{


				ProgressBarView Deleteview = new ProgressBarView();
				Deleteview.Show();


				foreach (InfoView info in AllViews)
				{

					var Views = info.Models.Select(x => x.Views).Flatten().Cast<MyView>().Where(e => e.IsCheck == true).Select(e => e.view);

					if (!Deleteview.Create(AllViews.Count, " Đang Xóa View ")) break;

					if (Views.Count() > 0 && Views != null)
					{

						foreach (var v in Views)
						{
							transGroup.Start("Deleteview");
							transGroup.SetFailuresPreprocessorInTransaction();

							try
							{
								if (ActiveData.ActiveView.Id != v.Id)
								{
									ActiveData.Document.Delete(v.Id);
								}

							}
							catch
							{

							}


							transGroup.Commit();
						}
					}

					Deleteview.Close();
				}

			}
		}


		public void DeleteSheet()
		{
			using (Transaction transGroup = new Transaction(ActiveData.Document))
			{


				ProgressBarView Deleteview = new ProgressBarView();

				Deleteview.Show();

				foreach (InfoView info in AllViewSheet)
				{

					if (!Deleteview.Create(AllViewSheet.Count, " Đang Xóa Sheet ")) break;



					var Views = info.Models.Select(x => x.Views).Flatten().Cast<MyView>().Where(e => e.IsCheck == true).Select(e => e.view);

					if (Views.Count() > 0 && Views != null)
					{

						foreach (var v in Views)
						{
							transGroup.Start("Delete Sheet");

							transGroup.SetFailuresPreprocessorInTransaction();


							try
							{
								ActiveData.Document.Delete(v.Id);
							}
							catch
							{

							}

							transGroup.Commit();
						}
					}
				}

				Deleteview.Close();


			}


		}

		public void DeleteFileLink()
		{


			using (Transaction transGroup = new Transaction(ActiveData.Document))
			{


				ProgressBarView Deleteview = new ProgressBarView();

				Deleteview.Show();

				foreach (InfoLink info in AllLinkFile)
				{

					if (!Deleteview.Create(AllLinkFile.Count, " Đang Xóa File Link ")) break;

					IEnumerable<ElementId> IDs = info.Links.Where(e => e.IsCheck).Select(e => e.Id);



					if (IDs.Count() > 0)
					{

						foreach (var v in IDs)
						{

							transGroup.Start("Delete File Link");

							transGroup.SetFailuresPreprocessorInTransaction();


							try
							{
								ActiveData.Document.Delete(v);
							}
							catch
							{

							}

							transGroup.Commit();

						}
					}
				}

				Deleteview.Close();


			}

		}

		public void DeleteSheetTemplate()
		{

			using (Transaction transGroup = new Transaction(ActiveData.Document))
			{


				ProgressBarView Deleteview = new ProgressBarView();

				Deleteview.Show();

				foreach (InfoView info in AllViewTemplate)
				{

					if (!Deleteview.Create(AllViewTemplate.Count, " Đang Xóa ViewTeamplate ")) break;



					var Views = info.Models.Select(x => x.Views).Flatten().Cast<MyView>().Where(e => e.IsCheck == true).Select(e => e.view);

					if (Views.Count() > 0 && Views != null)
					{

						foreach (var v in Views)
						{
							try
							{

								transGroup.Start("Delete ViewTeamplate");

								transGroup.SetFailuresPreprocessorInTransaction();

								ActiveData.Document.Delete(v.Id);

								transGroup.Commit();
							}
							catch
							{

							}
						}
					}
				}

				Deleteview.Close();


			}

		}

		public void BtnCheckAllLinePattern()
		{
			foreach (LinePatternModels LPM in AllLinePattern)
			{
				LPM.IsCheck = true;
			}
		}

		public void BtnCheckNoneLinePattern()
		{
			foreach (LinePatternModels LPM in AllLinePattern)
			{
				LPM.IsCheck = false;
			}
		}

		public void BtnCheckAllFillPattern()
		{
			foreach (FillPatternsModel LPM in AllFillPattern)
			{
				LPM.IsCheck = true;
			}
		}

		public void BtnCheckNoneFillPattern()
		{
			foreach (FillPatternsModel LPM in AllFillPattern)
			{
				LPM.IsCheck = false;
			}
		}

		public void BtnCheckAllPara()
		{
			foreach (ParameterModel LPM in AllParameter)
			{
				LPM.IsCheck = true;
			}
		}

		public void BtnCheckNonePara()
		{
			foreach (ParameterModel LPM in AllParameter)
			{
				LPM.IsCheck = false;
			}
		}

		public void Execute()
		{
			btnDelete = new RelayCommand(ButtonDelete);

			btnNotOnSheet = new RelayCommand(NotOnSheet);

			btnOnSheet = new RelayCommand(OnSheet);

			btnCheckLinePar = new RelayCommand<bool>(EventCheckPatter);

			btnCheckAllLinePattern = new RelayCommand(BtnCheckAllLinePattern);

			btnCheckNoneLinePattern = new RelayCommand(BtnCheckNoneLinePattern);

			btnCheckAllFillPattern = new RelayCommand(BtnCheckAllFillPattern);

			btnCheckNoneFillPattern = new RelayCommand(BtnCheckNoneFillPattern);

			btnCheckAllFillPara = new RelayCommand(BtnCheckAllPara);

			btnCheckNoneFillPara = new RelayCommand(BtnCheckNonePara);

			WinDowsMain = new PurgeViews(this);

			WinDowsMain.Show();


		}

	}
}
