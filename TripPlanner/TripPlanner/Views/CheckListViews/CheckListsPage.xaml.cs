using TripPlanner.ViewModels.CheckList;

namespace TripPlanner.Views.CheckListViews;

public partial class CheckListsPage : ContentPage
{
    public CheckListsPage(CheckListsViewModel checklistViewModel)
    {
        InitializeComponent();
        BindingContext = checklistViewModel;
    }
}