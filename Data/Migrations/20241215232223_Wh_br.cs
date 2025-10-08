using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Shusha_project_BackUp.Data.Migrations
{
    /// <inheritdoc />
    public partial class Wh_br : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "total_No_of_herd",
                table: "Branches");

            migrationBuilder.RenameColumn(
                name: "No_of_herd",
                table: "Wards",
                newName: "No_of_Whherd");

            migrationBuilder.AddColumn<int>(
                name: "No_of_Brherd",
                table: "Wards",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "No_of_Brherd",
                table: "Wards");

            migrationBuilder.RenameColumn(
                name: "No_of_Whherd",
                table: "Wards",
                newName: "No_of_herd");

            migrationBuilder.AddColumn<int>(
                name: "total_No_of_herd",
                table: "Branches",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }
    }
}
