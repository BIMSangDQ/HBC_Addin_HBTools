using System;
using System.Drawing;
using Autodesk.Revit.DB;
using Constant;
using Utility;


namespace Model.Entity
{
	public class CurveInfo
	{
		public int Value { get; set; } = 0;
		public bool IsShow { get; set; } = false;
		public Curve Curve { get; set; }
		public CurveInfo(Curve curve, int value = 0)
		{
			Curve = curve;
			Value = value;

			if (curve is Arc) IsShow = false;
			else if (curve is Line) IsShow = true;
			else throw new Model.Exception.CaseNotCheckException();
		}

		public void CreateGeometryOnBitmap(Bitmap bmp)
		{
			double feet2MM = 1.0.feet2Milimeter();
			Pen pen = new Pen(System.Drawing.Color.Black, (float)0.4);
			var graphics = Graphics.FromImage(bmp);

			if (Curve is Line)
			{
				Line l = Curve as Line;

				var pnt1 = l.GetEndPoint(0) * feet2MM;
				var pnt2 = l.GetEndPoint(1) * feet2MM;
				PointF p1 = new PointF((float)pnt1.X, (float)pnt1.Y), p2 = new PointF((float)pnt2.X, (float)pnt2.Y);
				graphics.DrawLine(pen, p1, p2);

				var vecX = l.Direction;
				var vecY = Autodesk.Revit.DB.XYZ.BasisZ.CrossProduct(vecX);
				var insertPnt = (pnt1 + pnt2) / 2 + vecY * 5 - vecX * 1;
				PointF p3 = new PointF(0, 0);
				double ang1 = Math.Atan(vecX.Y / vecX.X) * 180 / Math.PI;
				if (vecX.X >= 0) ang1 *= 1;
				else ang1 = ang1 * 1 - 180;
				if (ang1 > 135)
				{
					ang1 -= 180;
					insertPnt -= vecY * 4;
				}
				if (ang1 < -135)
				{
					ang1 += 180;
					insertPnt -= vecY * 4;
				}

				graphics.TranslateTransform((float)insertPnt.X, (float)insertPnt.Y);
				graphics.RotateTransform((float)ang1);
				graphics.DrawString(Value.ToString(), new Font("Calibri", (float)ConstantValue.BitmapTextHeightMM), Brushes.Black, p3);
			}
			else if (Curve is Arc)
			{
				Arc arc = Curve as Arc;
				var center = arc.Center * feet2MM;
				double radius = arc.Radius * feet2MM;
				RectangleF rec = new RectangleF((float)(center.X - radius),
					(float)(center.Y - radius), (float)radius * 2, (float)radius * 2);

				var vec1 = arc.GetEndPoint(0) * feet2MM - center;
				var vec2 = arc.GetEndPoint(1) * feet2MM - center;

				double ang1 = Math.Atan(vec1.Y / vec1.X) * 180 / Math.PI;
				if (vec1.X >= 0) ang1 *= 1;
				else ang1 = ang1 * 1 - 180;

				double ang2 = Math.Atan(vec2.Y / vec2.X) * 180 / Math.PI;
				if (vec2.X >= 0) ang2 *= 1;
				else ang2 = ang2 * 1 - 180;

				double delta = ang2 - ang1;
				if (delta > 180) delta -= 360;
				if (delta < -180) delta += 360;
				graphics.DrawArc(pen, rec, (float)ang1, (float)delta);
			}
			else
			{
				throw new Model.Exception.CaseNotCheckException();
			}
		}
	}
}
