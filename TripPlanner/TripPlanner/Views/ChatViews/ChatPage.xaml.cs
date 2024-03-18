using CommunityToolkit.Maui.Alerts;
using CommunityToolkit.Maui.Core;
using CommunityToolkit.Maui.Core.Extensions;
using CommunityToolkit.Maui.Views;
using Microsoft.AspNetCore.SignalR;
using Microsoft.AspNetCore.SignalR.Client;
using Microsoft.EntityFrameworkCore.Diagnostics;
using MvvmHelpers;
using Newtonsoft.Json;
using System.Collections.ObjectModel;
using System.Reflection.Metadata.Ecma335;
using System.Windows.Input;
using TripPlanner.Controls.QuestionnaireControls;
using TripPlanner.Models.DTO.MessageDTOs;
using TripPlanner.Models.DTO.MessageDTOs.QuestionnaireDTOs;
using TripPlanner.Models.DTO.TourDTOs;
using TripPlanner.Models.DTO.UserDTOs;
using TripPlanner.Models.Models.UserModels;
using TripPlanner.Services;
using TripPlanner.ViewModels;
using TripPlanner.ViewModels.Chat;
using TripPlanner.ViewModels.Home;

namespace TripPlanner.Views.ChatViews;

public partial class ChatPage : ContentPage, IQueryAttributable
{
    private readonly NotificationViewModel m_NotificationViewModel;
    private readonly Configuration m_Configuration;
    private readonly ChatService m_ChatService;
    private readonly TourService m_TourService;
    private HubConnection m_Connection;
    private int TourId;
    int ParticipantCount;
    int CurrentLastViewElement;
    bool InsertMsg;

    ObservableRangeCollection<MessageDTO> Messages;

    public ChatPage(Configuration configuration, TourService tourService, NotificationViewModel notificationViewModel, ChatService chatService)
    {
        InitializeComponent();
        m_ChatService = chatService;
        m_Configuration = configuration;
        m_TourService = tourService;

        Messages = new ObservableRangeCollection<MessageDTO>();
        m_NotificationViewModel = notificationViewModel;
        MessagesList.ItemsSource = Messages;
        ParticipantCount = 0;
        CurrentLastViewElement = 0;
        InsertMsg = false;

        BindingContext = this;
    }

    public async void ApplyQueryAttributes(IDictionary<string, object> query)
    {
        TourId = (int)query["passTourId"];
        await LoadData();
        await Connect();
    }

    public void Scrolling(object obj, ItemsViewScrolledEventArgs args)
    {
        CurrentLastViewElement = args.LastVisibleItemIndex;
    }

    private async Task Connect()
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
                    if (msg.UserId == m_Configuration.User.Id)
                        MessagesList.ItemsUpdatingScrollMode = ItemsUpdatingScrollMode.KeepLastItemInView;
                    else
                    {
                        //jesli inny uzytkwonik dodal wiadomosc to podazaj za najnowszymi tylko wtedy kiedy czat jest zeskrolowany
                        //na sam do³ lub prawie czyli widoczne najnowsza lub 2 najnowsza wiadomoœæ
                        if(CurrentLastViewElement < Messages.Count - 3)
                            MessagesList.ItemsUpdatingScrollMode = ItemsUpdatingScrollMode.KeepScrollOffset;

                        if (Shell.Current.CurrentPage is ChatPage)
                        {
                            MainThread.BeginInvokeOnMainThread(() =>
                            {
                                string s = "";
                                if (msg.Content.Length >= 20)
                                    s = msg.Content.Substring(0, 20) + "...";
                                else
                                    s = msg.Content.Substring(0, msg.Content.Length);

                                var confirmCopyToast = Toast.Make($"{s}", ToastDuration.Short, 14);
                                confirmCopyToast.Show();
                            });
                        }
                    }

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
                        notificationDTO.Name = $"Nowe og³oszenie w wyjeŸdzie: ";
                        notificationDTO.Message = $"{sN} doda³ nowe og³oszenie o treœci \"{msg.Content}\", dodane: {msg.Date:hh:mm dd.MM.yyyy}";
                        notificationDTO.IconPath = "checklist_sec.png";
                        notificationDTO.Type = NotificationType.NotifyMessageAddedAlert;
                        await m_NotificationViewModel.SendNotifyToUsersOfTour(notificationDTO);
                    }
                    catch (Exception) { }

                    MessagesList.ItemsUpdatingScrollMode = ItemsUpdatingScrollMode.KeepLastItemInView;
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
                        notificationDTO.Name = $"Nowa ankieta w wyjeŸdzie: ";
                        notificationDTO.Message = $"Nowa ankieta o nazwie \"{msg.Content}\", z {msg.Answers.Count} odpowiedziami";
                        notificationDTO.IconPath = "checklist_sec.png";
                        notificationDTO.Type = NotificationType.QuestionnaireMessageAddedAlert;
                        await m_NotificationViewModel.SendNotifyToUsersOfTour(notificationDTO);
                    }
                    catch (Exception) { }

                    MessagesList.ItemsUpdatingScrollMode = ItemsUpdatingScrollMode.KeepLastItemInView;
                    Messages.Add(msg);
                }
            });

            m_Connection.On<string>("QuestionnaireVoteReceived", (message) =>
            {
                QuestionnaireDTO msg = JsonConvert.DeserializeObject<QuestionnaireDTO>(message);
                QuestionnaireDTO elem = (QuestionnaireDTO)Messages.FirstOrDefault(u => u.Id == msg.Id);
                elem.Answers = msg.Answers;
                elem.VoteForLabel = msg.VoteForLabel;
            });

            m_Connection.On<string>("GetOldestMessages", (message) =>
            {
                JsonSerializerSettings settings = new JsonSerializerSettings { TypeNameHandling = TypeNameHandling.All };
                List<MessageDTO> msgs = JsonConvert.DeserializeObject<List<MessageDTO>>(message, settings);

                MessagesList.ItemsUpdatingScrollMode = ItemsUpdatingScrollMode.KeepLastItemInView;
                Messages.AddRange(msgs);
            });

            m_Connection.On<string>("SetConnection", (message) =>
            {
                JsonSerializerSettings settings = new JsonSerializerSettings { TypeNameHandling = TypeNameHandling.All };
                List<MessageDTO> msgs = JsonConvert.DeserializeObject<List<MessageDTO>>(message, settings);

                Messages.AddRange(msgs.GetRange(0, msgs.Count - 1));
                MessagesList.ItemsUpdatingScrollMode = ItemsUpdatingScrollMode.KeepLastItemInView;
                Messages.Add(msgs.Last());
            });

            await m_Connection.StartAsync();
            await m_Connection.InvokeCoreAsync("SetConnection", args: new[] { TourId.ToString() });
        }
        catch (HubException)
        {
            await Shell.Current.CurrentPage.DisplayAlert("B³¹d", $"Nie uda³o wykonaæ operacji", "Ok");
        }
        catch (Exception)
        {
            await Shell.Current.CurrentPage.DisplayAlert("B³¹d", $"Nieznany b³¹d", "Ok");
        }
    }

    void OnCollectionViewRemainingItemsThresholdReached(object sender, EventArgs e)
    {

    }

    public void GoBack(object obj, EventArgs args)
    {
        m_Connection.InvokeCoreAsync("LeaveGroup", args: new[] { TourId.ToString() });

        var navigationParameter = new Dictionary<string, object>
            {
                { "passTourId",  TourId}
            };
        Shell.Current.GoToAsync($"Tour", navigationParameter);
    }

    public void SendTextMessage(object obj, EventArgs args)
    {
        try
        {
            //walidacja treœci wiadomoœci
            string msg2 = Message.Text.TrimStart().TrimEnd();

            if (string.IsNullOrEmpty(msg2))
                return;

            CreateTextMessageDTO msg = new CreateTextMessageDTO
            {
                Content = msg2,
                UserId = m_Configuration.User.Id,
                TourId = TourId
            };

            string json = JsonConvert.SerializeObject(msg);
            m_Connection.InvokeCoreAsync("SendTextMessage", args: new[] { json });
            Message.Text = String.Empty;
        }
        catch (HubException)
        {
            Shell.Current.CurrentPage.DisplayAlert("B³¹d", "Nie uda³o siê wys³aæ wiadomoœci", "Ok");
        }
        catch (Exception)
        {
            Shell.Current.CurrentPage.DisplayAlert("B³¹d", $"Nieznany b³¹d", "Ok");
        }
    }

    public void SendNoticeMessage(object obj, EventArgs args)
    {
        try
        {
            //walidacja treœci wiadomoœci
            string msg2 = Message.Text.TrimStart().TrimEnd();

            if (string.IsNullOrEmpty(msg2))
                return;

            CreateNoticeMessageDTO msg = new CreateNoticeMessageDTO
            {
                Content = msg2,
                UserId = m_Configuration.User.Id,
                TourId = TourId
            };

            string json = JsonConvert.SerializeObject(msg);
            m_Connection.InvokeCoreAsync("SendNoticeMessage", args: new[] { json });
            Message.Text = String.Empty;
        }
        catch (HubException ex)
        {
            Shell.Current.CurrentPage.DisplayAlert("B³¹d", $"{ex.Message}", "Ok");
        }
        catch (Exception)
        {
            Shell.Current.CurrentPage.DisplayAlert("B³¹d", $"Nieznany b³¹d", "Ok");
        }
    }

    public void ShowMoreChatAction(object obj, EventArgs args)
    {
        Shell.Current.CurrentPage.ShowPopupAsync(new ChatAdditionalMenuPopup(m_TourService, TourId));
    }

    public void Vote(object obj, EventArgs args)
    {
        try
        {
            View button = null;
            if (obj is ImageButton) button = (ImageButton)obj;
            else                    button = (Button)obj;
            var item = (QuestionnaireAnswerDTO)button.BindingContext;

            if (item.Votes.Any(u => u.UserId == m_Configuration.User.Id))
                return;

            CreateQuestionnaireVoteDTO msg = new CreateQuestionnaireVoteDTO
            {
                QuestionnaireId = item.QuestionnaireId,
                AnswerId = item.Id,
                TourId = TourId,
                UserId = m_Configuration.User.Id
            };

            string json = JsonConvert.SerializeObject(msg);
            m_Connection.InvokeCoreAsync("SendQuestionnaireVote", args: new[] { json });
            var confirmCopyToast = Toast.Make($"Oddano g³os", ToastDuration.Long, 14);
            confirmCopyToast.Show();
        }
        catch (HubException ex)
        {
            Shell.Current.CurrentPage.DisplayAlert("B³¹d", $"{ex.Message}", "Ok");
        }
        catch (Exception)
        {
            Shell.Current.CurrentPage.DisplayAlert("B³¹d", $"Nieznany b³¹d", "Ok");
        }
    }

    public async void ShowVoters(object obj, EventArgs args)
    {
        var button = (ImageButton)obj;
        var item = (QuestionnaireAnswerDTO)button.BindingContext;

        var res = await m_ChatService.GetAnswerVoters(item.Id, TourId);
        Shell.Current.CurrentPage.ShowPopupAsync(new PeopleChatListPopups($"Zag³osowali na \"{item.Answer}\"", res));
    }


    private async Task LoadData()
    {
        TourDTO tour = await m_TourService.GetTourWithParticipants(TourId);
        ParticipantCount = tour.Participants.Count;
        ParticipantCountLabel.Text = tour.Participants.Count.ToString();
    }
}