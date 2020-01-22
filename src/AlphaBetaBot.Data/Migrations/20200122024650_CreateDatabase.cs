using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

namespace AlphaBetaBot.Data.Migrations
{
    public partial class CreateDatabase : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "raids",
                columns: table => new
                {
                    key = table.Column<int>(nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    created_at = table.Column<DateTimeOffset>(nullable: false),
                    last_updated_at = table.Column<DateTimeOffset>(nullable: false),
                    RaidTime = table.Column<DateTimeOffset>(nullable: false),
                    Raid = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_raids", x => x.key);
                });

            migrationBuilder.CreateTable(
                name: "users",
                columns: table => new
                {
                    key = table.Column<int>(nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    created_at = table.Column<DateTimeOffset>(nullable: false),
                    last_updated_at = table.Column<DateTimeOffset>(nullable: false),
                    snowflake_id = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_users", x => x.key);
                });

            migrationBuilder.CreateTable(
                name: "characters",
                columns: table => new
                {
                    key = table.Column<int>(nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    created_at = table.Column<DateTimeOffset>(nullable: false),
                    last_updated_at = table.Column<DateTimeOffset>(nullable: false),
                    OwnerId = table.Column<int>(nullable: true),
                    CharacterName = table.Column<string>(nullable: false),
                    Class = table.Column<int>(nullable: false),
                    Role = table.Column<int>(nullable: false),
                    WowRaidId = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_characters", x => x.key);
                    table.ForeignKey(
                        name: "FK_characters_users_OwnerId",
                        column: x => x.OwnerId,
                        principalTable: "users",
                        principalColumn: "key",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_characters_raids_WowRaidId",
                        column: x => x.WowRaidId,
                        principalTable: "raids",
                        principalColumn: "key",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_characters_OwnerId",
                table: "characters",
                column: "OwnerId");

            migrationBuilder.CreateIndex(
                name: "IX_characters_WowRaidId",
                table: "characters",
                column: "WowRaidId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "characters");

            migrationBuilder.DropTable(
                name: "users");

            migrationBuilder.DropTable(
                name: "raids");
        }
    }
}
