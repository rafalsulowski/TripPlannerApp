using TripPlanner.ViewModels;
using TripPlanner.ViewModels.Home;

namespace TripPlanner.Views.HomeViews;

public partial class NotificationPage : ContentPage
{
    public NotificationPage(NotificationViewModel notificationViewModel)
	{
		InitializeComponent();
		BindingContext = notificationViewModel;
    }
}