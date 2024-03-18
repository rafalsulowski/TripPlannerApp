using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TripPlanner.Helpers;
using TripPlanner.Models.DTO.BillDTOs;

namespace TripPlanner.DataTemplates
{
    public class OtherUserBalanceDataTemplateSelector : DataTemplateSelector
    {
        public DataTemplate OweOtherUser { get; set; }
        public DataTemplate BorrowedOtherUser { get; set; }

        protected override DataTemplate OnSelectTemplate(object item, BindableObject container)
        {
            var elem = (OtherUser)item;
            if (elem.Saldo > 0)
                return BorrowedOtherUser;
            else
                return OweOtherUser;
        }
    }
}
