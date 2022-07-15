using System;
using System.Collections.Generic;
using System.Linq;
using Autodesk.Revit.DB;

namespace Utils
{
	/// <summary>
	/// Lấy lấy các vật thể giao với soild list
	/// </summary>
	public static class IntersectsUtils
	{




		public static List<Element> IntersectBounbingbox(this Element ele, Document doc)
		{
			BoundingBoxXYZ bb = ele.get_BoundingBox(null);

			Outline outline = new Outline(bb.Min, bb.Max);

			BoundingBoxIntersectsFilter bbfilter = new BoundingBoxIntersectsFilter(outline, 10.0.MmToFoot());

			return new FilteredElementCollector(doc).WherePasses(bbfilter).Where(e => e.Id != ele.Id)

				.ToList();
		}

		public static List<Element> IntersectBounbingbox(this Element ele, Document doc, View view)
		{

			// Lấy boundingbox view 3D
			BoundingBoxXYZ bb = ele.get_BoundingBox(null);

			Outline outline = new Outline(bb.Min, bb.Max);

			BoundingBoxIntersectsFilter bbfilter = new BoundingBoxIntersectsFilter(outline, 10.0.MmToFoot());

			var T = new FilteredElementCollector(doc, view.Id).WherePasses(bbfilter).Where(e => e.Id != ele.Id)
				.ToList();

			return T;
		}




		public static List<Element> GetElementIntersectsSolid(this Solid soid, Document doc)
		{
			var collector = new FilteredElementCollector(doc)

			 .WherePasses(new ElementIntersectsSolidFilter(soid)).WhereElementIsNotElementType().Cast<Element>().ToList();

			return collector;
		}


		public static List<Element> GetElementIntersectsSolid(this Solid soid, List<Element> Eles, Document doc)
		{
			var collector = new FilteredElementCollector(doc, Eles.Select(e => e.Id).ToList())

			 .WherePasses(new ElementIntersectsSolidFilter(soid))

			 .WhereElementIsNotElementType().Cast<Element>().ToList();

			return collector;
		}


		public static List<Element> GetElementIntersectsSolid(this Solid soid, Document doc, View View)
		{
			var collector = new FilteredElementCollector(doc, View.Id)

				.WherePasses(new ElementIntersectsSolidFilter(soid))

				.WhereElementIsNotElementType().Cast<Element>().ToList();

			return collector;
		}


		public static List<Face> GetFacesToSoild(this List<Solid> Solids)
		{
			if (Solids == null) return null;

			List<Face> Feces = new List<Face>();

			foreach (Solid S in Solids)
			{
				foreach (Face F in S.Faces)
				{
					Feces.Add(F);
				}
			}
			return Feces;
		}


		public static List<Edge> GetEdgesToSoild(this List<Solid> Solids)
		{
			List<Edge> Edg = new List<Edge>();

			foreach (Solid S in Solids)
			{
				foreach (Edge F in S.Edges)
				{
					Edg.Add(F);
				}
			}
			return Edg;
		}

		public static List<Face> GetFacesToSoild(this Solid Solid)
		{
			if (Solid == null) return null;

			List<Face> Feces = new List<Face>();

			foreach (Face F in Solid.Faces)
			{
				Feces.Add(F);
			}

			return Feces;
		}


		public static List<Edge> GetEdgesToSoild(this Solid Solid)
		{
			List<Edge> Edg = new List<Edge>();

			foreach (Edge F in Solid.Edges)
			{
				Edg.Add(F);
			}

			return Edg;
		}


		public static bool IsCollision(this Element E1, Element E2)
		{
			try
			{
				Solid firstSolid = E1.GetAllSolids().UnionSoilds();

				Solid secondSolid = E2.GetAllSolids().UnionSoilds();

				if (firstSolid == null || secondSolid == null)
				{
					return false;
				}
				else
				{
					Solid solidOne = SolidUtils.Clone(firstSolid);

					Solid solidTwo = SolidUtils.Clone(secondSolid);

					Solid solidUnion = new List<Solid>() { solidOne, solidTwo }.UnionSoilds();

					if (null == solidUnion)
					{
						try
						{
							Solid solidIntersect = BooleanOperationsUtils.ExecuteBooleanOperation(solidOne, solidTwo, BooleanOperationsType.Intersect);
							if (solidIntersect.Volume > MathUtils.EPSILON_VOLUME)
							{
								return true;
							}
							else
							{
								if (IntersectSolids(solidOne, solidTwo))
								{
									return true;
								}
								else if (IntersectSolids(solidTwo, solidOne))
								{
									return true;
								}
								else
								{
									return false;
								}
							}
						}
						catch
						{
							try
							{
								Solid solidDifference = BooleanOperationsUtils.ExecuteBooleanOperation(solidOne, solidTwo, BooleanOperationsType.Difference);
								if (Math.Abs(solidOne.Volume - solidDifference.Volume) > MathUtils.EPSILON_VOLUME)
								{
									return true;
								}
								else
								{
									if (IntersectSolids(solidOne, solidTwo))
									{
										return true;
									}
									else if (IntersectSolids(solidTwo, solidOne))
									{
										return true;
									}
									else
									{
										return false;
									}
								}
							}
							catch
							{
								if (IntersectSolids(solidOne, solidTwo))
								{
									return true;
								}
								else if (IntersectSolids(solidTwo, solidOne))
								{
									return true;
								}
								else
								{
									return false;
								}
							}
						}
					}
					else
					{
						bool booArea = Math.Abs(solidUnion.SurfaceArea - solidOne.SurfaceArea - solidTwo.SurfaceArea) > MathUtils.EPSILON_VOLUME;
						bool booEdge = Math.Abs(solidOne.Edges.Size + solidTwo.Edges.Size - solidUnion.Edges.Size) > 0;
						if (booArea || booEdge)
						{
							return true;
						}
						else
						{
							if (IntersectSolids(solidOne, solidTwo))
							{
								return true;
							}
							else if (IntersectSolids(solidTwo, solidOne))
							{
								return true;
							}
							else
							{
								return false;
							}
						}
					}
				}
			}
			catch
			{
				List<Solid> S1 = E1.GetAllSolids();

				List<Solid> S2 = E2.GetAllSolids();

				List<Face> Fs1 = GetFacesToSoild(S1);

				List<Edge> Ed2 = GetEdgesToSoild(S2);

				foreach (Face F in Fs1)
				{

					foreach (Edge e in Ed2)
					{

						if (F.Intersect(e.AsCurve()) == SetComparisonResult.Overlap)
						{
							return true;

						}
					}
				}

				List<Face> Fs2 = GetFacesToSoild(S2);

				List<Edge> Ed1 = GetEdgesToSoild(S1);

				foreach (Face F in Fs2)
				{

					foreach (Edge e in Ed1)
					{
						if (F.Intersect(e.AsCurve()) == SetComparisonResult.Overlap)
						{
							return true;
						}
					}
				}
				return false;
			}
		}
		public static bool IntersectSolids(Solid solidOne, Solid solidTwo)
		{
			foreach (Face faceOne in solidOne.Faces)
			{
				foreach (Edge edge in solidTwo.Edges)
				{
					IList<XYZ> epts = edge.Tessellate();
					foreach (XYZ ept in epts)
					{
						try
						{
							IntersectionResult intersectionResult = faceOne.Project(ept);
							if (null != intersectionResult)
							{
								if (Math.Abs(intersectionResult.Distance) < 0.001)
								{
									intersectionResult.Dispose();
									return true;
								}
								intersectionResult.Dispose();
							}
						}
						catch { }
					}
				}
			}
			return false;
		}




	}
}
