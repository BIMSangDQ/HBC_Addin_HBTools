using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HoaBinhTools.SynchronizedData.Models.WallTypeModel
{
	public class CurtainWallItem
	{
		public string WallTypeName { get; set; }
		public string Wallfunction { get; set; }
		public int Wall_AutomaticallyEmbed { get; set; }
		public string CurtainPanel {get; set; }
		public int JoinCondition { get; set; }
		public string StructuralMaterial { get; set; }
		public int LayoutVert { get; set; }
		public double SpacingVert { get; set; }
		public int AdjustBorderVert { get; set; }
		public int LayoutHoriz { get; set; }
		public double SpacingHoriz { get; set; }
		public int AdjustBorderHoriz { get; set; }
		public string InteriorTypeVert { get; set; }
		public string Border1Vert { get; set; }
		public string Border2Vert { get; set; }
		public string InteriorTypeHoriz { get; set; }
		public string Border1Horiz { get; set; }
		public string Border2Horiz { get; set; }
	}
}
