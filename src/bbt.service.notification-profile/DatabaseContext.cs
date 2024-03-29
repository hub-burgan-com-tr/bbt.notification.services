using Microsoft.EntityFrameworkCore;
using Notification.Profile.Model.Database;

public class DatabaseContext : DbContext
{
    public DbSet<Source> Sources { get; set; }
    public DbSet<Consumer> Consumers { get; set; }
    public DbSet<SourceService> SourceServices { get; set; }
    public DbSet<Log> Logs { get; set; }
    public DbSet<MessageNotificationLog> MessageNotificationLogs { get; set; }
    public DbSet<ReminderDefinition> ReminderDefinitions { get; set; }
    public DbSet<NotificationLog> NotificationLogs { get; set; }
    public DbSet<ProductCode> ProductCodes { get; set; }
    public DbSet<UserRegistry> UserRegistry { get; set; }
    public DbSet<SourceLog> SourceLogs { get; set; }
    public string DbPath { get; private set; }
    public DatabaseContext()
    {
        DbPath = $"notification.db";
    }

    protected override void OnConfiguring(DbContextOptionsBuilder options)
    {
        //options.UseSqlite($"Data Source={DbPath}");
        IConfigurationRoot configuration = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
            .AddJsonFile($"appsettings.{GetEnviroment()}.json", false, true)
            .AddEnvironmentVariables()
            .Build();

        options.UseSqlServer(configuration.GetConnectionString("DefaultConnection"), b => b.MigrationsAssembly("bbt.service.notification-profile"));

        options.EnableSensitiveDataLogging();
    }

    string? GetEnviroment()
    {
        return Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");
    }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        builder.Entity<Consumer>().OwnsOne(e => e.Phone);

        builder.Entity<Source>()
            .HasOne(s => s.Parent)
            .WithMany(s => s.Children)
            .HasForeignKey(s => s.ParentId);

        builder.Entity<Consumer>()
           .Property<long>("$id")
           .ValueGeneratedOnAdd();

        builder.Entity<Consumer>()
            .HasKey(c => c.Id)
            .IsClustered(false);

        builder.Entity<Consumer>()
          .HasIndex("$id")
          .IsUnique()
          .IsClustered(true);

        builder.Entity<SourceParameter>().HasKey(pc => new { pc.SourceId, pc.JsonPath, pc.Type });

        builder.Entity<Source>().HasData(
          new Source
          {
              Id = 1,
              Title_TR = "CashBackTR",
              Title_EN = "CashBackEN",
              DisplayType = SourceDisplayType.NotDisplay,
              Topic = "CAMPAIGN_CASHBACK_ACCOUNTING_INFO",
              KafkaUrl = "x",
              ApiKey = "",
              Secret = "",
              PushServiceReference = "notify_push_incoming_eft",
              SmsServiceReference = "9cab7fdc-76a4-44be-b6fa-101f13729875",
              EmailServiceReference = "notify_email_incoming_eft",
              KafkaCertificate = "x"
          });

        builder.Entity<Consumer>(c =>
        {
            c.HasData(
            new
            {
                Id = new Guid("2e15d57c-26e3-4e78-94f9-8649b3302555"),
                Client = (long)0,
                User = (long)0,
                SourceId = 1,
                IsPushEnabled = false,
                IsSmsEnabled = true,
                IsEmailEnabled = false,
                IsStaff = false
            });
            c.OwnsOne(e => e.Phone).HasData(new { ConsumerId = new Guid("2e15d57c-26e3-4e78-94f9-8649b3302555"), CountryCode = 90, Prefix = 530, Number = 3855206 });
        });

        builder.Entity<Consumer>(c =>
        {
            c.HasData(
            new Consumer
            {
                Id = new Guid("3e15d57c-26e3-4e78-94f9-8649b3302555"),
                Client = (long)0,
                User = (long)0,
                SourceId = 1,
                Filter = "",
                IsPushEnabled = false,
                IsSmsEnabled = true,
                IsEmailEnabled = false,
                IsStaff = false
            });
            c.OwnsOne(e => e.Phone).HasData(new { ConsumerId = new Guid("3e15d57c-26e3-4e78-94f9-8649b3302555"), CountryCode = 90, Prefix = 530, Number = 3855206 });
        });
        builder.Entity<SourceService>().HasData(
          new SourceService
          {
              Id = 1,
              SourceId = 1,
              ServiceUrl = "X",

          });
        builder.Entity<Log>()
                    .Property(f => f.Id)
                    .ValueGeneratedOnAdd();

        builder.Entity<MessageNotificationLog>()
                  .Property(f => f.Id)
                  .ValueGeneratedOnAdd();

        builder.Entity<ReminderDefinition>()
                 .Property(f => f.Id)
                 .ValueGeneratedOnAdd();

        builder.Entity<NotificationLog>()
           .Property(f => f.Id)
           .ValueGeneratedOnAdd();

        builder.Entity<NotificationLog>()
                 .ToTable("NotificationLogs", b => b.IsTemporal());

        builder.Entity<ProductCode>()
                  .Property(f => f.Id)
                  .ValueGeneratedOnAdd();

        builder.Entity<UserRegistry>()
                .Property(f => f.Id)
                .ValueGeneratedOnAdd();

        builder.Entity<SourceLog>()
          .Property(f => f.Id)
          .ValueGeneratedOnAdd();

        builder.Entity<SourceLog>()
                 .ToTable("SourceLogs", b => b.IsTemporal());
    }
}