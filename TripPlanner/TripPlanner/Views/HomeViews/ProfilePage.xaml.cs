using TripPlanner.Models.Models.UserModels;
using TripPlanner.Services;
using TripPlanner.ViewModels;
using TripPlanner.ViewModels.User;

namespace TripPlanner.Views.HomeViews;

public partial class ProfilePage : ContentPage
{
    Configuration m_Configuration;
    UserService m_UserService;
    public ProfilePage(ProfileViewModel profileViewModel, UserService userService, Configuration conf)
	{
		InitializeComponent();
		BindingContext = profileViewModel;
        m_UserService = userService;
        m_Configuration = conf;
	}
    protected override async void OnAppearing()
    {
        base.OnAppearing();
        var user = await m_UserService.GetUserByEmial(m_Configuration.User.Email);
        if (user != null && user.Id != -1)
        {
            m_Configuration.User = user;
        }
    }
}