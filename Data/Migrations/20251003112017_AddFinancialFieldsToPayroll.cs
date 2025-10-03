using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EmployeesManagment.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddFinancialFieldsToPayroll : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Payrolls_Employees_EmployeeId1",
                table: "Payrolls");

            migrationBuilder.DropIndex(
                name: "IX_Payrolls_EmployeeId1",
                table: "Payrolls");

            migrationBuilder.DropColumn(
                name: "EmployeeId1",
                table: "Payrolls");

            migrationBuilder.AlterColumn<int>(
                name: "PayrollId",
                table: "Payrolls",
                type: "int",
                nullable: false,
                oldClrType: typeof(long),
                oldType: "bigint")
                .Annotation("SqlServer:Identity", "1, 1")
                .OldAnnotation("SqlServer:Identity", "1, 1");

            migrationBuilder.AddColumn<decimal>(
                name: "Bonus",
                table: "Payrolls",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "Penalty",
                table: "Payrolls",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "SocialSecurity",
                table: "Payrolls",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "Tax",
                table: "Payrolls",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Bonus",
                table: "Payrolls");

            migrationBuilder.DropColumn(
                name: "Penalty",
                table: "Payrolls");

            migrationBuilder.DropColumn(
                name: "SocialSecurity",
                table: "Payrolls");

            migrationBuilder.DropColumn(
                name: "Tax",
                table: "Payrolls");

            migrationBuilder.AlterColumn<long>(
                name: "PayrollId",
                table: "Payrolls",
                type: "bigint",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int")
                .Annotation("SqlServer:Identity", "1, 1")
                .OldAnnotation("SqlServer:Identity", "1, 1");

            migrationBuilder.AddColumn<int>(
                name: "EmployeeId1",
                table: "Payrolls",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Payrolls_EmployeeId1",
                table: "Payrolls",
                column: "EmployeeId1");

            migrationBuilder.AddForeignKey(
                name: "FK_Payrolls_Employees_EmployeeId1",
                table: "Payrolls",
                column: "EmployeeId1",
                principalTable: "Employees",
                principalColumn: "Id");
        }
    }
}
