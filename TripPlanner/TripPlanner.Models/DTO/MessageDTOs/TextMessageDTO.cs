using TripPlanner.Models.Models.MessageModels;

namespace TripPlanner.Models.DTO.MessageDTOs
{
    public class TextMessageDTO : MessageDTO
    {
        public override TextMessage MapFromDTO()
        {
            return new TextMessage
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
