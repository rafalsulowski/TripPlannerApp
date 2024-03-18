using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TripPlanner.Models.DTO.ScheduleDTOs;

namespace TripPlanner.Converter
{
    internal class DurationConverter : IValueConverter
    {
        public object Convert(object values, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            return "";
            //DateTime startTime = (DateTime)values;
            //DateTime stopTime = (DateTime)values[1];
            //return $"({(startTime - stopTime):hh}h {(startTime - stopTime):mm}m)";
        }

        public object ConvertBack(object value, Type targetTypes, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException("Going back, this action isn't supported.");
        }
    }
}
