using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Shusha_project_BackUp.Data.Migrations
{
    /// <inheritdoc />
    public partial class traderSales : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "TraderSales",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    NoOfWhiteEggs = table.Column<int>(type: "int", nullable: false),
                    WhiteEggPrice = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    NoOfBrownEggs = table.Column<int>(type: "int", nullable: false),
                    BrownEggPrice = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    NoOfBrokenEggs = table.Column<int>(type: "int", nullable: false),
                    BrokenEggPrice = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    NoOfDoubleEggs = table.Column<int>(type: "int", nullable: false),
                    DoubleEggPrice = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    IsPaid = table.Column<bool>(type: "bit", nullable: false),
                    date = table.Column<DateOnly>(type: "date", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TraderSales", x => x.Id);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "TraderSales");
        }
    }
}
