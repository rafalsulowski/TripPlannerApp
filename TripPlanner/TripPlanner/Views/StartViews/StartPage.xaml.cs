
using TripPlanner.Services;

namespace TripPlanner.Views.StartViews;

public partial class StartPage : ContentPage
{
    public StartPage()
    {
        InitializeComponent();
    }

    public async void GoLogin(object e, EventArgs args)
    {
        await Shell.Current.GoToAsync("/Login");
    }

    public async void GoRegister(object e, EventArgs args)
    {
        await Shell.Current.GoToAsync("/Register");
    }
}

