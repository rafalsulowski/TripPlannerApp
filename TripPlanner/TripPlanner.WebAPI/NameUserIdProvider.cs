using Microsoft.AspNetCore.SignalR;
using System.Security.Claims;

namespace TripPlanner.WebAPI
{
    public class NameUserIdProvider : IUserIdProvider
    {
        public string GetUserId(HubConnectionContext connection)
        {
            return connection.User?.FindFirst(ClaimTypes.Email)?.Value!;
        }
    }
}
