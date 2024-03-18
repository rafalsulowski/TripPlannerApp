
using TripPlanner.Models.DTO.CheckListDTOs;
using TripPlanner.Models.DTO.RouteDTOs;
using TripPlanner.Models.DTO.MessageDTOs.QuestionnaireDTOs;
using TripPlanner.Models.Models.CheckListModels;
using TripPlanner.Models.Models.MessageModels.QuestionnaireModels;
using TripPlanner.Models.Models.RouteModels;
using TripPlanner.Models.Models.TourModels;
using TripPlanner.Models.DTO.CultureDTOs;
using TripPlanner.Models.Models.CultureModels;
using TripPlanner.Models.Models.BillModels;
using TripPlanner.Models.DTO.BillDTOs;
using TripPlanner.Models.Models.ScheduleModels;
using TripPlanner.Models.DTO.ScheduleDTOs;
using TripPlanner.Models.DTO.MessageDTOs;
using TripPlanner.Models.Models.MessageModels;

namespace TripPlanner.Models.DTO.TourDTOs
{
    public class TourDTO
    {
        public int Id { get; set; }

        public ICollection<ParticipantTourDTO> Participants { get; set; } = new List<ParticipantTourDTO>();
        public ICollection<CheckListDTO> CheckLists { get; set; } = new List<CheckListDTO>();
        public ICollection<MessageDTO> Messages { get; set; } = new List<MessageDTO>();
        public ICollection<RouteDTO> Routes { get; set; } = new List<RouteDTO>();
        public ICollection<CultureAssistanceDTO> Cultures { get; set; } = new List<CultureAssistanceDTO>();
        public ICollection<ShareDTO> Shares { get; set; } = new List<ShareDTO>();
        public ICollection<ScheduleDayDTO> Schedule { get; set; } = new List<ScheduleDayDTO>();

        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string TargetCountry { get; set; } = string.Empty;
        public string TargetRegion { get; set; } = string.Empty; //okolica, gdzie mniej wiecej jedziemy, np. miasto, kraina geograficzna, województwo
        public int MaxParticipant { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public DateTime CreateDate { get; set; }
        public string InviteLink { get; set; } = string.Empty;
        public string ImagePath { get; set; } = string.Empty;
        public string WeatherCords { get; set; } = string.Empty; //wspolzedne geograficzne dla map np. latitude=52.52&longitude=13.41


        public static implicit operator Tour(TourDTO data)
        {
            if (data == null)
                return null;

            return new Tour
            {
                Id = data.Id,
                Participants = data.Participants.Select(u => (ParticipantTour)u).ToList(),
                CheckLists = data.CheckLists.Select(u => (CheckList)u).ToList(),
                Messages = data.Messages.Select(u => u.MapFromDTO()).ToList(),
                Routes = data.Routes.Select(u => (Route)u).ToList(),
                Cultures = data.Cultures.Select(u => (CultureAssistance)u).ToList(),
                Shares = data.Shares.Select(u => (Share)u).ToList(),
                Schedule = data.Schedule.Select(u => (ScheduleDay)u).ToList(),
                Title = data.Title,
                Description = data.Description,
                TargetCountry = data.TargetCountry,
                MaxParticipant = data.MaxParticipant,
                StartDate = data.StartDate,
                EndDate = data.EndDate,
                CreateDate = data.CreateDate,
                InviteLink = data.InviteLink,
                ImagePath = data.ImagePath,
                TargetRegion = data.TargetRegion,
                WeatherCords = data.WeatherCords,
            };
        }
    }
}
