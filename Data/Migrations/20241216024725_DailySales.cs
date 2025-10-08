using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Shusha_project_BackUp.Data.Migrations
{
    /// <inheritdoc />
    public partial class DailySales : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "DailySales",
                columns: table => new
                {
                    dailySales_id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    wardID = table.Column<int>(type: "int", nullable: false),
                    no_of_carton_WhEggs = table.Column<int>(type: "int", nullable: false),
                    Whprice = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    no_of_carton_BrEggs = table.Column<int>(type: "int", nullable: false),
                    Brprice = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    no_of_carton_broken = table.Column<int>(type: "int", nullable: false),
                    Broprice = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    double_eggs = table.Column<int>(type: "int", nullable: false),
                    Doprice = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Date = table.Column<DateOnly>(type: "date", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DailySales", x => x.dailySales_id);
                    table.ForeignKey(
                        name: "FK_DailySales_Wards_wardID",
                        column: x => x.wardID,
                        principalTable: "Wards",
                        principalColumn: "Ward_ID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_DailySales_wardID",
                table: "DailySales",
                column: "wardID");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "DailySales");
        }
    }
}
