
using TripPlanner.Models.Models;
using TripPlanner.Models.Models.BillModels;
using TripPlanner.DataAccess.IRepository;

namespace TripPlanner.DataAccess.Repository
{
    public class TransferRepository : Repository<Transfer>, ITransferRepository
    {
        private ApplicationDbContext _context;
        public TransferRepository(ApplicationDbContext context) : base(context)
        {
            _context = context;
        }

        public async Task<RepositoryResponse<bool>> Update(Transfer post)
        {
            var postDB = await GetFirstOrDefault(u => u.Id == u.Id);
            if (postDB == null)
            {
                return new RepositoryResponse<bool>
                {
                    Success = false,
                    Data = false,
                    Message = "Transfer with this Id was not found."
                };
            }
            _context.Transfers.Attach(post);
            _context.Entry(post).State = Microsoft.EntityFrameworkCore.EntityState.Modified;
            return new RepositoryResponse<bool> { Data = true };
        }
    }
}
