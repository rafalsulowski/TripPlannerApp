using TripPlanner.Models.DTO.RouteDTOs;

namespace TripPlanner.Models.Models.RouteModels
{
    public class Stopover
    {
        public int Id { get; set; }

        public Route Route { get; set; } = null!;
        public int RouteId { get; set; }

        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string Location { get; set; } = string.Empty;
        public int BreakTime { get; set; }


        public static implicit operator StopoverDTO(Stopover data)
        {
            if (data == null)
                return null;

            return new StopoverDTO
            {
                Id = data.Id,
                RouteId = data.RouteId,
                Name = data.Name,
                Description = data.Description,
                Location = data.Location,
                BreakTime = data.BreakTime
            };
        }
    }
}
