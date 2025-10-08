using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Shusha_project_BackUp.Data.Migrations
{
    /// <inheritdoc />
    public partial class feed_inv : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "Feeds_Factoryfeeds_Id",
                table: "Daily_Productions",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Daily_Productions_Feeds_Factoryfeeds_Id",
                table: "Daily_Productions",
                column: "Feeds_Factoryfeeds_Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Daily_Productions_Feeds_Factory_Feeds_Factoryfeeds_Id",
                table: "Daily_Productions",
                column: "Feeds_Factoryfeeds_Id",
                principalTable: "Feeds_Factory",
                principalColumn: "feeds_Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Daily_Productions_Feeds_Factory_Feeds_Factoryfeeds_Id",
                table: "Daily_Productions");

            migrationBuilder.DropIndex(
                name: "IX_Daily_Productions_Feeds_Factoryfeeds_Id",
                table: "Daily_Productions");

            migrationBuilder.DropColumn(
                name: "Feeds_Factoryfeeds_Id",
                table: "Daily_Productions");
        }
    }
}
