using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TripPlanner.Models.Models.TourModels;
using TripPlanner.Models.Models;
using TripPlanner.Models.Models.BillModels;

namespace TripPlanner.Models.DTO.BillDTOs
{
    public class CreateTransferDTO
    {
        public int TourId { get; set; }
        public int CreatorId { get; set; }
        public int SenderId { get; set; }
        public int RecipientId { get; set; }

        public DateTime CreatedDate { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public decimal Value { get; set; }
        public string ImageFilePath { get; set; } = string.Empty;


        public static implicit operator Transfer(CreateTransferDTO data)
        {
            if (data == null)
                return null;

            return new Transfer
            {
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
