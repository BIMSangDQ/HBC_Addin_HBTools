using System;
using Autodesk.Revit.DB;

namespace Utility
{
	public class FuncFailuresPreprocessor : IFailuresPreprocessor
	{
		public Func<FailuresAccessor, FailureProcessingResult> FuncAccessor { get; set; }
		public FailureProcessingResult PreprocessFailures(FailuresAccessor failuresAccessor)
		{
			return FuncAccessor(failuresAccessor);
		}
	}
}
