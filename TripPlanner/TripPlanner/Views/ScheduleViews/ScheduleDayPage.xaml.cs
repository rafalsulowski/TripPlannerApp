using TripPlanner.ViewModels.Schedule;

namespace TripPlanner.Views.ScheduleViews;

public partial class ScheduleDayPage : ContentPage
{
	public ScheduleDayPage(ScheduleDayViewModel scheduleDayViewModel)
	{
		InitializeComponent();
		BindingContext = scheduleDayViewModel;
	}
}