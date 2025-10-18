// Copyright (c) Timofei Zhakov. All Rights Reserved
// Licensed under the Apache License, Version 2.0.

using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace MKW.GUI.Controls
{
    public class PasswordBoxBehavior
    {
        public const string SelectAllTextOnFocusPropertyPropertyName = "SelectAllTextOnFocus";

        public static readonly DependencyProperty SelectAllTextOnFocusProperty =
            DependencyProperty.RegisterAttached(
                "SelectAllTextOnFocusPropertyPropertyName",
                typeof(bool),
                typeof(PasswordBoxBehavior),
                new UIPropertyMetadata(false, OnSelectAllTextOnFocusChanged));

        public static void SetSelectAllTextOnFocus(UIElement element, bool value)
        {
            element.SetValue(SelectAllTextOnFocusProperty, value);
        }

        public static bool GetSelectAllTextOnFocus(UIElement element)
        {
            return (bool)element.GetValue(SelectAllTextOnFocusProperty);
        }

        private static void OnSelectAllTextOnFocusChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is PasswordBox input && e.NewValue is bool newValue)
            {
                if (newValue)
                {
                    input.GotFocus += PasswordBox_GotFocus;
                    input.PreviewMouseDown += PasswordBox_PreviewMouseDown;
                }
                else
                {
                    input.GotFocus -= PasswordBox_GotFocus;
                    input.PreviewMouseDown -= PasswordBox_PreviewMouseDown;
                }
            }
        }

        private static void PasswordBox_GotFocus(object sender, RoutedEventArgs e)
        {
            PasswordBox? input = e.OriginalSource as PasswordBox;
            input?.SelectAll();
        }

        private static void PasswordBox_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            if (sender is PasswordBox input)
            {
                if (!input.IsKeyboardFocusWithin)
                {
                    e.Handled = true;
                    input.Focus();
                }
            }
        }
    }
}
