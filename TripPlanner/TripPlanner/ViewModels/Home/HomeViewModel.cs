using CommunityToolkit.Maui.Alerts;
using CommunityToolkit.Maui.Core;
using CommunityToolkit.Maui.Core.Extensions;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Collections.ObjectModel;
using TripPlanner.Models.DTO.MessageDTOs;
using TripPlanner.Models.DTO.TourDTOs;
using TripPlanner.Models.Models.TourModels;
using TripPlanner.Services;

namespace TripPlanner.ViewModels.Home
{
    public partial class HomeViewModel : ObservableObject, IQueryAttributable
    {
        private readonly UserService m_UserService;
        private readonly Configuration m_Configuration;

        [ObservableProperty]
        public ObservableCollection<TourDTO> tours;

        [ObservableProperty]
        bool refresh;

        public HomeViewModel(Configuration configuration, UserService userService)
        {
            m_Configuration = configuration;
            m_UserService = userService;
            Refresh = false; 
        }

        [RelayCommand]
        async void Appearing()
        {
            LoadData();
            //LoadDataTest();
        }
        public async void ApplyQueryAttributes(IDictionary<string, object> query)
        {
            bool b = (bool)query["Reload"];
        }

        [RelayCommand]
        async Task OpenCalendar()
        {
            await Shell.Current.GoToAsync("Calendar");
        }

        [RelayCommand]
        async Task CreateTrip()
        {
            await Shell.Current.GoToAsync("CreateTour");
        }

        [RelayCommand]
        async Task ShowTour(TourDTO tour)
        {
            var navigationParameter = new Dictionary<string, object>
            {
                { "passTourId",  tour.Id}
            };
            await Shell.Current.GoToAsync($"Tour", navigationParameter);
        }

        [RelayCommand]
        async Task ShowNotification()
        {
            await Shell.Current.GoToAsync("Notifications");
        }

        [RelayCommand]
        async Task RefreshView()
        {
            Refresh = true;
            LoadData();

            var confirmCopyToast = Toast.Make("Odświerzono listę wycieczek", ToastDuration.Short, 14);
            await confirmCopyToast.Show();
            Refresh = false;
        }

        private void LoadData()
        {
            var res = m_UserService.GetToursOfUser(m_Configuration.User.Id).Result;
            if (res != null)
                Tours = res.ToObservableCollection<TourDTO>();
        }

        private void LoadDataTest()
        {
            //Tours = new ObservableCollection<TourDTO> { new TourDTO()
            //{
            //    Id = 1,
            //    Title = "asd",
            //    Messages = new List<MessageDTO>
            //    {
            //        new TextMessageDTO{ Content = "asd", Date = DateTime.Now, UserId = 21}
            //    }
            //} }

        }
    }
}
