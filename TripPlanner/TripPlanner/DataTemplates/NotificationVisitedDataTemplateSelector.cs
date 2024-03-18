using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TripPlanner.Helpers;
using TripPlanner.Models.DTO.BillDTOs;
using TripPlanner.Models.DTO.UserDTOs;

namespace TripPlanner.DataTemplates
{
    public class NotificationVisitedDataTemplateSelector : DataTemplateSelector
    {
        public DataTemplate Visited { get; set; }
        public DataTemplate NotVisited { get; set; }

        protected override DataTemplate OnSelectTemplate(object item, BindableObject container)
        {
            return ((NotificationDTO)item).IsVisited ? Visited : NotVisited;
        }
    }
}
