using TripPlanner.ViewModels.Weather;

namespace TripPlanner.Views.WeatherViews;

public partial class WeatherPage : ContentPage
{
	public WeatherPage(WeatherViewModel weatherViewModel)
	{
		InitializeComponent();
		BindingContext = weatherViewModel;
	}
}