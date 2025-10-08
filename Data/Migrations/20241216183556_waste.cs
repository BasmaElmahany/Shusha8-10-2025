using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Shusha_project_BackUp.Data.Migrations
{
    /// <inheritdoc />
    public partial class waste : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<double>(
                name: "waste_poultry",
                table: "DailySales",
                type: "float",
                nullable: true);

            migrationBuilder.AddColumn<double>(
                name: "waste_price",
                table: "DailySales",
                type: "float",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "waste_poultry",
                table: "DailySales");

            migrationBuilder.DropColumn(
                name: "waste_price",
                table: "DailySales");
        }
    }
}
