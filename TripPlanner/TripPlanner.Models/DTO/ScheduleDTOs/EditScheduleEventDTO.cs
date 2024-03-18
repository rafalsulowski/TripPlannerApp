﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TripPlanner.Models.Models.ScheduleModels;

namespace TripPlanner.Models.DTO.ScheduleDTOs
{
    public class EditScheduleEventDTO
    {
        public string Name { get; set; } = string.Empty;
        public DateTime StartTime { get; set; }
        public DateTime StopTime { get; set; }


        public static implicit operator ScheduleEvent(EditScheduleEventDTO data)
        {
            if (data == null)
                return null;

            return new ScheduleEvent
            {
                Name = data.Name,
                StopTime = data.StopTime,
                StartTime = data.StartTime,
            };
        }
    }
}
