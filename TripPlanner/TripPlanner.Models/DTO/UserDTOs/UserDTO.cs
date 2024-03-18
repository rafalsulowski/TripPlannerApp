using TripPlanner.Models.DTO.BillDTOs;
using TripPlanner.Models.DTO.CheckListDTOs;
using TripPlanner.Models.DTO.MessageDTOs;
using TripPlanner.Models.DTO.MessageDTOs.QuestionnaireDTOs;
using TripPlanner.Models.DTO.RouteDTOs;
using TripPlanner.Models.DTO.TourDTOs;
using TripPlanner.Models.Models.BillModels;
using TripPlanner.Models.Models.CheckListModels;
using TripPlanner.Models.Models.MessageModels;
using TripPlanner.Models.Models.MessageModels.QuestionnaireModels;
using TripPlanner.Models.Models.RouteModels;
using TripPlanner.Models.Models.TourModels;
using TripPlanner.Models.Models.UserModels;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace TripPlanner.Models.DTO.UserDTOs
{
    public class UserDTO
    {
        public int Id { get; set; }

        public ICollection<NotificationDTO> Notifications { get; set; } = new List<NotificationDTO>();
        public ICollection<CheckListDTO> CheckLists { get; set; } = new List<CheckListDTO>();
        public ICollection<ParticipantTourDTO> ParticipantTours { get; set; } = new List<ParticipantTourDTO>();
        public ICollection<QuestionnaireVoteDTO> QuestionnaireVotes { get; set; } = new List<QuestionnaireVoteDTO>();
        public ICollection<MessageDTO> Messages { get; set; } = new List<MessageDTO>();
        public ICollection<RouteDTO> Routes { get; set; } = new List<RouteDTO>();
        public ICollection<BillContributorDTO> BillContributors { get; set; } = new List<BillContributorDTO>();
        public ICollection<BillDTO> BillsPayed { get; set; } = new List<BillDTO>();
        public ICollection<TransferDTO> TransfersSender { get; set; } = new List<TransferDTO>();
        public ICollection<TransferDTO> TransfersRecipient { get; set; } = new List<TransferDTO>();
        public ICollection<ShareDTO> Shares { get; set; } = new List<ShareDTO>();

        public string Email { get; set; } = string.Empty;
        public string FullName { get; set; } = string.Empty;
        public string Phone { get; set; } = string.Empty;
        public string FullAddress { get; set; } = string.Empty;
        public string City { get; set; } = string.Empty;
        public bool IsActivated { get; set; }
        public DateTime DateOfBirth { get; set; }


        public static implicit operator User(UserDTO User)
        {
            if (User == null)
                return null;

            return new User
            {
                Id = User.Id,
                Notifications = User.Notifications.Select(u => (Notification)u).ToList(),
                CheckLists = User.CheckLists.Select(u => (CheckList)u).ToList(),
                ParticipantTours = User.ParticipantTours.Select(u => (ParticipantTour)u).ToList(),
                QuestionnaireVotes = User.QuestionnaireVotes.Select(u => (QuestionnaireVote)u).ToList(),
                Messages = User.Messages.Select(u => u.MapFromDTO()).ToList(),
                Routes = User.Routes.Select(u => (Route)u).ToList(),
                BillContributors = User.BillContributors.Select(u => (BillContributor)u).ToList(),
                BillsPayed = User.BillsPayed.Select(u => (Bill)u).ToList(),
                TransfersSender = User.TransfersSender.Select(u => (Transfer)u).ToList(),
                TransfersRecipient = User.TransfersRecipient.Select(u => (Transfer)u).ToList(),
                Shares = User.Shares.Select(u => (Share)u).ToList(),
                Email = User.Email,
                Phone = User.Phone,
                IsActivated = User.IsActivated,
                FullName = User.FullName,
                FullAddress = User.FullAddress,
                City = User.City,
                DateOfBirth = User.DateOfBirth,
            };
        }
    }
}
