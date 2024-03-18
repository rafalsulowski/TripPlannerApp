using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TripPlanner.Models.DTO.TourDTOs;
using TripPlanner.Models.DTO.UserDTOs;

namespace TripPlanner.DataTemplates
{
    public class FriendDataTemplateSelector : DataTemplateSelector
    {
        public DataTemplate IsParticipant { get; set; }
        public DataTemplate IsNotParticipant { get; set; }

        protected override DataTemplate OnSelectTemplate(object item, BindableObject container)
        {
            return ((ExtendFriendDTO)item).IsParticipant? IsParticipant: IsNotParticipant;
        }
    }
}
