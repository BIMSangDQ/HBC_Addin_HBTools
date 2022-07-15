using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Constant;
using Model.Entity;
using SingleData;

namespace Utility
{
	public static class RebarShapeInfoUtil
	{
		private static RevitModelData revitModelData
		{
			get { return RevitModelData.Instance; }
		}

		public static RebarShapeInfo GetCurve2DsFromRebar(this Autodesk.Revit.DB.Structure.Rebar rebar)
		{
			var entRebar = new Model.Entity.Rebar(rebar);
			var paraNames = entRebar.DimensionNames;

			entRebar.TransformCurvesForExcelSchedule();

			#region Create Key
			StringBuilder sb = new StringBuilder();
			sb.Append(entRebar.RebarBarType.Name);
			sb.Append("__");
			sb.Append(rebar.LookupParameter("Partition").AsString());
			sb.Append("__");
			sb.Append(rebar.LookupParameter("Rebar Number").AsString());
			sb.Append("__");
			string suffix = rebar.LookupParameter("Rebar Number Suffix")?.AsString();
			if (suffix != null && suffix.Length != 0)
			{
				sb.Append(suffix);
			}

			var key = sb.ToString();
			#endregion

			var rsi = new RebarShapeInfo
			{
				ID = rebar.Id.IntegerValue,
				curves = entRebar.CenterCurves.Concat(entRebar.DetailCurves).ToList(),
				supressBendCurves = entRebar.SupressBendCurves,
				paraNames = paraNames,
				key = key,
				RebarShape = entRebar.RebarShape.Name
			};

			return rsi;
		}

		public static void Initial(this RebarShapeInfo rsi)
		{
			var supressBendCurves = rsi.supressBendCurves;
			var rebarShape = rsi.RebarShape;
			var curves = rsi.curves;

			if (!rebarShape.Contains("TD_O_01"))
			{
				var bindingIndexs = rsi.BindingIndexs;
				var notSupressChecks = rsi.NotSupressCheck;
				for (int i = 0; i < supressBendCurves.Count; i++)
				{
					if (revitModelData.ExcludeAntiTwistingSquareStirrupShapeNames
						.Contains(rebarShape))
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

					if (supressBendCurves[i] is Autodesk.Revit.DB.Line)
					{
						Autodesk.Revit.DB.Line line = supressBendCurves[i] as Autodesk.Revit.DB.Line;
						for (int j = 0; j < curves.Count; j++)
						{
							if (curves[j] is Autodesk.Revit.DB.Arc) continue;
							if (GeometryUtil.IsPointInLine2D(curves[j].GetEndPoint(0), line))
							{
								bindingIndexs[i] = j;
								notSupressChecks[i] = true;
								break;
							}
						}
					}
					if (supressBendCurves[i] is Autodesk.Revit.DB.Arc)
					{
						Autodesk.Revit.DB.Arc arc1 = supressBendCurves[i] as Autodesk.Revit.DB.Arc;
						for (int j = 0; j < curves.Count; j++)
						{
							if (curves[j] is Autodesk.Revit.DB.Line) continue;
							Autodesk.Revit.DB.Arc arc2 = curves[j] as Autodesk.Revit.DB.Arc;
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
		}


		public static void Edit(this RebarShapeInfo rsi)
		{
			//rsi.curves.Concat(rsi.DetailCurves).EvaluateCurveOn2D();
			//throw new Model.Exception.CaseNotCheckException();

			rsi.curves = rsi.ScaleCurves(rsi.curves);
			rsi.supressBendCurves = rsi.ScaleCurves(rsi.supressBendCurves);

			rsi.curves = rsi.CombineAndMoveToZero(rsi.curves);
			rsi.TranslateSupressCurvesBaseOnCurves();

			rsi.modifyCurves = new List<Autodesk.Revit.DB.Curve>();
			for (int i = 0; i < rsi.BindingIndexs.Length; i++)
			{
				if (rsi.NotSupressCheck[i])
				{
					rsi.modifyCurves.Add(rsi.curves[rsi.BindingIndexs[i]]);
				}
				else
				{
					rsi.modifyCurves.Add(rsi.supressBendCurves[i]);
				}
			}

			rsi.modifyCurves = rsi.IntersectionAndCombineCurve(rsi.modifyCurves);

			double width = 0, height = 0;
			for (int i = 0; i < rsi.curves.Count; i++)
			{
				Autodesk.Revit.DB.XYZ pnt = rsi.curves[i].GetEndPoint(0);
				if (width < pnt.X) width = pnt.X;
				if (height < pnt.Y) height = pnt.Y;

				if (i == rsi.curves.Count - 1)
				{
					pnt = rsi.curves[i].GetEndPoint(1);
					if (width < pnt.X) width = pnt.X;
					if (height < pnt.Y) height = pnt.Y;
				}
			}

			rsi.Width = (int)(Math.Ceiling(width.feet2Milimeter()) + ConstantValue.BitmapOffsetValueMM);
			rsi.Height = (int)(Math.Ceiling(height.feet2Milimeter()) + ConstantValue.BitmapOffsetValueMM);
		}
		public static List<Autodesk.Revit.DB.Curve> ScaleCurves(this RebarShapeInfo rsi, List<Autodesk.Revit.DB.Curve> curves)
		{
			var rebarShape = rsi.RebarShape;

			if (!rebarShape.Contains("TD_O_01"))
			{
				if (rsi.MaxLength == 0) rsi.MaxLength = curves.Max(x => x.Length);
				for (int i = 0; i < curves.Count; i++)
				{
					if (curves[i] is Autodesk.Revit.DB.Line)
					{
						Autodesk.Revit.DB.Line line = curves[i] as Autodesk.Revit.DB.Line;
						double ratio = 1;
						if (curves[i].Length >= 2 * rsi.MaxLength / 3)
						{
						}
						else if (curves[i].Length >= rsi.MaxLength / 5)
						{
							ratio = (double)2 / 3;
						}
						else
						{
							ratio = (double)1 / 3;
						}
						if (revitModelData.ExcludeSquareStirrupShapeNames.Contains(rsi.RebarShape)
							&& (i == 2 || i == 8))
						{
							ratio = ratio * 1.3;
						}
						if (revitModelData.ExcludeAntiTwistingSquareStirrupShapeNames.Contains(rsi.RebarShape)
							&& (i == 2 || i == curves.Count - 3))
						{
							ratio = ratio * 1.3;
						}
						curves[i] = Autodesk.Revit.DB.Line.CreateBound(line.GetEndPoint(0),
							line.GetEndPoint(0) + line.Direction
							* ConstantValue.BitmapMaxControlLengthFeet * ratio);
					}
					if (curves[i] is Autodesk.Revit.DB.Arc)
					{
						Autodesk.Revit.DB.Arc arc = curves[i] as Autodesk.Revit.DB.Arc;
						curves[i] = arc.CreateTransformed(Autodesk.Revit.DB.Transform.Identity.ScaleBasis(
							ConstantValue.BitmapMaxControlRadiusFeet / arc.Radius));
					}
				}
			}
			else
			{
				var length = ConstantValue.BitmapMaxControlLengthFeet;
				var firstArc = curves[0] as Autodesk.Revit.DB.Arc;
				var radius = firstArc.Radius;

				var scaleTf = Autodesk.Revit.DB.Transform.Identity.ScaleBasis(length / (radius * 2));

				curves[0] = curves[0].CreateTransformed(Autodesk.Revit.DB.Transform.Identity.ScaleBasis(length / (radius * 2)));

				var ratio = 0.975;
				curves[1] = curves[1].CreateTransformed(Autodesk.Revit.DB.Transform.Identity.ScaleBasis(length / (radius * 2) * ratio));

			}

			return curves;
		}
		public static List<Autodesk.Revit.DB.Curve> CombineAndMoveToZero(this RebarShapeInfo rsi, List<Autodesk.Revit.DB.Curve> curves)
		{
			Autodesk.Revit.DB.XYZ fstPnt = curves[0].GetEndPoint(0);
			double minX = fstPnt.X, minY = fstPnt.Y, minZ = fstPnt.Z;
			for (int i = 0; i < curves.Count; i++)
			{
				if (i != 0)
				{
					curves[i] = curves[i].CreateTransformed(
						Autodesk.Revit.DB.Transform.CreateTranslation(
							curves[i - 1].GetEndPoint(1) - curves[i].GetEndPoint(0)));
				}

				Autodesk.Revit.DB.XYZ pnt = curves[i].GetEndPoint(0);
				if (minX > pnt.X) minX = pnt.X;
				if (minY > pnt.Y) minY = pnt.Y;
				if (minZ > pnt.Z) minZ = pnt.Z;

				if (i == curves.Count - 1)
				{
					pnt = curves[i].GetEndPoint(1);
					if (minX > pnt.X) minX = pnt.X;
					if (minY > pnt.Y) minY = pnt.Y;
					if (minZ > pnt.Z) minZ = pnt.Z;
				}
			}

			Autodesk.Revit.DB.Transform translate = Autodesk.Revit.DB.Transform.CreateTranslation(
				new Autodesk.Revit.DB.XYZ(-minX + ConstantValue.BitmapOffsetValueFeet, -minY + ConstantValue.BitmapOffsetValueFeet, -minZ));
			curves = curves.Select(x => x.CreateTransformed(translate)).ToList();

			return curves;
		}
		public static void TranslateSupressCurvesBaseOnCurves(this RebarShapeInfo rsi)
		{
			for (int i = 0; i < rsi.supressBendCurves.Count; i++)
			{
				if (!rsi.NotSupressCheck[i])
				{
					Autodesk.Revit.DB.XYZ p1 = rsi.supressBendCurves[i].GetEndPoint(0), p2 = rsi.supressBendCurves[i].GetEndPoint(1);
					Autodesk.Revit.DB.XYZ midPnt = (p1 + p2) / 2;
					rsi.supressBendCurves[i] = rsi.supressBendCurves[i].CreateTransformed(Autodesk.Revit.DB.Transform.CreateTranslation(
					rsi.curves[rsi.BindingIndexs[i]].GetEndPoint(0) - midPnt));
				}
			}
		}
		public static List<Autodesk.Revit.DB.Curve> IntersectionAndCombineCurve(this RebarShapeInfo rsi, List<Autodesk.Revit.DB.Curve> curves)
		{
			for (int i = 0; i < curves.Count; i++)
			{
				if (curves[i] is Autodesk.Revit.DB.Line)
				{
					Autodesk.Revit.DB.Line l1 = curves[i] as Autodesk.Revit.DB.Line;
					if (i != curves.Count - 1)
					{
						if (curves[i + 1] is Autodesk.Revit.DB.Line)
						{
							Autodesk.Revit.DB.Line l2 = curves[i + 1] as Autodesk.Revit.DB.Line;
							Autodesk.Revit.DB.XYZ intersectPnt = GeometryUtil.GetIntersectionPoint2D(l1, l2);
							if (intersectPnt == null) continue;
							curves[i] = Autodesk.Revit.DB.Line.CreateBound(l1.GetEndPoint(0), intersectPnt);
							curves[i + 1] = Autodesk.Revit.DB.Line.CreateBound(intersectPnt, l2.GetEndPoint(1));
						}
					}
				}
			}
			return curves;
		}
		public static bool SimpleCompare(this RebarShapeInfo rsi, RebarShapeInfo other)
		{
			return rsi.key == other.key;
		}
		public static bool SetValuesBaseOnFields(this RebarShapeInfo rsi, List<string> fieldNames, List<int> fieldValues)
		{
			rsi.Values = new List<int>();
			for (int i = 0; i < rsi.paraNames.Count; i++)
			{
				int j = fieldNames.IndexOf(rsi.paraNames[i]);
				if (j != -1)
				{
					rsi.Values.Add(fieldValues[j]);
				}
				else
				{
					return false;
				}
			}
			return true;
		}
		public static void CreateBitmap(this RebarShapeInfo rsi)
		{
			rsi.Bitmap = new System.Drawing.Bitmap(rsi.Width, rsi.Height);

			for (int i = 0; i < rsi.curves.Count; i++)
			{
				if (rsi.curves[i] is Autodesk.Revit.DB.Line)
				{
					rsi.CreateCurveOnBitmap(rsi.curves[i]);
				}
				else if (rsi.curves[i] is Autodesk.Revit.DB.Arc)
				{
					rsi.CreateCurveOnBitmap(rsi.curves[i], i);
				}
				else
				{
					throw new Exception();
				}
			}
			int j = 0;
			for (int i = 0; i < rsi.modifyCurves.Count; i++)
			{
				if (rsi.modifyCurves[i] is Autodesk.Revit.DB.Arc) continue;
				if (i == 5)
				{
					string str = string.Empty;
				}
				try
				{
					if (revitModelData.ExcludeAntiTwistingSquareStirrupShapeNames
						.Contains(rsi.RebarShape))
					{
						if (i == 4 || i == 5)
						{
							j++; continue;
						}
					}
					rsi.CreateTextOnBitmap(rsi.modifyCurves[i] as Autodesk.Revit.DB.Line, rsi.Values[j], j);
				}
				catch (System.ArgumentOutOfRangeException)
				{
					if (rsi.RebarShape == "TD_01" || rsi.RebarShape == "TD_03") continue;
					throw;
				}
				j++;
			}
		}
		private static void CreateCurveOnBitmap(this RebarShapeInfo rsi, Autodesk.Revit.DB.Curve curve, int i = -1)
		{
			double feet2MM = 1.0.feet2Milimeter();
			System.Drawing.Pen pen = new System.Drawing.Pen(System.Drawing.Color.Black, (float)0.4);
			var graphics = System.Drawing.Graphics.FromImage(rsi.Bitmap);

			if (curve is Autodesk.Revit.DB.Line)
			{
				Autodesk.Revit.DB.Line l = curve as Autodesk.Revit.DB.Line;
				Autodesk.Revit.DB.XYZ pnt1 = l.GetEndPoint(0) * feet2MM, pnt2 = l.GetEndPoint(1) * feet2MM;
				System.Drawing.PointF p1 = new System.Drawing.PointF((float)pnt1.X, (float)pnt1.Y), p2 = new System.Drawing.PointF((float)pnt2.X, (float)pnt2.Y);
				graphics.DrawLine(pen, p1, p2);
			}
			else if (curve is Autodesk.Revit.DB.Arc)
			{
				Autodesk.Revit.DB.Arc arc = curve as Autodesk.Revit.DB.Arc;
				Autodesk.Revit.DB.XYZ center = arc.Center * feet2MM;
				double radius = arc.Radius * feet2MM;
				System.Drawing.RectangleF rec = new System.Drawing.RectangleF((float)(center.X - radius),
					(float)(center.Y - radius), (float)radius * 2, (float)radius * 2);

				Autodesk.Revit.DB.XYZ vec1 = arc.GetEndPoint(0) * feet2MM - center;
				Autodesk.Revit.DB.XYZ vec2 = arc.GetEndPoint(1) * feet2MM - center;

				// Góc của điểm thứ nhất
				double ang1 = Math.Atan(vec1.Y / vec1.X) * 180 / Math.PI;
				if (vec1.X >= 0) ang1 *= 1;
				else ang1 = ang1 * 1 - 180;

				// Góc của điểm thứ hai
				double ang2 = Math.Atan(vec2.Y / vec2.X) * 180 / Math.PI;
				if (vec2.X >= 0) ang2 *= 1;
				else ang2 = ang2 * 1 - 180;

				// Delta của 2 góc
				double delta = ang2 - ang1;
				if (delta > 180) delta -= 360;
				if (delta < -180) delta += 360;

				double absAng1 = Math.Abs(ang1);

				// Góc bằng 180 độ
				if (Math.Abs(delta).IsEqual(180))
				{
					Autodesk.Revit.DB.XYZ midPnt = arc.Evaluate(0.5, true);
					// Vector từ tâm đến điểm giữa cung tròn
					Autodesk.Revit.DB.XYZ vecMid = midPnt - center;
					if (rsi.RebarShape.Contains("TD_O_01"))
					{
						if (rsi.curves[i - 1].GetEndPoint(0).X < rsi.curves[i - 1].GetEndPoint(1).X)
						{
							if (rsi.curves[i].GetEndPoint(0).Y < rsi.curves[i].GetEndPoint(1).Y)
							{
								vecMid = new Autodesk.Revit.DB.XYZ(Math.Abs(vecMid.X), vecMid.Y, vecMid.Z);
							}
							else
							{
								vecMid = new Autodesk.Revit.DB.XYZ(-Math.Abs(vecMid.X), vecMid.Y, vecMid.Z);
							}
						}
						else
						{
							if (rsi.curves[i].GetEndPoint(0).Y < rsi.curves[i].GetEndPoint(1).Y)
							{
								vecMid = new Autodesk.Revit.DB.XYZ(Math.Abs(vecMid.X), vecMid.Y, vecMid.Z);
							}
							else
							{
								vecMid = new Autodesk.Revit.DB.XYZ(-Math.Abs(vecMid.X), vecMid.Y, vecMid.Z);
							}
						}
					}
					else
					{
						if (rsi.curves[i - 1].GetEndPoint(0).X < rsi.curves[i - 1].GetEndPoint(1).X)
						{
							if (rsi.curves[i - 1].GetEndPoint(1).Y < rsi.curves[i + 1].GetEndPoint(0).Y)
							{
								vecMid = new Autodesk.Revit.DB.XYZ(Math.Abs(vecMid.X), vecMid.Y, vecMid.Z);
							}
							else
							{
								vecMid = new Autodesk.Revit.DB.XYZ(-Math.Abs(vecMid.X), vecMid.Y, vecMid.Z);
							}
						}
						else
						{
							if (rsi.curves[i - 1].GetEndPoint(1).Y < rsi.curves[i + 1].GetEndPoint(0).Y)
							{
								vecMid = new Autodesk.Revit.DB.XYZ(Math.Abs(vecMid.X), vecMid.Y, vecMid.Z);
							}
							else
							{
								vecMid = new Autodesk.Revit.DB.XYZ(-Math.Abs(vecMid.X), vecMid.Y, vecMid.Z);
							}
						}

					}
					// Góc của điểm giữa
					double angMid = Math.Atan(vecMid.Y / vecMid.X) * 180 / Math.PI;
					if (vecMid.X >= 0) angMid *= 1;
					else angMid = angMid * 1 - 180;

					double delta1 = angMid - ang1;
					delta1 = rsi.ModifyDeltaInArc180(delta1, rsi.curves.IndexOf(curve), 1);
					double delta2 = ang2 - angMid;
					delta2 = rsi.ModifyDeltaInArc180(delta2, rsi.curves.IndexOf(curve), 2);

					graphics.DrawArc(pen, rec, (float)ang1, (float)delta1);
					graphics.DrawArc(pen, rec, (float)angMid, (float)delta2);
				}
				// Góc khác 180 độ
				else
				{
					graphics.DrawArc(pen, rec, (float)ang1, (float)delta);
				}
			}
			else
			{
				throw new Exception();
			}
		}
		public static void CreateTextOnBitmap(this RebarShapeInfo rsi, Autodesk.Revit.DB.Line line, int value, int j)
		{
			if (rsi.RebarShape.Contains("TD_02") || rsi.RebarShape.Contains("TD_01"))
			{
				if (j < 3)
				{
					double feet2MM = 1.0.feet2Milimeter();
					var graphics = System.Drawing.Graphics.FromImage(rsi.Bitmap);

					Autodesk.Revit.DB.XYZ pnt1 = line.GetEndPoint(0) * feet2MM, pnt2 = line.GetEndPoint(1) * feet2MM;
					Autodesk.Revit.DB.XYZ vecX = line.Direction;
					Autodesk.Revit.DB.XYZ vecY = Autodesk.Revit.DB.XYZ.BasisZ.CrossProduct(vecX);
					Autodesk.Revit.DB.XYZ insertPnt = (pnt1 + pnt2) / 2 + vecY * 5 - vecX * 1;
					System.Drawing.PointF p3 = new System.Drawing.PointF(0, 0);
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
					graphics.DrawString(value.ToString(), new System.Drawing.Font("Arial", (float)ConstantValue.BitmapTextHeightMM), System.Drawing.Brushes.Black, p3);
				}
			}
			else
			{
				double feet2MM = 1.0.feet2Milimeter();
				var graphics = System.Drawing.Graphics.FromImage(rsi.Bitmap);

				Autodesk.Revit.DB.XYZ pnt1 = line.GetEndPoint(0) * feet2MM, pnt2 = line.GetEndPoint(1) * feet2MM;
				Autodesk.Revit.DB.XYZ vecX = line.Direction;
				Autodesk.Revit.DB.XYZ vecY = Autodesk.Revit.DB.XYZ.BasisZ.CrossProduct(vecX);
				Autodesk.Revit.DB.XYZ insertPnt = (pnt1 + pnt2) / 2 + vecY * 5 - vecX * 1;
				System.Drawing.PointF p3 = new System.Drawing.PointF(0, 0);
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
				graphics.DrawString(value.ToString(), new System.Drawing.Font("Arial", (float)ConstantValue.BitmapTextHeightMM), System.Drawing.Brushes.Black, p3);
			}
		}
		public static double ModifyDeltaInArc180(this RebarShapeInfo rsi, double delta, int index, int indexDelta)
		{
			var checkOver180 = true;
			if (rsi.RebarShape == "TD_V_180")
			{
				var y1 = rsi.curves[0].GetEndPoint(0).Y;
				var y2 = rsi.curves[2].GetEndPoint(0).Y;
				// Khi ra hình ảnh, các giá trị Y ngược chiều (tính từ trên xuống)
				if (y1 < y2 && index == 1)
				{
					checkOver180 = false;
				}
				if (y1 > y2 && index == 5)
				{
					if (indexDelta == 2)
					{
						return delta + 360;
					}
					checkOver180 = false;
				}
			}
			if (checkOver180)
			{
				if (delta > 180) delta -= 360;
				if (delta < -180) delta += 360;
			}
			return delta;
		}
	}
}
