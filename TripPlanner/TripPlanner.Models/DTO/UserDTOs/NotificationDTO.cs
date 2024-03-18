using CommunityToolkit.Mvvm.ComponentModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TripPlanner.Models.Models.UserModels;

namespace TripPlanner.Models.DTO.UserDTOs
{
    public class NotificationDTO : ObservableObject
    {
        public int Id { get; set; }
        public int UserId { get; set; }

        public int TourId { get; set; }
        public NotificationType Type { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Message { get; set; } = string.Empty;
        public DateTime CreatedDate { get; set; }
        public string IconPath { get; set; } = string.Empty;

        bool isVisited;
        public bool IsVisited
        {
            get => isVisited;
            set => SetProperty(ref isVisited, value);
        }


        public static implicit operator Notification(NotificationDTO data)
        {
            if (data == null)
                return null;

            return new Notification
            {
                Id = data.Id,
                TourId = data.TourId,
                UserId = data.UserId,
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
