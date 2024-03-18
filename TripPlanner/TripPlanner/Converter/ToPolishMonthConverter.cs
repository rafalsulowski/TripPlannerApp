
namespace TripPlanner.Converter
{
    internal class ToPolishMonthConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            DateTime dt = (DateTime)value;
            string mName = "";
            switch(dt.Month)
            {
                case 1:
                    mName = "Sty";
                    break;
                case 2:
                    mName = "Lut";
                    break;
                case 3:
                    mName = "Mar";
                    break;
                case 4:
                    mName = "Kwi";
                    break;
                case 5:
                    mName = "Maj";
                    break;
                case 6:
                    mName = "Cze";
                    break;
                case 7:
                    mName = "Lip";
                    break;
                case 8:
                    mName = "Sie";
                    break;
                case 9:
                    mName = "Wrz";
                    break;
                case 10:
                    mName = "Paź";
                    break;
                case 11:
                    mName = "Lis";
                    break;
                case 12:
                    mName = "Gru";
                    break;

                default:
                    mName = "Sty";
                    break;
            }

            return $"{dt.Day} {mName} {dt.Year}";
        }

        public object ConvertBack(object value, Type targetTypes, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException("Going back to what you had isn't supported.");
        }
    }
}
