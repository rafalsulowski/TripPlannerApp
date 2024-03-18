using TripPlanner.Models.Models.MessageModels;

namespace TripPlanner.Models.DTO.MessageDTOs
{
    public class NoticeMessageDTO : MessageDTO
    {
        public override NoticeMessage MapFromDTO()
        {
            return new NoticeMessage
            {
                Id = Id,
                UserId = UserId,
                TourId = TourId,
                Content = Content,
                Date = Date,
            };
        }
    }
}
