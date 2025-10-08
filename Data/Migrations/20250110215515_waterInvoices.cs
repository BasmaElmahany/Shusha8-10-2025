using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Shusha_project_BackUp.Data.Migrations
{
    /// <inheritdoc />
    public partial class waterInvoices : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Broprice",
                table: "DailySales");

            migrationBuilder.DropColumn(
                name: "Brprice",
                table: "DailySales");

            migrationBuilder.DropColumn(
                name: "Doprice",
                table: "DailySales");

            migrationBuilder.DropColumn(
                name: "Total",
                table: "DailySales");

            migrationBuilder.DropColumn(
                name: "Whprice",
                table: "DailySales");

            migrationBuilder.DropColumn(
                name: "waste_price",
                table: "DailySales");

           

            migrationBuilder.CreateTable(
                name: "wardsStocks",
                columns: table => new
                {
                    stockID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    whiteEggs = table.Column<int>(type: "int", nullable: false),
                    rest_whEggs = table.Column<int>(type: "int", nullable: false),
                    brownEggs = table.Column<int>(type: "int", nullable: false),
                    rest_brEggs = table.Column<int>(type: "int", nullable: false),
                    brokenEggs = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    rest_bkEggs = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    doubleEggs = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    rest_dbEggs = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    wardID = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_wardsStocks", x => x.stockID);
                    table.ForeignKey(
                        name: "FK_wardsStocks_Wards_wardID",
                        column: x => x.wardID,
                        principalTable: "Wards",
                        principalColumn: "Ward_ID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Water_Invoices",
                columns: table => new
                {
                    ID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Date = table.Column<DateTime>(type: "datetime2", nullable: false),
                    amount = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Invoice_photo = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Water_Invoices", x => x.ID);
                });

            migrationBuilder.CreateIndex(
                name: "IX_wardsStocks_wardID",
                table: "wardsStocks",
                column: "wardID");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "wardsStocks");

            migrationBuilder.DropTable(
                name: "Water_Invoices");

            migrationBuilder.AddColumn<decimal>(
                name: "Broprice",
                table: "DailySales",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "Brprice",
                table: "DailySales",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "Doprice",
                table: "DailySales",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<double>(
                name: "Total",
                table: "DailySales",
                type: "float",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<decimal>(
                name: "Whprice",
                table: "DailySales",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "waste_price",
                table: "DailySales",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);

           
        }
    }
}
