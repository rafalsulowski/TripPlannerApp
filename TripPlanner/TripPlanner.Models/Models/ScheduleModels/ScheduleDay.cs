
using TripPlanner.Models.DTO.ScheduleDTOs;
using TripPlanner.Models.Models.TourModels;

namespace TripPlanner.Models.Models.ScheduleModels
{
    public class ScheduleDay
    {
        public int Id { get; set; }


        public int TourId { get; set; }
        public Tour Tour { get; set; } = null!;
        public ICollection<ScheduleEvent> Events { get; set; } = new List<ScheduleEvent>();


        public DateTime Date { get; set; }
        public string Description { get; set; } = string.Empty;


        public static implicit operator ScheduleDayDTO(ScheduleDay data)
        {
            if (data == null)
                return null;

            return new ScheduleDayDTO
            {
                Id = data.Id,
                Date = data.Date,
                Description = data.Description,
                TourId = data.TourId,
                Events = data.Events.Select(u => (ScheduleEventDTO)u).ToList(),
            };
        }
    }
}
