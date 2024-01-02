using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AptitudeWebApp.Migrations
{
    /// <inheritdoc />
    public partial class addMissingStuff : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsActive",
                table: "Exams");

            migrationBuilder.AddColumn<Guid>(
                name: "ApplicantId",
                table: "Exams",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<int>(
                name: "CurrentQuestionIndex",
                table: "Exams",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "SelectedAnswers",
                table: "Exams",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<int>(
                name: "TotalTimeAllowedInSeconds",
                table: "Exams",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "CompletedExamTypes",
                table: "ApplicantExams",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ApplicantId",
                table: "Exams");

            migrationBuilder.DropColumn(
                name: "CurrentQuestionIndex",
                table: "Exams");

            migrationBuilder.DropColumn(
                name: "SelectedAnswers",
                table: "Exams");

            migrationBuilder.DropColumn(
                name: "TotalTimeAllowedInSeconds",
                table: "Exams");

            migrationBuilder.DropColumn(
                name: "CompletedExamTypes",
                table: "ApplicantExams");

            migrationBuilder.AddColumn<bool>(
                name: "IsActive",
                table: "Exams",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }
    }
}
