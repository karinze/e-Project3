using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AptitudeWebApp.Migrations
{
    /// <inheritdoc />
    public partial class removeUnnecessaryTables : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ExamQuestions_ExamCategories_ExamCategoryCategoryId",
                table: "ExamQuestions");

            migrationBuilder.DropTable(
                name: "ApplicantCategories");

            migrationBuilder.DropTable(
                name: "ExamCategories");

            migrationBuilder.DropColumn(
                name: "ApplicantId",
                table: "Exams");

            migrationBuilder.DropColumn(
                name: "EndTime",
                table: "Exams");

            migrationBuilder.DropColumn(
                name: "CategoryId",
                table: "ExamQuestions");

            migrationBuilder.DropColumn(
                name: "ExamStartDate",
                table: "ApplicantExams");

            migrationBuilder.DropColumn(
                name: "IsCompleted",
                table: "ApplicantExams");

            migrationBuilder.RenameColumn(
                name: "HasStartedYet",
                table: "Exams",
                newName: "ExamTypeId");

            migrationBuilder.RenameColumn(
                name: "QuestionMark",
                table: "ExamQuestions",
                newName: "QuestionScore");

            migrationBuilder.RenameColumn(
                name: "ExamCategoryCategoryId",
                table: "ExamQuestions",
                newName: "ExamId");

            migrationBuilder.RenameIndex(
                name: "IX_ExamQuestions_ExamCategoryCategoryId",
                table: "ExamQuestions",
                newName: "IX_ExamQuestions_ExamId");

            migrationBuilder.AddColumn<string>(
                name: "Email",
                table: "Managers",
                type: "nvarchar(255)",
                maxLength: 255,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "PhoneNumber",
                table: "Managers",
                type: "nvarchar(255)",
                maxLength: 255,
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsActive",
                table: "Exams",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<int>(
                name: "ExamTypeId",
                table: "ExamQuestions",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AlterColumn<string>(
                name: "Username",
                table: "Applicants",
                type: "nvarchar(512)",
                maxLength: 512,
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(512)",
                oldMaxLength: 512,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Password",
                table: "Applicants",
                type: "nvarchar(512)",
                maxLength: 512,
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(512)",
                oldMaxLength: 512,
                oldNullable: true);

            migrationBuilder.AddColumn<string>(
                name: "PhoneNumber",
                table: "Applicants",
                type: "nvarchar(255)",
                maxLength: 255,
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "CurrentExamTypeId",
                table: "ApplicantExams",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "ExamId",
                table: "ApplicantExams",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddForeignKey(
                name: "FK_ExamQuestions_Exams_ExamId",
                table: "ExamQuestions",
                column: "ExamId",
                principalTable: "Exams",
                principalColumn: "ExamId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ExamQuestions_Exams_ExamId",
                table: "ExamQuestions");

            migrationBuilder.DropColumn(
                name: "Email",
                table: "Managers");

            migrationBuilder.DropColumn(
                name: "PhoneNumber",
                table: "Managers");

            migrationBuilder.DropColumn(
                name: "IsActive",
                table: "Exams");

            migrationBuilder.DropColumn(
                name: "ExamTypeId",
                table: "ExamQuestions");

            migrationBuilder.DropColumn(
                name: "PhoneNumber",
                table: "Applicants");

            migrationBuilder.DropColumn(
                name: "CurrentExamTypeId",
                table: "ApplicantExams");

            migrationBuilder.DropColumn(
                name: "ExamId",
                table: "ApplicantExams");

            migrationBuilder.RenameColumn(
                name: "ExamTypeId",
                table: "Exams",
                newName: "HasStartedYet");

            migrationBuilder.RenameColumn(
                name: "QuestionScore",
                table: "ExamQuestions",
                newName: "QuestionMark");

            migrationBuilder.RenameColumn(
                name: "ExamId",
                table: "ExamQuestions",
                newName: "ExamCategoryCategoryId");

            migrationBuilder.RenameIndex(
                name: "IX_ExamQuestions_ExamId",
                table: "ExamQuestions",
                newName: "IX_ExamQuestions_ExamCategoryCategoryId");

            migrationBuilder.AddColumn<int>(
                name: "ApplicantId",
                table: "Exams",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<DateTime>(
                name: "EndTime",
                table: "Exams",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "CategoryId",
                table: "ExamQuestions",
                type: "int",
                nullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Username",
                table: "Applicants",
                type: "nvarchar(512)",
                maxLength: 512,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(512)",
                oldMaxLength: 512);

            migrationBuilder.AlterColumn<string>(
                name: "Password",
                table: "Applicants",
                type: "nvarchar(512)",
                maxLength: 512,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(512)",
                oldMaxLength: 512);

            migrationBuilder.AddColumn<DateTime>(
                name: "ExamStartDate",
                table: "ApplicantExams",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsCompleted",
                table: "ApplicantExams",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.CreateTable(
                name: "ApplicantCategories",
                columns: table => new
                {
                    ApplicantCategoryId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ApplicantId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CategoryId = table.Column<int>(type: "int", nullable: true),
                    HasCleared = table.Column<bool>(type: "bit", nullable: false),
                    Score = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ApplicantCategories", x => x.ApplicantCategoryId);
                });

            migrationBuilder.CreateTable(
                name: "ExamCategories",
                columns: table => new
                {
                    CategoryId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CategoryName = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    ExamId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ExamCategories", x => x.CategoryId);
                    table.ForeignKey(
                        name: "FK_ExamCategories_Exams_ExamId",
                        column: x => x.ExamId,
                        principalTable: "Exams",
                        principalColumn: "ExamId");
                });

            migrationBuilder.CreateIndex(
                name: "IX_ExamCategories_ExamId",
                table: "ExamCategories",
                column: "ExamId");

            migrationBuilder.AddForeignKey(
                name: "FK_ExamQuestions_ExamCategories_ExamCategoryCategoryId",
                table: "ExamQuestions",
                column: "ExamCategoryCategoryId",
                principalTable: "ExamCategories",
                principalColumn: "CategoryId");
        }
    }
}
