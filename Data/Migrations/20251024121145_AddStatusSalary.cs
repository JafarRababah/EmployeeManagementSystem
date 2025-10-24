using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EmployeesManagment.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddStatusSalary : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "StatusId",
                table: "Salaries",
                type: "int",
                nullable: false,
                defaultValue: 2);

            migrationBuilder.CreateIndex(
                name: "IX_Salaries_StatusId",
                table: "Salaries",
                column: "StatusId");

            migrationBuilder.AddForeignKey(
                name: "FK_Salaries_SystemCodeDetails_StatusId",
                table: "Salaries",
                column: "StatusId",
                principalTable: "SystemCodeDetails",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Salaries_SystemCodeDetails_StatusId",
                table: "Salaries");

            migrationBuilder.DropIndex(
                name: "IX_Salaries_StatusId",
                table: "Salaries");

            migrationBuilder.DropColumn(
                name: "StatusId",
                table: "Salaries");
        }
    }
}
