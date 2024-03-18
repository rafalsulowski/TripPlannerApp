using TripPlanner.ViewModels.Shares;

namespace TripPlanner.Views.ShareViews;

public partial class DivisionTypePage : ContentPage
{
	public DivisionTypePage(DivisionTypeViewModel divisionTypeViewModel)
	{
		InitializeComponent();
		BindingContext = divisionTypeViewModel;
    }
}