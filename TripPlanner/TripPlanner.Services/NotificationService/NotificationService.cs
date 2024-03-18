using Azure;
using Microsoft.Extensions.Configuration;
using System.Linq.Expressions;
using TripPlanner.DataAccess.IRepository;
using TripPlanner.Models.DTO.UserDTOs;
using TripPlanner.Models.Models;
using TripPlanner.Models.Models.TourModels;
using TripPlanner.Models.Models.UserModels;
using TripPlanner.Services.Notificationservice;
using TripPlanner.Services.TourService;

namespace TripPlanner.Services.NotificationService
{
    public class NotificationService : INotificationService
    {
        private readonly INotificationRepository _NotificationRepository;
        private readonly ITourService _TourService;
        public NotificationService(INotificationRepository NotificationRepository, ITourService tourService) 
        {
            _NotificationRepository = NotificationRepository;
            _TourService = tourService;
        }

        public async Task<RepositoryResponse<Notification>> CreateNotification(Notification Notification)
        {
            Notification res = null;
            if (Notification.UserId == -1 && Notification.TourId != -1)
            {
                //dla uczesntikow wycieczki
                var Tour = await _TourService.GetTourAsync(u => u.Id ==  Notification.TourId, "Participants");

                List<Notification> toAdd = new List<Notification>();
                if (Tour.Data != null)
                {
                    foreach (var part in Tour.Data.Participants)
                    {
                        Notification not = new Notification();
                        not.UserId = part.UserId;
                        not.TourId = Notification.TourId;
                        not.IsVisited = Notification.IsVisited;
                        not.CreatedDate = Notification.CreatedDate;
                        not.Name = Notification.Name;
                        not.Message = Notification.Message;
                        not.IconPath = Notification.IconPath;
                        not.Type = Notification.Type;
                        toAdd.Add(not);
                    }
                }
                
                _NotificationRepository.AddRange(toAdd);
                var response = await _NotificationRepository.SaveChangesAsync();
                if (!response.Success)
                    return new RepositoryResponse<Notification> { Data = null, Message = response.Message, Success = false };
                res = toAdd[0];
            }
            else if(Notification.UserId != -1 && Notification.TourId == -1)
            {
                //dla konkretnego adresata
                Notification.TourId = 0;
                res = _NotificationRepository.Add(Notification);

                var response = await _NotificationRepository.SaveChangesAsync();
                if (!response.Success)
                    return new RepositoryResponse<Notification> { Data = null, Message = response.Message, Success = false };
            }
            else
                return new RepositoryResponse<Notification> { Data = null, Message = "Złe dane obiektu CreateNotificationDTO", Success = false };

            return new RepositoryResponse<Notification> { Data = res, Message = "", Success = true };
        }

        public async Task<RepositoryResponse<bool>> DeleteNotification(Notification Notification)
        {
            _NotificationRepository.Remove(Notification);
            var response = await _NotificationRepository.SaveChangesAsync();
            return response;
        }

        public async Task<RepositoryResponse<Notification>> GetNotificationAsync(Expression<Func<Notification, bool>> filter, string? includeProperties = null)
        {
            var response = await _NotificationRepository.GetFirstOrDefault(filter, includeProperties);
            return response;
        }

        public async Task<RepositoryResponse<List<Notification>>> GetNotificationsAsync(Expression<Func<Notification, bool>>? filter = null, string? includeProperties = null)
        {
            var response = await _NotificationRepository.GetAll(filter, includeProperties);
            return response;
        }


        public async Task<RepositoryResponse<bool>> NoticeVisitNotificationOfId(int Bill)
        {
            var response = await _NotificationRepository.GetFirstOrDefault(u => u.Id == Bill);
            if(response.Success && response.Data != null)
            {
                response.Data.IsVisited = true;
                return await UpdateNotification(response.Data);
            }
            else
                return new RepositoryResponse<bool> { Success = false, Data = false, Message = $"Powiadomienie o id = {Bill} nie istanieje" };
        }

        public async Task<RepositoryResponse<bool>> UpdateNotification(Notification Notification)
        {
            var response = await _NotificationRepository.Update(Notification);
            if (response.Success == false)
            {
                return response;
            }
            response = await _NotificationRepository.SaveChangesAsync();
            return response;
        }

    }
}
