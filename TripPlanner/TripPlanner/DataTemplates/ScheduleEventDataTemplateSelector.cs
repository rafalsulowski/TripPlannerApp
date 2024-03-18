using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TripPlanner.Helpers;
using TripPlanner.Models.DTO.BillDTOs;
using TripPlanner.Models.DTO.ScheduleDTOs;

namespace TripPlanner.DataTemplates
{
    public class ScheduleEventDataTemplateSelector : DataTemplateSelector
    {
        public DataTemplate Simple { get; set; }
        public DataTemplate WithDuration { get; set; }

        protected override DataTemplate OnSelectTemplate(object item, BindableObject container)
        {
            var elem = (ScheduleEventDTO)item;
            if (elem.StartTime == elem.StopTime)
                return Simple;
            else
                return WithDuration;
        }
    }
}
