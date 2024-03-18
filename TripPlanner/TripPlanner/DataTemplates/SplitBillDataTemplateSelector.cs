using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TripPlanner.Helpers;
using TripPlanner.Models.DTO.BillDTOs;
using TripPlanner.Models.Models.BillModels;
using TripPlanner.ViewModels.Shares;

namespace TripPlanner.DataTemplates
{
    public class SplitBillDataTemplateSelector : DataTemplateSelector
    {
        public DataTemplate Equally       { get; set; }
        public DataTemplate Unequally     { get; set; }
        public DataTemplate ByPercentages { get; set; }
        public DataTemplate ByShares      { get; set; }
        public DataTemplate ByAdjustment  { get; set; }

        protected override DataTemplate OnSelectTemplate(object item, BindableObject container)
        {
            var elem = (SplitBillView)item;
            switch (elem.BillType)
            {
                case BillType.Equally:
                    return Equally;
                case BillType.Unequally:
                    return Unequally;
                case BillType.ByPercentages:
                    return ByPercentages;
                case BillType.ByShares:
                    return ByShares;
                case BillType.ByAdjustment:
                    return ByAdjustment;
                default:
                    return Equally;
            }
        }
    }
}
