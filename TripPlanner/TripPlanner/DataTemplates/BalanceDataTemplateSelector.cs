using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TripPlanner.Helpers;
using TripPlanner.Models.DTO.BillDTOs;

namespace TripPlanner.DataTemplates
{
    public class BalanceDataTemplateSelector : DataTemplateSelector
    {
        public DataTemplate Owe { get; set; }
        public DataTemplate Borrowed { get; set; }
        public DataTemplate NotInvolved { get; set; }

        protected override DataTemplate OnSelectTemplate(object item, BindableObject container)
        {
            var elem = (UserBalance)item;
            if (elem.Saldo < 0)
                return Borrowed;
            else if (elem.Saldo > 0)
                return Owe;
            else
                return NotInvolved;
        }
    }
}
