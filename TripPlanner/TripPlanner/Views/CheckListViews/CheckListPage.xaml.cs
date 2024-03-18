using TripPlanner.ViewModels.CheckList;

namespace TripPlanner.Views.CheckListViews;

public partial class CheckListPage : ContentPage
{
	public CheckListPage(CheckListViewModel checkListViewModel)
	{
		InitializeComponent();
		BindingContext = checkListViewModel;
	}
}