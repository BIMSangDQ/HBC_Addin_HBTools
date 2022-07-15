using System;

namespace RevisionMultiSheet
{
	class GlobalVar
	{
		public const double FLOOR_VERTEX_OFFSET = 0.2;
		public const double WALL_VERTEX_OFFSET = 5;

		public const String SHARED_PARAM_FILE_EXT = ".txt";
		public const String SHARED_PARAM_FILE_NAME = "BIMAreaCalculationSharedParameter";

		public const String BOTTOM_FACE_AREA_PARAM = "Bottom Area";
		public const String SIDE_FACE_AREA_PARAM = "SideFace Area";
		public const String TOP_FACE_AREA_PARAM = "Top Area";

		//public const int DECIMAL_DIGIT_ROUNDED = 6;

		public const String IMAGES_FOLDER = "img";

		public const double DISTANCE_COMPARE_TOLERANCE = 1.0e-3;
	}
}
