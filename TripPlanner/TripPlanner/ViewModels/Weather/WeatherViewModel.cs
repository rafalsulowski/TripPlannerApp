using CommunityToolkit.Maui.Alerts;
using CommunityToolkit.Maui.Core;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using TripPlanner.Services;

namespace TripPlanner.ViewModels.Weather
{
    public partial class WeatherViewModel : ObservableObject, IQueryAttributable
    {
        private readonly Configuration m_Configuration;
        private readonly WeatherFastService m_WeatherFastService;
        private readonly TourService m_TourService;
        private int TourId;

        [ObservableProperty]
        bool isOrganizer;

        [ObservableProperty]
        WeatherModel weather;

        public WeatherViewModel(Configuration configuration, WeatherFastService weatherFastService, TourService tourService)
        {
            m_TourService = tourService;
            m_Configuration = configuration;
            m_WeatherFastService = weatherFastService;
            Weather = new WeatherModel();
            IsOrganizer = false;
        }

        public async void ApplyQueryAttributes(IDictionary<string, object> query)
        {
            TourId = (int)query["passTourId"];
            await LoadData();
        }

        [RelayCommand]
        async Task GoBack()
        {
            var navigationParameter = new Dictionary<string, object>
            {
                { "passTourId",  TourId}
            };
            await Shell.Current.GoToAsync($"/Tour", navigationParameter);
        }

        [RelayCommand]
        public async Task EditLocation()
        {
            if(!IsOrganizer)
            {
                await Shell.Current.DisplayAlert("Uwaga", "Nie masz uprawnień do wykonania tej akcji", "Ok");
                return;
            }

            var res = await Shell.Current.DisplayPromptAsync("Wprowadź nową lokalizacje", "Miejscowości np. Zakopane, Split, Paryż", "Ok", "Anuluj");

            if(!string.IsNullOrEmpty(res))
            {
                var response = await m_TourService.UpdateWeatherCords(TourId, res);
                if(response.Success)
                {
                    var confirmCopyToast = Toast.Make($"Zmieniono lokalizację pogodny na {res}", ToastDuration.Long, 14);
                    await confirmCopyToast.Show();
                    await LoadWeatherData();
                }
                else
                    await Shell.Current.DisplayAlert("Błąd", "Nie udało się zmienić lokalizacji pogody", "Ok");
            }
            else
                await Shell.Current.DisplayAlert("Błąd", "Lokalizacja nie może być pusta", "Ok");
        }

        private async Task LoadData()
        {
            await LoadUserData();
            await LoadWeatherData();
        }

        private async Task LoadUserData()
        {
            var resultUser = await m_TourService.GetTourExtendParticipantById(TourId, m_Configuration.User.Id);
            if (resultUser != null)
                IsOrganizer = resultUser.IsOrganizer;
        }

        private async Task LoadWeatherData()
        {
            var result = await m_WeatherFastService.GetWeatherOn14Days(TourId);
            Weather = result.Data;
            if(!result.Success)
            {
                Weather = new WeatherModel();
                Weather.Location = "Błędna lokalizacja";
                await Shell.Current.CurrentPage.DisplayAlert("Błąd", "Nie udało się pobrać pogody, sprawdź swoje połączenie z internetem lub ustaw poprawną lokalizację", "Ok");
            }
        }
    }
}
