using CommunityToolkit.Maui.Alerts;
using CommunityToolkit.Maui.Core;
using CommunityToolkit.Maui.Views;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using TripPlanner.Controls.QuestionnaireControls;
using TripPlanner.Models.DTO.MessageDTOs.QuestionnaireDTOs;
using TripPlanner.Services;
using TripPlanner.Views.ChatViews;

namespace TripPlanner.ViewModels.Chat
{
    public partial class QuestionnaireViewModel : ObservableObject
    {
        private readonly Configuration m_Configuration;
        private readonly ChatService m_ChatService;

        [ObservableProperty]
        QuestionnaireDTO questionnaire;

        public QuestionnaireViewModel(Configuration configuration, ChatService chatService, QuestionnaireDTO questionnaire)
        {
            m_Configuration = configuration;
            m_ChatService = chatService;
            Questionnaire = questionnaire;
        }


        [RelayCommand]
        async Task Vote(AnswerGDTO answer)
        {
            var res = await m_ChatService.VoteForAnswer(m_Configuration.User.Id, answer.Id);
            if (res.Success)
            {
                //jakaś zmiana w interfejsie uzytkownika, odświerzenie ankiety
                var confirmCopyToast = Toast.Make($"Oddano swój głos", ToastDuration.Long, 14);
                await confirmCopyToast.Show();
            }
            else
                await Shell.Current.CurrentPage.DisplayAlert("Błąd", res.Message, "Ok");
        }
    }
}
