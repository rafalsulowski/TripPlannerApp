using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using TripPlanner.Models;
using TripPlanner.Models.Models;
using TripPlanner.Models.Models.MessageModels;

namespace TripPlanner.Services.ChatService
{
    public interface IChatService
    {
        Task<RepositoryResponse<List<TextMessage>>> GetTextMessagesAsync(Expression<Func<TextMessage, bool>>? filter = null, string? includeProperties = null);
        Task<RepositoryResponse<TextMessage>> GetTextMessageAsync(Expression<Func<TextMessage, bool>> filter, string? includeProperties = null);
        Task<RepositoryResponse<List<NoticeMessage>>> GetNoticeMessagesAsync(Expression<Func<NoticeMessage, bool>>? filter = null, string? includeProperties = null);
        Task<RepositoryResponse<NoticeMessage>> GetNoticeMessageAsync(Expression<Func<NoticeMessage, bool>> filter, string? includeProperties = null);
        Task<RepositoryResponse<bool>> AddTextMessage(TextMessage Message);
        Task<RepositoryResponse<bool>> AddNoticeMessage(NoticeMessage Message);
        Task<RepositoryResponse<bool>> DeleteTextMessage(TextMessage Message);
        Task<RepositoryResponse<bool>> DeleteNoticeMessage(NoticeMessage Message);
    }
}
