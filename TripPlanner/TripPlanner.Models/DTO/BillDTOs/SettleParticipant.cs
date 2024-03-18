using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TripPlanner.Models.DTO.BillDTOs
{
    public class SettleParticipant
    {
        public string Name { get; set; } = string.Empty;
        public decimal Value { get; set; }
    }
}
