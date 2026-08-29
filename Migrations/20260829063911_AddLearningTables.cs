using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace be.Migrations
{
    /// <inheritdoc />
    public partial class AddLearningTables : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_LearningContent_LearningModule_LearningModuleId",
                table: "LearningContent");

            migrationBuilder.DropForeignKey(
                name: "FK_LearningModule_TrainingDomain_TrainingDomainId",
                table: "LearningModule");

            migrationBuilder.DropForeignKey(
                name: "FK_LearningProgress_LearningContent_LearningContentId",
                table: "LearningProgress");

            migrationBuilder.DropForeignKey(
                name: "FK_LearningProgress_Users_UserId",
                table: "LearningProgress");

            migrationBuilder.DropPrimaryKey(
                name: "PK_TrainingDomain",
                table: "TrainingDomain");

            migrationBuilder.DropPrimaryKey(
                name: "PK_LearningProgress",
                table: "LearningProgress");

            migrationBuilder.DropPrimaryKey(
                name: "PK_LearningModule",
                table: "LearningModule");

            migrationBuilder.DropPrimaryKey(
                name: "PK_LearningContent",
                table: "LearningContent");

            migrationBuilder.RenameTable(
                name: "TrainingDomain",
                newName: "TrainingDomains");

            migrationBuilder.RenameTable(
                name: "LearningProgress",
                newName: "LearningProgresses");

            migrationBuilder.RenameTable(
                name: "LearningModule",
                newName: "LearningModules");

            migrationBuilder.RenameTable(
                name: "LearningContent",
                newName: "LearningContents");

            migrationBuilder.RenameIndex(
                name: "IX_LearningProgress_UserId_LearningContentId",
                table: "LearningProgresses",
                newName: "IX_LearningProgresses_UserId_LearningContentId");

            migrationBuilder.RenameIndex(
                name: "IX_LearningProgress_LearningContentId",
                table: "LearningProgresses",
                newName: "IX_LearningProgresses_LearningContentId");

            migrationBuilder.RenameIndex(
                name: "IX_LearningModule_TrainingDomainId",
                table: "LearningModules",
                newName: "IX_LearningModules_TrainingDomainId");

            migrationBuilder.RenameIndex(
                name: "IX_LearningContent_LearningModuleId",
                table: "LearningContents",
                newName: "IX_LearningContents_LearningModuleId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_TrainingDomains",
                table: "TrainingDomains",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_LearningProgresses",
                table: "LearningProgresses",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_LearningModules",
                table: "LearningModules",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_LearningContents",
                table: "LearningContents",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_LearningContents_LearningModules_LearningModuleId",
                table: "LearningContents",
                column: "LearningModuleId",
                principalTable: "LearningModules",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_LearningModules_TrainingDomains_TrainingDomainId",
                table: "LearningModules",
                column: "TrainingDomainId",
                principalTable: "TrainingDomains",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_LearningProgresses_LearningContents_LearningContentId",
                table: "LearningProgresses",
                column: "LearningContentId",
                principalTable: "LearningContents",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_LearningProgresses_Users_UserId",
                table: "LearningProgresses",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_LearningContents_LearningModules_LearningModuleId",
                table: "LearningContents");

            migrationBuilder.DropForeignKey(
                name: "FK_LearningModules_TrainingDomains_TrainingDomainId",
                table: "LearningModules");

            migrationBuilder.DropForeignKey(
                name: "FK_LearningProgresses_LearningContents_LearningContentId",
                table: "LearningProgresses");

            migrationBuilder.DropForeignKey(
                name: "FK_LearningProgresses_Users_UserId",
                table: "LearningProgresses");

            migrationBuilder.DropPrimaryKey(
                name: "PK_TrainingDomains",
                table: "TrainingDomains");

            migrationBuilder.DropPrimaryKey(
                name: "PK_LearningProgresses",
                table: "LearningProgresses");

            migrationBuilder.DropPrimaryKey(
                name: "PK_LearningModules",
                table: "LearningModules");

            migrationBuilder.DropPrimaryKey(
                name: "PK_LearningContents",
                table: "LearningContents");

            migrationBuilder.RenameTable(
                name: "TrainingDomains",
                newName: "TrainingDomain");

            migrationBuilder.RenameTable(
                name: "LearningProgresses",
                newName: "LearningProgress");

            migrationBuilder.RenameTable(
                name: "LearningModules",
                newName: "LearningModule");

            migrationBuilder.RenameTable(
                name: "LearningContents",
                newName: "LearningContent");

            migrationBuilder.RenameIndex(
                name: "IX_LearningProgresses_UserId_LearningContentId",
                table: "LearningProgress",
                newName: "IX_LearningProgress_UserId_LearningContentId");

            migrationBuilder.RenameIndex(
                name: "IX_LearningProgresses_LearningContentId",
                table: "LearningProgress",
                newName: "IX_LearningProgress_LearningContentId");

            migrationBuilder.RenameIndex(
                name: "IX_LearningModules_TrainingDomainId",
                table: "LearningModule",
                newName: "IX_LearningModule_TrainingDomainId");

            migrationBuilder.RenameIndex(
                name: "IX_LearningContents_LearningModuleId",
                table: "LearningContent",
                newName: "IX_LearningContent_LearningModuleId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_TrainingDomain",
                table: "TrainingDomain",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_LearningProgress",
                table: "LearningProgress",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_LearningModule",
                table: "LearningModule",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_LearningContent",
                table: "LearningContent",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_LearningContent_LearningModule_LearningModuleId",
                table: "LearningContent",
                column: "LearningModuleId",
                principalTable: "LearningModule",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_LearningModule_TrainingDomain_TrainingDomainId",
                table: "LearningModule",
                column: "TrainingDomainId",
                principalTable: "TrainingDomain",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_LearningProgress_LearningContent_LearningContentId",
                table: "LearningProgress",
                column: "LearningContentId",
                principalTable: "LearningContent",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_LearningProgress_Users_UserId",
                table: "LearningProgress",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
