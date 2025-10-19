using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Shusha_project_BackUp.Data.Migrations
{
    /// <inheritdoc />
    public partial class yearBudget : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Waste_Sales_Branches_branchId",
                table: "Waste_Sales");

            migrationBuilder.AlterColumn<int>(
                name: "branchId",
                table: "Waste_Sales",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.CreateTable(
                name: "Budget",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    year = table.Column<int>(type: "int", nullable: false),
                    egg = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    waste = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    herd = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    total = table.Column<decimal>(type: "decimal(18,2)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Budget", x => x.id);
                });

            migrationBuilder.AddForeignKey(
                name: "FK_Waste_Sales_Branches_branchId",
                table: "Waste_Sales",
                column: "branchId",
                principalTable: "Branches",
                principalColumn: "branch_id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Waste_Sales_Branches_branchId",
                table: "Waste_Sales");

            migrationBuilder.DropTable(
                name: "Budget");

            migrationBuilder.AlterColumn<int>(
                name: "branchId",
                table: "Waste_Sales",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Waste_Sales_Branches_branchId",
                table: "Waste_Sales",
                column: "branchId",
                principalTable: "Branches",
                principalColumn: "branch_id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
