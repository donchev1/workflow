using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Organiser.Data.Migrations
{
    public partial class sdd : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Departments",
                columns: table => new
                {
                    DepartmentId = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Name = table.Column<string>(nullable: true),
                    UserId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Departments", x => x.DepartmentId);
                });

            migrationBuilder.CreateTable(
                name: "Logs",
                columns: table => new
                {
                    LogId = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    ActionRecord = table.Column<string>(type: "ntext", nullable: false),
                    CreatedAt = table.Column<DateTime>(nullable: false),
                    OrderNumber = table.Column<string>(nullable: true),
                    UserName = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Logs", x => x.LogId);
                });

            migrationBuilder.CreateTable(
                name: "NewMessagesMonitor",
                columns: table => new
                {
                    NewMessagesMonitorId = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Covertirung = table.Column<bool>(nullable: false),
                    Fahrer = table.Column<bool>(nullable: false),
                    Falcen = table.Column<bool>(nullable: false),
                    Folirung = table.Column<bool>(nullable: false),
                    Handarbeit = table.Column<bool>(nullable: false),
                    Inkchet = table.Column<bool>(nullable: false),
                    Lager = table.Column<bool>(nullable: false)
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
                    Customer = table.Column<string>(nullable: true),
                    DeadLineDate = table.Column<DateTime>(nullable: false),
                    EntitiesCompleted = table.Column<int>(nullable: false),
                    EntitiesInProgress = table.Column<int>(nullable: false),
                    EntitiesNotProcessed = table.Column<int>(nullable: false),
                    EntityCount = table.Column<int>(nullable: false),
                    EntityType = table.Column<string>(maxLength: 30, nullable: false),
                    FinshedAt = table.Column<DateTime>(nullable: false),
                    OrderNumber = table.Column<string>(maxLength: 50, nullable: false),
                    StartedAt = table.Column<DateTime>(nullable: false),
                    Status = table.Column<string>(maxLength: 30, nullable: true)
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
                name: "DepartmentStates",
                columns: table => new
                {
                    DepartmentStateId = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    EntitiesInProgress = table.Column<int>(nullable: false),
                    EntitiesPassed = table.Column<int>(nullable: false),
                    EntitiesRFC = table.Column<int>(nullable: false),
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
                    table.PrimaryKey("PK_DepartmentStates", x => x.DepartmentStateId);
                    table.ForeignKey(
                        name: "ForeignKey_DepartmentState_Order",
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
                name: "IX_DepartmentStates_OrderId",
                table: "DepartmentStates",
                column: "OrderId");

            migrationBuilder.CreateIndex(
                name: "IX_UserRoles_UserId",
                table: "UserRoles",
                column: "UserId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Departments");

            migrationBuilder.DropTable(
                name: "DepartmentStates");

            migrationBuilder.DropTable(
                name: "Logs");

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
