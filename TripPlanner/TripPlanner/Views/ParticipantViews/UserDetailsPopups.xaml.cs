using CommunityToolkit.Maui.Views;
using TripPlanner.Helpers;
using TripPlanner.Models.DTO.TourDTOs;
using TripPlanner.Models.DTO.UserDTOs;
using TripPlanner.ViewModels;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace TripPlanner.Views.ParticipantViews;

public partial class UserDetailsPopups : Popup
{
	ExtendParticipantDTO Participant;
	public UserDetailsPopups(ExtendParticipantDTO extendParticipantDTO, int userLogged, bool isOrganizer)
	{
		InitializeComponent();
        Participant = extendParticipantDTO;
        Name.Text = Participant.FullName;
        Telfon.Text = "Telefon: " + Participant.Phone;
        Nickname.Text = "Ksywka: " + (string.IsNullOrEmpty(Participant.Nickname) ? "<brak>" : Participant.Nickname);
        Function.Text = "Funkcja: " + (Participant.IsOrganizer ? "organizator" : "uczestnik");
        Email.Text = "E-mail: " + Participant.Email;
        City.Text = "Miasto zamieszkania: " + Participant.City;
        BirthDate.Text = "Data urodzenia: " + Participant.DateOfBirth.ToShortDateString();
        Age.Text = "Wiek: " + Participant.Age.ToString();

        bool credits = extendParticipantDTO.UserId == userLogged || isOrganizer ? true : false;
        ChangeNicknameButton.IsVisible = credits;
        DeleteButton.IsVisible = credits;

        if(extendParticipantDTO.UserId == userLogged)
        {
            DeleteButton.Text = "Opuœæ wyjazd";
        }
        else
            DeleteButton.Text = "Usuñ uczestnika";

    }

    private async void GoBack(object sender, EventArgs e)
    {
        await CloseAsync();
    }
}