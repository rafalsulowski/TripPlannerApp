using CommunityToolkit.Maui.Alerts;
using CommunityToolkit.Maui.Core;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TripPlanner.Models.DTO.UserDTOs;
using TripPlanner.Models.Models;
using TripPlanner.Services;

namespace TripPlanner.ViewModels.User
{
    public partial class LoginViewModel : ObservableObject
    {
        private Configuration m_Configuration;
        private readonly UserService m_UserService;
        private HttpClient m_HttpClient;

        [ObservableProperty]
        string email;

        [ObservableProperty]
        string password;

        public LoginViewModel(Configuration configuration, UserService userService, IHttpClientFactory httpClient)
        {
            m_Configuration = configuration;
            if (m_Configuration.IsLoggedIn) //jesli jestwesmy zalogowaniu ale wlacza sie storna profile
                Shell.Current.GoToAsync("Profile");

            m_HttpClient = httpClient.CreateClient("httpClient");
            m_UserService = userService;

            Email = "rmsulowski@gmail.com";
            password = "Qwerty123*";
        }

        [RelayCommand]
        async Task GoBack()
        {
            await Shell.Current.GoToAsync($"/Start");
        }
        

        [RelayCommand]
        async Task ChangePassword()
        {
            var navigationParameter = new Dictionary<string, object>
            {
                { "passIsFromUserDetailPage",  false}
            };
            await Shell.Current.GoToAsync($"Start/ChangePassword", navigationParameter);
        }

        [RelayCommand]
        async Task GoRegister()
        {
            await Shell.Current.GoToAsync($"/Start/Register");
        }

        [RelayCommand]
        async Task Submit()
        {
            //walidacja
            if(string.IsNullOrEmpty(Email))
            {
                await Shell.Current.CurrentPage.DisplayAlert("Błąd", "Pole e-mail nie może być puste", "Ok");
                return;
            }

            if (string.IsNullOrEmpty(Password))
            {
                await Shell.Current.CurrentPage.DisplayAlert("Błąd", "Pole hasła nie może być puste", "Ok");
                return;
            }
            
            RepositoryResponse<string> response = await m_UserService.Login(Email, Password);
            UserDTO response2 = await m_UserService.GetUserByEmial(Email);

            if(response.Success && response.Data != null && response2 != null)
            {
                m_Configuration.IsLoggedIn = true;
                m_Configuration.User = response2;
                m_HttpClient.DefaultRequestHeaders.Add("Authorization", "Bearer " + response.Data);

                var confirmCopyToast = Toast.Make("Zalogowano", ToastDuration.Long, 14);
                await confirmCopyToast.Show();
            }
            else
            {
                await Shell.Current.CurrentPage.DisplayAlert("Błąd", response.Message, "Ok");
                return;
            }


            var navigationParameter = new Dictionary<string, object>
            {
                { "Reload",  false}
            };
            await Shell.Current.GoToAsync($"///Home", navigationParameter);
        }
    }
}
