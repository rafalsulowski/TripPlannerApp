using TripPlanner.Models.DTO.UserDTOs;
using TripPlanner.Models.Models.BillModels;

namespace TripPlanner
{
    public class Configuration
    {
        public readonly int AddChatMessagesWhileReload = 200; //ile wiadomosci dodatkowo wyswietlic na czacie przy odswierzeniu okna
        public readonly int WeatherDaysForecast = 14; //ile dni do przodu pobierac pogodę

        //public bool IsLoggedIn = true;
        public bool IsLoggedIn = false;
        
        public readonly string WeatherApiKey = "T7L3SQPQFTS43N9C5GWD2Z4U2";
        public readonly string WeatherApiUrl = "https://weather.visualcrossing.com/VisualCrossingWebServices/rest/services/timeline";

        //public readonly string WebApiUrl = DeviceInfo.Platform == DevicePlatform.Android ? "https://80506417585769.lhr.life/api" : "https://80506417585769.lhr.life/api";
        //public readonly string WssChatUrl = DeviceInfo.Platform == DevicePlatform.Android ? "https://80506417585769.lhr.life/chat" : "https://80506417585769.lhr.life/chat";
        //public readonly string WssCheckListUrl = DeviceInfo.Platform == DevicePlatform.Android ? "https://80506417585769.lhr.life/checklist" : "https://80506417585769.lhr.life/checklist";
        //public readonly string WssNotificationUrl = DeviceInfo.Platform == DevicePlatform.Android ? "https://80506417585769.lhr.life/notification" : "https://80506417585769.lhr.life/notification";
        //public readonly string WssMapsUrl = DeviceInfo.Platform == DevicePlatform.Android ? "https://80506417585769.lhr.life/maps" : "https://80506417585769.lhr.life/maps";



        public readonly string WebApiUrl = DeviceInfo.Platform == DevicePlatform.Android ? "http://10.0.2.2:5119/api" : "http://localhost:5119/api";
        public readonly string WssChatUrl = DeviceInfo.Platform == DevicePlatform.Android ? "http://10.0.2.2:5119/chat" : "http://localhost:5119/chat";
        public readonly string WssCheckListUrl = DeviceInfo.Platform == DevicePlatform.Android ? "http://10.0.2.2:5119/checklist" : "http://localhost:5119/checklist";
        public readonly string WssNotificationUrl = DeviceInfo.Platform == DevicePlatform.Android ? "http://10.0.2.2:5119/notification" : "http://localhost:5119/notification";
        public readonly string WssMapsUrl = DeviceInfo.Platform == DevicePlatform.Android ? "http://10.0.2.2:5119/maps" : "http://localhost:5119/maps";


        public UserDTO User { get; set; } = null;

        //public UserDTO User { get; set; } = new UserDTO
        //{
        //    Id = 21,
        //    FullName = "Rafał Sulowski",
        //    FullAddress = "Willowa 34a, Lublin 20-819",
        //    City = "Lublin",
        //    Email = "rmsulowski@gmail.com"
        //};

        public string GetLongNameOfDayWeek(DateTime dt)
        {
            switch (dt.DayOfWeek)
            {
                case DayOfWeek.Sunday:
                    return "Niedziela";
                case DayOfWeek.Monday:
                    return "Poniedzialek";
                case DayOfWeek.Tuesday:
                    return "Wtorek";
                case DayOfWeek.Wednesday:
                    return "Sroda";
                case DayOfWeek.Thursday:
                    return "Czwartek";
                case DayOfWeek.Friday:
                    return "Piatek";
                case DayOfWeek.Saturday:
                    return "Sobota";
                default:
                    return "";
            }
        }

        public string GetShortNameOfDayWeek(DateTime dt)
        {
            switch (dt.DayOfWeek)
            {
                case DayOfWeek.Sunday:
                    return "Nd";
                case DayOfWeek.Monday:
                    return "Pn";
                case DayOfWeek.Tuesday:
                    return "Wt";
                case DayOfWeek.Wednesday:
                    return "Śr";
                case DayOfWeek.Thursday:
                    return "Czw";
                case DayOfWeek.Friday:
                    return "Pt";
                case DayOfWeek.Saturday:
                    return "Sb";
                default:
                    return "";
            }
        }

        public string GetTourTitleWithoutWhiteSpacesAndPolishCharacter(string title)
        {
            return normalizeString(title.Trim().Replace(' ', '_'));
        }

        public string normalizeString(string str)
        {
            string newStr = "";
            foreach(char c in str)
                switch (c)
                {
                    case 'ą':
                        newStr += 'a';
                        break;
                    case 'ć':
                        newStr += 'c';
                        break;
                    case 'ę':
                        newStr += 'e';
                        break;
                    case 'ł':
                        newStr += 'l';
                        break;
                    case 'ń':
                        newStr += 'n';
                        break;
                    case 'ó':
                        newStr += 'o';
                        break;
                    case 'ś':
                        newStr += 's';
                        break;
                    case 'ż':
                    case 'ź':
                        newStr += 'z';
                        break;
                    default:
                        newStr += c;
                        break;
                }
            return newStr;
        }

        public string GetBillTypeName(BillType type)
        {
            switch (type)
            {
                case BillType.Equally:
                    return "równomierny";
                case BillType.Unequally:
                    return "nierównomierny";
                case BillType.ByPercentages:
                    return "procentowy";
                case BillType.ByShares:
                    return "przez udział";
                case BillType.ByAdjustment:
                    return "przez dodanie";
                default:
                    return "równomierny";
            }
        }
    }
}
