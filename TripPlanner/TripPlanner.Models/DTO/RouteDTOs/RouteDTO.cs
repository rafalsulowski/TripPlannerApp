using TripPlanner.Models.Models.RouteModels;

namespace TripPlanner.Models.DTO.RouteDTOs
{
    public class RouteDTO
    {
        public int Id { get; set; }

        public int? TourId { get; set; }
        public int UserId { get; set; }
        public ICollection<StopoverDTO> Stopovers { get; set; } = new List<StopoverDTO>();

        public string Name { get; set; } = string.Empty;
        public string StartLocation { get; set; } = string.Empty;
        public string ArriveLocation { get; set; } = string.Empty;
        public DateTime StartDate { get; set; }
        public DateTime ArriveDate { get; set; }


        public static implicit operator Route(RouteDTO data)
        {
            if (data == null)
                return null;

            return new Route
            {
                Id = data.Id,
                UserId = data.UserId,
                TourId = data.TourId,
                Stopovers = data.Stopovers.Select(u => (Stopover)u).ToList(),
                Name = data.Name,
                StartLocation = data.StartLocation,
                StartDate = data.StartDate,
                ArriveDate = data.ArriveDate,
                ArriveLocation = data.ArriveLocation
            };
        }
    }
}
