using Newtonsoft.Json;
using System.Net.Http.Json;
using System.Text;
using TripPlanner.Models.DTO.CheckListDTOs;
using TripPlanner.Models.Models;

namespace TripPlanner.Services
{
    public class CheckListService
    {
        private readonly HttpClient m_HttpClient;
        private readonly Configuration m_Configuration;

        public CheckListService(IHttpClientFactory httpClient, Configuration configuration)
        {
            m_HttpClient = httpClient.CreateClient("httpClient");
            m_Configuration = configuration;
        }



        // Zwraca checkliste o danym id
        public async Task<CheckListDTO> GetCheckListById(int checkListId)
        {
            try
            {
                HttpResponseMessage response = m_HttpClient.GetAsync($"{m_Configuration.WebApiUrl}/CheckList/getWithFields/{checkListId}").Result;
                if (response.IsSuccessStatusCode)
                {
                    return await response.Content.ReadFromJsonAsync<CheckListDTO>();
                }
            }
            catch (Exception) { }
            return null;
        }


        // Zwraca checklisty z wyjazdu o danym id
        public async Task<List<CheckListDTO>> GetCheckListFromTour(int tourId)
        {
            try
            {
                HttpResponseMessage response = m_HttpClient.GetAsync($"{m_Configuration.WebApiUrl}/CheckList/getFromTour/{tourId}").Result;
                if (response.IsSuccessStatusCode)
                {
                    return await response.Content.ReadFromJsonAsync<List<CheckListDTO>>();
                }
            }
            catch (Exception) { }
            return null;
        }


        // Tworzenie nowej checklisty
        public async Task<RepositoryResponse<int>> CreateCheckList(CreateCheckListDTO checkList)
        {
            string errMsg = "";
            try
            {
                string json = JsonConvert.SerializeObject(checkList);
                StringContent httpContent = new StringContent(json, Encoding.UTF8, "application/json");
                HttpResponseMessage response = m_HttpClient.PostAsync($"{m_Configuration.WebApiUrl}/CheckList/createCheckList", httpContent).Result;
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

        // Dodaje pole do checklisty
        public async Task<RepositoryResponse<bool>> AddCheckListField(CreateCheckListFieldDTO createCheckListField, int userId)
        {
            string errMsg = "";
            try
            {
                string json = JsonConvert.SerializeObject(createCheckListField);
                StringContent httpContent = new StringContent(json, Encoding.UTF8, "application/json");
                HttpResponseMessage response = m_HttpClient.PostAsync($"{m_Configuration.WebApiUrl}/CheckList/addCheckListField/{userId}", httpContent).Result;
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

        // Edycja checklisty (zmiana nazwy lub zmiana dostepnosci lub obie te rzeczy)
        public async Task<RepositoryResponse<bool>> UpdateCheckList(int CheckListId, int userId, EditCheckListDTO editCheckListDTO)
        {
            string errMsg = "";
            try
            {
                string json = JsonConvert.SerializeObject(editCheckListDTO);
                StringContent httpContent = new StringContent(json, Encoding.UTF8, "application/json");
                HttpResponseMessage response = m_HttpClient.PutAsync($"{m_Configuration.WebApiUrl}/CheckList/{CheckListId}/{userId}", httpContent).Result;
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

        // Usuwa pole w checkliscie
        public async Task<RepositoryResponse<bool>> DeleteCheckListField(int CheckListId, int CheckListFieldId, int userId)
        {
            string errMsg = "";
            try
            {
                HttpResponseMessage response = m_HttpClient.DeleteAsync($"{m_Configuration.WebApiUrl}/CheckList/{CheckListId}/deleteCheckListField/{CheckListFieldId}/{userId}").Result;
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

        // Usuwa checkliste
        public async Task<RepositoryResponse<bool>> DeleteCheckList(int CheckListId, int userId)
        {
            string errMsg = "";
            try
            {
                HttpResponseMessage response = m_HttpClient.DeleteAsync($"{m_Configuration.WebApiUrl}/CheckList/{CheckListId}/{userId}").Result;
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
