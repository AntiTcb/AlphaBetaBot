using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace AlphaBetaBot.Data.Migrations
{
    public partial class Guilds : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<DateTimeOffset>(
                name: "CreatedAt",
                table: "users",
                nullable: false,
                defaultValue: new DateTimeOffset(new DateTime(2020, 1, 28, 5, 22, 24, 218, DateTimeKind.Unspecified).AddTicks(3305), new TimeSpan(0, 0, 0, 0, 0)),
                oldClrType: typeof(DateTimeOffset),
                oldType: "timestamp with time zone",
                oldDefaultValue: new DateTimeOffset(new DateTime(2020, 1, 28, 5, 16, 7, 996, DateTimeKind.Unspecified).AddTicks(4628), new TimeSpan(0, 0, 0, 0, 0)));

            migrationBuilder.AlterColumn<DateTimeOffset>(
                name: "CreatedAt",
                table: "raids",
                nullable: false,
                defaultValue: new DateTimeOffset(new DateTime(2020, 1, 28, 5, 22, 24, 227, DateTimeKind.Unspecified).AddTicks(9381), new TimeSpan(0, 0, 0, 0, 0)),
                oldClrType: typeof(DateTimeOffset),
                oldType: "timestamp with time zone",
                oldDefaultValue: new DateTimeOffset(new DateTime(2020, 1, 28, 5, 16, 8, 5, DateTimeKind.Unspecified).AddTicks(7378), new TimeSpan(0, 0, 0, 0, 0)));

            migrationBuilder.AlterColumn<DateTimeOffset>(
                name: "CreatedAt",
                table: "raid_participants",
                nullable: false,
                defaultValue: new DateTimeOffset(new DateTime(2020, 1, 28, 5, 22, 24, 229, DateTimeKind.Unspecified).AddTicks(8648), new TimeSpan(0, 0, 0, 0, 0)),
                oldClrType: typeof(DateTimeOffset),
                oldType: "timestamp with time zone",
                oldDefaultValue: new DateTimeOffset(new DateTime(2020, 1, 28, 5, 16, 8, 7, DateTimeKind.Unspecified).AddTicks(6426), new TimeSpan(0, 0, 0, 0, 0)));

            migrationBuilder.CreateTable(
                name: "guilds",
                columns: table => new
                {
                    Id = table.Column<decimal>(nullable: false),
                    CreatedAt = table.Column<DateTimeOffset>(nullable: false, defaultValue: new DateTimeOffset(new DateTime(2020, 1, 28, 5, 22, 24, 236, DateTimeKind.Unspecified).AddTicks(1299), new TimeSpan(0, 0, 0, 0, 0))),
                    RaidSignupChannelId = table.Column<decimal>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_guilds", x => x.Id);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "guilds");

            migrationBuilder.AlterColumn<DateTimeOffset>(
                name: "CreatedAt",
                table: "users",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTimeOffset(new DateTime(2020, 1, 28, 5, 16, 7, 996, DateTimeKind.Unspecified).AddTicks(4628), new TimeSpan(0, 0, 0, 0, 0)),
                oldClrType: typeof(DateTimeOffset),
                oldDefaultValue: new DateTimeOffset(new DateTime(2020, 1, 28, 5, 22, 24, 218, DateTimeKind.Unspecified).AddTicks(3305), new TimeSpan(0, 0, 0, 0, 0)));

            migrationBuilder.AlterColumn<DateTimeOffset>(
                name: "CreatedAt",
                table: "raids",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTimeOffset(new DateTime(2020, 1, 28, 5, 16, 8, 5, DateTimeKind.Unspecified).AddTicks(7378), new TimeSpan(0, 0, 0, 0, 0)),
                oldClrType: typeof(DateTimeOffset),
                oldDefaultValue: new DateTimeOffset(new DateTime(2020, 1, 28, 5, 22, 24, 227, DateTimeKind.Unspecified).AddTicks(9381), new TimeSpan(0, 0, 0, 0, 0)));

            migrationBuilder.AlterColumn<DateTimeOffset>(
                name: "CreatedAt",
                table: "raid_participants",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTimeOffset(new DateTime(2020, 1, 28, 5, 16, 8, 7, DateTimeKind.Unspecified).AddTicks(6426), new TimeSpan(0, 0, 0, 0, 0)),
                oldClrType: typeof(DateTimeOffset),
                oldDefaultValue: new DateTimeOffset(new DateTime(2020, 1, 28, 5, 22, 24, 229, DateTimeKind.Unspecified).AddTicks(8648), new TimeSpan(0, 0, 0, 0, 0)));
        }
    }
}
