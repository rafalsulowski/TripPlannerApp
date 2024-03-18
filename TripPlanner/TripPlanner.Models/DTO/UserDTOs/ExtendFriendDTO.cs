namespace TripPlanner.Models.DTO.UserDTOs
{
    public class ExtendFriendDTO
    {
        public int UserId { get; set; }
        public int Order { get; set; }
        public string Email { get; set; } = string.Empty;
        public string Phone { get; set; } = string.Empty;
        public string FullName { get; set; } = string.Empty;
        public DateTime DateOfBirth { get; set; }
        public string City { get; set; } = string.Empty;
        public int Age { get; set; }
        public bool IsParticipant { get; set; }
    }
}
