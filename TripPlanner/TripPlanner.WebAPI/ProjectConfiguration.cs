namespace TripPlanner.WebAPI
{
    public static class ProjectConfiguration
    {
        public const bool HideContorller = true;
        
        public const string UrlToConfirmChangeAccountPassword = "http://localhost:5119/api/User/changeAccountPassword/"; //musi korespondowac z koncówką nazwy endpointu wołanego po wejsciu w link z maila
        public const string UrlToConfirmActivateAccount = "http://localhost:5119/api/User/activateAccount/"; //musi korespondowac z koncówką nazwy endpointu wołanego po wejsciu w link z maila

        public const string ClientMauiAppVersion = "1.0.0";
        public const string ClientWebAppVersion = "1.0.0";
        public const string ServerVersion = "1.0.0";
        public const string DataBaseVersion = "1.0.0";
        public const string WholeSystemVersion = "1.0.0";
    }
}
