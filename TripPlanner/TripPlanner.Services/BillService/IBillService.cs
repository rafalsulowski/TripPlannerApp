using System.Linq.Expressions;
using TripPlanner.Models.DTO.BillDTOs;
using TripPlanner.Models.Models;
using TripPlanner.Models.Models.BillModels;
using TripPlanner.Models.Models.TourModels;

namespace TripPlanner.Services.BillService
{
    public interface IBillService
    {
        Task<RepositoryResponse<List<Bill>>> GetBillsAsync(Expression<Func<Bill, bool>>? filter = null, string? includeProperties = null);
        Task<RepositoryResponse<Bill>> GetBillAsync(Expression<Func<Bill, bool>> filter, string? includeProperties = null);
        Task<RepositoryResponse<List<Transfer>>> GetTransfersAsync(Expression<Func<Transfer, bool>>? filter = null, string? includeProperties = null);
        Task<RepositoryResponse<Transfer>> GetTransferAsync(Expression<Func<Transfer, bool>> filter, string? includeProperties = null);
        Task<RepositoryResponse<List<SharePresentationDTO>>> GetSharesPresentationAsync(int userId, int tourId);
        Task<RepositoryResponse<BillPresentationDTO>> GetBillPresentation(int userId, int billId, int tourId);
        Task<RepositoryResponse<TransferPresentationDTO>> GetTransferPresentation(int userId, int transferId, int tourId);
        Task<RepositoryResponse<Balance>> GetBalance(int tourId);
        Task<RepositoryResponse<UserBalance>> GetBalanceOfUser(int userId, int tourId);
        Task<RepositoryResponse<bool>> CreateTransfer(Transfer Bill);
        Task<RepositoryResponse<bool>> CreateBill(Bill Bill);
        Task<RepositoryResponse<bool>> UpdateBill(Bill Bill);
        Task<RepositoryResponse<bool>> UpdateTransfer(Transfer Bill);
        Task<RepositoryResponse<bool>> DeleteBill(Bill Bill);
        Task<RepositoryResponse<bool>> DeleteTransfer(Transfer Bill);
        Task<RepositoryResponse<bool>> DeleteBillContributors(BillContributor Bill);
    }
}
