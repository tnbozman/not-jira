using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace NotJira.Api.Migrations
{
    /// <inheritdoc />
    public partial class AddTeamReleaseAndLeSS : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "AssigneeId",
                table: "Stories",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "AssigneeName",
                table: "Stories",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "ReleaseId",
                table: "Stories",
                type: "integer",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "TeamId",
                table: "Stories",
                type: "integer",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "PlanningOneNotes",
                table: "Sprints",
                type: "character varying(4000)",
                maxLength: 4000,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "RetroNotes",
                table: "Sprints",
                type: "character varying(4000)",
                maxLength: 4000,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ReviewNotes",
                table: "Sprints",
                type: "character varying(4000)",
                maxLength: 4000,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "AssigneeId",
                table: "Spikes",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "AssigneeName",
                table: "Spikes",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "ReleaseId",
                table: "Spikes",
                type: "integer",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "TeamId",
                table: "Spikes",
                type: "integer",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "Releases",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Name = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    Description = table.Column<string>(type: "character varying(2000)", maxLength: 2000, nullable: true),
                    StartDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    ReleaseDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    Status = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    ProjectId = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Releases", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Releases_Projects_ProjectId",
                        column: x => x.ProjectId,
                        principalTable: "Projects",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Teams",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Name = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    Description = table.Column<string>(type: "character varying(2000)", maxLength: 2000, nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    ProjectId = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Teams", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Teams_Projects_ProjectId",
                        column: x => x.ProjectId,
                        principalTable: "Projects",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "TeamPlannings",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    PlanningTwoNotes = table.Column<string>(type: "character varying(4000)", maxLength: 4000, nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    SprintId = table.Column<int>(type: "integer", nullable: false),
                    TeamId = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TeamPlannings", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TeamPlannings_Sprints_SprintId",
                        column: x => x.SprintId,
                        principalTable: "Sprints",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_TeamPlannings_Teams_TeamId",
                        column: x => x.TeamId,
                        principalTable: "Teams",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Stories_ReleaseId",
                table: "Stories",
                column: "ReleaseId");

            migrationBuilder.CreateIndex(
                name: "IX_Stories_TeamId",
                table: "Stories",
                column: "TeamId");

            migrationBuilder.CreateIndex(
                name: "IX_Spikes_ReleaseId",
                table: "Spikes",
                column: "ReleaseId");

            migrationBuilder.CreateIndex(
                name: "IX_Spikes_TeamId",
                table: "Spikes",
                column: "TeamId");

            migrationBuilder.CreateIndex(
                name: "IX_Releases_ProjectId",
                table: "Releases",
                column: "ProjectId");

            migrationBuilder.CreateIndex(
                name: "IX_TeamPlannings_SprintId",
                table: "TeamPlannings",
                column: "SprintId");

            migrationBuilder.CreateIndex(
                name: "IX_TeamPlannings_TeamId",
                table: "TeamPlannings",
                column: "TeamId");

            migrationBuilder.CreateIndex(
                name: "IX_Teams_ProjectId",
                table: "Teams",
                column: "ProjectId");

            migrationBuilder.AddForeignKey(
                name: "FK_Spikes_Releases_ReleaseId",
                table: "Spikes",
                column: "ReleaseId",
                principalTable: "Releases",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);

            migrationBuilder.AddForeignKey(
                name: "FK_Spikes_Teams_TeamId",
                table: "Spikes",
                column: "TeamId",
                principalTable: "Teams",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);

            migrationBuilder.AddForeignKey(
                name: "FK_Stories_Releases_ReleaseId",
                table: "Stories",
                column: "ReleaseId",
                principalTable: "Releases",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);

            migrationBuilder.AddForeignKey(
                name: "FK_Stories_Teams_TeamId",
                table: "Stories",
                column: "TeamId",
                principalTable: "Teams",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Spikes_Releases_ReleaseId",
                table: "Spikes");

            migrationBuilder.DropForeignKey(
                name: "FK_Spikes_Teams_TeamId",
                table: "Spikes");

            migrationBuilder.DropForeignKey(
                name: "FK_Stories_Releases_ReleaseId",
                table: "Stories");

            migrationBuilder.DropForeignKey(
                name: "FK_Stories_Teams_TeamId",
                table: "Stories");

            migrationBuilder.DropTable(
                name: "Releases");

            migrationBuilder.DropTable(
                name: "TeamPlannings");

            migrationBuilder.DropTable(
                name: "Teams");

            migrationBuilder.DropIndex(
                name: "IX_Stories_ReleaseId",
                table: "Stories");

            migrationBuilder.DropIndex(
                name: "IX_Stories_TeamId",
                table: "Stories");

            migrationBuilder.DropIndex(
                name: "IX_Spikes_ReleaseId",
                table: "Spikes");

            migrationBuilder.DropIndex(
                name: "IX_Spikes_TeamId",
                table: "Spikes");

            migrationBuilder.DropColumn(
                name: "AssigneeId",
                table: "Stories");

            migrationBuilder.DropColumn(
                name: "AssigneeName",
                table: "Stories");

            migrationBuilder.DropColumn(
                name: "ReleaseId",
                table: "Stories");

            migrationBuilder.DropColumn(
                name: "TeamId",
                table: "Stories");

            migrationBuilder.DropColumn(
                name: "PlanningOneNotes",
                table: "Sprints");

            migrationBuilder.DropColumn(
                name: "RetroNotes",
                table: "Sprints");

            migrationBuilder.DropColumn(
                name: "ReviewNotes",
                table: "Sprints");

            migrationBuilder.DropColumn(
                name: "AssigneeId",
                table: "Spikes");

            migrationBuilder.DropColumn(
                name: "AssigneeName",
                table: "Spikes");

            migrationBuilder.DropColumn(
                name: "ReleaseId",
                table: "Spikes");

            migrationBuilder.DropColumn(
                name: "TeamId",
                table: "Spikes");
        }
    }
}
