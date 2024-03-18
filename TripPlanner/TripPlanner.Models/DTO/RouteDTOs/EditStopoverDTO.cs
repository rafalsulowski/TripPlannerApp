using TripPlanner.Models.Models.RouteModels;

namespace TripPlanner.Models.DTO.RouteDTOs
{
    public class EditStopoverDTO
    {
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string Location { get; set; } = string.Empty;
        public int BreakTime { get; set; }


        public static implicit operator Stopover(EditStopoverDTO data)
        {
            if (data == null)
                return null;

            return new Stopover
            {
                Name = data.Name,
                Description = data.Description,
                Location = data.Location,
                BreakTime = data.BreakTime
            };
        }
    }
}
