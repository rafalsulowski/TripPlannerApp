using TripPlanner.ViewModels;
using TripPlanner.ViewModels.Friend;
using TripPlanner.ViewModels.User;

namespace TripPlanner.Views.HomeViews;

public partial class FriendsPage : ContentPage
{
    public FriendsPage(FriendsViewModel friendsViewModel)
	{
		InitializeComponent();
		BindingContext = friendsViewModel;
	}
}