using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Media;
using HoaBinhTools.FramingInformation.Db;
//using HoaBinhTools.FramingInformation.LocalDbFraming;
//using HoaBinhTools.FramingInformation.Models.CreateRebar;
using HoaBinhTools.FramingInformation.ViewModels;
using HoaBinhTools.FramingInformation.Views;

namespace HoaBinhTools.FramingInformation.Models.DrawSectionCanvas
{
	public class SectionStirrup
	{
		public FramingInfoViewModels MainFramingViewModel;
		public ObservableCollection<SStirrup> SStirrups;
		public ObservableCollection<Stirrup> Stirrups;
		public ObservableCollection<DbAddT1> AddT1s;
		public ObservableCollection<DbAddT2> AddT2s;
		public ObservableCollection<DbAddB2> AddB2s;
		public ObservableCollection<DbAddB1> AddB1s;

		public void DrawSection(FramingInfoViewModels MainFramingViewModel, FramingInfoView FormMain, MovableRestBeam Movab)
		{
			this.MainFramingViewModel = MainFramingViewModel;

			//Xóa hình cũ
			List<UIElement> itemsToremove = new List<UIElement>();
			foreach (UIElement ui in FormMain.myCanvas.Children)
			{
				itemsToremove.Add(ui);
			}

			foreach (UIElement ui in itemsToremove)
			{
				FormMain.myCanvas.Children.Remove(ui);
			}

			if (Movab.Type.ToString() == "ND")
			{
				//Check tỉ lệ
				double B = (double)Movab.Width;
				double H = (double)Movab.Hight;

				double tl = 200 / H;
				tl = tl < 200 / B ? tl : 200 / H;

				double b = B * tl;
				double h = H * tl;

				double[] px =
				{
					(270 - b) / 2,
					270 - (270 - b) / 2,
					270 - (270 - b) / 2,
					(270 - b) / 2,
					(270 - b) / 2,
				};

				double[] py =
				{
					(270 - h) / 2,
					(270 - h) / 2,
					270 - (270 - h) / 2,
					270 - (270 - h) / 2,
					(270 - h) / 2,
				};

				drawDim_Hor(FormMain, B.ToString(), px[0], py[3], px[1], py[2]);

				drawDim_Ver(FormMain, H.ToString(), px[1], py[1], px[2], py[2]);

				PointCollection points = new PointCollection();
				for (int i = 0; i < 5; i++)
				{
					points.Add(new Point(px[i], py[i]));
				}

				//Đường bao
				System.Windows.Shapes.Polyline pl = new System.Windows.Shapes.Polyline();
				pl.Points = points;
				pl.Stroke = System.Windows.Media.Brushes.Black;
				pl.Name = "DuongBao";
				FormMain.myCanvas.Children.Add(pl);

				//Vẽ thép đai
				#region Đai chủ
				System.Windows.Shapes.Ellipse e1 = new System.Windows.Shapes.Ellipse();
				e1.Width = 10;
				e1.Height = 10;
				e1.Stroke = System.Windows.Media.Brushes.Magenta;
				e1.Margin = new Thickness(px[0] + 10, py[0] + 10, 10, 10);
				e1.Clip = new RectangleGeometry { Rect = new Rect(0, 0, 5, 5) };
				FormMain.myCanvas.Children.Add(e1);


				System.Windows.Shapes.Ellipse e2 = new System.Windows.Shapes.Ellipse();
				e2.Width = 10;
				e2.Height = 10;
				e2.Stroke = System.Windows.Media.Brushes.Magenta;
				e2.Margin = new Thickness(px[1] - 20, py[0] + 10, 10, 10);
				e2.Clip = new RectangleGeometry { Rect = new Rect(5, 0, 5, 5) };
				FormMain.myCanvas.Children.Add(e2);

				System.Windows.Shapes.Ellipse e3 = new System.Windows.Shapes.Ellipse();
				e3.Width = 10;
				e3.Height = 10;
				e3.Stroke = System.Windows.Media.Brushes.Magenta;
				e3.Margin = new Thickness(px[1] - 20, py[3] - 20, 10, 10);
				e3.Clip = new RectangleGeometry { Rect = new Rect(5, 5, 5, 5) };
				FormMain.myCanvas.Children.Add(e3);

				System.Windows.Shapes.Ellipse e4 = new System.Windows.Shapes.Ellipse();
				e4.Width = 10;
				e4.Height = 10;
				e4.Stroke = System.Windows.Media.Brushes.Magenta;
				e4.Margin = new Thickness(px[0] + 10, py[3] - 20, 10, 10);
				e4.Clip = new RectangleGeometry { Rect = new Rect(0, 5, 5, 5) };
				FormMain.myCanvas.Children.Add(e4);


				System.Windows.Shapes.Line line_1 = new System.Windows.Shapes.Line();
				line_1.Stroke = System.Windows.Media.Brushes.Magenta;
				line_1.X1 = px[0] + 15;
				line_1.X2 = px[1] - 15;
				line_1.Y1 = py[0] + 10;
				line_1.Y2 = py[0] + 10;
				line_1.Name = "Nhap_chu_12";
				FormMain.myCanvas.Children.Add(line_1);

				System.Windows.Shapes.Line line_2 = new System.Windows.Shapes.Line();
				line_2.Stroke = System.Windows.Media.Brushes.Magenta;
				line_2.X1 = px[1] - 10;
				line_2.X2 = px[1] - 10;
				line_2.Y1 = py[1] + 15;
				line_2.Y2 = py[2] - 15;
				line_2.Name = "Nhap_chu_24";
				FormMain.myCanvas.Children.Add(line_2);

				System.Windows.Shapes.Line line_3 = new System.Windows.Shapes.Line();
				line_3.Stroke = System.Windows.Media.Brushes.Magenta;
				line_3.X1 = px[0] + 10;
				line_3.X2 = px[0] + 10;
				line_3.Y1 = py[1] + 15;
				line_3.Y2 = py[2] - 15;
				line_3.Name = "Nhap_chu_13";
				FormMain.myCanvas.Children.Add(line_3);

				System.Windows.Shapes.Line line_4 = new System.Windows.Shapes.Line();
				line_4.Stroke = System.Windows.Media.Brushes.Magenta;
				line_4.X1 = px[0] + 15;
				line_4.X2 = px[1] - 15;
				line_4.Y1 = py[2] - 10;
				line_4.Y2 = py[2] - 10;
				line_4.Name = "Nhap_chu_24";
				FormMain.myCanvas.Children.Add(line_4);
				#endregion

				#region Thép chủ

				int slT1 = this.MainFramingViewModel.SL_T1 > 1 ? this.MainFramingViewModel.SL_T1 : 2;

				double kc_X = (b - 34) / (slT1 - 1);

				for (int i = 0; i < slT1; i++)
				{
					System.Windows.Shapes.Ellipse el1 = new System.Windows.Shapes.Ellipse();
					el1.Width = 10;
					el1.Height = 10;
					el1.Stroke = System.Windows.Media.Brushes.Red;
					el1.Fill = System.Windows.Media.Brushes.Red;
					el1.Margin = new Thickness(px[0] + 12 + kc_X * i, py[0] + 12, 10, 10);
					FormMain.myCanvas.Children.Add(el1);
				}

				int slB1 = this.MainFramingViewModel.SL_B1 > 1 ? this.MainFramingViewModel.SL_B1 : 2;

				kc_X = (b - 34) / (slB1 - 1);
				for (int i = 0; i < slB1; i++)
				{
					System.Windows.Shapes.Ellipse el1 = new System.Windows.Shapes.Ellipse();
					el1.Width = 10;
					el1.Height = 10;
					el1.Stroke = System.Windows.Media.Brushes.Red;
					el1.Fill = System.Windows.Media.Brushes.Red;
					el1.Margin = new Thickness(px[0] + 12 + kc_X * i, py[2] - 22, 10, 10);
					FormMain.myCanvas.Children.Add(el1);
				}
				#endregion

				#region Vẽ dim


				#endregion

				//DrawC(MainFramingViewModel, FormMain, Movab.id.ToString());
				DrawV(MainFramingViewModel, FormMain, Movab.id.ToString());
			}
		}
		public void DrawC(FramingInfoViewModels MainFramingViewModel, FramingInfoView FormMain, string Id)//, ObservableCollection<SStirrup> SStirrup)
		{
			try
			{
				this.MainFramingViewModel = MainFramingViewModel;
				this.SStirrups = MainFramingViewModel.ListSStirrup;
				this.Stirrups = MainFramingViewModel.ListStirrup;

				System.Windows.Shapes.Polyline pl = new System.Windows.Shapes.Polyline();
				List<UIElement> itemsToremove = new List<UIElement>();
				foreach (UIElement ui in FormMain.myCanvas.Children)
				{
					if (ui.GetType().ToString() == "System.Windows.Shapes.Polyline")
					{
						System.Windows.Shapes.Polyline L = (System.Windows.Shapes.Polyline)ui;
						if (L.Name.IndexOf("DuongBao") >= 0)
						{
							pl = L;
						}
					}
				}

				PointCollection points = pl.Points;

				int slT1 = this.MainFramingViewModel.SL_T1;


				double kc_X = (points[1].X - points[0].X - 34) / (slT1 - 1);

				//string SQL_String = string.Format("SELECT * FROM SStirrup WHERE ElementID = '{0}' AND TYPE = 'DaiC'", Id);
				//System.Data.DataTable SID = LocalDb.Get_DataTable(SQL_String);

				foreach (SStirrup C in this.SStirrups)
				{
					if (C.Type == "DaiC")
					{
						int vitri = int.Parse(C.Vitri1.ToString());

						System.Windows.Shapes.Ellipse el1 = new System.Windows.Shapes.Ellipse();
						el1.Width = 16;
						el1.Height = 16;
						el1.Stroke = System.Windows.Media.Brushes.Magenta;
						el1.Margin = new Thickness(points[0].X + 9 + kc_X * (vitri - 1), points[0].Y + 10, 10, 10);
						el1.Clip = new RectangleGeometry { Rect = new Rect(0, 0, 16, 8) };
						el1.Name = string.Format("DaiC_{0}_1", vitri);
						FormMain.myCanvas.Children.Add(el1);

						System.Windows.Shapes.Ellipse el2 = new System.Windows.Shapes.Ellipse();
						el2.Width = 16;
						el2.Height = 16;
						el2.Stroke = System.Windows.Media.Brushes.Magenta;
						el2.Margin = new Thickness(points[0].X + 9 + kc_X * (vitri - 1), points[2].Y - 26, 10, 10);
						el2.Clip = new RectangleGeometry { Rect = new Rect(0, 8, 16, 8) };
						el2.Name = string.Format("DaiC_{0}_2", vitri);
						FormMain.myCanvas.Children.Add(el2);

						System.Windows.Shapes.Line line_1 = new System.Windows.Shapes.Line();
						line_1.Stroke = System.Windows.Media.Brushes.Magenta;
						line_1.X1 = points[0].X + 9 + kc_X * (vitri - 1);
						line_1.X2 = points[0].X + 9 + kc_X * (vitri - 1);
						line_1.Y1 = points[0].Y + 18;
						line_1.Y2 = points[2].Y - 18;
						line_1.Name = string.Format("DaiC_{0}_3", vitri);
						FormMain.myCanvas.Children.Add(line_1);

						System.Windows.Shapes.Line line_2 = new System.Windows.Shapes.Line();
						line_2.Stroke = System.Windows.Media.Brushes.Magenta;
						line_2.X1 = points[0].X + 25 + kc_X * (vitri - 1);
						line_2.X2 = points[0].X + 25 + kc_X * (vitri - 1);
						line_2.Y1 = points[0].Y + 18;
						line_2.Y2 = points[0].Y + 38;
						line_2.Name = string.Format("DaiC_{0}_3", vitri);
						FormMain.myCanvas.Children.Add(line_2);

						System.Windows.Shapes.Line line_3 = new System.Windows.Shapes.Line();
						line_3.Stroke = System.Windows.Media.Brushes.Magenta;
						line_3.X1 = points[0].X + 25 + kc_X * (vitri - 1);
						line_3.X2 = points[0].X + 25 + kc_X * (vitri - 1);
						line_3.Y1 = points[2].Y - 18;
						line_3.Y2 = points[2].Y - 38;
						line_3.Name = string.Format("DaiC_{0}_4", vitri);
						FormMain.myCanvas.Children.Add(line_3);
					}
				}
			}
			catch (Exception e)
			{
				Console.WriteLine(e);
				throw;
			}

		}
		public void DrawV(FramingInfoViewModels MainFramingViewModel, FramingInfoView FormMain, string Id)
		{
			try
			{
				this.MainFramingViewModel = MainFramingViewModel;
				this.SStirrups = MainFramingViewModel.ListSStirrup;
				this.Stirrups = MainFramingViewModel.ListStirrup;

				System.Windows.Shapes.Polyline pl = new System.Windows.Shapes.Polyline();
				List<UIElement> itemsToremove = new List<UIElement>();
				foreach (UIElement ui in FormMain.myCanvas.Children)
				{
					if (ui.GetType().ToString() == "System.Windows.Shapes.Polyline")
					{
						System.Windows.Shapes.Polyline L = (System.Windows.Shapes.Polyline)ui;
						if (L.Name.IndexOf("DuongBao") >= 0)
						{
							pl = L;
						}
					}
				}

				PointCollection points = pl.Points;

				int slT1 = this.MainFramingViewModel.SL_T1;


				double kc_X = (points[1].X - points[0].X - 34) / (slT1 - 1);

				//string SQL_String = string.Format("SELECT * FROM SStirrup WHERE ElementID = '{0}' AND TYPE = 'Dai'", Id);
				//System.Data.DataTable SID = LocalDb.Get_DataTable(SQL_String);

				foreach (SStirrup C in this.SStirrups)
				{
					if (C.Type == "Dai")
					{
						int vitri1 = int.Parse(C.Vitri1.ToString());
						int vitri2 = int.Parse(C.Vitri2.ToString());

						double b = (vitri2 - vitri1) * kc_X;

						System.Windows.Shapes.Ellipse el1 = new System.Windows.Shapes.Ellipse();
						el1.Width = 10;
						el1.Height = 10;
						el1.Stroke = System.Windows.Media.Brushes.Magenta;
						el1.Margin = new Thickness(points[0].X + 8 + kc_X * (vitri1 - 1), points[0].Y + 10, 10, 10);
						el1.Clip = new RectangleGeometry { Rect = new Rect(0, 0, 5, 5) };
						el1.Name = string.Format("Dai_{0}_{1}_1", vitri1, vitri2);
						FormMain.myCanvas.Children.Add(el1);

						System.Windows.Shapes.Ellipse el2 = new System.Windows.Shapes.Ellipse();
						el2.Width = 10;
						el2.Height = 10;
						el2.Stroke = System.Windows.Media.Brushes.Magenta;
						el2.Margin = new Thickness(points[0].X + 15 + kc_X * (vitri2 - 1), points[0].Y + 10, 10, 10);
						el2.Clip = new RectangleGeometry { Rect = new Rect(5, 0, 5, 5) };
						el2.Name = string.Format("Dai_{0}_{1}_2", vitri1, vitri2);
						FormMain.myCanvas.Children.Add(el2);

						System.Windows.Shapes.Ellipse el3 = new System.Windows.Shapes.Ellipse();
						el3.Width = 10;
						el3.Height = 10;
						el3.Stroke = System.Windows.Media.Brushes.Magenta;
						el3.Margin = new Thickness(points[0].X + 8 + kc_X * (vitri1 - 1), points[2].Y - 20, 10, 10);
						el3.Clip = new RectangleGeometry { Rect = new Rect(0, 5, 5, 5) };
						el3.Name = string.Format("Dai_{0}_{1}_3", vitri1, vitri2);
						FormMain.myCanvas.Children.Add(el3);

						System.Windows.Shapes.Ellipse el4 = new System.Windows.Shapes.Ellipse();
						el4.Width = 10;
						el4.Height = 10;
						el4.Stroke = System.Windows.Media.Brushes.Magenta;
						el4.Margin = new Thickness(points[0].X + 15 + kc_X * (vitri2 - 1), points[2].Y - 20, 10, 10);
						el4.Clip = new RectangleGeometry { Rect = new Rect(5, 5, 5, 5) };
						el4.Name = string.Format("Dai_{0}_{1}_4", vitri1, vitri2);
						FormMain.myCanvas.Children.Add(el4);

						System.Windows.Shapes.Line line_1 = new System.Windows.Shapes.Line();
						line_1.Stroke = System.Windows.Media.Brushes.Magenta;
						line_1.X1 = points[0].X + 8 + kc_X * (vitri1 - 1);
						line_1.X2 = points[0].X + 8 + kc_X * (vitri1 - 1);
						line_1.Y1 = points[0].Y + 15;
						line_1.Y2 = points[2].Y - 15;
						line_1.Name = string.Format("Dai_{0}_{1}_5", vitri1, vitri2);
						FormMain.myCanvas.Children.Add(line_1);

						System.Windows.Shapes.Line line_3 = new System.Windows.Shapes.Line();
						line_3.Stroke = System.Windows.Media.Brushes.Magenta;
						line_3.X1 = points[0].X + 25 + kc_X * (vitri2 - 1);
						line_3.X2 = points[0].X + 25 + kc_X * (vitri2 - 1);
						line_3.Y1 = points[0].Y + 15;
						line_3.Y2 = points[2].Y - 15;
						line_3.Name = string.Format("Dai_{0}_{1}_5", vitri1, vitri2);
						FormMain.myCanvas.Children.Add(line_3);
					}
				}
			}
			catch (Exception e)
			{
				throw;
			}
		}
		public void drawDim_Hor(FramingInfoView FormMain, string textdim, double x1, double y1, double x2, double y2)
		{
			System.Windows.Shapes.Line line_1 = new System.Windows.Shapes.Line();
			line_1.Stroke = System.Windows.Media.Brushes.Black;
			line_1.X1 = x1;
			line_1.X2 = x1;
			line_1.Y1 = y1 + 5;
			line_1.Y2 = y1 + 35;
			line_1.StrokeThickness = 0.5;
			FormMain.myCanvas.Children.Add(line_1);

			System.Windows.Shapes.Line line_3 = new System.Windows.Shapes.Line();
			line_3.Stroke = System.Windows.Media.Brushes.Black;
			line_3.X1 = x2;
			line_3.X2 = x2;
			line_3.Y1 = y1 + 5;
			line_3.Y2 = y1 + 35;
			line_3.StrokeThickness = 0.5;
			FormMain.myCanvas.Children.Add(line_3);

			System.Windows.Shapes.Line line_2 = new System.Windows.Shapes.Line();
			line_2.Stroke = System.Windows.Media.Brushes.Black;
			line_2.X1 = x1 - 5;
			line_2.X2 = x2 + 5;
			line_2.Y1 = y1 + 30;
			line_2.Y2 = y1 + 30;
			line_2.StrokeThickness = 0.5;
			FormMain.myCanvas.Children.Add(line_2);

			System.Windows.Shapes.Line line_4 = new System.Windows.Shapes.Line();
			line_4.Stroke = System.Windows.Media.Brushes.Black;
			line_4.X1 = x1 - 2;
			line_4.X2 = x1 + 2;
			line_4.Y1 = y1 + 28;
			line_4.Y2 = y1 + 32;
			line_4.StrokeThickness = 1.5;
			FormMain.myCanvas.Children.Add(line_4);

			System.Windows.Shapes.Line line_5 = new System.Windows.Shapes.Line();
			line_5.Stroke = System.Windows.Media.Brushes.Black;
			line_5.X1 = x2 - 2;
			line_5.X2 = x2 + 2;
			line_5.Y1 = y1 + 28;
			line_5.Y2 = y1 + 32;
			line_5.StrokeThickness = 1.5;
			FormMain.myCanvas.Children.Add(line_5);

			System.Windows.Controls.TextBlock tb = new System.Windows.Controls.TextBlock();
			tb.Text = textdim;
			tb.HorizontalAlignment = HorizontalAlignment.Center;
			tb.VerticalAlignment = VerticalAlignment.Bottom;
			FormMain.myCanvas.Children.Add(tb);
			tb.Margin = new Thickness((x1 + x2) / 2 - 5, y1 + 15, 0, 0);
		}
		public void drawDim_Ver(FramingInfoView FormMain, string textdim, double x1, double y1, double x2, double y2)
		{
			System.Windows.Shapes.Line line_1 = new System.Windows.Shapes.Line();
			line_1.Stroke = System.Windows.Media.Brushes.Black;
			line_1.X1 = x1 + 5;
			line_1.X2 = x1 + 35;
			line_1.Y1 = y1;
			line_1.Y2 = y1;
			line_1.StrokeThickness = 0.5;
			FormMain.myCanvas.Children.Add(line_1);

			System.Windows.Shapes.Line line_3 = new System.Windows.Shapes.Line();
			line_3.Stroke = System.Windows.Media.Brushes.Black;
			line_3.X1 = x1 + 5;
			line_3.X2 = x1 + 35;
			line_3.Y1 = y2;
			line_3.Y2 = y2;
			line_3.StrokeThickness = 0.5;
			FormMain.myCanvas.Children.Add(line_3);

			System.Windows.Shapes.Line line_2 = new System.Windows.Shapes.Line();
			line_2.Stroke = System.Windows.Media.Brushes.Black;
			line_2.X1 = x1 + 30;
			line_2.X2 = x1 + 30;
			line_2.Y1 = y1 - 5;
			line_2.Y2 = y2 + 5;
			line_2.StrokeThickness = 0.5;
			FormMain.myCanvas.Children.Add(line_2);

			System.Windows.Shapes.Line line_4 = new System.Windows.Shapes.Line();
			line_4.Stroke = System.Windows.Media.Brushes.Black;
			line_4.X1 = x1 + 28;
			line_4.X2 = x1 + 32;
			line_4.Y1 = y1 - 2;
			line_4.Y2 = y1 + 2;
			line_4.StrokeThickness = 1.5;
			FormMain.myCanvas.Children.Add(line_4);

			System.Windows.Shapes.Line line_5 = new System.Windows.Shapes.Line();
			line_5.Stroke = System.Windows.Media.Brushes.Black;
			line_5.X1 = x1 + 28;
			line_5.X2 = x1 + 32;
			line_5.Y1 = y2 - 2;
			line_5.Y2 = y2 + 2;
			line_5.StrokeThickness = 1.5;
			FormMain.myCanvas.Children.Add(line_5);

			System.Windows.Controls.TextBox tb = new System.Windows.Controls.TextBox();
			tb.Text = textdim;
			tb.HorizontalAlignment = HorizontalAlignment.Center;
			tb.VerticalAlignment = VerticalAlignment.Center;
			tb.Background = new SolidColorBrush(Colors.Transparent);
			tb.BorderBrush = new SolidColorBrush(Colors.Transparent);
			FormMain.myCanvas.Children.Add(tb);
			tb.Margin = new Thickness(x1 + +15, (y1 + y2) / 2, 0, 0);
			RotateTransform rotateTransform1 = new RotateTransform(-90, x1 + +15, (y1 + y2) / 2);
			tb.LayoutTransform = rotateTransform1;
		}

		#region Mặt cắt dọc

		public void DrawSection2(FramingInfoViewModels MainFramingViewModel, FramingInfoView FormMain, MovableRestBeam Movab)
		{
			this.MainFramingViewModel = MainFramingViewModel;
			this.SStirrups = MainFramingViewModel.ListSStirrup;
			this.Stirrups = MainFramingViewModel.ListStirrup;
			this.AddT1s = MainFramingViewModel.listAddT1;
			this.AddT2s = MainFramingViewModel.ListAddT2;
			this.AddB2s = MainFramingViewModel.ListAddB2;
			this.AddB1s = MainFramingViewModel.ListAddB1;

			//Xóa hình cũ
			List<UIElement> itemsToremove = new List<UIElement>();
			foreach (UIElement ui in FormMain.AddSection.Children)
			{
				itemsToremove.Add(ui);
			}

			foreach (UIElement ui in itemsToremove)
			{
				FormMain.AddSection.Children.Remove(ui);
			}

			//Vẽ đường bao
			double[] px =
			{
				60,110,110,570,570,620,620,570,570,110,110,60,60
			};

			double[] py =
			{
				60,60,85,85,60,60,210,210,185,185,210,210,60
			};

			PointCollection points = new PointCollection();
			for (int i = 0; i < 13; i++)
			{
				points.Add(new Point(px[i], py[i]));
			}

			//Đường bao
			System.Windows.Shapes.Polyline pl = new System.Windows.Shapes.Polyline();
			pl.Points = points;
			pl.Stroke = System.Windows.Media.Brushes.Black;
			pl.Name = "DuongBao";
			FormMain.AddSection.Children.Add(pl);

			drawDim_Ver2(FormMain, Movab.Hight.ToString(), 620, 85, 620, 185);  //Dim đứng
			drawDim_Hor2(FormMain, Movab.Length.ToString(), 110, 50, 570, 50);  // Dim ngang

			//Vẽ rải đai
			string id = Movab.id.ToString();

			#region Đai

			string kc1 = Save.Default.StirrupSup;
			string kc2 = Save.Default.StirrupSpan;
			string kc3 = Save.Default.StirrupSup;
			string Dia = Save.Default.StirrupDia;

			foreach (Stirrup span in this.Stirrups)
			{
				if (span.Vitri.ToString() == "1" && span.EleID == id)
				{
					kc1 = span.Space.ToString();
					Dia = span.Diameter.ToString().Replace(" ", "");
				}
				else if (span.Vitri.ToString() == "2" && span.EleID == id)
				{
					kc2 = span.Space.ToString();
					Dia = span.Diameter.ToString().Replace(" ", "");
				}
				else if (span.Vitri.ToString() == "3" && span.EleID == id)
				{
					kc2 = span.Space.ToString();
					Dia = span.Diameter.ToString().Replace(" ", "");
				}

			}

			if (kc1 == kc2 && kc1 == kc3)
			{
				drawDim_Hor2(FormMain, Dia + "-" + kc1, 110, 67, 570, 70);
			}
			else if (kc1 == kc2)
			{
				drawDim_Hor2(FormMain, Dia + "-" + kc1, 110, 67, 345, 70);
				drawDim_Hor2(FormMain, Dia + "-" + kc1, 345, 67, 570, 70);
			}
			else if (kc2 == kc3)
			{
				drawDim_Hor2(FormMain, Dia + "-" + kc1, 110, 67, 225, 70);
				drawDim_Hor2(FormMain, Dia + "-" + kc2, 225, 67, 570, 70);
			}
			else
			{
				drawDim_Hor2(FormMain, Dia + "-" + kc1, 110, 67, 225, 70);
				drawDim_Hor2(FormMain, Dia + "-" + kc2, 225, 67, 455, 70);
				drawDim_Hor2(FormMain, Dia + "-" + kc3, 455, 67, 570, 70);
			}
			#endregion

			#region AddT1
			foreach (DbAddT1 span in this.AddT1s)
			{
				if (span.Vitri.ToString() == "1" && int.Parse(span.Count.ToString()) > 0 && span.EleID == id)
				{
					System.Windows.Shapes.Line line_1 = new System.Windows.Shapes.Line();
					line_1.Stroke = System.Windows.Media.Brushes.Red;
					line_1.X1 = 110;
					line_1.X2 = 225;
					line_1.Y1 = 90;
					line_1.Y2 = 90;
					line_1.StrokeThickness = 2;
					FormMain.AddSection.Children.Add(line_1);

					drawTag(FormMain, span.Count.ToString() + "-" + span.Diameter.ToString(), 150, 90, "T1");
				}
				else if (span.Vitri.ToString() == "2" && int.Parse(span.Count.ToString()) > 0 && span.EleID == id)
				{
					System.Windows.Shapes.Line line_2 = new System.Windows.Shapes.Line();
					line_2.Stroke = System.Windows.Media.Brushes.Red;
					line_2.X1 = 225;
					line_2.X2 = 455;
					line_2.Y1 = 90;
					line_2.Y2 = 90;
					line_2.StrokeThickness = 2;
					FormMain.AddSection.Children.Add(line_2);
				}
				else if (span.Vitri.ToString() == "3" && int.Parse(span.Count.ToString()) > 0 && span.EleID == id)
				{
					System.Windows.Shapes.Line line_3 = new System.Windows.Shapes.Line();
					line_3.Stroke = System.Windows.Media.Brushes.Red;
					line_3.X1 = 455;
					line_3.X2 = 570;
					line_3.Y1 = 90;
					line_3.Y2 = 90;
					line_3.StrokeThickness = 2;
					FormMain.AddSection.Children.Add(line_3);
					drawTag(FormMain, span.Count.ToString() + "-" + span.Diameter.ToString(), 500, 90, "T1");
				}
			}
			#endregion

			#region AddT2
			//SQL = string.Format("SELECT * FROM AddT2 WHERE ElementID = '{0}' ORDER BY Vitri", id);
			//System.Data.DataTable SID2 = LocalDb.Get_DataTable(SQL);

			foreach (DbAddT2 span in this.AddT2s)
			{
				if (span.Vitri.ToString() == "1" && int.Parse(span.Count.ToString()) > 0 && span.EleID == id)
				{
					System.Windows.Shapes.Line line_1 = new System.Windows.Shapes.Line();
					line_1.Stroke = System.Windows.Media.Brushes.Red;
					line_1.X1 = 110;
					line_1.X2 = 225;
					line_1.Y1 = 97;
					line_1.Y2 = 97;
					line_1.StrokeThickness = 2;
					FormMain.AddSection.Children.Add(line_1);

					drawTag(FormMain, span.Count.ToString() + "-" + span.Diameter.ToString(), 150, 97, "T2");
				}
				else if (span.Vitri.ToString() == "2" && int.Parse(span.Count.ToString()) > 0 && span.EleID == id)
				{
					System.Windows.Shapes.Line line_2 = new System.Windows.Shapes.Line();
					line_2.Stroke = System.Windows.Media.Brushes.Red;
					line_2.X1 = 225;
					line_2.X2 = 455;
					line_2.Y1 = 97;
					line_2.Y2 = 97;
					line_2.StrokeThickness = 2;
					FormMain.AddSection.Children.Add(line_2);
				}
				else if (span.Vitri.ToString() == "3" && int.Parse(span.Count.ToString()) > 0 && span.EleID == id)
				{
					System.Windows.Shapes.Line line_3 = new System.Windows.Shapes.Line();
					line_3.Stroke = System.Windows.Media.Brushes.Red;
					line_3.X1 = 455;
					line_3.X2 = 570;
					line_3.Y1 = 97;
					line_3.Y2 = 97;
					line_3.StrokeThickness = 2;
					FormMain.AddSection.Children.Add(line_3);
					drawTag(FormMain, span.Count.ToString() + "-" + span.Diameter.ToString(), 500, 97, "T2");
				}
			}
			#endregion

			#region AddB2
			//SQL = string.Format("SELECT * FROM AddB2 WHERE ElementID = '{0}' ORDER BY Vitri", id);
			//System.Data.DataTable SID3 = LocalDb.Get_DataTable(SQL);

			foreach (DbAddB2 span in this.AddB2s)
			{
				if (span.Vitri.ToString() == "1" && int.Parse(span.Count.ToString()) > 0 && span.EleID == id)
				{
					System.Windows.Shapes.Line line_1 = new System.Windows.Shapes.Line();
					line_1.Stroke = System.Windows.Media.Brushes.Red;
					line_1.X1 = 110;
					line_1.X2 = 185;
					line_1.Y1 = 173;
					line_1.Y2 = 173;
					line_1.StrokeThickness = 2;
					FormMain.AddSection.Children.Add(line_1);
				}
				else if (span.Vitri.ToString() == "2" && int.Parse(span.Count.ToString()) > 0 && span.EleID == id)
				{
					System.Windows.Shapes.Line line_2 = new System.Windows.Shapes.Line();
					line_2.Stroke = System.Windows.Media.Brushes.Red;
					line_2.X1 = 185;
					line_2.X2 = 495;
					line_2.Y1 = 173;
					line_2.Y2 = 173;
					line_2.StrokeThickness = 2;
					FormMain.AddSection.Children.Add(line_2);

					drawTag(FormMain, span.Count.ToString() + "-" + span.Diameter.ToString(), 340, 173, "B2");
				}
				else if (span.Vitri.ToString() == "3" && int.Parse(span.Count.ToString()) > 0 && span.EleID == id)
				{
					System.Windows.Shapes.Line line_3 = new System.Windows.Shapes.Line();
					line_3.Stroke = System.Windows.Media.Brushes.Red;
					line_3.X1 = 495;
					line_3.X2 = 570;
					line_3.Y1 = 173;
					line_3.Y2 = 173;
					line_3.StrokeThickness = 2;
					FormMain.AddSection.Children.Add(line_3);
				}
			}
			#endregion

			#region AddB1
			//SQL = string.Format("SELECT * FROM AddB1 WHERE ElementID = '{0}' ORDER BY Vitri", id);
			//System.Data.DataTable SID4 = LocalDb.Get_DataTable(SQL);

			foreach (DbAddB1 span in this.AddB1s)
			{
				if (span.Vitri.ToString() == "1" && int.Parse(span.Count.ToString()) > 0)
				{
					System.Windows.Shapes.Line line_1 = new System.Windows.Shapes.Line();
					line_1.Stroke = System.Windows.Media.Brushes.Red;
					line_1.X1 = 110;
					line_1.X2 = 185;
					line_1.Y1 = 180;
					line_1.Y2 = 180;
					line_1.StrokeThickness = 2;
					FormMain.AddSection.Children.Add(line_1);
				}
				else if (span.Vitri.ToString() == "2" && int.Parse(span.Count.ToString()) > 0)
				{
					System.Windows.Shapes.Line line_2 = new System.Windows.Shapes.Line();
					line_2.Stroke = System.Windows.Media.Brushes.Red;
					line_2.X1 = 185;
					line_2.X2 = 495;
					line_2.Y1 = 180;
					line_2.Y2 = 180;
					line_2.StrokeThickness = 2;
					FormMain.AddSection.Children.Add(line_2);

					drawTag(FormMain, span.Count.ToString() + "-" + span.Diameter.ToString(), 340, 180, "B1");
				}
				else if (span.Vitri.ToString() == "3" && int.Parse(span.Count.ToString()) > 0)
				{
					System.Windows.Shapes.Line line_3 = new System.Windows.Shapes.Line();
					line_3.Stroke = System.Windows.Media.Brushes.Red;
					line_3.X1 = 495;
					line_3.X2 = 570;
					line_3.Y1 = 180;
					line_3.Y2 = 180;
					line_3.StrokeThickness = 2;
					FormMain.AddSection.Children.Add(line_3);
				}
			}
			#endregion
		}

		public void drawDim_Hor2(FramingInfoView FormMain, string textdim, double x1, double y1, double x2, double y2)
		{
			System.Windows.Shapes.Line line_1 = new System.Windows.Shapes.Line();
			line_1.Stroke = System.Windows.Media.Brushes.Black;
			line_1.X1 = x1;
			line_1.X2 = x1;
			line_1.Y1 = y1 - 5;
			line_1.Y2 = y1 - 35;
			line_1.StrokeThickness = 0.5;
			FormMain.AddSection.Children.Add(line_1);

			System.Windows.Shapes.Line line_2 = new System.Windows.Shapes.Line();
			line_2.Stroke = System.Windows.Media.Brushes.Black;
			line_2.X1 = x2;
			line_2.X2 = x2;
			line_2.Y1 = y1 - 5;
			line_2.Y2 = y1 - 35;
			line_2.StrokeThickness = 0.5;
			FormMain.AddSection.Children.Add(line_2);

			System.Windows.Shapes.Line line_3 = new System.Windows.Shapes.Line();
			line_3.Stroke = System.Windows.Media.Brushes.Black;
			line_3.X1 = x1 - 5;
			line_3.X2 = x2 + 5;
			line_3.Y1 = y1 - 30;
			line_3.Y2 = y1 - 30;
			line_3.StrokeThickness = 0.5;
			FormMain.AddSection.Children.Add(line_3);

			System.Windows.Shapes.Line line_4 = new System.Windows.Shapes.Line();
			line_4.Stroke = System.Windows.Media.Brushes.Black;
			line_4.X1 = x1 - 2;
			line_4.X2 = x1 + 2;
			line_4.Y1 = y1 - 28;
			line_4.Y2 = y1 - 32;
			line_4.StrokeThickness = 1.5;
			FormMain.AddSection.Children.Add(line_4);

			System.Windows.Shapes.Line line_5 = new System.Windows.Shapes.Line();
			line_5.Stroke = System.Windows.Media.Brushes.Black;
			line_5.X1 = x2 - 2;
			line_5.X2 = x2 + 2;
			line_5.Y1 = y1 - 28;
			line_5.Y2 = y1 - 32;
			line_5.StrokeThickness = 1.5;
			FormMain.AddSection.Children.Add(line_5);

			System.Windows.Controls.TextBlock tb = new System.Windows.Controls.TextBlock();
			tb.Text = textdim;
			tb.HorizontalAlignment = HorizontalAlignment.Center;
			tb.VerticalAlignment = VerticalAlignment.Bottom;
			FormMain.AddSection.Children.Add(tb);
			tb.Margin = new Thickness((x1 + x2) / 2 - 20, y1 - 45, 0, 0);
		}
		public void drawDim_Ver2(FramingInfoView FormMain, string textdim, double x1, double y1, double x2, double y2)
		{
			System.Windows.Shapes.Line line_1 = new System.Windows.Shapes.Line();
			line_1.Stroke = System.Windows.Media.Brushes.Black;
			line_1.X1 = x1 + 5;
			line_1.X2 = x1 + 35;
			line_1.Y1 = y1;
			line_1.Y2 = y1;
			line_1.StrokeThickness = 0.5;
			FormMain.AddSection.Children.Add(line_1);

			System.Windows.Shapes.Line line_2 = new System.Windows.Shapes.Line();
			line_2.Stroke = System.Windows.Media.Brushes.Black;
			line_2.X1 = x1 + 5;
			line_2.X2 = x1 + 35;
			line_2.Y1 = y2;
			line_2.Y2 = y2;
			line_2.StrokeThickness = 0.5;
			FormMain.AddSection.Children.Add(line_2);

			System.Windows.Shapes.Line line_3 = new System.Windows.Shapes.Line();
			line_3.Stroke = System.Windows.Media.Brushes.Black;
			line_3.X1 = x1 + 30;
			line_3.X2 = x1 + 30;
			line_3.Y1 = y1 - 5;
			line_3.Y2 = y2 + 5;
			line_3.StrokeThickness = 0.5;
			FormMain.AddSection.Children.Add(line_3);

			System.Windows.Shapes.Line line_4 = new System.Windows.Shapes.Line();
			line_4.Stroke = System.Windows.Media.Brushes.Black;
			line_4.X1 = x1 + 28;
			line_4.X2 = x1 + 32;
			line_4.Y1 = y1 - 2;
			line_4.Y2 = y1 + 2;
			line_4.StrokeThickness = 1.5;
			FormMain.AddSection.Children.Add(line_4);

			System.Windows.Shapes.Line line_5 = new System.Windows.Shapes.Line();
			line_5.Stroke = System.Windows.Media.Brushes.Black;
			line_5.X1 = x1 + 28;
			line_5.X2 = x1 + 32;
			line_5.Y1 = y2 - 2;
			line_5.Y2 = y2 + 2;
			line_5.StrokeThickness = 1.5;
			FormMain.AddSection.Children.Add(line_5);

			System.Windows.Controls.TextBox tb = new System.Windows.Controls.TextBox();
			tb.Text = textdim;
			tb.HorizontalAlignment = HorizontalAlignment.Center;
			tb.VerticalAlignment = VerticalAlignment.Center;
			tb.Background = new SolidColorBrush(Colors.Transparent);
			tb.BorderBrush = new SolidColorBrush(Colors.Transparent);
			FormMain.AddSection.Children.Add(tb);
			tb.Margin = new Thickness(x1 + +15, (y1 + y2) / 2, 0, 0);
			RotateTransform rotateTransform1 = new RotateTransform(-90, x1 + +15, (y1 + y2) / 2);
			tb.LayoutTransform = rotateTransform1;
		}

		public void drawTag(FramingInfoView FormMain, string textdim, double x1, double y1, string vitri)
		{
			double b = -40;
			if (vitri == "T1")
			{
				b = -40;
			}
			else if (vitri == "T2")
			{
				b = -30;
			}
			else if (vitri == "B2")
			{
				b = 30;
			}
			else if (vitri == "B1")
			{
				b = 40;
			}


			System.Windows.Shapes.Line line_1 = new System.Windows.Shapes.Line();
			line_1.Stroke = System.Windows.Media.Brushes.Black;
			line_1.X1 = x1;
			line_1.X2 = x1;
			line_1.Y1 = y1;
			line_1.Y2 = y1 + b;
			line_1.StrokeThickness = 0.5;
			FormMain.AddSection.Children.Add(line_1);

			System.Windows.Shapes.Line line_2 = new System.Windows.Shapes.Line();
			line_2.Stroke = System.Windows.Media.Brushes.Black;
			line_2.X1 = x1;
			line_2.X2 = x1 + 15;
			line_2.Y1 = y1 + b;
			line_2.Y2 = y1 + b;
			line_2.StrokeThickness = 0.5;
			FormMain.AddSection.Children.Add(line_2);

			System.Windows.Shapes.Line line_3 = new System.Windows.Shapes.Line();
			line_3.Stroke = System.Windows.Media.Brushes.Black;
			line_3.X1 = x1 - 2;
			line_3.X2 = x1 + 2;
			line_3.Y1 = y1 - 2;
			line_3.Y2 = y1 + 2;
			line_3.StrokeThickness = 0.5;
			FormMain.AddSection.Children.Add(line_3);

			System.Windows.Controls.TextBlock tb = new System.Windows.Controls.TextBlock();
			tb.Text = textdim;
			tb.HorizontalAlignment = HorizontalAlignment.Left;
			tb.VerticalAlignment = VerticalAlignment.Bottom;
			FormMain.AddSection.Children.Add(tb);
			tb.Margin = new Thickness(x1 + 22, y1 + b - 10, 0, 0);
		}
		#endregion
	}
}
