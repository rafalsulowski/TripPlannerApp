using Microsoft.AspNetCore.Mvc;
using TripPlanner.Services.UserService;
using TripPlanner.Services.TourService;
using TripPlanner.Models.DTO.BillDTOs;
using TripPlanner.Models.Models;
using TripPlanner.Models.Models.BillModels;
using TripPlanner.Services.BillService;
using System.Collections.Generic;
using TripPlanner.Models.DTO.TourDTOs;
using TripPlanner.Models.Models.TourModels;
using Microsoft.AspNetCore.Authorization;

namespace TripPlanner.WebAPI.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    //[ApiExplorerSettings(IgnoreApi = ProjectConfiguration.HideContorller)]
    public class BillController : ControllerBase
    {
        private readonly IBillService _BillService;
        private readonly IUserService _UserService;
        private readonly ITourService _TourService;

        public BillController(IBillService BillService, IUserService userService, ITourService tourService)
        {
            _BillService = BillService;
            _UserService = userService;
            _TourService = tourService;
        }
        

        [HttpGet("getSharesPresentation/{userId}/{tourId}")]
        public async Task<ActionResult<RepositoryResponse<List<SharePresentationDTO>>>> GetSharesPresentation(int userId, int tourId)
        {
            var response = await _BillService.GetSharesPresentationAsync(userId, tourId);
            if (response.Success)
                return Ok(new RepositoryResponse<List<SharePresentationDTO>> { Data = response.Data, Message = "", Success = true });
            else
                return BadRequest(new RepositoryResponse<List<SharePresentationDTO>> { Data = null, Message = response.Message, Success = false });
        }

        [HttpGet("getBillPresentation/{userId}/{billId}/{tourId}")]
        public async Task<ActionResult<RepositoryResponse<BillPresentationDTO>>> GetBillPresentation(int userId, int billId, int tourId)
        {
            var response = await _BillService.GetBillPresentation(userId, billId, tourId);
            if (response.Success)
                return Ok(new RepositoryResponse<BillPresentationDTO> { Data = response.Data, Message = "", Success = true });
            else
                return BadRequest(new RepositoryResponse<BillPresentationDTO> { Data = null, Message = response.Message, Success = false });
        }

        [HttpGet("getTransferPresentation/{userId}/{transferId}/{tourId}")]
        public async Task<ActionResult<RepositoryResponse<TransferPresentationDTO>>> GetTransferPresentation(int userId, int transferId, int tourId)
        {
            var response = await _BillService.GetTransferPresentation(userId, transferId, tourId);
            if (response.Success)
                return Ok(new RepositoryResponse<TransferPresentationDTO> { Data = response.Data, Message = "", Success = true });
            else
                return BadRequest(new RepositoryResponse<TransferPresentationDTO> { Data = null, Message = response.Message, Success = false });
        }

        [HttpGet("getBalance/{tourId}")]
        public async Task<ActionResult<RepositoryResponse<Balance>>> GetBalance(int tourId)
        {
            var response = await _BillService.GetBalance(tourId);
            if (response.Success)
                return Ok(new RepositoryResponse<Balance> { Data = response.Data, Message = "", Success = true });
            else
                return BadRequest(new RepositoryResponse<Balance> { Data = null, Message = response.Message, Success = false });
        }
        
        [HttpGet("getBalanceOfUser/{userId}/{tourId}")]
        public async Task<ActionResult<RepositoryResponse<UserBalance>>> GetBalanceOfUser(int userId, int tourId)
        {
            var response = await _BillService.GetBalanceOfUser(userId, tourId);
            if (response.Success)
                return Ok(new RepositoryResponse<UserBalance> { Data = response.Data, Message = "", Success = true });
            else
                return BadRequest(new RepositoryResponse<UserBalance> { Data = null, Message = response.Message, Success = false });
        }

        [HttpPost("createBill")]
        public async Task<ActionResult<RepositoryResponse<bool>>> CreateBill([FromBody] CreateBillDTO Bill)
        {
            var resp = await _TourService.GetTourAsync(u => u.Id == Bill.TourId);
            if (resp.Data == null)
            {
                return new RepositoryResponse<bool> { Success = false, Message = $"Nie istnieje wyjazd o id = {Bill.TourId}" };
            }
            if (resp.Data.Shares.FirstOrDefault(u => ((Bill)u).Name == Bill.Name) != null)
            {
                return new RepositoryResponse<bool> { Success = false, Message = $"Dana wyjazd posiada już rachunek o nazwie = {Bill.Name}" };
            }
            var resp2 = await _UserService.GetUserAsync(u => u.Id == Bill.CreatorId);
            if (resp2.Data == null)
            {
                return new RepositoryResponse<bool> { Success = false, Message = $"Nie istnieje użytkownik o id = {Bill.CreatorId}" };
            }
            var resp3 = await _UserService.GetUserAsync(u => u.Id == Bill.PayerId);
            if (resp3.Data == null)
            {
                return new RepositoryResponse<bool> { Success = false, Message = $"Nie istnieje użytkownik o id = {Bill.PayerId}" };
            }

            Bill newBill = Bill;

            var response = await _BillService.CreateBill(newBill);
            if (response.Success)
                return Ok(new RepositoryResponse<bool> { Data = true, Success = true, Message = "" });
            else
                return BadRequest(new RepositoryResponse<bool> { Data = true, Success = false, Message = response.Message });
        }

        [HttpPost("createTransfer")]
        public async Task<ActionResult<RepositoryResponse<bool>>> CreateTransfer([FromBody] CreateTransferDTO transfer)
        {
            var resp = await _TourService.GetTourAsync(u => u.Id == transfer.TourId);
            if (resp.Data == null)
            {
                return new RepositoryResponse<bool> { Data = false, Success = false, Message = $"Nie istnieje wyjazd o id = {transfer.TourId}" };
            }
            var resp2 = await _UserService.GetUserAsync(u => u.Id == transfer.CreatorId);
            if (resp2.Data == null)
            {
                return new RepositoryResponse<bool> { Data = false, Success = false, Message = $"Nie istnieje użytkownik o id = {transfer.CreatorId}" };
            }
            var resp3 = await _UserService.GetUserAsync(u => u.Id == transfer.SenderId);
            if (resp3.Data == null)
            {
                return new RepositoryResponse<bool> { Data = false, Success = false, Message = $"Nie istnieje użytkownik o id = {transfer.SenderId}" };
            }
            var resp4 = await _UserService.GetUserAsync(u => u.Id == transfer.RecipientId);
            if (resp4.Data == null)
            {
                return new RepositoryResponse<bool> { Data = false, Success = false, Message = $"Nie istnieje użytkownik o id = {transfer.RecipientId}" };
            }

            Transfer newTranfser = transfer;
            var response = await _BillService.CreateTransfer(newTranfser);
            if (response.Success)
                return Ok(new RepositoryResponse<bool> { Data = true, Success = true, Message = "" });
            else
                return BadRequest(new RepositoryResponse<bool> { Data = true, Success = false, Message = response.Message });
        }

        [HttpPut("updateBill/{billId}/{userId}")]
        public async Task<ActionResult<RepositoryResponse<bool>>> UpdateBill(int billid, int userId, [FromBody] CreateBillDTO bill)
        {
            var resp2 = await _BillService.GetBillAsync(u => u.Id == billid);
            if (resp2.Success == false || resp2.Data == null)
            {
                return new RepositoryResponse<bool> { Success = false, Message = $"Nie istnieje rachunek o id = {billid}" };
            }

            var resp = await _UserService.GetUserAsync(u => u.Id == userId);
            if (resp.Success == false || resp.Data == null)
            {
                return new RepositoryResponse<bool> { Success = false, Message = $"Nie istnieje użytkownik o id = {userId}" };
            }

            //walidacja uczesnika czy moze modyfikować rachunek
            var resp3 = await _TourService.GetTourAsync(u => u.Id == resp2.Data.TourId, "Participants");
            if(resp3.Success == false || resp3.Data == null)
            {
                return new RepositoryResponse<bool> { Success = false, Message = $"Nie istnieje wyjazd o id = {resp2.Data.TourId}" };
            }
            ParticipantTour? participant = resp3.Data.Participants.FirstOrDefault(p => p.UserId == userId);
            if (participant == null || (participant.IsOrganizer == false && bill.CreatorId != participant.UserId
                || bill.PayerId != participant.UserId))
            {
                return new RepositoryResponse<bool> { Success = false, Message = $"Odmowa dostępu!" };
            }

            Bill newBill = bill;
            newBill.Id = billid;
            newBill.TourId = resp2.Data.TourId;

            var response = await _BillService.UpdateBill(newBill);
            if (response.Success)
                return Ok(new RepositoryResponse<bool> { Data = true, Message = "", Success = true });
            else
                return BadRequest(new RepositoryResponse<bool> { Data = false, Message = response.Message, Success = false });
        }

        [HttpPut("updateTransfer{transferId}/{userId}")]
        public async Task<ActionResult<RepositoryResponse<bool>>> UpdateTransfer(int transferId, int userId, [FromBody] CreateTransferDTO transfer)
        {
            var resp2 = await _BillService.GetTransferAsync(u => u.Id == transferId);
            if (resp2.Success == false || resp2.Data == null)
            {
                return new RepositoryResponse<bool> { Success = false, Message = $"Nie istnieje rachunek o id = {transferId}" };
            }

            var resp = await _UserService.GetUserAsync(u => u.Id == userId);
            if (resp.Success == false || resp.Data == null)
            {
                return new RepositoryResponse<bool> { Success = false, Message = $"Nie istnieje użytkownik o id = {userId}" };
            }

            //walidacja uczesnika czy moze modyfikować rachunek
            var resp3 = await _TourService.GetTourAsync(u => u.Id == resp2.Data.TourId, "Participants");
            if (resp3.Success == false || resp3.Data == null)
            {
                return new RepositoryResponse<bool> { Success = false, Message = $"Nie istnieje wyjazd o id = {resp2.Data.TourId}" };
            }
            ParticipantTour? participant = resp3.Data.Participants.FirstOrDefault(p => p.UserId == userId);
            if (participant == null || (participant.IsOrganizer == false && (transfer.CreatorId != participant.UserId
                || transfer.SenderId != participant.UserId || transfer.RecipientId != participant.UserId)))
            {
                return new RepositoryResponse<bool> { Success = false, Message = $"Odmowa dostępu!" };
            }

            Transfer newTransfer = transfer;
            newTransfer.Id = transferId;
            newTransfer.TourId = resp2.Data.TourId;

            var response = await _BillService.UpdateTransfer(newTransfer);
            if (response.Success)
                return Ok(new RepositoryResponse<bool> { Data = true, Message = "", Success = true });
            else
                return BadRequest(new RepositoryResponse<bool> { Data = false, Message = response.Message, Success = false });
        }

        [HttpDelete("deleteBill/{billId}/{userId}")]
        public async Task<ActionResult<RepositoryResponse<bool>>> DeleteBill(int billId, int userId)
        {
            var resp2 = await _BillService.GetBillAsync(u => u.Id == billId);
            if (resp2.Success == false || resp2.Data == null)
            {
                return new RepositoryResponse<bool> { Success = false, Message = $"Nie istnieje rachunek o id = {billId}" };
            }
            Bill bill = resp2.Data;

            var resp = await _TourService.GetTourAsync(u => u.Id == bill.TourId, "Participants");
            if (resp.Data == null)
            {
                return new RepositoryResponse<bool> { Success = false, Message = $"Nie istnieje wyjazd o id = {bill.TourId}" };
            }

            var resp3 = await _UserService.GetUserAsync(u => u.Id == userId);
            if (resp3.Data == null)
            {
                return new RepositoryResponse<bool> { Success = false, Message = $"Nie istnieje użytkownik o id = {userId}" };
            }

            //walidacja użytkownika
            ParticipantTour? participant = resp.Data.Participants.FirstOrDefault(p => p.UserId == userId);
            if (participant == null || (participant.IsOrganizer == false && (bill.CreatorId != participant.UserId
                || bill.PayerId != participant.UserId)))
            {
                return new RepositoryResponse<bool> { Success = false, Message = $"Odmowa dostępu!" };
            }

            var response = await _BillService.DeleteBill(new Bill() { Id = billId });
            if (response.Success)
                return Ok(new RepositoryResponse<bool> { Data = true, Message = "", Success = true });
            else
                return BadRequest(new RepositoryResponse<bool> { Data = false, Message = response.Message, Success = false });
        }

        [HttpDelete("deleteTransfer/{transferId}/{userId}")]
        public async Task<ActionResult<RepositoryResponse<bool>>> DeleteTransfer(int transferId, int userId)
        {
            var resp2 = await _BillService.GetTransferAsync(u => u.Id == transferId);
            if (resp2.Success == false || resp2.Data == null)
            {
                return new RepositoryResponse<bool> { Success = false, Message = $"Nie istnieje rachunek o id = {transferId}" };
            }
            Transfer transfer = resp2.Data;

            var resp = await _TourService.GetTourAsync(u => u.Id == transfer.TourId, "Participants");
            if (resp.Data == null)
            {
                return new RepositoryResponse<bool> { Success = false, Message = $"Nie istnieje wyjazd o id = {transfer.TourId}" };
            }

            var resp3 = await _UserService.GetUserAsync(u => u.Id == userId);
            if (resp3.Data == null)
            {
                return new RepositoryResponse<bool> { Success = false, Message = $"Nie istnieje użytkownik o id = {userId}" };
            }

            //walidacja użytkownika
            ParticipantTour? participant = resp.Data.Participants.FirstOrDefault(p => p.UserId == userId);
            if (participant == null || (participant.IsOrganizer == false && (transfer.CreatorId != participant.UserId
                || transfer.SenderId != participant.UserId || transfer.RecipientId != participant.UserId)))
            {
                return new RepositoryResponse<bool> { Success = false, Message = $"Odmowa dostępu!" };
            }

            var response = await _BillService.DeleteTransfer(new Transfer() { Id = transferId });
            if (response.Success)
                return Ok(new RepositoryResponse<bool> { Data = true, Message = "", Success = true });
            else
                return BadRequest(new RepositoryResponse<bool> { Data = false, Message = response.Message, Success = false });
        }
    }
}
