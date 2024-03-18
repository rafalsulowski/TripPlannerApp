using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TripPlanner.ViewModels.Home
{
    public partial class ConfigurationViewModel : ObservableObject
    {
        [ObservableProperty]
        bool isChecked;

        public ConfigurationViewModel()
        {
        }

        [RelayCommand]
        async void Appearing()
        {
            IsChecked = Preferences.Default.Get("DarkTheme", false); //false == domyślnie tryb jasny
        }


        [RelayCommand]
        async Task GoBack()
        {
            await Shell.Current.GoToAsync("///Profile");
        }

        [RelayCommand]
        async Task ChangeTheme()
        {
            IsChecked = !IsChecked;
            if (IsChecked)
            {
                Preferences.Default.Set("DarkTheme", true);
                Application.Current.UserAppTheme = AppTheme.Dark;
            }
            else
            {
                Preferences.Default.Set("DarkTheme", false);
                Application.Current.UserAppTheme = AppTheme.Light;
            }
            await Shell.Current.GoToAsync("/Settings", false);
        }
    }
}
