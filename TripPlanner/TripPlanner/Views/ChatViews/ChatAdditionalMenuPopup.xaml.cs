using CommunityToolkit.Maui.Views;
using Microsoft.AspNetCore.SignalR.Client;
using TripPlanner.Models.DTO.TourDTOs;
using TripPlanner.Services;

namespace TripPlanner.Views.ChatViews;

public partial class ChatAdditionalMenuPopup : Popup
{
    TourService m_TourService;
    int TourId;
	public ChatAdditionalMenuPopup(TourService tourService, int tourId)
	{
		InitializeComponent();
        m_TourService = tourService;
        TourId = tourId;
	}

    async void AddQuestionnaire(object sender, EventArgs args)
    {
        await CloseAsync();
        var navigationParameter = new Dictionary<string, object>
            {
                { "passTourId",  TourId},
                { "IsFromChat",  true},
            };
        await Shell.Current.GoToAsync($"Tour/Chat/CreateQuestionnaire", navigationParameter);
    }

    async void ShowPeopleOnChat(object sender, EventArgs args)
    {
        List<ExtendParticipantDTO> list = await m_TourService.GetTourExtendParticipant(TourId);
        List<string> list2 = new List<string>();
        foreach (ExtendParticipantDTO extendParticipant in list)
        {
            list2.Add(string.IsNullOrEmpty(extendParticipant.Nickname) ? extendParticipant.FullName : extendParticipant.Nickname);
        }

        await CloseAsync();
        await Shell.Current.CurrentPage.ShowPopupAsync(new PeopleChatListPopups("Lista osób czatu", list2));
    }
}