// Copyright (c) Timofei Zhakov. All Rights Reserved
// Licensed under the Apache License, Version 2.0.

using System.Globalization;
using System.Text;
using System.Windows.Data;

namespace MKW.GUI
{
    public class SingleLineConverter : IValueConverter
    {
        public static IValueConverter Instance { get; } = new SingleLineConverter();

        public object? Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is string str)
            {
                return CompactText(str);
            }
            else
            {
                return null;
            }
        }

        public object? ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return null;
        }

        public static string CompactText(string text)
        {
            StringBuilder sb = new StringBuilder();

            bool hasWhitespace = false;
            foreach (char ch in text)
            {
                if (char.IsWhiteSpace(ch))
                {
                    hasWhitespace = true;
                }
                else
                {
                    if (hasWhitespace)
                    {
                        sb.Append(' ');
                        hasWhitespace = false;
                    }

                    sb.Append(ch);
                }
            }

            return sb.ToString();
        }
    }
}
