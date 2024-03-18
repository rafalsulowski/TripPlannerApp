using CommunityToolkit.Maui.Alerts;
using CommunityToolkit.Maui.Core;
using CommunityToolkit.Maui.Views;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using TripPlanner.Models.DTO.UserDTOs;
using TripPlanner.Services;
using TripPlanner.Views.HomeViews;
using TripPlanner.Views.StartViews;

namespace TripPlanner.ViewModels.User
{
    public partial class ProfileViewModel : ObservableObject
    {
        private readonly UserService m_UserService;
        private readonly Configuration m_Configuration;
        private HttpClient m_HttpClient;

        [ObservableProperty]
        UserDTO user;

        public ProfileViewModel(UserService userService, Configuration configuration, IHttpClientFactory httpClient)
        {
            m_HttpClient = httpClient.CreateClient("httpClient");
            m_UserService = userService;
            m_Configuration = configuration;

            user = m_Configuration.User;
        }

        [RelayCommand]
        async Task Logout()
        {
            m_HttpClient.DefaultRequestHeaders.Remove("Authorization");
            m_Configuration.User = new UserDTO { Id = -1 };
            m_Configuration.IsLoggedIn = false;

            var confirmCopyToast = Toast.Make("Wylogowano", ToastDuration.Long, 14);
            await confirmCopyToast.Show();

            await Shell.Current.GoToAsync("Start");
        }

        [RelayCommand]
        async Task GoSettings()
        {
            await Shell.Current.GoToAsync("/Settings");
        }

        [RelayCommand]
        async Task ScanyQrCode()
        {
            var confirmCopyToast = Toast.Make("Funkcjonalność niezaimplementowana", ToastDuration.Long, 14);
            await confirmCopyToast.Show();
        }

        [RelayCommand]
        async Task GoCheckList()
        {
            var confirmCopyToast = Toast.Make("Funkcjonalność niezaimplementowana", ToastDuration.Long, 14);
            await confirmCopyToast.Show();
        }

        [RelayCommand]
        async Task GoRoutes()
        {
            var confirmCopyToast = Toast.Make("Funkcjonalność niezaimplementowana", ToastDuration.Long, 14);
            await confirmCopyToast.Show();
        }

        [RelayCommand]
        async Task ShowUserData()
        {
            var res = await Shell.Current.ShowPopupAsync(new ComfirmUserPopups(m_UserService, m_Configuration.User));

            if(res != null && (bool)res == true)
                await Shell.Current.GoToAsync($"/Details");
        }
    }
}
