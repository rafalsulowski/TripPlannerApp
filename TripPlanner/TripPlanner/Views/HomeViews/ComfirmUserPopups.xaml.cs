using CommunityToolkit.Maui.Views;
using TripPlanner.Models.DTO.UserDTOs;
using TripPlanner.Services;

namespace TripPlanner.Views.HomeViews;

public partial class ComfirmUserPopups : Popup
{
    private UserDTO m_User;
    private UserService m_UserService;

    public ComfirmUserPopups(UserService service, UserDTO user)
    {
        InitializeComponent();
        m_User = user;
        m_UserService = service;
    }

    async void Submit_Clicked(Object sender, EventArgs e)
    {
        var rsp = await m_UserService.Login(m_User.Email, m_Pass.Text);
        if (rsp.Success)
        {
            await CloseAsync(true);
        }
        else
        {
            await Shell.Current.CurrentPage.DisplayAlert("B³¹d", "Niepoprawne has³o", "Popraw");
        }
    }
}