using TripPlanner.Models.Models.CultureModels;

namespace TripPlanner.Models.DTO.CultureDTOs
{
    public class CultureAssistanceDTO
    {
        public int TourId { get; set; }
        public int CultureId { get; set; }
        public bool IsPrincipal { get; set; }


        public static implicit operator CultureAssistance(CultureAssistanceDTO data)
        {
            if (data == null)
                return null;

            return new CultureAssistance
            {
                TourId = data.TourId,
                CultureId = data.CultureId,
                IsPrincipal = data.IsPrincipal
            };
        }
    }
}
