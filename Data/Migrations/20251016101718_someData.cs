using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Shusha_project_BackUp.Data.Migrations
{
    /// <inheritdoc />
    public partial class someData : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "branchId",
                table: "Waste_Sales",
                type: "int",
                nullable: true,
                defaultValue: 0);

            migrationBuilder.AddColumn<decimal>(
                name: "wasteFees",
                table: "Waste_Sales",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<DateOnly>(
                name: "Date_of_payment",
                table: "TraderSales",
                type: "date",
                nullable: false,
                defaultValue: new DateOnly(1, 1, 1));

            migrationBuilder.CreateIndex(
                name: "IX_Waste_Sales_branchId",
                table: "Waste_Sales",
                column: "branchId");

            migrationBuilder.AddForeignKey(
                name: "FK_Waste_Sales_Branches_branchId",
                table: "Waste_Sales",
                column: "branchId",
                principalTable: "Branches",
                principalColumn: "branch_id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Waste_Sales_Branches_branchId",
                table: "Waste_Sales");

            migrationBuilder.DropIndex(
                name: "IX_Waste_Sales_branchId",
                table: "Waste_Sales");

            migrationBuilder.DropColumn(
                name: "branchId",
                table: "Waste_Sales");

            migrationBuilder.DropColumn(
                name: "wasteFees",
                table: "Waste_Sales");

            migrationBuilder.DropColumn(
                name: "Date_of_payment",
                table: "TraderSales");
        }
    }
}
