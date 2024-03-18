using TripPlanner.ViewModels.Home;

namespace TripPlanner.Views.HomeViews;

public partial class NotificationDetailsPage : ContentPage
{
	public NotificationDetailsPage(NotificationDetailsViewModel notificationDetailsViewModel)
	{
		InitializeComponent();
		BindingContext = notificationDetailsViewModel;
	}
}