using Newtonsoft.Json;
using System.Net.Http.Json;
using System.Text;
using TripPlanner.Models.DTO.ScheduleDTOs;
using TripPlanner.Models.DTO.TourDTOs;
using TripPlanner.Models.Models;
using TripPlanner.Models.Models.ScheduleModels;

namespace TripPlanner.Services
{
    public class ScheduleService
    {
        private readonly HttpClient m_HttpClient;
        private readonly Configuration m_Configuration;

        public ScheduleService(IHttpClientFactory httpClient, Configuration configuration)
        {
            m_HttpClient = httpClient.CreateClient("httpClient");
            m_Configuration = configuration;
        }


        // Zwraca harmonogram
        public async Task<List<ScheduleDayDTO>> GetSchedule(int tourId)
        {
            try
            {
                HttpResponseMessage response = m_HttpClient.GetAsync($"{m_Configuration.WebApiUrl}/Schedule/getSchedule/{tourId}").Result;
                if (response.IsSuccessStatusCode)
                {
                    return await response.Content.ReadFromJsonAsync<List<ScheduleDayDTO>>();
                }
            }
            catch (Exception) { }
            return null;
        }

        // Zwraca caly obiekt dnia z harmonogramu
        public async Task<ScheduleDayDTO> GetScheduleDay(int scheduleDayId)
        {
            try
            {
                HttpResponseMessage response = m_HttpClient.GetAsync($"{m_Configuration.WebApiUrl}/Schedule/getScheduleDay/{scheduleDayId}").Result;
                if (response.IsSuccessStatusCode)
                {
                    return await response.Content.ReadFromJsonAsync<ScheduleDayDTO>();
                }
            }
            catch (Exception) { }
            return null;
        }


        // Tworzenie nowego wydarzenia podczas dnia
        public async Task<RepositoryResponse<bool>> CreateScheduleEvent(CreateScheduleEventDTO newEvent)
        {
            string errMsg = "";
            try
            {
                string json = JsonConvert.SerializeObject(newEvent);
                StringContent httpContent = new StringContent(json, Encoding.UTF8, "application/json");
                HttpResponseMessage response = m_HttpClient.PostAsync($"{m_Configuration.WebApiUrl}/Schedule/scheduleEvent", httpContent).Result;
                if (response.IsSuccessStatusCode)
                {
                    RepositoryResponse<bool> resp = await response.Content.ReadFromJsonAsync<RepositoryResponse<bool>>();
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

        // Usuwa uczestnika
        public async Task<RepositoryResponse<bool>> UpdateScheduleEvent(int ScheduleDayId, int ScheduleEventId, int userId, EditScheduleEventDTO editEvent)
        {
            string errMsg = "";
            try
            {
                string json = JsonConvert.SerializeObject(editEvent);
                StringContent httpContent = new StringContent(json, Encoding.UTF8, "application/json");
                HttpResponseMessage response = m_HttpClient.PutAsync($"{m_Configuration.WebApiUrl}/Schedule/{ScheduleDayId}/editEvent/{ScheduleEventId}/{userId}", httpContent).Result;
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

        // Usuwa uczestnika
        public async Task<RepositoryResponse<bool>> DeleteScheduleEvent(int ScheduleDayId, int ScheduleEventId, int userId)
        {
            string errMsg = "";
            try
            {
                HttpResponseMessage response = m_HttpClient.DeleteAsync($"{m_Configuration.WebApiUrl}/Schedule/{ScheduleDayId}/deleteEvent/{ScheduleEventId}/{userId}").Result;
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
