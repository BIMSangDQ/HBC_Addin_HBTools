using System.Collections.Generic;

namespace HoaBinhTools.SynchronizedData.Models
{
	public class FloorTypeItem
	{
		public string FloorTypeName { get; set; }
		public string FamilyName { get; set; }
		public int Exterior { get; set; }
		public int Interior { get; set; }
		public int StructureIndex { get; set; }
		public string ScaleFillColor { get; set; }
		public string ScaleFillPattern { get; set; }
		public string FloorFunction { get; set; }
		public List<CompoundStructureFloorType> CompoundStructureFloorTypeLayers { get; set; }

		public FloorTypeItem
			(
			string floorTypeName, string familyName, List<CompoundStructureFloorType> compoundStructureFloorTypeLayers,
			int exterior, int interior, int structureIndex,
			string scaleFillColor, string scaleFillPattern,string floorfunction)
		{
			FloorTypeName = floorTypeName;
			FamilyName = familyName;
			Exterior = exterior;
			Interior = interior;
			StructureIndex = structureIndex;
			CompoundStructureFloorTypeLayers = compoundStructureFloorTypeLayers;
			ScaleFillColor = scaleFillColor;
			ScaleFillPattern = scaleFillPattern;
			FloorFunction = floorfunction;
		}
	}

	public class CompoundStructureFloorType
	{
		public string LayerFunction { get; set; }
		public int Layerid { get; set; }
		public double Layerwidth { get; set; }
		public string MaterialName { get; set; }

		public CompoundStructureFloorType(string layerFunction, int layerid, double layerwidth,
			string materialName)
		{
			LayerFunction = layerFunction;
			Layerid = layerid;
			Layerwidth = layerwidth;
			MaterialName = materialName;
		}
	}
}
