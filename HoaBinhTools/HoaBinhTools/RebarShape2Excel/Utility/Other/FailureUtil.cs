using System.Linq;
using SingleData;

namespace Utility
{
	public static partial class FailureUtil
	{
		public static TemplateData templateData
		{
			get
			{
				return TemplateData.Instance;
			}
		}
		public static RevitData revitData
		{
			get
			{
				return RevitData.Instance;
			}
		}
		public static Model.Entity.Failure GetFailure(Model.Entity.FailureType failureType)
		{
			var failure = templateData.Failures.FirstOrDefault(x => x.FailureType == failureType);
			if (failure == null)
			{
				failure = new Model.Entity.Failure(failureType);
				templateData.Failures.Add(failure);
			}
			return failure;
		}

		public static void SetFailuresPreprocessorInTransaction()
		{
			var tx = revitData.Transaction;
			var failuresPreprocessor = new FuncFailuresPreprocessor()
			{ FuncAccessor = x => PreprocessFailures(x) };
			var failureHandlingOptions = tx.GetFailureHandlingOptions();
			failureHandlingOptions.SetFailuresPreprocessor(failuresPreprocessor);
			failureHandlingOptions.SetClearAfterRollback(true);
			tx.SetFailureHandlingOptions(failureHandlingOptions);
		}
		private static Autodesk.Revit.DB.FailureProcessingResult PreprocessFailures
			(Autodesk.Revit.DB.FailuresAccessor failuresAccessor)
		{
			var failureMessageAccessors = failuresAccessor.GetFailureMessages();

			foreach (var failureMessageAccessor in failureMessageAccessors)
			{
				var fdId = failureMessageAccessor.GetFailureDefinitionId();
				if (fdId.Guid == templateData.Edit_Group_Outside_Error)
				{
					failuresAccessor.DeleteWarning(failureMessageAccessor);
				}
				if (fdId.Guid == templateData.Dimension_Not_Valid_Error)
				{
					if (failureMessageAccessor.HasResolutions())
					{
						failuresAccessor.ResolveFailure(failureMessageAccessor);
					}
					return Autodesk.Revit.DB.FailureProcessingResult.ProceedWithCommit;
				}
			}
			return Autodesk.Revit.DB.FailureProcessingResult.Continue;

			#region SourceCode
			//string transactionName = failuresAccessor.GetTransactionName();
			//IList<FailureMessageAccessor> failureMessageAccessors = failuresAccessor.GetFailureMessages();

			//foreach (FailureMessageAccessor failureMessageAccessor in failureMessageAccessors)
			//{
			//    if (failureMessageAccessor.GetSeverity() == FailureSeverity.Warning)
			//    {
			//        WarningAndErrorReport reporstWarning = new WarningAndErrorReport(failureMessageAccessor.GetFailingElementIds(), failureMessageAccessor.GetDescriptionText());
			//        Warnings.Add(reporstWarning);
			//        failuresAccessor.DeleteWarning(failureMessageAccessor);
			//    }
			//    else if (failureMessageAccessor.GetSeverity() == FailureSeverity.Error)
			//    {
			//        WarningAndErrorReport reporstError = new WarningAndErrorReport(failureMessageAccessor.GetFailingElementIds(), failureMessageAccessor.GetDescriptionText());
			//        Errors.Add(reporstError);
			//        if (failureMessageAccessor.HasResolutions())
			//        {
			//            failuresAccessor.ResolveFailure(failureMessageAccessor);
			//        }
			//        return FailureProcessingResult.ProceedWithCommit;
			//    }
			//}
			//return FailureProcessingResult.Continue;
			#endregion
		}
	}
}
