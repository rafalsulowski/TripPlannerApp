using CommunityToolkit.Maui.Views;
using TripPlanner.Models.DTO.UserDTOs;
using TripPlanner.Services;

namespace TripPlanner.Views.HomeViews;

public partial class TextEditUserPopups : Popup
{
    public TextEditUserPopups(string text)
    {
        InitializeComponent();
        m_Val.Text = text;
    }

    async void Submit_Clicked(Object sender, EventArgs e)
    {
        if(string.IsNullOrEmpty(m_Val.Text))
            await Shell.Current.CurrentPage.DisplayAlert("B³¹d", "Wartoœæ nie mo¿e byæ pusta", "Popraw");
        else
        {
            await CloseAsync(m_Val.Text);
        }    
    }
}