using CommunityToolkit.Maui.Views;
using TripPlanner.Models;
using TripPlanner.Models.DTO.TourDTOs;

namespace TripPlanner.Views.ParticipantViews;

public partial class AddParticipantPopup : Popup
{
    private int TourId;
	public AddParticipantPopup(int tourId, string inviteLink)
	{
		InitializeComponent();
        LabelLink.Text = inviteLink;
        TourId = tourId;
    }

	public async void GoToFriendList_Cliked(object sender, EventArgs e)
	{
        await CloseAsync();
        var navigationParameter = new Dictionary<string, object>
            {
                { "passTourId",  TourId}
            };
        await Shell.Current.GoToAsync($"Tour/Participants/AddParticipantFromFriends", navigationParameter);
    }
}