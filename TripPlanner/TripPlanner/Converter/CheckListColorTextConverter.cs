using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TripPlanner.Converter
{
    internal class CheckListColorTextConverter : IValueConverter
    {
        public object Convert(object values, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if ((bool)values)
                return Color.FromRgb(147, 147, 147);
            else
                return Color.FromRgb(82,171,152);
        }

        public object ConvertBack(object value, Type targetTypes, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException("Going back, this action isn't supported.");
        }
    }
}
