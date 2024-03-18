using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TripPlanner.Models.Models.BillModels;

namespace TripPlanner.Models.DTO.BillDTOs
{
    public class BillPresentationDTO
    {
        public int Id { get; set; }

        public string Name { get; set; } = string.Empty;
        public decimal Value { get; set; }
        public DateTime CreatedDate { get; set; }
        public string CreatorName { get; set; } = string.Empty;
        public string PayerName { get; set; } = string.Empty;
        public BillType BillType { get; set; }
        public ICollection<ExtendBillContributorDTO> Contributors { get; set; } = new List<ExtendBillContributorDTO>();
        public string ImageFilePath { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
    }
}
