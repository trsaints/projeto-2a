using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Agendai.Data.Database.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Events",
                columns: table => new
                {
                    Id = table.Column<ulong>(type: "INTEGER", nullable: false),
                    Name = table.Column<string>(type: "TEXT", nullable: false),
                    AgendaName = table.Column<string>(type: "TEXT", nullable: true),
                    Repeats = table.Column<int>(type: "INTEGER", nullable: false),
                    Reminders = table.Column<string>(type: "TEXT", nullable: true),
                    InitialDue = table.Column<DateTime>(type: "TEXT", nullable: false),
                    Due = table.Column<DateTime>(type: "TEXT", nullable: false),
                    Description = table.Column<string>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Events", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Shifts",
                columns: table => new
                {
                    Id = table.Column<ulong>(type: "INTEGER", nullable: false),
                    Name = table.Column<string>(type: "TEXT", nullable: false),
                    Duration = table.Column<TimeOnly>(type: "TEXT", nullable: false),
                    Status = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Shifts", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Todos",
                columns: table => new
                {
                    Id = table.Column<ulong>(type: "INTEGER", nullable: false),
                    Name = table.Column<string>(type: "TEXT", nullable: false),
                    ListName = table.Column<string>(type: "TEXT", nullable: true),
                    Status = table.Column<int>(type: "INTEGER", nullable: false),
                    FinishedShifts = table.Column<uint>(type: "INTEGER", nullable: false),
                    TotalShifts = table.Column<uint>(type: "INTEGER", nullable: false),
                    Repeats = table.Column<int>(type: "INTEGER", nullable: false),
                    Reminders = table.Column<string>(type: "TEXT", nullable: true),
                    InitialDue = table.Column<DateTime>(type: "TEXT", nullable: false),
                    Due = table.Column<DateTime>(type: "TEXT", nullable: false),
                    Description = table.Column<string>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Todos", x => x.Id);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Events");

            migrationBuilder.DropTable(
                name: "Shifts");

            migrationBuilder.DropTable(
                name: "Todos");
        }
    }
}
