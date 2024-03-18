using CommunityToolkit.Maui.Alerts;
using CommunityToolkit.Maui.Core;
using CommunityToolkit.Maui.Core.Extensions;
using CommunityToolkit.Maui.Views;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TripPlanner.Models.DTO.BillDTOs;
using TripPlanner.Models.DTO.TourDTOs;
using TripPlanner.Models.DTO.UserDTOs;
using TripPlanner.Models.Models.BillModels;
using TripPlanner.Models.Models.TourModels;
using TripPlanner.Models.Models.UserModels;
using TripPlanner.Services;
using TripPlanner.Views.HomeViews;

namespace TripPlanner.ViewModels.Friend
{
    public partial class FriendsViewModel : ObservableObject
    {
        private readonly Configuration m_Configuration;
        private readonly UserService m_UserService;
        private FirendDetailsPopups m_FirendDetailsPopups;
        private ObservableCollection<ExtendFriendDTO> FriendsRef;

        [ObservableProperty]
        bool refresh;

        [ObservableProperty]
        string searchPrompt;

        [ObservableProperty]
        bool deleteSearchPromptVisible;

        [ObservableProperty]
        ObservableCollection<ExtendFriendDTO> friends;


        public FriendsViewModel(Configuration configuration, TourService tourService, UserService userService)
        {
            m_Configuration = configuration;
            m_UserService = userService;
            Refresh = false;
            Friends = new ObservableCollection<ExtendFriendDTO>();
            FriendsRef = new ObservableCollection<ExtendFriendDTO>();
            SearchPrompt = "";
            DeleteSearchPromptVisible = false;
        }

        [RelayCommand]
        async void Appearing()
        {
            await LoadData();
        }


        [RelayCommand]
        async Task GoAdd()
        {
            await Shell.Current.GoToAsync("AddFriend");
        }

        [RelayCommand]
        public async Task Searching()
        {
            DeleteSearchPromptVisible = SearchPrompt.Length == 0 ? false : true;
            if (string.IsNullOrEmpty(SearchPrompt))
                Friends = FriendsRef;
            else
                Friends = FriendsRef.Where(i => i.FullName.StartsWith(SearchPrompt, StringComparison.OrdinalIgnoreCase)).ToObservableCollection();
        }

        [RelayCommand]
        public async Task DeleteSearching()
        {
            SearchPrompt = "";
            await Searching();
        }

        [RelayCommand]
        public async Task FriendDetails(ExtendFriendDTO extendParticipantDTO)
        {
            m_FirendDetailsPopups = new FirendDetailsPopups(extendParticipantDTO);
            await Shell.Current.CurrentPage.ShowPopupAsync(m_FirendDetailsPopups);
        }

        [RelayCommand]
        async Task DeleteFriend(string id)
        {
            int userId = Int32.Parse(id);
            var elem = FriendsRef.FirstOrDefault(u => u.UserId == userId);
            string name = elem != null ? elem.FullName : "";

            var result = await Shell.Current.CurrentPage.DisplayAlert("Uwaga", $"Czy na pewno chcesz usunąć {name} z listy znajomych?", "Tak", "Nie");
            if (result)
            {
                var response = await m_UserService.DeleteFriend(userId);
                if (response.Success)
                {
                    await m_FirendDetailsPopups.CloseAsync();
                    var confirmCopyToast = Toast.Make($"Usunięto {name} z listy znajomych", ToastDuration.Short, 14);
                    await confirmCopyToast.Show();
                    await RefreshViewAfterRemove();
                }
                else
                    await Shell.Current.CurrentPage.DisplayAlert("Błąd", response.Message, "Ok");
            }
        }

        [RelayCommand]
        public async Task CopyToClipboard(string phone)
        {
            await Clipboard.Default.SetTextAsync(phone);

            var confirmCopyToast = Toast.Make($"Skopiowano telefon {phone} do schowka", ToastDuration.Short, 14);
            await confirmCopyToast.Show();
        }

        [RelayCommand]
        async Task RefreshViewAfterRemove()
        {
            Refresh = true;
            await LoadData();
            Refresh = false;
        }

        [RelayCommand]
        async Task RefreshView()
        {
            Refresh = true;

            await LoadData();
            var confirmCopyToast = Toast.Make("Odświerzono listę znajomych", ToastDuration.Short, 14);
            await confirmCopyToast.Show();
            Refresh = false;
        }


        private async Task LoadData()
        {
            var value = await m_UserService.GetFriends(m_Configuration.User.Id);

            if (value is null)
            {
                await Shell.Current.CurrentPage.DisplayAlert("Błąd", "Nie udało się pobrać znajomych", "Ok");
            }
            else
            {
                Friends = value.ToObservableCollection();
                FriendsRef = value.ToObservableCollection();
            }
        }
    }
}
