using TripPlanner.Models.Models.MessageModels;

namespace TripPlanner.Models.DTO.MessageDTOs
{
    public class CreateTextMessageDTO
    {
        public int UserId { get; set; }
        public int TourId { get; set; }

        public string Content { get; set; } = string.Empty;


        public static implicit operator TextMessage(CreateTextMessageDTO data)
        {
            if (data == null)
                return null;

            return new TextMessage
            {
                UserId = data.UserId,
                TourId = data.TourId,
                Content = data.Content,
            };
        }
    }
}
