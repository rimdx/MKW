using System.Globalization;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Media;

namespace MKW.GUI
{
    public class TreeViewIndentConverter : IValueConverter
    {
        public double IndentSize { get; set; } = 16.0;

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is TreeViewItem item)
            {
                int level = 0;
                DependencyObject parent = VisualTreeHelper.GetParent(item);

                while (parent != null)
                {
                    if (parent is TreeViewItem) level++;
                    parent = VisualTreeHelper.GetParent(parent);
                }

                return level * IndentSize;
            }

            return 0;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
