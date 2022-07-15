#region Using
using System.Collections;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Media;
#endregion

namespace BeyCons.Core.FormUtils
{
    public static class UIUtils
    {
        public static List<T> GetLogicalChildCollection<T>(object parent) where T : DependencyObject
        {
            List<T> logicalCollection = new List<T>();
            GetLogicalChildCollection(parent as DependencyObject, logicalCollection);
            return logicalCollection;
        }

        private static void GetLogicalChildCollection<T>(DependencyObject parent, List<T> logicalCollection) where T : DependencyObject
        {
            IEnumerable children = LogicalTreeHelper.GetChildren(parent);
            foreach (object child in children)
            {
                if (child is DependencyObject)
                {
                    DependencyObject depChild = child as DependencyObject;
                    if (child is T)
                    {
                        logicalCollection.Add(child as T);
                    }
                    GetLogicalChildCollection(depChild, logicalCollection);
                }
            }
        }

        public static System.Windows.Controls.CheckBox CheckBox(string nameCheckBox)
        {
            System.Windows.Controls.CheckBox getCheckBox = null;
            foreach (System.Windows.Controls.CheckBox checkBox in WindowData.Instance.CheckBoxs)
            {
                if (checkBox.Name == nameCheckBox)
                {
                    getCheckBox = checkBox;
                    break;
                }
            }
            return getCheckBox;
        }

        public static System.Windows.Controls.ComboBox ComboBox(string nameComboBox)
        {
            System.Windows.Controls.ComboBox getComboBox = null;
            foreach (System.Windows.Controls.ComboBox comboBox in WindowData.Instance.ComboBoxs)
            {
                if (comboBox.Name == nameComboBox)
                {
                    getComboBox = comboBox;
                    break;
                }
            }
            return getComboBox;
        }
        public static System.Windows.Controls.ListBox ListBox(string nameListBox)
        {
            System.Windows.Controls.ListBox getListBox = null;
            foreach (System.Windows.Controls.ListBox comboBox in WindowData.Instance.ListBoxs)
            {
                if (comboBox.Name == nameListBox)
                {
                    getListBox = comboBox;
                    break;
                }
            }
            return getListBox;
        }

        public static System.Windows.Controls.TabItem TabItem(string nameTabItem)
        {
            System.Windows.Controls.TabItem getTabItem = null;
            foreach (System.Windows.Controls.TabItem tabItem in WindowData.Instance.TabItems)
            {
                if (tabItem.Name == nameTabItem)
                {
                    getTabItem = tabItem;
                    break;
                }
            }
            return getTabItem;
        }

        public static System.Windows.Controls.ListView ListView(string nameListView)
        {
            System.Windows.Controls.ListView getListView = null;
            foreach (System.Windows.Controls.ListView listView in WindowData.Instance.ListView)
            {
                if (listView.Name == nameListView)
                {
                    getListView = listView;
                    break;
                }
            }
            return getListView;
        }
        public static System.Windows.Controls.TextBox TextBox(string nameTextBox)
        {
            System.Windows.Controls.TextBox getTextBox = null;
            foreach (System.Windows.Controls.TextBox textBox in WindowData.Instance.TextBoxs)
            {
                if (textBox.Name == nameTextBox)
                {
                    getTextBox = textBox;
                    break;
                }
            }
            return getTextBox;
        }

        public static IEnumerable<T> FindVisualChildren<T>(DependencyObject depObj) where T : DependencyObject
        {
            if (depObj != null)
            {
                for (int i = 0; i < VisualTreeHelper.GetChildrenCount(depObj); i++)
                {
                    DependencyObject child = VisualTreeHelper.GetChild(depObj, i);
                    if (child != null && child is T)
                    {
                        yield return (T)child;
                    }

                    foreach (T childOfChild in FindVisualChildren<T>(child))
                    {
                        yield return childOfChild;
                    }
                }
            }
        }
    }
}