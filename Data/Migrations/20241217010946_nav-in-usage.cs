using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Shusha_project_BackUp.Data.Migrations
{
    /// <inheritdoc />
    public partial class navinusage : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "MedicineID",
                table: "medicineUsed",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_medicineUsed_MedicineID",
                table: "medicineUsed",
                column: "MedicineID");

            migrationBuilder.AddForeignKey(
                name: "FK_medicineUsed_medicines_MedicineID",
                table: "medicineUsed",
                column: "MedicineID",
                principalTable: "medicines",
                principalColumn: "ID",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_medicineUsed_medicines_MedicineID",
                table: "medicineUsed");

            migrationBuilder.DropIndex(
                name: "IX_medicineUsed_MedicineID",
                table: "medicineUsed");

            migrationBuilder.DropColumn(
                name: "MedicineID",
                table: "medicineUsed");
        }
    }
}
