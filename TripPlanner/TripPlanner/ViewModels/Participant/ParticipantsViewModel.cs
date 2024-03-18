using CommunityToolkit.Maui.Alerts;
using CommunityToolkit.Maui.Core;
using CommunityToolkit.Maui.Core.Extensions;
using CommunityToolkit.Maui.Views;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Collections.ObjectModel;
using TripPlanner.Models.DTO.TourDTOs;
using TripPlanner.Models.DTO.UserDTOs;
using TripPlanner.Models.Models.UserModels;
using TripPlanner.Services;
using TripPlanner.ViewModels.Home;
using TripPlanner.Views.ParticipantViews;

namespace TripPlanner.ViewModels.Participant
{
    public partial class ParticipantsViewModel : ObservableObject, IQueryAttributable
    {
        private readonly Configuration m_Configuration;
        private readonly TourService m_TourService;
        private readonly NotificationViewModel m_NotificationViewModel;
        private UserDetailsPopups m_DetailsPopups;
        private ObservableCollection<ExtendParticipantDTO> ParticipantsRef;        
        private int TourId;
        private string InviteLink;

        [ObservableProperty]
        bool refresh;

        [ObservableProperty]
        bool isOrganizer;

        [ObservableProperty]
        string searchPrompt;

        [ObservableProperty]
        bool deleteSearchPromptVisible;

        [ObservableProperty]
        ExtendParticipantDTO userDetailsParticipant;
        
        [ObservableProperty]
        ObservableCollection<ExtendParticipantDTO> participants;

        public ParticipantsViewModel(Configuration configuration, TourService tourService, NotificationViewModel notificationViewModel)
        {
            m_Configuration = configuration;
            m_TourService = tourService;
            m_NotificationViewModel = notificationViewModel;
            userDetailsParticipant = new ExtendParticipantDTO();
            Participants = new ObservableCollection<ExtendParticipantDTO>();
            ParticipantsRef = new ObservableCollection<ExtendParticipantDTO>();
            IsOrganizer = false;
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
            await Shell.Current.GoToAsync($"Tour", navigationParameter);
        }

        [RelayCommand]
        async Task Add()
        {
            if (IsOrganizer)
            {
                await Shell.Current.CurrentPage.ShowPopupAsync(new AddParticipantPopup(TourId, InviteLink));
            }
            else
                await Shell.Current.CurrentPage.DisplayAlert("Błąd", $"Nie masz uprawnień", "Ok");

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
                Participants = ParticipantsRef;
            else
                Participants = ParticipantsRef.Where(i => i.FullName.StartsWith(SearchPrompt, StringComparison.OrdinalIgnoreCase) || i.Nickname.StartsWith(SearchPrompt, StringComparison.OrdinalIgnoreCase)).ToObservableCollection();
        }

        [RelayCommand]
        public async Task DeleteSearching()
        {
            SearchPrompt = "";
            await Searching();
        }

        [RelayCommand]
        public async Task GoToFriendsList()
        {
            var navigationParameter = new Dictionary<string, object>
            {
                { "passTourId",  TourId}
            };
            await Shell.Current.GoToAsync($"/AddParticipantFromFriends", navigationParameter);
        }
        
        [RelayCommand]
        public async Task UserDetails(ExtendParticipantDTO extendParticipantDTO)
        {
            var res = await m_TourService.GetTourExtendParticipantById(TourId, extendParticipantDTO.UserId);
            if(res == null)
            {
                await Shell.Current.CurrentPage.DisplayAlert("Błąd", "Nie udało się pobrać danych o użytkowniku, sprawdź swoje połączenie z internetem", "Ok");
            }
            else
            {
                UserDetailsParticipant = res;
                m_DetailsPopups = new UserDetailsPopups(res, m_Configuration.User.Id, IsOrganizer);
                await Shell.Current.CurrentPage.ShowPopupAsync(m_DetailsPopups);
            }
        }

        [RelayCommand]
        public async Task ChangeNickname()
        {
            string result = await Shell.Current.CurrentPage.DisplayPromptAsync("Wprowadź nową ksywkę", "", "Ok", "", UserDetailsParticipant.Nickname);

            if(string.IsNullOrEmpty(result))
            {
                await Shell.Current.CurrentPage.DisplayAlert("Błąd","Wartość nie może być pusta", "Ok");
            }
            else
            {
                var res = await m_TourService.UpdateParticipantNickname(TourId, UserDetailsParticipant.UserId, result);
                if (res.Success)
                {
                    await LoadData();
                    var confirmCopyToast = Toast.Make("Zmieniono ksywkę", ToastDuration.Short, 14);
                    await confirmCopyToast.Show();

                    await m_DetailsPopups.CloseAsync();
                    UserDetailsParticipant.Nickname = result;
                }
                else
                {
                    await Shell.Current.CurrentPage.DisplayAlert("Błąd", res.Message, "Ok");
                }
            }
        }

        [RelayCommand]
        async Task LeftTour(ExtendParticipantDTO participant)
        {
            int sum = 0;
            foreach (var el in ParticipantsRef)
                if (el.IsOrganizer)
                    sum++;

            if (IsOrganizer && sum <= 1)
            {
                await Shell.Current.CurrentPage.DisplayAlert("Uwaga", "Jesteś jedynym organiazatorem wyjazdu, nie możesz wyjśc ponieważ wyjazd musi mieć minimalnie jednego organiazatora", "Ok");
            }
            else
            {
                var res = await Shell.Current.CurrentPage.DisplayAlert("Uwaga", "Czy na pewno chcesz opuścić wyjazd?", "Dalej", "Anuluj");
                if (res)
                {
                    var response = await m_TourService.DeleteParticipant(TourId, participant.UserId);
                    if (response.Success)
                    {
                        var navigationParameter = new Dictionary<string, object> 
                        { 
                            { "Reload", true }
                        };
                        await Shell.Current.GoToAsync("//Home", navigationParameter);
                        var confirmCopyToast = Toast.Make($"Opuszczono wyjazd", ToastDuration.Long, 14);
                        await confirmCopyToast.Show();
                    }
                    else
                        await Shell.Current.CurrentPage.DisplayAlert("Błąd", response.Message, "Ok");
                }
            }
        }

        [RelayCommand]
        async Task DeleteParticipantFromDetails()
        {
            await m_DetailsPopups.CloseAsync();
            await DeleteParticipant(UserDetailsParticipant);
        }

        [RelayCommand]
        async Task DeleteParticipant(ExtendParticipantDTO participant)
        {
            if(UserDetailsParticipant.UserId == m_Configuration.User.Id)
            {
                await LeftTour(UserDetailsParticipant);
            }
            else if (IsOrganizer)
            {
                ExtendParticipantDTO actualParticipant = m_TourService.GetTourExtendParticipantById(TourId, participant.UserId).Result;
                if (actualParticipant == null)
                {
                    await Shell.Current.CurrentPage.DisplayAlert("Uwaga", "Ten użytkownik nie jest już uczestnikiem tej wycieczki, odświerz listę", "Ok");
                }
                else
                {
                    var response = await m_TourService.DeleteParticipant(TourId, participant.UserId);
                    if (response.Success)
                    {
                        await LoadData();
                        string name = participant.Nickname == "" ? participant.FullName : participant.Nickname;
                        var confirmCopyToast = Toast.Make($"Usunięto {name} z wyjazdu", ToastDuration.Long, 14);
                        await confirmCopyToast.Show();
                    }
                    else
                        await Shell.Current.CurrentPage.DisplayAlert("Błąd", response.Message, "Ok");
                }
            }
            else
                await Shell.Current.CurrentPage.DisplayAlert("Błąd", $"Nie masz uprawnień", "Ok");
        }

        [RelayCommand]
        async Task MakeOrganizer(ExtendParticipantDTO participant)
        {
            if (IsOrganizer)
            {
                if (participant.IsOrganizer)
                {
                    await Shell.Current.CurrentPage.DisplayAlert("Uwaga", "Ten użytkownik jest już organizatorem tej wycieczki, odświerz listę", "Ok");
                }
                else
                {
                    var res = await Shell.Current.CurrentPage.DisplayAlert("Uwaga", "Spowoduje to, że nowy organizator będzie mieć dostęp do wszystkich czynności, takich jakie ty masz, nie udzielaj pozwoleń ograniazatora osobom do których nie masz zaufania.", "Dalej", "Anuluj");
                    if (res)
                    {
                        Random rnd = new Random();
                        int a = rnd.Next(0, 10);
                        int b = rnd.Next(0, 10);
                        int c = a + b;
                        string result = await Shell.Current.CurrentPage.DisplayPromptAsync("Walidacja", $"Podaj wynik działania: {a} + {b} = ?", initialValue: "0", maxLength: 2, keyboard: Keyboard.Numeric);

                        if (result != c.ToString())
                            await Shell.Current.CurrentPage.DisplayAlert("Uwaga", "Walidacja niepoprawna! Operacja anulowana.", "Ok");
                        else
                        {
                            ExtendParticipantDTO actualParticipant = m_TourService.GetTourExtendParticipantById(TourId, participant.UserId).Result;
                            if (actualParticipant == null)
                            {
                                await Shell.Current.CurrentPage.DisplayAlert("Uwaga", "Ten użytkownik nie jest już uczestnikiem tej wycieczki, odświerz listę", "Ok");
                            }
                            else if (actualParticipant.IsOrganizer)
                            {
                                await Shell.Current.CurrentPage.DisplayAlert("Uwaga", "Ten użytkownik jest już organizatorem tej wycieczki, odświerz listę", "Ok");
                            }
                            else if (actualParticipant != null && actualParticipant.IsOrganizer == false)
                            {
                                var response = await m_TourService.AddOrganizer(TourId, participant.UserId);
                                if (response.Success)
                                {
                                    //powiadomienie
                                    try
                                    {
                                        string name2 = string.IsNullOrEmpty(participant.Nickname) ? participant.FullName : participant.Nickname;

                                        CreateNotificationDTO notificationDTO = new CreateNotificationDTO();
                                        notificationDTO.UserId = -1;
                                        notificationDTO.TourId = TourId;
                                        notificationDTO.AddNotifyToParticipantsOfTour = true;
                                        notificationDTO.IsVisited = false;
                                        notificationDTO.CreatedDate = DateTime.Now;
                                        notificationDTO.Name = $"Dodano nowego organizatora wyjazdu: ";
                                        notificationDTO.Message = $"{m_Configuration.User.FullName} dodał nowego organizatora: {name2}";
                                        notificationDTO.IconPath = "organizer_sec.png";
                                        notificationDTO.Type = NotificationType.MakeNewOrganizerAlert;
                                        await m_NotificationViewModel.SendNotifyToUsersOfTour(notificationDTO);
                                    }
                                    catch (Exception) { }

                                    await LoadData();
                                    string name = participant.Nickname == "" ? participant.FullName : participant.Nickname;
                                    var confirmCopyToast = Toast.Make($"{name} jest teraz organizatorem wyjazdu", ToastDuration.Long, 14);
                                    await confirmCopyToast.Show();
                                }
                                else
                                    await Shell.Current.CurrentPage.DisplayAlert("Błąd", response.Message, "Ok");
                            }
                        }
                    }
                }
            }
            else
                await Shell.Current.CurrentPage.DisplayAlert("Błąd", $"Nie masz uprawnień", "Ok");
        }

        [RelayCommand]
        async Task DeleteOrganizer(ExtendParticipantDTO participant)
        {
            if (IsOrganizer)
            {
                if (Participants.Select(u => u.IsOrganizer).Where(u => u == true).Count() == 1)
                {
                    await Shell.Current.CurrentPage.DisplayAlert("Uwaga", "Wyjazd musi posiadać minimalnie jendego organiazatora", "Ok");
                    return;
                }

                if (!participant.IsOrganizer)
                {
                    await Shell.Current.CurrentPage.DisplayAlert("Uwaga", "Ten użytkownik nie jest już organizatorem tej wycieczki, odświerz listę", "Ok");
                }
                else
                {
                    string msg = "";
                    if (participant.UserId == m_Configuration.User.Id)
                        msg = "Spowoduje to, że utracisz możliwość wykonywania czynności organizatora";
                    else
                        msg = "Spowoduje to, że uczestnik utraci możliwość wykonywania czynności organizatora";

                    var res = await Shell.Current.CurrentPage.DisplayAlert("Uwaga", msg, "Dalej", "Anuluj");
                    if (res)
                    {
                        Random rnd = new Random();
                        int a = rnd.Next(0, 10);
                        int b = rnd.Next(0, 10);
                        int c = a + b;
                        string result = await Shell.Current.CurrentPage.DisplayPromptAsync("Walidacja", $"Podaj wynik działania: {a} + {b} = ?", initialValue: "", maxLength: 2, keyboard: Keyboard.Numeric);

                        if (result != c.ToString())
                            await Shell.Current.CurrentPage.DisplayAlert("Uwaga", "Walidacja niepoprawna! Operacja anulowana.", "Ok");
                        else
                        {
                            ExtendParticipantDTO actualParticipant = m_TourService.GetTourExtendParticipantById(TourId, participant.UserId).Result;
                            if (actualParticipant == null)
                            {
                                await Shell.Current.CurrentPage.DisplayAlert("Uwaga", "Ten użytkownik nie jest już uczestnikiem tej wycieczki, odświerz listę", "Ok");
                            }
                            else if (!actualParticipant.IsOrganizer)
                            {
                                await Shell.Current.CurrentPage.DisplayAlert("Uwaga", "Ten użytkownik nie jest już organizatorem tej wycieczki, odświerz listę", "Ok");
                            }
                            else if (actualParticipant != null && actualParticipant.IsOrganizer == true)
                            {
                                var response = await m_TourService.DeleteOrganizer(TourId, participant.UserId);
                                if (response.Success)
                                {
                                    await LoadData();
                                    string name = participant.Nickname == "" ? participant.FullName : participant.Nickname;
                                    var confirmCopyToast = Toast.Make($"{name} nie jest już organizatorem wyjazdu", ToastDuration.Long, 14);
                                    await confirmCopyToast.Show();
                                }
                                else
                                    await Shell.Current.CurrentPage.DisplayAlert("Błąd", response.Message, "Ok");
                            }
                        }
                    }
                }
            }
            else
                await Shell.Current.CurrentPage.DisplayAlert("Błąd", $"Nie masz uprawnień", "Ok");
        }

        [RelayCommand]
        public async Task CopyToClipboard()
        {
            await Clipboard.Default.SetTextAsync(InviteLink);

            var confirmCopyToast = Toast.Make("Skopiowano do schowka", ToastDuration.Short, 14);
            await confirmCopyToast.Show();
        }

        [RelayCommand]
        async Task RefreshView()
        {
            Refresh = true;

            await LoadData();
            var confirmCopyToast = Toast.Make("Odświerzono listę uczestników", ToastDuration.Short, 14);
            await confirmCopyToast.Show();
            Refresh = false;
        }
        
        private async Task LoadData()
        {
            var res = await m_TourService.GetTourWithParticipants(TourId);
            if (res is null)
            {
                await Shell.Current.CurrentPage.DisplayAlert("Błąd", "Nie udało się pobrać danych wyjazdu", "Ok");
                return;
            }

            var val = res.Participants.FirstOrDefault(u => u.UserId == m_Configuration.User.Id);
            if (val is null)
            {
                await Shell.Current.CurrentPage.DisplayAlert("Błąd", "Brak ciebie na liście uczestników wyjazdu", "Ok");
                return;
            }

            var value = await m_TourService.GetTourExtendParticipant(TourId);
            if (value is null)
            {
                await Shell.Current.CurrentPage.DisplayAlert("Błąd", "Nie udało się pobrać listy uczestników wyjazdu", "Ok");
            }

            InviteLink = res.InviteLink;
            IsOrganizer = val.IsOrganizer;
            Participants = value.ToObservableCollection();
            ParticipantsRef = value.ToObservableCollection();
        }
    }
}
