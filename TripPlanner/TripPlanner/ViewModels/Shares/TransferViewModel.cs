using CommunityToolkit.Maui.Alerts;
using CommunityToolkit.Maui.Core;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using TripPlanner.Models.DTO.BillDTOs;
using TripPlanner.Services;

namespace TripPlanner.ViewModels.Shares
{
    public partial class TransferViewModel : ObservableObject, IQueryAttributable
    {
        private readonly ShareService m_ShareService;
        private readonly TourService m_TourService;
        private readonly Configuration m_Configuration;
        private int TourId;
        private int TransferId;

        [ObservableProperty]
        TransferPresentationDTO transfer;

        public TransferViewModel(ShareService shareService, Configuration configuration, TourService tourService)
        {
            m_ShareService = shareService;
            m_TourService = tourService;
            m_Configuration = configuration;

            Transfer = new TransferPresentationDTO();
        }

        public async void ApplyQueryAttributes(IDictionary<string, object> query)
        {
            TourId = (int)query["passTourId"];
            TransferId = (int)query["passShareId"];
            await LoadData();
        }

        [RelayCommand]
        async Task GoBack()
        {
            var navigationParameter = new Dictionary<string, object>
            {
                { "passTourId",  TourId}
            };
            await Shell.Current.GoToAsync($"Tour/Shares", navigationParameter);
        }

        [RelayCommand]
        async Task Delete()
        {
            var res = await Shell.Current.CurrentPage.DisplayAlert("Uwaga", "Czy na pewno chcesz usunąć transakcje?", "Tak", "Nie");
            if (!res)
                return;

            var response = await m_ShareService.DeleteTransfer(TransferId);

            if(response.Success)
            {
                var confirmCopyToast = Toast.Make($"Usunięto transakcje", ToastDuration.Short, 14);
                await confirmCopyToast.Show();
                var navigationParameter = new Dictionary<string, object>
                {
                    { "passTourId",  TourId}
                };
                await Shell.Current.GoToAsync($"Tour/Shares", navigationParameter);
            }
            else
                await Shell.Current.CurrentPage.DisplayAlert("Błąd", response.Message, "Ok");
        }

        private async Task LoadData()
        {
            var res = await m_ShareService.GetTransfer(TransferId, TourId);

            if (res.Success)
                Transfer = res.Data;
            else
                await Shell.Current.CurrentPage.DisplayAlert("Błąd", res.Message, "Ok");
        }

    }
}
