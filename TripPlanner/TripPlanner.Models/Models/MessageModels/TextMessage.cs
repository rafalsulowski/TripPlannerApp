using TripPlanner.Models.DTO.MessageDTOs;

namespace TripPlanner.Models.Models.MessageModels
{
    public class TextMessage : Message
    {
        public override TextMessageDTO MapToDTO()
        {
            return new TextMessageDTO
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
