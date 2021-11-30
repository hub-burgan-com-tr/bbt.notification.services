﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

#nullable disable

namespace Notification.Profile.Migrations
{
    [DbContext(typeof(DatabaseContext))]
    [Migration("20211130065903_reset2")]
    partial class reset2
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "6.0.0")
                .HasAnnotation("Relational:MaxIdentifierLength", 128);

            SqlServerModelBuilderExtensions.UseIdentityColumns(modelBuilder, 1L, 1);

            modelBuilder.Entity("Consumer", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<long>("$id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("bigint");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<long>("$id"), 1L, 1);

                    b.Property<long>("Client")
                        .HasColumnType("bigint");

                    b.Property<string>("DeviceKey")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Email")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Filter")
                        .HasColumnType("nvarchar(max)");

                    b.Property<bool>("IsEmailEnabled")
                        .HasColumnType("bit");

                    b.Property<bool>("IsPushEnabled")
                        .HasColumnType("bit");

                    b.Property<bool>("IsSmsEnabled")
                        .HasColumnType("bit");

                    b.Property<string>("SourceId")
                        .HasColumnType("nvarchar(450)");

                    b.Property<long>("User")
                        .HasColumnType("bigint");

                    b.HasKey("Id");

                    SqlServerKeyBuilderExtensions.IsClustered(b.HasKey("Id"), false);

                    b.HasIndex("$id")
                        .IsUnique();

                    SqlServerIndexBuilderExtensions.IsClustered(b.HasIndex("$id"));

                    b.HasIndex("SourceId");

                    b.ToTable("Consumers");

                    b.HasData(
                        new
                        {
                            Id = new Guid("1e15d57c-26e3-4e78-94f9-8649b3302555"),
                            Client = 123456L,
                            Filter = "data.amount >= 500",
                            IsEmailEnabled = false,
                            IsPushEnabled = false,
                            IsSmsEnabled = true,
                            SourceId = "[SAMPLE]Incoming-EFT",
                            User = 123456L
                        },
                        new
                        {
                            Id = new Guid("2e15d57c-26e3-4e78-94f9-8649b3302555"),
                            Client = 123456L,
                            IsEmailEnabled = false,
                            IsPushEnabled = false,
                            IsSmsEnabled = true,
                            SourceId = "[SAMPLE]Incoming-EFT",
                            User = 123456L
                        },
                        new
                        {
                            Id = new Guid("3e15d57c-26e3-4e78-94f9-8649b3302555"),
                            Client = 0L,
                            Filter = "data.amount >= 500000",
                            IsEmailEnabled = false,
                            IsPushEnabled = false,
                            IsSmsEnabled = true,
                            SourceId = "[SAMPLE]Incoming-EFT",
                            User = 123456L
                        });
                });

            modelBuilder.Entity("ConsumerVariant", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<Guid>("ConsumerId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<string>("Key")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Value")
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.HasIndex("ConsumerId");

                    b.ToTable("ConsumerVariants");

                    b.HasData(
                        new
                        {
                            Id = new Guid("444fcb0a-d7a7-4891-9a95-826cbef5d792"),
                            ConsumerId = new Guid("2e15d57c-26e3-4e78-94f9-8649b3302555"),
                            Key = "IBAN",
                            Value = "TR58552069008"
                        });
                });

            modelBuilder.Entity("Source", b =>
                {
                    b.Property<string>("Id")
                        .HasColumnType("nvarchar(450)");

                    b.Property<string>("ApiKey")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("EmailServiceReference")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("PushServiceReference")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Secret")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("SmsServiceReference")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Title")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Topic")
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.ToTable("Sources");

                    b.HasData(
                        new
                        {
                            Id = "[SAMPLE]Incoming-EFT",
                            ApiKey = "a1b2c33d4e5f6g7h8i9jakblc",
                            EmailServiceReference = "notify_email_incoming_eft",
                            PushServiceReference = "notify_push_incoming_eft",
                            Secret = "11561681-8ba5-4b46-bed0-905ae1769bc6",
                            SmsServiceReference = "notify_sms_incoming_eft",
                            Title = "Gelen EFT",
                            Topic = "http://localhost:8082/topics/cdc_eft/incoming_eft"
                        },
                        new
                        {
                            Id = "[SAMPLE]Incoming-FAST",
                            ApiKey = "a1b2c33d4e5f6g7h8i9jakblc",
                            EmailServiceReference = "notify_email_incoming_fast",
                            PushServiceReference = "notify_push_incoming_fast",
                            Secret = "11561681-8ba5-4b46-bed0-905ae1769bc6",
                            SmsServiceReference = "notify_sms_incoming_fast",
                            Title = "Gelen Fast",
                            Topic = "http://localhost:8082/topics/cdc_eft/incoming_fast"
                        },
                        new
                        {
                            Id = "[SAMPLE]Incoming-QR",
                            ApiKey = "a1b2c33d4e5f6g7h8i9jakblc",
                            EmailServiceReference = "notify_email_incoming_qr",
                            PushServiceReference = "notify_push_incoming_qr",
                            Secret = "11561681-8ba5-4b46-bed0-905ae1769bc6",
                            SmsServiceReference = "notify_sms_incoming_qr",
                            Title = "Gelen EFT",
                            Topic = "http://localhost:8082/topics/cdc_eft/incoming_qr"
                        });
                });

            modelBuilder.Entity("Consumer", b =>
                {
                    b.HasOne("Source", "Source")
                        .WithMany()
                        .HasForeignKey("SourceId");

                    b.OwnsOne("Phone", "Phone", b1 =>
                        {
                            b1.Property<Guid>("ConsumerId")
                                .HasColumnType("uniqueidentifier");

                            b1.Property<int>("CountryCode")
                                .HasColumnType("int");

                            b1.Property<int>("Number")
                                .HasColumnType("int");

                            b1.Property<int>("Prefix")
                                .HasColumnType("int");

                            b1.HasKey("ConsumerId");

                            b1.ToTable("Consumers");

                            b1.WithOwner()
                                .HasForeignKey("ConsumerId");

                            b1.HasData(
                                new
                                {
                                    ConsumerId = new Guid("1e15d57c-26e3-4e78-94f9-8649b3302555"),
                                    CountryCode = 90,
                                    Number = 3855206,
                                    Prefix = 530
                                },
                                new
                                {
                                    ConsumerId = new Guid("2e15d57c-26e3-4e78-94f9-8649b3302555"),
                                    CountryCode = 90,
                                    Number = 3855206,
                                    Prefix = 530
                                },
                                new
                                {
                                    ConsumerId = new Guid("3e15d57c-26e3-4e78-94f9-8649b3302555"),
                                    CountryCode = 90,
                                    Number = 3855206,
                                    Prefix = 530
                                });
                        });

                    b.Navigation("Phone");

                    b.Navigation("Source");
                });

            modelBuilder.Entity("ConsumerVariant", b =>
                {
                    b.HasOne("Consumer", "Consumer")
                        .WithMany("Variants")
                        .HasForeignKey("ConsumerId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Consumer");
                });

            modelBuilder.Entity("Consumer", b =>
                {
                    b.Navigation("Variants");
                });
#pragma warning restore 612, 618
        }
    }
}