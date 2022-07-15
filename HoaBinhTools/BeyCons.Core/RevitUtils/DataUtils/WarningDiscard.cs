#region Using
using Autodesk.Revit.DB;
using BeyCons.Core.FormUtils.Reports;
using System.Collections.Generic;
using System.Collections.ObjectModel;
#endregion

namespace BeyCons.Core.RevitUtils.DataUtils
{
    public class WarningDiscard : IFailuresPreprocessor
    {
        public ObservableCollection<WarningAndErrorReport> Warnings { get; set; } = new ObservableCollection<WarningAndErrorReport>();
        public ObservableCollection<WarningAndErrorReport> Errors { get; set; } = new ObservableCollection<WarningAndErrorReport>();
        public FailureProcessingResult PreprocessFailures(FailuresAccessor failuresAccessor)
        {
            IList<FailureMessageAccessor> failureMessageAccessors = failuresAccessor.GetFailureMessages();

            foreach (FailureMessageAccessor failureMessageAccessor in failureMessageAccessors)
            {
                if (failureMessageAccessor.GetSeverity() == FailureSeverity.Warning)
                {
                    WarningAndErrorReport reporstWarning = new WarningAndErrorReport(failureMessageAccessor.GetFailingElementIds(), failureMessageAccessor.GetDescriptionText());
                    Warnings.Add(reporstWarning);
                    failuresAccessor.DeleteWarning(failureMessageAccessor);
                }
                else if (failureMessageAccessor.GetSeverity() == FailureSeverity.Error)
                {
                    WarningAndErrorReport reporstError = new WarningAndErrorReport(failureMessageAccessor.GetFailingElementIds(), failureMessageAccessor.GetDescriptionText());
                    Errors.Add(reporstError);
                    if (failureMessageAccessor.HasResolutions())
                    {
                        failuresAccessor.ResolveFailure(failureMessageAccessor);
                    }
                    return FailureProcessingResult.ProceedWithCommit;
                }
            }
            return FailureProcessingResult.Continue;
        }
    }
}