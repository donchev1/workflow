﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Organiser.Data.Context;

namespace Organiser.Data.Migrations
{
    [DbContext(typeof(AppDbContext))]
    partial class AppDbContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "2.2.1-servicing-10028")
                .HasAnnotation("Relational:MaxIdentifierLength", 128)
                .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

            modelBuilder.Entity("Organiser.Data.Models.Department", b =>
                {
                    b.Property<int>("DepartmentId")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<string>("Name");

                    b.Property<int>("UserId");

                    b.HasKey("DepartmentId");

                    b.ToTable("Departments");
                });

            modelBuilder.Entity("Organiser.Data.Models.DepartmentState", b =>
                {
                    b.Property<int>("DepartmentStateId")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<int>("EntitiesInProgress");

                    b.Property<int>("EntitiesPassed");

                    b.Property<int>("EntitiesRFC");

                    b.Property<DateTime>("Finish");

                    b.Property<int>("LocationPosition");

                    b.Property<string>("Name");

                    b.Property<int>("OrderId");

                    b.Property<DateTime>("Start");

                    b.Property<string>("Status");

                    b.Property<int>("TotalEntityCount");

                    b.HasKey("DepartmentStateId");

                    b.HasIndex("OrderId");

                    b.ToTable("DepartmentStates");
                });

            modelBuilder.Entity("Organiser.Data.Models.Log", b =>
                {
                    b.Property<int>("LogId")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<string>("ActionRecord");

                    b.Property<DateTime>("CreatedAt");

                    b.Property<string>("OrderNumber");

                    b.Property<int?>("UserId");

                    b.Property<string>("UserName");

                    b.HasKey("LogId");

                    b.HasIndex("UserId");

                    b.ToTable("Logs");
                });

            modelBuilder.Entity("Organiser.Data.Models.NewMessagesMonitor", b =>
                {
                    b.Property<int>("NewMessagesMonitorId")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<bool>("Covertirung");

                    b.Property<bool>("Fahrer");

                    b.Property<bool>("Falcen");

                    b.Property<bool>("Folirung");

                    b.Property<bool>("Handarbeit");

                    b.Property<bool>("Inkchet");

                    b.Property<bool>("Lager");

                    b.HasKey("NewMessagesMonitorId");

                    b.ToTable("NewMessagesMonitor");
                });

            modelBuilder.Entity("Organiser.Data.Models.Note", b =>
                {
                    b.Property<int>("NoteId")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<string>("Author");

                    b.Property<string>("Content");

                    b.Property<DateTime>("CreatedAt");

                    b.Property<int>("Location");

                    b.HasKey("NoteId");

                    b.ToTable("Notes");
                });

            modelBuilder.Entity("Organiser.Data.Models.Order", b =>
                {
                    b.Property<int>("OrderId")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<DateTime>("CreatedAt");

                    b.Property<string>("Customer");

                    b.Property<DateTime>("DeadLineDate");

                    b.Property<int>("EntitiesCompleted");

                    b.Property<int>("EntitiesInProgress");

                    b.Property<int>("EntitiesNotProcessed");

                    b.Property<int>("EntityCount");

                    b.Property<string>("EntityType");

                    b.Property<DateTime>("FinshedAt");

                    b.Property<string>("OrderNumber");

                    b.Property<DateTime>("StartedAt");

                    b.Property<string>("Status");

                    b.HasKey("OrderId");

                    b.ToTable("Orders");
                });

            modelBuilder.Entity("Organiser.Data.Models.User", b =>
                {
                    b.Property<int>("UserId")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<string>("ConfirmPassword");

                    b.Property<bool>("IsAdmin");

                    b.Property<string>("Password");

                    b.Property<string>("UserName");

                    b.HasKey("UserId");

                    b.ToTable("Users");
                });

            modelBuilder.Entity("Organiser.Data.Models.UserRole", b =>
                {
                    b.Property<int>("UserRoleId")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<int>("Role");

                    b.Property<int>("UserId");

                    b.HasKey("UserRoleId");

                    b.HasIndex("UserId");

                    b.ToTable("UserRoles");
                });

            modelBuilder.Entity("Organiser.Data.Models.DepartmentState", b =>
                {
                    b.HasOne("Organiser.Data.Models.Order", "Order")
                        .WithMany("DepartmentStates")
                        .HasForeignKey("OrderId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("Organiser.Data.Models.Log", b =>
                {
                    b.HasOne("Organiser.Data.Models.User")
                        .WithMany("Logs")
                        .HasForeignKey("UserId");
                });

            modelBuilder.Entity("Organiser.Data.Models.UserRole", b =>
                {
                    b.HasOne("Organiser.Data.Models.User", "User")
                        .WithMany("UserRoles")
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade);
                });
#pragma warning restore 612, 618
        }
    }
}
