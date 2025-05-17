using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Projekt_2.Migrations
{
    /// <inheritdoc />
    public partial class TestAnswersNumerations : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "Numeration",
                table: "TestAnswers",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Numeration",
                table: "TestAnswers");
        }
    }
}
