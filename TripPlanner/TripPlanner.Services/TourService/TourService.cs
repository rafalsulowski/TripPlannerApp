using System.Linq.Expressions;
using TripPlanner.Models.Models;
using TripPlanner.Models.Models.TourModels;
using TripPlanner.Models.Models.CultureModels;
using TripPlanner.Services.ScheduleService;
using TripPlanner.Models.DTO.TourDTOs;
using TripPlanner.Models.Models.ScheduleModels;
using TripPlanner.Models.Models.UserModels;
using TripPlanner.DataAccess.IRepository;

namespace TripPlanner.Services.TourService
{
    public class TourService : ITourService
    {
        private readonly ITourRepository _TourRepository;
        private readonly IUserRepository _UserRepository;
        private readonly IParticipantTourRepository _ParticipantTourRepository;
        private readonly ICultureAssistanceRepository _CultureAssistanceRepository;
        private readonly IScheduleService _ScheduleService;
        private readonly IQuestionnaireRepository _QuestionnaireRepository;

        public TourService(IUserRepository userRepository, ITourRepository TourRepository, IScheduleService scheduleService,
            IParticipantTourRepository participantTourRepository, ICultureAssistanceRepository __CultureAssistanceRepository, IQuestionnaireRepository questionnaireRepository)
        {
            _TourRepository = TourRepository;
            _UserRepository = userRepository;
            _ParticipantTourRepository = participantTourRepository;
            _CultureAssistanceRepository = __CultureAssistanceRepository;
            _ScheduleService = scheduleService;
            _QuestionnaireRepository = questionnaireRepository;
        }

        public async Task<RepositoryResponse<bool>> CreateTour(Tour Tour, int userId)
        {
            _TourRepository.Add(Tour);
            var response = await _TourRepository.SaveChangesAsync();

            //dodanie pierwszego uczestnia
            ParticipantTourDTO participant = new ParticipantTourDTO();
            participant.UserId = userId;
            participant.TourId = Tour.Id;
            participant.IsOrganizer = true;
            participant.AccessionDate = Tour.CreateDate;
            var response3 = await AddParticipantToTour(participant);
            if (response3.Success == false)
            {
                await DeleteTour(Tour);
                return new RepositoryResponse<bool> { Data = false, Success = false, Message = $"Nie udało się utowrzyć wycieczki ze względu na błąd podczas dodawania pierwszego uczestnika" };
            }

            //utworzenie harmongramu
            List<ScheduleDay> schedule = new List<ScheduleDay>();
            int daysCount = (Tour.EndDate - Tour.StartDate).Days;

            for (int i = 0; i <= daysCount; i++)
            {
                DateTimeOffset offsest = Tour.StartDate.Add(new TimeSpan(i, 0, 0, 0));
                var resp4 = await _ScheduleService.CreateScheduleDay(new ScheduleDay
                {
                    Date = offsest.Date,
                    Description = "",
                    TourId = Tour.Id,
                });
                if (resp4.Success == false)
                {
                    await DeleteTour(Tour);
                    return new RepositoryResponse<bool> { Data = false, Success = false, Message = $"Nie udało się utowrzyć wycieczki ze względu na błąd podczas tworzenia harmonogramu" };
                }
            }
            return response;
        }

        public async Task<RepositoryResponse<bool>> DeleteTour(Tour Tour)
        {
            _TourRepository.Remove(Tour);
            var response = await _TourRepository.SaveChangesAsync();
            return response;
        }

        public async Task<RepositoryResponse<Tour>> GetTourAsync(Expression<Func<Tour, bool>> filter, string? includeProperties = null)
        {
            var response = await _TourRepository.GetFirstOrDefault(filter, includeProperties);
            if(response.Data != null)
                response.Data.Messages = response.Data.Messages.OrderBy(u => u.Date).ToList();
            return response;
        }

        public async Task<RepositoryResponse<List<Tour>>> GetToursAsync(Expression<Func<Tour, bool>>? filter = null, string? includeProperties = null)
        {
            var response = await _TourRepository.GetAll(filter, includeProperties);
            return response;
        }

        public async Task<RepositoryResponse<ParticipantTour>> GetParticipantAsync(Expression<Func<ParticipantTour, bool>> filter, string? includeProperties = null)
        {
            var response = await _ParticipantTourRepository.GetFirstOrDefault(filter, includeProperties);
            return response;
        }

        public async Task<RepositoryResponse<List<ParticipantTour>>> GetParticipantsAsync(Expression<Func<ParticipantTour, bool>>? filter = null, string? includeProperties = null)
        {
            var response = await _ParticipantTourRepository.GetAll(filter, includeProperties);
            return response;
        }

        public async Task<RepositoryResponse<CultureAssistance>> GetCultureAssistanceAsync(Expression<Func<CultureAssistance, bool>> filter, string? includeProperties = null)
        {
            var response = await _CultureAssistanceRepository.GetFirstOrDefault(filter, includeProperties);
            return response;
        }

        public async Task<RepositoryResponse<List<CultureAssistance>>> GetCulturesAssistanceAsync(Expression<Func<CultureAssistance, bool>>? filter = null, string? includeProperties = null)
        {
            var response = await _CultureAssistanceRepository.GetAll(filter, includeProperties);
            return response;
        }

        public async Task<RepositoryResponse<ExtendParticipantDTO>> GetTourExtendParticipantById(int tourId, int userId)
        {
            var resp = await _ParticipantTourRepository.GetFirstOrDefault(u => u.TourId == tourId && u.UserId == userId);

            if (resp.Data is null)
                return new RepositoryResponse<ExtendParticipantDTO> { Data = null, Message = "", Success = false };

            var resp2 = _UserRepository.GetFirstOrDefault(u => u.Id == userId).Result;

            if (resp2.Data is null)
                return new RepositoryResponse<ExtendParticipantDTO> { Data = null, Message = "", Success = false };

            User user = resp2.Data as User;

            ExtendParticipantDTO participantDTO = new ExtendParticipantDTO();
            participantDTO.UserId = user.Id;
            participantDTO.Order = 0;
            participantDTO.Email = user.Email;
            participantDTO.Phone = user.Phone;
            participantDTO.City = user.City;
            participantDTO.Nickname = resp.Data.Nickname;
            participantDTO.Age = CalculateAge(user.DateOfBirth, DateTime.Now);
            participantDTO.DateOfBirth = user.DateOfBirth;
            participantDTO.FullName = user.FullName;
            participantDTO.IsOrganizer = resp.Data.IsOrganizer;
            
            return new RepositoryResponse<ExtendParticipantDTO> { Data = participantDTO, Message = "", Success = true };
        }

        public async Task<RepositoryResponse<List<ExtendParticipantDTO>>> GetTourExtendParticipants(int tourId)
        {
            var resp = await _ParticipantTourRepository.GetAll(u => u.TourId == tourId);
            List<ExtendParticipantDTO> listReturn = new List<ExtendParticipantDTO>();
            if (resp.Success)
            {
                if(resp.Data is null)
                {
                    return new RepositoryResponse<List<ExtendParticipantDTO>> { Data = listReturn, Message = "", Success = true};
                }

                List<ParticipantTour> list = resp.Data;
                for (int i = 0; i < list.Count; i++)
                {
                    User? user = _UserRepository.GetFirstOrDefault(u => u.Id == list[i].UserId).Result?.Data;

                    if(user is null)
                        return new RepositoryResponse<List<ExtendParticipantDTO>> { Data = listReturn, Message = "", Success = true};

                    ExtendParticipantDTO participantDTO = new ExtendParticipantDTO();
                    participantDTO.UserId = user.Id;
                    participantDTO.Order = i + 1;
                    participantDTO.Email = user.Email;
                    participantDTO.City = user.City;
                    participantDTO.Phone = user.Phone;
                    if(string.IsNullOrEmpty(list[i].Nickname))
                    {
                        participantDTO.FullName = user.FullName;
                        participantDTO.Nickname = "";
                    }
                    else
                    {
                        participantDTO.FullName = list[i].Nickname;
                        participantDTO.Nickname = user.FullName;
                    }
                    participantDTO.Age = CalculateAge(user.DateOfBirth, DateTime.Now);
                    participantDTO.DateOfBirth = user.DateOfBirth;
                    participantDTO.IsOrganizer = list[i].IsOrganizer;

                    listReturn.Add(participantDTO);
                }

            }
    
            return new RepositoryResponse<List<ExtendParticipantDTO>> { Data = listReturn, Message = "", Success = true};
        }

        public async Task<RepositoryResponse<List<ExtendParticipantDTO>>> GetParticipantsNames(int tourId)
        {
            var resp = await _ParticipantTourRepository.GetAll(u => u.TourId == tourId);
            List<ExtendParticipantDTO> listReturn = new List<ExtendParticipantDTO>();
            if (resp.Success)
            {
                if (resp.Data is null)
                {
                    return new RepositoryResponse<List<ExtendParticipantDTO>> { Data = listReturn, Message = "", Success = true };
                }

                List<ParticipantTour> list = resp.Data;
                for (int i = 0; i < list.Count; i++)
                {
                    User? user = _UserRepository.GetFirstOrDefault(u => u.Id == list[i].UserId).Result?.Data;

                    if (user is null)
                        return new RepositoryResponse<List<ExtendParticipantDTO>> { Data = listReturn, Message = "", Success = true };

                    ExtendParticipantDTO participantDTO = new ExtendParticipantDTO();
                    participantDTO.Nickname = list[i].Nickname;
                    participantDTO.FullName = user.FullName;
                    listReturn.Add(participantDTO);
                }

            }

            return new RepositoryResponse<List<ExtendParticipantDTO>> { Data = listReturn, Message = "", Success = true };
        }

        public int CalculateAge(DateTime birthDate, DateTime now)
        {
            int age = now.Year - birthDate.Year;

            if (birthDate > now.AddYears(-age))
                age--;
            return age;
        }

        public async Task<RepositoryResponse<bool>> UpdateTour(Tour Tour)
        {
            var response = await _TourRepository.Update(Tour);
            if (response.Success == false)
            {
                return response;
            }
            response = await _TourRepository.SaveChangesAsync();
            return response;
        }

        public async Task<RepositoryResponse<bool>> AddCultureAssistanceToTour(CultureAssistance Contribute)
        {
            await _TourRepository.AddCultureAssistanceToTour(Contribute);
            return await _TourRepository.SaveChangesAsync();
        }

        public async Task<RepositoryResponse<bool>> DeleteCultureAssistanceFromTour(CultureAssistance Contribute)
        {
            await _TourRepository.DeleteCultureAssistanceFromTour(Contribute);
            return await _TourRepository.SaveChangesAsync();
        }

        public async Task<RepositoryResponse<bool>> AddParticipantToTour(ParticipantTour Contribute)
        {
            await _TourRepository.AddParticipantToTour(Contribute);
            return await _TourRepository.SaveChangesAsync();
        }

        public async Task<RepositoryResponse<bool>> DeleteParticipantFromTour(ParticipantTour Contribute)
        {
            await _TourRepository.DeleteParticipantFromTour(Contribute);
            return await _TourRepository.SaveChangesAsync();
        }

    }
}