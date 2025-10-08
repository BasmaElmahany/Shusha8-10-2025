using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Shusha_project_BackUp.Data.Migrations
{
    /// <inheritdoc />
    public partial class editInvoices : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Electricity_Invoices_Branches_branch_Id",
                table: "Electricity_Invoices");

            migrationBuilder.DropIndex(
                name: "IX_Electricity_Invoices_branch_Id",
                table: "Electricity_Invoices");

            migrationBuilder.DropColumn(
                name: "branch_Id",
                table: "Electricity_Invoices");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "branch_Id",
                table: "Electricity_Invoices",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_Electricity_Invoices_branch_Id",
                table: "Electricity_Invoices",
                column: "branch_Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Electricity_Invoices_Branches_branch_Id",
                table: "Electricity_Invoices",
                column: "branch_Id",
                principalTable: "Branches",
                principalColumn: "branch_id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
