using TripPlanner.ViewModels.Friend;
using TripPlanner.ViewModels.User;

namespace TripPlanner.Views.FriendViews;

public partial class AddFriendPage : ContentPage
{
	public AddFriendPage(AddFriendViewModel addFriendViewModel)
	{
		InitializeComponent();
		BindingContext = addFriendViewModel;
	}
}