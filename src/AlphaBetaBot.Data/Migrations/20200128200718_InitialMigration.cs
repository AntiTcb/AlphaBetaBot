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
                name: "guilds",
                columns: table => new
                {
                    Id = table.Column<decimal>(nullable: false),
                    CreatedAt = table.Column<DateTimeOffset>(nullable: false, defaultValue: new DateTimeOffset(new DateTime(2020, 1, 28, 20, 7, 18, 248, DateTimeKind.Unspecified).AddTicks(6035), new TimeSpan(0, 0, 0, 0, 0))),
                    RaidSignupChannelId = table.Column<decimal>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_guilds", x => x.Id);
                });

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
                    CreatedAt = table.Column<DateTimeOffset>(nullable: false, defaultValue: new DateTimeOffset(new DateTime(2020, 1, 28, 20, 7, 18, 225, DateTimeKind.Unspecified).AddTicks(1950), new TimeSpan(0, 0, 0, 0, 0)))
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
                    CreatedAt = table.Column<DateTimeOffset>(nullable: false, defaultValue: new DateTimeOffset(new DateTime(2020, 1, 28, 20, 7, 18, 237, DateTimeKind.Unspecified).AddTicks(6244), new TimeSpan(0, 0, 0, 0, 0))),
                    RaidTime = table.Column<string>(nullable: true),
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
                    Id = table.Column<int>(nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    CharacterId = table.Column<int>(nullable: false),
                    RaidId = table.Column<decimal>(nullable: false),
                    SignedUpAt = table.Column<DateTimeOffset>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_raid_participants", x => x.Id);
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

            migrationBuilder.InsertData(
                table: "raid_locations",
                columns: new[] { "RaidLocationId", "Name" },
                values: new object[,]
                {
                    { 0, "Onyxia" },
                    { 1, "MoltenCore" },
                    { 2, "BlackwingLair" },
                    { 3, "ZulGurub" },
                    { 4, "AQ20" },
                    { 5, "AQ40" },
                    { 6, "Naxxramas" }
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
                name: "IX_raid_participants_RaidId",
                table: "raid_participants",
                column: "RaidId");

            migrationBuilder.CreateIndex(
                name: "IX_raids_RaidLocationId",
                table: "raids",
                column: "RaidLocationId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "guilds");

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
