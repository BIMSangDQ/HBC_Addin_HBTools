using System;
using System.Collections.Generic;
using System.Linq;
using Autodesk.Revit.DB;
using MoreLinq;


namespace Utils
{
	public static class FaceUtils
	{
		const double _eps = 1.0E-4;


		// lấy các mặt phẳng vuông góc với vecto
		public static IList<PlanarFace> PerpendicularFace(this IList<PlanarFace> listFaces, XYZ vector)
		{
			IList<PlanarFace> planarFaceList = new List<PlanarFace>();
			foreach (PlanarFace listFace in listFaces)
			{
				if (IsPerpendicular(listFace, vector))
					planarFaceList.Add(listFace);
			}
			return planarFaceList;
		}




		public static double GetCrossSectionalAreaFraming(this FamilyInstance framing, Document doc)
		{
			Curve framingLocation = (framing.Location as LocationCurve).Curve;

			Transform transform = framingLocation.ComputeDerivatives(0, true);

			var planarFaces = framing.GetSolidsFromOriginalFamilyInstance().Faces;

			List<PlanarFace> pF = new List<PlanarFace>();

			foreach (var item in planarFaces)
			{

				if (item is PlanarFace p)
				{
					if (p != null)
					{
						pF.Add(p);
					}
				}

			}


			pF = pF.Where(e => e.FaceNormal.AngleBetweenTwoVectors(XYZ.BasisZ, false) > 45).ToList();



			foreach (PlanarFace planarFace in pF)
			{

				if (transform.BasisX.AngleBetweenTwoVectors(planarFace.FaceNormal, false) < 5)
				{


					CurveLoop curveLoop = planarFace.GetEdgesAsCurveLoops().MaxBy(x => x.GetExactLength()).FirstOrDefault();

					if (curveLoop.IsRectangular(curveLoop.GetPlane()))
					{
						IEnumerable<Line> lines = curveLoop.Select(x => x).OfType<Line>();

						Line width = lines.Where(y => y.Direction.AngleBetweenTwoVectors(XYZ.BasisZ, false) > 45).FirstOrDefault();

						Line height = lines.Where(y => y.Direction.AngleBetweenTwoVectors(XYZ.BasisZ, false) < 45).FirstOrDefault();

						if (null != width && null != height)
						{
							return width.Length.FootToMm() * height.Length.FootToMm();
						}
					}
				}
			}
			return -1;
		}





		// Summary:
		//     Item 1 là chiều rông width Iteam 2 là chiều cao Hight

		public static Tuple<double, double> GetWidthAndHight(this FamilyInstance framing, Document doc)
		{

			Curve framingLocation = (framing.Location as LocationCurve).Curve;

			Transform transform = framingLocation.ComputeDerivatives(0, true);

			var planarFaces = framing.GetSolidsFromOriginalFamilyInstance().Faces;

			List<Face> pF = new List<Face>();

			foreach (var item in planarFaces)
			{
				pF.Add(item as Face);
			}

			pF = pF.Where(e => e.ComputeNormal(new UV(0, 0)).AngleBetweenTwoVectors(XYZ.BasisZ, false) > 45).ToList();

			foreach (PlanarFace planarFace in pF)
			{

				if (transform.BasisX.AngleBetweenTwoVectors(planarFace.FaceNormal, false) < 5)
				{

					CurveLoop curveLoop = planarFace.GetEdgesAsCurveLoops().MaxBy(x => x.GetExactLength()).FirstOrDefault();

					if (curveLoop.IsRectangular(curveLoop.GetPlane()))
					{
						IEnumerable<Line> lines = curveLoop.Select(x => x).OfType<Line>();

						Line width = lines.Where(y => y.Direction.AngleBetweenTwoVectors(XYZ.BasisZ, false) > 45).FirstOrDefault();

						Line height = lines.Where(y => y.Direction.AngleBetweenTwoVectors(XYZ.BasisZ, false) < 45).FirstOrDefault();

						if (null != width && null != height)
						{
							return new Tuple<double, double>(Math.Round(width.Length.FootToMm(), 1), Math.Round(height.Length.FootToMm(), 0));
						}
					}
				}
			}
			return null;
		}



		// kiểm tra xem vecto có vuông góc với mặt phẳng ko
		public static bool IsPerpendicular(this PlanarFace face, XYZ vector)
		{
			bool flag = false;

			XYZ faceNormal = face.FaceNormal;
			// độ dài vecto / do dai 

			if ((
					vector.CrossProduct(faceNormal)
					/ (vector.GetLength() * faceNormal.GetLength())

				).GetLength() < _eps)

				flag = true;

			return flag;
		}



		public static PlanarFace GetTopFace(this List<PlanarFace> FlanarFaces)
		{
			PlanarFace Bottom_Face = null;

			foreach (PlanarFace pf in FlanarFaces)
			{

				if (null != pf && (Math.Abs(pf.FaceNormal.X - 0) < _eps && Math.Abs(pf.FaceNormal.Y - 0) < _eps))
				{
					if ((null == Bottom_Face) || (Bottom_Face.Origin.Z < pf.Origin.Z))
					{
						Bottom_Face = pf;
					}
				}
			}
			return Bottom_Face;
		}



		public static Face GetTopFace(this List<Face> faces)
		{
			foreach (var face in faces)
			{
				double Angle = Math.Abs(MathUtils.AngleBetweenTwoVectors(face.ComputeNormal(new UV((face.GetBoundingBox().Min.U + face.GetBoundingBox().Max.U) / 2, (face.GetBoundingBox().Min.V + face.GetBoundingBox().Max.V) / 2)) + face.ComputeNormal(new UV(0.5, 0.5)), XYZ.BasisZ, true));

				// bé hơn góc 45 // mặt top
				if (Angle <= 45)
				{
					if (face.Area > 1E-04)
					{
						return face;
					}
				}
			}

			return null;
		}


		// nếu mặt nghiên có thể lấy trọng tâm của face rồi xét cao đọ
		//public static PlanarFace GetBotFace(this List<Face> faces)
		//{
		//    PlanarFace Bottom_Face = null;

		//    foreach (Face f in faces)
		//    {
		//        PlanarFace pf = f as PlanarFace;

		//        if (null != pf && (Math.Abs(pf.FaceNormal.X - 0) < _eps && Math.Abs(pf.FaceNormal.Y - 0) < _eps))
		//        {
		//            if ((null == Bottom_Face) || (Bottom_Face.Origin.Z > pf.Origin.Z))
		//            {
		//                Bottom_Face = pf;
		//            }
		//        }
		//    }
		//    return Bottom_Face;
		//}


		//public static PlanarFace GetBotFace(this FaceArray faces)
		//{
		//    PlanarFace Bottom_Face = null;

		//    foreach (Face f in faces)
		//    {
		//        PlanarFace pf = f as PlanarFace;

		//        if (null != pf && (Math.Abs(pf.FaceNormal.X - 0) < _eps && Math.Abs(pf.FaceNormal.Y - 0) < _eps))
		//        {
		//            if ((null == Bottom_Face) || (Bottom_Face.Origin.Z > pf.Origin.Z))
		//            {
		//                Bottom_Face = pf;
		//            }
		//        }
		//    }
		//    return Bottom_Face;
		//}

		//public static PlanarFace GetBotFace(this List<PlanarFace> FlanarFaces)
		//{
		//    PlanarFace Bottom_Face = null;

		//    foreach (PlanarFace pf in FlanarFaces)
		//    {

		//        if (null != pf && (Math.Abs(pf.FaceNormal.X - 0) < _eps && Math.Abs(pf.FaceNormal.Y - 0) < _eps))
		//        {
		//            if ((null == Bottom_Face) || (Bottom_Face.Origin.Z > pf.Origin.Z))
		//            {
		//                Bottom_Face = pf;
		//            }
		//        }
		//    }
		//    return Bottom_Face;
		//}


		public static PlanarFace GetBotFaceOrigin(this List<PlanarFace> faces)
		{
			PlanarFace Bottom_Face = null;



			foreach (PlanarFace f in faces)
			{


				if (f is PlanarFace planar)
				{
					if (Math.Abs((planar.FaceNormal.Z + 1)) < 0.01)
					{
						if (planar.Area > 0.01)
						{
							Bottom_Face = planar;
						}
					}
				}
			}
			return Bottom_Face;
		}


		public static PlanarFace GetBotFaceVector(this Solid solid)
		{
			PlanarFace planarFaces = null;

			List<Face> faces = solid.GetFacesToSoild();

			foreach (Face face in faces)
			{
				if (face is PlanarFace planar)
				{
					if (Math.Abs((planar.FaceNormal.Z + 1)) < 0.01)
					{
						if (planar.Area > 0.000025.MmToFoot())
						{
							planarFaces = planar;
						}
					}
				}
			}

			return planarFaces;
		}

		public static PlanarFace GetBotFace(this Solid solid)
		{
			PlanarFace Bottom_Face = null;

			FaceArray faces = solid.Faces;

			foreach (Face f in faces)
			{
				PlanarFace pf = f as PlanarFace;

				if (null != pf && (Math.Abs(pf.FaceNormal.X - 0) < _eps && Math.Abs(pf.FaceNormal.Y - 0) < _eps))
				{
					if ((null == Bottom_Face) || (Bottom_Face.Origin.Z > pf.Origin.Z))
					{
						Bottom_Face = pf;
					}
				}
			}
			return Bottom_Face;
		}

	}
}
