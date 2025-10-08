using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Shusha_project_BackUp.Data.Migrations
{
    /// <inheritdoc />
    public partial class init222 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Daily_Productions_Feeds_Factory_Feeds_Factoryfeeds_Id",
                table: "Daily_Productions");

            migrationBuilder.DropTable(
                name: "Feeds_Factory");

            migrationBuilder.CreateTable(
                name: "boughtStocks",
                columns: table => new
                {
                    feeds_Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Quantity = table.Column<int>(type: "int", nullable: false),
                    CostPerTon = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    TotalCost = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    DatePurchased = table.Column<DateTime>(type: "datetime2", nullable: false)
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
                    Quantity = table.Column<int>(type: "int", nullable: false),
                    CostPerTon = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    TotalCost = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    DateProduced = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProducedStocks", x => x.feeds_Id);
                });

            migrationBuilder.AddForeignKey(
                name: "FK_Daily_Productions_ProducedStocks_Feeds_Factoryfeeds_Id",
                table: "Daily_Productions",
                column: "Feeds_Factoryfeeds_Id",
                principalTable: "ProducedStocks",
                principalColumn: "feeds_Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Daily_Productions_ProducedStocks_Feeds_Factoryfeeds_Id",
                table: "Daily_Productions");

            migrationBuilder.DropTable(
                name: "boughtStocks");

            migrationBuilder.DropTable(
                name: "ProducedStocks");

            migrationBuilder.CreateTable(
                name: "Feeds_Factory",
                columns: table => new
                {
                    feeds_Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Date = table.Column<DateOnly>(type: "date", nullable: false),
                    Feed_Inventory = table.Column<double>(type: "float", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Feeds_Factory", x => x.feeds_Id);
                });

            migrationBuilder.AddForeignKey(
                name: "FK_Daily_Productions_Feeds_Factory_Feeds_Factoryfeeds_Id",
                table: "Daily_Productions",
                column: "Feeds_Factoryfeeds_Id",
                principalTable: "Feeds_Factory",
                principalColumn: "feeds_Id");
        }
    }
}
