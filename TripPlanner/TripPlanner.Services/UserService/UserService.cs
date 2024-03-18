using System.Linq.Expressions;
using TripPlanner.Services.TourService;
using TripPlanner.Services.QuestionnaireVoteService;
using TripPlanner.Models.Models;
using TripPlanner.Models.Models.UserModels;
using TripPlanner.Services.BillService;
using TripPlanner.Models.Models.TourModels;
using TripPlanner.Models.DTO.UserDTOs;
using TripPlanner.DataAccess.IRepository;

namespace TripPlanner.Services.UserService
{
    public class UserService : IUserService
    {
        private readonly IUserRepository _UserRepository;
        private readonly IFriendRepository _FriendRepository;
        private readonly ITourService _TourService;
        //private readonly IBillService _IBillService;
        private readonly IQuestionnaireVoteService _QuestionnaireVoteService;
        public UserService(IUserRepository userRepository, ITourService __TourService, /*IBillService __IBillService,*/
            IQuestionnaireVoteService __QuestionnaireVoteService, IFriendRepository friendRepository)
        {
            _UserRepository = userRepository;
            _TourService = __TourService;
            _QuestionnaireVoteService = __QuestionnaireVoteService;
            //_IBillService = __IBillService;
            _FriendRepository = friendRepository;
        }

        public async Task<RepositoryResponse<bool>> CreateUser(User user)
        {
            _UserRepository.Add(user);
            var response = await _UserRepository.SaveChangesAsync();
            return response;
        }

        public async Task<RepositoryResponse<bool>> DeleteUser(User user)
        {
            var resp = await _UserRepository.GetFirstOrDefault(u => u.Id == user.Id, "BillContributors,QuestionnaireVotes");
            var resp2 = await _FriendRepository.GetAll(u => u.Friend1Id == user.Id || u.Friend2Id == user.Id);
            if (resp.Data == null)
                return new RepositoryResponse<bool> { Data = true, Message = "Uzytkownik zostal usuniety", Success = true };

            List<Friend> friends = new List<Friend>();
            if (resp2.Data != null)
                friends = resp2.Data;

            //removing Friends
            foreach (var friend in friends)
                _FriendRepository.Remove(friend);

            User UserDB = resp.Data;

            //removing QuestionnaireVotes
            foreach (var Questionnaires in UserDB.QuestionnaireVotes)
                await _QuestionnaireVoteService.DeleteQuestionnaireVote(Questionnaires);

            //problem z zapetlaniem sie serwisow
            // UserService -> BillService -> UserService
            //uzywam tych serwisow do usuwania oraz pobierania info o uzytkowniku

            ////removing BillContributors
            //foreach (var BillContributors in UserDB.BillContributors)
            //    await _IBillService.DeleteBillContributors(BillContributors);

            _UserRepository.Remove(UserDB);
            var response = await _UserRepository.SaveChangesAsync();
            return response;
        }

        public async Task<RepositoryResponse<List<ExtendFriendDTO>>> GetFriends(int userId, int tourId)
        {
            var resp = await _FriendRepository.GetAll(u => u.Friend1Id == userId || u.Friend2Id == userId);

            RepositoryResponse<Tour> resp2 = new RepositoryResponse<Tour>();
            Tour tour = null;
            if (tourId != -1)
            {
                resp2 = await _TourService.GetTourAsync(u => u.Id == tourId, "Participants");
                tour = resp2.Data;
            }

            List<ExtendFriendDTO> listReturn = new List<ExtendFriendDTO>();
            if (resp.Success)
            {
                if (resp.Data is null)
                {
                    return new RepositoryResponse<List<ExtendFriendDTO>> { Data = listReturn, Message = "", Success = true };
                }
                
                List<Friend> list = resp.Data;
                for (int i = 0; i < list.Count; i++)
                {
                    int userIdToSearch = list[i].Friend1Id == userId ? list[i].Friend2Id : list[i].Friend1Id;  
                    User? user = _UserRepository.GetFirstOrDefault(u => u.Id == userIdToSearch).Result?.Data;

                    if (user is null)
                        return new RepositoryResponse<List<ExtendFriendDTO>> { Data = listReturn, Message = "", Success = true };

                    ExtendFriendDTO participantDTO = new ExtendFriendDTO();
                    participantDTO.UserId = user.Id;
                    participantDTO.Phone = user.Phone;
                    participantDTO.DateOfBirth = user.DateOfBirth;
                    participantDTO.Order = i + 1;
                    participantDTO.Email = user.Email;
                    participantDTO.City = user.City;
                    participantDTO.Age = _TourService.CalculateAge(user.DateOfBirth, DateTime.Now);
                    participantDTO.FullName = user.FullName;

                    if(tour != null)
                    {
                        bool val = tour.Participants.FirstOrDefault(p => p.UserId == user.Id) != null;
                        participantDTO.IsParticipant = val;
                    }

                    listReturn.Add(participantDTO);
                }

            }

            return new RepositoryResponse<List<ExtendFriendDTO>> { Data = listReturn, Message = "", Success = true };
        }


        public async Task<RepositoryResponse<List<ExtendUserDTO>>> ExtendUserDto(int userId)
        {
            var users = await _UserRepository.GetAll();
            if (users.Data == null)
                return new RepositoryResponse<List<ExtendUserDTO>> { Data = new List<ExtendUserDTO>(), Message = "", Success = true };


            List<ExtendUserDTO> result = new List<ExtendUserDTO>();
            foreach (var user in users.Data)
            {
                if (user.Id == userId)
                    continue;

                ExtendUserDTO extend = new ExtendUserDTO();
                extend.FullName = user.FullName;
                extend.FullAddress = user.FullAddress;
                extend.Id = user.Id;
                extend.Phone = user.Phone;
                extend.City = user.City;
                extend.DateOfBirth = user.DateOfBirth;
                extend.Email = user.Email;
                extend.IsActivated = user.IsActivated;
                extend.IsFriend = false;

                var resp = await _FriendRepository.GetFirstOrDefault(u => (u.Friend1Id == userId && u.Friend2Id == user.Id) || (u.Friend2Id == userId && u.Friend1Id == user.Id));
                if (resp.Data != null)
                    extend.IsFriend = true;
                
                result.Add(extend);
            }

            return new RepositoryResponse<List<ExtendUserDTO>> { Data = result, Message = "", Success = true };
        }


        public async Task<RepositoryResponse<User>> GetUserAsync(Expression<Func<User, bool>> filter, string? includeProperties = null)
        {
            var response = await _UserRepository.GetFirstOrDefault(filter, includeProperties);
            return response;
        }

        public async Task<RepositoryResponse<List<User>>> GetUsersAsync(Expression<Func<User, bool>>? filter = null, string? includeProperties = null)
        {
            var response = await _UserRepository.GetAll(filter, includeProperties);
            return response;
        }

        public async Task<RepositoryResponse<bool>> UpdateUser(User user)
        {
            var response = await _UserRepository.Update(user);
            if(response.Success==false)
            {
                return response;
            }
            response = await _UserRepository.SaveChangesAsync();
            return response;
        }

        public async Task<RepositoryResponse<Friend>> GetFriendAsync(Expression<Func<Friend, bool>> filter, string? includeProperties = null)
        {
            var response = await _FriendRepository.GetFirstOrDefault(filter, includeProperties);
            return response;
        }
    }
}
