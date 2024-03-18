using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TripPlanner.Models.DTO.UserDTOs;

namespace TripPlanner.Models.Models.UserModels
{
    public enum NotificationType
    {
        NotifyMessageAddedAlert = 0,       // ok   podczas dodawania wiadomość typu Ważna
        QuestionnaireMessageAddedAlert,    // ok   podczas dodawania nakiety
        BillAddedAlert,                    // ok   podczas dodawania rachunku
        TransferAddedAlert,                // ok   podczas dodawania
        RemindToPayAlert,                  // ok   powiadomienie dla kogos kto jest winny pieniadze
        AddedNewParticipantAlert,          // ok   podczas dodawania nowego uczestnika wycieczki
        MakeNewOrganizerAlert,             // ok   podczas dodawania nowego uczestnika wycieczki
        CheckListAddedAlert,               // ok   podczas dodawania checklisty
        NewFriendAlert,                    // ok   podczas dodawania nowego znajomego
                                                
        OtherImportantAlert,               // ok   powiadomienie typu Ważne dotyczy nieokreślonego zdarzenia
        OtherRedundantAlert,               // ok   powiadomienie typu niewazne dotyczy nieokreślonego zdarzenia
    }

    public class Notification
    {
        public int Id { get; set; }

        public int UserId { get; set; } //adresat gdy tourId == -1 lub nadawca gdy tourId != -1 
        public User User { get; set; } = null!;

        public int TourId { get; set; }
        public NotificationType Type { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Message { get; set; } = string.Empty;
        public DateTime CreatedDate { get; set; }
        public string IconPath { get; set; } = string.Empty;
        public bool IsVisited { get; set; }


        public static implicit operator NotificationDTO(Notification data)
        {
            if (data == null)
                return null;

            return new NotificationDTO
            {
                Id = data.Id,
                TourId = data.TourId,
                CreatedDate = data.CreatedDate,
                IconPath = data.IconPath,
                Message = data.Message,
                Name = data.Name,
                Type = data.Type,
                IsVisited = data.IsVisited,
            };
        }
    }
}
