#region Using
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
#endregion

namespace BeyCons.Core.FormUtils.TagHelpers
{
    public class TextBoxHelper : DependencyObject
    {
        #region IsNumberic
        public static bool GetIsNumeric(DependencyObject obj)
        {
            return (bool)obj.GetValue(IsNumericProperty);
        }

        public static void SetIsNumeric(DependencyObject obj, bool value)
        {
            obj.SetValue(IsNumericProperty, value);
        }

        public static readonly DependencyProperty IsNumericProperty = DependencyProperty.RegisterAttached("IsNumeric", typeof(bool), typeof(TextBoxHelper), new PropertyMetadata(false, new PropertyChangedCallback((s, e) =>
        {
            if (s is TextBox targetTextbox)
            {
                if ((bool)e.OldValue && !((bool)e.NewValue))
                {
                    targetTextbox.PreviewTextInput -= TargetTextboxNumeric_PreviewTextInput;

                }
                if ((bool)e.NewValue)
                {
                    targetTextbox.PreviewTextInput += TargetTextboxNumeric_PreviewTextInput;
                    targetTextbox.PreviewKeyDown += TargetTextboxNumeric_PreviewKeyDown;
                }
            }
        })));

        static void TargetTextboxNumeric_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            char newChar = e.Text.ToString()[0];
            e.Handled = !char.IsNumber(newChar) && !(newChar == '-');
        }
        #endregion

        static void TargetTextboxNumeric_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            e.Handled = e.Key == Key.Space;
        }

        #region IsNonNegative
        public static bool GetIsNonNegative(DependencyObject obj)
        {
            return (bool)obj.GetValue(IsNonNegative);
        }

        public static void SetIsNonNegative(DependencyObject obj, bool value)
        {
            obj.SetValue(IsNonNegative, value);
        }

        public static readonly DependencyProperty IsNonNegative = DependencyProperty.RegisterAttached("IsNonNegative", typeof(bool), typeof(TextBoxHelper), new PropertyMetadata(false, new PropertyChangedCallback((s, e) =>
        {
            if (s is TextBox targetTextbox)
            {
                if ((bool)e.OldValue && !((bool)e.NewValue))
                {
                    targetTextbox.PreviewTextInput -= TargetTextboxNonNegative_PreviewTextInput;

                }
                if ((bool)e.NewValue)
                {
                    targetTextbox.PreviewTextInput += TargetTextboxNonNegative_PreviewTextInput;
                    targetTextbox.PreviewKeyDown += TargetTextboxNumeric_PreviewKeyDown;
                }
            }
        })));

        static void TargetTextboxNonNegative_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            char newChar = e.Text.ToString()[0];
            e.Handled = !char.IsNumber(newChar);
        }
        #endregion
    }
}