using TripPlanner.Models.Models.TourModels;

namespace TripPlanner.Models.DTO.TourDTOs
{
    public class ParticipantTourDTO
    {
        public int UserId { get; set; }
        public int TourId { get; set; }

        public string Nickname { get; set; } = string.Empty;
        public bool IsOrganizer { get; set; }
        public DateTime AccessionDate {  get; set; }


        public static implicit operator ParticipantTour(ParticipantTourDTO data)
        {
            if (data == null)
                return null;

            return new ParticipantTour
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
