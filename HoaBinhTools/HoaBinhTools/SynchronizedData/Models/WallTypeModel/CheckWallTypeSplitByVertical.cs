using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autodesk.Revit.DB;
using Utils;

namespace HoaBinhTools.SynchronizedData.Models.WallTypeModel
{
	public class CheckWallTypeSplitByVertical
	{
		public static bool IsWallTypeSplitByVertical(WallType wallType)
		{
			CompoundStructure compoundStructure = wallType.GetCompoundStructure();
			IList<CompoundStructureLayer> layers = compoundStructure.GetLayers();
			List<CompoundStructureWallType> listLayers = new List<CompoundStructureWallType>();

			IList<int> Extendable_Top = compoundStructure.GetExtendableRegionIds(true);
			IList<int> Extendable_Bot = compoundStructure.GetExtendableRegionIds(false);
			for (int i = 0; i < layers.Count; i++)
			{
				CompoundStructureLayer Layer = layers[i];
				string LayerFunction = Layer.Function.ToString();
				int layerid = Layer.LayerId;
				double layerwidth = Layer.Width;
				string MaterialId = Layer.MaterialId.ToString();
				ElementId id = Layer.MaterialId;
				Material m = ActiveData.Document.GetElement(id) as Material;
				bool TopExtension = false;
				bool BotExtension = false;

				#region
				IList<int> ints = compoundStructure.GetRegionsAssociatedToLayer(i);
				foreach (int i2 in ints)
				{
					if (TopExtension == false) TopExtension = Extendable_Top.IndexOf(i2) > -1 ? true : false;
					if (BotExtension == false) BotExtension = Extendable_Bot.IndexOf(i2) > -1 ? true : false;
				}
				double minU = 100;
				double minV = 100;
				double maxU = -100;
				double maxV = -100;

				foreach (int RegionId in ints)
				{
					IList<int> SegIds = compoundStructure.GetSegmentIds();
					foreach (int segId in SegIds)
					{
						IList<int> intsRegions = compoundStructure.GetAdjacentRegions(segId);
						if (intsRegions.Contains(RegionId))
						{
							UV endPoint1 = UV.Zero;
							UV endPoint2 = UV.Zero;
							compoundStructure.GetSegmentEndPoints(segId, RegionId, out endPoint1, out endPoint2);

							if (minU >= endPoint1.U) minU = endPoint1.U;
							if (minV >= endPoint1.V) minV = endPoint1.V;
							if (maxU <= endPoint1.U) maxU = endPoint1.U;
							if (maxV <= endPoint1.V) maxV = endPoint1.V;

							if (minU >= endPoint2.U) minU = endPoint2.U;
							if (minV >= endPoint2.V) minV = endPoint2.V;
							if (maxU <= endPoint2.U) maxU = endPoint2.U;
							if (maxV <= endPoint2.V) maxV = endPoint2.V;
						}
					}
				}

				if (Math.Round(Math.Abs(maxU - minU), 5) != Math.Round(layerwidth, 5)) layerwidth = 0;
				string materialName = "<By Category>";
				try
				{
					materialName = m.Name;
				}
				catch
				{ }

				#endregion
				CompoundStructureWallType la = new CompoundStructureWallType(LayerFunction, layerid, layerwidth, materialName, minU, minV, maxU, maxV, TopExtension, BotExtension);
				listLayers.Add(la);
			}

			bool isVert = false;
			for (int i = 0; i < layers.Count-1; i++)
			{
				for (int j = i+1; j < layers.Count; j++)
				{
					if (listLayers[i].MinU == listLayers[j].MinU && listLayers[i].MaxU == listLayers[j].MaxU)
					{
						isVert = true;
						return isVert;
					}
				}
			}

			return isVert;
		}
	}
}
