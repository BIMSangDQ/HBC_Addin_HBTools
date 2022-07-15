#region Using
using BeyCons.Core.FormUtils.ControlViews; 
#endregion

namespace BeyCons.Core.Extensions
{
    public static class NotifyUtils
    {
        public static void LackOfLibrary()
        {
            Notification.ShowDialog("Please install the library for the current Revit version.", false);
        }

        public static void SelectAgain()
        {
            Notification.ShowDialog("Please select elements again.", false);
        }

        public static void OnlyCategory()
        {
            Notification.ShowDialog("Only have a element in the categorys that you selected.", false);
        }

        public static void SelectOnlyElement()
        {
            Notification.ShowDialog("The number of elelemt must be greater than one.", false);
        }

        public static void NoCategorySelect()
        {
            Notification.ShowDialog("No Category is selected.", false);
        }

        public static void IncorrectPath()
        {
            Notification.ShowDialog("File path incorrect.", false);
        }

        public static NotifyResult StartAddin()
        {
            NotifyResult notifyResult = Notification.ShowDialog("Please synchronize with central or save project before using the add-in.", true);
            return notifyResult;
        }

        public static NotifyResult StartAddinAutoJoint()
        {
            NotifyResult notifyResult = Notification.ShowDialog("Đồng bộ hoặc lưu dự án trước khi sử dụng add-in.\n\n\nLưu ý:\n" +
                "Xem xét trình tự thi công cầu thang, ram dốc để hiển thi đúng khi tính cốp pha cột vách dầm sàn.", true);
            return notifyResult;
        }

        public static void SaveFile()
        {
            Notification.ShowDialog("Please save the project.", false);
        }

        public static void Update()
        {
            Notification.ShowDialog("Update successful.", false);
        }

        public static void ResetView()
        {
            Notification.ShowDialog("Please reset the current view again.", false);
        }

        public static void ItemsSourceEmpty()
        {
            Notification.ShowDialog("Don't have the item in list view.", false);
        }
    }
}
