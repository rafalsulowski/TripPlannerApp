using TripPlanner.Models.DTO.TourDTOs;
using TripPlanner.Models.Models.UserModels;

namespace TripPlanner.Models.Models.TourModels
{
    public class ParticipantTour
    {
        public User User { get; set; } = null!;
        public int UserId { get; set; }

        public Tour Tour { get; set; } = null!;
        public int TourId { get; set; }

        public string Nickname { get; set; } = string.Empty;
        public bool IsOrganizer { get; set; }
        public DateTime AccessionDate { get; set; }


        public static implicit operator ParticipantTourDTO(ParticipantTour data)
        {
            if (data == null)
                return null;

            return new ParticipantTourDTO
            {
                UserId = data.UserId,
                TourId = data.TourId,
                Nickname = data.Nickname,
                IsOrganizer = data.IsOrganizer,
                AccessionDate = data.AccessionDate,
            };
        }
    }
}
