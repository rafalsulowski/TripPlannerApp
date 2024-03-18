using CommunityToolkit.Maui.Alerts;
using CommunityToolkit.Maui.Core;
using CommunityToolkit.Maui.Core.Extensions;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.AspNetCore.SignalR;
using Microsoft.AspNetCore.SignalR.Client;
using Newtonsoft.Json;
using System.Collections.ObjectModel;
using TripPlanner.Models.DTO.UserDTOs;
using TripPlanner.Models.Models.TourModels;
using TripPlanner.Models.Models.UserModels;
using TripPlanner.Services;

namespace TripPlanner.ViewModels.Home
{
    public partial class NotificationDetailsViewModel : ObservableObject, IQueryAttributable
    {
        private readonly Configuration m_Configuration;
        private readonly NotificationService m_NotificationService;
        private int NotificationId;


        [ObservableProperty]
        bool isVisible;

        [ObservableProperty]
        NotificationDTO notification;

        public NotificationDetailsViewModel(Configuration configuration, NotificationService notificationService)
        {
            m_Configuration = configuration;
            m_NotificationService = notificationService;
            Notification = new NotificationDTO();
            IsVisible = false;
        }

        public async void ApplyQueryAttributes(IDictionary<string, object> query)
        {
            NotificationId = (int)query["passNotificationId"];
            await LoadData();
        }


        [RelayCommand]
        async Task GoBack()
        {
            await Shell.Current.GoToAsync("/Notifications");
        }

        [RelayCommand]
        async Task GoAction()
        {
            switch(Notification.Type)
            {
                case NotificationType.RemindToPayAlert:
                    var navigationParameter = new Dictionary<string, object>
                    {
                        { "passTourId",  Notification.TourId}
                    };
                    await Shell.Current.GoToAsync($"Tour/Shares/Balance", navigationParameter);
                    break;

                case NotificationType.NotifyMessageAddedAlert:
                    navigationParameter = new Dictionary<string, object>
                    {
                        { "passTourId",  Notification.TourId}
                    };
                    await Shell.Current.GoToAsync($"Tour/Chat", navigationParameter);
                    break;

                case NotificationType.QuestionnaireMessageAddedAlert:
                    navigationParameter = new Dictionary<string, object>
                    {
                        { "passTourId",  Notification.TourId}
                    };
                    await Shell.Current.GoToAsync($"Tour/Chat", navigationParameter);
                    break;

                case NotificationType.BillAddedAlert:
                    navigationParameter = new Dictionary<string, object>
                    {
                        { "passTourId",  Notification.TourId}
                    };
                    await Shell.Current.GoToAsync($"Tour/Shares", navigationParameter);
                    break;

                case NotificationType.TransferAddedAlert:
                    navigationParameter = new Dictionary<string, object>
                    {
                        { "passTourId",  Notification.TourId}
                    };
                    await Shell.Current.GoToAsync($"Tour/Shares", navigationParameter);
                    break;

                case NotificationType.AddedNewParticipantAlert:
                    navigationParameter = new Dictionary<string, object>
                    {
                        { "passTourId",  Notification.TourId}
                    };
                    await Shell.Current.GoToAsync($"Tour/Participants", navigationParameter);
                    break;

                case NotificationType.MakeNewOrganizerAlert:
                    navigationParameter = new Dictionary<string, object>
                    {
                        { "passTourId",  Notification.TourId}
                    };
                    await Shell.Current.GoToAsync($"Tour/Participants", navigationParameter);
                    break;

                case NotificationType.CheckListAddedAlert:
                    navigationParameter = new Dictionary<string, object>
                    {
                        { "passTourId",  Notification.TourId}
                    };
                    await Shell.Current.GoToAsync($"Tour/CheckLists", navigationParameter);
                    break;

                case NotificationType.NewFriendAlert:
                    await Shell.Current.GoToAsync("Friends");
                    break;

                case NotificationType.OtherImportantAlert:
                    navigationParameter = new Dictionary<string, object>
                    {
                        { "Reload", false }
                    };
                    await Shell.Current.GoToAsync("Home", navigationParameter);
                    break;

                case NotificationType.OtherRedundantAlert:
                    navigationParameter = new Dictionary<string, object>
                    {
                        { "Reload", false }
                    };
                    await Shell.Current.GoToAsync("Home", navigationParameter);
                    break;

                default:
                    break;
            }
        }

        private async Task LoadData()
        {
            var res = await m_NotificationService.GetNotificationById(NotificationId);
            if (res != null)
                Notification = res;
        }
    }
}
