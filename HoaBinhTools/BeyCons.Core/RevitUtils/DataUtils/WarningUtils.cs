#region Using
using Autodesk.Revit.DB;
using System.Collections.Generic;
using System.Collections.ObjectModel;
#endregion

namespace BeyCons.Core.RevitUtils.DataUtils
{
    public class WarningUtils
    {
        public WarningDiscard WarningDiscard { get; set; }
        public FailureHandlingOptions GetFailureHandling(Transaction transaction)
        {
            WarningDiscard = new WarningDiscard();
            FailureHandlingOptions failureHandlingOptions = transaction.GetFailureHandlingOptions();
            failureHandlingOptions.SetFailuresPreprocessor(WarningDiscard);
            failureHandlingOptions.SetClearAfterRollback(true);
            transaction.SetFailureHandlingOptions(failureHandlingOptions);
            return failureHandlingOptions;
        }
    }
}
