using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace AlphaBetaBot.Data.Migrations
{
    public partial class VirtualNavigationProperties : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<DateTimeOffset>(
                name: "CreatedAt",
                table: "users",
                nullable: false,
                defaultValue: new DateTimeOffset(new DateTime(2020, 1, 24, 22, 28, 15, 680, DateTimeKind.Unspecified).AddTicks(1788), new TimeSpan(0, 0, 0, 0, 0)),
                oldClrType: typeof(DateTimeOffset),
                oldType: "timestamp with time zone",
                oldDefaultValue: new DateTimeOffset(new DateTime(2020, 1, 24, 21, 51, 15, 132, DateTimeKind.Unspecified).AddTicks(7836), new TimeSpan(0, 0, 0, 0, 0)));

            migrationBuilder.AlterColumn<DateTimeOffset>(
                name: "CreatedAt",
                table: "raids",
                nullable: false,
                defaultValue: new DateTimeOffset(new DateTime(2020, 1, 24, 22, 28, 15, 692, DateTimeKind.Unspecified).AddTicks(5239), new TimeSpan(0, 0, 0, 0, 0)),
                oldClrType: typeof(DateTimeOffset),
                oldType: "timestamp with time zone",
                oldDefaultValue: new DateTimeOffset(new DateTime(2020, 1, 24, 21, 51, 15, 144, DateTimeKind.Unspecified).AddTicks(5833), new TimeSpan(0, 0, 0, 0, 0)));

            migrationBuilder.AlterColumn<DateTimeOffset>(
                name: "CreatedAt",
                table: "raid_participants",
                nullable: false,
                defaultValue: new DateTimeOffset(new DateTime(2020, 1, 24, 22, 28, 15, 694, DateTimeKind.Unspecified).AddTicks(8617), new TimeSpan(0, 0, 0, 0, 0)),
                oldClrType: typeof(DateTimeOffset),
                oldType: "timestamp with time zone",
                oldDefaultValue: new DateTimeOffset(new DateTime(2020, 1, 24, 21, 51, 15, 147, DateTimeKind.Unspecified).AddTicks(899), new TimeSpan(0, 0, 0, 0, 0)));
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<DateTimeOffset>(
                name: "CreatedAt",
                table: "users",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTimeOffset(new DateTime(2020, 1, 24, 21, 51, 15, 132, DateTimeKind.Unspecified).AddTicks(7836), new TimeSpan(0, 0, 0, 0, 0)),
                oldClrType: typeof(DateTimeOffset),
                oldDefaultValue: new DateTimeOffset(new DateTime(2020, 1, 24, 22, 28, 15, 680, DateTimeKind.Unspecified).AddTicks(1788), new TimeSpan(0, 0, 0, 0, 0)));

            migrationBuilder.AlterColumn<DateTimeOffset>(
                name: "CreatedAt",
                table: "raids",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTimeOffset(new DateTime(2020, 1, 24, 21, 51, 15, 144, DateTimeKind.Unspecified).AddTicks(5833), new TimeSpan(0, 0, 0, 0, 0)),
                oldClrType: typeof(DateTimeOffset),
                oldDefaultValue: new DateTimeOffset(new DateTime(2020, 1, 24, 22, 28, 15, 692, DateTimeKind.Unspecified).AddTicks(5239), new TimeSpan(0, 0, 0, 0, 0)));

            migrationBuilder.AlterColumn<DateTimeOffset>(
                name: "CreatedAt",
                table: "raid_participants",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTimeOffset(new DateTime(2020, 1, 24, 21, 51, 15, 147, DateTimeKind.Unspecified).AddTicks(899), new TimeSpan(0, 0, 0, 0, 0)),
                oldClrType: typeof(DateTimeOffset),
                oldDefaultValue: new DateTimeOffset(new DateTime(2020, 1, 24, 22, 28, 15, 694, DateTimeKind.Unspecified).AddTicks(8617), new TimeSpan(0, 0, 0, 0, 0)));
        }
    }
}
