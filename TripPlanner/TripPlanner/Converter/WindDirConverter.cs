using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TripPlanner.Converter
{
    internal class WindDirConverter : IValueConverter
    {
        public object Convert(object values, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            int degre = (int)values;
            if (degre < 30 || degre > 330)
                return "N";
            else if (degre < 60)
                return "NE";
            else if (degre < 120)
                return "NE";
            else if (degre < 150)
                return "SE";
            else if (degre < 210)
                return "S";
            else if (degre < 240)
                return "SW";
            else if (degre < 300)
                return "W";
            else if (degre < 330)
                return "NW";
            else
                return "N";
        }

        public object ConvertBack(object value, Type targetTypes, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException("Going back, this action isn't supported.");
        }
    }
}
