
using Microsoft.Maui.ApplicationModel;
using OnScreenSizeMarkup.Maui.Categories;
using OnScreenSizeMarkup.Maui.Mappings;

namespace TripPlanner;
public partial class App : Application
{

    public App()
    {
        //OnScreenSizeMarkup.Maui.Manager.Current.Mappings = new List<SizeMappingInfo>
        //{
        //    new SizeMappingInfo(3.9, ScreenCategories.ExtraSmall, ScreenSizeCompareModes.SmallerOrEqualsTo),
        //    new SizeMappingInfo(4.9, ScreenCategories.Small, ScreenSizeCompareModes.SmallerOrEqualsTo),
        //    new SizeMappingInfo(6.2, ScreenCategories.Medium, ScreenSizeCompareModes.SmallerOrEqualsTo),
        //    new SizeMappingInfo(7.9, ScreenCategories.Large, ScreenSizeCompareModes.SmallerOrEqualsTo),
        //    new SizeMappingInfo(200.0, ScreenCategories.ExtraLarge, ScreenSizeCompareModes.SmallerOrEqualsTo),
        //};

        InitializeComponent();
        MainPage = new AppShell();
        Shell.Current.GoToAsync("Start");
    }
}
