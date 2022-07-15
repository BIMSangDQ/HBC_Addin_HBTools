using System;
using System.Collections.Generic;
using System.Linq;
using Autodesk.Revit.DB;

namespace Utility
{
	public static class ViewSectionUtil
	{
		/// <summary>
		/// Tạo và trả về mặt cắt cho đối tượng tường đang xét
		/// </summary>
		/// <param name="linkedDoc">Document chứa đối tượng tường đang xét</param>
		/// <param name="doc">Document liên kết</param>
		/// <param name="id">Địa chỉ Id của đối tượng tường đang xét</param>
		/// <param name="viewName">Tên của mặt cắt trả về</param>
		/// <param name="offset">Giá trị offset từ biên đối tượng tường</param>
		/// <returns></returns>
		public static ViewSection CreateWallSection(Document linkedDoc, Document doc, ElementId id, string viewName, double offset)
		{
			Element e = linkedDoc.GetElement(id);
			if (!(e is Wall)) throw new Exception("Element is not a wall!");
			Wall wall = (Wall)e;
			Line line = (wall.Location as LocationCurve).Curve as Line;

			ViewFamilyType vft = new FilteredElementCollector(doc).OfClass(typeof(ViewFamilyType)).Cast<ViewFamilyType>().FirstOrDefault<ViewFamilyType>(x => ViewFamily.Section == x.ViewFamily);

			XYZ p1 = line.GetEndPoint(0), p2 = line.GetEndPoint(1);
			List<XYZ> ps = new List<XYZ> { p1, p2 };
			ps = ps.OrderBy(pnt => pnt.Z).ThenBy(pnt => pnt.Y).ThenBy(pnt => pnt.Z).ToList();
			p1 = ps[0]; p2 = ps[1];

			BoundingBoxXYZ bb = wall.get_BoundingBox(null);
			double minZ = bb.Min.Z, maxZ = bb.Max.Z;

			double l = (p2 - p1).GetLength();
			double h = maxZ - minZ;
			double w = wall.WallType.Width;

			XYZ min = new XYZ(-l / 2 - offset, minZ - offset, -w - offset);
			XYZ max = new XYZ(l / 2 + offset, maxZ + offset, w + offset);

			Transform tf = Transform.Identity;
			tf.Origin = (p1 + p2) / 2;
			tf.BasisX = (p1 - p2).Normalize();
			tf.BasisY = XYZ.BasisZ;
			tf.BasisZ = tf.BasisX.CrossProduct(tf.BasisY);

			BoundingBoxXYZ sectionBox = new BoundingBoxXYZ()
			{
				Transform = tf,
				Min = min,
				Max = max
			};
			ViewSection vs = ViewSection.CreateSection(doc, vft.Id, sectionBox);

			XYZ wallDir = (p2 - p1).Normalize();
			XYZ upDir = XYZ.BasisZ;
			XYZ viewDir = wallDir.CrossProduct(upDir);

			min = p1 - wallDir * offset - viewDir * offset;
			min = new XYZ(min.X, min.Y, minZ - offset);
			max = p2 + wallDir * offset + viewDir * offset;
			max = new XYZ(max.X, max.Y, maxZ + offset);

			tf = vs.get_BoundingBox(null).Transform.Inverse;
			max = tf.OfPoint(max);
			min = tf.OfPoint(min);
			double maxx = 0, maxy = 0, maxz = 0, minx = 0, miny = 0, minz = 0;
			if (max.Z > min.Z)
			{
				maxz = max.Z;
				minz = min.Z;
			}
			else
			{
				maxz = min.Z;
				minz = max.Z;
			}


			if (Math.Round(max.X, 4) == Math.Round(min.X, 4))
			{
				maxx = max.X;
				minx = minz;
			}
			else if (max.X > min.X)
			{
				maxx = max.X;
				minx = min.X;
			}

			else
			{
				maxx = min.X;
				minx = max.X;
			}

			if (Math.Round(max.Y, 4) == Math.Round(min.Y, 4))
			{
				maxy = max.Y;
				miny = minz;
			}
			else if (max.Y > min.Y)
			{
				maxy = max.Y;
				miny = min.Y;
			}

			else
			{
				maxy = min.Y;
				miny = max.Y;
			}

			BoundingBoxXYZ sectionView = new BoundingBoxXYZ();
			sectionView.Max = new XYZ(maxx, maxy, maxz);
			sectionView.Min = new XYZ(minx, miny, minz);

			vs.get_Parameter(BuiltInParameter.VIEWER_VOLUME_OF_INTEREST_CROP).Set(ElementId.InvalidElementId);

			vs.get_Parameter(BuiltInParameter.VIEWER_BOUND_FAR_CLIPPING).Set(0);

			vs.CropBoxActive = true;
			vs.CropBoxVisible = true;

			doc.Regenerate();

			vs.CropBox = sectionView;
			vs.Name = viewName;
			return vs;
		}

		/// <summary>
		/// Tạo và trả về mặt cắt callout cho đối tượng sàn đang xét
		/// </summary>
		/// <param name="doc">Document chứa đối tượng sàn đang xét</param>
		/// <param name="views">List tất cả các view trong mô hình</param>
		/// <param name="level">Tên level chưa đối tượng sàn</param>
		/// <param name="bb">BoundingBox của đối tượng sàn</param>
		/// <param name="viewName">Tên mặt cắt trả về</param>
		/// <param name="offset">Giá trị offset từ biên đối tượng sàn</param>
		/// <returns></returns>
		public static View CreateFloorCallout(Document doc, List<View> views, string level, BoundingBoxXYZ bb, string viewName, double offset)
		{
			ViewFamilyType vft = new FilteredElementCollector(doc).OfClass(typeof(ViewFamilyType))
				.Cast<ViewFamilyType>().FirstOrDefault(x => ViewFamily.FloorPlan == x.ViewFamily);
			XYZ max = bb.Max + XYZ.BasisX * offset + XYZ.BasisY * offset + XYZ.BasisZ * offset;
			XYZ min = bb.Max - XYZ.BasisX * offset - XYZ.BasisY * offset - XYZ.BasisZ * offset;
			bb = new BoundingBoxXYZ { Max = max, Min = min };
			View pv = null;
			string s = string.Empty;
			bool check = false;
			foreach (View v in views)
			{
				try
				{
					s = v.LookupParameter("Associated Level").AsString();
					if (s == level) { pv = v; check = true; break; }
				}
				catch
				{
					continue;
				}
			}
			if (!check) throw new Exception("Invalid level name!");
			View vs = ViewSection.CreateCallout(doc, pv.Id, vft.Id, min, max);
			vs.CropBox = bb;
			vs.Name = viewName;
			return vs;
		}
	}
}
