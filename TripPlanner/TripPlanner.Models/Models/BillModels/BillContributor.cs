using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TripPlanner.Models.DTO.BillDTOs;
using TripPlanner.Models.Models.UserModels;

namespace TripPlanner.Models.Models.BillModels
{
    public class BillContributor
    {
        public int BillId { get; set; }
        public Bill Bill { get; set; } = null!;
        public int UserId { get; set; }
        public User User { get; set; } = null!;
        public decimal Due { get; set; }


        public static implicit operator BillContributorDTO(BillContributor data)
        {
            if (data == null)
                return null;

            return new BillContributorDTO
            {
                BillId = data.BillId,
                UserId = data.UserId,
                Due = data.Due,
            };
        }
    }
}
