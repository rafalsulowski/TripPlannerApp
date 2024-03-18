using TripPlanner.ViewModels.User;

namespace TripPlanner.Views.StartViews;

public partial class LoginPage : ContentPage
{
    Configuration m_Configuration;
	public LoginPage(LoginViewModel loginViewModel, Configuration conf)
	{
		InitializeComponent();
        m_Configuration = conf;
        BindingContext = loginViewModel;
    }
    protected override async void OnAppearing()
    {
        base.OnAppearing();
        
        if(m_Configuration != null && m_Configuration.IsLoggedIn)
        {
            await Shell.Current.GoToAsync("///Profile");
        }
    }
}