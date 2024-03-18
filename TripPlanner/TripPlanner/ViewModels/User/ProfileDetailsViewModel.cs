using CommunityToolkit.Maui.Alerts;
using CommunityToolkit.Maui.Core;
using CommunityToolkit.Maui.Views;
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
using TripPlanner.Views.HomeViews;

namespace TripPlanner.ViewModels.User
{
    public partial class ProfileDetailsViewModel : ObservableObject
    {
        private readonly UserService m_UserService;
        private readonly Configuration m_Configuration;

        [ObservableProperty]
        UserDTO user;

        public ProfileDetailsViewModel(UserService userService, Configuration configuration)
        {
            m_UserService = userService;
            m_Configuration = configuration;

            User = m_Configuration.User;
        }

        [RelayCommand]
        async Task GoBack()
        {
            await Shell.Current.GoToAsync($"///Profile");
        }

        [RelayCommand]
        async Task ChangePassword()
        {
            var navigationParameter = new Dictionary<string, object>
            {
                { "passIsFromUserDetailPage",  true}
            };
            await Shell.Current.GoToAsync($"Start/ChangePassword", navigationParameter);
        }

        [RelayCommand]
        async Task EditEmial()
        {
            string res = (string)await Shell.Current.ShowPopupAsync(new TextEditUserPopups(User.Email));
            if (res == null || string.IsNullOrEmpty(res))
            {
                return;
            }

            CreateUserDTO edit = CreateEditUser();
            edit.Email = res;

            var resp = await m_UserService.UpdateUser(User.Id, edit);
            await CompleteUpdateUser(resp, res);
        }

        [RelayCommand]
        async Task EditName()
        {
            string res = (string)await Shell.Current.ShowPopupAsync(new TextEditUserPopups(User.FullName));
            if (res == null || string.IsNullOrEmpty(res))
            {
                return;
            }

            CreateUserDTO edit = CreateEditUser();
            edit.FullName = res;

            var resp = await m_UserService.UpdateUser(User.Id, edit);
            await CompleteUpdateUser(resp);
        }

        [RelayCommand]
        async Task EditPhone()
        {
            string res = (string)await Shell.Current.ShowPopupAsync(new TextEditUserPopups(User.Phone));
            if (res == null || string.IsNullOrEmpty(res))
            {
                return;
            }

            CreateUserDTO edit = CreateEditUser();
            edit.Phone = res;

            var resp = await m_UserService.UpdateUser(User.Id, edit);
            await CompleteUpdateUser(resp);
        }

        [RelayCommand]
        async Task EditAddress()
        {
            string res = (string)await Shell.Current.ShowPopupAsync(new TextEditUserPopups(User.FullAddress));
            if (res == null || string.IsNullOrEmpty(res))
            {
                return;
            }

            CreateUserDTO edit = CreateEditUser();
            edit.FullAddress = res;

            var resp = await m_UserService.UpdateUser(User.Id, edit);
            await CompleteUpdateUser(resp);
        }

        [RelayCommand]
        async Task EditCity()
        {
            string res = (string)await Shell.Current.ShowPopupAsync(new TextEditUserPopups(User.City));
            if (res == null || string.IsNullOrEmpty(res))
            {
                return;
            }

            CreateUserDTO edit = CreateEditUser();
            edit.City = res;

            var resp = await m_UserService.UpdateUser(User.Id, edit);
            await CompleteUpdateUser(resp);
        }

        [RelayCommand]
        async Task EditBirth()
        {
            string res = (string)await Shell.Current.ShowPopupAsync(new TextEditUserPopups(User.DateOfBirth.ToString("dd:MM:yyyy")));
            if (res == null || string.IsNullOrEmpty(res))
            {
                return;
            }

            List<string> dates = new List<string>();
            dates = res.Split(":").ToList();

            if(dates.Count != 3)
            {
                await Shell.Current.CurrentPage.DisplayAlert("Błąd", "Zły format daty, data powinna byc zapisaan [dzień]:[miesiąc]:[rok] np. 12.09.2003", "Ok");
                return;
            }
            try
            {
                Int32.Parse(res.Split(":")[2]);
                Int32.Parse(res.Split(":")[1]);
                Int32.Parse(res.Split(":")[0]);
            }
            catch(Exception)
            {
                await Shell.Current.CurrentPage.DisplayAlert("Błąd", "Zły format daty, data powinna byc zapisaan [dzień]:[miesiąc]:[rok] np. 12.09.2003", "Ok");
                return;
            }

            DateTime dt = new DateTime(Int32.Parse(res.Split(":")[2]), Int32.Parse(res.Split(":")[1]), Int32.Parse(res.Split(":")[0]));

            CreateUserDTO edit = CreateEditUser();
            edit.DateOfBirth = dt;

            var resp = await m_UserService.UpdateUser(User.Id, edit);
            await CompleteUpdateUser(resp);
        }

        private CreateUserDTO CreateEditUser()
        {
            CreateUserDTO edit = new CreateUserDTO();
            edit.FullAddress = User.FullAddress;
            edit.Email = User.Email;
            edit.IsActivated = true;
            edit.City = User.City;
            edit.DateOfBirth = User.DateOfBirth;
            edit.Phone = User.Phone;
            edit.FullName = User.FullName;
            return edit;
        }

        private async Task CompleteUpdateUser(RepositoryResponse<bool> resp, string newEmail = "")
        {
            if (resp.Success)
            {
                var confirmCopyToast = Toast.Make("Dane zostały zmodyfikowane", ToastDuration.Short, 14);
                await confirmCopyToast.Show();
                
                UserDTO newUserResp = await m_UserService.GetUserByEmial(string.IsNullOrEmpty(newEmail) ? User.Email : newEmail);
                if(newUserResp != null && newUserResp.Id != -1)
                {
                    m_Configuration.User = newUserResp;
                    User = newUserResp;
                }
                else
                    await Shell.Current.CurrentPage.DisplayAlert("Błąd", resp.Message, "Ok");
            }
            else
                await Shell.Current.CurrentPage.DisplayAlert("Błąd", resp.Message, "Ok");
        }
    }
}
