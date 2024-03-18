using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TripPlanner.Models.Models.BillModels;
using TripPlanner.Models.Models;

namespace TripPlanner.DataAccess.IRepository
{
    public interface IBillContributorRepository : IRepository<BillContributor>
    {
        Task<RepositoryResponse<bool>> Update(BillContributor post);
    }
}
