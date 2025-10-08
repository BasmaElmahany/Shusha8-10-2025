using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Shusha_project_BackUp.Data.Migrations
{
    /// <inheritdoc />
    public partial class editFinal : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Daily_Productions_Feedstocks_Feeds_FactoryID",
                table: "Daily_Productions");

            migrationBuilder.DropTable(
                name: "FeedstockTransactions");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Feedstocks",
                table: "Feedstocks");

            migrationBuilder.RenameTable(
                name: "Feedstocks",
                newName: "Feedstock");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Feedstock",
                table: "Feedstock",
                column: "ID");

            migrationBuilder.CreateTable(
                name: "feed_Usages",
                columns: table => new
                {
                    ID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Qty = table.Column<int>(type: "int", nullable: false),
                    Date = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_feed_Usages", x => x.ID);
                });

            migrationBuilder.CreateTable(
                name: "feedInventories",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Tons_qty = table.Column<int>(type: "int", nullable: false),
                    ton_cost = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Date = table.Column<DateTime>(type: "datetime2", nullable: false),
                    total = table.Column<decimal>(type: "decimal(18,2)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_feedInventories", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "FeedPurchases",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Tons_qty = table.Column<int>(type: "int", nullable: false),
                    price = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Date = table.Column<DateTime>(type: "datetime2", nullable: false),
                    total = table.Column<decimal>(type: "decimal(18,2)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FeedPurchases", x => x.Id);
                });

            migrationBuilder.AddForeignKey(
                name: "FK_Daily_Productions_Feedstock_Feeds_FactoryID",
                table: "Daily_Productions",
                column: "Feeds_FactoryID",
                principalTable: "Feedstock",
                principalColumn: "ID");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Daily_Productions_Feedstock_Feeds_FactoryID",
                table: "Daily_Productions");

            migrationBuilder.DropTable(
                name: "feed_Usages");

            migrationBuilder.DropTable(
                name: "feedInventories");

            migrationBuilder.DropTable(
                name: "FeedPurchases");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Feedstock",
                table: "Feedstock");

            migrationBuilder.RenameTable(
                name: "Feedstock",
                newName: "Feedstocks");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Feedstocks",
                table: "Feedstocks",
                column: "ID");

            migrationBuilder.CreateTable(
                name: "FeedstockTransactions",
                columns: table => new
                {
                    ID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    FeedstockID = table.Column<int>(type: "int", nullable: false),
                    CostPerTon = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Date = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Purpose = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Quantity = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Type = table.Column<int>(type: "int", nullable: false)
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
    }
}
