using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Shusha_project_BackUp.Data.Migrations
{
    /// <inheritdoc />
    public partial class init55 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "boughtFeedUsages",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    StockId = table.Column<int>(type: "int", nullable: false),
                    QuantityUsed = table.Column<int>(type: "int", nullable: false),
                    DateUsed = table.Column<DateTime>(type: "datetime2", nullable: false),
                    BoughtStockfeeds_Id = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_boughtFeedUsages", x => x.Id);
                    table.ForeignKey(
                        name: "FK_boughtFeedUsages_boughtStocks_BoughtStockfeeds_Id",
                        column: x => x.BoughtStockfeeds_Id,
                        principalTable: "boughtStocks",
                        principalColumn: "feeds_Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "producedFeedUsages",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    StockId = table.Column<int>(type: "int", nullable: false),
                    QuantityUsed = table.Column<int>(type: "int", nullable: false),
                    DateUsed = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ProducedStockfeeds_Id = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_producedFeedUsages", x => x.Id);
                    table.ForeignKey(
                        name: "FK_producedFeedUsages_ProducedStocks_ProducedStockfeeds_Id",
                        column: x => x.ProducedStockfeeds_Id,
                        principalTable: "ProducedStocks",
                        principalColumn: "feeds_Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_boughtFeedUsages_BoughtStockfeeds_Id",
                table: "boughtFeedUsages",
                column: "BoughtStockfeeds_Id");

            migrationBuilder.CreateIndex(
                name: "IX_producedFeedUsages_ProducedStockfeeds_Id",
                table: "producedFeedUsages",
                column: "ProducedStockfeeds_Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "boughtFeedUsages");

            migrationBuilder.DropTable(
                name: "producedFeedUsages");
        }
    }
}
