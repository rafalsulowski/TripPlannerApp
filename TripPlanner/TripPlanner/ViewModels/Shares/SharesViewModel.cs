using CommunityToolkit.Maui.Alerts;
using CommunityToolkit.Maui.Core;
using CommunityToolkit.Maui.Core.Extensions;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Collections.ObjectModel;
using TripPlanner.Models.DTO.BillDTOs;
using TripPlanner.Models.Models.BillModels;
using TripPlanner.Services;

namespace TripPlanner.ViewModels.Shares
{
    public partial class SharesViewModel : ObservableObject, IQueryAttributable
    {
        private readonly Configuration m_Configuration;
        private readonly ShareService m_ShareService;
        private ObservableCollection<SharePresentationDTO> SharesRef;
        private int TourId;

        [ObservableProperty]
        bool refresh;

        [ObservableProperty]
        string searchPrompt;

        [ObservableProperty]
        bool deleteSearchPromptVisible;

        [ObservableProperty]
        ObservableCollection<SharePresentationDTO> shares;

        public SharesViewModel(Configuration configuration, ShareService shareService)
        {
            m_Configuration = configuration;
            m_ShareService = shareService;
            Shares = new ObservableCollection<SharePresentationDTO>();
            SharesRef = new ObservableCollection<SharePresentationDTO>();
            SearchPrompt = "";
            DeleteSearchPromptVisible = false;
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
        async Task GoBalance()
        {
            var navigationParameter = new Dictionary<string, object>
            {
                { "passTourId",  TourId}
            };
            await Shell.Current.GoToAsync($"Tour/Shares/Balance", navigationParameter);
        }

        [RelayCommand]
        async Task GoSettle()
        {
            var navigationParameter = new Dictionary<string, object>
            {
                { "passTourId",  TourId},
                { "passTransfer",  null},
                { "SelectRecipient",  false},
                { "IsAllParticipantMode",  false},
                { "IsFromBalancePage",  false },
            };
            await Shell.Current.GoToAsync($"Tour/Shares/CreateTransferSelect", navigationParameter);
        }

        [RelayCommand]
        async Task AddBill()
        {
            var navigationParameter = new Dictionary<string, object>
            {
                { "passTourId",  TourId},
                { "passBill",  null},
                { "passSplitBillView",  null},
                { "passSplitBillViewAccept",  true},
                { "passIsEditing",  false}
            };
            await Shell.Current.GoToAsync($"Tour/Shares/CreateBill", navigationParameter);
        }

        [RelayCommand]
        async Task GoShare(SharePresentationDTO share)
        {
            var navigationParameter = new Dictionary<string, object>
            {
                { "passTourId",  TourId},
                { "passShareId",  share.Id}
            };

            if (share.Type == SharePresentationDTO.ShareType.Bill)
            {
                await Shell.Current.GoToAsync($"Tour/Shares/Bill", navigationParameter);
            }
            else if (share.Type == SharePresentationDTO.ShareType.Transfer)
            {
                await Shell.Current.GoToAsync($"Tour/Shares/Transfer", navigationParameter);
            }
        }

        [RelayCommand]
        async Task Export()
        {
            var confirmCopyToast = Toast.Make("Funkcjonalność niezaimplementowana!", ToastDuration.Long, 14);
            await confirmCopyToast.Show();
        }

        [RelayCommand]
        public async Task Searching()
        {
            DeleteSearchPromptVisible = SearchPrompt.Length == 0 ? false : true;
            if (string.IsNullOrEmpty(SearchPrompt))
                Shares = SharesRef;
            else
                Shares = SharesRef.Where(i => i.Name.StartsWith(SearchPrompt, StringComparison.OrdinalIgnoreCase)).ToObservableCollection();
        }

        [RelayCommand]
        public async Task DeleteSearching()
        {
            SearchPrompt = "";
            await Searching();
        }

        [RelayCommand]
        async Task RefreshView()
        {
            Refresh = true;
            await LoadData();

            var confirmCopyToast = Toast.Make("Odświerzono listę rachunków", ToastDuration.Short, 14);
            await confirmCopyToast.Show();
            Refresh = false;
        }

        private async Task LoadData()
        {
            var result = await m_ShareService.GetSharesByTourId(TourId);

            if (!result.Success)
            {
                await Shell.Current.CurrentPage.DisplayAlert("Błąd", result.Message, "Ok");
            }
            else
            {
                result.Data.Reverse();
                Shares = result.Data.ToObservableCollection();
                SharesRef = result.Data.ToObservableCollection();
            }
        }
    }
}
