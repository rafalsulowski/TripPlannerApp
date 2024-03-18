using CommunityToolkit.Maui.Alerts;
using CommunityToolkit.Maui.Core;
using CommunityToolkit.Maui.Core.Extensions;
using CommunityToolkit.Maui.Views;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.AspNetCore.SignalR;
using Microsoft.AspNetCore.SignalR.Client;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Newtonsoft.Json;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Diagnostics.Metrics;
using TripPlanner.Controls.QuestionnaireControls;
using TripPlanner.Models.DTO.MessageDTOs;
using TripPlanner.Models.DTO.MessageDTOs.QuestionnaireDTOs;
using TripPlanner.Models.DTO.UserDTOs;
using TripPlanner.Models.Models.UserModels;
using TripPlanner.Services;
using TripPlanner.ViewModels.Home;
using TripPlanner.Views.ChatViews;

namespace TripPlanner.ViewModels.Chat
{
    public partial class ChatViewModel : ObservableObject, IQueryAttributable
    {
        private readonly NotificationViewModel m_NotificationViewModel;
        private readonly Configuration m_Configuration;
        private readonly TourService m_TourService;
        private readonly ChatService m_ChatService;
        private HubConnection m_Connection;
        private int TourId;


        [ObservableProperty]
        ObservableCollection<MessageDTO> messages;

        [ObservableProperty]
        string message;

        public ChatViewModel(Configuration configuration, TourService tourService, ChatService chatService, NotificationViewModel notificationViewModel)
        {
            m_Configuration = configuration;
            m_TourService = tourService;
            m_ChatService = chatService;
            Messages = new ObservableCollection<MessageDTO>();
            m_NotificationViewModel = notificationViewModel;
        }

        [RelayCommand]
        void Appearing()
        {
        }

        public async void ApplyQueryAttributes(IDictionary<string, object> query)
        {
            TourId = (int)query["passTourId"];
            await Connect();
        }

        async Task Connect()
        {
            try
            {
                m_Connection = new HubConnectionBuilder()
                .WithUrl(m_Configuration.WssChatUrl)
                .Build();

                m_Connection.On<string>("TextMessageReceived", (message) =>
                {
                    TextMessageDTO msg = JsonConvert.DeserializeObject<TextMessageDTO>(message);
                    if (msg != null)
                    {
                        Messages.Add(msg);
                    }
                });

                m_Connection.On<string>("NoticeMessageReceived", async (message) =>
                {
                    NoticeMessageDTO msg = JsonConvert.DeserializeObject<NoticeMessageDTO>(message);
                    if (msg != null)
                    {
                        //powiadomienie
                        try
                        {
                            var senderName = await m_TourService.GetTourExtendParticipantById(TourId, msg.UserId);
                            string sN = string.IsNullOrEmpty(senderName.Nickname) ? senderName.FullName : senderName.Nickname;

                            CreateNotificationDTO notificationDTO = new CreateNotificationDTO();
                            notificationDTO.UserId = -1; //adresat
                            notificationDTO.TourId = TourId;
                            notificationDTO.AddNotifyToParticipantsOfTour = true;
                            notificationDTO.IsVisited = false;
                            notificationDTO.AddNotifyToParticipantsOfTour = true;
                            notificationDTO.CreatedDate = DateTime.Now;
                            notificationDTO.Name = $"Nowe ogłoszenie w wyjeździe: ";
                            notificationDTO.Message = $"{sN} dodał nowe ogłoszenie o treści \"{msg.Content}\", dodane: {msg.Date:hh:mm dd.MM.yyyy}";
                            notificationDTO.IconPath = "checklist_sec.png";
                            notificationDTO.Type = NotificationType.NotifyMessageAddedAlert;
                            await m_NotificationViewModel.SendNotifyToUsersOfTour(notificationDTO);
                        }
                        catch (Exception) { }

                        Messages.Add(msg);
                    }
                });

                m_Connection.On<string>("QuestionnaireReceived", async (message) =>
                {
                    QuestionnaireDTO msg = JsonConvert.DeserializeObject<QuestionnaireDTO>(message);
                    if (msg != null)
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
                            notificationDTO.Name = $"Nowa ankieta w wyjeździe: ";
                            notificationDTO.Message = $"Nowa ankieta o nazwie \"{msg.Content}\", z {msg.Answers.Count} odpowiedziami";
                            notificationDTO.IconPath = "checklist_sec.png";
                            notificationDTO.Type = NotificationType.QuestionnaireMessageAddedAlert;
                            await m_NotificationViewModel.SendNotifyToUsersOfTour(notificationDTO);
                        }
                        catch (Exception) { }

                        Messages.Add(msg);
                    }
                });

                m_Connection.On<string>("QuestionnaireVoteReceived", (message) =>
                {
                    QuestionnaireDTO msg = JsonConvert.DeserializeObject<QuestionnaireDTO>(message);
                    var elem = Messages.FirstOrDefault(u=> u.Id == msg.Id);
                    int index = Messages.IndexOf(elem);
                    if (msg != null && index != -1)
                    {
                        Messages.RemoveAt(index);
                        Messages.Insert(index, msg); //poprawic na akutalizowanie
                        //View.CollectionView.ScrollTo(elem);
                    }
                });

                m_Connection.On<string>("SetConnection", (message) =>
                {
                    JsonSerializerSettings settings = new JsonSerializerSettings { TypeNameHandling = TypeNameHandling.All };
                    List<MessageDTO> msgs = JsonConvert.DeserializeObject<List<MessageDTO>>(message, settings);
                    Messages = msgs.ToObservableCollection();
                    //if(Messages.Any())
                    //    View.CollectionView.ScrollTo(Messages.Last(), ScrollToPosition.End, animate: false);
                });

                await m_Connection.StartAsync();
                await m_Connection.InvokeCoreAsync("SetConnection", args: new[] { TourId.ToString() });
            }
            catch (HubException)
            {
                await Shell.Current.CurrentPage.DisplayAlert("Błąd", $"Nie udało wykonać operacji", "Ok");
            }
            catch(Exception)
            {
                await Shell.Current.CurrentPage.DisplayAlert("Błąd", $"Nieznany błąd", "Ok");
            }
        }

        [RelayCommand]
        async Task GoBack()
        {
            await m_Connection.InvokeCoreAsync("LeaveGroup", args: new[] { TourId.ToString() });

            var navigationParameter = new Dictionary<string, object>
            {
                { "passTourId",  TourId}
            };
            await Shell.Current.GoToAsync($"Tour", navigationParameter);
        }

        [RelayCommand]
        async Task SendTextMessage()
        {
            try
            {
                //walidacja treści wiadomości
                Message = Message.TrimStart().TrimEnd();

                if (string.IsNullOrEmpty(Message))
                    return;

                CreateTextMessageDTO msg = new CreateTextMessageDTO
                {
                    Content = Message,
                    UserId = m_Configuration.User.Id,
                    TourId = TourId
                };

                string json = JsonConvert.SerializeObject(msg);
                await m_Connection.InvokeCoreAsync("SendTextMessage", args: new[] { json });
                Message = String.Empty;
            }
            catch (HubException)
            {
                await Shell.Current.CurrentPage.DisplayAlert("Błąd", "Nie udało się wysłać wiadomości", "Ok");
            }
            catch (Exception)
            {
                await Shell.Current.CurrentPage.DisplayAlert("Błąd", $"Nieznany błąd", "Ok");
            }
        }

        [RelayCommand]
        async Task SendNoticeMessage()
        {
            try
            {
                //walidacja treści wiadomości pod wzgledem prób hackowania
                Message = Message.TrimStart().TrimEnd();

                if (string.IsNullOrEmpty(Message))
                    return;

                CreateNoticeMessageDTO msg = new CreateNoticeMessageDTO
                {
                    Content = Message,
                    UserId = m_Configuration.User.Id,
                    TourId = TourId
                };

                string json = JsonConvert.SerializeObject(msg);
                await m_Connection.InvokeCoreAsync("SendNoticeMessage", args: new[] { json });
                Message = String.Empty;
            }
            catch (HubException ex)
            {
                await Shell.Current.CurrentPage.DisplayAlert("Błąd", $"{ex.Message}", "Ok");
            }
            catch (Exception)
            {
                await Shell.Current.CurrentPage.DisplayAlert("Błąd", $"Nieznany błąd", "Ok");
            }
        }

        [RelayCommand]
        async Task ShowMoreChatAction()
        {
            await Shell.Current.CurrentPage.ShowPopupAsync(new ChatAdditionalMenuPopup(m_TourService, TourId));
        }


        [RelayCommand]
        async Task Vote(AnswerGDTO answer)
        {
            try
            {
                CreateQuestionnaireVoteDTO msg = new CreateQuestionnaireVoteDTO
                {
                    QuestionnaireId = answer.QuestionnaireId,
                    AnswerId = answer.Id,
                    TourId = TourId,
                    UserId = m_Configuration.User.Id
                };

                string json = JsonConvert.SerializeObject(msg);
                await m_Connection.InvokeCoreAsync("SendQuestionnaireVote", args: new[] { json });
                var confirmCopyToast = Toast.Make($"Oddano głos", ToastDuration.Long, 14);
                await confirmCopyToast.Show();
            }
            catch (HubException ex)
            {
                await Shell.Current.CurrentPage.DisplayAlert("Błąd", $"{ex.Message}", "Ok");
            }
            catch (Exception)
            {
                await Shell.Current.CurrentPage.DisplayAlert("Błąd", $"Nieznany błąd", "Ok");
            }
        }

        [RelayCommand]
        async Task ShowVoters(AnswerGDTO answer)
        {
            var res = await m_ChatService.GetAnswerVoters(answer.Id, TourId);
            await Shell.Current.CurrentPage.ShowPopupAsync(new PeopleChatListPopups($"Zagłosowali na \"{answer.Answer}\"", res));
        }
    }
}
