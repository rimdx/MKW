using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace MKW.GUI.Behaviors
{
    public static class TextBoxBehavior
    {
        public const string SelectAllTextOnFocusPropertyPropertyName = "SelectAllTextOnFocus";

        public static readonly DependencyProperty SelectAllTextOnFocusProperty =
            DependencyProperty.RegisterAttached(
                "SelectAllTextOnFocusPropertyPropertyName",
                typeof(bool),
                typeof(TextBoxBehavior),
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
            if (d is TextBox textBox && e.NewValue is bool newValue)
            {
                if (newValue)
                {
                    textBox.GotFocus += TextBox_GotFocus;
                    textBox.PreviewMouseDown += TextBox_PreviewMouseDown;
                }
                else
                {
                    textBox.GotFocus -= TextBox_GotFocus;
                    textBox.PreviewMouseDown -= TextBox_PreviewMouseDown;
                }
            }
        }

        private static void TextBox_GotFocus(object sender, RoutedEventArgs e)
        {
            TextBox? textBox = e.OriginalSource as TextBox;
            textBox?.SelectAll();
        }

        private static void TextBox_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            if (sender is TextBox textBox)
            {
                if (textBox.IsReadOnly || !textBox.IsKeyboardFocusWithin)
                {
                    e.Handled = true;
                    textBox.Focus();
                }
            }
        }
    }
}
