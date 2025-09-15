using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Media3D;

namespace MKW.GUI
{
    public static class ListViewBehaviours
    {
        public const string DeselectOnClickAwayPropertyName = "DeselectOnClickAway";

        public static readonly DependencyProperty DeselectOnClickAwayProperty =
            DependencyProperty.RegisterAttached(
                nameof(DeselectOnClickAwayPropertyName),
                typeof(bool),
                typeof(ListViewBehaviours),
                new PropertyMetadata(false, OnDeselectOnClickAwayChanged));

        public static void SetDeselectOnClickAway(UIElement element, bool value)
        {
            element.SetValue(DeselectOnClickAwayProperty, value);
        }

        public static bool GetDeselectOnClickAway(UIElement element)
        {
            return (bool)element.GetValue(DeselectOnClickAwayProperty);
        }

        private static void OnDeselectOnClickAwayChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is ListView listView)
            {
                if ((bool)e.NewValue)
                {
                    listView.PreviewMouseDown += ListView_PreviewMouseDown;
                }
                else
                {
                    listView.PreviewMouseDown -= ListView_PreviewMouseDown;
                }
            }
        }

        private static void ResetSelection(ListView listView)
        {
            if (listView.SelectedItem != null)
            {
                listView.SelectedItem = null;
                Keyboard.ClearFocus();
            }
        }

        private static void ListView_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            if (sender is ListView listView)
            {
                if (ShouldResetSelection(e.OriginalSource as DependencyObject))
                {
                    ResetSelection(listView);
                }
            }
        }

        private static bool ShouldResetSelection(DependencyObject? element)
        {
            while (element != null)
            {
                if (element is ListViewItem || element is GridViewColumnHeader)
                {
                    return false;
                }

                if (element is Visual || element is Visual3D)
                {
                    element = VisualTreeHelper.GetParent(element);
                }
                else
                {
                    element = LogicalTreeHelper.GetParent(element);
                }
            }

            return true;
        }
    }
}
