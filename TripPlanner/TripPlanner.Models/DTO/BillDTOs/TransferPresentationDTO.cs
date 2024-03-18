using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TripPlanner.Models.Models.BillModels;

namespace TripPlanner.Models.DTO.BillDTOs
{
    public class TransferPresentationDTO
    {
        public int Id { get; set; }

        public DateTime CreatedDate { get; set; }
        public string PayerName { get; set; } = string.Empty;
        public string ReceiverName { get; set; } = string.Empty;
        public string CreatorName { get; set; } = string.Empty;
        public decimal Value { get; set; }
        public string ImageFilePath { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
    }
}
