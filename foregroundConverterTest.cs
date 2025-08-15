using System;
using System.Globalization;
using System.Windows.Data;

namespace Notepad__
{
    public class foregroundConverterTest : IValueConverter
    {
        public object Convert(object value, Type typeTarget, object param, CultureInfo culture)
        {
            if (value.ToString().EndsWith("Hello"))
            {
                return "Cyan";
            }
            if (value.ToString().EndsWith("World"))
            {
                return "Red";
            }
            else if(value.ToString().EndsWith(" "))
            {
                return "Pink";
            }    
            return "";
        }
        public object ConvertBack(object value, Type typeTarget, object param, CultureInfo culture)
        {
            throw new NotImplementedException();

        }
    }
}
