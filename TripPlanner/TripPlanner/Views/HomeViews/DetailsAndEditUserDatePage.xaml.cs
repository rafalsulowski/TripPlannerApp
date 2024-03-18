using TripPlanner.ViewModels.User;

namespace TripPlanner.Views.HomeViews;

public partial class DetailsAndEditUserDatePage : ContentPage
{
	public DetailsAndEditUserDatePage(ProfileDetailsViewModel profileViewModel)
	{
		InitializeComponent();
		BindingContext = profileViewModel;
    }
}