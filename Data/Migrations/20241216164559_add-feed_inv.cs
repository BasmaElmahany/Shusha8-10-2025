using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Shusha_project_BackUp.Data.Migrations
{
    /// <inheritdoc />
    public partial class addfeed_inv : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<double>(
                name: "Feed_Inventory",
                table: "Daily_Productions",
                type: "float",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<int>(
                name: "feeds_Id",
                table: "Daily_Productions",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Daily_Productions_feeds_Id",
                table: "Daily_Productions",
                column: "feeds_Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Daily_Productions_Feeds_Factory_feeds_Id",
                table: "Daily_Productions",
                column: "feeds_Id",
                principalTable: "Feeds_Factory",
                principalColumn: "feeds_Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Daily_Productions_Feeds_Factory_feeds_Id",
                table: "Daily_Productions");

            migrationBuilder.DropIndex(
                name: "IX_Daily_Productions_feeds_Id",
                table: "Daily_Productions");

            migrationBuilder.DropColumn(
                name: "Feed_Inventory",
                table: "Daily_Productions");

            migrationBuilder.DropColumn(
                name: "feeds_Id",
                table: "Daily_Productions");
        }
    }
}
