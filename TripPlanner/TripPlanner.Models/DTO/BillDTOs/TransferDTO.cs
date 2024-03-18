using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TripPlanner.Models.Models;
using TripPlanner.Models.Models.BillModels;

namespace TripPlanner.Models.DTO.BillDTOs
{
    public class TransferDTO : ShareDTO
    {
        public int SenderId { get; set; }
        public int RecipientId { get; set; }


        public static implicit operator Transfer(TransferDTO data)
        {
            if (data == null)
                return null;

            return new Transfer
            {
                Id = data.Id,
                TourId = data.TourId,
                Description = data.Description,
                Name = data.Name,
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
