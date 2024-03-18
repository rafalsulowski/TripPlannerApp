using Microsoft.EntityFrameworkCore;
using TripPlanner.DataAccess.IRepository;
using TripPlanner.DataAccess.Repository;
using TripPlanner.Models;
using TripPlanner.Models.Models;
using TripPlanner.Models.Models.CheckListModels;

namespace TripPlanner.DataAccess.Repository
{
    public class CheckListRepository : Repository<CheckList>, ICheckListRepository
    {
        private ApplicationDbContext _context;
        public CheckListRepository(ApplicationDbContext context) : base(context)
        {
            _context = context;
        }

        public async Task<RepositoryResponse<bool>> Update(CheckList post)
        {
            var postDB = await GetFirstOrDefault(u => u.Id == post.Id);
            if (postDB.Data == null)
            {
                return new RepositoryResponse<bool>
                {
                    Success = false,
                    Data = false,
                    Message = "CheckList with this Id was not found."
                };
            }
            _context.CheckLists.Attach(post);
            _context.Entry(post).State = EntityState.Modified;
            return new RepositoryResponse<bool> { Data = true };
        }

        public async Task<RepositoryResponse<bool>> AddFieldToCheckList(CheckListField CheckListField)
        {
            var CheckListFieldDB = _context.CheckListFields.FirstOrDefault(u => u.Name == CheckListField.Name && u.CheckListId== CheckListField.CheckListId);
            if (CheckListFieldDB == null)
            {
                _context.CheckListFields.Add(CheckListField);
            }
            else
            {
                _context.CheckListFields.Attach(CheckListField);
                _context.Entry(CheckListField).State = EntityState.Modified;
            }
            return new RepositoryResponse<bool> { Data = true };
        }

        public async Task<RepositoryResponse<bool>> UpdateCheckListField(CheckListField CheckListField)
        {
            var CheckListFieldDB = _context.CheckListFields.FirstOrDefault(u => u.CheckListId == CheckListField.CheckListId && u.Id == CheckListField.Id);
            if (CheckListFieldDB == null)
            {
                return new RepositoryResponse<bool>
                {
                    Success = false,
                    Data = false,
                    Message = "Nie istnieje take pole w checkliscie"
                };
            }
            _context.Entry(CheckListFieldDB).State = EntityState.Detached;
            _context.CheckListFields.Attach(CheckListField);
            _context.Entry(CheckListField).State = EntityState.Modified;
            return new RepositoryResponse<bool> { Data = true };
        }

        public async Task<RepositoryResponse<bool>> DeleteFieldFromCheckList(CheckListField CheckListField)
        {
            var res = _context.CheckListFields.FirstOrDefault(u => u.Id == CheckListField.Id&& u.CheckListId == CheckListField.CheckListId);
            if (res != null)
            {
                _context.CheckListFields.Remove(res);
            }
            return new RepositoryResponse<bool> { Data = true };
        }
    }
}
