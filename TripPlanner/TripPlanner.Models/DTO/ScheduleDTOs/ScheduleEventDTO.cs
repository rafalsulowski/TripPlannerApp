
using TripPlanner.Models.Models.ScheduleModels;

namespace TripPlanner.Models.DTO.ScheduleDTOs
{
    public class ScheduleEventDTO
    {
        public int Id { get; set; }

        public int ScheduleDayId { get; set; }

        public string Name { get; set; } = string.Empty;
        public DateTime StartTime { get; set; }
        public DateTime StopTime { get; set; }


        public static implicit operator ScheduleEvent(ScheduleEventDTO data)
        {
            if (data == null)
                return null;

            return new ScheduleEvent
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
