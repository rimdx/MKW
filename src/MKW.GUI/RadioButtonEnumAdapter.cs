using System.Globalization;
using System.Windows.Data;

namespace MKW.GUI
{
    public class RadioButtonEnumAdapter : IValueConverter
    {
        public object Convert(object value,
                              Type targetType,
                              object parameter,
                              CultureInfo culture)
        {
            if (parameter != null && parameter.Equals(value))
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public object ConvertBack(object value,
                                  Type targetType,
                                  object parameter,
                                  CultureInfo culture)
        {
            if (value != null && value.Equals(true))
            {
                return parameter;
            }
            else
            {
                return Binding.DoNothing;
            }
        }
    }
}
