﻿// <auto-generated />
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.EntityFrameworkCore.Storage.Internal;
using Organiser.Models;
using System;

namespace Organiser.Migrations
{
    [DbContext(typeof(AppDbContext))]
    [Migration("20190206084537_sdd")]
    partial class sdd
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "2.0.1-rtm-125")
                .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

            modelBuilder.Entity("Organiser.Models.Department", b =>
                {
                    b.Property<int>("DepartmentId")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("Name");

                    b.Property<int>("UserId");

                    b.HasKey("DepartmentId");

                    b.ToTable("Departments");
                });

            modelBuilder.Entity("Organiser.Models.DepartmentState", b =>
                {
                    b.Property<int>("DepartmentStateId")
                        .ValueGeneratedOnAdd();

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

            modelBuilder.Entity("Organiser.Models.Log", b =>
                {
                    b.Property<int>("LogId")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("ActionRecord")
                        .IsRequired()
                        .HasColumnType("ntext");

                    b.Property<DateTime>("CreatedAt");

                    b.Property<string>("OrderNumber");

                    b.Property<string>("UserName");

                    b.HasKey("LogId");

                    b.ToTable("Logs");
                });

            modelBuilder.Entity("Organiser.Models.NewMessagesMonitor", b =>
                {
                    b.Property<int>("NewMessagesMonitorId")
                        .ValueGeneratedOnAdd();

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

            modelBuilder.Entity("Organiser.Models.Note", b =>
                {
                    b.Property<int>("NoteId")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("Author");

                    b.Property<string>("Content")
                        .IsRequired()
                        .HasColumnType("ntext");

                    b.Property<DateTime>("CreatedAt");

                    b.Property<int>("Location");

                    b.HasKey("NoteId");

                    b.ToTable("Notes");
                });

            modelBuilder.Entity("Organiser.Models.Order", b =>
                {
                    b.Property<int>("OrderId")
                        .ValueGeneratedOnAdd();

                    b.Property<DateTime>("CreatedAt");

                    b.Property<string>("Customer");

                    b.Property<DateTime>("DeadLineDate");

                    b.Property<int>("EntitiesCompleted");

                    b.Property<int>("EntitiesInProgress");

                    b.Property<int>("EntitiesNotProcessed");

                    b.Property<int>("EntityCount");

                    b.Property<string>("EntityType")
                        .IsRequired()
                        .HasMaxLength(30);

                    b.Property<DateTime>("FinshedAt");

                    b.Property<string>("OrderNumber")
                        .IsRequired()
                        .HasMaxLength(50);

                    b.Property<DateTime>("StartedAt");

                    b.Property<string>("Status")
                        .HasMaxLength(30);

                    b.HasKey("OrderId");

                    b.ToTable("Orders");
                });

            modelBuilder.Entity("Organiser.Models.User", b =>
                {
                    b.Property<int>("UserId")
                        .ValueGeneratedOnAdd();

                    b.Property<bool>("IsAdmin");

                    b.Property<string>("Password")
                        .IsRequired()
                        .HasMaxLength(25);

                    b.Property<string>("UserName")
                        .IsRequired()
                        .HasMaxLength(30);

                    b.HasKey("UserId");

                    b.ToTable("Users");
                });

            modelBuilder.Entity("Organiser.Models.UserRole", b =>
                {
                    b.Property<int>("UserRoleId")
                        .ValueGeneratedOnAdd();

                    b.Property<int>("Role");

                    b.Property<int>("UserId");

                    b.HasKey("UserRoleId");

                    b.HasIndex("UserId");

                    b.ToTable("UserRoles");
                });

            modelBuilder.Entity("Organiser.Models.DepartmentState", b =>
                {
                    b.HasOne("Organiser.Models.Order", "Order")
                        .WithMany("DepartmentStates")
                        .HasForeignKey("OrderId")
                        .HasConstraintName("ForeignKey_DepartmentState_Order")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("Organiser.Models.UserRole", b =>
                {
                    b.HasOne("Organiser.Models.User", "User")
                        .WithMany("UserRoles")
                        .HasForeignKey("UserId")
                        .HasConstraintName("ForeignKey_UserRole_User")
                        .OnDelete(DeleteBehavior.Cascade);
                });
#pragma warning restore 612, 618
        }
    }
}