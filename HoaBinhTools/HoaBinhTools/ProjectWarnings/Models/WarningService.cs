using System.Collections.Generic;
using System.Linq;
using Autodesk.Revit.DB;
using MoreLinq;
using Utils;

namespace HoaBinhTools.ProjectWarnings.Models
{
	public class WarningService
	{
		public static List<ElementId> CompareSolidWall(List<ElementId> ids)
		{
			List<Wall> walls = ids.Select(e => ActiveData.Document.GetElement(e) as Wall).ToList();

			Wall wallMaxSolid = null;

			List<ElementId> SolidZero = new List<ElementId>();

			try
			{
				var maxVolumn = double.MinValue;

				foreach (var ws in walls)
				{
					var wSolid = ws.GetAllSolids().First().Volume;

					if (wSolid > maxVolumn)
					{
						maxVolumn = wSolid;

						wallMaxSolid = ws;
					}
				}



				if (wallMaxSolid == null) return SolidZero;

				Solid maxSolid = wallMaxSolid.GetAllSolids().First();

				foreach (var w in walls)
				{
					if (ActiveData.Document.GetElement(w.Id) == null) continue;

					if (w.Id != wallMaxSolid.Id)
					{
						var solid = w.GetAllSolids().First().Clone();

						try
						{
							Solid remainSolid = BooleanOperationsUtils.ExecuteBooleanOperation(solid, maxSolid, BooleanOperationsType.Difference);

							if (remainSolid.Volume < 0.001)
							{
								SolidZero.Add(w.Id);
							}
						}
						catch
						{

						}
					}
				}
			}
			catch
			{

			}

			return SolidZero;
		}

		public static List<ElementId> CompareSolidFloor(List<ElementId> ids)
		{
			List<Floor> walls = ids.Select(e => ActiveData.Document.GetElement(e) as Floor).ToList();

			Floor wallMaxSolid = null;

			List<ElementId> SolidZero = new List<ElementId>();

			try
			{
				var maxVolumn = double.MinValue;

				foreach (var ws in walls)
				{
					var wSolid = ws.GetAllSolids().First().Volume;

					if (wSolid > maxVolumn)
					{
						maxVolumn = wSolid;

						wallMaxSolid = ws;
					}
				}

				if (wallMaxSolid == null) return SolidZero;

				Solid maxSolid = wallMaxSolid.GetAllSolids().First();

				foreach (var w in walls)
				{
					if (ActiveData.Document.GetElement(w.Id) == null) continue;

					if (w.Id != wallMaxSolid.Id)
					{
						var solid = w.GetAllSolids().First().Clone();

						try
						{
							Solid remainSolid = BooleanOperationsUtils.ExecuteBooleanOperation(solid, maxSolid, BooleanOperationsType.Difference);

							if (remainSolid.Volume < 0.001)
							{
								SolidZero.Add(w.Id);
							}
						}
						catch
						{

						}
					}
				}
			}
			catch
			{

			}

			return SolidZero;
		}

	}
}
