using Newtonsoft.Json;
using System.Net.Http.Json;
using System.Text;
using TripPlanner.Models.DTO.TourDTOs;
using TripPlanner.Models.Models;

namespace TripPlanner.Services
{
    public class TourService
    {
        private readonly HttpClient m_HttpClient;
        private readonly Configuration m_Configuration;

        public TourService(IHttpClientFactory httpClient, Configuration configuration)
        {
            m_HttpClient = httpClient.CreateClient("httpClient");
            m_Configuration = configuration;
        }

        
        // Zwraca wyjazd o danym id
        public async Task<TourDTO> GetTourToPdfById(int tourId)
        {
            try
            {
                HttpResponseMessage response = m_HttpClient.GetAsync($"{m_Configuration.WebApiUrl}/Tour/{tourId}/getTourToPdfById").Result;
                if (response.IsSuccessStatusCode)
                {
                    return await response.Content.ReadFromJsonAsync<TourDTO>();
                }
            }
            catch (Exception) { }
            return null;
        }

        // Zwraca wyjazd o danym id
        public async Task<TourDTO> GetTourById(int tourId)
        {
            try
            {
                HttpResponseMessage response = m_HttpClient.GetAsync($"{m_Configuration.WebApiUrl}/Tour/{tourId}").Result;
                if (response.IsSuccessStatusCode)
                {
                    return await response.Content.ReadFromJsonAsync<TourDTO>();
                }
            }
            catch (Exception) { }
            return null;
        }

        // Zwraca wyjazd o danym id z wiadomościami z chatu
        public async Task<TourDTO> GetTourWithMessages(int tourId)
        {
            try
            {
                HttpResponseMessage response = m_HttpClient.GetAsync($"{m_Configuration.WebApiUrl}/Tour/{tourId}/getWithMessages").Result;
                if (response.IsSuccessStatusCode)
                {
                    return await response.Content.ReadFromJsonAsync<TourDTO>();
                }
            }
            catch (Exception) { }
            return null;
        }

        // Zwraca wyjazd o danym id z uczestnikami (podstawowe informacje)
        public async Task<TourDTO> GetTourWithParticipants(int tourId)
        {
            try
            {
                HttpResponseMessage response = m_HttpClient.GetAsync($"{m_Configuration.WebApiUrl}/Tour/{tourId}/getWithParticipants").Result;
                if (response.IsSuccessStatusCode)
                {
                    return await response.Content.ReadFromJsonAsync<TourDTO>();
                }
            }
            catch (Exception) { }
            return null;
        }

        // Zwraca listę z wszelkimi potrzebnymi informacjami o wszystkich uczestnikach
        // Zwraca listę ExtendParticipantDTO, uzupełniając wszystkie infromacje
        public async Task<List<ExtendParticipantDTO>> GetTourExtendParticipant(int tourId)
        {
            try
            {
                HttpResponseMessage response = m_HttpClient.GetAsync($"{m_Configuration.WebApiUrl}/Tour/{tourId}/getExtendParticipants").Result;
                if (response.IsSuccessStatusCode)
                {
                    return await response.Content.ReadFromJsonAsync<List<ExtendParticipantDTO>>();
                }
            }
            catch (Exception) { }
            return null;
        }

        // Zwraca wszelkie potrzebne informacje o uczestniku o danym id
        public async Task<ExtendParticipantDTO> GetTourExtendParticipantById(int tourId, int userId)
        {
            try
            {
                HttpResponseMessage response = m_HttpClient.GetAsync($"{m_Configuration.WebApiUrl}/Tour/{tourId}/getExtendParticipantsById/{userId}").Result;
                if (response.IsSuccessStatusCode)
                {
                    return await response.Content.ReadFromJsonAsync<ExtendParticipantDTO>();
                }
            }
            catch (Exception) { }
            return null;
        }

        // Zwraca listę imion, nazwisk oraz nikców, wszystkich uczestników
        // Zwraca listę ExtendParticipantDTO, jednakże nie uzupełnia wszystkich
        // informacji klasy tylko: Fullname oraz Nickname
        public async Task<List<ExtendParticipantDTO>> GetTourParticipantsNames(int tourId)
        {
            try
            {
                HttpResponseMessage response = m_HttpClient.GetAsync($"{m_Configuration.WebApiUrl}/Tour/{tourId}/getParticipantsNames").Result;
                if (response.IsSuccessStatusCode)
                {
                    return await response.Content.ReadFromJsonAsync<List<ExtendParticipantDTO>>();
                }
            }
            catch (Exception) { }
            return null;
        }

        // Zwraca true jesli uzytkownik o userId jest organizatorem wycieczki o tourId
        public async Task<bool> IsUserOrganizerTour(int tourId, int userId)
        {
            try
            {
                HttpResponseMessage response = m_HttpClient.GetAsync($"{m_Configuration.WebApiUrl}/Tour/{tourId}/participant/{userId}").Result;
                if (response.IsSuccessStatusCode)
                {
                    ParticipantTourDTO obj = await response.Content.ReadFromJsonAsync<ParticipantTourDTO>();
                    return obj.IsOrganizer;
                }
            }
            catch (Exception) { }
            return false;
        }


        // Tworzenie nowego wyjazdu
        public async Task<RepositoryResponse<int>> CreateTour(CreateTourDTO tour)
        {
            string errMsg = "";
            try
            {
                string json = JsonConvert.SerializeObject(tour);
                StringContent httpContent = new StringContent(json, Encoding.UTF8, "application/json");
                HttpResponseMessage response = m_HttpClient.PostAsync($"{m_Configuration.WebApiUrl}/Tour", httpContent).Result;
                if (response.IsSuccessStatusCode)
                {
                    RepositoryResponse<int> resp = await response.Content.ReadFromJsonAsync<RepositoryResponse<int>>();
                    if (resp.Success)
                        return new RepositoryResponse<int> { Data = resp.Data, Message = "", Success = true };
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
            return new RepositoryResponse<int> { Data = -1, Message = errMsg, Success = false };
        }

        // Dodaje uczestnika
        public async Task<RepositoryResponse<bool>> AddParticipant(int tourId, int userId)
        {
            string errMsg = "";
            try
            {
                ParticipantTourDTO participant = new ParticipantTourDTO
                {
                    UserId = userId,
                    TourId = tourId,
                    IsOrganizer = false,
                    AccessionDate = DateTime.Now,
                    Nickname = ""
                };

                string json = JsonConvert.SerializeObject(participant);
                StringContent httpContent = new StringContent(json, Encoding.UTF8, "application/json");
                HttpResponseMessage response = m_HttpClient.PostAsync($"{m_Configuration.WebApiUrl}/Tour/addParticipant", httpContent).Result;
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

        // Zmiana ksywki uczestnika
        public async Task<RepositoryResponse<bool>> UpdateParticipantNickname(int tourId, int userId, string newNick)
        {
            string errMsg = "";
            try
            {
                string json = JsonConvert.SerializeObject(newNick);
                StringContent httpContent = new StringContent(json, Encoding.UTF8, "application/json");
                HttpResponseMessage response = m_HttpClient.PutAsync($"{m_Configuration.WebApiUrl}/Tour/{tourId}/updateParticipantNickname/{userId}", httpContent).Result;
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
        public async Task<RepositoryResponse<bool>> DeleteParticipant(int tourId, int userId)
        {
            string errMsg = "";
            try
            {
                HttpResponseMessage response = m_HttpClient.DeleteAsync($"{m_Configuration.WebApiUrl}/Tour/{tourId}/deleteParticipant/{userId}").Result;
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
        public async Task<RepositoryResponse<bool>> AddOrganizer(int tourId, int userId)
        {
            string errMsg = "";
            try
            {
                HttpResponseMessage response = m_HttpClient.PutAsync($"{m_Configuration.WebApiUrl}/Tour/{tourId}/makeOrganizer/{userId}", null).Result;
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

        // Usuwa organizatora
        public async Task<RepositoryResponse<bool>> DeleteOrganizer(int tourId, int userId)
        {
            string errMsg = "";
            try
            {
                HttpResponseMessage response = m_HttpClient.PutAsync($"{m_Configuration.WebApiUrl}/Tour/{tourId}/deleteOrganizer/{userId}", null).Result;
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

        // Zmiana lokalziacji dla pogody
        public async Task<RepositoryResponse<bool>> UpdateWeatherCords(int tourId, string loc)
        {
            string errMsg = "";
            try
            {
                string json = JsonConvert.SerializeObject(loc);
                StringContent httpContent = new StringContent(json, Encoding.UTF8, "application/json");
                HttpResponseMessage response = m_HttpClient.PutAsync($"{m_Configuration.WebApiUrl}/Tour/{tourId}/updateWeatherCords", httpContent).Result;
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
