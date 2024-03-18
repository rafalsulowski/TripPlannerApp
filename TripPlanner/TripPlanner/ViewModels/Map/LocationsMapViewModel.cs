using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.AspNetCore.SignalR;
using Microsoft.AspNetCore.SignalR.Client;
using TripPlanner.Services;

namespace TripPlanner.ViewModels.Map
{
    public partial class LocationsMapViewModel : ObservableObject, IQueryAttributable
    {
        private readonly TourService m_TourService;
        private readonly Configuration m_Configuration;
        private HubConnection m_Connection;
        private int TourId;
     
        [ObservableProperty]
        bool isOrganizer;

        public LocationsMapViewModel(Configuration configuration, CheckListService checkListService, TourService tourService)
        {
            m_Configuration = configuration;
            m_TourService = tourService;
            IsOrganizer = false;
        }

        public async void ApplyQueryAttributes(IDictionary<string, object> query)
        {
            TourId = (int)query["passTourId"];
            //await Connect();
        }

        async Task Connect()
        {
            var res = await m_TourService.GetTourExtendParticipantById(TourId, m_Configuration.User.Id);
            if (res is null)
            {
                await Shell.Current.CurrentPage.DisplayAlert("Błąd", "Nie jesteś już uczestnikiem tej wycieczki", "Ok");
                await GoBack();
            }
            else
                IsOrganizer = res.IsOrganizer;

            try
            {
                m_Connection = new HubConnectionBuilder()
                .WithUrl(m_Configuration.WssMapsUrl)
                .Build();

                m_Connection.On<string>("GetUpdate", (message) =>
                {
                });

                m_Connection.On<string>("SetConnection", (message) =>
                {
                });

                await m_Connection.StartAsync();
                await m_Connection.InvokeCoreAsync("SetConnection", args: new[] { TourId.ToString() });
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

        void GoBackSynch()
        {
            GoBack();
        }

        [RelayCommand]
        async Task GoBack()
        {
            //await m_Connection.InvokeCoreAsync("LeaveGroup", args: new[] { TourId.ToString() });

            var navigationParameter = new Dictionary<string, object>
            {
                { "passTourId",  TourId}
            };
            await Shell.Current.GoToAsync($"Tour", navigationParameter);
        }
    }
}
