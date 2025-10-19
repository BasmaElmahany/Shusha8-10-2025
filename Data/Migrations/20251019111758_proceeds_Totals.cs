using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Shusha_project_BackUp.Data.Migrations
{
    /// <inheritdoc />
    public partial class proceeds_Totals : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "proceeds_Totals",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Date = table.Column<DateOnly>(type: "date", nullable: false),
                    Egg = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    broken_Egg = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    double_Egg = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    herd = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Waste = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    waste_fees = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Miscellaneous = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    total = table.Column<decimal>(type: "decimal(18,2)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_proceeds_Totals", x => x.id);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "proceeds_Totals");
        }
    }
}
