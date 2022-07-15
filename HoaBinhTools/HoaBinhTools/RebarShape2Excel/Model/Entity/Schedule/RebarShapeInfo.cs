using System;
using System.Collections.Generic;
using System.Drawing;
using Autodesk.Revit.DB;
using SingleData;
using Utility;


namespace Model.Entity
{
	public class RebarShapeInfo
	{
		public string GUID { get; set; }
		public int ID { get; set; }
		public int Width { get; set; } = 0;
		public int Height { get; set; } = 0;
		public Bitmap Bitmap { get; set; }
		public List<Curve> curves { get; set; }

		private int[] bindingIndexs;
		public int[] BindingIndexs
		{
			get
			{
				if (bindingIndexs == null) bindingIndexs = new int[supressBendCurves.Count];
				return bindingIndexs;
			}
		}

		private bool[] notSupressChecks;
		public bool[] NotSupressCheck
		{
			get
			{
				if (notSupressChecks == null) notSupressChecks = new bool[supressBendCurves.Count];
				return notSupressChecks;
			}
		}

		public List<Curve> supressBendCurves { get; set; }
		public List<Curve> modifyCurves { get; set; }
		public List<string> paraNames { get; set; }
		public string key { get; set; }
		public List<int> Values { get; set; }
		public double MaxLength { get; set; }
		public string RebarShape { get; set; }

		public RebarShapeInfo()
		{

		}

		public RebarShapeInfo(int id, List<Curve> curves, List<Curve> supressBendCurves, List<string> paraNames, string key, string rebarShape)
		{
			var revitModelData = RevitModelData.Instance;

			this.ID = id;
			this.curves = curves;
			this.paraNames = paraNames;
			this.supressBendCurves = supressBendCurves;
			this.key = key;
			this.RebarShape = rebarShape;

			bindingIndexs = new int[supressBendCurves.Count];
			notSupressChecks = new bool[supressBendCurves.Count];

			for (int i = 0; i < supressBendCurves.Count; i++)
			{
				if (revitModelData.ExcludeAntiTwistingSquareStirrupShapeNames
					.Contains(RebarShape))
				{
					if (i == 0)
					{
						bindingIndexs[i] = 0; continue;
					}
					if (i == supressBendCurves.Count - 1)
					{
						bindingIndexs[i] = curves.Count - 1; continue;
					}
				}

				if (supressBendCurves[i] is Line)
				{
					Line line = supressBendCurves[i] as Line;
					for (int j = 0; j < curves.Count; j++)
					{
						if (curves[j] is Arc) continue;
						if (GeometryUtil.IsPointInLine2D(curves[j].GetEndPoint(0), line))
						{
							bindingIndexs[i] = j;
							notSupressChecks[i] = true;
							break;
						}
					}
				}
				if (supressBendCurves[i] is Arc)
				{
					Arc arc1 = supressBendCurves[i] as Arc;
					for (int j = 0; j < curves.Count; j++)
					{
						if (curves[j] is Line) continue;
						Arc arc2 = curves[j] as Arc;
						if (arc1.Center.IsEqual(arc2.Center) && arc1.Radius.IsEqual(arc2.Radius))
						{
							bindingIndexs[i] = j;
							notSupressChecks[i] = true;
							break;
						}
					}
				}
			}

			// Check supressLine
			for (int i = 0; i < bindingIndexs.Length; i++)
			{
				if (bindingIndexs[i] == 0 && i != 0)
				{
					bindingIndexs[i] = bindingIndexs[i + 1] - 1;
				}
			}
		}

		internal void CreateTextOnBitmap(Arc arc, int v, int j)
		{
			throw new NotImplementedException();
		}
	}
}
