using System;
using System.Collections.Generic;
using Autodesk.Revit.DB;

namespace Utils
{
	public static class FailureUtil
	{

		public static void SetFailuresPreprocessorInTransaction(this Autodesk.Revit.DB.Transaction tx)
		{
			var failuresPreprocessor = new FuncFailuresPreprocessor()
			{
				FuncAccessor = x => PreprocessFailures(x)
			};

			var failureHandlingOptions = tx.GetFailureHandlingOptions();

			failureHandlingOptions.SetFailuresPreprocessor(failuresPreprocessor);

			failureHandlingOptions.SetClearAfterRollback(true);

			tx.SetFailureHandlingOptions(failureHandlingOptions);
		}


		private static Autodesk.Revit.DB.FailureProcessingResult PreprocessFailures(Autodesk.Revit.DB.FailuresAccessor failuresAccessor)
		{
			IList<FailureMessageAccessor> failureMessageAccessors = failuresAccessor.GetFailureMessages();

			foreach (FailureMessageAccessor failureMessageAccessor in failureMessageAccessors)
			{
				FailureDefinitionId fdId = failureMessageAccessor.GetFailureDefinitionId();

				if (fdId == BuiltInFailures.OverlapFailures.FloorsOverlap)
				{

					//SingleData.ModelData.FailingElementIds = failureMessageAccessor.GetFailingElementIds().ToList();


					//SingleData.ModelData.check = true;

				}

				if (null != fdId)
				{
					failuresAccessor.DeleteWarning(failureMessageAccessor);
				}

				//Xóa
				// failuresAccessor.ResolveFailure(failureMessageAccessor);
				// return Autodesk.Revit.DB.FailureProcessingResult.ProceedWithCommit;

			}
			return Autodesk.Revit.DB.FailureProcessingResult.Continue;
		}







		public class WarningDiscard : IFailuresPreprocessor
		{
			FailureProcessingResult IFailuresPreprocessor.PreprocessFailures(FailuresAccessor failuresAccessor)
			{
				String transactionName = failuresAccessor.GetTransactionName();

				IList<FailureMessageAccessor> fmas = failuresAccessor.GetFailureMessages();

				if (fmas.Count == 0)
				{
					return FailureProcessingResult.Continue;
				}

				bool isResolved = false;


				foreach (FailureMessageAccessor fma in fmas)
				{
					if (fma.HasResolutions())
					{
						failuresAccessor.ResolveFailure(fma);
						isResolved = true;

					}


				}

				if (isResolved)
				{
					return FailureProcessingResult.ProceedWithCommit;
				}

				return FailureProcessingResult.Continue;
			}
		}











	}





	public class FuncFailuresPreprocessor : IFailuresPreprocessor
	{
		public Func<FailuresAccessor, FailureProcessingResult> FuncAccessor { get; set; }
		public FailureProcessingResult PreprocessFailures(FailuresAccessor failuresAccessor)
		{
			return FuncAccessor(failuresAccessor);
		}


	}
}
