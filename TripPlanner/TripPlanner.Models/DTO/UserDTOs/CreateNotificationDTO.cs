using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TripPlanner.Models.Models.TourModels;
using TripPlanner.Models.Models.UserModels;

namespace TripPlanner.Models.DTO.UserDTOs
{
    public class CreateNotificationDTO
    {
        public int UserId { get; set; } //adresat powiadomienia
        public List<int> OtherUsers { get; set; } = new List<int>(); //lista uzytkownikow dla kotrych DODATKOWO wytorzyc powaidomienie
        public int TourId { get; set; } //identyfikator wycieczki dla ktorego wytworzone zostana powiadomienia dla uczestnikow
        public bool AddNotifyToParticipantsOfTour { get; set; } //identyfikator wycieczki dla ktorego wytworzone zostana powiadomienia dla uczestnikow
        public NotificationType Type { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Message { get; set; } = string.Empty;
        public DateTime CreatedDate { get; set; }
        public string IconPath { get; set; } = string.Empty;
        public bool IsVisited { get; set; }


        public static implicit operator Notification(CreateNotificationDTO data)
        {
            if (data == null)
                return null;

            return new Notification
            {
                TourId = data.TourId,
                UserId = data.UserId,
                CreatedDate = data.CreatedDate,
                IconPath = data.IconPath,
                Message = data.Message,
                Name = data.Name,
                IsVisited = data.IsVisited,
                Type = data.Type,
            };
        }
    }
}
