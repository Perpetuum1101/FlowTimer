using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace FlowTimer.UI.Components.Converters
{
    public class BoolToStrikethrough : IValueConverter
    {
        public object Convert(object value, Type targetType, object para, CultureInfo culture)
        {
            if ((bool)value)
            {
                return TextDecorations.Strikethrough;
            }
            else
            {
                return null;
            }
        }

        public object ConvertBack(object value, Type targetType, object para, CultureInfo culture)
        {
            return new NotImplementedException();
        }
    }
}
