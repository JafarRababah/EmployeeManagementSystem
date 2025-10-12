using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EmployeesManagment.Data.Migrations
{
    /// <inheritdoc />
    public partial class IsFullAttendanceColumn : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsFullAttendance",
                table: "Payrolls",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsFullAttendance",
                table: "Payrolls");
        }
    }
}
