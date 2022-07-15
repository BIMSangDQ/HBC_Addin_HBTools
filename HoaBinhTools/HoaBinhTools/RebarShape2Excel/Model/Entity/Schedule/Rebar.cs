using System.Collections.Generic;
using Utility;

namespace Model.Entity
{
	public partial class Rebar
	{
		public Rebar(Autodesk.Revit.DB.Element element)
		{
			RevitRebar = element;
			if (element is Autodesk.Revit.DB.Structure.Rebar)
			{
				SingleRebar = element as Autodesk.Revit.DB.Structure.Rebar;
			}
			else if (element is Autodesk.Revit.DB.Structure.RebarInSystem)
			{
				SystemRebar = element as Autodesk.Revit.DB.Structure.RebarInSystem;
			}
			else
			{
				throw new Model.Exception.CaseNotCheckException();
			}
		}
		public Rebar(Autodesk.Revit.DB.Reference rf) : this(rf.ToRevitElement())
		{
			//var curves = new List<Autodesk.Revit.DB.Curve>();

			//var geoElem = SingleRebar.GetGeometryObjectFromReference(rf) as Autodesk.Revit.DB.GeometryElement;
			//foreach (var geoObj in geoElem)
			//{
			//    curves.Add(geoObj as Autodesk.Revit.DB.Curve);
			//}

			//CenterCurves = curves;
		}
		public Autodesk.Revit.DB.Structure.Rebar SingleRebar { get; set; }
		public Autodesk.Revit.DB.Structure.RebarInSystem SystemRebar { get; set; }
		public Autodesk.Revit.DB.Element RevitRebar { get; set; }

		private Autodesk.Revit.DB.Structure.RebarShapeDrivenAccessor rebarShapeDrivenAccessor;
		public Autodesk.Revit.DB.Structure.RebarShapeDrivenAccessor RebarShapeDrivenAccessor
		{
			get
			{
				if (rebarShapeDrivenAccessor == null)
					rebarShapeDrivenAccessor = this.GetRebarShapeDrivenAccessor();
				return rebarShapeDrivenAccessor;
			}
		}

		private Autodesk.Revit.DB.Structure.RebarShape rebarShape;
		public Autodesk.Revit.DB.Structure.RebarShape RebarShape
		{
			get
			{
				if (rebarShape == null) rebarShape = this.GetRebarShape();
				return rebarShape;
			}
		}

		private Autodesk.Revit.DB.Structure.RebarBarType rebarBarType;
		public Autodesk.Revit.DB.Structure.RebarBarType RebarBarType
		{
			get
			{
				if (rebarBarType == null) rebarBarType = this.GetRebarBarType();
				return rebarBarType;
			}
		}

		private Autodesk.Revit.DB.Structure.RebarStyle? rebarStyle;
		public Autodesk.Revit.DB.Structure.RebarStyle? RebarStyle
		{
			get
			{
				if (rebarStyle == null)
				{
					rebarStyle = RebarShape.RebarStyle;
				}
				return rebarStyle;
			}
		}

		private Autodesk.Revit.DB.Structure.RebarBendData rebarBendData;
		public Autodesk.Revit.DB.Structure.RebarBendData RebarBendData
		{
			get
			{
				if (rebarBendData == null) rebarBendData = this.GetRebarBendData();
				return rebarBendData;
			}
		}

		private Autodesk.Revit.DB.Line distributionPath;
		public Autodesk.Revit.DB.Line DistributionPath
		{
			get
			{
				if (distributionPath == null)
					distributionPath = this.GetDistributionPath();
				return distributionPath;
			}
		}

		private Autodesk.Revit.DB.XYZ distributionDirection;
		public Autodesk.Revit.DB.XYZ DistributionDirection
		{
			get
			{
				if (distributionDirection == null)
					distributionDirection = this.GetDistributionDirection();
				return distributionDirection;
			}
		}

		private List<Autodesk.Revit.DB.Curve> centerCurves;
		public List<Autodesk.Revit.DB.Curve> CenterCurves
		{
			get
			{
				if (centerCurves == null) centerCurves = this.GetCenterCurves();
				return centerCurves;
			}
			set
			{
				centerCurves = value;
			}
		}

		private List<Autodesk.Revit.DB.Curve> supressBendCurves;
		public List<Autodesk.Revit.DB.Curve> SupressBendCurves
		{
			get
			{
				if (supressBendCurves == null) supressBendCurves = this.GetSupressBendCurves();
				return supressBendCurves;
			}
			set
			{
				supressBendCurves = value;
			}
		}

		private List<Autodesk.Revit.DB.Curve> detailCurves;
		public List<Autodesk.Revit.DB.Curve> DetailCurves
		{
			get
			{
				if (detailCurves == null) detailCurves = this.GetDetailCurves();
				return detailCurves;
			}
		}

		private List<string> dimensionNames;
		public List<string> DimensionNames
		{
			get
			{
				if (dimensionNames == null) dimensionNames = this.GetDimensionNames();
				return dimensionNames;
			}
		}
		private List<double> dimensionValues { get; set; }
		public List<double> DimensionValues
		{
			get
			{
				if (dimensionValues == null) dimensionValues = this.GetDimensionValues();
				return dimensionValues;
			}
		}

		private int quantity;
		public int Quantity
		{
			get
			{
				if (quantity == 0) quantity = this.GetQuantity();
				return quantity;
			}
		}

		private double arrayLength;
		public double ArrayLength
		{
			get
			{
				if (arrayLength == 0) arrayLength = DistributionPath.Length;
				return arrayLength;
			}
		}

		private double spacing;
		public double Spacing
		{
			get
			{
				if (spacing == 0 && Quantity != 1)
				{
					spacing = ArrayLength / (Quantity - 1);
				}
				return spacing;
			}
		}

		public List<int> HookTextIndexs { get; set; } = new List<int>();
	}
}