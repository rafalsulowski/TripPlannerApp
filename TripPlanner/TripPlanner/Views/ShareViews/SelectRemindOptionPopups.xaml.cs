using CommunityToolkit.Maui.Views;
using TripPlanner.Models.DTO.BillDTOs;

namespace TripPlanner.Views.ShareViews;

public partial class SelectRemindOptionPopups : Popup
{
	public SelectRemindOptionPopups()
	{
		InitializeComponent();
    }
    async void GoBack(Object sender, EventArgs e)
    {
        await CloseAsync();
    }

    async void SendEmial(Object sender, EventArgs e)
    {
        await CloseAsync("email");
    }
    async void SendNotification(Object sender, EventArgs e)
    {
        await CloseAsync("notification");
    }
}