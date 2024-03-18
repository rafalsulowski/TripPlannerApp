using CommunityToolkit.Maui.Views;
using TripPlanner.Models.DTO.TourDTOs;

namespace TripPlanner.Views.ShareViews;

public partial class SelectPayerPopups : Popup
{
	private List<ExtendParticipantDTO> Participants;
	private int ActualSelectedUserId;

    public SelectPayerPopups(List<ExtendParticipantDTO> participants, int actualSelectedUserId)
	{
		InitializeComponent();
        Participants = participants;
		ActualSelectedUserId = actualSelectedUserId;
        
		List.ItemsSource = Participants;
    }

	public async void SelectParticipant(object sender, EventArgs e)
	{
		await CloseAsync();
	}
}