using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TripPlanner.Models.Models.RouteModels;
using TripPlanner.Models.Models;
using TripPlanner.Models.Models.BillModels;

namespace TripPlanner.DataAccess.IRepository
{
    public interface ITransferRepository : IRepository<Transfer>
    {
        Task<RepositoryResponse<bool>> Update(Transfer post);
    }
}
