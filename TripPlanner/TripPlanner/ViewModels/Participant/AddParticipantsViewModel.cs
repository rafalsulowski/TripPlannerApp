
using CommunityToolkit.Maui.Alerts;
using CommunityToolkit.Maui.Core;
using CommunityToolkit.Maui.Core.Extensions;
using CommunityToolkit.Maui.Views;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Collections.ObjectModel;
using TripPlanner.Models.DTO.TourDTOs;
using TripPlanner.Models.DTO.UserDTOs;
using TripPlanner.Models.Models.TourModels;
using TripPlanner.Models.Models.UserModels;
using TripPlanner.Services;
using TripPlanner.ViewModels.Home;

namespace TripPlanner.ViewModels.Participant
{
    public partial class AddParticipantsViewModel : ObservableObject, IQueryAttributable
    {
        private readonly Configuration m_Configuration;
        private readonly NotificationViewModel m_NotificationViewModel;
        private readonly TourService m_TourService;
        private readonly UserService m_UserService;
        private ObservableCollection<ExtendFriendDTO> FriendsRef;
        private int TourId;

        [ObservableProperty]
        bool refresh;

        [ObservableProperty]
        string searchPrompt;

        [ObservableProperty]
        bool deleteSearchPromptVisible;

        [ObservableProperty]
        ObservableCollection<ExtendFriendDTO> friends;

        public AddParticipantsViewModel(Configuration configuration, TourService tourService, UserService userService, NotificationViewModel notificationViewModel)
        {
            m_Configuration = configuration;
            m_NotificationViewModel = notificationViewModel;
            m_TourService = tourService;
            m_UserService = userService;
            Friends = new ObservableCollection<ExtendFriendDTO>();
            FriendsRef = new ObservableCollection<ExtendFriendDTO>();
            SearchPrompt = "";
            DeleteSearchPromptVisible = false;
        }

        public void ApplyQueryAttributes(IDictionary<string, object> query)
        {
            TourId = (int)query["passTourId"];
            LoadData();
        }

        [RelayCommand]
        async Task GoBack()
        {
            var navigationParameter = new Dictionary<string, object>
            {
                { "passTourId",  TourId}
            };
            await Shell.Current.GoToAsync($"/Tour/Participants", navigationParameter);
        }

        [RelayCommand]
        async Task Add(ExtendFriendDTO friend)
        {
            if (friend.IsParticipant)
            {
                var confirmCopyToast = Toast.Make("Użytkownik jest już uczestnikiem wyjazdu", ToastDuration.Short, 14);
                await confirmCopyToast.Show();
            }
            else
            {
                var result = await Shell.Current.CurrentPage.DisplayAlert("Uwaga", $"Czy na pewno chcesz dodać {friend.FullName} do wyjazdu?", "Dodaj", "Anuluj");
                if (result)
                {
                    var response = await m_TourService.AddParticipant(TourId, friend.UserId);
                    if (response.Success)
                    {
                        //powiadomienie
                        try
                        {
                            CreateNotificationDTO notificationDTO = new CreateNotificationDTO();
                            notificationDTO.UserId = -1; //adresat
                            notificationDTO.TourId = TourId;
                            notificationDTO.AddNotifyToParticipantsOfTour = true;
                            notificationDTO.IsVisited = false;
                            notificationDTO.CreatedDate = DateTime.Now;
                            notificationDTO.Name = $"Dodano nowego uczestnika do wyjazdu: ";
                            notificationDTO.Message = $"{m_Configuration.User.FullName} dodał nowego uczestnika: {friend.FullName}";
                            notificationDTO.IconPath = "person_chat_sec.png";
                            notificationDTO.Type = NotificationType.NewFriendAlert;
                            await m_NotificationViewModel.SendNotifyToUsersOfTour(notificationDTO);
                        }
                        catch (Exception) { }

                        await RefreshViewAfterAdd();
                    }
                    else
                        await Shell.Current.CurrentPage.DisplayAlert("Błąd", response.Message, "Ok");
                }
            }
        }

        [RelayCommand]
        async Task DeleteParticipantFromFriendList(ExtendFriendDTO participant)
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
                    LoadData();
                    string name = participant.FullName;
                    var confirmCopyToast = Toast.Make($"Usunięto {name} z wyjazdu", ToastDuration.Long, 14);
                    await confirmCopyToast.Show();
                }
                else
                    await Shell.Current.CurrentPage.DisplayAlert("Błąd", response.Message, "Ok");
            }
        }

        [RelayCommand]
        public async Task Searching()
        {
            DeleteSearchPromptVisible = SearchPrompt.Length == 0 ? false : true;
            if (string.IsNullOrEmpty(SearchPrompt))
                Friends = FriendsRef;
            else
                Friends = FriendsRef.Where(i => i.FullName.StartsWith(SearchPrompt, StringComparison.OrdinalIgnoreCase))?.ToObservableCollection();
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
            LoadData();

            var confirmCopyToast = Toast.Make("Odświerzono listę znajomych", ToastDuration.Short, 14);
            await confirmCopyToast.Show();
            Refresh = false;
        }

        async Task RefreshViewAfterAdd()
        {
            LoadData();
            var confirmCopyToast = Toast.Make("Dodano do wyjazdu", ToastDuration.Short, 14);
            await confirmCopyToast.Show();
        }

        private async void LoadData()
        {
            var value = m_UserService.GetFriendsWithInfoAboutTour(m_Configuration.User.Id, TourId).Result;

            if (value is null)
            {
                await Shell.Current.CurrentPage.DisplayAlert("Błąd", "Nie udało się pobrać listy znajomych", "Ok");
            }
            else
            {
                Friends = value.ToObservableCollection();
                FriendsRef = value.ToObservableCollection();
            }
        }

    }
}