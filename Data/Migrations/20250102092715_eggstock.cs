using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Shusha_project_BackUp.Data.Migrations
{
    /// <inheritdoc />
    public partial class eggstock : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Daily_Distributions_Centers_centerId",
                table: "Daily_Distributions");

            migrationBuilder.DropForeignKey(
                name: "FK_Daily_Distributions_Daily_Productions_productionId",
                table: "Daily_Distributions");

            migrationBuilder.DropIndex(
                name: "IX_Daily_Distributions_productionId",
                table: "Daily_Distributions");

            migrationBuilder.RenameColumn(
                name: "productionId",
                table: "Daily_Distributions",
                newName: "brEgg_platesnumber");

            migrationBuilder.RenameColumn(
                name: "Egg_number",
                table: "Daily_Distributions",
                newName: "WhEgg_platesnumber");

            migrationBuilder.AlterColumn<int>(
                name: "centerId",
                table: "Daily_Distributions",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AddColumn<decimal>(
                name: "BrokenEgg_platesnumber",
                table: "Daily_Distributions",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<DateTime>(
                name: "Date",
                table: "Daily_Distributions",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<decimal>(
                name: "doubleEgg_platesnumber",
                table: "Daily_Distributions",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.CreateTable(
                name: "Egg_stock",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    plates_whiteEggs = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    plates_BrownEggs = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    brokenPlates = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    doubleEggs = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    total = table.Column<decimal>(type: "decimal(18,2)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Egg_stock", x => x.Id);
                });

            migrationBuilder.AddForeignKey(
                name: "FK_Daily_Distributions_Centers_centerId",
                table: "Daily_Distributions",
                column: "centerId",
                principalTable: "Centers",
                principalColumn: "centerId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Daily_Distributions_Centers_centerId",
                table: "Daily_Distributions");

            migrationBuilder.DropTable(
                name: "Egg_stock");

            migrationBuilder.DropColumn(
                name: "BrokenEgg_platesnumber",
                table: "Daily_Distributions");

            migrationBuilder.DropColumn(
                name: "Date",
                table: "Daily_Distributions");

            migrationBuilder.DropColumn(
                name: "doubleEgg_platesnumber",
                table: "Daily_Distributions");

            migrationBuilder.RenameColumn(
                name: "brEgg_platesnumber",
                table: "Daily_Distributions",
                newName: "productionId");

            migrationBuilder.RenameColumn(
                name: "WhEgg_platesnumber",
                table: "Daily_Distributions",
                newName: "Egg_number");

            migrationBuilder.AlterColumn<int>(
                name: "centerId",
                table: "Daily_Distributions",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Daily_Distributions_productionId",
                table: "Daily_Distributions",
                column: "productionId");

            migrationBuilder.AddForeignKey(
                name: "FK_Daily_Distributions_Centers_centerId",
                table: "Daily_Distributions",
                column: "centerId",
                principalTable: "Centers",
                principalColumn: "centerId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Daily_Distributions_Daily_Productions_productionId",
                table: "Daily_Distributions",
                column: "productionId",
                principalTable: "Daily_Productions",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
