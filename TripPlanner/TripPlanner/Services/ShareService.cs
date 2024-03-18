using Newtonsoft.Json;
using System.Net.Http.Json;
using System.Text;
using TripPlanner.Models.DTO.BillDTOs;
using TripPlanner.Models.DTO.TourDTOs;
using TripPlanner.Models.Models;
using TripPlanner.Models.Models.BillModels;
using TripPlanner.Models.Models.TourModels;

namespace TripPlanner.Services
{
    public class ShareService
    {
        private readonly HttpClient m_HttpClient;
        private readonly Configuration m_Configuration;

        public ShareService(IHttpClientFactory httpClient, Configuration configuration)
        {
            m_HttpClient = httpClient.CreateClient("httpClient");
            m_Configuration = configuration;
        }


        // Zwraca listę wymian dla wyjazdu o danym id
        public async Task<RepositoryResponse<List<SharePresentationDTO>>> GetSharesByTourId(int tourId)
        {
            string errMsg = "";
            try
            {
                HttpResponseMessage response = m_HttpClient.GetAsync($"{m_Configuration.WebApiUrl}/Bill/getSharesPresentation/{m_Configuration.User.Id}/{tourId}").Result;
                if (response.IsSuccessStatusCode)
                {
                    RepositoryResponse<List<SharePresentationDTO>> resp = await response.Content.ReadFromJsonAsync<RepositoryResponse<List<SharePresentationDTO>>>();
                    if (resp.Success)
                        return new RepositoryResponse<List<SharePresentationDTO>> { Data = resp.Data, Message = "", Success = true };
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
            return new RepositoryResponse<List<SharePresentationDTO>> { Data = new List<SharePresentationDTO>(), Message = errMsg, Success = false };
        }

        // Zwraca info o rachunku
        public async Task<RepositoryResponse<BillPresentationDTO>> GetBill(int billId, int tourId)
        {
            string errMsg = "";
            try
            {
                HttpResponseMessage response = m_HttpClient.GetAsync($"{m_Configuration.WebApiUrl}/Bill/getBillPresentation/{m_Configuration.User.Id}/{billId}/{tourId}").Result;
                if (response.IsSuccessStatusCode)
                {
                    RepositoryResponse<BillPresentationDTO> resp = await response.Content.ReadFromJsonAsync<RepositoryResponse<BillPresentationDTO>>();
                    if (resp.Success)
                        return new RepositoryResponse<BillPresentationDTO> { Data = resp.Data, Message = "", Success = true };
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
            return new RepositoryResponse<BillPresentationDTO> { Data = new BillPresentationDTO(), Message = errMsg, Success = false };
        }

        // Zwraca info o transferze
        public async Task<RepositoryResponse<TransferPresentationDTO>> GetTransfer(int transferId, int tourId)
        {
            string errMsg = "";
            try
            {
                HttpResponseMessage response = m_HttpClient.GetAsync($"{m_Configuration.WebApiUrl}/Bill/getTransferPresentation/{m_Configuration.User.Id}/{transferId}/{tourId}").Result;
                if (response.IsSuccessStatusCode)
                {
                    RepositoryResponse<TransferPresentationDTO> resp = await response.Content.ReadFromJsonAsync<RepositoryResponse<TransferPresentationDTO>>();
                    if (resp.Success)
                        return new RepositoryResponse<TransferPresentationDTO> { Data = resp.Data, Message = "", Success = true };
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
            return new RepositoryResponse<TransferPresentationDTO> { Data = new TransferPresentationDTO(), Message = errMsg, Success = false };
        }

        // Zwraca info o bilansie wyjazdu
        public async Task<RepositoryResponse<Balance>> GetBalance(int tourId)
        {
            string errMsg = "";
            try
            {
                HttpResponseMessage response = await m_HttpClient.GetAsync($"{m_Configuration.WebApiUrl}/Bill/getBalance/{tourId}");
                if (response.IsSuccessStatusCode)
                {
                    RepositoryResponse<Balance> resp = await response.Content.ReadFromJsonAsync<RepositoryResponse<Balance>>();
                    if (resp.Success)
                        return new RepositoryResponse<Balance> { Data = resp.Data, Message = "", Success = true };
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
            return new RepositoryResponse<Balance> { Data = new Balance(), Message = errMsg, Success = false };
        }

        public async Task<RepositoryResponse<UserBalance>> GetBalanceOfUser(int userId, int tourId)
        {
            string errMsg = "";
            try
            {
                HttpResponseMessage response = m_HttpClient.GetAsync($"{m_Configuration.WebApiUrl}/Bill/getBalanceOfUser/{userId}/{tourId}").Result;
                if (response.IsSuccessStatusCode)
                {
                    RepositoryResponse<UserBalance> resp = await response.Content.ReadFromJsonAsync<RepositoryResponse<UserBalance>>();
                    if (resp.Success)
                        return new RepositoryResponse<UserBalance> { Data = resp.Data, Message = "", Success = true };
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
            return new RepositoryResponse<UserBalance> { Data = new UserBalance(), Message = errMsg, Success = false };
        }


        // Tworzenie nowego rachunku
        public async Task<RepositoryResponse<bool>> CreateBill(CreateBillDTO bill)
        {
            string errMsg = "";
            try
            {
                string json = JsonConvert.SerializeObject(bill);
                StringContent httpContent = new StringContent(json, Encoding.UTF8, "application/json");
                HttpResponseMessage response = m_HttpClient.PostAsync($"{m_Configuration.WebApiUrl}/Bill/createBill", httpContent).Result;
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

        // Tworzenie nowej transakcji
        public async Task<RepositoryResponse<bool>> CreateTransfer(CreateTransferDTO bill)
        {
            string errMsg = "";
            try
            {
                string json = JsonConvert.SerializeObject(bill);
                StringContent httpContent = new StringContent(json, Encoding.UTF8, "application/json");
                HttpResponseMessage response = m_HttpClient.PostAsync($"{m_Configuration.WebApiUrl}/Bill/createTransfer", httpContent).Result;
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

        // Modyfikuje rachunek
        public async Task<RepositoryResponse<bool>> UpdateBill(CreateBillDTO bill)
        {
            string errMsg = "";
            try
            {
                string json = JsonConvert.SerializeObject(bill);
                StringContent httpContent = new StringContent(json, Encoding.UTF8, "application/json");
                HttpResponseMessage response = m_HttpClient.PutAsync($"{m_Configuration.WebApiUrl}/Bill/updateBill/", httpContent).Result;
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

        // Modyfikuje transakcje
        public async Task<RepositoryResponse<bool>> UpdateTransfer(CreateTransferDTO transfer)
        {
            string errMsg = "";
            try
            {
                string json = JsonConvert.SerializeObject(transfer);
                StringContent httpContent = new StringContent(json, Encoding.UTF8, "application/json");
                HttpResponseMessage response = m_HttpClient.PutAsync($"{m_Configuration.WebApiUrl}/Bill/updateTransfer/", httpContent).Result;
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

        // Usuwa rachunek
        public async Task<RepositoryResponse<bool>> DeleteBill(int billId)
        {
            string errMsg = "";
            try
            {
                HttpResponseMessage response = m_HttpClient.DeleteAsync($"{m_Configuration.WebApiUrl}/Bill/deleteBill/{billId}").Result;
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

        // Usuwa transakcje
        public async Task<RepositoryResponse<bool>> DeleteTransfer(int billId)
        {
            string errMsg = "";
            try
            {
                HttpResponseMessage response = m_HttpClient.DeleteAsync($"{m_Configuration.WebApiUrl}/Bill/deleteTransfer/{billId}").Result;
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
