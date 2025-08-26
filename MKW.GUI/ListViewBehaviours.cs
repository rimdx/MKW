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
                    listView.Loaded += ListView_Loaded;
                }
                else
                {
                    listView.PreviewMouseDown -= ListView_PreviewMouseDown;
                    listView.Loaded -= ListView_Loaded;
                }
            }
        }

        private static void ResetSelection(ListView listView)
        {
            {
                if (listView.SelectedItem != null)
                {
                    listView.SelectedItem = null;
                    Keyboard.ClearFocus();
                }
            }
        }

        private static void ListView_Loaded(object sender, RoutedEventArgs e)
        {
            if (sender is ListView listView)
            {
                Window window = Window.GetWindow(listView);

                if (window != null)
                {
                    window.PreviewMouseDown += (sender, we) =>
                    {
                        if (!IsClickInsideListView(listView, we.OriginalSource as DependencyObject))
                        {
                            ResetSelection(listView);
                        }
                    };
                }
            }
        }

        private static void ListView_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            if (sender is ListView listView)
            {
                ResetSelection(listView);
            }
        }

        private static bool IsClickInsideListView(ListView listView, DependencyObject? source)
        {
            while (source != null)
            {
                if (source == listView)
                {
                    return true;
                }

                if (source is Visual || source is Visual3D)
                {
                    source = VisualTreeHelper.GetParent(source);
                }
                else
                {
                    source = LogicalTreeHelper.GetParent(source);
                }
            }

            return false;
        }
    }
}
