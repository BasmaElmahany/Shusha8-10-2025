using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Shusha_project_BackUp.Data.Migrations
{
    /// <inheritdoc />
    public partial class HerdSales : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "HerdSales",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    NO_wh_herd = table.Column<int>(type: "int", nullable: false),
                    weight_white = table.Column<double>(type: "float", nullable: false),
                    wh_price_kilo = table.Column<double>(type: "float", nullable: false),
                    req_total_wh = table.Column<double>(type: "float", nullable: false),
                    NO_br_herd = table.Column<int>(type: "int", nullable: false),
                    weight_brown = table.Column<double>(type: "float", nullable: false),
                    br_price_kilo = table.Column<double>(type: "float", nullable: false),
                    req_total_br = table.Column<double>(type: "float", nullable: false),
                    Date = table.Column<DateOnly>(type: "date", nullable: false),
                    total_request_proceed = table.Column<double>(type: "float", nullable: false),
                    IsPaid = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_HerdSales", x => x.id);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "HerdSales");
        }
    }
}
