using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Shusha_project_BackUp.Data.Migrations
{
    /// <inheritdoc />
    public partial class edit_feed_dis : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Feeds_Distributions_Branches_branchId",
                table: "Feeds_Distributions");

            migrationBuilder.DropIndex(
                name: "IX_Feeds_Distributions_branchId",
                table: "Feeds_Distributions");

            migrationBuilder.DropColumn(
                name: "branchId",
                table: "Feeds_Distributions");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "branchId",
                table: "Feeds_Distributions",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_Feeds_Distributions_branchId",
                table: "Feeds_Distributions",
                column: "branchId");

            migrationBuilder.AddForeignKey(
                name: "FK_Feeds_Distributions_Branches_branchId",
                table: "Feeds_Distributions",
                column: "branchId",
                principalTable: "Branches",
                principalColumn: "branch_id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
