using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EmployeesManagment.Data.Migrations
{
    /// <inheritdoc />
    public partial class PayrollToEmployeeRelation : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Payrolls_AspNetUsers_UserId",
                table: "Payrolls");

            migrationBuilder.DropIndex(
                name: "IX_Payrolls_UserId",
                table: "Payrolls");

            migrationBuilder.DropColumn(
                name: "UserId",
                table: "Payrolls");

            migrationBuilder.AddColumn<int>(
                name: "EmployeeId",
                table: "Payrolls",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "EmployeeId1",
                table: "Payrolls",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Payrolls_EmployeeId",
                table: "Payrolls",
                column: "EmployeeId");

            migrationBuilder.CreateIndex(
                name: "IX_Payrolls_EmployeeId1",
                table: "Payrolls",
                column: "EmployeeId1");

            migrationBuilder.AddForeignKey(
                name: "FK_Payrolls_Employees_EmployeeId",
                table: "Payrolls",
                column: "EmployeeId",
                principalTable: "Employees",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Payrolls_Employees_EmployeeId1",
                table: "Payrolls",
                column: "EmployeeId1",
                principalTable: "Employees",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Payrolls_Employees_EmployeeId",
                table: "Payrolls");

            migrationBuilder.DropForeignKey(
                name: "FK_Payrolls_Employees_EmployeeId1",
                table: "Payrolls");

            migrationBuilder.DropIndex(
                name: "IX_Payrolls_EmployeeId",
                table: "Payrolls");

            migrationBuilder.DropIndex(
                name: "IX_Payrolls_EmployeeId1",
                table: "Payrolls");

            migrationBuilder.DropColumn(
                name: "EmployeeId",
                table: "Payrolls");

            migrationBuilder.DropColumn(
                name: "EmployeeId1",
                table: "Payrolls");

            migrationBuilder.AddColumn<string>(
                name: "UserId",
                table: "Payrolls",
                type: "nvarchar(450)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateIndex(
                name: "IX_Payrolls_UserId",
                table: "Payrolls",
                column: "UserId");

            migrationBuilder.AddForeignKey(
                name: "FK_Payrolls_AspNetUsers_UserId",
                table: "Payrolls",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
