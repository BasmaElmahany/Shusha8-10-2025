using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Shusha_project_BackUp.Data.Migrations
{
    /// <inheritdoc />
    public partial class Create : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Branches",
                columns: table => new
                {
                    branch_id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    branch_name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    total_No_of_herd = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Branches", x => x.branch_id);
                });

            migrationBuilder.CreateTable(
                name: "Centers",
                columns: table => new
                {
                    centerId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    centerName = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Centers", x => x.centerId);
                });

            migrationBuilder.CreateTable(
                name: "Employees",
                columns: table => new
                {
                    emp_id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    emp_name = table.Column<int>(type: "int", nullable: false),
                    NID = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    phoneNumber = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Employees", x => x.emp_id);
                });

            migrationBuilder.CreateTable(
                name: "Inventories",
                columns: table => new
                {
                    Inv_Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    No_of_eggs = table.Column<int>(type: "int", nullable: false),
                    Hens_waste = table.Column<double>(type: "float", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Inventories", x => x.Inv_Id);
                });

            migrationBuilder.CreateTable(
                name: "sales",
                columns: table => new
                {
                    sales_ID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    no_ofEggs = table.Column<int>(type: "int", nullable: false),
                    Egg_Carton = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Hens_waste = table.Column<double>(type: "float", nullable: false),
                    waste_price = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    no_of_hens = table.Column<int>(type: "int", nullable: false),
                    hen_price = table.Column<decimal>(type: "decimal(18,2)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_sales", x => x.sales_ID);
                });

            migrationBuilder.CreateTable(
                name: "Solar_Invoices",
                columns: table => new
                {
                    invoice_id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    amount = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Date = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Solar_Invoices", x => x.invoice_id);
                });

            migrationBuilder.CreateTable(
                name: "Electricity_Invoices",
                columns: table => new
                {
                    ID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Date = table.Column<DateTime>(type: "datetime2", nullable: false),
                    amount = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Invoice_photo = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    branch_Id = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Electricity_Invoices", x => x.ID);
                    table.ForeignKey(
                        name: "FK_Electricity_Invoices_Branches_branch_Id",
                        column: x => x.branch_Id,
                        principalTable: "Branches",
                        principalColumn: "branch_id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Wards",
                columns: table => new
                {
                    Ward_ID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    WardName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    No_of_herd = table.Column<int>(type: "int", nullable: false),
                    branchId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Wards", x => x.Ward_ID);
                    table.ForeignKey(
                        name: "FK_Wards_Branches_branchId",
                        column: x => x.branchId,
                        principalTable: "Branches",
                        principalColumn: "branch_id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Contracts",
                columns: table => new
                {
                    contract_id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    contract_description = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    emp_id = table.Column<int>(type: "int", nullable: false),
                    Gross_salary = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Medical_Assurance = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    taxes = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Date = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Contracts", x => x.contract_id);
                    table.ForeignKey(
                        name: "FK_Contracts_Employees_emp_id",
                        column: x => x.emp_id,
                        principalTable: "Employees",
                        principalColumn: "emp_id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Daily_Productions",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ward_id = table.Column<int>(type: "int", nullable: true),
                    No_of_Wheggs = table.Column<int>(type: "int", nullable: false),
                    No_of_Breggs = table.Column<int>(type: "int", nullable: false),
                    Whdead_herd = table.Column<int>(type: "int", nullable: false),
                    Brdead_herd = table.Column<int>(type: "int", nullable: false),
                    Date = table.Column<DateOnly>(type: "date", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Daily_Productions", x => x.id);
                    table.ForeignKey(
                        name: "FK_Daily_Productions_Wards_ward_id",
                        column: x => x.ward_id,
                        principalTable: "Wards",
                        principalColumn: "Ward_ID");
                });

            migrationBuilder.CreateTable(
                name: "Feeds_Distributions",
                columns: table => new
                {
                    dis_Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    no_of_tones = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    branchId = table.Column<int>(type: "int", nullable: false),
                    WardID = table.Column<int>(type: "int", nullable: false),
                    Date = table.Column<DateOnly>(type: "date", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Feeds_Distributions", x => x.dis_Id);
                    table.ForeignKey(
                        name: "FK_Feeds_Distributions_Branches_branchId",
                        column: x => x.branchId,
                        principalTable: "Branches",
                        principalColumn: "branch_id",
                        onDelete: ReferentialAction.NoAction);
                    table.ForeignKey(
                        name: "FK_Feeds_Distributions_Wards_WardID",
                        column: x => x.WardID,
                        principalTable: "Wards",
                        principalColumn: "Ward_ID",
                        onDelete: ReferentialAction.NoAction);
                });

            migrationBuilder.CreateTable(
                name: "Feeds_Factory",
                columns: table => new
                {
                    feeds_Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Feed_Inventory = table.Column<double>(type: "float", nullable: false),
                    Date = table.Column<DateTime>(type: "datetime2", nullable: false),
                    WardID = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Feeds_Factory", x => x.feeds_Id);
                    table.ForeignKey(
                        name: "FK_Feeds_Factory_Wards_WardID",
                        column: x => x.WardID,
                        principalTable: "Wards",
                        principalColumn: "Ward_ID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Daily_Distributions",
                columns: table => new
                {
                    dis_id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    centerId = table.Column<int>(type: "int", nullable: false),
                    productionId = table.Column<int>(type: "int", nullable: false),
                    Egg_number = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Daily_Distributions", x => x.dis_id);
                    table.ForeignKey(
                        name: "FK_Daily_Distributions_Centers_centerId",
                        column: x => x.centerId,
                        principalTable: "Centers",
                        principalColumn: "centerId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Daily_Distributions_Daily_Productions_productionId",
                        column: x => x.productionId,
                        principalTable: "Daily_Productions",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Contracts_emp_id",
                table: "Contracts",
                column: "emp_id",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Daily_Distributions_centerId",
                table: "Daily_Distributions",
                column: "centerId");

            migrationBuilder.CreateIndex(
                name: "IX_Daily_Distributions_productionId",
                table: "Daily_Distributions",
                column: "productionId");

            migrationBuilder.CreateIndex(
                name: "IX_Daily_Productions_ward_id",
                table: "Daily_Productions",
                column: "ward_id");

            migrationBuilder.CreateIndex(
                name: "IX_Electricity_Invoices_branch_Id",
                table: "Electricity_Invoices",
                column: "branch_Id");

            migrationBuilder.CreateIndex(
                name: "IX_Feeds_Distributions_branchId",
                table: "Feeds_Distributions",
                column: "branchId");

            migrationBuilder.CreateIndex(
                name: "IX_Feeds_Distributions_WardID",
                table: "Feeds_Distributions",
                column: "WardID");

            migrationBuilder.CreateIndex(
                name: "IX_Feeds_Factory_WardID",
                table: "Feeds_Factory",
                column: "WardID");

            migrationBuilder.CreateIndex(
                name: "IX_Wards_branchId",
                table: "Wards",
                column: "branchId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Contracts");

            migrationBuilder.DropTable(
                name: "Daily_Distributions");

            migrationBuilder.DropTable(
                name: "Electricity_Invoices");

            migrationBuilder.DropTable(
                name: "Feeds_Distributions");

            migrationBuilder.DropTable(
                name: "Feeds_Factory");

            migrationBuilder.DropTable(
                name: "Inventories");

            migrationBuilder.DropTable(
                name: "sales");

            migrationBuilder.DropTable(
                name: "Solar_Invoices");

            migrationBuilder.DropTable(
                name: "Employees");

            migrationBuilder.DropTable(
                name: "Centers");

            migrationBuilder.DropTable(
                name: "Daily_Productions");

            migrationBuilder.DropTable(
                name: "Wards");

            migrationBuilder.DropTable(
                name: "Branches");
        }
    }
}
