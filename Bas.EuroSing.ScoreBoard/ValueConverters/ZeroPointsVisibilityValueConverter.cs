using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;

namespace Bas.EuroSing.ScoreBoard.ValueConverters
{
    internal class ZeroPointsVisibilityValueConverter : IValueConverter
    {
        // Converts an int to visibility, where a value of 0 is collapsed, and everything else is visible.
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is int && 
                (int)value == 0)
            {
                return Visibility.Collapsed;
            }
            else
            {
                return Visibility.Visible;
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
