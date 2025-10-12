using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Shusha_project_BackUp.Data.Migrations
{
    /// <inheritdoc />
    public partial class wasteSalesaddsomePropertiesIntraderSales : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<decimal>(
                name: "BrownMedEggPrice",
                table: "TraderSales",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "MedDoubleEggPrice",
                table: "TraderSales",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "MedWhiteEggPrice",
                table: "TraderSales",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "NewBrownEggPrice",
                table: "TraderSales",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "NewDoubleEggPrice",
                table: "TraderSales",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "NewWhiteEggPrice",
                table: "TraderSales",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<int>(
                name: "NoOfMedBrownEggs",
                table: "TraderSales",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "NoOfMedDoubleEggs",
                table: "TraderSales",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "NoOfMedWhiteEggs",
                table: "TraderSales",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "NoOfNewBrownEggs",
                table: "TraderSales",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "NoOfNewDoubleEggs",
                table: "TraderSales",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "NoOfNewWhiteEggs",
                table: "TraderSales",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "BrownMedEggPrice",
                table: "TraderSales");

            migrationBuilder.DropColumn(
                name: "MedDoubleEggPrice",
                table: "TraderSales");

            migrationBuilder.DropColumn(
                name: "MedWhiteEggPrice",
                table: "TraderSales");

            migrationBuilder.DropColumn(
                name: "NewBrownEggPrice",
                table: "TraderSales");

            migrationBuilder.DropColumn(
                name: "NewDoubleEggPrice",
                table: "TraderSales");

            migrationBuilder.DropColumn(
                name: "NewWhiteEggPrice",
                table: "TraderSales");

            migrationBuilder.DropColumn(
                name: "NoOfMedBrownEggs",
                table: "TraderSales");

            migrationBuilder.DropColumn(
                name: "NoOfMedDoubleEggs",
                table: "TraderSales");

            migrationBuilder.DropColumn(
                name: "NoOfMedWhiteEggs",
                table: "TraderSales");

            migrationBuilder.DropColumn(
                name: "NoOfNewBrownEggs",
                table: "TraderSales");

            migrationBuilder.DropColumn(
                name: "NoOfNewDoubleEggs",
                table: "TraderSales");

            migrationBuilder.DropColumn(
                name: "NoOfNewWhiteEggs",
                table: "TraderSales");
        }
    }
}
