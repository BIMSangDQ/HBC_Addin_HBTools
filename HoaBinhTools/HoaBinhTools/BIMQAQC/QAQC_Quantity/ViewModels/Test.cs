
#region Beam
//List<Element> ListBeam = ListElement.Where(x => x.Category.Name == "Structural Framing").ToList();

////ProgressBarInstance progressBarInstance = new ProgressBarInstance("Check Beam.", ListBeam.Count);
//foreach (Element Element in ListBeam)
//{
//	//progressBarInstance.Start();
//	if (ListElementHaveVoid.Contains(Element.Id)) continue;

//	double Height = Element.GetProximityHeight();
//	double Width = GetProximityWidthFraming(Element) / 2;

//	List<Solid> solids = GeometryUtils.GetSolidsFromInstanceElement(Element, options, true).ToUnionSolids();
//	List<PlanarFace> planarFaces = new List<PlanarFace>();
//	XYZ Zvt = new XYZ(0, 0, -1);
//	XYZ Yvt = new XYZ();
//	XYZ Yvt2 = new XYZ();

//	bool IsFail = false;

//	XYZ XYZFail = new XYZ();
//	foreach (Solid solid in solids)
//	{
//		List<PlanarFace> p = GeometryLib.GetPlanarFaceHaveNormalVectorParallelVector(solid, new XYZ(0, 0, -1));
//		PlanarFace face = p[0];

//		Yvt = face.YVector;
//		Yvt2 = -1 * Yvt;
//		LocationCurve locationCurve = Element.Location as LocationCurve;
//		if (locationCurve.Curve is Line locationLine)
//		{

//			XYZ lineDirection = locationLine.Direction;
//			double lenght = locationLine.Length;

//			int maxstepHor = (int)(lenght / increment);
//			int maxstepVert = (int)(Height / (increment * 2));
//			int maxstepHorY = (int)(Width / (increment * 2));

//			SolidCurveIntersectionOptions solidCurveIntersectionOptions = new SolidCurveIntersectionOptions();
//			solidCurveIntersectionOptions.ResultType = SolidCurveIntersectionMode.CurveSegmentsInside;

//			for (int j = 1; j < 5; j++)//maxstepVert-1
//			{
//				for (int i = 1; i < maxstepHor - 1; i++)
//				{
//					XYZ newXYZ = LocationStartPoint(Element, Height, Width) + i * increment * lineDirection + j * increment * Zvt;

//					for (int k = 0; k < maxstepHorY; k++)
//					{
//						XYZ newXYZHor = newXYZ + k * increment * Yvt;
//						Solid solidfromPoint = GeometryUtils.CreateCubeSolid(newXYZHor, 5 / 304.8);

//						IsFail = false;
//						foreach (Solid solidintersec in SolidGeneric)
//						{
//							IsFail = true;
//							//Không giao thì check tiếp
//							if (IntersectUtils.DoesIntersect(solidfromPoint, solidintersec) == false)
//							{
//								continue;
//							}
//							//Có giao
//							else
//							{
//								IsFail = false;
//								break;
//							}
//						}
//						if (IsFail)
//						{
//							XYZFailList.Add(newXYZHor);
//							break;
//						}

//						//
//						newXYZHor = newXYZ + k * increment * Yvt2;
//						solidfromPoint = GeometryUtils.CreateCubeSolid(newXYZHor, 5 / 304.8);

//						IsFail = false;
//						foreach (Solid solidintersec in SolidGeneric)
//						{
//							IsFail = true;
//							if (IntersectUtils.DoesIntersect(solidfromPoint, solidintersec) == false)
//							{
//								continue;
//							}
//							else
//							{
//								IsFail = false;
//								break;
//							}
//						}
//						if (IsFail)
//						{
//							XYZFailList.Add(newXYZHor);
//							break;
//						}
//					}
//					if (IsFail) break;
//				}
//				if (IsFail) break;
//			}

//			///This Element have void
//			///check các element va chạm => Check void
//			///
//			if (IsFail)
//			{
//				ListElementHaveVoid.Add(Element.Id);
//				ListElement1.Add(Element.Id);

//				List<Element> elements = IntersectUtils.DoIntersect(Element, ListElement, options, true, 0);
//				foreach (Element e in elements)
//				{
//					ListElementHaveVoid.Add(e.Id);
//				}
//				break;
//			}
//		}
//	}
//}


#endregion
