using Newtonsoft.Json;
using System.Net.Http.Json;
using System.Text;
using TripPlanner.Models.DTO.MessageDTOs.QuestionnaireDTOs;
using TripPlanner.Models.DTO.TourDTOs;
using TripPlanner.Models.Models;
using TripPlanner.Models.Models.MessageModels.QuestionnaireModels;

namespace TripPlanner.Services
{
    public class ChatService
    {
        private readonly HttpClient m_HttpClient;
        private readonly Configuration m_Configuration;

        public ChatService(IHttpClientFactory httpClient, Configuration configuration)
        {
            m_HttpClient = httpClient.CreateClient("httpClient");
            m_Configuration = configuration;
        }

        //Zwraca wszystkie oddane głosy na odpowiedź o danym id, ankiety o danym id
        public async Task<List<string>> GetAnswerVoters(int answerId, int tourId)
        {
            try
            {
                HttpResponseMessage response = m_HttpClient.GetAsync($"{m_Configuration.WebApiUrl}/Questionnaire/{answerId}/votes/{tourId}").Result;
                if (response.IsSuccessStatusCode)
                {
                    return await response.Content.ReadFromJsonAsync<List<string>>();
                }
            }
            catch (Exception) { }
            return null;
        }

        //Zwraca wszystkie oddane głosy na odpowiedź o danym id, ankiety o danym id
        public async Task<List<QuestionnaireDTO>> GetQuestionnairesFromTour(int tourId)
        {
            try
            {
                HttpResponseMessage response = m_HttpClient.GetAsync($"{m_Configuration.WebApiUrl}/Questionnaire/getFromTour/{tourId}").Result;
                if (response.IsSuccessStatusCode)
                {
                    var r = await response.Content.ReadFromJsonAsync<List<QuestionnaireDTO>>();
                    return r;
                }
            }
            catch (Exception) { }
            return null;
        }


        //Tworzenie nowej ankiety
        public async Task<RepositoryResponse<bool>> CreateQuestionnaire(CreateQuestionnaireDTO questionnaire)
        {
            string errMsg = "";
            try
            {
                string json = JsonConvert.SerializeObject(questionnaire);
                StringContent httpContent = new StringContent(json, Encoding.UTF8, "application/json");
                HttpResponseMessage response = m_HttpClient.PostAsync($"{m_Configuration.WebApiUrl}/Questionnaire/create", httpContent).Result;
                if (response.IsSuccessStatusCode)
                {
                    var resp = await response.Content.ReadFromJsonAsync<RepositoryResponse<bool>>();
                    if (resp.Success)
                        return new RepositoryResponse<bool> { Data = false, Message = "", Success = true };
                    else
                        errMsg = resp.Message;
                }
                else
                    errMsg = $"Kod błędu: {response.StatusCode}";
            }
            catch (Exception e)
            {
                if (e.InnerException is TaskCanceledException)
					errMsg = "Przepraszamy, serwer poza zasięgiem, sprawdź swoje połączenie internetowe lub spróbuj później";
				else
					errMsg = $"Wyjątek: {e.Message}";
            }
            return new RepositoryResponse<bool> { Data = false, Message = errMsg, Success = false };
        }

        //Zagłosuj na daną odpowiedź
        public async Task<RepositoryResponse<bool>> VoteForAnswer(int userId, int answerId)
        {
            string errMsg = "";
            try
            {
                QuestionnaireVoteDTO vote = new QuestionnaireVoteDTO 
                {
                    UserId = userId,
                    QuestionnaireAnswerId = answerId
                };
                string json = JsonConvert.SerializeObject(vote);
                StringContent httpContent = new StringContent(json, Encoding.UTF8, "application/json");
                HttpResponseMessage response = m_HttpClient.PostAsync($"{m_Configuration.WebApiUrl}/Questionnaire/addVote", httpContent).Result;
                if (response.IsSuccessStatusCode)
                {
                    var resp = await response.Content.ReadFromJsonAsync<RepositoryResponse<bool>>();
                    if (resp.Success)
                        return new RepositoryResponse<bool> { Data = true, Message = "", Success = true };
                    else
                        errMsg = resp.Message;
                }
                else
                    errMsg = $"Kod błędu: {response.StatusCode}";
            }
            catch (Exception e)
            {
                if (e.InnerException is TaskCanceledException)
					errMsg = "Przepraszamy, serwer poza zasięgiem, sprawdź swoje połączenie internetowe lub spróbuj później";
				else
					errMsg = $"Wyjątek: {e.Message}";
            }
            return new RepositoryResponse<bool> { Data = false, Message = errMsg, Success = false };
        }
    }
}
