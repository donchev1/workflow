using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace Organiser.Migrations
{
    public partial class init : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "NewMessagesMonitor",
                columns: table => new
                {
                    NewMessagesMonitorId = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Drivers = table.Column<bool>(nullable: false),
                    Falcing = table.Column<bool>(nullable: false),
                    Folirane = table.Column<bool>(nullable: false),
                    InkChet = table.Column<bool>(nullable: false),
                    Kovertirane = table.Column<bool>(nullable: false),
                    ManualWork = table.Column<bool>(nullable: false),
                    Sklad = table.Column<bool>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_NewMessagesMonitor", x => x.NewMessagesMonitorId);
                });

            migrationBuilder.CreateTable(
                name: "Notes",
                columns: table => new
                {
                    NoteId = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Author = table.Column<string>(nullable: true),
                    Content = table.Column<string>(type: "ntext", nullable: false),
                    CreatedAt = table.Column<DateTime>(nullable: false),
                    Location = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Notes", x => x.NoteId);
                });

            migrationBuilder.CreateTable(
                name: "Orders",
                columns: table => new
                {
                    OrderId = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    CreatedAt = table.Column<DateTime>(nullable: false),
                    CurrentLocation = table.Column<string>(maxLength: 20, nullable: true),
                    EntitiesCompleted = table.Column<int>(nullable: false),
                    EntitiesInProgress = table.Column<int>(nullable: false),
                    EntitiesNotProcessed = table.Column<int>(nullable: false),
                    EntityCount = table.Column<int>(nullable: false),
                    EntityType = table.Column<string>(maxLength: 20, nullable: true),
                    OrderNumber = table.Column<string>(maxLength: 50, nullable: false),
                    Status = table.Column<string>(maxLength: 50, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Orders", x => x.OrderId);
                });

            migrationBuilder.CreateTable(
                name: "Users",
                columns: table => new
                {
                    UserId = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    IsAdmin = table.Column<bool>(nullable: false),
                    Password = table.Column<string>(maxLength: 25, nullable: false),
                    UserName = table.Column<string>(maxLength: 30, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Users", x => x.UserId);
                });

            migrationBuilder.CreateTable(
                name: "LocStates",
                columns: table => new
                {
                    LocStateId = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    EntitiesInProgress = table.Column<int>(nullable: false),
                    EntitiesReadyForCollection = table.Column<int>(nullable: false),
                    Finish = table.Column<DateTime>(nullable: false),
                    LocationPosition = table.Column<int>(nullable: false),
                    Name = table.Column<string>(nullable: true),
                    OrderId = table.Column<int>(nullable: false),
                    Start = table.Column<DateTime>(nullable: false),
                    Status = table.Column<string>(nullable: true),
                    TotalEntityCount = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LocStates", x => x.LocStateId);
                    table.ForeignKey(
                        name: "ForeignKey_LocState_Order",
                        column: x => x.OrderId,
                        principalTable: "Orders",
                        principalColumn: "OrderId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "UserRoles",
                columns: table => new
                {
                    UserRoleId = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Role = table.Column<int>(nullable: false),
                    UserId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserRoles", x => x.UserRoleId);
                    table.ForeignKey(
                        name: "ForeignKey_UserRole_User",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "UserId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_LocStates_OrderId",
                table: "LocStates",
                column: "OrderId");

            migrationBuilder.CreateIndex(
                name: "IX_UserRoles_UserId",
                table: "UserRoles",
                column: "UserId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "LocStates");

            migrationBuilder.DropTable(
                name: "NewMessagesMonitor");

            migrationBuilder.DropTable(
                name: "Notes");

            migrationBuilder.DropTable(
                name: "UserRoles");

            migrationBuilder.DropTable(
                name: "Orders");

            migrationBuilder.DropTable(
                name: "Users");
        }
    }
}
