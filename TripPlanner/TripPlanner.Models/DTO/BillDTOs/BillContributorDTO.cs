using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TripPlanner.Models.Models;
using TripPlanner.Models.Models.BillModels;

namespace TripPlanner.Models.DTO.BillDTOs
{
    public class BillContributorDTO
    {
        public int BillId { get; set; }
        public int UserId { get; set; }
        public decimal Due { get; set; }


        public static implicit operator BillContributor(BillContributorDTO data)
        {
            if (data == null)
                return null;

            return new BillContributor
            {
                BillId = data.BillId,
                UserId = data.UserId,
                Due = data.Due,
            };
        }
    }
}
