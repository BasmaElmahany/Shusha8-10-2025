using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Shusha_project_BackUp.Data.Migrations
{
    /// <inheritdoc />
    public partial class del_feed_ward : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Feeds_Factory_Wards_WardID",
                table: "Feeds_Factory");

            migrationBuilder.DropIndex(
                name: "IX_Feeds_Factory_WardID",
                table: "Feeds_Factory");

            migrationBuilder.DropColumn(
                name: "WardID",
                table: "Feeds_Factory");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "WardID",
                table: "Feeds_Factory",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_Feeds_Factory_WardID",
                table: "Feeds_Factory",
                column: "WardID");

            migrationBuilder.AddForeignKey(
                name: "FK_Feeds_Factory_Wards_WardID",
                table: "Feeds_Factory",
                column: "WardID",
                principalTable: "Wards",
                principalColumn: "Ward_ID",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
