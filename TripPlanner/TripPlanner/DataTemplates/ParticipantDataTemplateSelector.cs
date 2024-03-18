using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TripPlanner.Helpers;
using TripPlanner.Models.DTO.TourDTOs;
using TripPlanner.Models.DTO.UserDTOs;

namespace TripPlanner.DataTemplates
{
    public class ParticipantDataTemplateSelector : DataTemplateSelector
    {
        public DataTemplate MyPersonNormal { get; set; }
        public DataTemplate MyPersonOrganizer { get; set; }
        public DataTemplate OrganizerParticipant { get; set; }
        public DataTemplate NormalParticipant { get; set; }

        protected override DataTemplate OnSelectTemplate(object item, BindableObject container)
        {
            try
            {
                Configuration configuration = ServicesHelper.Current.GetService<Configuration>();

                if (configuration == null)
                    throw new Exception();
                else if (((ExtendParticipantDTO)item).UserId == configuration.User.Id && ((ExtendParticipantDTO)item).IsOrganizer)
                    return MyPersonOrganizer;
                else if (((ExtendParticipantDTO)item).UserId == configuration.User.Id)
                    return MyPersonNormal;
                else if (((ExtendParticipantDTO)item).IsOrganizer)
                    return OrganizerParticipant;
                else
                    return NormalParticipant;
            }
            catch (Exception)
            {
                Shell.Current.CurrentPage.DisplayAlert("Awaria", "Zły system operacyjny! Czat jest dostępny tylko na: Windows, Android, Ios, MacCatalyst", "Ok");
                return NormalParticipant;
            }
        }
    }
}
