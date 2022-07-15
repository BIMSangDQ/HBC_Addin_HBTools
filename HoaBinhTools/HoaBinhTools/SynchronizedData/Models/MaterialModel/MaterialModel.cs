using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autodesk.Revit.DB;

namespace HoaBinhTools.SynchronizedData.Models.MaterialModel
{
	public class MaterialModel
	{
		public string Name { get; set; }
		public string Description { get; set; }
		public int KeyNote { get; set; }
		public string Mark { get; set; }
		public bool UserRenderAppearance { get; set; }
		public int Color { get; set; }
		public int Tranferancy { get; set; }
		public string CutBackgroundPatternColor { get; set; }
		public string CutBackgroundPatternName { get; set; }
		public string CutForegroundPatternColor { get; set; }
		public string CutForegroundPatternName { get; set; }
		public string SurfaceBackgroundPatternColor { get; set; }
		public string SurfaceBackgroundPatternName { get; set; }
		public string SurfaceForegroundPatternColor { get; set; }
		public string SurfaceForegroundPatternName { get; set; }
		public string MaterialCategory { get; set; }
		public string MaterialClass { get; set; }
		public int Shininess { get; set; }
		public int Smoothness { get; set; }
		public string AppearanceName { get; set; }
		public string StructuralAssetName { get; set; }
		public string ThemalAssetName { get; set; }
	}
}
