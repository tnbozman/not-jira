using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace NotJira.Api.Migrations
{
    /// <inheritdoc />
    public partial class AddExternalEntitiesFeature : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ExternalEntities",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Type = table.Column<int>(type: "integer", nullable: false),
                    Name = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    Email = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: true),
                    Organization = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: true),
                    Phone = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    Notes = table.Column<string>(type: "character varying(2000)", maxLength: 2000, nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    ProjectId = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ExternalEntities", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ExternalEntities_Projects_ProjectId",
                        column: x => x.ProjectId,
                        principalTable: "Projects",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Tags",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Name = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    Description = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    ProjectId = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Tags", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Tags_Projects_ProjectId",
                        column: x => x.ProjectId,
                        principalTable: "Projects",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Interviews",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Type = table.Column<int>(type: "integer", nullable: false),
                    InterviewDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    Interviewer = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    Summary = table.Column<string>(type: "character varying(2000)", maxLength: 2000, nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    ExternalEntityId = table.Column<int>(type: "integer", nullable: false),
                    ProjectId = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Interviews", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Interviews_ExternalEntities_ExternalEntityId",
                        column: x => x.ExternalEntityId,
                        principalTable: "ExternalEntities",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Interviews_Projects_ProjectId",
                        column: x => x.ProjectId,
                        principalTable: "Projects",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Outcomes",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Description = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: false),
                    Priority = table.Column<int>(type: "integer", nullable: false),
                    Context = table.Column<string>(type: "character varying(2000)", maxLength: 2000, nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    ExternalEntityId = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Outcomes", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Outcomes_ExternalEntities_ExternalEntityId",
                        column: x => x.ExternalEntityId,
                        principalTable: "ExternalEntities",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Problems",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Description = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: false),
                    Severity = table.Column<int>(type: "integer", nullable: false),
                    Context = table.Column<string>(type: "character varying(2000)", maxLength: 2000, nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    ExternalEntityId = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Problems", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Problems_ExternalEntities_ExternalEntityId",
                        column: x => x.ExternalEntityId,
                        principalTable: "ExternalEntities",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "EntityTag",
                columns: table => new
                {
                    ExternalEntityId = table.Column<int>(type: "integer", nullable: false),
                    TagId = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EntityTag", x => new { x.ExternalEntityId, x.TagId });
                    table.ForeignKey(
                        name: "FK_EntityTag_ExternalEntities_ExternalEntityId",
                        column: x => x.ExternalEntityId,
                        principalTable: "ExternalEntities",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_EntityTag_Tags_TagId",
                        column: x => x.TagId,
                        principalTable: "Tags",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "OutcomeTag",
                columns: table => new
                {
                    OutcomeId = table.Column<int>(type: "integer", nullable: false),
                    TagId = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OutcomeTag", x => new { x.OutcomeId, x.TagId });
                    table.ForeignKey(
                        name: "FK_OutcomeTag_Outcomes_OutcomeId",
                        column: x => x.OutcomeId,
                        principalTable: "Outcomes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_OutcomeTag_Tags_TagId",
                        column: x => x.TagId,
                        principalTable: "Tags",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "SuccessMetrics",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Description = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: false),
                    TargetValue = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    CurrentValue = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    Unit = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    OutcomeId = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SuccessMetrics", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SuccessMetrics_Outcomes_OutcomeId",
                        column: x => x.OutcomeId,
                        principalTable: "Outcomes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "InterviewNotes",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Content = table.Column<string>(type: "character varying(2000)", maxLength: 2000, nullable: false),
                    RelatedProblemId = table.Column<int>(type: "integer", nullable: true),
                    RelatedOutcomeId = table.Column<int>(type: "integer", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    InterviewId = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_InterviewNotes", x => x.Id);
                    table.ForeignKey(
                        name: "FK_InterviewNotes_Interviews_InterviewId",
                        column: x => x.InterviewId,
                        principalTable: "Interviews",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_InterviewNotes_Outcomes_RelatedOutcomeId",
                        column: x => x.RelatedOutcomeId,
                        principalTable: "Outcomes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_InterviewNotes_Problems_RelatedProblemId",
                        column: x => x.RelatedProblemId,
                        principalTable: "Problems",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateTable(
                name: "OutcomeProblem",
                columns: table => new
                {
                    OutcomesId = table.Column<int>(type: "integer", nullable: false),
                    ProblemsId = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OutcomeProblem", x => new { x.OutcomesId, x.ProblemsId });
                    table.ForeignKey(
                        name: "FK_OutcomeProblem_Outcomes_OutcomesId",
                        column: x => x.OutcomesId,
                        principalTable: "Outcomes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_OutcomeProblem_Problems_ProblemsId",
                        column: x => x.ProblemsId,
                        principalTable: "Problems",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ProblemTag",
                columns: table => new
                {
                    ProblemId = table.Column<int>(type: "integer", nullable: false),
                    TagId = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProblemTag", x => new { x.ProblemId, x.TagId });
                    table.ForeignKey(
                        name: "FK_ProblemTag_Problems_ProblemId",
                        column: x => x.ProblemId,
                        principalTable: "Problems",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ProblemTag_Tags_TagId",
                        column: x => x.TagId,
                        principalTable: "Tags",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_EntityTag_TagId",
                table: "EntityTag",
                column: "TagId");

            migrationBuilder.CreateIndex(
                name: "IX_ExternalEntities_ProjectId",
                table: "ExternalEntities",
                column: "ProjectId");

            migrationBuilder.CreateIndex(
                name: "IX_InterviewNotes_InterviewId",
                table: "InterviewNotes",
                column: "InterviewId");

            migrationBuilder.CreateIndex(
                name: "IX_InterviewNotes_RelatedOutcomeId",
                table: "InterviewNotes",
                column: "RelatedOutcomeId");

            migrationBuilder.CreateIndex(
                name: "IX_InterviewNotes_RelatedProblemId",
                table: "InterviewNotes",
                column: "RelatedProblemId");

            migrationBuilder.CreateIndex(
                name: "IX_Interviews_ExternalEntityId",
                table: "Interviews",
                column: "ExternalEntityId");

            migrationBuilder.CreateIndex(
                name: "IX_Interviews_ProjectId",
                table: "Interviews",
                column: "ProjectId");

            migrationBuilder.CreateIndex(
                name: "IX_OutcomeProblem_ProblemsId",
                table: "OutcomeProblem",
                column: "ProblemsId");

            migrationBuilder.CreateIndex(
                name: "IX_Outcomes_ExternalEntityId",
                table: "Outcomes",
                column: "ExternalEntityId");

            migrationBuilder.CreateIndex(
                name: "IX_OutcomeTag_TagId",
                table: "OutcomeTag",
                column: "TagId");

            migrationBuilder.CreateIndex(
                name: "IX_Problems_ExternalEntityId",
                table: "Problems",
                column: "ExternalEntityId");

            migrationBuilder.CreateIndex(
                name: "IX_ProblemTag_TagId",
                table: "ProblemTag",
                column: "TagId");

            migrationBuilder.CreateIndex(
                name: "IX_SuccessMetrics_OutcomeId",
                table: "SuccessMetrics",
                column: "OutcomeId");

            migrationBuilder.CreateIndex(
                name: "IX_Tags_ProjectId_Name",
                table: "Tags",
                columns: new[] { "ProjectId", "Name" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "EntityTag");

            migrationBuilder.DropTable(
                name: "InterviewNotes");

            migrationBuilder.DropTable(
                name: "OutcomeProblem");

            migrationBuilder.DropTable(
                name: "OutcomeTag");

            migrationBuilder.DropTable(
                name: "ProblemTag");

            migrationBuilder.DropTable(
                name: "SuccessMetrics");

            migrationBuilder.DropTable(
                name: "Interviews");

            migrationBuilder.DropTable(
                name: "Problems");

            migrationBuilder.DropTable(
                name: "Tags");

            migrationBuilder.DropTable(
                name: "Outcomes");

            migrationBuilder.DropTable(
                name: "ExternalEntities");
        }
    }
}
