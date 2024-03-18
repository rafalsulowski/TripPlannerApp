
namespace TripPlanner.Models.DTO.TourDTOs
{
    public class ExtendParticipantDTO
    {
        public int UserId { get; set; }
        public int Order { get; set; }
        public string Email { get; set; } = string.Empty;
        public string Phone { get; set; } = string.Empty;
        public string FullName { get; set; } = string.Empty;
        public string Nickname { get; set; } = string.Empty;
        public string City { get; set; } = string.Empty;
        public bool IsOrganizer { get; set; }
        public DateTime DateOfBirth { get; set; }
        public int Age { get; set; }
    }
}
