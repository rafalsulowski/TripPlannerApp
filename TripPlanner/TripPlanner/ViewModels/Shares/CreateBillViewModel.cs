using CommunityToolkit.Maui.Alerts;
using CommunityToolkit.Maui.Core;
using CommunityToolkit.Maui.Core.Extensions;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Collections.ObjectModel;
using System.Xml.Linq;
using TripPlanner.Models;
using TripPlanner.Models.DTO.BillDTOs;
using TripPlanner.Models.DTO.TourDTOs;
using TripPlanner.Models.DTO.UserDTOs;
using TripPlanner.Models.Models;
using TripPlanner.Models.Models.BillModels;
using TripPlanner.Models.Models.MessageModels;
using TripPlanner.Models.Models.TourModels;
using TripPlanner.Services;
using TripPlanner.ViewModels.Home;
using TripPlanner.Views.ShareViews;

namespace TripPlanner.ViewModels.Shares
{
    public partial class CreateBillViewModel : ObservableObject, IQueryAttributable
    {
        private readonly ShareService m_ShareService;
        private readonly TourService m_TourService;
        private readonly Configuration m_Configuration;
        private int TourId;
        private decimal OldValue;
        private bool SplitAccepted;
        private SplitBillView SplitItem;
        private readonly NotificationViewModel m_NotificationViewModel;

        [ObservableProperty]
        CreateBillDTO bill;

        [ObservableProperty]
        decimal value;

        [ObservableProperty]
        List<ExtendParticipantDTO> participants;

        [ObservableProperty]
        bool isExpanded;

        [ObservableProperty]
        bool isDescriptionVisible;

        [ObservableProperty]
        bool isPromptDescriptionVisible;

        [ObservableProperty]
        string divisionName;

        [ObservableProperty]
        string headerText;

        [ObservableProperty]
        bool isEditing;

        [ObservableProperty]
        ExtendParticipantDTO payerParticipant;

        public CreateBillViewModel(ShareService shareService, Configuration configuration, TourService tourService, NotificationViewModel notificationViewModel)
        {
            m_ShareService = shareService;
            m_TourService = tourService;
            m_Configuration = configuration;
            m_NotificationViewModel = notificationViewModel;

            HeaderText = "Dodajesz rachunek";
            IsEditing = false;
            SplitAccepted = false;
            IsExpanded = false;
            isDescriptionVisible = false;
            isPromptDescriptionVisible = true;
            OldValue = Value = 150;

            SplitItem = null;
            Participants = new List<ExtendParticipantDTO>();
            Bill = new CreateBillDTO 
            {
                PayerId = m_Configuration.User.Id,
                TourId = TourId,
                BillType = BillType.Equally,
                CreatedDate = new DateTime(2023, 12, 29),
                Value = new decimal(0),
                Name = "Zakupy biedronka 29.12.2023r.",
                Description = "alko + jedzenie na pierwsyz dzień dla wszyskich",
                CreatorId = m_Configuration.User.Id,
                ImageFilePath = "",
                Contributors = new List<BillContributorDTO>()
            };
        }

        public async void ApplyQueryAttributes(IDictionary<string, object> query)
        {
            TourId = (int)query["passTourId"];
            var resultFromDividePage = (CreateBillDTO)query["passBill"];
            var splitBillView = (SplitBillView)query["passSplitBillView"]; 
            SplitAccepted = (bool)query["passSplitBillViewAccept"];
            IsEditing = (bool)query["passIsEditing"];
            await LoadData();

            if (IsEditing)
                HeaderText = "Edaycja rachunku";
            else
                HeaderText = "Dodajesz rachunek";

            if (resultFromDividePage != null)
            {
                Bill = resultFromDividePage;
                OldValue = Value;
                Value = Bill.Value;
            }
            if(splitBillView != null)
            {
                SplitItem = splitBillView;
            }

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
        async Task ChangePayer(ExtendParticipantDTO participant)
        {
            PayerParticipant = participant;
            Bill.PayerId = participant.UserId;
            IsExpanded = false;
        }

        [RelayCommand]
        async Task GoDefDivision()
        {
            if (OldValue != Value)
                SplitItem = null;
            Bill.Value = Value;
            var navigationParameter = new Dictionary<string, object>
            {
                { "passTourId",  TourId},
                { "passBill", Bill},
                { "passSplitBillView",  SplitItem}
            };
            await Shell.Current.GoToAsync("Tour/Shares/CreateBill/DivisionType", navigationParameter);
        }

        [RelayCommand]
        async Task AddImage()
        {
            var confirmCopyToast = Toast.Make("Funkcjonalność niezaimplementowana", ToastDuration.Short, 14);
            await confirmCopyToast.Show();
        }

        [RelayCommand]
        async Task AddDescription()
        {
            Bill.Description = await Shell.Current.CurrentPage.DisplayPromptAsync("Notatka", "", "Ok", "");
            if(Bill.Description.Length > 0)
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
        async Task GoNext()
        {
            //walidacja
            if (string.IsNullOrEmpty(Bill.Name))
            {
                await Shell.Current.CurrentPage.DisplayAlert("Niepoprawne dane", $"Tytul rachunku nie może być pusty", "Ok");
                return;
            }
            if (Bill.Value == 0)
            {
                await Shell.Current.CurrentPage.DisplayAlert("Niepoprawne dane", $"Wartość rachunku nie może wynosić zero", "Ok");
                return;
            }
            if (!SplitAccepted)
            {
                await Shell.Current.CurrentPage.DisplayAlert("Niepoprawne dane", $"Nie ustawiłeś podziału", "Ok");
                return;
            }

            if(Bill.Value != Value)
            {
                SplitAccepted = false;
                await Shell.Current.CurrentPage.DisplayAlert("Niepoprawne dane", $"Zmieniłeś wartość rachunku bez ponownego ustawienia podziału", "Ok");
                return;
            }


            Bill.Value = Value;
            Bill.Contributors.Clear();
            for(int i = 0; i < SplitItem.Contributors.Count; i++)
            {
                Bill.Contributors.Add(new BillContributorDTO
                {
                    UserId = SplitItem.Contributors[i].UserId,
                    Due = SplitItem.Contributors[i].Due,
                    BillId = 0
                });
            }

            Bill.CreatedDate = DateTime.Now;
            Bill.TourId = TourId;
            Bill.CreatorId = m_Configuration.User.Id;
            Bill.ImageFilePath = "";

            if(IsEditing)
            {
                RepositoryResponse<bool> resp = m_ShareService.UpdateBill(Bill).Result;
                if (!resp.Success)
                {
                    await Shell.Current.CurrentPage.DisplayAlert("Błąd", $"{resp.Message}", "Ok");
                    return;
                }

                var confirmCopyToast = Toast.Make($"Zmodyfikowano rachunek", ToastDuration.Short, 14);
                await confirmCopyToast.Show();

                var navigationParameter = new Dictionary<string, object>
                {
                    { "passTourId",  TourId}
                };
                await Shell.Current.GoToAsync($"/Tour/Shares", navigationParameter);
            }
            else
            {
                RepositoryResponse<bool> resp = m_ShareService.CreateBill(Bill).Result;
                if (!resp.Success)
                {
                    await Shell.Current.CurrentPage.DisplayAlert("Błąd", $"{resp.Message}", "Ok");
                    return;
                }

                //powiadomienie
                try
                {
                    var senderName = await m_TourService.GetTourExtendParticipantById(TourId, Bill.PayerId);
                    string sN = string.IsNullOrEmpty(senderName.Nickname) ? senderName.FullName : senderName.Nickname;

                    CreateNotificationDTO notificationDTO = new CreateNotificationDTO();
                    notificationDTO.UserId = -1; //adresat
                    notificationDTO.TourId = TourId;
                    notificationDTO.AddNotifyToParticipantsOfTour = true;
                    notificationDTO.IsVisited = false;
                    notificationDTO.CreatedDate = DateTime.Now;
                    notificationDTO.Name = $"Nowy rachunek w wyjeździe: ";
                    notificationDTO.Message = $"Nowy rachunek o wartości {Bill.Value:N2}, osoba płacąca: {sN}, sposób podziału: \"{m_Configuration.GetBillTypeName(Bill.BillType)}\", nazwa \"{Bill.Name}\"";
                    notificationDTO.IconPath = "bill_sec.png";
                    notificationDTO.Type = Models.Models.UserModels.NotificationType.BillAddedAlert;
                    await m_NotificationViewModel.SendNotifyToUsersOfTour(notificationDTO);
                }
                catch (Exception) { }

                var confirmCopyToast = Toast.Make($"Dodano rachunek", ToastDuration.Short, 14);
                await confirmCopyToast.Show();

                var navigationParameter = new Dictionary<string, object>
                {
                    { "passTourId",  TourId}
                };
                await Shell.Current.GoToAsync($"/Tour/Shares", navigationParameter);
            }
        }

        public static SplitBillView CreateSplitView(BillPresentationDTO BillPrez)
        {
            SplitBillView splitBill = new SplitBillView();
            switch (BillPrez.BillType)
            {
                case BillType.Equally:
                    splitBill.BillType = BillType.Equally;
                    splitBill.Icon = "pen_sec.png";
                    splitBill.Title = "Równomiernie";
                    splitBill.Description = "Wybierz które osoby mają dzielić się wspólnie rachunkiem";
                    splitBill.IsCheckedAll = true;
                    splitBill.SplitFirstInfo = $"{(BillPrez.Value / BillPrez.Contributors.Count):N2}zł / osobę";
                    splitBill.SplitSecondInfo = $"(wszyscy się składają)";
                    splitBill.Contributors = BillPrez.Contributors.ToObservableCollection();
                    return splitBill;

                case BillType.Unequally:
                    splitBill.BillType = Models.Models.BillModels.BillType.Unequally;
                    splitBill.Icon = "pen_sec.png";
                    splitBill.Title = "Nierównomiernie";
                    splitBill.Description = "Wprowadź dokładnie ile dane osoby są dłużne";
                    splitBill.SplitFirstInfo = $"{0:N2}zł z {BillPrez.Value}zł";
                    splitBill.SplitSecondInfo = $"pozostało {BillPrez.Value}zł";
                    splitBill.Contributors = BillPrez.Contributors.ToObservableCollection();
                    return splitBill;

                case BillType.ByPercentages:
                    splitBill.BillType = Models.Models.BillModels.BillType.ByPercentages;
                    splitBill.Icon = "pen_sec.png";
                    splitBill.Title = "Procentowo";
                    splitBill.Description = "Wprowadź procentowy udział dla twojej sytuacji";
                    splitBill.IsCheckedAll = true;
                    splitBill.SplitFirstInfo = $"0% z 100%";
                    splitBill.SplitSecondInfo = $"pozostało 100%";
                    splitBill.Contributors = BillPrez.Contributors.ToObservableCollection();
                    return splitBill;

                case BillType.ByShares:
                    splitBill.BillType = Models.Models.BillModels.BillType.ByShares;
                    splitBill.Icon = "pen_sec.png";
                    splitBill.Title = "Przez udział";
                    splitBill.Description = "Wprowadź wielkość udziału jaką wykorzystały dane osoby";
                    splitBill.IsCheckedAll = true;
                    splitBill.SplitFirstInfo = $"0 udziałów";
                    splitBill.SplitSecondInfo = "";
                    splitBill.Contributors = BillPrez.Contributors.ToObservableCollection();
                    return splitBill;

                case BillType.ByAdjustment:
                    splitBill.BillType = Models.Models.BillModels.BillType.ByAdjustment;
                    splitBill.Icon = "pen_sec.png";
                    splitBill.Title = "Przez dodanie wartości";
                    splitBill.Description = "Wprowadź kto musi dodatkowo zapłacić";
                    splitBill.IsCheckedAll = true;
                    splitBill.SplitFirstInfo = "";
                    splitBill.SplitSecondInfo = "";
                    splitBill.Contributors = BillPrez.Contributors.ToObservableCollection();
                    return splitBill;

                default:
                    return null;
            }
        }

        private async Task LoadData()
        {
            var result = await m_TourService.GetTourExtendParticipant(TourId);

            if (result.Any())
            {
                Participants = result;

                PayerParticipant = new ExtendParticipantDTO
                {
                    FullName = Participants.FirstOrDefault(u => u.UserId == m_Configuration.User.Id).FullName,
                    UserId = m_Configuration.User.Id,
                };
            }
            else
                PayerParticipant = new ExtendParticipantDTO
                {
                    FullName = m_Configuration.User.FullName,
                    UserId = m_Configuration.User.Id,
                };
        }
    }
}
