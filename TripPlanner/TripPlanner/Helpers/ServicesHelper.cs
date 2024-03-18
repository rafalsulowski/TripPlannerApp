
namespace TripPlanner.Helpers
{
    public static class ServicesHelper
    {
        public static object GetService<TService>()
        {
            return Current.GetService<TService>();
        }

        public static IServiceProvider Current =>
#if WINDOWS10_0_19041_0_OR_GREATER
                    MauiWinUIApplication.Current.Services;
#elif ANDROID
                    MauiApplication.Current.Services;
#elif IOS || MACCATALYST
                    MauiUIApplicationDelegate.Current.Services;
#else
                    null;
#endif
    }
}
