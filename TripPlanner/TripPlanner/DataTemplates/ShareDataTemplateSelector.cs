using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TripPlanner.Helpers;
using TripPlanner.Models.DTO.BillDTOs;

namespace TripPlanner.DataTemplates
{
    public class ShareDataTemplateSelector : DataTemplateSelector
    {
        public DataTemplate Transfer { get; set; }
        public DataTemplate BillNotInvolved { get; set; }
        public DataTemplate BillBorrowed { get; set; }
        public DataTemplate BillOwe { get; set; }

        protected override DataTemplate OnSelectTemplate(object item, BindableObject container)
        {
            var elem = (SharePresentationDTO)item;
            if (elem.Type == SharePresentationDTO.ShareType.Bill)
            {
                if (elem.Value < 0)
                    return BillBorrowed;
                else if (elem.Value > 0)
                    return BillOwe;
                else
                    return BillNotInvolved;
            }
            else
            {
                return Transfer;
            }            
        }
    }
}
