using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Shusha_project_BackUp.Data.Migrations
{
    /// <inheritdoc />
    public partial class init66 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Daily_Productions_ProducedStocks_Feeds_Factoryfeeds_Id",
                table: "Daily_Productions");

            migrationBuilder.DropTable(
                name: "boughtFeedUsages");

            migrationBuilder.DropTable(
                name: "producedFeedUsages");

            migrationBuilder.DropTable(
                name: "boughtStocks");

            migrationBuilder.DropTable(
                name: "ProducedStocks");

            migrationBuilder.RenameColumn(
                name: "Feeds_Factoryfeeds_Id",
                table: "Daily_Productions",
                newName: "Feeds_FactoryID");

            migrationBuilder.RenameIndex(
                name: "IX_Daily_Productions_Feeds_Factoryfeeds_Id",
                table: "Daily_Productions",
                newName: "IX_Daily_Productions_Feeds_FactoryID");

            migrationBuilder.CreateTable(
                name: "Feedstocks",
                columns: table => new
                {
                    ID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Quantity = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    CostPerTon = table.Column<decimal>(type: "decimal(18,2)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Feedstocks", x => x.ID);
                });

            migrationBuilder.CreateTable(
                name: "FeedstockTransactions",
                columns: table => new
                {
                    ID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    FeedstockID = table.Column<int>(type: "int", nullable: false),
                    Date = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Type = table.Column<int>(type: "int", nullable: false),
                    Quantity = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    CostPerTon = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Purpose = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FeedstockTransactions", x => x.ID);
                    table.ForeignKey(
                        name: "FK_FeedstockTransactions_Feedstocks_FeedstockID",
                        column: x => x.FeedstockID,
                        principalTable: "Feedstocks",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_FeedstockTransactions_FeedstockID",
                table: "FeedstockTransactions",
                column: "FeedstockID");

            migrationBuilder.AddForeignKey(
                name: "FK_Daily_Productions_Feedstocks_Feeds_FactoryID",
                table: "Daily_Productions",
                column: "Feeds_FactoryID",
                principalTable: "Feedstocks",
                principalColumn: "ID");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Daily_Productions_Feedstocks_Feeds_FactoryID",
                table: "Daily_Productions");

            migrationBuilder.DropTable(
                name: "FeedstockTransactions");

            migrationBuilder.DropTable(
                name: "Feedstocks");

            migrationBuilder.RenameColumn(
                name: "Feeds_FactoryID",
                table: "Daily_Productions",
                newName: "Feeds_Factoryfeeds_Id");

            migrationBuilder.RenameIndex(
                name: "IX_Daily_Productions_Feeds_FactoryID",
                table: "Daily_Productions",
                newName: "IX_Daily_Productions_Feeds_Factoryfeeds_Id");

            migrationBuilder.CreateTable(
                name: "boughtStocks",
                columns: table => new
                {
                    feeds_Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CostPerTon = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    DatePurchased = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Quantity = table.Column<int>(type: "int", nullable: false),
                    TotalCost = table.Column<decimal>(type: "decimal(18,2)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_boughtStocks", x => x.feeds_Id);
                });

            migrationBuilder.CreateTable(
                name: "ProducedStocks",
                columns: table => new
                {
                    feeds_Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CostPerTon = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    DateProduced = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Quantity = table.Column<int>(type: "int", nullable: false),
                    TotalCost = table.Column<decimal>(type: "decimal(18,2)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProducedStocks", x => x.feeds_Id);
                });

            migrationBuilder.CreateTable(
                name: "boughtFeedUsages",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    BoughtStockfeeds_Id = table.Column<int>(type: "int", nullable: false),
                    DateUsed = table.Column<DateTime>(type: "datetime2", nullable: false),
                    QuantityUsed = table.Column<int>(type: "int", nullable: false),
                    StockId = table.Column<int>(type: "int", nullable: false)
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
                    ProducedStockfeeds_Id = table.Column<int>(type: "int", nullable: false),
                    DateUsed = table.Column<DateTime>(type: "datetime2", nullable: false),
                    QuantityUsed = table.Column<int>(type: "int", nullable: false),
                    StockId = table.Column<int>(type: "int", nullable: false)
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

            migrationBuilder.AddForeignKey(
                name: "FK_Daily_Productions_ProducedStocks_Feeds_Factoryfeeds_Id",
                table: "Daily_Productions",
                column: "Feeds_Factoryfeeds_Id",
                principalTable: "ProducedStocks",
                principalColumn: "feeds_Id");
        }
    }
}
