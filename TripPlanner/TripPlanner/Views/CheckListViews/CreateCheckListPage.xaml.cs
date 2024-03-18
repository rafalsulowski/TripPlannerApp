using TripPlanner.ViewModels.CheckList;

namespace TripPlanner.Views.CheckListViews;

public partial class CreateCheckListPage : ContentPage
{
	public CreateCheckListPage(CreateCheckListViewModels createCheckListViewModels)
	{
		InitializeComponent();
		BindingContext = createCheckListViewModels;
	}
}