using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EmployeesManagment.Data.Migrations
{
    /// <inheritdoc />
    public partial class EmployeeNationalId : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "KRAPIN",
                table: "Employees",
                newName: "NationalId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "NationalId",
                table: "Employees",
                newName: "KRAPIN");
        }
    }
}
