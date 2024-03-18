using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Net;
using System.Security.Claims;
using System.Text;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace TripPlanner.WebAPI.Middlewares
{
    public class AuthenticationMiddleware
    {
        private readonly ILogger<ExceptionHandlerMiddleware> _logger;
        private readonly RequestDelegate _next;

        public AuthenticationMiddleware(
            ILogger<ExceptionHandlerMiddleware> logger,
            RequestDelegate next)
        {
            _logger = logger;
            _next = next;
        }

        public async Task InvokeAsync(HttpContext httpContext)
        {
            if (httpContext.Request.Path == "/api/User/login")
                await _next(httpContext);
            else
            {
                var token = httpContext.Request.Headers["Authorization"].ToString().Replace("Bearer ", "");
                try
                {
                    // Create a token handler and validate the token
                    var tokenHandler = new JwtSecurityTokenHandler();
                    var claimsPrincipal = tokenHandler.ValidateToken(token, new TokenValidationParameters
                    {
                        ValidateIssuerSigningKey = true,
                        ValidIssuer = AuthenticationSettings.Issuer,
                        ValidAudience = AuthenticationSettings.Audience,
                        IssuerSigningKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(AuthenticationSettings.JwtKey))
                    }, out SecurityToken validatedToken);

                    var userId = claimsPrincipal.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                    httpContext.Items["UserId"] = userId;
                    await _next(httpContext);
                }
                catch (Exception)
                {
                    throw new Exception("Użytkownik nie nieautoryzowany");
                }
            }
        }
    }
}
