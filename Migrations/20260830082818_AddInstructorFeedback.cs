using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace be.Migrations
{
    /// <inheritdoc />
    public partial class AddInstructorFeedback : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "InstructorFeedbacks",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    SimulationSessionId = table.Column<Guid>(type: "uuid", nullable: false),
                    InstructorId = table.Column<Guid>(type: "uuid", nullable: false),
                    OverallComments = table.Column<string>(type: "text", nullable: false),
                    EvaluatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_InstructorFeedbacks", x => x.Id);
                    table.ForeignKey(
                        name: "FK_InstructorFeedbacks_SimulationSessions_SimulationSessionId",
                        column: x => x.SimulationSessionId,
                        principalTable: "SimulationSessions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_InstructorFeedbacks_Users_InstructorId",
                        column: x => x.InstructorId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "FeedbackImprovementAreas",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    InstructorFeedbackId = table.Column<Guid>(type: "uuid", nullable: false),
                    Description = table.Column<string>(type: "text", nullable: false),
                    Order = table.Column<int>(type: "integer", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FeedbackImprovementAreas", x => x.Id);
                    table.ForeignKey(
                        name: "FK_FeedbackImprovementAreas_InstructorFeedbacks_InstructorFeed~",
                        column: x => x.InstructorFeedbackId,
                        principalTable: "InstructorFeedbacks",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "FeedbackRecommendedModules",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    InstructorFeedbackId = table.Column<Guid>(type: "uuid", nullable: false),
                    LearningModuleId = table.Column<Guid>(type: "uuid", nullable: false),
                    RecommendationType = table.Column<string>(type: "text", nullable: true),
                    Order = table.Column<int>(type: "integer", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FeedbackRecommendedModules", x => x.Id);
                    table.ForeignKey(
                        name: "FK_FeedbackRecommendedModules_InstructorFeedbacks_InstructorFe~",
                        column: x => x.InstructorFeedbackId,
                        principalTable: "InstructorFeedbacks",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_FeedbackRecommendedModules_LearningModules_LearningModuleId",
                        column: x => x.LearningModuleId,
                        principalTable: "LearningModules",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "FeedbackStrengths",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    InstructorFeedbackId = table.Column<Guid>(type: "uuid", nullable: false),
                    Description = table.Column<string>(type: "text", nullable: false),
                    Order = table.Column<int>(type: "integer", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FeedbackStrengths", x => x.Id);
                    table.ForeignKey(
                        name: "FK_FeedbackStrengths_InstructorFeedbacks_InstructorFeedbackId",
                        column: x => x.InstructorFeedbackId,
                        principalTable: "InstructorFeedbacks",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "SafetyRecommendations",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    InstructorFeedbackId = table.Column<Guid>(type: "uuid", nullable: false),
                    Description = table.Column<string>(type: "text", nullable: false),
                    Priority = table.Column<int>(type: "integer", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SafetyRecommendations", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SafetyRecommendations_InstructorFeedbacks_InstructorFeedbac~",
                        column: x => x.InstructorFeedbackId,
                        principalTable: "InstructorFeedbacks",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ViolationExplanations",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    InstructorFeedbackId = table.Column<Guid>(type: "uuid", nullable: false),
                    SimulationEventId = table.Column<Guid>(type: "uuid", nullable: false),
                    Explanation = table.Column<string>(type: "text", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ViolationExplanations", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ViolationExplanations_InstructorFeedbacks_InstructorFeedbac~",
                        column: x => x.InstructorFeedbackId,
                        principalTable: "InstructorFeedbacks",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ViolationExplanations_SimulationEvents_SimulationEventId",
                        column: x => x.SimulationEventId,
                        principalTable: "SimulationEvents",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_FeedbackImprovementAreas_InstructorFeedbackId",
                table: "FeedbackImprovementAreas",
                column: "InstructorFeedbackId");

            migrationBuilder.CreateIndex(
                name: "IX_FeedbackRecommendedModules_InstructorFeedbackId",
                table: "FeedbackRecommendedModules",
                column: "InstructorFeedbackId");

            migrationBuilder.CreateIndex(
                name: "IX_FeedbackRecommendedModules_LearningModuleId",
                table: "FeedbackRecommendedModules",
                column: "LearningModuleId");

            migrationBuilder.CreateIndex(
                name: "IX_FeedbackStrengths_InstructorFeedbackId",
                table: "FeedbackStrengths",
                column: "InstructorFeedbackId");

            migrationBuilder.CreateIndex(
                name: "IX_InstructorFeedbacks_InstructorId",
                table: "InstructorFeedbacks",
                column: "InstructorId");

            migrationBuilder.CreateIndex(
                name: "IX_InstructorFeedbacks_SimulationSessionId",
                table: "InstructorFeedbacks",
                column: "SimulationSessionId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_SafetyRecommendations_InstructorFeedbackId",
                table: "SafetyRecommendations",
                column: "InstructorFeedbackId");

            migrationBuilder.CreateIndex(
                name: "IX_ViolationExplanations_InstructorFeedbackId",
                table: "ViolationExplanations",
                column: "InstructorFeedbackId");

            migrationBuilder.CreateIndex(
                name: "IX_ViolationExplanations_SimulationEventId",
                table: "ViolationExplanations",
                column: "SimulationEventId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "FeedbackImprovementAreas");

            migrationBuilder.DropTable(
                name: "FeedbackRecommendedModules");

            migrationBuilder.DropTable(
                name: "FeedbackStrengths");

            migrationBuilder.DropTable(
                name: "SafetyRecommendations");

            migrationBuilder.DropTable(
                name: "ViolationExplanations");

            migrationBuilder.DropTable(
                name: "InstructorFeedbacks");
        }
    }
}
