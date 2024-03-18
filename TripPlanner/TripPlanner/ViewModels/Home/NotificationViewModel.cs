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

namespace TripPlanner.ViewModels.Home
{
    public partial class NotificationViewModel : ObservableObject
    {
        private HubConnection m_Connection;
        private readonly Configuration m_Configuration;

        [ObservableProperty]
        ObservableCollection<NotificationDTO> notifications;

        public NotificationViewModel(Configuration configuration)
        {
            m_Configuration = configuration;
            Notifications = new ObservableCollection<NotificationDTO>();
            Connect();
        }

        [RelayCommand]
        async void Appearing()
        {
            await GetAllNotification();
        }

        async Task Connect()
        {
            try
            {
                m_Connection = new HubConnectionBuilder()
                .WithUrl(m_Configuration.WssNotificationUrl)
                .Build();

                m_Connection.On<string>("GetAllNotificationReceived", (message) =>
                {
                    List<NotificationDTO> n = JsonConvert.DeserializeObject<List<NotificationDTO>>(message);
                    Notifications = n.ToObservableCollection();
                    Notifications = Notifications.Reverse().ToObservableCollection();
                });

                m_Connection.On<string>("NotificationReceived", (message) =>
                {
                    NotificationDTO n = JsonConvert.DeserializeObject<NotificationDTO>(message);
                    Notifications.Insert(0, n);
                });


                m_Connection.On<string>("SetConnection", (message) =>
                {
                    List<NotificationDTO> n = JsonConvert.DeserializeObject<List<NotificationDTO>>(message);
                    Notifications = n.ToObservableCollection();
                    Notifications = Notifications.Reverse().ToObservableCollection();
                });


                await m_Connection.StartAsync();
                string json2 = JsonConvert.SerializeObject(m_Configuration.User);
                await m_Connection.InvokeCoreAsync("SetConnection", args: new[] { json2 });
            }
            catch (HubException)
            {
                await Shell.Current.CurrentPage.DisplayAlert("Błąd", $"Nie udało wykonać operacji", "Ok");
            }
            catch (Exception)
            {
                await Shell.Current.CurrentPage.DisplayAlert("Błąd", $"Nieznany błąd", "Ok");
            }
        }

        [RelayCommand]
        async Task GoToNotification(NotificationDTO not)
        {
            await SetVisitTrueNotifyOfId(not.Id);

            var navigationParameter = new Dictionary<string, object>
            {
                { "passNotificationId",  not.Id}
            };
            await Shell.Current.GoToAsync($"Notifications/NotificationDetails", navigationParameter);
        }

        public async Task GetAllNotification()
        {
            try
            {
                await m_Connection.InvokeCoreAsync("GetAllNotification", args: new[] { m_Configuration.User.Id.ToString() });
            }
            catch (Exception)
            {
                await Shell.Current.CurrentPage.DisplayAlert("Błąd", "Nie udało się pobrać listy powiadomień!", "Ok");
            }
        }

        public async Task SendNotifyToUsersOfTour(CreateNotificationDTO notify)
        {
            try
            {
                string json2 = JsonConvert.SerializeObject(notify);
                await m_Connection.InvokeCoreAsync("SendNotifyToUsersOfTour", args: new[] { json2 });
            }
            catch (Exception)
            {
                await Shell.Current.CurrentPage.DisplayAlert("Błąd", "Nie udało się wysłać powiadomienia!", "Ok");
            }
        }

        public async Task SendNotifyOnlyToUserOfId(CreateNotificationDTO notify)
        {
            try
            {
                string json2 = JsonConvert.SerializeObject(notify);
                await m_Connection.InvokeCoreAsync("SendNotifyOnlyToUserOfId", args: new[] { json2 });
            }
            catch (Exception)
            {
                await Shell.Current.CurrentPage.DisplayAlert("Błąd", "Nie udało się wysłać powiadomienia!", "Ok");
            }
        }

        public async Task SendNotifyToUserOfIdAndMe(CreateNotificationDTO notify)
        {
            try
            {
                string json2 = JsonConvert.SerializeObject(notify);
                await m_Connection.InvokeCoreAsync("SendNotifyToUserOfIdAndMe", args: new[] { json2 });
            }
            catch (Exception)
            {
                await Shell.Current.CurrentPage.DisplayAlert("Błąd", "Nie udało się wysłać powiadomienia!", "Ok");
            }
        }

        public async Task SetVisitTrueNotifyOfId(int id)
        {
            try
            {
                await m_Connection.InvokeCoreAsync("SetVisitTrueNotifyOfId", args: new[] { id.ToString() });
            }
            catch (Exception)
            {
                await Shell.Current.CurrentPage.DisplayAlert("Błąd", "Nie udało się wysłać powiadomienia!", "Ok");
            }
        }
    }
}
