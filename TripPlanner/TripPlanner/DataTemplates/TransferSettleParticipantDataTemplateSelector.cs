using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TripPlanner.Models.DTO.BillDTOs;
using TripPlanner.Models.DTO.TourDTOs;
using TripPlanner.Models.DTO.UserDTOs;

namespace TripPlanner.DataTemplates
{
    public class TransferSettleParticipantDataTemplateSelector : DataTemplateSelector
    {
        public DataTemplate BillBorrowed { get; set; }
        public DataTemplate BillOwe { get; set; }

        protected override DataTemplate OnSelectTemplate(object item, BindableObject container)
        {
            return ((OtherUser)item).Saldo > 0 ? BillOwe : BillBorrowed;
        }
    }
}
