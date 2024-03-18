using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TripPlanner.Models.DTO.UserDTOs;

namespace TripPlanner.Converter
{
    internal class NotificationIconToGrayConverter : IValueConverter
    {
        public object Convert(object values, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (values == null)
                return "";

            NotificationDTO n = (NotificationDTO)values;
            string source = "";
            if (n.IsVisited && !string.IsNullOrEmpty(n.IconPath))
            {
                string path = n.IconPath; //np. bill_circle_sec.png
                int index = path.LastIndexOf('_');
                source = path.Substring(0, index) + "_gray.png";
            }
            else
                return n.IconPath;

            return source;
        }

        public object ConvertBack(object value, Type targetTypes, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException("Going back, this action isn't supported.");
        }
    }
}
