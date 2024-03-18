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
using Plugin.Media;
using Plugin.Media.Abstractions;
using CommunityToolkit.Maui.Views;
using TripPlanner.Views.StartViews;
//using ImageCropper.Maui;

namespace TripPlanner.ViewModels.User
{
    public partial class RegisterViewModel : ObservableObject
    {
        private Configuration m_Configuration;
        private readonly UserService m_UserService;

        [ObservableProperty]
        string email;

        [ObservableProperty]
        string fullName;

        [ObservableProperty]
        string password;

        [ObservableProperty]
        string password2;

        [ObservableProperty]
        string userImage;

        [ObservableProperty]
        string phone;

        [ObservableProperty]
        string aAA;

        public RegisterViewModel(Configuration configuration, UserService userService)
        {
            m_Configuration = configuration;
            m_UserService = userService;
            UserImage = "person_add_sec.png";
            aAA = "a";
        }
        

        [RelayCommand]
        async Task GoBack()
        {
            await Shell.Current.GoToAsync($"/Start");
        }

        [RelayCommand]
        async Task GoLogin()
        {
            await Shell.Current.GoToAsync($"/Start/Login");
        }


        [RelayCommand]
        async Task AddImage()
        {
            int res = (int)await Shell.Current.CurrentPage.ShowPopupAsync(new SelectImageSourcePopups());

            if(res == 1) //do picture
            {
                var options = new StoreCameraMediaOptions { CompressionQuality = 100 };
                var result = await CrossMedia.Current.TakePhotoAsync(options);

                var fileInfo = new FileInfo(result?.Path);
                var fileLength = fileInfo.Length;
                AAA = $"Image size: {fileLength / 1000} kB";

                //new ImageCropper()
                //{
                //    Success = (imageFile) =>
                //    {
                //        Dispatcher.Dispatch(() =>
                //        {
                //            imageView.Source = ImageSource.FromFile(imageFile);
                //        });
                //    }
                //}.Show(this);

            }
            else //select image from gallery
            {

            }


            //rejestracja uzytkownika ze zdjeciem
        }


            [RelayCommand]
        async Task Submit()
        {
            //walidacja
            if (string.IsNullOrEmpty(Email))
            {
                await Shell.Current.CurrentPage.DisplayAlert("Błąd", "E-mail nie może być pusty", "Ok");
                return;
            }

            if (string.IsNullOrEmpty(Phone))
            {
                await Shell.Current.CurrentPage.DisplayAlert("Błąd", "Musisz podać aktualny numer telefonu", "Ok");
                return;
            }

            if (Phone.Length < 9 || Phone.Length > 14)
            {
                await Shell.Current.CurrentPage.DisplayAlert("Błąd", "Podany numer telefonu jest zbyt krótki lub długi", "Ok");
                return;
            }

            if (string.IsNullOrEmpty(FullName))
            {
                await Shell.Current.CurrentPage.DisplayAlert("Błąd", "Musisz podać imię i nazwisko", "Ok");
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

            bool emailFree = await m_UserService.EmailIsFree(Email);

            if (!emailFree)
            {
                await Shell.Current.CurrentPage.DisplayAlert("Błąd", "E-mail jest już przypisany do innego konta", "Ok");
                return;
            }

            CreateUserDTO createUserDTO = new CreateUserDTO
            {
                Email = Email,
                FullName = FullName,
                PasswordHash = Password,
                Phone = Phone
            };

            RepositoryResponse<bool> response = await m_UserService.Register(createUserDTO);

            if(!response.Success)
            {
                await Shell.Current.CurrentPage.DisplayAlert("Błąd", response.Message, "Ok");
                return;
            }

            //strona aby sprawdzic swoj email
            var navigationParameter = new Dictionary<string, object>
                {
                    { "passNote",  "Na twój e-mail został wysłana wiadomość z linkiem do aktywacji konta"},
                };
            await Shell.Current.GoToAsync($"/Start/CheckEmail", navigationParameter);
        }
    }
}

