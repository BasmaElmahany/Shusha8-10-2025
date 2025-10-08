using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Shusha_project_BackUp.Data.Migrations
{
    /// <inheritdoc />
    public partial class inventory : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "No_of_eggs",
                table: "Inventories",
                newName: "No_of_Wheggs");

            migrationBuilder.AddColumn<DateOnly>(
                name: "Date",
                table: "Inventories",
                type: "date",
                nullable: false,
                defaultValue: new DateOnly(1, 1, 1));

            migrationBuilder.AddColumn<double>(
                name: "Feeds",
                table: "Inventories",
                type: "float",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<int>(
                name: "No_of_Breggs",
                table: "Inventories",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "No_of_Brokeneggs",
                table: "Inventories",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "No_of_Double_eggs",
                table: "Inventories",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Date",
                table: "Inventories");

            migrationBuilder.DropColumn(
                name: "Feeds",
                table: "Inventories");

            migrationBuilder.DropColumn(
                name: "No_of_Breggs",
                table: "Inventories");

            migrationBuilder.DropColumn(
                name: "No_of_Brokeneggs",
                table: "Inventories");

            migrationBuilder.DropColumn(
                name: "No_of_Double_eggs",
                table: "Inventories");

            migrationBuilder.RenameColumn(
                name: "No_of_Wheggs",
                table: "Inventories",
                newName: "No_of_eggs");
        }
    }
}
