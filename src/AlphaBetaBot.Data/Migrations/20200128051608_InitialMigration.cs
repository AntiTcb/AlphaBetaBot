using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

namespace AlphaBetaBot.Data.Migrations
{
    public partial class InitialMigration : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "raid_locations",
                columns: table => new
                {
                    RaidLocationId = table.Column<int>(nullable: false),
                    Name = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_raid_locations", x => x.RaidLocationId);
                });

            migrationBuilder.CreateTable(
                name: "users",
                columns: table => new
                {
                    Id = table.Column<decimal>(nullable: false),
                    CreatedAt = table.Column<DateTimeOffset>(nullable: false, defaultValue: new DateTimeOffset(new DateTime(2020, 1, 28, 5, 16, 7, 996, DateTimeKind.Unspecified).AddTicks(4628), new TimeSpan(0, 0, 0, 0, 0)))
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_users", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "raids",
                columns: table => new
                {
                    Id = table.Column<decimal>(nullable: false),
                    CreatedAt = table.Column<DateTimeOffset>(nullable: false, defaultValue: new DateTimeOffset(new DateTime(2020, 1, 28, 5, 16, 8, 5, DateTimeKind.Unspecified).AddTicks(7378), new TimeSpan(0, 0, 0, 0, 0))),
                    RaidTime = table.Column<DateTimeOffset>(nullable: false),
                    RaidLocationId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_raids", x => x.Id);
                    table.ForeignKey(
                        name: "FK_raids_raid_locations_RaidLocationId",
                        column: x => x.RaidLocationId,
                        principalTable: "raid_locations",
                        principalColumn: "RaidLocationId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "characters",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    OwnerId = table.Column<decimal>(nullable: true),
                    CharacterName = table.Column<string>(nullable: false),
                    Class = table.Column<int>(nullable: false),
                    Role = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_characters", x => x.Id);
                    table.ForeignKey(
                        name: "FK_characters_users_OwnerId",
                        column: x => x.OwnerId,
                        principalTable: "users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "raid_participants",
                columns: table => new
                {
                    CharacterId = table.Column<int>(nullable: false),
                    RaidId = table.Column<decimal>(nullable: false),
                    Id = table.Column<decimal>(nullable: false),
                    CreatedAt = table.Column<DateTimeOffset>(nullable: false, defaultValue: new DateTimeOffset(new DateTime(2020, 1, 28, 5, 16, 8, 7, DateTimeKind.Unspecified).AddTicks(6426), new TimeSpan(0, 0, 0, 0, 0)))
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_raid_participants", x => new { x.RaidId, x.CharacterId });
                    table.ForeignKey(
                        name: "FK_raid_participants_characters_CharacterId",
                        column: x => x.CharacterId,
                        principalTable: "characters",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_raid_participants_raids_RaidId",
                        column: x => x.RaidId,
                        principalTable: "raids",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_characters_OwnerId",
                table: "characters",
                column: "OwnerId");

            migrationBuilder.CreateIndex(
                name: "IX_raid_participants_CharacterId",
                table: "raid_participants",
                column: "CharacterId");

            migrationBuilder.CreateIndex(
                name: "IX_raids_RaidLocationId",
                table: "raids",
                column: "RaidLocationId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "raid_participants");

            migrationBuilder.DropTable(
                name: "characters");

            migrationBuilder.DropTable(
                name: "raids");

            migrationBuilder.DropTable(
                name: "users");

            migrationBuilder.DropTable(
                name: "raid_locations");
        }
    }
}
