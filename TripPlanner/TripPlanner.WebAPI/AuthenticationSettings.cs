namespace TripPlanner.WebAPI
{
    public static class AuthenticationSettings
    {
        public static string JwtKey { get; set; } = "sauidhufihaiuhfuidshfuihdfuisdhiugdsuighsuidhfuisdhuisdhufigsdhuighsduihgui";
        public static string Issuer { get; set; } = "https://localhost:44441;http://localhost:5074";
        public static string Audience { get; set; } = "*"; //"https://localhost:44441;http://localhost:5074";
        public static double JwtExpiresDays { get; set; } = 1;
        public static int IpRateLimit { get; set; } = 2;
        public static string IpRateLimitPolicies { get; set; } = "RateLimitsPolicies";
        public static string EmailAppAdress { get; set; } = "rafali13@wp.pl";
        public static string EmailAppPassword { get; set; } = "Rafal130801*";
    }
}
