using Microsoft.EntityFrameworkCore;
using TripPlanner.Models.Models;
using TripPlanner.Models.Models.BillModels;
using TripPlanner.Models.Models.CheckListModels;
using TripPlanner.Models.Models.CultureModels;
using TripPlanner.Models.Models.MessageModels;
using TripPlanner.Models.Models.MessageModels.QuestionnaireModels;
using TripPlanner.Models.Models.RouteModels;
using TripPlanner.Models.Models.ScheduleModels;
using TripPlanner.Models.Models.TourModels;
using TripPlanner.Models.Models.UserModels;

namespace TripPlanner.DataAccess
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {

        }

        public DbSet<ScheduleDay> ScheduleDays { get; set; }
        public DbSet<ScheduleEvent> ScheduleEvents { get; set; }
        public DbSet<BillContributor> BillContributors { get; set; }
        public DbSet<Friend> Friends { get; set; }
        public DbSet<Notification> Notifications { get; set; }
        public DbSet<Bill> Bills { get; set; }
        public DbSet<Transfer> Transfers { get; set; }
        public DbSet<CheckList> CheckLists { get; set; }
        public DbSet<CheckListField> CheckListFields { get; set; }
        public DbSet<Culture> Cultures { get; set; }
        public DbSet<CultureAssistance> CultureAssistances { get; set; }
        public DbSet<TextMessage> TextMessages { get; set; }
        public DbSet<NoticeMessage> NoticeMessages { get; set; }
        public DbSet<ParticipantTour> ParticipantTours { get; set; }
        public DbSet<Questionnaire> Questionnaires { get; set; }
        public DbSet<QuestionnaireAnswer> QuestionnaireAnswers { get; set; }
        public DbSet<QuestionnaireVote> QuestionnaireVotes { get; set; }
        public DbSet<Route> Routes { get; set; }
        public DbSet<Stopover> Stopovers { get; set; }
        public DbSet<Tour> Tours { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<ActiviteCode> ActiviteCodes { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            #region User
            modelBuilder.Entity<User>()
                .HasKey(e => e.Id);

            //relacje
            modelBuilder.Entity<User>()
                .HasMany(sc => sc.ParticipantTours)
                .WithOne(s => s.User)
                .OnDelete(DeleteBehavior.Cascade)
                .HasForeignKey(sc => sc.UserId)
                .IsRequired();

            modelBuilder.Entity<User>()
                .HasMany(sc => sc.Notifications)
                .WithOne(s => s.User)
                .OnDelete(DeleteBehavior.Cascade)
                .HasForeignKey(sc => sc.UserId)
                .IsRequired();

            modelBuilder.Entity<User>()
                .HasMany(sc => sc.Shares)
                .WithOne(s => s.Creator)
                .OnDelete(DeleteBehavior.Cascade)
                .HasForeignKey(sc => sc.CreatorId)
                .IsRequired();

            modelBuilder.Entity<User>()
                .HasMany(sc => sc.BillsPayed)
                .WithOne(s => s.Payer)
                .OnDelete(DeleteBehavior.NoAction)
                .HasForeignKey(sc => sc.PayerId)
                .IsRequired();

            modelBuilder.Entity<User>()
                .HasMany(sc => sc.BillContributors)
                .WithOne(s => s.User)
                .OnDelete(DeleteBehavior.NoAction)
                .HasForeignKey(sc => sc.UserId)
                .IsRequired();

            modelBuilder.Entity<User>()
                .HasMany(sc => sc.TransfersSender)
                .WithOne(s => s.Sender)
                .OnDelete(DeleteBehavior.NoAction)
                .HasForeignKey(sc => sc.SenderId)
                .IsRequired();

            modelBuilder.Entity<User>()
                .HasMany(sc => sc.TransfersRecipient)
                .WithOne(s => s.Recipient)
                .OnDelete(DeleteBehavior.NoAction)
                .HasForeignKey(sc => sc.RecipientId)
                .IsRequired();

            modelBuilder.Entity<User>()
                .HasMany(u => u.QuestionnaireVotes)
                .WithOne(s => s.User)
                .HasForeignKey(u => u.UserId)
                .OnDelete(DeleteBehavior.NoAction)
                .IsRequired();

            modelBuilder.Entity<User>()
                .HasMany(u => u.Messages)
                .WithOne( u => u.User)
                .HasForeignKey(u => u.UserId)
                .OnDelete(DeleteBehavior.Cascade)
                .IsRequired();

            modelBuilder.Entity<User>()
                .HasMany(u => u.Routes)
                .WithOne(u => u.User)
                .HasForeignKey(u => u.UserId)
                .OnDelete(DeleteBehavior.NoAction)
                .IsRequired();

            modelBuilder.Entity<User>()
                .HasMany(u => u.CheckLists)
                .WithOne(u => u.User)
                .HasForeignKey(u => u.UserId)
                .OnDelete(DeleteBehavior.NoAction)
                .IsRequired();
            // koniec realcji

            modelBuilder.Entity<User>()
                .Property(s => s.FullName)
                .IsRequired();

            modelBuilder.Entity<User>()
                .Property(s => s.City)
                .IsRequired();

            modelBuilder.Entity<User>()
                .Property(s => s.PasswordHash)
                .IsRequired();

            modelBuilder.Entity<User>()
                .Property(s => s.Email)
                .IsRequired();
            
            modelBuilder.Entity<User>()
                .Property(s => s.FullAddress)
                .IsRequired();

            modelBuilder.Entity<User>()
                .Property(s => s.DateOfBirth)
                .IsRequired();

            modelBuilder.Entity<User>()
                .Property(s => s.IsActivated)
                .IsRequired();
            #endregion

            #region Friend
            modelBuilder.Entity<Friend>().HasKey(sc => new { sc.Friend2Id, sc.Friend1Id});

            //relacje
            modelBuilder.Entity<Friend>()
                .HasOne(sc => sc.Friend1)
                .WithMany()
                .OnDelete(DeleteBehavior.NoAction)
                .HasForeignKey(sc => sc.Friend1Id)
                .IsRequired();

            modelBuilder.Entity<Friend>()
                .HasOne(sc => sc.Friend2)
                .WithMany()
                .OnDelete(DeleteBehavior.NoAction)
                .HasForeignKey(sc => sc.Friend2Id)
                .IsRequired();
            //koniec relacji
            #endregion

            #region ActiviteCode
            modelBuilder.Entity<ActiviteCode>()
                .HasKey(e => e.Id);

            modelBuilder.Entity<ActiviteCode>()
                .Property(s => s.UserId)
                .IsRequired();

            modelBuilder.Entity<ActiviteCode>()
                .Property(s => s.Code)
                .IsRequired();

            modelBuilder.Entity<ActiviteCode>()
                .Property(s => s.ExpireDate)
                .IsRequired();
            #endregion

            #region Tour
            modelBuilder.Entity<Tour>()
                .HasKey(e => e.Id);

            //relacje
            modelBuilder.Entity<Tour>()
                .HasMany(sc => sc.Messages)
                .WithOne(s => s.Tour)
                .OnDelete(DeleteBehavior.Cascade)
                .HasForeignKey(sc => sc.TourId)
                .IsRequired();

            modelBuilder.Entity<Tour>()
                .HasMany(sc => sc.Schedule)
                .WithOne(s => s.Tour)
                .OnDelete(DeleteBehavior.Cascade)
                .HasForeignKey(sc => sc.TourId)
                .IsRequired();

            modelBuilder.Entity<Tour>()
                .HasMany(sc => sc.Cultures)
                .WithOne(s => s.Tour)
                .OnDelete(DeleteBehavior.Cascade)
                .HasForeignKey(sc => sc.TourId)
                .IsRequired();

            modelBuilder.Entity<Tour>()
                .HasMany(sc => sc.Shares)
                .WithOne(s => s.Tour)
                .OnDelete(DeleteBehavior.Cascade)
                .HasForeignKey(sc => sc.TourId)
                .IsRequired();

            modelBuilder.Entity<Tour>()
                .HasMany(sc => sc.Participants)
                .WithOne(s => s.Tour)
                .HasForeignKey(sc => sc.TourId)
                .OnDelete(DeleteBehavior.Cascade)
                .IsRequired();

            modelBuilder.Entity<Tour>()
                .HasMany(sc => sc.CheckLists)
                .WithOne(s => s.Tour)
                .HasForeignKey(sc => sc.TourId)
                .OnDelete(DeleteBehavior.SetNull)
                .IsRequired(false);

            modelBuilder.Entity<Tour>()
                .HasMany(sc => sc.Routes)
                .WithOne(s => s.Tour)
                .HasForeignKey(sc => sc.TourId)
                .OnDelete(DeleteBehavior.SetNull)
                .IsRequired(false);
            // koniec realcji

            modelBuilder.Entity<Tour>()
                .Property(s => s.StartDate)
                .IsRequired();

            modelBuilder.Entity<Tour>()
                .Property(s => s.MaxParticipant)
                .IsRequired();

            modelBuilder.Entity<Tour>()
                .Property(s => s.EndDate)
                .IsRequired();

            modelBuilder.Entity<Tour>()
                .Property(s => s.TargetCountry)
                .IsRequired();

            modelBuilder.Entity<Tour>()
                .Property(s => s.Title)
                .IsRequired();
            #endregion

            #region Share
            modelBuilder.Entity<Share>().UseTpcMappingStrategy();

            modelBuilder.Entity<Share>()
                .HasKey(e => e.Id);

            modelBuilder.Entity<Share>()
                .Property(e => e.CreatedDate)
                .IsRequired();

            modelBuilder.Entity<Share>()
                .Property(e => e.Name)
                .IsRequired();

            modelBuilder.Entity<Share>()
                .Property(e => e.Description)
                .IsRequired();

            modelBuilder.Entity<Share>()
                .Property(e => e.ImageFilePath)
                .IsRequired();

            modelBuilder.Entity<Share>()
                .Property(e => e.Value)
                .HasPrecision(12,2)
                .IsRequired();
            #endregion

            #region Bill
            modelBuilder.Entity<Bill>().ToTable("Bills");

            //relacje
            modelBuilder.Entity<Bill>()
                .HasMany(sc => sc.Contributors)
                .WithOne(s => s.Bill)
                .OnDelete(DeleteBehavior.Cascade)
                .HasForeignKey(sc => sc.BillId)
                .IsRequired();
            // koniec realcji

            modelBuilder.Entity<Bill>()
                .Property(s => s.BillType)
                .IsRequired();
            #endregion

            #region Transfer
            modelBuilder.Entity<Transfer>().ToTable("Transfers");
            
            //relacje
            // koniec realcji
            #endregion

            #region BillContributor
            modelBuilder.Entity<BillContributor>().HasKey(sc => new { sc.UserId, sc.BillId });

            modelBuilder.Entity<BillContributor>()
                .Property(s => s.Due)
                .HasPrecision(12, 2)
                .IsRequired();
            #endregion

            #region Notification
            modelBuilder.Entity<Notification>()
                .HasKey(e => e.Id);

            //relacje
            // koniec realcji

            modelBuilder.Entity<Notification>()
                .Property(s => s.CreatedDate)
                .IsRequired();

            modelBuilder.Entity<Notification>()
                .Property(s => s.Name)
                .IsRequired();

            modelBuilder.Entity<Notification>()
                .Property(s => s.IsVisited)
                .IsRequired();

            modelBuilder.Entity<Notification>()
                .Property(s => s.Message)
                .IsRequired();

            modelBuilder.Entity<Notification>()
                .Property(s => s.IconPath)
                .IsRequired();

            modelBuilder.Entity<Notification>()
                .Property(s => s.Type)
                .IsRequired();

            modelBuilder.Entity<Notification>()
                .Property(s => s.TourId)
                .IsRequired();
            #endregion

            #region ScheduleDay
            modelBuilder.Entity<ScheduleDay>()
                .HasKey(e => e.Id);

            //relacje
            modelBuilder.Entity<ScheduleDay>()
                .HasMany(sc => sc.Events)
                .WithOne(s => s.ScheduleDay)
                .HasForeignKey(sc => sc.ScheduleDayId)
                .OnDelete(DeleteBehavior.Cascade)
                .IsRequired();
            // koniec realcji

            modelBuilder.Entity<ScheduleDay>()
                .Property(s => s.Date)
                .IsRequired();

            modelBuilder.Entity<ScheduleDay>()
                .Property(s => s.Description)
                .IsRequired();
            #endregion

            #region ScheduleEvent
            modelBuilder.Entity<ScheduleEvent>()
                .HasKey(e => e.Id);

            //relacje
            // koniec realcji

            modelBuilder.Entity<ScheduleEvent>()
                .Property(s => s.StartTime)
                .IsRequired();

            modelBuilder.Entity<ScheduleEvent>()
                .Property(s => s.StopTime)
                .IsRequired();

            modelBuilder.Entity<ScheduleEvent>()
              .Property(s => s.Name)
              .IsRequired();
            #endregion

            #region CheckList
            modelBuilder.Entity<CheckList>()
                .HasKey(e => e.Id);

            //relacje
            modelBuilder.Entity<CheckList>()
                .HasMany(sc => sc.Fields)
                .WithOne(s => s.CheckList)
                .OnDelete(DeleteBehavior.Cascade)
                .HasForeignKey(sc => sc.CheckListId)
                .IsRequired();

            //modelBuilder.Entity<CheckList>()
            //    .HasOne(u => u.User)
            //    .WithMany(u => u.CheckLists)
            //    .HasForeignKey(u => u.UserId)
            //    .OnDelete(DeleteBehavior.NoAction)
            //    .IsRequired();
            // koniec realcji

            modelBuilder.Entity<CheckList>()
                .Property(s => s.IsPublic)
                .IsRequired();

            modelBuilder.Entity<CheckList>()
                .Property(s => s.Name)
                .IsRequired();
            #endregion

            #region CheckListField
            modelBuilder.Entity<CheckListField>()
                .HasKey(e => e.Id);

            modelBuilder.Entity<CheckListField>()
                .Property(s => s.IsChecked)
                .IsRequired();

            modelBuilder.Entity<CheckListField>()
                .Property(s => s.Multiplicity)
                .IsRequired();

            modelBuilder.Entity<CheckListField>()
                .Property(s => s.Name)
                .IsRequired();
            #endregion

            #region Culture
            modelBuilder.Entity<Culture>()
                .HasKey(e => e.Id);

            //relacje
            modelBuilder.Entity<Culture>()
                .HasMany(sc => sc.CultureAssistances)
                .WithOne(s => s.Culture)
                .OnDelete(DeleteBehavior.Cascade)
                .HasForeignKey(sc => sc.CultureId)
                .IsRequired();
            // koniec realcji

            modelBuilder.Entity<Culture>()
                .Property(s => s.Name)
                .IsRequired();

            modelBuilder.Entity<Culture>()
                .Property(s => s.Description)
                .IsRequired();

            modelBuilder.Entity<Culture>()
                .Property(s => s.Country)
                .IsRequired();

            modelBuilder.Entity<Culture>()
                .Property(s => s.Religion)
                .IsRequired();

            modelBuilder.Entity<Culture>()
                .Property(s => s.Goverment)
                .IsRequired();

            modelBuilder.Entity<Culture>()
                .Property(s => s.GeograpInformation)
                .IsRequired();

            modelBuilder.Entity<Culture>()
                .Property(s => s.Manners)
                .IsRequired();

            modelBuilder.Entity<Culture>()
                .Property(s => s.LanguageAssistance)
                .IsRequired();

            modelBuilder.Entity<Culture>()
                .Property(s => s.ProperBehaviour)
                .IsRequired();
            #endregion

            #region CultureAssistance
            modelBuilder.Entity<CultureAssistance>().HasKey(sc => new { sc.CultureId, sc.TourId });

            modelBuilder.Entity<CultureAssistance>()
                .Property(s => s.IsPrincipal)
                .IsRequired();
            #endregion

            #region Message
            modelBuilder.Entity<Message>().UseTpcMappingStrategy();

            modelBuilder.Entity<Message>()
                .HasKey(e => e.Id);

            modelBuilder.Entity<Message>()
                .Property(e => e.Content)
                .IsRequired();

            modelBuilder.Entity<Message>()
                .Property(e => e.Date)
                .IsRequired();
            #endregion

            #region TextMessage
            modelBuilder.Entity<TextMessage>().ToTable("TextMessages");

            //relacje
            //koniec relacji
            #endregion

            #region NoticeMessage
            modelBuilder.Entity<NoticeMessage>().ToTable("NoticeMessages");

            //relacje
            //koniec relacji
            #endregion

            #region Questionnaire
            modelBuilder.Entity<Questionnaire>().ToTable("Questionnaires");

            //relacje
            modelBuilder.Entity<Questionnaire>()
                .HasMany(sc => sc.Answers)
                .WithOne(s => s.Questionnaire)
                .OnDelete(DeleteBehavior.Cascade)
                .HasForeignKey(sc => sc.QuestionnaireId)
                .IsRequired();
            // koniec realcji

            modelBuilder.Entity<Questionnaire>()
                .Property(s => s.VoteForLabel)
                .IsRequired();
            #endregion

            #region ParticipantTour
            modelBuilder.Entity<ParticipantTour>().HasKey(sc => new { sc.UserId, sc.TourId });
            #endregion

            #region QuestionnaireAnswer
            modelBuilder.Entity<QuestionnaireAnswer>()
                .HasKey(e => e.Id);

            //relacje
            modelBuilder.Entity<QuestionnaireAnswer>()
                .HasMany(sc => sc.Votes)
                .WithOne(s => s.QuestionnaireAnswer)
                .OnDelete(DeleteBehavior.Cascade)
                .HasForeignKey(sc => sc.QuestionnaireAnswerId)
                .IsRequired();
            // koniec realcji

            modelBuilder.Entity<QuestionnaireAnswer>()
                .Property(s => s.Answer)
                .IsRequired();
            #endregion

            #region QuestionnaireVote
            modelBuilder.Entity<QuestionnaireVote>().HasKey(sc => new { sc.UserId, sc.QuestionnaireAnswerId });

            //relacje
            //koniec relacji
            #endregion

            #region Route
            modelBuilder.Entity<Route>()
                .HasKey(e => e.Id);

            //relacje
            modelBuilder.Entity<Route>()
                .HasMany(sc => sc.Stopovers)
                .WithOne(s => s.Route)
                .OnDelete(DeleteBehavior.Cascade)
                .HasForeignKey(sc => sc.RouteId)
                .IsRequired();

            //modelBuilder.Entity<Route>()
            //   .HasOne(u => u.User)
            //   .WithMany(u => u.Routes)
            //   .HasForeignKey(u => u.UserId)
            //   .OnDelete(DeleteBehavior.NoAction)
            //   .IsRequired(false);
            // koniec realcji

            modelBuilder.Entity<Route>()
                .Property(s => s.Name)
                .IsRequired();

            modelBuilder.Entity<Route>()
                .Property(s => s.StartLocation)
                .IsRequired();

            modelBuilder.Entity<Route>()
                .Property(s => s.StartDate)
                .IsRequired();

            modelBuilder.Entity<Route>()
                .Property(s => s.ArriveDate)
                .IsRequired();

            modelBuilder.Entity<Route>()
                .Property(s => s.ArriveLocation)
                .IsRequired();
            #endregion

            #region Stopover
            modelBuilder.Entity<Stopover>()
                .HasKey(e => e.Id);

            modelBuilder.Entity<Stopover>()
                .Property(s => s.BreakTime)
                .IsRequired();

            modelBuilder.Entity<Stopover>()
                .Property(s => s.Description)
                .IsRequired();

            modelBuilder.Entity<Stopover>()
                .Property(s => s.Location)
                .IsRequired();

            modelBuilder.Entity<Stopover>()
                .Property(s => s.Name)
                .IsRequired();
            #endregion
        }
    }
}
