#region Using
using System.Collections.Generic;
using System.Windows;
#endregion

namespace BeyCons.Core.FormUtils
{
    public class WindowData
    {
        public static WindowData Instance { get; set; }        

        #region Variable
        private List<System.Windows.Controls.CheckBox> checkBoxs;
        private List<System.Windows.Controls.ComboBox> comboBoxes;
        private List<System.Windows.Controls.TabItem> tabItems;
        private List<System.Windows.Controls.ListView> listView;
        private List<System.Windows.Controls.ListBox> listBoxes;
        private List<System.Windows.Controls.TextBox> textBox;
        #endregion

        #region Properties
        public Window UIWindow { get; set; }
        public List<System.Windows.Controls.ListBox> ListBoxs
        {
            get
            {
                if (null == listBoxes) listBoxes = UIUtils.GetLogicalChildCollection<System.Windows.Controls.ListBox>(UIWindow);
                return listBoxes;
            }
        }
        public List<System.Windows.Controls.TextBox> TextBoxs
        {
            get
            {
                if (null == textBox) textBox = UIUtils.GetLogicalChildCollection<System.Windows.Controls.TextBox>(UIWindow);
                return textBox;
            }
        }
        public List<System.Windows.Controls.CheckBox> CheckBoxs
        {
            get
            {
                if (null == checkBoxs) checkBoxs = UIUtils.GetLogicalChildCollection<System.Windows.Controls.CheckBox>(UIWindow);
                return checkBoxs;
            }
        }
        public List<System.Windows.Controls.ComboBox> ComboBoxs
        {
            get
            {
                if (null == comboBoxes) comboBoxes = UIUtils.GetLogicalChildCollection<System.Windows.Controls.ComboBox>(UIWindow);
                return comboBoxes;
            }
        }
        public List<System.Windows.Controls.TabItem> TabItems
        {
            get
            {
                if (null == tabItems) tabItems = UIUtils.GetLogicalChildCollection<System.Windows.Controls.TabItem>(UIWindow);
                return tabItems;
            }
        }
        public List<System.Windows.Controls.ListView> ListView
        {
            get
            {
                if (null == listView) listView = UIUtils.GetLogicalChildCollection<System.Windows.Controls.ListView>(UIWindow);
                return listView;
            }
        }
        #endregion
    }
}
