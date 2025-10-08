using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Shusha_project_BackUp.Data.Migrations
{
    /// <inheritdoc />
    public partial class delete : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Daily_Productions_Feedstock_Feeds_FactoryID",
                table: "Daily_Productions");

            migrationBuilder.DropTable(
                name: "Feedstock");

            migrationBuilder.DropIndex(
                name: "IX_Daily_Productions_Feeds_FactoryID",
                table: "Daily_Productions");

            migrationBuilder.DropColumn(
                name: "Feed_Inventory",
                table: "Daily_Productions");

            migrationBuilder.DropColumn(
                name: "Feeds_FactoryID",
                table: "Daily_Productions");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<double>(
                name: "Feed_Inventory",
                table: "Daily_Productions",
                type: "float",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<int>(
                name: "Feeds_FactoryID",
                table: "Daily_Productions",
                type: "int",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "Feedstock",
                columns: table => new
                {
                    ID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CostPerTon = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Quantity = table.Column<decimal>(type: "decimal(18,2)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Feedstock", x => x.ID);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Daily_Productions_Feeds_FactoryID",
                table: "Daily_Productions",
                column: "Feeds_FactoryID");

            migrationBuilder.AddForeignKey(
                name: "FK_Daily_Productions_Feedstock_Feeds_FactoryID",
                table: "Daily_Productions",
                column: "Feeds_FactoryID",
                principalTable: "Feedstock",
                principalColumn: "ID");
        }
    }
}
