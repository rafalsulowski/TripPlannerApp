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
using TripPlanner.Models.Models.BillModels;
using TripPlanner.Models.Models.CheckListModels;
using TripPlanner.Models.Models.TourModels;
using TripPlanner.Services;

namespace TripPlanner.ViewModels.User
{
    public partial class ChangePasswordViewModel : ObservableObject, IQueryAttributable
    {
        private HttpClient m_HttpClient;
        private Configuration m_Configuration;
        private readonly UserService m_UserService;
        private bool IsFromUserDetailPage;

        [ObservableProperty]
        private bool emailVisivle;

        [ObservableProperty]
        bool enable;

        [ObservableProperty]
        string password;

        [ObservableProperty]
        string password2;

        [ObservableProperty]
        string email;


        public ChangePasswordViewModel(Configuration configuration, UserService userService, IHttpClientFactory httpClient)
        {
            m_Configuration = configuration;
            m_UserService = userService;
            IsFromUserDetailPage = false;
            m_HttpClient = httpClient.CreateClient("httpClient");
            Enable = true;
            IsFromUserDetailPage = false;
            EmailVisivle = true;
        }

        public async void ApplyQueryAttributes(IDictionary<string, object> query)
        {
            IsFromUserDetailPage = (bool)query["passIsFromUserDetailPage"];
            EmailVisivle = !IsFromUserDetailPage;
        }

        [RelayCommand]
        async Task GoBack()
        {
            if(IsFromUserDetailPage)
                await Shell.Current.GoToAsync($"Profile/Details");
            else
                await Shell.Current.GoToAsync($"Start/Login");
        }

        [RelayCommand]
        async Task Submit()
        {
            Enable = false;

            
            if (EmailVisivle && string.IsNullOrEmpty(Email))
            {
                await Shell.Current.CurrentPage.DisplayAlert("Błąd", "Musisz podać email należący do konta", "Ok");
                return;
            }
            if (string.IsNullOrEmpty(Password))
            {
                await Shell.Current.CurrentPage.DisplayAlert("Błąd", "Hasło powinno posiadać co najmniej 8 znaków, jedną literę dużą, jedną małą, jedną cyfrę oraz znak specjalny", "Ok");
                return;
            }

            if (string.IsNullOrEmpty(Password2))
            {
                await Shell.Current.CurrentPage.DisplayAlert("Błąd", "Hasło powinno posiadać co najmniej 8 znaków, jedną literę dużą, jedną małą, jedną cyfrę oraz znak specjalny", "Ok");
                return;
            }

            if (Password != Password2)
            {
                await Shell.Current.CurrentPage.DisplayAlert("Błąd", "Hasła są różne", "Ok");
                return;
            }

            if (Password.Length < 8)
            {
                await Shell.Current.CurrentPage.DisplayAlert("Błąd", "Hasło powinno posiadać co najmniej 8 znaków, jedną literę dużą, jedną małą, jedną cyfrę oraz znak specjalny", "Ok");
                return;
            }

            if (Password != Password2)
            {
                await Shell.Current.CurrentPage.DisplayAlert("Błąd", "Hasła są różne", "Ok");
                return;
            }

            if(IsFromUserDetailPage)
            {
                var resp = await m_UserService.ChangePassword(m_Configuration.User.Id, Password);

                if(resp.Success)
                {
                    m_HttpClient.DefaultRequestHeaders.Remove("Authorization"); //mozliwe wylogowanie
                    m_Configuration.User = new UserDTO { Id = -1 };
                    m_Configuration.IsLoggedIn = false;
                }
                else
                {
                    await Shell.Current.CurrentPage.DisplayAlert("Błąd", resp.Message, "Ok");
                    return;
                }
            }
            else
            {
                var resp = await m_UserService.GetUserByEmial(Email);
                if(resp == null)
                {
                    await Shell.Current.CurrentPage.DisplayAlert("Błąd", "Nie ma konta z takim emailem", "Ok");
                    return;
                }
                var resp2 = await m_UserService.ChangePassword(resp.Id, Password);
                if(!resp2.Success)
                {
                    await Shell.Current.CurrentPage.DisplayAlert("Błąd", resp2.Message, "Ok");
                    return;
                }
            }

            //strona aby powiadomic uzytkownika aby sprawdzil swoj email
            var navigationParameter = new Dictionary<string, object>
            {
                { "passNote",  "Potwierdź zmianę hasła klikając w link który został wysłany w wiadomości na twój e-mail"},
            };
            await Shell.Current.GoToAsync($"/Start/CheckEmail", navigationParameter);
        }
    }
}

