using TripPlanner.Models.DTO.MessageDTOs;

namespace TripPlanner.Models.Models.MessageModels
{
    /// <summary>
    /// Typ wiadomości wyświetlanej na czacie: ogłoszenie/ważna informacja/wyróżniona (tylko dla organizatora)
    /// </summary>
    public class NoticeMessage : Message
    {
        public override NoticeMessageDTO MapToDTO()
        {
            return new NoticeMessageDTO
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
