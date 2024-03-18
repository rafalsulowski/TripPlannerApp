using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace TripPlanner.ViewModels.Home
{
    public partial class CalendarViewModel : ObservableObject
    {
        [RelayCommand]
        async Task GoBack()
        {
            await Shell.Current.GoToAsync("..");
        }

        [RelayCommand]
        async Task GoSettings()
        {
            await Shell.Current.GoToAsync("..");
        }
    }
}
