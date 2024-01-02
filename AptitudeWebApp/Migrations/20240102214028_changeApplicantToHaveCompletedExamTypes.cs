using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AptitudeWebApp.Migrations
{
    /// <inheritdoc />
    public partial class changeApplicantToHaveCompletedExamTypes : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CompletedExamTypes",
                table: "ApplicantExams");

            migrationBuilder.AddColumn<string>(
                name: "CompletedExamTypes",
                table: "Applicants",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CompletedExamTypes",
                table: "Applicants");

            migrationBuilder.AddColumn<string>(
                name: "CompletedExamTypes",
                table: "ApplicantExams",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }
    }
}
