using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Net;
using System.Reflection;
using System.Text;
using System.Windows.Media.Imaging;
using Autodesk.Revit.UI;
using HoaBinhTools.QLUser;
using HoaBinhTools.QLUser.Models;
using HoaBinhTools.BIMQAQC.ModelChecker.ViewModels;
using Utils;
using Autodesk.Revit.UI.Events;
using Autodesk.Revit.DB;
using System;
using Autodesk.Revit.DB.Events;
using System.Text.RegularExpressions;
using System.Linq;

namespace HoaBinhTools.Apps
{
	public class ExternalApplication : IExternalApplication
	{
		public static UIControlledApplication _cachedUiCtrApp;
		public Result OnStartup(UIControlledApplication application)
		{
			_cachedUiCtrApp = application;

			RibbonPanel panel = CreateRibbonPanel(application, "BIM HBC Str", "Structure 1.6.1");

			RibbonPanel pane2 = CreateRibbonPanel(application, "BIM HBC Str", "Autocad2Revit");

			RibbonPanel panelnewBIM = CreateRibbonPanel(application, "BiMHoaBinh 2.5.7.0", "General 1.6.5");

			RibbonPanel panelnewBIM_Sync = CreateRibbonPanel(application, "BiMHoaBinh 2.5.7.0", "Sync Data");

			RibbonPanel panelnewBIMQAQC_Sync = CreateRibbonPanel(application, "BiMHoaBinh 2.5.7.0", "BIM_QAQC");

			string thisAsseabpatch = Assembly.GetExecutingAssembly().Location;

			List<string> Permis = Permission();

			try
			{
				#region GENERAL

				#region QAQC
				Bitmap PngQAQC = global::HoaBinhTools.Properties.Resources.timeline_check_outline;
				PushButton PusButQAQC = panelnewBIMQAQC_Sync.AddItem(new PushButtonData("BIM_QAQC", " QAQC Model ", thisAsseabpatch, "HoaBinhTools.BIMQAQC.ModelChecker.Command.QAQC_LaunchCommand")) as PushButton;
				PusButQAQC.LargeImage = GetBitMapImage(PngQAQC);

				Bitmap PngQAQCStr = global::HoaBinhTools.Properties.Resources.chart_tree;
				PushButton PusButQAQCStr = panelnewBIMQAQC_Sync.AddItem(new PushButtonData("BIM_QAQCStr", " QAQC Str ", thisAsseabpatch, "HoaBinhTools.BIMQAQC.QAQC_Quantity.Command.QAQC_QuantityCommand")) as PushButton;
				PusButQAQCStr.LargeImage = GetBitMapImage(PngQAQCStr);
				#endregion

				#region Syndata
				Bitmap PngSync = global::HoaBinhTools.Properties.Resources.dbSyncIcon;
				Bitmap PngCheck = global::HoaBinhTools.Properties.Resources.Sync;
				PushButton PusButSync = panelnewBIM_Sync.AddItem(new PushButtonData("SyncData", " SyncData ", thisAsseabpatch, "HoaBinhTools.SynchronizedData.Models.SyncDataCmd")) as PushButton;
				PusButSync.LargeImage = GetBitMapImage(PngSync);

				//PushButton PusButcount = panelnewBIM_Sync.AddItem(new PushButtonData("CountDbData", " Check ", thisAsseabpatch, "HoaBinhTools.SynchronizedData.Models.CheckUpdateCmd")) as PushButton;
				//PusButcount.LargeImage = GetBitMapImage(PngCheck);

				PushButtonData pushButtonDataCount = new PushButtonData("CountDbData", " Check ", thisAsseabpatch, "HoaBinhTools.SynchronizedData.Models.CheckUpdateCmd");
				TextBoxData textBoxData = new TextBoxData("TimeCheck");
				TextBoxData textBoxData2 = new TextBoxData("Count");

				IList<RibbonItem> stackedItem = panelnewBIM_Sync.AddStackedItems(pushButtonDataCount, textBoxData, textBoxData2);

				PushButton PusButcount = stackedItem[0] as PushButton;
				PusButcount.Image = GetBitMapImage(PngCheck);

				TextBox textBox = stackedItem[1] as TextBox;
				textBox.ToolTip = "Time Sync";
				textBox.Width = 150;

				TextBox textBox2 = stackedItem[2] as TextBox;
				textBox2.ToolTip = "Count";
				textBox2.Width = 150;


				#endregion

				#region purge
				Bitmap PngPurge = global::HoaBinhTools.Properties.Resources.PurgeImage;
				PushButton PusButSettingPurge = panelnewBIM.AddItem(new PushButtonData("Purge", " Purge v2 ", thisAsseabpatch, "HoaBinhTools.PurgeViewsV2.Models.PurgeCmd.PurgeCmd")) as PushButton;
				PusButSettingPurge.LargeImage = GetBitMapImage(PngPurge);


				Bitmap PngWarning = global::HoaBinhTools.Properties.Resources.PurgeImage;
				PushButton PusButSettingWarning = panelnewBIM.AddItem(new PushButtonData("Warning", " Warning v2 ", thisAsseabpatch, "HoaBinhTools.ProjectWarnings.WarningsCmd.WarningCmd")) as PushButton;
				PusButSettingWarning.LargeImage = GetBitMapImage(PngWarning);
				#endregion

				#region Schedule2Excel

				Bitmap S2Excel = global::HoaBinhTools.Properties.Resources.export_32;
				PushButtonData PusButSchedule2Excel = new PushButtonData("Schedule2Excel", " Schedule 2Excel ", thisAsseabpatch, "Schedule2Excel2k16.Schedule2ExcelCommand");
				PusButSchedule2Excel.LargeImage = GetBitMapImage(S2Excel);

				Bitmap excel2S = global::HoaBinhTools.Properties.Resources.import_32;
				PushButtonData PusButExcel2Schedule = new PushButtonData("Excel2Schedule", " Excel 2Schedule ", thisAsseabpatch, "Schedule2Excel2k16.Excel2ScheduleCommand");
				PusButExcel2Schedule.LargeImage = GetBitMapImage(excel2S);

				PushButtonData PusButExcelRebarShape = new PushButtonData("RebarShape", " RebarShape \n2Schedule ", thisAsseabpatch, "Schedule2Excel.Command.Schedule2ExcelCommand");
				PusButExcelRebarShape.LargeImage = GetBitMapImage(S2Excel);

				string ScheduleVersion = "1.0.1";
				string ScheduleName = "Schedule";
				SplitButtonData splitButtonDataSchedule = new SplitButtonData(ScheduleName.Replace(" ", string.Empty), $"{ScheduleName}\n{ScheduleVersion}");
				SplitButton splitButtonSchedule = panelnewBIM.AddItem(splitButtonDataSchedule) as SplitButton;
				splitButtonSchedule.AddPushButton(PusButSchedule2Excel);
				splitButtonSchedule.AddPushButton(PusButExcel2Schedule);
				splitButtonSchedule.AddPushButton(PusButExcelRebarShape);
				splitButtonSchedule.IsSynchronizedWithCurrentItem = false;
				#endregion

				#region SheetMarker

				Bitmap sheetmaker = global::HoaBinhTools.Properties.Resources.sheet_maker_32;
				PushButtonData PusButSheetMaker = new PushButtonData("SheetMaker", " Sheet Maker ", thisAsseabpatch, "SheetDuplicateAndAlignView.SheetMakerCommand");
				PusButSheetMaker.LargeImage = GetBitMapImage(sheetmaker);

				Bitmap duplicate = global::HoaBinhTools.Properties.Resources.duplicate_view_32;
				PushButtonData PusButDuplicateView = new PushButtonData("DuplicateView", " Duplicate View ", thisAsseabpatch, "SheetDuplicateAndAlignView.DuplicateViewCommand");
				PusButDuplicateView.LargeImage = GetBitMapImage(duplicate);

				Bitmap align = global::HoaBinhTools.Properties.Resources.duplicate_view_32;
				PushButtonData PusButAlignView = new PushButtonData("AlignView", " Align View ", thisAsseabpatch, "SheetDuplicateAndAlignView.AlignViewCommand");
				PusButAlignView.LargeImage = GetBitMapImage(align);

				Bitmap revision = global::HoaBinhTools.Properties.Resources.Revision_32;
				PushButtonData PusButRevision = new PushButtonData("Revision", " Revision MultiSheet ", thisAsseabpatch, "RevisionMultiSheet.RevisionCommand");
				PusButRevision.LargeImage = GetBitMapImage(revision);

				string SheetmakerVersion = "1.0.1";
				string SheetmakerName = "SheetMaker";
				SplitButtonData splitButtonDataSheetMaker = new SplitButtonData(SheetmakerName.Replace(" ", string.Empty), $"{SheetmakerName}\n{SheetmakerVersion}");
				SplitButton splitButtonSheetMaker = panelnewBIM.AddItem(splitButtonDataSheetMaker) as SplitButton;
				splitButtonSheetMaker.AddPushButton(PusButSheetMaker);
				splitButtonSheetMaker.AddPushButton(PusButDuplicateView);
				splitButtonSheetMaker.AddPushButton(PusButAlignView);
				splitButtonSheetMaker.AddPushButton(PusButRevision);
				splitButtonSheetMaker.IsSynchronizedWithCurrentItem = false;
				#endregion

				#region NumberSheet
				Bitmap sheetStt = global::HoaBinhTools.Properties.Resources.Sheetstt;
				PushButtonData PusButSheetStt = new PushButtonData("SheetStt", " STT \nSheet ", thisAsseabpatch, "CreateRibbonTab.OrderingNumberForSheetCommand");
				PusButSheetStt.LargeImage = GetBitMapImage(sheetStt);

				Bitmap sheetNumber = global::HoaBinhTools.Properties.Resources.SheetNumber;
				PushButtonData PusButSheetNumber = new PushButtonData("Sheet Number", " Sheet \nNumber ", thisAsseabpatch, "CreateRibbonTab.AutoSheetNumberCommand");
				PusButSheetNumber.LargeImage = GetBitMapImage(sheetNumber);

				string SheetNumberVersion = "1.0.1";
				string SheetNumberName = "SheetNumber";
				SplitButtonData splitButtonDataSheet = new SplitButtonData(SheetNumberName.Replace(" ", string.Empty), $"{SheetNumberName}\n{SheetNumberVersion}");
				SplitButton splitButtonSheet = panelnewBIM.AddItem(splitButtonDataSheet) as SplitButton;
				splitButtonSheet.AddPushButton(PusButSheetStt);
				splitButtonSheet.AddPushButton(PusButSheetNumber);
				splitButtonSheet.IsSynchronizedWithCurrentItem = false;
				#endregion

				#region DuplicatingSheets
				Bitmap pngDup32 = global::HoaBinhTools.Properties.Resources.dup_32;    // Nút vào user
				PushButton PusDuplicate = panelnewBIM.AddItem(new PushButtonData("DuplicateSheets", " DuplicateSheets ", thisAsseabpatch, "DuplicatingSheets.Command")) as PushButton;
				PusDuplicate.LargeImage = GetBitMapImage(pngDup32);
				#endregion

				#endregion

				#region STRUCTURAL
				#region Learn Concrete
				Bitmap Png = global::HoaBinhTools.Properties.Resources.BTL;     // Nút lệnh bê tông lót
				PushButton PusButLeanConcrete = panel.AddItem(new PushButtonData("LeanConcrete", " Lean Concrete ", thisAsseabpatch, "HoaBinhTools.LeanConcrete.Models.LeanConcreteCmd")) as PushButton;
				PusButLeanConcrete.LargeImage = GetBitMapImage(Png);
				PusButLeanConcrete.ToolTip = "Dựng lớp bê tông lót của các đối tượng dầm, móng, ...";
				#endregion

				#region Framing
				Bitmap framing = global::HoaBinhTools.Properties.Resources.beam_n;
				PushButton PusButFramingInformation = panel.AddItem(new PushButtonData("FramingInformation", " Beam Rebar ", thisAsseabpatch, "HoaBinhTools.FramingInformation.FramingInformationCmd")) as PushButton;
				PusButFramingInformation.LargeImage = GetBitMapImage(framing);

				//Bitmap settingframing = global::HoaBinhTools.Properties.Resources.Image_Setting_32;
				//PushButtonData PusButSettingGeoInfo = new PushButtonData("SettingGeoInfo", " Setting GeoInfo ", thisAsseabpatch, "HoaBinhTools.FramingInformation.SettingGeoInfoRevitCmd");
				//PusButSettingGeoInfo.LargeImage = GetBitMapImage(settingframing);

				//string FramingInfors = "1.0.1";
				//string FramingInforstName = "Framing Information";
				//SplitButtonData splitButtonDataFraming = new SplitButtonData(FramingInforstName.Replace(" ", string.Empty), $"{FramingInforstName}\n{FramingInfors}");
				//SplitButton splitButtonFraming = panel.AddItem(splitButtonDataFraming) as SplitButton;
				//splitButtonFraming.AddPushButton(PusButFramingInformation);
				//splitButtonFraming.AddPushButton(PusButSettingGeoInfo);
				//splitButtonFraming.IsSynchronizedWithCurrentItem = false;
				#endregion

				#region Bộ tool BIMHBC cũ
				//Structural
				Bitmap beamMark = global::HoaBinhTools.Properties.Resources.set_mark_beam_32;
				PushButton PusButBeamMark = panel.AddItem(new PushButtonData("BeamMark", " Beam Mark ", thisAsseabpatch, "CreateRibbonTab.SetMarkForBeam")) as PushButton;
				PusButBeamMark.LargeImage = GetBitMapImage(beamMark);

				//Rebar
				Bitmap filterRebar = global::HoaBinhTools.Properties.Resources.filter_rebar_32;
				PushButton PusButFilterRebar = panel.AddItem(new PushButtonData("FilterRebar", " Filter Rebar ", thisAsseabpatch, "CreateRibbonTab.filterRebar")) as PushButton;
				PusButFilterRebar.LargeImage = GetBitMapImage(filterRebar);

				Bitmap SetPartition = global::HoaBinhTools.Properties.Resources.set_rebar_partiton_32;
				PushButton PusButSetPartion = panel.AddItem(new PushButtonData("SetPartition", " SetPartition ", thisAsseabpatch, "CreateRibbonTab.SetRebarPartition")) as PushButton;
				PusButSetPartion.LargeImage = GetBitMapImage(SetPartition);
				#endregion

				#region AutoCad 2 Revit
				Bitmap autoCad = global::HoaBinhTools.Properties.Resources.autocad;
				PushButtonData PusButFoundationModel = new PushButtonData("Autocad2Revit_Foundation", " Foundation model ", thisAsseabpatch,
					"HoaBinhTools.AutocadToRevit.FoudationSlab.Models.FoundationSlabCmd.FoundationSlabCmd") as PushButtonData;
				PusButFoundationModel.LargeImage = GetBitMapImage(autoCad);

				PushButtonData PusButColumnModel = new PushButtonData("Autocad2Revit_Column", " Column model ", thisAsseabpatch,
					"HoaBinhTools.AutocadToRevit.Column.Models.ColumnCmd.ColumnCmd") as PushButtonData;
				PusButColumnModel.LargeImage = GetBitMapImage(autoCad);

				PushButtonData PusButWallModel = new PushButtonData("Autocad2Revit_Wall", " Wall model ", thisAsseabpatch,
					"HoaBinhTools.AutocadToRevit.Wall.Models.Wallcmd.Wallcmd") as PushButtonData;
				PusButWallModel.LargeImage = GetBitMapImage(autoCad);

				PushButtonData PusButBeamModel = new PushButtonData("Autocad2Revit_Beam", " Beam model ", thisAsseabpatch,
					"HoaBinhTools.AutocadToRevit.Beam.Models.BeamCmd.BeamCmd") as PushButtonData;
				PusButBeamModel.LargeImage = GetBitMapImage(autoCad);

				string AutocadVersion = "1.0.1";
				string AutocadName = "Autocad2revit";
				SplitButtonData splitButtonDataAutocad = new SplitButtonData(AutocadName.Replace(" ", string.Empty), $"{AutocadName}\n{AutocadVersion}");
				SplitButton splitButtonAutocacd2Revit = pane2.AddItem(splitButtonDataAutocad) as SplitButton;
				splitButtonAutocacd2Revit.AddPushButton(PusButFoundationModel);
				splitButtonAutocacd2Revit.AddPushButton(PusButColumnModel);
				splitButtonAutocacd2Revit.AddPushButton(PusButWallModel);
				splitButtonAutocacd2Revit.AddPushButton(PusButBeamModel);
				splitButtonAutocacd2Revit.IsSynchronizedWithCurrentItem = true;
				#endregion
				#endregion

				if (Permis[0] == "true" || Permis[1] == "true") // All hoặc General
				{
					PusButSettingPurge.Enabled = true;
					PusButSettingWarning.Enabled = true;
					splitButtonSchedule.Enabled = true;
					splitButtonSheetMaker.Enabled = true;
					splitButtonSheet.Enabled = true;
					PusDuplicate.Enabled = true;
				}
				else
				{
					PusButSettingPurge.Enabled = false;
					PusButSettingWarning.Enabled = false;
					splitButtonSchedule.Enabled = false;
					splitButtonSheetMaker.Enabled = false;
					splitButtonSheet.Enabled = false;
					PusDuplicate.Enabled = false;
				}

				if (Permis[0] == "true" || Permis[2] == "true") // All hoặc Arc
				{
				}

				if (Permis[0] == "true" || Permis[3] == "true") // All hoặc structural
				{
					PusButLeanConcrete.Enabled = true;
					PusButFramingInformation.Enabled = true;
					PusButBeamMark.Enabled = true;
					PusButFilterRebar.Enabled = true;
					PusButSetPartion.Enabled = true;
					splitButtonAutocacd2Revit.Enabled = true;
				}
				else
				{
					PusButLeanConcrete.Enabled = false;
					PusButFramingInformation.Enabled = false;
					PusButBeamMark.Enabled = false;
					PusButFilterRebar.Enabled = false;
					PusButSetPartion.Enabled = false;
					splitButtonAutocacd2Revit.Enabled = false;
				}
				
				application.ControlledApplication.DocumentOpened += new EventHandler<DocumentOpenedEventArgs>(application_DocumentOpened);
				
				//Check autorun
				if (QAQCSetting.Default.IsAutoRun == true)
				{
					DateTime d1 = DateTime.Now;
					DateTime d2 = DateTime.Now.AddDays(-2);
					try
					{
						d2 = QAQCSetting.Default.ExecuteTime;
					}
					catch
					{ }
					TimeSpan t = d1 - d2;

					//t.Days>0
					if (t.Days > 0)
					{
						if (QAQCSetting.Default.TimeTrigers == "Day")
						{
							application.DialogBoxShowing += UiAppOnDialogBoxShowing;
							application.Idling += OnIdling;
						}
						else if (QAQCSetting.Default.TimeTrigers == "Week")
						{
							if (QAQCSetting.Default.DatesApps == d1.DayOfWeek.ToString())
							{
								string hour = QAQCSetting.Default.TimeApps;
								hour = hour.Replace("~", "");
								hour = hour.Replace("h", "");
								hour = hour.Replace("am", "");
								hour = hour.Replace("pm", "");
								if (QAQCSetting.Default.TimeApps.Contains("pm"))
									hour = (int.Parse(hour) + 12).ToString();
								if (d1.Hour <= int.Parse(hour))
								{
									application.DialogBoxShowing += UiAppOnDialogBoxShowing;
									application.Idling += OnIdling;
								}
							}
						}
						else if (QAQCSetting.Default.TimeTrigers == "Month")
						{
							string day = QAQCSetting.Default.DatesApps;
							day = day.Replace("st", "");
							day = day.Replace("rd", "");
							day = day.Replace("nd", "");
							day = day.Replace("th", "");
							if (int.Parse(day) == int.Parse(d1.Day.ToString()))
							{
								string hour = QAQCSetting.Default.TimeApps;
								hour = hour.Replace("~", "");
								hour = hour.Replace("h", "");
								hour = hour.Replace("am", "");
								hour = hour.Replace("pm", "");
								if (QAQCSetting.Default.TimeApps.Contains("pm"))
									hour = (int.Parse(hour) + 12).ToString();
								if (d1.Hour <= int.Parse(hour))
								{
									application.DialogBoxShowing += UiAppOnDialogBoxShowing;
									application.Idling += OnIdling;
								}
							}
						}
					}
				}
				return Result.Succeeded;
			}
			catch { return Result.Cancelled; }
		}

		public Result OnShutdown(UIControlledApplication application)
		{
			application.Idling -= OnIdling;
			application.ControlledApplication.DocumentOpened -= application_DocumentOpened;
			application.DialogBoxShowing -= UiAppOnDialogBoxShowing;
			return Result.Succeeded;
		}

		private void OnIdling(object sender, IdlingEventArgs e)
		{

			UIApplication evenUI = sender as UIApplication;
			if (evenUI == null)
				return;

			Autodesk.Revit.ApplicationServices.Application app = evenUI.Application;
			if (app != null)
			{
				app.FailuresProcessing += FaliureProcessor;
				DateTime d1 = DateTime.Now;

				string hour = QAQCSetting.Default.TimeApps;
				hour = hour.Replace("~", "");
				hour = hour.Replace("h", "");
				hour = hour.Replace("am", "");
				hour = hour.Replace("pm", "");
				if (QAQCSetting.Default.TimeApps.Contains("pm"))
					hour = (int.Parse(hour) + 12).ToString();

				if (d1.Hour == int.Parse(hour))
				{
					UIApplication uiapp = sender as UIApplication;
					Document document = uiapp.ActiveUIDocument.Document;
					if (uiapp != null)
					{
						try
						{
							LaunchViewModels LaunchViewModels = new LaunchViewModels(uiapp);
							LaunchViewModels.GetInforListFile();
							//LaunchViewModels.Excute();
							QAQCSetting.Default.ExecuteTime = d1;
							QAQCSetting.Default.Save();
							_cachedUiCtrApp.Idling -= OnIdling;
						}
						catch
						{ }
					}
				}
			}
		}

		public void application_DocumentOpened(object sender, DocumentOpenedEventArgs args)
		{
			Document document = args.Document;

			string filePath = GetInforOrtherFile.GetfileName(document);
				var s = filePath.Split('\\');
				string fileName = s[s.Length - 1];
				string version = document.Application.VersionName;

				string pattern = @"^(?<Project>(.+))-HBC_(?<Discipline>(A|S|M))?-";
				Regex reg = new Regex(pattern);

				string Project = "";
				string Discipline = "";
				foreach (Match result in reg.Matches(fileName))
				{
					Project = result.Groups["Project"].ToString();
					Discipline = result.Groups["Discipline"].ToString();
				}

				//Điền tên 
				if (Discipline != "" && Project != "")
				{
					string url = $"https://script.google.com/macros/s/AKfycbzrc0RftlgusrsHDzcC4G1SYdDQp0EunaAtu-jSZvfUQ653Brs/exec?Project={Project}&Discipline={Discipline}&FileName={fileName}&FilePath={filePath}&Rvv={version}";
					HttpWebRequest req;
					HttpWebResponse res = null;
					req = (HttpWebRequest)WebRequest.Create(url);
					res = (HttpWebResponse)req.GetResponse();
					res.Close();
				}
				else
				{
					string patterncheck = @"^Project([0-9]+)";
					Regex reg1 = new Regex(patterncheck);

					if (reg1.Match(fileName).Success == false)
					{
						string url = $"https://script.google.com/macros/s/AKfycbwZvNLk-bY3tMJCSfIIrOelzFz9xvZIeEDI74LZ/exec?FileName={fileName}&FilePath={filePath}&Rvv={version}";
						HttpWebRequest req;
						HttpWebResponse res = null;
						req = (HttpWebRequest)WebRequest.Create(url);
						res = (HttpWebResponse)req.GetResponse();
						res.Close();
					}
				}
		}

		public BitmapImage GetBitMapImage(Bitmap Bm)
		{
			var memory = new MemoryStream();
			Bm.Save(memory, ImageFormat.Png);
			memory.Position = 0;
			var BmImage = new BitmapImage();
			BmImage.BeginInit();
			BmImage.StreamSource = memory;
			BmImage.CacheOption = BitmapCacheOption.OnLoad;
			BmImage.EndInit();
			BmImage.Freeze();

			return BmImage;
		}
		public RibbonPanel CreateRibbonPanel(UIControlledApplication a, string NameTab, string NamePanel)
		{
			string tab = NameTab;

			RibbonPanel ribbonPanel = null;
			try
			{
				a.CreateRibbonTab(tab);
			}
			catch { }

			try
			{
				RibbonPanel panel = a.CreateRibbonPanel(tab, NamePanel);
			}
			catch { }

			List<RibbonPanel> panels = a.GetRibbonPanels(tab);

			foreach (RibbonPanel p in panels)
			{
				if (p.Name == NamePanel)
				{
					ribbonPanel = p;
				}

			}

			return ribbonPanel;

		}

		//Array quyền truy cập vào các addin
		private List<string> Permission()
		{
			HddSerialNumber hdd = new HddSerialNumber();
			string hddSerial = hdd.GetHDDSerialNumber("");
			string ComputerUser = hdd.ComputerName();

			PutVersiondata(hddSerial, ComputerUser);

			string url = string.Format("https://script.google.com/macros/s/AKfycbwpelUfS-l-9okUm8fufz_M1Mb5cBRuO2Q3uWOqWnFHVVIzqoQ/exec?id={0}&username={1}", hddSerial, ComputerUser);
			HttpWebRequest req;
			HttpWebResponse res = null;

			SaveUser.Default.Hdd = hddSerial;
			SaveUser.Default.Computer = ComputerUser;
			SaveUser.Default.Save();

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

				List<string> permission = new List<string>();
				var p = user.Split('|');

				if (p.Length > 3)
				{
					for (int i = 0; i < p.Length; i++)
					{
						p[i] = p[i].Replace("\0", "");
						permission.Add(p[i]);
					}
				}
				else
				{
					for (int i = 0; i < 5; i++)
					{
						permission.Add("false");
					}
				}
				return permission;

			}
			finally
			{
				if (res != null)
					res.Close();
			}
		}

		//Update khi đưa lên ggdriver
		public void PutVersiondata(string hddSerial, string ComputerUser)
		{
			//AKfycbwd7K7WNUVZCM6Cs83N6E2vcU5PBub9HyzXUJWiRt0kLUmAmUy4
			string url = string.Format("https://script.google.com/macros/s/AKfycbwd7K7WNUVZCM6Cs83N6E2vcU5PBub9HyzXUJWiRt0kLUmAmUy4/exec?Id={0}&UserName={1}&verAddin=200722", hddSerial, ComputerUser);
			HttpWebRequest req;
			HttpWebResponse res = null;

			req = (HttpWebRequest)WebRequest.Create(url);
			res = (HttpWebResponse)req.GetResponse();
			res.Close();
		}

		private string Process(byte[] data, int read)
		{
			string v = (ASCIIEncoding.ASCII.GetString(data));
			return v;
		}

		//private static async void UiAppOnDialogBoxShowing(object sender, DialogBoxShowingEventArgs args)
		//{
		//	switch (args)
		//	{
		//		// (Konrad) Dismiss Unresolved References pop-up.
		//		case TaskDialogShowingEventArgs args2:
		//			if (args2.DialogId == "TaskDialog_Unresolved_References")
		//				args2.OverrideResult(1002);
		//			break;
		//		// (Konrad) Dismiss View Title Warning pop-up.
		//		case DialogBoxShowingEventArgs args4:
		//			if (args4.DialogId == "Dialog_Revit_DocWarnDialog")
		//				await Win32Api.ClickOk();
		//			break;
		//		default:
		//			return;
		//	}
		//}

		void UiAppOnDialogBoxShowing(object sender, DialogBoxShowingEventArgs e)
		{
			// MessageBox.Show("Got you!!!!");
			TaskDialogShowingEventArgs e2
			= e as TaskDialogShowingEventArgs;

			string s = string.Empty;

			if (null != e2)
			{
				/*
				TaskDialogResult
				None = 0,
				Ok = 1,
				Cancel = 2,
				Retry = 4,
				Yes = 6,
				No = 7,
				Close = 8,
				CommandLink1 = 1001,
				CommandLink2 = 1002,
				CommandLink3 = 1003,
				CommandLink4 = 1004
				*/
				bool isConfirm = false;
				int dialogResult = 0;

				if (e2.DialogId.Equals("TaskDialog_Update_Resources"))
				{
					isConfirm = true;
					dialogResult = (int)TaskDialogResult.CommandLink1;
				}
				else if (e2.DialogId.Equals("TaskDialog_Unresolved_References"))
				{
					isConfirm = true;
					dialogResult = (int)TaskDialogResult.CommandLink2;
				}
				else if (e2.DialogId.Equals("TaskDialog_Missing_Third_Party_Updaters"))
				{
					isConfirm = true;
					dialogResult = (int)TaskDialogResult.CommandLink2;
				}
				else if (e2.DialogId.Equals("TaskDialog_Missing_Third_Party_Updater"))
				{
					isConfirm = true;
					dialogResult = (int)TaskDialogResult.CommandLink2;
				}
				else if (e2.DialogId.Equals("TaskDialog_Local_Changes_Not_Synchronized_With_Central"))
				{
					isConfirm = true;
					dialogResult = (int)TaskDialogResult.CommandLink2;
				}
				else if (e2.DialogId.Equals("TaskDialog_Default_Family_Template_File_Invalid"))
				{
					isConfirm = true;
					dialogResult = (int)TaskDialogResult.Close;
				}
				else if (e2.DialogId.StartsWith("TaskDialog"))
				{
					isConfirm = true;
					dialogResult = (int)TaskDialogResult.Close;
				}
				else if (e2.Message.Equals("Some numerical data within the imported file was out of range.  This numerical data has been truncated."))
				{
					isConfirm = true;
					dialogResult = (int)TaskDialogResult.Close;
				}
				else if (e2.Message.Equals("Some entities were lost during import."))
				{
					isConfirm = true;
					dialogResult = (int)TaskDialogResult.Close;
				}

				if (isConfirm)
				{
					e2.OverrideResult(dialogResult);
					s += ", auto-confirmed.";
				}
				else
				{
					s = string.Format(
					", dialog id {0}, message '{1}'",
					e2.DialogId, e2.Message);
				}
			}


		}
		private void FaliureProcessor(object sender, FailuresProcessingEventArgs e)
		{
			bool hasFailure = false;
			FailuresAccessor fas = e.GetFailuresAccessor();
			List<FailureMessageAccessor> fma = fas.GetFailureMessages().ToList();
			List<ElementId> ElemntsToDelete = new List<ElementId>();
			foreach (FailureMessageAccessor fa in fma)
			{
				try
				{
					List<ElementId> FailingElementIds = fa.GetFailingElementIds().ToList();
					ElementId FailingElementId = FailingElementIds[0];
					if (!ElemntsToDelete.Contains(FailingElementId))
					{
						ElemntsToDelete.Add(FailingElementId);
					}

					hasFailure = true;
					fas.DeleteWarning(fa);

				}
				catch (Exception ex)
				{
				}
			}
			if (ElemntsToDelete.Count > 0)
			{
				fas.DeleteElements(ElemntsToDelete);
			}
			if (hasFailure)
			{
				e.SetProcessingResult(FailureProcessingResult.ProceedWithCommit);
			}
			e.SetProcessingResult(FailureProcessingResult.Continue);
		}

	}
}
