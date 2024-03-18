using Microsoft.AspNetCore.SignalR;
using TripPlanner.Models.DTO.MessageDTOs;
using Newtonsoft.Json;
using TripPlanner.Models.Models.MessageModels;
using TripPlanner.Services.TourService;
using TripPlanner.Services.UserService;
using TripPlanner.Services.ChatService;
using TripPlanner.Services.QuestionnaireService;
using TripPlanner.Models.Models.MessageModels.QuestionnaireModels;
using TripPlanner.Models.DTO.MessageDTOs.QuestionnaireDTOs;
using TripPlanner.Models.DTO.TourDTOs;
using System.Collections.Immutable;
using TripPlanner.Services.CheckListService;
using TripPlanner.Models.Models.CheckListModels;
using TripPlanner.Models.DTO.CheckListDTOs;
using TripPlanner.Services.Notificationservice;
using TripPlanner.Models.DTO.UserDTOs;
using TripPlanner.Models.Models.UserModels;
using TripPlanner.Models.Models;

namespace TripPlanner.WebAPI.Hubs
{
    public class NotificationHub : Hub
    {
        private readonly INotificationService _NotificationService;
        private readonly IUserService _UserService;
        private readonly ITourService _TourService;
        public NotificationHub(INotificationService checkListService, IUserService userService, ITourService tourService)
        {
            _NotificationService = checkListService;
            _UserService = userService;
            _TourService = tourService;
        }

        public Task JoinGroup(string groupName)
        {
            return Groups.AddToGroupAsync(Context.ConnectionId, groupName);
        }

        public Task LeaveGroup(string groupName)
        {
            return Groups.RemoveFromGroupAsync(Context.ConnectionId, groupName);
        }

        public async Task SetVisitTrueNotifyOfId(string json) //wyslanie do adresata powiadomienia oraz do wolajacego funkcje
        {
            int userId = int.Parse(json);
            var res = await _NotificationService.NoticeVisitNotificationOfId(userId);
        }

        public async Task SendNotifyToUserOfIdAndMe(string json) //wyslanie do adresata powiadomienia oraz do wolajacego funkcje
        {
            CreateNotificationDTO notify = JsonConvert.DeserializeObject<CreateNotificationDTO>(json);
            if (notify == null)
                throw new HubException($"Nie udało się deserializować informacji");


            notify.Name += GetNotificationTitleName(notify.TourId);
            notify.IconPath = GetNotificationIconPath(notify.Type);
            var res = await AddNotify(notify);

            if (res.Success && res.Data != null)
            {
                string json2 = JsonConvert.SerializeObject((NotificationDTO)res.Data);

                //wysalnie do wołającego oraz adresata powiadomienia
                await Clients.Caller.SendAsync("NotificationReceived", json2);
                await Clients.Group("User" + notify.UserId.ToString()).SendAsync("NotificationReceived", json2);
            }
            else
                throw new HubException(res.Message);
        }

        public async Task SendNotifyOnlyToUserOfId(string json) //wyslanie do usera adresata powiadomienia
        {
            CreateNotificationDTO notify = JsonConvert.DeserializeObject<CreateNotificationDTO>(json);
            if (notify == null)
                throw new HubException($"Nie udało się deserializować informacji");


            notify.Name += GetNotificationTitleName(notify.TourId);
            notify.IconPath = GetNotificationIconPath(notify.Type);
            var res = await AddNotify(notify);

            if (res.Success && res.Data != null)
            {
                string json2 = JsonConvert.SerializeObject((NotificationDTO)res.Data);

                //wysalnie do adresata powiadomienia
                await Clients.Group("User" + notify.UserId.ToString()).SendAsync("NotificationReceived", json2);
            }
            else
                throw new HubException(res.Message);
        }

        public async Task SendNotifyToUsersOfTour(string json) //wyslanie do wszystkich uczestnikow wycieczki o danym id
        {
            CreateNotificationDTO notify = JsonConvert.DeserializeObject<CreateNotificationDTO>(json);
            if (notify == null)
                throw new HubException($"Nie udało się deserializować informacji");

            notify.Name += GetNotificationTitleName(notify.TourId);
            notify.IconPath = GetNotificationIconPath(notify.Type);
            var res = await AddNotify(notify);

            if (res.Success && res.Data != null)
            {
                string json2 = JsonConvert.SerializeObject((NotificationDTO)res.Data);

                //wysalnie do wszystkich uczestnikow wycieczki o danym id
                await Clients.Group("Tour" + notify.TourId.ToString()).SendAsync("NotificationReceived", json2);
            }
            else
                throw new HubException(res.Message);
        }


        public async Task GetAllNotification(string id) //zapytanie o pobranie powaidomien dla uzytkownika o danym id
        {
            int userId = int.Parse(id);

            var res = await _NotificationService.GetNotificationsAsync(u => u.UserId == userId);
            List<NotificationDTO> notifications = new List<NotificationDTO>();
            if (res.Success && res.Data != null)
                notifications = res.Data.Select(u => (NotificationDTO)u).ToList();
            else
                throw new Exception("Błąd pobrania powiadomień");

            string json2 = JsonConvert.SerializeObject(notifications);
            await Clients.Caller.SendAsync("GetAllNotificationReceived", json2);
        }


        private string GetNotificationTitleName(int id)
        {
            if (id != -1)
            {
                var resp = _TourService.GetTourAsync(u => u.Id == id).Result;
                return resp.Data != null ? resp.Data.Title : "";
            }
            return "";
        }

        private async Task<RepositoryResponse<Notification>> AddNotify(CreateNotificationDTO notificationDTO)
        {
            var resp = new RepositoryResponse<Notification> { Success = true };

            int tourId = notificationDTO.TourId;
            if (!notificationDTO.AddNotifyToParticipantsOfTour)
            { //tourId bylo tylko potrzebne do uzupelnienia daty, usuwamy tourId zeby inni uczestnicy wycieczki nie dostali powiadomienia
                notificationDTO.TourId = -1;

                if (notificationDTO.UserId != -1)
                    resp = await _NotificationService.CreateNotification(notificationDTO);

                if (!resp.Success)
                    throw new Exception("Błąd zapisu powaidomienia!");

            }
            else
            {
                //tourId jest po to aby oprocz dodania nazwy tytulu do nazwy powiadomienia to dodatkowo stworzyc powiadomienia tez dla pozostalcyh uczestnikow
                resp = await _NotificationService.CreateNotification(notificationDTO);

                if (!resp.Success)
                    throw new Exception("Błąd zapisu powaidomienia!");
            }


            foreach (int i in notificationDTO.OtherUsers)
            {
                Notification not = new Notification();
                not.UserId = i;
                not.TourId = notificationDTO.TourId;
                not.IsVisited = notificationDTO.IsVisited;
                not.CreatedDate = notificationDTO.CreatedDate;
                not.Name = notificationDTO.Name;
                not.Message = notificationDTO.Message;
                not.IconPath = notificationDTO.IconPath;
                not.Type = notificationDTO.Type;

                resp = await _NotificationService.CreateNotification(not);
                if (!resp.Success)
                    throw new Exception("Błąd zapisu powaidomienia!");
            }

            return resp;
        }


        private string GetNotificationIconPath(NotificationType type)
        {
            switch (type)
            {
                case NotificationType.NotifyMessageAddedAlert:
                    return "exclamation_sec.png";
                case NotificationType.QuestionnaireMessageAddedAlert:
                    return "checklist_sec.png";
                case NotificationType.BillAddedAlert:
                    return "bill_sec.png";
                case NotificationType.TransferAddedAlert:
                    return "money_coin_sec.png";
                case NotificationType.RemindToPayAlert:
                    return "money_coin_sec.png";
                case NotificationType.AddedNewParticipantAlert:
                    return "person_chat_sec.png";
                case NotificationType.MakeNewOrganizerAlert:
                    return "organizer_sec.png";
                case NotificationType.CheckListAddedAlert:
                    return "checklist_sec.png";
                case NotificationType.NewFriendAlert:
                    return "person_chat_sec.png";
                case NotificationType.OtherImportantAlert:
                    return "exclamation_sec.png";
                case NotificationType.OtherRedundantAlert:
                    return "exclamation_sec.png";
                default:
                    return "exclamation_sec.png";
            }
        }

        public async Task SetConnection(string json)
        {
            UserDTO user = JsonConvert.DeserializeObject<UserDTO>(json);
            if (user == null)
                throw new HubException($"Nie udało się deserializować informacji");

            var userWithParticipants = await _UserService.GetUserAsync(u => u.Id == user.Id, "ParticipantTours");
            if (!userWithParticipants.Success || userWithParticipants.Data == null)
                throw new Exception("Błąd pobrania danych użytkownika");

            var res = await _NotificationService.GetNotificationsAsync(u => u.UserId == user.Id);
            List<NotificationDTO> notifications = new List<NotificationDTO>();
            if (res.Success && res.Data != null)
                notifications = res.Data.Select(u => (NotificationDTO)u).ToList();
            else
                throw new Exception("Błąd pobrania powiadomień");

            //dodanie uzytkownika do grupy User + swoje Id oraz do grup wyjazdów do jakich należy
            foreach (var participant in userWithParticipants.Data.ParticipantTours)
                await Groups.AddToGroupAsync(Context.ConnectionId, "Tour" + participant.TourId.ToString());
            await Groups.AddToGroupAsync(Context.ConnectionId, "User" + user.Id.ToString());

            string json2 = JsonConvert.SerializeObject(notifications);
            await Clients.Caller.SendAsync("SetConnection", json2);
        }

    }
}
