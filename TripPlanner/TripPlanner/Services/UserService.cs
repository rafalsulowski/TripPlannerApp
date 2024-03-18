using Microsoft.Maui.Graphics.Text;
using Newtonsoft.Json;
using System.Net;
using System.Net.Http.Json;
using System.Text;
using TripPlanner.Models.DTO.BillDTOs;
using TripPlanner.Models.DTO.TourDTOs;
using TripPlanner.Models.DTO.UserDTOs;
using TripPlanner.Models.Models;
using TripPlanner.Models.Models.UserModels;

namespace TripPlanner.Services
{
    public class UserService
    {
        private readonly HttpClient m_HttpClient;
        private readonly Configuration m_Configuration;

        public UserService(IHttpClientFactory httpClient, Configuration configuration)
        {
            m_HttpClient = httpClient.CreateClient("httpClient");
            m_Configuration = configuration;
        }

        
        // Zwraca uczestnika po danym emailu
        public async Task<UserDTO> GetUserByEmial(string email)
        {
            try
            {
                HttpResponseMessage response = m_HttpClient.GetAsync($"{m_Configuration.WebApiUrl}/User/email/{email}").Result;
                if (response.IsSuccessStatusCode)
                {
                    return await response.Content.ReadFromJsonAsync<UserDTO>();
                }
            }
            catch (Exception) { }
            return null;
        }

        // Zwraca liste uzytkownikow aplikacji
        public async Task<List<ExtendUserDTO>> GetUsers(int userId)
        {
            try
            {
                HttpResponseMessage response = m_HttpClient.GetAsync($"{m_Configuration.WebApiUrl}/User/extendUserDto/{userId}").Result;
                if (response.IsSuccessStatusCode)
                {
                    return await response.Content.ReadFromJsonAsync<List<ExtendUserDTO>>();
                }
            }
            catch (Exception) { }
            return null;
        }

        // Zwraca liste znajomych
        public async Task<List<ExtendFriendDTO>> GetFriends(int userId)
        {
            try
            {
                HttpResponseMessage response = m_HttpClient.GetAsync($"{m_Configuration.WebApiUrl}/User/{userId}/GetFriends").Result;
                if (response.IsSuccessStatusCode)
                {
                    return await response.Content.ReadFromJsonAsync<List<ExtendFriendDTO>>();
                }
            }
            catch (Exception) { }
            return null;
        }

        // Zwraca liste wyjazdów do których należy dany użytkownik
        public async Task<List<TourDTO>> GetToursOfUser(int userId)
        {
            try
            {
                HttpResponseMessage response = m_HttpClient.GetAsync($"{m_Configuration.WebApiUrl}/User/GetToursOfUser/{userId}").Result;
                if (response.IsSuccessStatusCode)
                {
                    return await response.Content.ReadFromJsonAsync<List<TourDTO>>();
                }
            }
            catch (Exception) { }
            return null;
        }

        // Zwraca liste znajomych z wyjazdu
        public async Task<List<ExtendFriendDTO>> GetFriendsWithInfoAboutTour(int userId, int tourId)
        {
            try
            {
                HttpResponseMessage response = m_HttpClient.GetAsync($"{m_Configuration.WebApiUrl}/User/{userId}/GetFriendsWithInfoAboutTour/{tourId}").Result;
                if (response.IsSuccessStatusCode)
                {
                    return await response.Content.ReadFromJsonAsync<List<ExtendFriendDTO>>();
                }
            }
            catch (Exception) { }
            return null;
        }

        // Zwraca bool czy taki email jest juz zarejestrowany
        public async Task<bool> EmailIsFree(string emial)
        {
            try
            {
                string json = JsonConvert.SerializeObject(emial);
                StringContent httpContent = new StringContent(json, Encoding.UTF8, "application/json");
                HttpResponseMessage response = m_HttpClient.PostAsync($"{m_Configuration.WebApiUrl}/User/emailIsFree", httpContent).Result;
                if (response.IsSuccessStatusCode)
                {
                    return await response.Content.ReadFromJsonAsync<bool>();
                }
            }
            catch (Exception) { }
            return false;
        }
        
        
        // Logowanie
        public async Task<RepositoryResponse<string>> Login(string emial, string password)
        {
            string errMsg = "";
            try
            {
                LoginDTO loginDto = new LoginDTO
                {
                    Email = emial,
                    PasswordHash = password
                };

                string json = JsonConvert.SerializeObject(loginDto);
                StringContent httpContent = new StringContent(json, Encoding.UTF8, "application/json");
                HttpResponseMessage response = m_HttpClient.PostAsync($"{m_Configuration.WebApiUrl}/User/login", httpContent).Result;
                if (response.IsSuccessStatusCode)
                {
                    RepositoryResponse<string> resp = await response.Content.ReadFromJsonAsync<RepositoryResponse<string>>();
                    if (resp.Success)
                        return new RepositoryResponse<string> { Data = resp.Data, Message = "", Success = true };
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
            return new RepositoryResponse<string> { Data = "", Message = errMsg, Success = false };
        }

        // Dodawanie znajomego
        public async Task<RepositoryResponse<bool>> AddFriend(FriendDTO firend)
        {
            string errMsg = "";
            try
            {
                string json = JsonConvert.SerializeObject(firend);
                StringContent httpContent = new StringContent(json, Encoding.UTF8, "application/json");
                HttpResponseMessage response = m_HttpClient.PostAsync($"{m_Configuration.WebApiUrl}/User/addFriend", httpContent).Result;
                if (response.IsSuccessStatusCode)
                {
                    RepositoryResponse<bool> resp = await response.Content.ReadFromJsonAsync<RepositoryResponse<bool>>();
                    if (resp.Success)
                        return new RepositoryResponse<bool> { Data = resp.Data, Message = "", Success = true };
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

        // Usuwanie znajomego
        public async Task<RepositoryResponse<bool>> DeleteFriend(int firendId)
        {
            string errMsg = "";
            try
            {
                HttpResponseMessage response = await m_HttpClient.DeleteAsync($"{m_Configuration.WebApiUrl}/User/{m_Configuration.User.Id}/deleteFriend/{firendId}");
                if (response.IsSuccessStatusCode)
                {
                    RepositoryResponse<bool> resp = await response.Content.ReadFromJsonAsync<RepositoryResponse<bool>>();
                    if (resp.Success)
                        return new RepositoryResponse<bool> { Data = resp.Data, Message = "", Success = true };
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

        //Tworzenie nowego użytkownika
        public async Task<RepositoryResponse<bool>> Register(CreateUserDTO user)
        {
            string errMsg = "";
            try
            {
                string json = JsonConvert.SerializeObject(user);
                StringContent httpContent = new StringContent(json, Encoding.UTF8, "application/json");
                HttpResponseMessage response = m_HttpClient.PostAsync($"{m_Configuration.WebApiUrl}/User", httpContent).Result;
                if (response.IsSuccessStatusCode)
                {
                    RepositoryResponse<bool> resp = await response.Content.ReadFromJsonAsync<RepositoryResponse<bool>>();
                    if (resp.Success)
                        return new RepositoryResponse<bool> { Data = resp.Data, Message = "", Success = true };
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

        //Edycja użytkownika
        public async Task<RepositoryResponse<bool>> UpdateUser(int id, CreateUserDTO user)
        {
            string errMsg = "";
            try
            {
                string json = JsonConvert.SerializeObject(user);
                StringContent httpContent = new StringContent(json, Encoding.UTF8, "application/json");
                HttpResponseMessage response = m_HttpClient.PutAsync($"{m_Configuration.WebApiUrl}/User/{id}", httpContent).Result;
                if (response.IsSuccessStatusCode)
                {
                    RepositoryResponse<bool> resp = await response.Content.ReadFromJsonAsync<RepositoryResponse<bool>>();
                    if (resp.Success)
                        return new RepositoryResponse<bool> { Data = resp.Data, Message = "", Success = true };
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

        //Zmiana hasła
        public async Task<RepositoryResponse<bool>> ChangePassword(int userId, string Password)
        {
            string errMsg = "";
            try
            {
                string json = JsonConvert.SerializeObject(Password);
                StringContent httpContent = new StringContent(json, Encoding.UTF8, "application/json");
                HttpResponseMessage response = m_HttpClient.PutAsync($"{m_Configuration.WebApiUrl}/User/{userId}/changePassword", httpContent).Result;
                if (response.IsSuccessStatusCode)
                {
                    RepositoryResponse<bool> resp = await response.Content.ReadFromJsonAsync<RepositoryResponse<bool>>();
                    if (resp.Success)
                        return new RepositoryResponse<bool> { Data = resp.Data, Message = "", Success = true };
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

        //Wysłanie maila przypominajacego o wyrownaniu zaleglosci pienieznych
        public async Task<RepositoryResponse<bool>> SendRemindEmail(int tourId, int waitingUserId, OtherUser owedUser)
        {
            string errMsg = "";
            try
            {
                string json = JsonConvert.SerializeObject(owedUser);
                StringContent httpContent = new StringContent(json, Encoding.UTF8, "application/json");
                HttpResponseMessage response = m_HttpClient.PostAsync($"{m_Configuration.WebApiUrl}/User/{waitingUserId}/sendRemindEmail/{tourId}", httpContent).Result;
                if (response.IsSuccessStatusCode)
                {
                    RepositoryResponse<bool> resp = await response.Content.ReadFromJsonAsync<RepositoryResponse<bool>>();
                    if (resp.Success)
                        return new RepositoryResponse<bool> { Data = resp.Data, Message = "", Success = true };
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
