using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TripPlanner.Helpers;
using TripPlanner.Models.DTO.BillDTOs;
using TripPlanner.Models.Models.BillModels;
using TripPlanner.ViewModels.CheckList;
using TripPlanner.ViewModels.Shares;

namespace TripPlanner.DataTemplates
{
    public class CheckListViewDataTemplateSelector : DataTemplateSelector
    {
        public DataTemplate CheckLists       { get; set; }
        public DataTemplate Questionnaires     { get; set; }

        protected override DataTemplate OnSelectTemplate(object item, BindableObject container)
        {
            var elem = (CheckListView)item;
            return elem.ViewType switch
            {
                ViewType.CheckLists => CheckLists,
                ViewType.Questionnaires => Questionnaires,
                _ => CheckLists,
            };
        }
    }
}
