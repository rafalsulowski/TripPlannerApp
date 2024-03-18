using TripPlanner.Models.Models.RouteModels;

namespace TripPlanner.Models.DTO.RouteDTOs
{
    public class EditRouteDTO
    {
        public string Name { get; set; } = string.Empty;
        public string StartLocation { get; set; } = string.Empty;
        public string ArriveLocation { get; set; } = string.Empty;
        public DateTime StartDate { get; set; }
        public DateTime ArriveDate { get; set; }


        public static implicit operator Route(EditRouteDTO data)
        {
            if (data == null)
                return null;

            return new Route
            {
                Name = data.Name,
                StartLocation = data.StartLocation,
                StartDate = data.StartDate,
                ArriveDate = data.ArriveDate,
                ArriveLocation = data.ArriveLocation
            };
        }
    }
}
