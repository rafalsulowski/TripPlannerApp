using Newtonsoft.Json;
using System.Net.Http.Json;
using System.Text;
using TripPlanner.Models.DTO.ScheduleDTOs;
using TripPlanner.Models.DTO.TourDTOs;
using TripPlanner.Models.DTO.UserDTOs;
using TripPlanner.Models.Models;
using TripPlanner.Models.Models.ScheduleModels;

namespace TripPlanner.Services
{
    public class NotificationService
    {
        private readonly HttpClient m_HttpClient;
        private readonly Configuration m_Configuration;

        public NotificationService(IHttpClientFactory httpClient, Configuration configuration)
        {
            m_HttpClient = httpClient.CreateClient("httpClient");
            m_Configuration = configuration;
        }


        // Zwraca powaidomienie o danym id
        public async Task<NotificationDTO> GetNotificationById(int id)
        {
            try
            {
                HttpResponseMessage response = m_HttpClient.GetAsync($"{m_Configuration.WebApiUrl}/User/getNotificationById/{id}").Result;
                if (response.IsSuccessStatusCode)
                {
                    return await response.Content.ReadFromJsonAsync<NotificationDTO>();
                }
            }
            catch (Exception) { }
            return null;
        }
    }
}
