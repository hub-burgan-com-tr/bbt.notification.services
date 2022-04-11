using Microsoft.EntityFrameworkCore;


public class DatabaseContext : DbContext
{
    public DbSet<Source> Sources { get; set; }
    public DbSet<Consumer> Consumers { get; set; }
    public DbSet<SourceService> SourceServices { get; set; }
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
            
            
        options.UseSqlServer(configuration.GetConnectionString("DefaultConnection"));
        options.EnableSensitiveDataLogging();
    }
    

    string? GetEnviroment()
    {
        return Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");
    }

    protected override void OnModelCreating(ModelBuilder builder)
    {

        builder.Entity<Consumer>().OwnsOne(e => e.Phone);

        /* TODO: SQL Edge not supporting memory optimized tables
        builder.Entity<Consumer>(c =>
        {
            c.OwnsOne(e => e.Phone).IsMemoryOptimized();
            c.IsMemoryOptimized();
        });
        */

        builder.Entity<Source>()
            .HasOne(s => s.Parent)
            .WithMany(s=> s.Children)
            .HasForeignKey(s=> s.ParentId);



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
              Title_TR = "Gelen EFT",
              Title_EN = "Incoming EFT",
              DisplayType = SourceDisplayType.DisplayAndSetSwitchParametersChannelsInfo,
              Topic = "http://localhost:8082/topics/cdc_eft/incoming_eft",
              KafkaUrl = "test",
              ApiKey = "a1b2c33d4e5f6g7h8i9jakblc",
              Secret = "11561681-8ba5-4b46-bed0-905ae1769bc6",
              PushServiceReference = "notify_push_incoming_eft",
              SmsServiceReference = "notify_sms_incoming_eft",
              EmailServiceReference = "notify_email_incoming_eft"
          });

        builder.Entity<Source>().HasData(
               new Source
               {
                   Id = 101,
                   Title_TR = "Gelen FAST",
                   Title_EN = "Incoming FAST",
                   ParentId = 1,
                   DisplayType = SourceDisplayType.DisplayAndSetSwitchParameters,
                   Topic = "http://localhost:8082/topics/cdc_eft/incoming_fast",
                   KafkaUrl = "test",
                   ApiKey = "a1b2c33d4e5f6g7h8i9jakblc",
                   Secret = "11561681-8ba5-4b46-bed0-905ae1769bc6",
                   PushServiceReference = "notify_push_incoming_fast",
                   SmsServiceReference = "notify_sms_incoming_fast",
                   EmailServiceReference = "notify_email_incoming_fast"
               },
               new Source
               {
                   Id = 10101,
                   Title_TR = "Ulasmayan FAST",
                   Title_EN = "Not Delivered FAST Messages",
                   ParentId = 101,
                   DisplayType = SourceDisplayType.Display,
                   Topic = "http://localhost:8082/topics/cdc_eft/incoming_fast_not_delivered",
                   KafkaUrl = "test",
                   ApiKey = "a1b2c33d4e5f6g7h8i9jakblc",
                   Secret = "11561681-8ba5-4b46-bed0-905ae1769bc6",
                   PushServiceReference = "notify_push_incoming_fast",
                   SmsServiceReference = "notify_sms_incoming_fast",
                   EmailServiceReference = "notify_email_incoming_fast"
               },
               new Source
               {
                   Id = 102,
                   Title_TR = "Gelen QR",
                   Title_EN = "Incoming QR",
                   ParentId = 1,
                   DisplayType = SourceDisplayType.DisplayAndSetSwitchParameters,
                   Topic = "http://localhost:8082/topics/cdc_eft/incoming_qr",
                   KafkaUrl = "test",
                   ApiKey = "a1b2c33d4e5f6g7h8i9jakblc",
                   Secret = "11561681-8ba5-4b46-bed0-905ae1769bc6",
                   PushServiceReference = "notify_push_incoming_qr",
                   SmsServiceReference = "notify_sms_incoming_qr",
                   EmailServiceReference = "notify_email_incoming_qr"
               }
             );

        builder.Entity<Consumer>(c =>
        {
            c.HasData(
            new Consumer
            {
                Id = new Guid("1e15d57c-26e3-4e78-94f9-8649b3302555"),
                Client = (long)123456,
                User = (long)123456,
                SourceId =1,
                Filter = "Message.data.amount >= 500 && Message.data.iban ==\"TR1234567\"",
                IsPushEnabled = false,
                IsSmsEnabled = true,
                IsEmailEnabled = false
            });
            c.OwnsOne(e => e.Phone).HasData(new { ConsumerId = new Guid("1e15d57c-26e3-4e78-94f9-8649b3302555"), CountryCode = 90, Prefix = 530, Number = 3855206 });

        });


        builder.Entity<SourceParameter>().HasData(
            new SourceParameter
            {

                SourceId = 101,
                JsonPath = "Message.data.amount",
                Type = SourceParameterType.GreaterThan,
                Title_TR = "Tutar",
                Title_EN = "Amount",
            },
            new SourceParameter
            {

                SourceId = 101,
                JsonPath = "Message.data.amount",
                Type = SourceParameterType.LessThan,
                Title_TR = "Tutar",
                Title_EN = "Amount",
            },
             new SourceParameter
             {

                 SourceId = 1,
                 JsonPath = "Message.data.amount",
                 Type = SourceParameterType.GreaterThan,
                 Title_TR = "Tutar",
                 Title_EN = "Amount",
             },
             new SourceParameter
             {

                 SourceId = 102,
                 JsonPath = "Message.data.amount",
                 Type = SourceParameterType.GreaterThan,
                 Title_TR = "Tutar",
                 Title_EN = "Amount",
             }

         );

        builder.Entity<Consumer>(c =>
        {
            c.HasData(
            new
            {
                Id = new Guid("2e15d57c-26e3-4e78-94f9-8649b3302555"),
                Client = (long)123456,
                User = (long)123456,
                SourceId = 102,
                IsPushEnabled = false,
                IsSmsEnabled = true,
                IsEmailEnabled = false
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
                User = (long)123456,
                SourceId = 1,
                Filter = "Message.data.amount >= 500000",
                IsPushEnabled = false,
                IsSmsEnabled = true,
                IsEmailEnabled = false
            });
            c.OwnsOne(e => e.Phone).HasData(new { ConsumerId = new Guid("3e15d57c-26e3-4e78-94f9-8649b3302555"), CountryCode = 90, Prefix = 530, Number = 3855206 });
        });
        builder.Entity<SourceService>().HasData(
          new SourceService
          {
              Id = 1,
              SourceId = 1,
              ServiceUrl = "localhost:/getcustomerId",
              
          },
          
          new SourceService
          {
              Id = 2,
              SourceId = 1,
              ServiceUrl = "localhost:/get2customerId",
              
          },
          new SourceService
          {
              Id = 3,
              SourceId = 101,
              ServiceUrl = "localhost:/get3customerId",
              
          });


    }
}