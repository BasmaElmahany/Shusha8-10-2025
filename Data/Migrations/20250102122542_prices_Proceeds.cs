using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Shusha_project_BackUp.Data.Migrations
{
    /// <inheritdoc />
    public partial class prices_Proceeds : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Prices",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    white_Egg_Price = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    brown_Egg_Price = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    broken_Egg_Price = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    double_Egg_Price = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Date = table.Column<DateOnly>(type: "date", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Prices", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "Proceeds",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    centerId = table.Column<int>(type: "int", nullable: false),
                    amount = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    rest_amount = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Date = table.Column<DateOnly>(type: "date", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Proceeds", x => x.id);
                    table.ForeignKey(
                        name: "FK_Proceeds_Centers_centerId",
                        column: x => x.centerId,
                        principalTable: "Centers",
                        principalColumn: "centerId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Request_Proceeds",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    centerID = table.Column<int>(type: "int", nullable: true),
                    requested_proceeds = table.Column<decimal>(type: "decimal(18,2)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Request_Proceeds", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Request_Proceeds_Centers_centerID",
                        column: x => x.centerID,
                        principalTable: "Centers",
                        principalColumn: "centerId");
                });

            migrationBuilder.CreateIndex(
                name: "IX_Proceeds_centerId",
                table: "Proceeds",
                column: "centerId");

            migrationBuilder.CreateIndex(
                name: "IX_Request_Proceeds_centerID",
                table: "Request_Proceeds",
                column: "centerID");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Prices");

            migrationBuilder.DropTable(
                name: "Proceeds");

            migrationBuilder.DropTable(
                name: "Request_Proceeds");
        }
    }
}
