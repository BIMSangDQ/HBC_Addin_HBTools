using System.Collections.Generic;
using Autodesk.Revit.DB;

namespace HoaBinhTools.AutocadToRevit.Utils
{
	public class DeleteWarningSuper : IFailuresPreprocessor
	{
		public int NumberErr;
		public FailureProcessingResult PreprocessFailures(FailuresAccessor failuresAccessor)
		{
			FailureProcessingResult failureProcessingResult;
			IList<FailureMessageAccessor> failList = failuresAccessor.GetFailureMessages();
			if (failList.Count != 0)
			{
				foreach (FailureMessageAccessor failure in failList)
				{
					FailureSeverity s = failure.GetSeverity();
					if (s == FailureSeverity.Warning)
					{
						failuresAccessor.DeleteWarning(failure);
					}
					else if (s == FailureSeverity.Error)
					{
						failuresAccessor.ResolveFailure(failure);
						NumberErr += 1;
					}
				}
				failureProcessingResult = FailureProcessingResult.ProceedWithCommit;
			}
			else
			{
				failureProcessingResult = FailureProcessingResult.Continue;
			}
			return failureProcessingResult;
		}
	}
}
