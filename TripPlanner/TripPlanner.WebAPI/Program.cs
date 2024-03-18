using Microsoft.EntityFrameworkCore;
using TripPlanner.DataAccess.Repository;
using TripPlanner.DataAccess;
using TripPlanner.Services.UserService;
using Microsoft.AspNetCore.Identity;
using TripPlanner.Models;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using TripPlanner.Services.ChatService;
using TripPlanner.Services.CheckListService;
using TripPlanner.Services.CheckListFieldService;
using TripPlanner.Services.CultureService;
using TripPlanner.Services.CultureAssistanceService;
using TripPlanner.Services.ParticipantTourService;
using TripPlanner.Services.QuestionnaireService;
using TripPlanner.Services.QuestionnaireAnswerService;
using TripPlanner.Services.QuestionnaireVoteService;
using TripPlanner.Services.RouteService;
using TripPlanner.Services.StopoverService;
using TripPlanner.Services.TourService;
using TripPlanner.Services.BillService;
using TripPlanner.Services.ScheduleService;
using TripPlanner.Models.Models.UserModels;
using TripPlanner.Services.FriendService;
using TripPlanner.Services.Notificationservice;
using TripPlanner.Services.NotificationService;
using Microsoft.AspNetCore.SignalR;
using TripPlanner.DataAccess.IRepository;
using TripPlanner.Services.ActiviceCodeService;
using TripPlanner.WebAPI.Middlewares;
using TripPlanner.WebAPI.Hubs;
using Microsoft.AspNetCore.RateLimiting;
using System.Threading.RateLimiting;
using Microsoft.OpenApi.Models;

namespace TripPlanner.WebAPI
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            builder.Services.AddAuthentication(option =>
            {
                option.DefaultAuthenticateScheme = "Bearer";
                option.DefaultScheme = "Bearer";
                option.DefaultChallengeScheme = "Bearer";
            }).AddJwtBearer(cfg =>
            {
                cfg.RequireHttpsMetadata = false;
                cfg.SaveToken = true;
                cfg.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidIssuer = AuthenticationSettings.Issuer,
                    ValidAudience = AuthenticationSettings.Audience,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(AuthenticationSettings.JwtKey))
                };
            });
            builder.Services.AddAuthorization();

            builder.Services.AddRateLimiter(options =>
            {
                options.RejectionStatusCode = StatusCodes.Status429TooManyRequests;

                options.AddPolicy("IpAddressPolicy", httpContext =>
                RateLimitPartition.GetFixedWindowLimiter(
                    partitionKey: httpContext.Connection.RemoteIpAddress?.ToString(),
                    factory: _ => new FixedWindowRateLimiterOptions
                    {
                        PermitLimit = 10,
                        Window = TimeSpan.FromSeconds(10)
                    }));
            });

            builder.Services.AddControllers();
            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen(option =>
            {
                option.SwaggerDoc("v1", new OpenApiInfo { Title = "TripSyncAPI", Version = "v1" });
                option.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
                {
                    In = ParameterLocation.Header,
                    Description = "Please enter a valid token",
                    Name = "Authorization",
                    Type = SecuritySchemeType.Http,
                    BearerFormat = "JWT",
                    Scheme = "Bearer"
                });
                option.AddSecurityRequirement(new OpenApiSecurityRequirement
                {
                    {
                        new OpenApiSecurityScheme
                        {
                            Reference = new OpenApiReference
                            {
                                Type=ReferenceType.SecurityScheme,
                                Id="Bearer"
                            }
                        },
                        new string[]{}
                    }
                });
            });

            builder.Services.AddSignalR();
            builder.Services.AddSingleton<IUserIdProvider, NameUserIdProvider>();

            if (Environment.MachineName == "RMSULOWSKR")
            {
                builder.Services.AddDbContext<ApplicationDbContext>(options => options.UseSqlServer(
                builder.Configuration.GetConnectionString("SqlConnectionString")), ServiceLifetime.Scoped); //TODO wy³aczyc dla publikacji
            }
            else
            {
                builder.Services.AddDbContext<ApplicationDbContext>(options => options.UseSqlServer(
                builder.Configuration.GetConnectionString("SqlConnectionStringACERRS")), ServiceLifetime.Scoped);
            }

            builder.Services.AddScoped<IUserRepository, UserRepository>();
            builder.Services.AddScoped<ICheckListRepository, CheckListRepository>();
            builder.Services.AddScoped<ICheckListFieldRepository, CheckListFieldRepository>();
            builder.Services.AddScoped<ICultureRepository, CultureRepository>();
            builder.Services.AddScoped<ICultureAssistanceRepository, CultureAssistanceRepository>();
            builder.Services.AddScoped<IParticipantTourRepository, ParticipantTourRepository>();
            builder.Services.AddScoped<IQuestionnaireVoteRepository, QuestionnaireVoteRepository>();
            builder.Services.AddScoped<IQuestionnaireAnswerRepository, QuestionnaireAnswerRepository>();
            builder.Services.AddScoped<IQuestionnaireRepository, QuestionnaireRepository>();
            builder.Services.AddScoped<IRouteRepository, RouteRepository>();
            builder.Services.AddScoped<IStopoverRepository, StopoverRepository>();
            builder.Services.AddScoped<ITextMessageRepository, TextMessageRepository>();
            builder.Services.AddScoped<INoticeMessageRepository, NoticeMessageRepository>();
            builder.Services.AddScoped<ITourRepository, TourRepository>();
            builder.Services.AddScoped<IBillRepository, BillRepository>();
            builder.Services.AddScoped<ITransferRepository, TransferRepository>();
            builder.Services.AddScoped<IBillContributorRepository, BillContributorRepository>();
            builder.Services.AddScoped<IScheduleDayRepository, ScheduleDayRepository>();
            builder.Services.AddScoped<IScheduleEventRepository, ScheduleEventRepository>();
            builder.Services.AddScoped<IFriendRepository, FriendRepository>();
            builder.Services.AddScoped<IActiviteCodeRepository, ActiviteCodeRepository>();
            builder.Services.AddScoped<INotificationRepository, NotificationRepository>();


            builder.Services.AddScoped<IChatService, ChatService>();
            builder.Services.AddScoped<ICheckListService, CheckListService>();
            builder.Services.AddScoped<ICheckListFieldService, CheckListFieldService>();
            builder.Services.AddScoped<ICultureService, CultureService>();
            builder.Services.AddScoped<ICultureAssistanceService, CultureAssistanceService>();
            builder.Services.AddScoped<IParticipantTourService, ParticipantTourService>();
            builder.Services.AddScoped<IQuestionnaireVoteService, QuestionnaireVoteService>();
            builder.Services.AddScoped<IQuestionnaireAnswerService, QuestionnaireAnswerService>();
            builder.Services.AddScoped<IQuestionnaireService, QuestionnaireService>();
            builder.Services.AddScoped<IRouteService, RouteService>();
            builder.Services.AddScoped<IStopoverService, StopoverService>();
            builder.Services.AddScoped<ITourService, TourService>();
            builder.Services.AddScoped<IBillService, BillService>();
            builder.Services.AddScoped<IScheduleService, ScheduleService>();
            builder.Services.AddScoped<IActiviteCodeService, ActiviteCodeService>();
            builder.Services.AddScoped<IFriendService, FriendService>();
            builder.Services.AddScoped<INotificationService, NotificationService>();
            builder.Services.AddScoped<IUserService, UserService>();

            builder.Services.AddScoped<IPasswordHasher<User>, PasswordHasher<User>>();
            
            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.MapHub<ChatHub>("/chat");
            app.MapHub<CheckListHub>("/checklist");
            app.MapHub<NotificationHub>("/notification");

            app.UseMiddleware<ExceptionHandlerMiddleware>();
            app.UseMiddleware<AuthenticationMiddleware>();

            app.UseAuthorization();
            app.MapControllers();

            app.Run();
        }
    }
}