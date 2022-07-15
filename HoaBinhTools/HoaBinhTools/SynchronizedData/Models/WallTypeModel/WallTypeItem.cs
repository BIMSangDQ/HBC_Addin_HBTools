using System.Collections.Generic;

namespace HoaBinhTools.SynchronizedData.Models
{
	public class WallTypeItem
	{
		public string WallTypeName { get; set; }
		public string Wallfunction { get; set; }
		public string FamilyName { get; set; }
		public int Exterior { get; set; }
		public int Interior { get; set; }
		public int StructureIndex { get; set; }
		public double MinLayerV { get; set; }
		public double MaxLayerV { get; set; }
		public string ScaleFillColor { get; set; }
		public string ScaleFillPattern { get; set; }

		public List<CompoundStructureWallType> CompoundStructureWallTypeLayers { get; set; }

		public WallTypeItem
			(
			string wallTypeName, string wallfunction, string familyName, List<CompoundStructureWallType> compoundStructureWallTypeLayers,
			int exterior, int interior, int structureIndex, double minLayerV, double maxLayerV,
			string scaleFillColor, string scaleFillPattern)
		{
			WallTypeName = wallTypeName;
			Wallfunction = wallfunction;
			FamilyName = familyName;
			Exterior = exterior;
			Interior = interior;
			StructureIndex = structureIndex;
			MinLayerV = minLayerV;
			MaxLayerV = maxLayerV;
			CompoundStructureWallTypeLayers = compoundStructureWallTypeLayers;
			ScaleFillColor = scaleFillColor;
			ScaleFillPattern = scaleFillPattern;
		}
	}

	public class CompoundStructureWallType
	{
		public string LayerFunction { get; set; }
		public int Layerid { get; set; }
		public double Layerwidth { get; set; }
		public string MaterialName { get; set; }
		public double MinU { get; set; }
		public double MinV { get; set; }
		public double MaxU { get; set; }
		public double MaxV { get; set; }
		public bool TopExtension { get; set; }
		public bool BotExtension { get; set; }

		public CompoundStructureWallType(string layerFunction, int layerid, double layerwidth,
			string materialName, double minU, double minV, double maxU, double maxV, bool Top, bool Bot )
		{
			LayerFunction = layerFunction;
			Layerid = layerid;
			Layerwidth = layerwidth;
			MaterialName = materialName;
			MinU = minU;
			MinV = minV;
			MaxU = maxU;
			MaxV = maxV;
			TopExtension = Top;
			BotExtension = Bot;
		}
	}
}
