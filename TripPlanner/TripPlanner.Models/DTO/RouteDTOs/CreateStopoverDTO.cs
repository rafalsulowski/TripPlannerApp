using TripPlanner.Models.Models.RouteModels;

namespace TripPlanner.Models.DTO.RouteDTOs
{
    public class CreateStopoverDTO
    {
        public int RouteId { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string Location { get; set; } = string.Empty;
        public int BreakTime { get; set; }


        public static implicit operator Stopover(CreateStopoverDTO data)
        {
            if (data == null)
                return null;

            return new Stopover
            {
                RouteId = data.RouteId,
                Name = data.Name,
                Description = data.Description,
                Location = data.Location,
                BreakTime = data.BreakTime
            };
        }
    }
}
