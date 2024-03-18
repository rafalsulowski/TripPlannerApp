using CommunityToolkit.Maui.Alerts;
using CommunityToolkit.Maui.Core;
using CommunityToolkit.Maui.Views;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TripPlanner.Models.DTO.BillDTOs;
using TripPlanner.Models.DTO.UserDTOs;
using TripPlanner.Models.Models;
using TripPlanner.Models.Models.BillModels;
using TripPlanner.Models.Models.UserModels;
using TripPlanner.Services;
using TripPlanner.ViewModels.Home;
using TripPlanner.Views.ShareViews;

namespace TripPlanner.ViewModels.Shares
{
    public partial class BalanceViewModel : ObservableObject, IQueryAttributable
    {
        private readonly Configuration m_Configuration;
        private readonly ShareService m_ShareService;
        private readonly UserService m_UserService;
        private readonly TourService m_TourService;
        private readonly NotificationViewModel m_NotificationViewModel;
        private int TourId;

        [ObservableProperty]
        Balance balance;

        [ObservableProperty]
        bool refresh;

        public BalanceViewModel(Configuration configuration, ShareService shareService, TourService tourService, NotificationViewModel notificationViewModel, UserService userService)
        {
            m_Configuration = configuration;
            m_ShareService = shareService;
            m_TourService = tourService;
            m_UserService = userService;
            m_NotificationViewModel = notificationViewModel;
            Refresh = false;
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
            await Shell.Current.GoToAsync($"/Tour/Shares", navigationParameter);
        }

        [RelayCommand]
        async Task GoSettleUp(OtherUser ou)
        {
            CreateTransferDTO transfer = new CreateTransferDTO();
            transfer.Value = Math.Abs(ou.Saldo);
            if(ou.Saldo > 0)
            {
                transfer.SenderId = ou.UserId;
                transfer.RecipientId = ou.UserBalanceId;
            }
            else
            {
                transfer.SenderId = ou.UserBalanceId;
                transfer.RecipientId = ou.UserId;
            }

            var navigationParameter = new Dictionary<string, object>
            {
                { "passTourId",  TourId},
                { "passTransfer",  transfer},
                { "SelectRecipient",  false},
                { "IsFromBalancePage",  true},
            };
            await Shell.Current.GoToAsync($"Tour/Shares/CreateTransferSelect/CreateTransferSubmit", navigationParameter);
        }

        [RelayCommand]
        async Task GoSettleUpClear()
        {
            var navigationParameter = new Dictionary<string, object>
            {
                { "passTourId",  TourId},
                { "passTransfer",  null},
                { "SelectRecipient",  false},
                { "IsAllParticipantMode",  false},
                { "IsFromBalancePage",  true},
            };
            await Shell.Current.GoToAsync($"Tour/Shares/CreateTransferSelect", navigationParameter);
        }

        [RelayCommand]
        async Task GoRemind(OtherUser ou)
        {
            string response = (string)await Shell.Current.CurrentPage.ShowPopupAsync(new SelectRemindOptionPopups());
            if(string.IsNullOrEmpty(response))
                return;
            
            if (ou.Saldo < 0) //uzytkownik ou zalozyl pieniadze 
            {
                ou.Saldo *= -1;
                int tmp = ou.UserBalanceId;
                ou.UserBalanceId = ou.UserId;
                ou.UserId = tmp;
            }

            if (response == "email")
            {
                var resp = await m_UserService.SendRemindEmail(TourId, ou.UserBalanceId, ou);
                if(!resp.Success)
                {
                    await Shell.Current.CurrentPage.DisplayAlert("Błąd", $"{resp.Message}", "Ok");
                    return;
                }
            }
            else if (response == "notification")
            {
                //powiadomienie
                try
                {
                    var senderName = await m_TourService.GetTourExtendParticipantById(TourId, ou.UserBalanceId);
                    var ownedUser = await m_TourService.GetTourExtendParticipantById(TourId, ou.UserId);
                    string sN = string.IsNullOrEmpty(senderName.Nickname) ? senderName.FullName : senderName.Nickname;
                    string ownN = string.IsNullOrEmpty(ownedUser.Nickname) ? ownedUser.FullName : ownedUser.Nickname;

                    CreateNotificationDTO notificationDTO = new CreateNotificationDTO();
                    notificationDTO.UserId = ou.UserId;
                    notificationDTO.OtherUsers = new List<int> { ou.UserBalanceId };
                    notificationDTO.TourId = TourId;
                    notificationDTO.AddNotifyToParticipantsOfTour = false;
                    notificationDTO.IsVisited = false;
                    notificationDTO.CreatedDate = DateTime.Now;
                    notificationDTO.Name = $"Przypomienie o zaległościach w wyjeździe: ";
                    notificationDTO.Message = $"{ownN} przypomninamy o wyregulowaniu należności do użytkownika {sN}, w wysokości {ou.Saldo}zł";
                    notificationDTO.IconPath = "money_coin_sec.png";
                    notificationDTO.Type = NotificationType.MakeNewOrganizerAlert;
                    await m_NotificationViewModel.SendNotifyToUserOfIdAndMe(notificationDTO);
                }
                catch (Exception)
                {
                    await Shell.Current.CurrentPage.DisplayAlert("Błąd", "Nie udało się wysłać przypomienia w formie powiadomienia", "Ok");
                    return;
                }
            }

            var confirmCopyToast = Toast.Make("Wysłano przypomienie", ToastDuration.Long, 14);
            await confirmCopyToast.Show();
        }

        [RelayCommand]
        async Task SendRemindEmial(int otherUserId)
        {

        }

        [RelayCommand]
        async Task SendRemindNotification(int otherUserId)
        {
        }


        [RelayCommand]
        async Task ChangeExpandList(UserBalance ot)
        {
            ot.IsExpand = !ot.IsExpand;
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
            var result = await m_ShareService.GetBalance(TourId);

            if (!result.Success)
            {
                await Shell.Current.CurrentPage.DisplayAlert("Błąd", result.Message, "Ok");
            }
            else
            {
                Balance = result.Data;
            }
        }
    }
}
