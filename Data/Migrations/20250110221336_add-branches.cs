using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Shusha_project_BackUp.Data.Migrations
{
    /// <inheritdoc />
    public partial class addbranches : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "BranchID",
                table: "Water_Invoices",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "BranchID",
                table: "Solar_Invoices",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "WardID",
                table: "medicineUsed",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "BranchID",
                table: "Electricity_Invoices",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_Water_Invoices_BranchID",
                table: "Water_Invoices",
                column: "BranchID");

            migrationBuilder.CreateIndex(
                name: "IX_Solar_Invoices_BranchID",
                table: "Solar_Invoices",
                column: "BranchID");

            migrationBuilder.CreateIndex(
                name: "IX_medicineUsed_WardID",
                table: "medicineUsed",
                column: "WardID");

            migrationBuilder.CreateIndex(
                name: "IX_Electricity_Invoices_BranchID",
                table: "Electricity_Invoices",
                column: "BranchID");

            migrationBuilder.AddForeignKey(
                name: "FK_Electricity_Invoices_Branches_BranchID",
                table: "Electricity_Invoices",
                column: "BranchID",
                principalTable: "Branches",
                principalColumn: "branch_id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_medicineUsed_Wards_WardID",
                table: "medicineUsed",
                column: "WardID",
                principalTable: "Wards",
                principalColumn: "Ward_ID",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Solar_Invoices_Branches_BranchID",
                table: "Solar_Invoices",
                column: "BranchID",
                principalTable: "Branches",
                principalColumn: "branch_id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Water_Invoices_Branches_BranchID",
                table: "Water_Invoices",
                column: "BranchID",
                principalTable: "Branches",
                principalColumn: "branch_id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Electricity_Invoices_Branches_BranchID",
                table: "Electricity_Invoices");

            migrationBuilder.DropForeignKey(
                name: "FK_medicineUsed_Wards_WardID",
                table: "medicineUsed");

            migrationBuilder.DropForeignKey(
                name: "FK_Solar_Invoices_Branches_BranchID",
                table: "Solar_Invoices");

            migrationBuilder.DropForeignKey(
                name: "FK_Water_Invoices_Branches_BranchID",
                table: "Water_Invoices");

            migrationBuilder.DropIndex(
                name: "IX_Water_Invoices_BranchID",
                table: "Water_Invoices");

            migrationBuilder.DropIndex(
                name: "IX_Solar_Invoices_BranchID",
                table: "Solar_Invoices");

            migrationBuilder.DropIndex(
                name: "IX_medicineUsed_WardID",
                table: "medicineUsed");

            migrationBuilder.DropIndex(
                name: "IX_Electricity_Invoices_BranchID",
                table: "Electricity_Invoices");

            migrationBuilder.DropColumn(
                name: "BranchID",
                table: "Water_Invoices");

            migrationBuilder.DropColumn(
                name: "BranchID",
                table: "Solar_Invoices");

            migrationBuilder.DropColumn(
                name: "WardID",
                table: "medicineUsed");

            migrationBuilder.DropColumn(
                name: "BranchID",
                table: "Electricity_Invoices");
        }
    }
}
