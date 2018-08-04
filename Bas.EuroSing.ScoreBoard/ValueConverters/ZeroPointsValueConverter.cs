using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace Bas.EuroSing.ScoreBoard.ValueConverters
{
    internal class ZeroPointsValueConverter : IValueConverter
    {
        // converts an amount of points to its string representation, with the exception that zero points should be shown as "-" instead of "0".
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            int intValue = (int)value;

            return (intValue == 0) ? "-" : intValue.ToString();
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
