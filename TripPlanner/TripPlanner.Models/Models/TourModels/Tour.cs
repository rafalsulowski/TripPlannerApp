using TripPlanner.Models.DTO.BillDTOs;
using TripPlanner.Models.DTO.CheckListDTOs;
using TripPlanner.Models.DTO.CultureDTOs;
using TripPlanner.Models.DTO.MessageDTOs;
using TripPlanner.Models.DTO.MessageDTOs.QuestionnaireDTOs;
using TripPlanner.Models.DTO.RouteDTOs;
using TripPlanner.Models.DTO.ScheduleDTOs;
using TripPlanner.Models.DTO.TourDTOs;
using TripPlanner.Models.Models.BillModels;
using TripPlanner.Models.Models.CheckListModels;
using TripPlanner.Models.Models.CultureModels;
using TripPlanner.Models.Models.MessageModels;
using TripPlanner.Models.Models.MessageModels.QuestionnaireModels;
using TripPlanner.Models.Models.RouteModels;
using TripPlanner.Models.Models.ScheduleModels;

namespace TripPlanner.Models.Models.TourModels
{
    public class Tour
    {
        public int Id { get; set; }

        public ICollection<ParticipantTour> Participants { get; set; } = new List<ParticipantTour>();
        public ICollection<CheckList> CheckLists { get; set; } = new List<CheckList>();
        public ICollection<Message> Messages { get; set; } = new List<Message>();
        public ICollection<Route> Routes { get; set; } = new List<Route>();
        public ICollection<CultureAssistance> Cultures { get; set; } = new List<CultureAssistance>();
        public ICollection<Share> Shares { get; set; } = new List<Share>();
        public ICollection<ScheduleDay> Schedule { get; set; } = new List<ScheduleDay>();

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
        public string WeatherCords { get; set; } = string.Empty; //wspolzedne geograficzne dla map np. latitude=52.52&longitude=13.41, lub: "Białka Tatrzanska, Polska"


        public static implicit operator TourDTO(Tour data)
        {
            if (data == null)
                return null;

            return new TourDTO
            {
                Id = data.Id,
                Participants = data.Participants.Select(u => (ParticipantTourDTO)u).ToList(),
                CheckLists = data.CheckLists.Select(u => (CheckListDTO)u).ToList(),
                Messages = data.Messages.Select(u => u.MapToDTO()).ToList(),
                Routes = data.Routes.Select(u => (RouteDTO)u).ToList(),
                Cultures = data.Cultures.Select(u => (CultureAssistanceDTO)u).ToList(),
                Shares = data.Shares.Select(u => (ShareDTO)u).ToList(),
                Schedule = data.Schedule.Select(u => (ScheduleDayDTO)u).ToList(),
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
