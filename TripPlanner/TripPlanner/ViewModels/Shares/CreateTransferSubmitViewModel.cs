using CommunityToolkit.Maui.Alerts;
using CommunityToolkit.Maui.Core;
using CommunityToolkit.Maui.Core.Extensions;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using System.Collections.ObjectModel;
using TripPlanner.Models.DTO.BillDTOs;
using TripPlanner.Models.DTO.TourDTOs;
using TripPlanner.Models.DTO.UserDTOs;
using TripPlanner.Models.Models;
using TripPlanner.Models.Models.BillModels;
using TripPlanner.Services;
using TripPlanner.ViewModels.Home;

namespace TripPlanner.ViewModels.Shares
{
    public partial class CreateTransferSubmitViewModel : ObservableObject, IQueryAttributable
    {
        private readonly ShareService m_ShareService;
        private readonly TourService m_TourService;
        private readonly NotificationViewModel m_NotificationViewModel;
        private readonly Configuration m_Configuration;
        private int TourId;
        private bool IsSelectRecipientMode;
        private bool IsFromBalancePage;

        [ObservableProperty]
        CreateTransferDTO transfer;

        [ObservableProperty]
        ExtendParticipantDTO recipient;

        [ObservableProperty]
        ExtendParticipantDTO sender;

        [ObservableProperty]
        bool isExpanded1;

        [ObservableProperty]
        bool isExpanded2;

        [ObservableProperty]
        ObservableCollection<ExtendParticipantDTO> allParticipants;

        [ObservableProperty]
        bool isDescriptionVisible;

        [ObservableProperty]
        bool isPromptDescriptionVisible;

        [ObservableProperty]
        string labelContent;

        public CreateTransferSubmitViewModel(ShareService shareService, Configuration configuration, TourService tourService, NotificationViewModel notificationViewModel)
        {
            m_ShareService = shareService;
            m_NotificationViewModel = notificationViewModel;
            m_TourService = tourService;
            m_Configuration = configuration;

            recipient = new ExtendParticipantDTO();
            sender = new ExtendParticipantDTO();
            isExpanded1 = false;
            isExpanded2 = false;
            IsDescriptionVisible = false; 
            IsFromBalancePage = false;
            IsPromptDescriptionVisible = true;
            AllParticipants = new ObservableCollection<ExtendParticipantDTO>();
            Transfer = new CreateTransferDTO();
        }

        public async void ApplyQueryAttributes(IDictionary<string, object> query)
        {
            TourId = (int)query["passTourId"];
            var res = (CreateTransferDTO)query["passTransfer"];
            IsSelectRecipientMode = (bool)query["SelectRecipient"];
            IsFromBalancePage = (bool)query["IsFromBalancePage"];
            await LoadData();

            if (res != null)
            {
                Transfer = res;
                Sender = AllParticipants.FirstOrDefault(u => u.UserId == Transfer.SenderId);
                Recipient = AllParticipants.FirstOrDefault(u => u.UserId == Transfer.RecipientId);
            }
            else
            {
                await Shell.Current.CurrentPage.DisplayAlert("Błąd", "Brak obiektu Transfer", "Ok");
                await GoBack();
            }

        }

        [RelayCommand]
        async Task ChangeSender(ExtendParticipantDTO participant)
        {
            Sender = participant;
            Transfer.SenderId = participant.UserId;
            IsExpanded1 = false;
        }

        [RelayCommand]
        async Task ChangeRecipient(ExtendParticipantDTO participant)
        {
            Recipient = participant;
            Transfer.RecipientId = participant.UserId;
            IsExpanded2 = false;
        }

        [RelayCommand]
        async Task ShowTransferInfo()
        {
            await Shell.Current.CurrentPage.DisplayAlert("Tutaj tylko dodajesz potwierdzenie wykonania rozliczenia", "Płatności dokonujesz za pomocą konwencjonalnych sposobów m.in. przez bank lub rozliczenie gotówkowe następnie dodajesz rozliczenie tutaj jako potwierdzenie", "Jasne");
        }

        [RelayCommand]
        async Task AddDescription()
        {
            Transfer.Description = await Shell.Current.CurrentPage.DisplayPromptAsync("Notatka", "", "Ok", "");
            if (Transfer.Description.Length > 0)
            {
                IsPromptDescriptionVisible = false;
                IsDescriptionVisible = true;
            }
            else
            {
                IsPromptDescriptionVisible = true;
                IsDescriptionVisible = false;
            }

        }

        [RelayCommand]
        async Task GoBack()
        {
            if (IsFromBalancePage) //go to balance
            {
                var navigationParameter = new Dictionary<string, object>
                {
                    { "passTourId",  TourId}
                };
                await Shell.Current.GoToAsync($"Tour/Shares/Balance", navigationParameter);
            }
            else //go to select sender and recipient transfer page
            {
                var navigationParameter = new Dictionary<string, object>
                {
                    { "passTourId",  TourId},
                    { "passTransfer",  Transfer},
                    { "SelectRecipient",  IsSelectRecipientMode},
                    { "IsAllParticipantMode",  IsSelectRecipientMode},
                    { "IsFromBalancePage",  false},
                };
                await Shell.Current.GoToAsync($"Tour/Shares/CreateTransferSelect", navigationParameter);
            }
        }

        [RelayCommand]
        async Task Submit()
        {
            //walidacja
            if (Transfer.Value == 0)
            {
                await Shell.Current.CurrentPage.DisplayAlert("Niepoprawne dane", $"Wartość transakcji nie może wynosić zero", "Ok");
                return;
            }
            if (Transfer.SenderId == Transfer.RecipientId)
            {
                await Shell.Current.CurrentPage.DisplayAlert("Niepoprawne dane", $"Nie możesz dokonać transakcji gdzie nadawcą i odbiorcą są te same osoby", "Ok");
                return;
            }

            Transfer.Name = "";
            Transfer.CreatedDate = DateTime.Now;
            Transfer.TourId = TourId;
            Transfer.ImageFilePath = "";
            Transfer.CreatorId = m_Configuration.User.Id;

            RepositoryResponse<bool> resp = m_ShareService.CreateTransfer(Transfer).Result;
            if (!resp.Success)
            {
                await Shell.Current.CurrentPage.DisplayAlert("Błąd", $"{resp.Message}", "Ok");
                return;
            }

            //powiadomienie
            try
            {
                var senderName = await m_TourService.GetTourExtendParticipantById(TourId, Transfer.SenderId);
                var recName = await m_TourService.GetTourExtendParticipantById(TourId, Transfer.RecipientId);
                string sN = string.IsNullOrEmpty(senderName.Nickname) ? senderName.FullName : senderName.Nickname;
                string rN = string.IsNullOrEmpty(recName.Nickname) ? recName.FullName : recName.Nickname;

                CreateNotificationDTO notificationDTO = new CreateNotificationDTO();
                notificationDTO.UserId = -1; //adresat
                notificationDTO.TourId = TourId;
                notificationDTO.AddNotifyToParticipantsOfTour = true;
                notificationDTO.IsVisited = false;
                notificationDTO.CreatedDate = DateTime.Now;
                notificationDTO.Name = $"Nowy transfer w wyjeździe: ";
                notificationDTO.Message = $"Transfer pieniędzy pomiędzy {sN} -> {rN}, w wysokości: {Transfer.Value}";
                notificationDTO.IconPath = "money_coin_sec.png";
                notificationDTO.Type = Models.Models.UserModels.NotificationType.TransferAddedAlert;
                await m_NotificationViewModel.SendNotifyToUsersOfTour(notificationDTO);
            }
            catch (Exception) { }

            var confirmCopyToast = Toast.Make($"Dodano transakcję", ToastDuration.Short, 14);
            await confirmCopyToast.Show();

            var navigationParameter = new Dictionary<string, object>
                {
                    { "passTourId",  TourId}
                };
            await Shell.Current.GoToAsync($"/Tour/Shares", navigationParameter);
        }

        private async Task LoadData()
        {
            var result = await m_TourService.GetTourExtendParticipant(TourId);

            if (result.Any())
            {
                AllParticipants = result.ToObservableCollection();
            }
        }
    }
}
