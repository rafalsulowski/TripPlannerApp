using System.Linq.Expressions;
using TripPlanner.DataAccess.IRepository;
using TripPlanner.Models.Models;
using TripPlanner.Models.Models.MessageModels;

namespace TripPlanner.Services.ChatService
{
    public class ChatService : IChatService
    {
        private readonly ITextMessageRepository _TextMessageRepository;
        private readonly INoticeMessageRepository _NoticeMessageRepository;
        public ChatService(ITextMessageRepository __TextMessageRepository, INoticeMessageRepository __NoticeMessageRepository)
        {
            _TextMessageRepository = __TextMessageRepository;
            _NoticeMessageRepository = __NoticeMessageRepository;
        }

        public async Task<RepositoryResponse<TextMessage>> GetTextMessageAsync(Expression<Func<TextMessage, bool>> filter, string? includeProperties = null)
        {
            var response = await _TextMessageRepository.GetFirstOrDefault(filter, includeProperties);
            return response;
        }

        public async Task<RepositoryResponse<List<TextMessage>>> GetTextMessagesAsync(Expression<Func<TextMessage, bool>>? filter = null, string? includeProperties = null)
        {
            var response = await _TextMessageRepository.GetAll(filter, includeProperties);
            return response;
        }

        public async Task<RepositoryResponse<NoticeMessage>> GetNoticeMessageAsync(Expression<Func<NoticeMessage, bool>> filter, string? includeProperties = null)
        {
            var response = await _NoticeMessageRepository.GetFirstOrDefault(filter, includeProperties);
            return response;
        }

        public async Task<RepositoryResponse<List<NoticeMessage>>> GetNoticeMessagesAsync(Expression<Func<NoticeMessage, bool>>? filter = null, string? includeProperties = null)
        {
            var response = await _NoticeMessageRepository.GetAll(filter, includeProperties);
            return response;
        }

        public async Task<RepositoryResponse<bool>> AddTextMessage(TextMessage Message)
        {
            _TextMessageRepository.Add(Message);
            return await _TextMessageRepository.SaveChangesAsync();
        }

        public async Task<RepositoryResponse<bool>> DeleteTextMessage(TextMessage Message)
        {
            _TextMessageRepository.Add(Message);
            return await _TextMessageRepository.SaveChangesAsync();
        }

        public async Task<RepositoryResponse<bool>> AddNoticeMessage(NoticeMessage Message)
        {
            _NoticeMessageRepository.Add(Message);
            return await _NoticeMessageRepository.SaveChangesAsync();
        }

        public async Task<RepositoryResponse<bool>> DeleteNoticeMessage(NoticeMessage Message)
        {
            _NoticeMessageRepository.Add(Message);
            return await _NoticeMessageRepository.SaveChangesAsync();
        }
    }
}
