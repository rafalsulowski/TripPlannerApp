using CommunityToolkit.Maui.Views;
using TripPlanner.Models.DTO.TourDTOs;
using TripPlanner.Models.DTO.UserDTOs;
using TripPlanner.Models.Models.UserModels;

namespace TripPlanner.Views.HomeViews;

public partial class FirendDetailsPopups : Popup
{
    ExtendFriendDTO FirendDto;
    public FirendDetailsPopups(ExtendFriendDTO friendDTO)
	{
		InitializeComponent();
        FirendDto = friendDTO;

        Name.Text = FirendDto.FullName;
        Telfon.Text = "Telefon: " + FirendDto.Phone;
        Email.Text = "E-mail: " + FirendDto.Email;
        City.Text = "Miasto zamieszkania: " + FirendDto.City;
        BirthDate.Text = "Data urodzenia: " + FirendDto.DateOfBirth.ToShortDateString();
        Age.Text = "Wiek: " + FirendDto.Age.ToString();
        Tel.Text = FirendDto.Phone;
        Id.Text = FirendDto.UserId.ToString();
    }

    private async void GoBack(object sender, EventArgs e)
    {
        await CloseAsync();
    }
}