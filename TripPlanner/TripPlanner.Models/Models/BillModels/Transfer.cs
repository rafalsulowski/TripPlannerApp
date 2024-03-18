using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TripPlanner.Models.DTO.BillDTOs;
using TripPlanner.Models.Models.UserModels;

namespace TripPlanner.Models.Models.BillModels
{
    public class Transfer : Share
    {
        public int SenderId { get; set; }
        public User Sender { get; set; } = null!;

        public int RecipientId { get; set; }
        public User Recipient { get; set; } = null!;


        public static implicit operator TransferDTO(Transfer data)
        {
            if (data == null)
                return null;

            return new TransferDTO
            {
                Id = data.Id,
                TourId = data.TourId,
                Name = data.Name,
                Description = data.Description,
                CreatedDate = data.CreatedDate,
                CreatorId = data.CreatorId,
                ImageFilePath = data.ImageFilePath,
                Value = data.Value,
                SenderId = data.SenderId,
                RecipientId = data.RecipientId
            };
        }
    }
}
