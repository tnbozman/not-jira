using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace NotJira.Api.Migrations
{
    /// <inheritdoc />
    public partial class AddUserStoryMap : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Sprints",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Name = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    Goal = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: true),
                    StartDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    EndDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    Status = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    ProjectId = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Sprints", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Sprints_Projects_ProjectId",
                        column: x => x.ProjectId,
                        principalTable: "Projects",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Themes",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Name = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    Description = table.Column<string>(type: "character varying(2000)", maxLength: 2000, nullable: true),
                    Order = table.Column<int>(type: "integer", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    ProjectId = table.Column<int>(type: "integer", nullable: false),
                    OutcomeId = table.Column<int>(type: "integer", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Themes", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Themes_Outcomes_OutcomeId",
                        column: x => x.OutcomeId,
                        principalTable: "Outcomes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_Themes_Projects_ProjectId",
                        column: x => x.ProjectId,
                        principalTable: "Projects",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Epics",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Name = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    Description = table.Column<string>(type: "character varying(2000)", maxLength: 2000, nullable: true),
                    Order = table.Column<int>(type: "integer", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    ThemeId = table.Column<int>(type: "integer", nullable: false),
                    OutcomeId = table.Column<int>(type: "integer", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Epics", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Epics_Outcomes_OutcomeId",
                        column: x => x.OutcomeId,
                        principalTable: "Outcomes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_Epics_Themes_ThemeId",
                        column: x => x.ThemeId,
                        principalTable: "Themes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Spikes",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Title = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: false),
                    Description = table.Column<string>(type: "character varying(2000)", maxLength: 2000, nullable: true),
                    InvestigationGoal = table.Column<string>(type: "character varying(2000)", maxLength: 2000, nullable: true),
                    Findings = table.Column<string>(type: "character varying(2000)", maxLength: 2000, nullable: true),
                    Order = table.Column<int>(type: "integer", nullable: false),
                    Priority = table.Column<int>(type: "integer", nullable: false),
                    Status = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    StoryPoints = table.Column<int>(type: "integer", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    EpicId = table.Column<int>(type: "integer", nullable: false),
                    SprintId = table.Column<int>(type: "integer", nullable: true),
                    OutcomeId = table.Column<int>(type: "integer", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Spikes", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Spikes_Epics_EpicId",
                        column: x => x.EpicId,
                        principalTable: "Epics",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Spikes_Outcomes_OutcomeId",
                        column: x => x.OutcomeId,
                        principalTable: "Outcomes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_Spikes_Sprints_SprintId",
                        column: x => x.SprintId,
                        principalTable: "Sprints",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateTable(
                name: "Stories",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Title = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: false),
                    Description = table.Column<string>(type: "character varying(2000)", maxLength: 2000, nullable: true),
                    SolutionDescription = table.Column<string>(type: "character varying(2000)", maxLength: 2000, nullable: true),
                    AcceptanceCriteria = table.Column<string>(type: "character varying(2000)", maxLength: 2000, nullable: true),
                    Order = table.Column<int>(type: "integer", nullable: false),
                    Priority = table.Column<int>(type: "integer", nullable: false),
                    Status = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    StoryPoints = table.Column<int>(type: "integer", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    EpicId = table.Column<int>(type: "integer", nullable: false),
                    SprintId = table.Column<int>(type: "integer", nullable: true),
                    OutcomeId = table.Column<int>(type: "integer", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Stories", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Stories_Epics_EpicId",
                        column: x => x.EpicId,
                        principalTable: "Epics",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Stories_Outcomes_OutcomeId",
                        column: x => x.OutcomeId,
                        principalTable: "Outcomes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_Stories_Sprints_SprintId",
                        column: x => x.SprintId,
                        principalTable: "Sprints",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Epics_OutcomeId",
                table: "Epics",
                column: "OutcomeId");

            migrationBuilder.CreateIndex(
                name: "IX_Epics_ThemeId",
                table: "Epics",
                column: "ThemeId");

            migrationBuilder.CreateIndex(
                name: "IX_Spikes_EpicId",
                table: "Spikes",
                column: "EpicId");

            migrationBuilder.CreateIndex(
                name: "IX_Spikes_OutcomeId",
                table: "Spikes",
                column: "OutcomeId");

            migrationBuilder.CreateIndex(
                name: "IX_Spikes_SprintId",
                table: "Spikes",
                column: "SprintId");

            migrationBuilder.CreateIndex(
                name: "IX_Sprints_ProjectId",
                table: "Sprints",
                column: "ProjectId");

            migrationBuilder.CreateIndex(
                name: "IX_Stories_EpicId",
                table: "Stories",
                column: "EpicId");

            migrationBuilder.CreateIndex(
                name: "IX_Stories_OutcomeId",
                table: "Stories",
                column: "OutcomeId");

            migrationBuilder.CreateIndex(
                name: "IX_Stories_SprintId",
                table: "Stories",
                column: "SprintId");

            migrationBuilder.CreateIndex(
                name: "IX_Themes_OutcomeId",
                table: "Themes",
                column: "OutcomeId");

            migrationBuilder.CreateIndex(
                name: "IX_Themes_ProjectId",
                table: "Themes",
                column: "ProjectId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Spikes");

            migrationBuilder.DropTable(
                name: "Stories");

            migrationBuilder.DropTable(
                name: "Epics");

            migrationBuilder.DropTable(
                name: "Sprints");

            migrationBuilder.DropTable(
                name: "Themes");
        }
    }
}
