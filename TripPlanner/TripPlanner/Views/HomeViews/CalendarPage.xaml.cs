using TripPlanner.ViewModels;
using TripPlanner.ViewModels.Home;

namespace TripPlanner.Views.HomeViews;

public partial class CalendarPage : ContentPage
{
    public CalendarPage(CalendarViewModel calendarViewModel)
	{
		InitializeComponent();
        BindingContext = calendarViewModel;
	}
}