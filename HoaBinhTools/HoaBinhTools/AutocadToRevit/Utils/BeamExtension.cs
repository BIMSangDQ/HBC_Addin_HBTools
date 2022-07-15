using System;
using Autodesk.Revit.DB;

namespace HoaBinhTools.AutocadToRevit.Utils
{
	public class BeamData
	{
		public BeamData(ImportInstance cadInstance, Line line)
		{
			string namelayer;

			try
			{
				GraphicsStyle graphicsStyle =
					cadInstance.Document.GetElement(line.GraphicsStyleId)
						as GraphicsStyle;

				Category styleCategory = graphicsStyle.GraphicsStyleCategory;

				if (styleCategory.Name.Contains("BIM_HB~"))
				{
					namelayer = styleCategory.Name;
					var strP = namelayer.Split('~');
					string tietdien = strP[2];
					TenDam = strP[1];
					var bxh = tietdien.Split('x');
					BeRong = double.Parse(bxh[0]);
					ChieuCao = Double.Parse(bxh[1]);
					Centerline = line;
				}
			}
			catch (Exception e)
			{
				throw;
			}

		}

		public double BeRong { get; set; }
		public double ChieuCao { get; set; }
		public string TenDam { get; set; }
		public Line Centerline { get; set; }
	}
}
