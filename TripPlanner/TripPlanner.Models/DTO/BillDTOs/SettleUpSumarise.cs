using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TripPlanner.Models.DTO.BillDTOs
{
    public class SettleUpSumarise
    {
        public decimal ObligedSumarise { get; set; }
        ICollection<SettleParticipant> RecipientsToSendMoney { get; set; } = new List<SettleParticipant>();

        public decimal BorrowSumarise { get; set; }
        ICollection<SettleParticipant> DebetorsToDownloadMoney { get; set; } = new List<SettleParticipant>();
    }
}
