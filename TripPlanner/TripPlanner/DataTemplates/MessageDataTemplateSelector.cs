using TripPlanner.Helpers;
using TripPlanner.Models.DTO.MessageDTOs;
using TripPlanner.Models.DTO.MessageDTOs.QuestionnaireDTOs;

namespace TripPlanner.DataTemplates
{
    public class MessageDataTemplateSelector : DataTemplateSelector
    {
        public DataTemplate NullMessage { get; set; }
        public DataTemplate NoticeMessage { get; set; }
        public DataTemplate TextOtherMessage { get; set; }
        public DataTemplate TextMyMessage { get; set; }
        public DataTemplate QuestionnaireMessage { get; set; }

        protected override DataTemplate OnSelectTemplate(object item, BindableObject container)
        {
            try
            {
                Configuration configuration = ServicesHelper.Current.GetService<Configuration>();
                if (item is TextMessageDTO)
                {
                    if (configuration.User.Id == ((TextMessageDTO)item).UserId)
                        return TextMyMessage;
                    else
                        return TextOtherMessage;
                }
                else if (item is QuestionnaireDTO)
                {
                    return QuestionnaireMessage;
                }
                else if (item is NoticeMessageDTO)
                {
                    return NoticeMessage;
                }
                else
                    return NullMessage;
            }
            catch (Exception)
            {
                Shell.Current.CurrentPage.DisplayAlert("Awaria", "Zły system operacyjny! Czat jest dostępny tylko na: Windows, Android, Ios, MacCatalyst", "Ok");
                return NullMessage;
            }
        }
    }
}
