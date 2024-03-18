using CommunityToolkit.Maui.Alerts;
using CommunityToolkit.Maui.Core;
using CommunityToolkit.Maui.Core.Extensions;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TripPlanner.Models.DTO.BillDTOs;
using TripPlanner.Models.DTO.TourDTOs;
using TripPlanner.Models.Models;
using TripPlanner.Models.Models.BillModels;
using TripPlanner.Services;

namespace TripPlanner.ViewModels.Shares
{
    public partial class CreateTransferSelectViewModel : ObservableObject, IQueryAttributable
    {
        private readonly ShareService m_ShareService;
        private readonly TourService m_TourService;
        private readonly Configuration m_Configuration;
        private int TourId;
        private bool IsSelectRecipientMode;
        private bool IsFromBalancePage;
        
        [ObservableProperty]
        CreateTransferDTO transfer;

        [ObservableProperty]
        ObservableCollection<OtherUser> participantsToSettle;

        [ObservableProperty]
        ObservableCollection<ExtendParticipantDTO> allParticipants;

        [ObservableProperty]
        bool isAllParticipantMode;

        [ObservableProperty]
        bool isSettleParticipantMode;

        [ObservableProperty]
        string modeLabel;

        [ObservableProperty]
        bool isVisibleChangeMode;

        [ObservableProperty]
        string headerLabel;
        public CreateTransferSelectViewModel(ShareService shareService, Configuration configuration, TourService tourService)
        {
            m_ShareService = shareService;
            m_TourService = tourService;
            m_Configuration = configuration;

            HeaderLabel = "Wybierz uczestnika";
            ModeLabel = "Inni uczestnicy";
            IsAllParticipantMode = false;
            IsSettleParticipantMode = true;
            IsVisibleChangeMode = true;
            IsFromBalancePage = false;
            IsSelectRecipientMode = false;
            ParticipantsToSettle = new ObservableCollection<OtherUser>();
            AllParticipants = new ObservableCollection<ExtendParticipantDTO>();
            Transfer = new CreateTransferDTO
            {
                SenderId = m_Configuration.User.Id,
                RecipientId = m_Configuration.User.Id,
                TourId = TourId,
                CreatedDate = new DateTime(2023, 12, 29),
                Value = new decimal(256.89),
                Name = "Zakupy biedronka 29.12.2023r.",
                Description = "alko + jedzenie na pierwsyz dzień dla wszyskich",
                CreatorId = m_Configuration.User.Id,
                ImageFilePath = "",
            };
        }

        public async void ApplyQueryAttributes(IDictionary<string, object> query)
        {
            TourId = (int)query["passTourId"];
            var transfer = (CreateTransferDTO)query["passTransfer"];
            IsSelectRecipientMode = (bool)query["SelectRecipient"];
            IsAllParticipantMode = (bool)query["IsAllParticipantMode"];
            IsFromBalancePage = (bool)query["IsFromBalancePage"];
            await LoadData();

            if (transfer != null)
                Transfer = transfer;

            if (IsAllParticipantMode)
                IsSettleParticipantMode = false;

            if (IsSelectRecipientMode)
            {
                if (transfer != null)
                {
                    int id = transfer.SenderId;
                    var elem = AllParticipants.FirstOrDefault(u => u.UserId == id);
                    if(elem != null)
                        AllParticipants.Remove(elem);
                }

                HeaderLabel = "Wybierz odbiorce";
                IsVisibleChangeMode = false;
                IsSettleParticipantMode = false;
                IsAllParticipantMode = true;
            }

        }

        [RelayCommand]
        async Task ChangeMode()
        {
            if(IsAllParticipantMode)
            {
                IsAllParticipantMode = false;
                IsSettleParticipantMode = true;
                ModeLabel = "Inni uczestnicy";
            }
            else if(IsSettleParticipantMode)
            {
                IsAllParticipantMode = true;
                IsSettleParticipantMode = false;
                ModeLabel = "Uczestnicy do rozliczenia";
            }
        }

        [RelayCommand]
        async Task Exit()
        {
            if(IsSelectRecipientMode)
            {   //goback
                var navigationParameter = new Dictionary<string, object>
                {
                    { "passTourId",  TourId},
                    { "passTransfer",  Transfer},
                    { "SelectRecipient",  false},
                    { "IsAllParticipantMode",  true},
                    { "IsFromBalancePage",  false},
                };
                await Shell.Current.GoToAsync($"Tour/Shares/CreateTransferSelect", navigationParameter);
            }
            else
            {
                var navigationParameter = new Dictionary<string, object>
                {
                    { "passTourId",  TourId}
                };
                await Shell.Current.GoToAsync($"Tour/Shares", navigationParameter);
            }
        }

        [RelayCommand]
        async Task GoNextOtherUser(OtherUser ot)
        {
            Transfer.Value = ot.Saldo;
            if(ot.Saldo < 0)
                Transfer.RecipientId = ot.UserId;
            else
                Transfer.SenderId = ot.UserId;
            var navigationParameter = new Dictionary<string, object>
            {
                { "passTourId",  TourId},
                { "passTransfer",  Transfer},
                { "SelectRecipient",  false},
                { "IsFromBalancePage",  false},
            };
            await Shell.Current.GoToAsync($"Tour/Shares/CreateTransferSelect/CreateTransferSubmit", navigationParameter);
        }

        [RelayCommand]
        async Task GoSelectRecipient(ExtendParticipantDTO particpant)
        {
            if(IsSelectRecipientMode)
            {
                Transfer.RecipientId = particpant.UserId;
                var navigationParameter = new Dictionary<string, object>
                {
                    { "passTourId",  TourId},
                    { "passTransfer",  Transfer},
                    { "SelectRecipient",  true},
                    { "IsFromBalancePage",  false},
                };
                await Shell.Current.GoToAsync($"Tour/Shares/CreateTransferSelect/CreateTransferSubmit", navigationParameter);
            }
            else
            {
                Transfer.SenderId = particpant.UserId;
                var navigationParameter = new Dictionary<string, object>
                {
                    { "passTourId",  TourId},
                    { "passTransfer",  Transfer},
                    { "SelectRecipient",  true},
                    { "IsAllParticipantMode",  true},
                    { "IsFromBalancePage",  false},
                };
                await Shell.Current.GoToAsync($"Tour/Shares/CreateTransferSelect", navigationParameter);
            }            
        }

        private async Task LoadData()
        {
            var result = await m_TourService.GetTourExtendParticipant(TourId);
            var result2 = await m_ShareService.GetBalanceOfUser(m_Configuration.User.Id, TourId);

            if(result2.Success)
            {
                ParticipantsToSettle = result2.Data.BalanceWithOtherUsers.ToObservableCollection();
            }

            if (result.Any())
            {
                AllParticipants = result.ToObservableCollection();
            }
        }
    }
}
