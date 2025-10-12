using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Shusha_project_BackUp.Data.Migrations
{
    /// <inheritdoc />
    public partial class Waste_Sales : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Waste_Sales",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    wasteMeters = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    price = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    TraderName = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Waste_Sales", x => x.Id);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Waste_Sales");
        }
    }
}
