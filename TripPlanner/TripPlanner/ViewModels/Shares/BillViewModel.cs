using CommunityToolkit.Maui.Alerts;
using CommunityToolkit.Maui.Core;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TripPlanner.Models.DTO.BillDTOs;
using TripPlanner.Services;

namespace TripPlanner.ViewModels.Shares
{
    public partial class BillViewModel : ObservableObject, IQueryAttributable
    {
        private readonly ShareService m_ShareService;
        private readonly TourService m_TourService;
        private readonly Configuration m_Configuration;
        private int TourId;
        private int BillId;

        [ObservableProperty]
        BillPresentationDTO bill;

        [ObservableProperty]
        string divisionName;

        public BillViewModel(ShareService shareService, Configuration configuration, TourService tourService)
        {
            m_ShareService = shareService;
            m_TourService = tourService;
            m_Configuration = configuration;

            Bill = new BillPresentationDTO();
        }

        public async void ApplyQueryAttributes(IDictionary<string, object> query)
        {
            TourId = (int)query["passTourId"];
            BillId = (int)query["passShareId"];
            await LoadData();
            DivisionName = m_Configuration.GetBillTypeName(Bill.BillType);
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
            var confirmCopyToast = Toast.Make($"Usunięto rachunek", ToastDuration.Short, 14);
            await confirmCopyToast.Show();
            var navigationParameter = new Dictionary<string, object>
            {
                { "passTourId",  TourId}
            };
            await Shell.Current.GoToAsync($"Tour/Shares", navigationParameter);
        }

        [RelayCommand]
        async Task Update()
        {
            SplitBillView split = CreateBillViewModel.CreateSplitView(Bill);

            var navigationParameter = new Dictionary<string, object>
            {
                { "passTourId",  TourId},
                { "passBill",  Bill},
                { "passSplitBillView",  split},
                { "passSplitBillViewAccept",  false},
                { "passIsEditing",  true}
            };
            await Shell.Current.GoToAsync($"Tour/Shares", navigationParameter);
        }

        private async Task LoadData()
        {
            var res = await m_ShareService.GetBill(BillId, TourId);

            if (res.Success)
                Bill = res.Data;
            else
                await Shell.Current.CurrentPage.DisplayAlert("Błąd", res.Message, "Ok");
        }

    }
}
