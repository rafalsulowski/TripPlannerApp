using CommunityToolkit.Maui.Alerts;
using CommunityToolkit.Maui.Core;
using CommunityToolkit.Maui.Views;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.AspNetCore.SignalR;
using Microsoft.AspNetCore.SignalR.Client;
using Newtonsoft.Json;
using System.Collections.ObjectModel;
using TripPlanner.Controls.QuestionnaireControls;
using TripPlanner.Models.DTO.MessageDTOs;
using TripPlanner.Models.DTO.MessageDTOs.QuestionnaireDTOs;
using TripPlanner.Services;
using TripPlanner.Views.ChatViews;

namespace TripPlanner.ViewModels.CheckList
{
    public partial class QuestionnaireStandAloneViewModel : ObservableObject, IQueryAttributable
    {
        private readonly Configuration m_Configuration;
        private readonly ChatService m_ChatService;
        private HubConnection m_Connection;
        private int TourId;
        private int QuestionnaireId;

        [ObservableProperty]
        QuestionnaireDTO questionnaireDto;

        [ObservableProperty]
        ObservableCollection<AnswerGDTO> answers;

        [ObservableProperty]
        string voteForLabel;

        public QuestionnaireStandAloneViewModel(Configuration configuration, ChatService chatService)
        {
            m_Configuration = configuration;
            m_ChatService = chatService;
            VoteForLabel = "";
            Answers = new ObservableCollection<AnswerGDTO>();
            QuestionnaireDto = new QuestionnaireDTO();
        }

        public async void ApplyQueryAttributes(IDictionary<string, object> query)
        {
            TourId = (int)query["passTourId"];
            QuestionnaireId = (int)query["passQuestionnaireId"];
            await Connect();
        }

        async Task Connect()
        {
            try
            {
                m_Connection = new HubConnectionBuilder()
                .WithUrl(m_Configuration.WssChatUrl)
                .Build();

                //gdy ktos zglosuje na questionnaire answer
                m_Connection.On<string>("QuestionnaireVoteReceived", async (message) =>
                {
                    QuestionnaireDTO msg = JsonConvert.DeserializeObject<QuestionnaireDTO>(message);
                    if (msg != null && msg.Id == QuestionnaireId)
                    {
                        QuestionnaireDto = msg;
                        CalculateShares();

                        var confirmCopyToast = Toast.Make($"Zaaktualizowano głosy", ToastDuration.Short, 14);
                        await confirmCopyToast.Show();
                    }
                });

                //gdy ktos usunie questionnaire
                m_Connection.On<string>("QuestionnaireDeleteReceived", (message) =>
                {
                    int msg = JsonConvert.DeserializeObject<int>(message);
                    if (msg == QuestionnaireId)
                    {
                        MainThread.BeginInvokeOnMainThread(() => Shell.Current.CurrentPage.DisplayAlert("Uwaga", $"Ankieta została usunięta", "Ok"));
                        MainThread.BeginInvokeOnMainThread(GoBackSynch);
                    }
                });

                //odebranie info o quiestionnaire i zarejestrowanie sie do grupy zwiazanej z chatem
                //(jesli kots zaglosuje na questionnaire na chatcie to tez tutaj otrzyma sygnal)
                m_Connection.On<string>("SetConnection", (message) =>
                {
                    QuestionnaireDTO msgs = JsonConvert.DeserializeObject<QuestionnaireDTO>(message);
                    if(msgs.Id == QuestionnaireId)
                    {
                        QuestionnaireDto = msgs;
                        CalculateShares();
                    }
                });

                await m_Connection.StartAsync();
                await m_Connection.InvokeCoreAsync("SetConnectionquestionnaireStandAlone", args: new[] { QuestionnaireId.ToString() });
            }
            catch (HubException)
            {
                await Shell.Current.CurrentPage.DisplayAlert("Błąd", $"Nie udało wykonać operacji", "Ok");
            }
            catch (Exception)
            {
                await Shell.Current.CurrentPage.DisplayAlert("Błąd", $"Nieznany błąd", "Ok");
            }
        }
        void GoBackSynch()
        {
            GoBack();
        }

        private void CalculateShares()
        {
            int votesSum = QuestionnaireDto.Answers.Sum(item => item.Votes.Count);
            VoteForLabel = "Nie oddałeś głosu";

            Answers.Clear();
            foreach (var answer in QuestionnaireDto.Answers)
            {
                var resp = Answers.FirstOrDefault(u => u.Id == answer.Id);
                if (resp != null)
                { //jesli juz isniteje odpowidz w tabeli to tylko ja modyfikuj (szybsza reakcja na interfejsie uzytkwonika)
                    resp.Answer = "test";
                }
                else
                {
                    AnswerGDTO ans = new AnswerGDTO();
                    ans.QuestionnaireId = answer.QuestionnaireId;
                    ans.Id = answer.Id;
                    ans.Answer = answer.Answer;
                    ans.AccurateIcon = "circle_sec.png";
                    ans.PercentageShare = (double)answer.Votes.Count / votesSum;

                    foreach (QuestionnaireVoteDTO vote in answer.Votes)
                    {
                        if (vote.UserId == m_Configuration.User.Id)
                        {
                            VoteForLabel = $"Zagłosowałeś na \"{ans.Answer}\"";
                            ans.AccurateIcon = "circle_ok_sec.png";
                        }
                    }
                    Answers.Add(ans);
                }
            }
        }

        [RelayCommand]
        async Task GoBack()
        {
            await m_Connection.InvokeCoreAsync("LeaveGroup", args: new[] { TourId.ToString() });

            var navigationParameter = new Dictionary<string, object>
            {
                { "passTourId",  TourId},
            };
            await Shell.Current.GoToAsync($"Tour/CheckLists", navigationParameter);
        }

        [RelayCommand]
        async Task Vote(AnswerGDTO answer)
        {
            try
            {
                CreateQuestionnaireVoteDTO msg = new CreateQuestionnaireVoteDTO
                {
                    QuestionnaireId = QuestionnaireId,
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
        async Task Delete()
        {
            var res = await Shell.Current.CurrentPage.DisplayAlert("Uwaga", "Czy na pewno chesz usunąć ankietę?", "Tak", "Nie");
            if (!res)
                return;

            try
            {
                await m_Connection.InvokeCoreAsync("QuestionnaireDelete", args: new[] { QuestionnaireId.ToString() });
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
