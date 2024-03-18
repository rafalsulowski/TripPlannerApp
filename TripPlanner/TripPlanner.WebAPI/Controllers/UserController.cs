using Microsoft.IdentityModel.Tokens;
using TripPlanner.Services.UserService;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using TripPlanner.Models.DTO.UserDTOs;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;
using TripPlanner.Models.Models;
using TripPlanner.Models.DTO.TourDTOs;
using TripPlanner.Models.Models.TourModels;
using TripPlanner.Services.TourService;
using TripPlanner.Models.Models.UserModels;
using TripPlanner.Services.FriendService;
using TripPlanner.Services.Notificationservice;
using TripPlanner.Services.ActiviceCodeService;
using System.Net.Mail;
using System.Net;
using TripPlanner.Models.DTO.BillDTOs;
using Microsoft.AspNetCore.Authorization;

namespace TripPlanner.WebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly IUserService _UserService;
        private readonly INotificationService _NotificationService;
        private readonly IActiviteCodeService _ActiviteCodeService;
        private readonly ITourService _TourService;
        private readonly IFriendService _FriendService;
        private readonly IPasswordHasher<User> _passwordHasher;

        public UserController(IUserService userService, IPasswordHasher<User> passwordHasher, ITourService tourService, IFriendService friendService, INotificationService notificationService, IActiviteCodeService activiteCodeService)
        {
            _UserService = userService;
            _passwordHasher = passwordHasher;
            _TourService = tourService;
            _FriendService = friendService;
            _NotificationService = notificationService;
            _ActiviteCodeService = activiteCodeService;
        }


        [Authorize]
        [HttpGet]
        public async Task<ActionResult<List<UserDTO>>> Get()
        {
            var response = await _UserService.GetUsersAsync();
            return Ok(response.Data?.Select(u => (UserDTO)u).ToList());
        }


        [Authorize]
        [HttpGet("extendUserDto/{userId}")]
        public async Task<ActionResult<List<ExtendUserDTO>>> ExtendUserDto(int userId)
        {
            var response = await _UserService.ExtendUserDto(userId);
            return response.Data;
        }


        [Authorize]
        [HttpGet("getToursOfUser/{userId}")]
        public async Task<ActionResult<List<TourDTO>>> GetToursOfUser(int userId)
        {
            string? authUserId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (authUserId == null)
                return Forbid("Musisz być zalogowany do przeprowadzenia tej akcji!");
            userId = int.Parse(authUserId);

            var response = await _UserService.GetUserAsync(u => u.Id == userId, "ParticipantTours");
            UserDTO res = response.Data;
            List<TourDTO> tours = new List<TourDTO>();

            if (response.Data == null || res.ParticipantTours.Count == 0)
                return Ok(tours);

            foreach(ParticipantTour participant in res.ParticipantTours)
            {
                var resp = await _TourService.GetTourAsync(u => u.Id == participant.TourId);
                if (resp.Data is not null)
                    tours.Add(resp.Data);
            }

            return Ok(tours);
        }

        //zwraca liste znajomych uzytkownika o danym id
        [Authorize]
        [HttpGet("{userId}/getFriends")]
        public async Task<ActionResult<List<ExtendFriendDTO>>> GetFriends(int userId)
        {
            RepositoryResponse<List<ExtendFriendDTO>> response = await _UserService.GetFriends(userId, -1);
            return Ok(response.Data);
        }
        
        //zwraca powiadomienie o danym id
        [Authorize]
        [HttpGet("getNotificationById/{NotifyId}")]
        public async Task<ActionResult<NotificationDTO>> GetNotificationById(int NotifyId)
        {
            RepositoryResponse<Notification> response = await _NotificationService.GetNotificationAsync(u => u.Id == NotifyId);
            return Ok((NotificationDTO)response.Data);
        }

        //zwraca liste znajomych uzytkownika o danym id, wraz z informacjami czy przyjaciele nalezą do wycieczki o tourId
        [Authorize]
        [HttpGet("{userId}/getFriendsWithInfoAboutTour/{tourId}")]
        public async Task<ActionResult<List<ExtendFriendDTO>>> GetFriendsWithInfoAboutTour(int userId, int tourId)
        {
            RepositoryResponse<List<ExtendFriendDTO>> response = await _UserService.GetFriends(userId, tourId);
            return Ok(response.Data);
        }

        [Authorize]
        [HttpGet("{UserId}/getWithCheckLists")]
        public async Task<ActionResult<UserDTO>> GetWithCheckLists(int UserId)
        {
            RepositoryResponse<User> response = await _UserService.GetUserAsync(u => u.Id == UserId, "CheckLists");
            return Ok((UserDTO)response.Data);
        }

        [Authorize]
        [HttpGet("{UserId}/getWithShares")]
        public async Task<ActionResult<UserDTO>> GetWithShares(int UserId)
        {
            var response = await _UserService.GetUserAsync(u => u.Id == UserId, "Shares");
            return Ok((UserDTO)response.Data);
        }

        [Authorize]
        [HttpGet("{UserId}/getWithParticipantTours")]
        public async Task<ActionResult<UserDTO>> GetWithParticipantTours(int UserId)
        {
            var response = await _UserService.GetUserAsync(u => u.Id == UserId, "ParticipantTours");
            return Ok((UserDTO)response.Data);
        }

        [Authorize]
        [HttpGet("{UserId}/getWithQuestionnaireVotes")]
        public async Task<ActionResult<UserDTO>> GetWithQuestionnaireVotes(int UserId)
        {
            var response = await _UserService.GetUserAsync(u => u.Id == UserId, "QuestionnaireVotes");
            return Ok((UserDTO)response.Data);
        }

        [Authorize]
        [HttpGet("{UserId}/getWithRoutes")]
        public async Task<ActionResult<UserDTO>> GetWithRoutes(int UserId)
        {
            var response = await _UserService.GetUserAsync(u => u.Id == UserId, "Routes");
            return Ok((UserDTO)response.Data);
        }

        [Authorize]
        [HttpGet("{UserId}/getWithMessages")]
        public async Task<ActionResult<UserDTO>> GetWithMessages(int UserId)
        {
            var response = await _UserService.GetUserAsync(u => u.Id == UserId, "Messages");
            return Ok((UserDTO)response.Data);
        }

        [Authorize]
        [HttpGet("{UserId}/getWithBillContributors")]
        public async Task<ActionResult<UserDTO>> GetWithBillContributors(int UserId)
        {
            var response = await _UserService.GetUserAsync(u => u.Id == UserId, "BillContributors");
            return Ok((UserDTO)response.Data);
        }

        [Authorize]
        [HttpGet("{UserId}/getWithBillPayed")]
        public async Task<ActionResult<UserDTO>> GetWithBillPayed(int UserId)
        {
            var response = await _UserService.GetUserAsync(u => u.Id == UserId, "BillsPayed");
            return Ok((UserDTO)response.Data);
        }

        [Authorize]
        [HttpGet("{UserId}/getWithTransfersSender")]
        public async Task<ActionResult<UserDTO>> GetWithTransfersSender(int UserId)
        {
            var response = await _UserService.GetUserAsync(u => u.Id == UserId, "TransfersSender");
            return Ok((UserDTO)response.Data);
        }

        [Authorize]
        [HttpGet("{UserId}/getWithTransfersRecipient")]
        public async Task<ActionResult<UserDTO>> GetWithTransfersRecipient(int UserId)
        {
            var response = await _UserService.GetUserAsync(u => u.Id == UserId, "TransfersRecipient");
            return Ok((UserDTO)response.Data);
        }

        [Authorize]
        [HttpGet("{id}")]
        public async Task<ActionResult<UserDTO>> Get(int id)
        {
            var response = await _UserService.GetUserAsync(u => u.Id == id);
            return Ok((UserDTO)response.Data);
        }

        [Authorize]
        [HttpGet("email/{email}")]
        public async Task<ActionResult<UserDTO>> GetByEmail(string email)
        {
            var response = await _UserService.GetUserAsync(u => u.Email == email);
            return Ok((UserDTO)response.Data);
        }


        [Authorize]
        [HttpPost("emailIsFree")]
        public async Task<ActionResult<bool>> EmailIsFree([FromBody] string email)
        {
            var userResponse = await _UserService.GetUserAsync(u => u.Email == email);

            if (userResponse.Data != null)
                return false;
            else
                return true;
        }

        //Rejestracja
        [HttpPost]
        public async Task<ActionResult<RepositoryResponse<bool>>> Create([FromBody] CreateUserDTO user)
        {
            var userResponse = await _UserService.GetUserAsync(u => u.Email == user.Email);
            if (userResponse.Data != null)
            {
                return new RepositoryResponse<bool> { Data = false, Success = false, Message = "Użytkownik z tym e-mail'em istnieje" };
            }

            User newUser = user;
            newUser.IsActivated = false;

            var hashed = _passwordHasher.HashPassword(newUser, user.PasswordHash);
            newUser.PasswordHash = hashed;
            var response = await _UserService.CreateUser(newUser); 
            if (!response.Success)
                return NotFound(new RepositoryResponse<bool> { Data = false, Success = false, Message = response.Message });

            var response2 = await SendEmail(newUser, true);
            if (response2.Success)
                return Ok(new RepositoryResponse<bool> { Data = true, Success = true, Message = "" });
            else
            {
                await _UserService.DeleteUser(newUser);
                return BadRequest(new RepositoryResponse<bool> { Data = false, Success = false, Message = $"Konto nie zostało utworzone. {response2.Message}"});
            }
        }


        [Authorize]
        [HttpGet("activateAccount/{key}")]
        public ContentResult ActivateAccount(string key)
        {
            var keyResp = _ActiviteCodeService.GetActiviteCodeOfUser(key).Result;
            var response = new HttpResponseMessage();

            if(keyResp.Success == false || keyResp.Data == null)
            {
                return new ContentResult
                {
                    ContentType = "text/html",
                    StatusCode = (int)HttpStatusCode.NotFound,
                    Content = System.IO.File.ReadAllText("HtmlPages\\FailureunAvailable.html")
                };
            }
            else if(keyResp.Data.ExpireDate < DateTime.Now)
            {
                return new ContentResult
                {
                    ContentType = "text/html",
                    StatusCode = (int)HttpStatusCode.BadRequest,
                    Content = System.IO.File.ReadAllText("HtmlPages\\FailureExpired.html")
                };
            }

            var user = _UserService.GetUserAsync(u => u.Id == keyResp.Data.UserId).Result;
            if (user.Success == false || user.Data == null)
            {
                return new ContentResult
                {
                    ContentType = "text/html",
                    StatusCode = (int)HttpStatusCode.NotFound,
                    Content = System.IO.File.ReadAllText("HtmlPages\\FailureUser.html")
                };
            }

            User us = user.Data;
            us.IsActivated = true;
            var resp = _UserService.UpdateUser(us).Result;
            if (resp.Success)
            {
                _ActiviteCodeService.DeleteActiviteCode(keyResp.Data);

                return new ContentResult
                {
                    ContentType = "text/html",
                    StatusCode = (int)HttpStatusCode.OK,
                    Content = System.IO.File.ReadAllText("HtmlPages\\Success.html")
                };
            }
            else
            {
                return new ContentResult
                {
                    ContentType = "text/html",
                    StatusCode = (int)HttpStatusCode.BadRequest,
                    Content = System.IO.File.ReadAllText("HtmlPages\\FailureOther.html")
                };
            }
        }


        [Authorize]
        [HttpGet("changeAccountPassword/{key}")]
        public ContentResult ChangeAccountPassword(string key)
        {
            var keyResp = _ActiviteCodeService.GetActiviteCodeOfUser(key).Result;
            var response = new HttpResponseMessage();
            if (keyResp.Success == false || keyResp.Data == null)
            {
                return new ContentResult
                {
                    ContentType = "text/html",
                    StatusCode = (int)HttpStatusCode.NotFound,
                    Content = System.IO.File.ReadAllText("HtmlPages\\FailureUnAvailable.html")
                };
            }
            else if (keyResp.Data.ExpireDate < DateTime.Now)
            {
                return new ContentResult
                {
                    ContentType = "text/html",
                    StatusCode = (int)HttpStatusCode.BadRequest,
                    Content = System.IO.File.ReadAllText("HtmlPages\\FailureExpired.html")
                };
            }

            var user = _UserService.GetUserAsync(u => u.Id == keyResp.Data.UserId).Result;
            if (user.Success == false || user.Data == null)
            {
                return new ContentResult
                {
                    ContentType = "text/html",
                    StatusCode = (int)HttpStatusCode.NotFound,
                    Content = System.IO.File.ReadAllText("HtmlPages\\FailureUser.html")
                };
            }

            User us = user.Data;
            us.IsActivated = true;
            var resp = _UserService.UpdateUser(us).Result;
            if (resp.Success)
            {
                _ActiviteCodeService.DeleteActiviteCode(keyResp.Data);

                return new ContentResult
                {
                    ContentType = "text/html",
                    StatusCode = (int)HttpStatusCode.OK,
                    Content = System.IO.File.ReadAllText("HtmlPages\\SuccessChangePass.html")
                };
            }
            else
            {
                return new ContentResult
                {
                    ContentType = "text/html",
                    StatusCode = (int)HttpStatusCode.BadRequest,
                    Content = System.IO.File.ReadAllText("HtmlPages\\FailureOther.html")
                };
            }
        }


        [Authorize]
        [HttpPost("addFriend")]
        public async Task<ActionResult<RepositoryResponse<bool>>> AddFriend([FromBody] FriendDTO Tour)
        {
            if(Tour.Friend1Id == Tour.Friend2Id)
            {
                return new RepositoryResponse<bool> { Data = false, Success = false, Message = $"Podano takie same id użytkowników" };
            }
            var resp = await _UserService.GetUserAsync(u => u.Id == Tour.Friend1Id);
            if (resp.Data == null)
            {
                return new RepositoryResponse<bool> { Data = false, Success = false, Message = $"Nie istnieje użytkownik o id = {Tour.Friend1Id}" };
            }
            var resp2 = await _UserService.GetUserAsync(u => u.Id == Tour.Friend2Id);
            if (resp2.Data == null)
            {
                return new RepositoryResponse<bool> { Data = false, Success = false, Message = $"Nie istnieje użytkownik o id = {Tour.Friend2Id}" };
            }
            var resp3 = _UserService.GetFriendAsync(u => u.Friend1Id == Tour.Friend1Id && u.Friend2Id == Tour.Friend2Id);
            if (resp3 != null)
            {
                if(resp3.Result.Data is not null)
                    return new RepositoryResponse<bool> { Data = false, Success = false, Message = $"Użytkownicy są już znajomymi" };
            }
            var resp4 = _UserService.GetFriendAsync(u => u.Friend1Id == Tour.Friend2Id && u.Friend2Id == Tour.Friend1Id);
            if (resp4 != null)
            {
                if (resp4.Result.Data is not null)
                    return new RepositoryResponse<bool> { Data = false, Success = false, Message = $"Użytkownicy są już znajomymi" };
            }

            Friend elem = Tour;

            var response = await _FriendService.CreateFriend(elem); 
            if (response.Success)
                return Ok(new RepositoryResponse<bool> { Data = true, Success = true, Message = "" });
            else
                return NotFound(new RepositoryResponse<bool> { Data = false, Success = false, Message = response.Message });
        }


        [Authorize]
        [HttpDelete("{userId}/deleteFriend/{user2Id}")]
        public async Task<ActionResult<RepositoryResponse<bool>>> DeleteFriend(int userId, int user2Id)
        {
            var resp = await _UserService.GetUserAsync(u => u.Id == userId);
            if (resp.Data == null)
            {
                return new RepositoryResponse<bool> { Data = false, Success = false, Message = $"Nie istnieje użytkownik o id = {userId}" };
            }
            var resp2 = await _UserService.GetUserAsync(u => u.Id == user2Id);
            if (resp2.Data == null)
            {
                return new RepositoryResponse<bool> { Data = false, Success = false, Message = $"Nie istnieje użytkownik o id = {userId}" };
            }

            var resp3 = await _UserService.GetFriendAsync(u => u.Friend1Id == userId && u.Friend2Id == user2Id);
            var resp4 = await _UserService.GetFriendAsync(u => u.Friend1Id == user2Id && u.Friend2Id == userId);
            if (resp3.Data == null && resp4.Data == null)
                return Ok(new RepositoryResponse<bool> { Data = true, Success = true, Message = "" });

            Friend elem = new Friend
            {
                Friend1Id = userId,
                Friend2Id = user2Id
            };

            if (resp3.Data != null)
                elem = resp3.Data;
            else
                elem = resp4.Data;

            var response2 = await _FriendService.DeleteFriend(elem);
            if (response2.Success)
                return Ok(new RepositoryResponse<bool> { Data = true, Success = true, Message = "" });
            else
                return NotFound(new RepositoryResponse<bool> { Data = false, Success = false, Message = response2.Message });
        }


        [Authorize]
        [HttpPut("{userId}/changePassword")]
        public async Task<ActionResult<RepositoryResponse<bool>>> ChangePassword(int userId, [FromBody] string pass)
        {
            var userResponse = await _UserService.GetUserAsync(u => u.Id == userId);
            if (userResponse.Data == null)
            {
                return new RepositoryResponse<bool> { Data = false, Success = false, Message = $"Nie istnieje uzytkownik o id = {userId}" };
            }

            User newUser = userResponse.Data;
            var oldPassHash = newUser.PasswordHash;
            var hashed = _passwordHasher.HashPassword(newUser, pass);
            newUser.PasswordHash = hashed;
            newUser.IsActivated = false;

            var response = await _UserService.UpdateUser(newUser);
            if (!response.Success)
                return new RepositoryResponse<bool> { Data = false, Success = false, Message = response.Message };

            var response2 = await SendEmail(newUser, true);
            if (response2.Success)
                return Ok(new RepositoryResponse<bool> { Data = true, Success = true, Message = "" });
            else
            {
                newUser.PasswordHash = oldPassHash;
                newUser.IsActivated = true;
                await _UserService.UpdateUser(newUser);
                return BadRequest(new RepositoryResponse<bool> { Data = false, Success = false, Message = $"Hasło nie zostało zmienione. {response2.Message}" });
            }
        }


        [Authorize]
        [HttpPut("{id}")]
        public async Task<ActionResult<RepositoryResponse<bool>>> Put(int id, [FromBody] CreateUserDTO user)
        {
            var userResponse = await _UserService.GetUserAsync(u => u.Id == id);
            if (userResponse.Data == null)
            {
                return new RepositoryResponse<bool> { Data = false, Success = false, Message = $"Nie istnieje uzytkownik o id = {id}" };
            }

            User newUser = userResponse.Data;

            if(newUser.Email != user.Email)
            {
                var resp = await _UserService.GetUserAsync(u => u.Email == user.Email);
                if (resp.Data != null)
                    return new RepositoryResponse<bool> { Data = false, Success = false, Message = $"Podany emial jest zajęty" };
            }

            newUser.Email = user.Email;
            newUser.IsActivated = user.IsActivated;
            newUser.FullAddress = user.FullAddress;
            newUser.City = user.City;
            newUser.DateOfBirth = user.DateOfBirth;
            newUser.Phone = user.Phone;
            newUser.FullName = user.FullName;
            newUser.Id = id;

            var response = await _UserService.UpdateUser(newUser);
            if (response.Success)
                return Ok(new RepositoryResponse<bool> { Data = true, Success = true, Message = "" });
            else
                return NotFound(new RepositoryResponse<bool> { Data = false, Success = false, Message = response.Message });
        }


        [Authorize]
        [HttpDelete("{id}")]
        public async Task<ActionResult<RepositoryResponse<bool>>> Delete(int id)
        {
            var response = await _UserService.DeleteUser(new User() { Id = id });
            if (response.Success)
                return Ok(new RepositoryResponse<bool> { Data = true, Success = true, Message = "" });
            else
                return NotFound(new RepositoryResponse<bool> { Data = false, Success = false, Message = response.Message });
        }


        [HttpPost("login")]
        public async Task<ActionResult<RepositoryResponse<string>>> Login([FromBody] LoginDTO loginUser)
        {
            var userResponse = await _UserService.GetUserAsync(u => u.Email == loginUser.Email);

            if (userResponse.Data == null)
                return new RepositoryResponse<string> { Message = "Błędne dane logowania", Data = "", Success = false };

            if (userResponse.Data.IsActivated == false)
                return new RepositoryResponse<string> { Message = "Użytkownik nie aktywował konta za pomocą linka wysłanego w wiadomości e-mail", Data = "", Success = false };

            var user = userResponse.Data;

            var result = _passwordHasher.VerifyHashedPassword(user, user.PasswordHash, loginUser.PasswordHash);
            if (result == PasswordVerificationResult.Failed)
                return new RepositoryResponse<string> { Message = "Błędne dane logowania", Data = "", Success = false };

            var claims = new List<Claim>()
            {
                new Claim(ClaimTypes.Name, user.FullName.ToString()),
                new Claim(ClaimTypes.DateOfBirth, user.DateOfBirth.ToString()),
                new Claim(ClaimTypes.StateOrProvince, user.FullAddress.ToString()),
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Email, user.Email.ToString())
            };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(AuthenticationSettings.JwtKey));
            var cred = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
            var expires = DateTime.Now.AddDays(AuthenticationSettings.JwtExpiresDays);

            var token = new JwtSecurityToken(AuthenticationSettings.Issuer,
                AuthenticationSettings.Audience,
                claims,
                expires: expires,
                signingCredentials: cred);
            var tokenHandler = new JwtSecurityTokenHandler();
            string tokenString = tokenHandler.WriteToken(token);
            return new RepositoryResponse<string> { Message = "", Data = tokenString, Success = true };
        }


        [Authorize]
        [HttpPost("{userId}/sendRemindEmail/{tourId}")]
        public async Task<ActionResult<RepositoryResponse<bool>>> SendRemindEmail(int userId, int tourId, [FromBody] OtherUser pass)
        {
            var userResponse = await _UserService.GetUserAsync(u => u.Id == userId);
            if (userResponse.Data == null)
            {
                return new RepositoryResponse<bool> { Data = false, Success = false, Message = $"Nie istnieje uzytkownik o id = {userId}" };
            }

            var userResponse2 = await _UserService.GetUserAsync(u => u.Id == pass.UserId);
            if (userResponse2.Data == null)
            {
                return new RepositoryResponse<bool> { Data = false, Success = false, Message = $"Nie istnieje uzytkownik o id = {pass.UserId}" };
            }

            var resp2 = await _TourService.GetTourAsync(u => u.Id == tourId);
            if (resp2.Data == null)
            {
                return new RepositoryResponse<bool> { Success = false, Message = $"Nie istnieje wyjazd o id = {tourId}" };
            }

            try
            {
                MailAddress fromMail = new MailAddress(AuthenticationSettings.EmailAppAdress);
                MailAddress toMail = new MailAddress(userResponse2.Data.Email);

                MailMessage email = new MailMessage(fromMail, toMail);
                email.Subject = $"Przypomienie wyregulowania należności do {userResponse.Data.FullName}";           
                email.Body = $"<br/><br/><h1>Witaj {userResponse2.Data.FullName}</h1><br/><br/>Zostałeś właśnie poproszony przez jednego z uczestników wyjazdu {resp2.Data.Title}, o wyregulowanie zaległości pieniężnych<br/>" +
                    $"z użytkownikiem {userResponse.Data.FullName}, w wysokości {pass.Saldo} PLN. Nie zwlekaj i ureguluj wszystkie zaległości już teraz" +
                    "<br/><br/><br/>Ta wiadomość została wygenerowana automatycznie i prosimy na nią nie odpowiadać.<br/><br/>Z poważaniem<br/><br/>Zespół TripPlanner.com";
                email.IsBodyHtml = true;

                SmtpClient smtp = new SmtpClient();
                smtp.Timeout = 30000;
                smtp.UseDefaultCredentials = false;
                smtp.Host = "smtp.wp.pl";
                smtp.Port = 587;
                smtp.Credentials = new NetworkCredential(AuthenticationSettings.EmailAppAdress, AuthenticationSettings.EmailAppPassword);
                smtp.DeliveryMethod = SmtpDeliveryMethod.Network;
                smtp.EnableSsl = true;

                await smtp.SendMailAsync(email);
                return new RepositoryResponse<bool> { Data = true, Success = true, Message = "" };
            }
            catch (SmtpException ex)
            {
                return new RepositoryResponse<bool> { Data = false, Success = false, Message = ex.Message };
            }
        }

        private async Task<RepositoryResponse<bool>> SendEmail(User newUser, bool passwordChange)
        {
            Random random = new Random();
            const string chars = "abcdefghijklmnoprstuwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            string key = new string(Enumerable.Repeat(chars, 20)
                .Select(s => s[random.Next(s.Length)]).ToArray());

            string varifyUrl = "";
            if(passwordChange)
                varifyUrl = $"{ProjectConfiguration.UrlToConfirmChangeAccountPassword}{key}";
            else
                varifyUrl = $"{ProjectConfiguration.UrlToConfirmActivateAccount}{key}";


            ActiviteCode code = new ActiviteCode
            {
                UserId = newUser.Id,
                Code = key,
                ExpireDate = DateTime.Now + TimeSpan.FromDays(1),
            };
            var response2 = await _ActiviteCodeService.CreateActiviteCode(code);
            if (!response2.Success)
                return new RepositoryResponse<bool> { Data = false, Success = false, Message = response2.Message };

            try
            {
                MailAddress fromMail = new MailAddress(AuthenticationSettings.EmailAppAdress);
                MailAddress toMail = new MailAddress(newUser.Email);

                MailMessage email = new MailMessage(fromMail, toMail);
                email.Subject = "Witaj w TripPlanner";
                if(passwordChange)
                    email.Body = $"<br/><br/><h1>Witaj {newUser.FullName}</h1><br/><br/>W celu zatwierdzenia zmiany hasła kliknij w link poniżej<br/>" +
                        " <br/><br/><a href='" + varifyUrl + "'>" + varifyUrl + "</a> " +
                        "<br/><br/><br/>Ta wiadomość została wygenerowana automatycznie i prosimy na nią nie odpowiadać.<br/><br/>Z poważaniem<br/><br/>Zespół TripPlanner.com";
                else
                    email.Body = $"<br/><br/><h1>Witaj {newUser.FullName}</h1><br/><br/>Bardzo cieszymy się, z powodu twojej rejestracji w TripPlanner, systemie do organizacji wyjazdów grupowych.<br/>Mam nadzieję" +
                        ", że razem z oprogramowaniem naszej firmy uda ci się zorganizować wspaniałe wyjazdy i przeżyć przez to niezapomiane chwile.<br/>" +
                        "W pierwszej kolejności jednak musisz aktywować swoje konto klikając w link poniżej:" +
                        " <br/><br/><a href='" + varifyUrl + "'>" + varifyUrl + "</a> " +
                        "<br/><br/><br/>Ta wiadomość została wygenerowana automatycznie i prosimy na nią nie odpowiadać.<br/><br/>Z poważaniem<br/><br/>Zespół TripPlanner.com";
                email.IsBodyHtml = true;

                SmtpClient smtp = new SmtpClient();
                smtp.Timeout = 10000;
                smtp.UseDefaultCredentials = false;
                smtp.Host = "smtp.wp.pl";
                smtp.Port = 587;
                smtp.Credentials = new NetworkCredential(AuthenticationSettings.EmailAppAdress, AuthenticationSettings.EmailAppPassword);
                smtp.DeliveryMethod = SmtpDeliveryMethod.Network;
                smtp.EnableSsl = true;

                await smtp.SendMailAsync(email);
                return new RepositoryResponse<bool> { Data = true, Success = true, Message = "" };
            }
            catch (SmtpException ex)
            {
                return new RepositoryResponse<bool> { Data = false, Success = false, Message = ex.Message };
            }
        }
    }
}
