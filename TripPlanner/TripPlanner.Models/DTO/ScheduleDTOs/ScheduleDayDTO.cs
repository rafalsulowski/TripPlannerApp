
using TripPlanner.Models.Models.ScheduleModels;
using TripPlanner.Models.Models.TourModels;

namespace TripPlanner.Models.DTO.ScheduleDTOs
{
    public class ScheduleDayDTO
    {
        public int Id { get; set; }

        public int TourId { get; set; }
        public ICollection<ScheduleEventDTO> Events { get; set; } = new List<ScheduleEventDTO>();

        public DateTime Date { get; set; }
        public string Description { get; set; } = string.Empty;


        public static implicit operator ScheduleDay(ScheduleDayDTO data)
        {
            if (data == null)
                return null;

            return new ScheduleDay
            {
                Id = data.Id,
                Date = data.Date,
                Description = data.Description,
                TourId = data.TourId,
                Events = data.Events.Select(u => (ScheduleEvent)u).ToList(),
            };
        }
    }
}
