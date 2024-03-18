
using TripPlanner.Models.DTO.ScheduleDTOs;

namespace TripPlanner.Models.Models.ScheduleModels
{
    public class ScheduleEvent
    {
        public int Id { get; set; }


        public int ScheduleDayId { get; set; }
        public ScheduleDay ScheduleDay { get; set; } = null!;


        public string Name { get; set; } = string.Empty;
        public DateTime StartTime { get; set; }
        public DateTime StopTime { get; set; }


        public static implicit operator ScheduleEventDTO(ScheduleEvent data)
        {
            if (data == null)
                return null;

            return new ScheduleEventDTO
            {
                Id = data.Id,
                Name = data.Name,
                StopTime = data.StopTime,
                StartTime = data.StartTime,
                ScheduleDayId = data.ScheduleDayId,
            };
        }
    }
}
