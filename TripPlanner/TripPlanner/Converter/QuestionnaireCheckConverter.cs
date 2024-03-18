using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TripPlanner.Helpers;
using TripPlanner.Models.DTO.MessageDTOs.QuestionnaireDTOs;

namespace TripPlanner.Converter
{
    internal class QuestionnaireCheckConverter : IValueConverter
    {
        public object Convert(object values, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            List<QuestionnaireVoteDTO> list = (List<QuestionnaireVoteDTO>)values;

            try
            {
                Configuration configuration = ServicesHelper.Current.GetService<Configuration>();
                if (configuration == null)
                    throw new Exception();

                if(list.Exists(u => u.UserId == configuration.User.Id))
                    return "circle_ok_sec.png";
                else
                    return "circle_sec.png";
            }
            catch (Exception)
            {
                Shell.Current.CurrentPage.DisplayAlert("Awaria", "Zły system operacyjny! Czat jest dostępny tylko na: Windows, Android, Ios, MacCatalyst", "Ok");
                return "circle_sec.png";
            }
        }

        public object ConvertBack(object value, Type targetTypes, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException("Going back, this action isn't supported.");
        }
    }
}
