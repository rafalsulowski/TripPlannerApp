using TripPlanner.Models.Models.UserModels;

namespace TripPlanner.Models.DTO.UserDTOs
{
    public class CreateUserDTO
    {
        public string Email { get; set; } = string.Empty;
        public string Phone { get; set; } = string.Empty;
        public string FullName { get; set; } = string.Empty;
        public string PasswordHash { get; set; } = string.Empty;
        public string FullAddress { get; set; } = string.Empty;
        public bool IsActivated { get; set; }
        public string City { get; set; } = string.Empty;
        public DateTime DateOfBirth { get; set; }

        public static implicit operator User(CreateUserDTO User)
        {
            if (User == null)
                return null;

            return new User
            {
                Email = User.Email,
                IsActivated = User.IsActivated,
                Phone = User.Phone,
                FullName = User.FullName,
                PasswordHash = User.PasswordHash,
                FullAddress = User.FullAddress,
                City = User.City,
                DateOfBirth = User.DateOfBirth
            };
        }
    }
}
