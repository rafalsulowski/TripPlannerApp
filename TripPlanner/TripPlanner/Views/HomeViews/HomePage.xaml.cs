using TripPlanner.ViewModels;
using TripPlanner.ViewModels.Home;

namespace TripPlanner.Views.HomeViews;

public partial class HomePage : ContentPage
{
    public HomePage(HomeViewModel homeViewModel)
    {
        InitializeComponent();
        BindingContext = homeViewModel;
    }
}