using CommunityToolkit.Maui.Alerts;
using CommunityToolkit.Maui.Core;
using CommunityToolkit.Maui.Core.Extensions;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using TripPlanner.Models.DTO.UserDTOs;
using TripPlanner.Models.Models.UserModels;
using TripPlanner.Services;
using TripPlanner.ViewModels.Home;

namespace TripPlanner.ViewModels.Friend
{
    public partial class AddFriendViewModel : ObservableObject
    {
        private readonly Configuration m_Configuration;
        private readonly NotificationViewModel m_NotificationViewModel;
        private readonly UserService m_UserService;
        private ObservableCollection<ExtendUserDTO> UsersRef;

        [ObservableProperty]
        int selection;

        [ObservableProperty]
        string searchPrompt;

        [ObservableProperty]
        bool deleteSearchPromptVisible;

        [ObservableProperty]
        ObservableCollection<ExtendUserDTO> users;

        public AddFriendViewModel(Configuration configuration, UserService userService, NotificationViewModel notificationViewModel)
        {
            m_Configuration = configuration;
            m_UserService = userService;
            m_NotificationViewModel = notificationViewModel;
            Selection = 0;
            Users = new ObservableCollection<ExtendUserDTO>();
            UsersRef = new ObservableCollection<ExtendUserDTO>();
            SearchPrompt = "";
            DeleteSearchPromptVisible = false;
            LoadData();
        }

        [RelayCommand]
        async Task GoBack()
        {
            await Shell.Current.GoToAsync("///Friends");
        }

        [RelayCommand]
        async Task Add(ExtendUserDTO user)
        {
            var result = await Shell.Current.CurrentPage.DisplayAlert("Uwaga", $"Czy na pewno chcesz dodać {user.FullName} do znajomych?", "Tak", "Nie");
            if (result)
            {
                FriendDTO friend = new FriendDTO();
                friend.Friend1Id = m_Configuration.User.Id;
                friend.Friend2Id = user.Id;

                var response = await m_UserService.AddFriend(friend);
                if (response.Success)
                {
                    //powiadomienie
                    try
                    {
                        CreateNotificationDTO notificationDTO = new CreateNotificationDTO();
                        notificationDTO.UserId = user.Id; //adresat
                        notificationDTO.OtherUsers = new List<int> { m_Configuration.User.Id }; //dodatkowo utworzo powaidomienie dal zalogowanego uzytkownika
                        notificationDTO.TourId = -1;
                        notificationDTO.AddNotifyToParticipantsOfTour = false;
                        notificationDTO.IsVisited = false;
                        notificationDTO.CreatedDate = DateTime.Now;
                        notificationDTO.Name = $"Nowy znajomy";
                        notificationDTO.Message = $"Nawiązano nową znajomość pomiędzy \"{m_Configuration.User.FullName}\" a \"{user.FullName}\"";
                        notificationDTO.IconPath = "bell_sec.png";
                        notificationDTO.Type = NotificationType.NewFriendAlert;
                        await m_NotificationViewModel.SendNotifyToUserOfIdAndMe(notificationDTO);
                    }
                    catch (Exception) { }

                    //logika
                    var confirmCopyToast = Toast.Make($"Dodano {user.FullName} do znajomych", ToastDuration.Short, 14);
                    await confirmCopyToast.Show();
                    
                    var elem2 = UsersRef.FirstOrDefault(u => u.Id == user.Id);
                    elem2.IsFriend = true;
                    await Searching();
                }
                else
                    await Shell.Current.CurrentPage.DisplayAlert("Błąd", response.Message, "Ok");
            }
        }

        [RelayCommand]
        async Task Searching()
        {
            DeleteSearchPromptVisible = SearchPrompt.Length == 0 ? false : true;
            switch(Selection)
            {
                case 0:
                    Users = UsersRef.Where(i => i.FullName.StartsWith(SearchPrompt, StringComparison.OrdinalIgnoreCase)).ToObservableCollection();
                    break;
                case 1:
                    Users = UsersRef.Where(i => i.Phone.StartsWith(SearchPrompt, StringComparison.OrdinalIgnoreCase)).ToObservableCollection();
                    break;
                case 2:
                    Users = UsersRef.Where(i => i.Email.StartsWith(SearchPrompt, StringComparison.OrdinalIgnoreCase)).ToObservableCollection();
                    break;
                default:
                    break;
            }
        }

        [RelayCommand]
        public async Task DeleteSearching()
        {
            SearchPrompt = "";
            await Searching();
        }


        [RelayCommand]
        async Task UserByNameSearching()
        {
            Selection = 0;
            Users = UsersRef.Where(i => i.FullName.StartsWith(SearchPrompt, StringComparison.OrdinalIgnoreCase)).ToObservableCollection();
        }

        [RelayCommand]
        async Task UserByEmialSearching()
        {
            Selection = 2;
            Users = UsersRef.Where(i => i.Email.StartsWith(SearchPrompt, StringComparison.OrdinalIgnoreCase)).ToObservableCollection();
        }

        [RelayCommand]
        async Task UserByPhoneSearching()
        {
            Selection = 1;
            Users = UsersRef.Where(i => i.Phone.StartsWith(SearchPrompt, StringComparison.OrdinalIgnoreCase)).ToObservableCollection();
        }

        [RelayCommand]
        async Task DeleteFriend(ExtendUserDTO user)
        {
            var result = await Shell.Current.CurrentPage.DisplayAlert("Uwaga", $"Czy na pewno chcesz usunąć {user.FullName} z listy znajomych?", "Tak", "Nie");
            if (result)
            {
                var response = await m_UserService.DeleteFriend(user.Id);
                if (response.Success)
                {
                    var confirmCopyToast = Toast.Make($"Usunięto {user.FullName} z listy znajomych", ToastDuration.Short, 14);
                    await confirmCopyToast.Show();
                    var elem2 = UsersRef.FirstOrDefault(u => u.Id == user.Id);
                    elem2.IsFriend = false;
                    await Searching();
                }
                else
                    await Shell.Current.CurrentPage.DisplayAlert("Błąd", response.Message, "Ok");
            }
        }

        private async void LoadData()
        {
            var value = m_UserService.GetUsers(m_Configuration.User.Id).Result;

            if (value is null)
            {
                await Shell.Current.CurrentPage.DisplayAlert("Błąd", "Nie udało się pobrać listy znajomych", "Ok");
            }
            else
            {
                Users = value.ToObservableCollection();
                UsersRef = value.ToObservableCollection();
            }
        }
    }
}
