using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EmployeesManagment.Data.Migrations
{
    /// <inheritdoc />
    public partial class WorkFlowUpdate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "DepartmentId",
                table: "WorkFlowUserGroups",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "DocumentTypeId",
                table: "WorkFlowUserGroups",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_WorkFlowUserGroups_DepartmentId",
                table: "WorkFlowUserGroups",
                column: "DepartmentId");

            migrationBuilder.CreateIndex(
                name: "IX_WorkFlowUserGroups_DocumentTypeId",
                table: "WorkFlowUserGroups",
                column: "DocumentTypeId");

            migrationBuilder.AddForeignKey(
                name: "FK_WorkFlowUserGroups_Departments_DepartmentId",
                table: "WorkFlowUserGroups",
                column: "DepartmentId",
                principalTable: "Departments",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_WorkFlowUserGroups_SystemCodeDetails_DocumentTypeId",
                table: "WorkFlowUserGroups",
                column: "DocumentTypeId",
                principalTable: "SystemCodeDetails",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_WorkFlowUserGroups_Departments_DepartmentId",
                table: "WorkFlowUserGroups");

            migrationBuilder.DropForeignKey(
                name: "FK_WorkFlowUserGroups_SystemCodeDetails_DocumentTypeId",
                table: "WorkFlowUserGroups");

            migrationBuilder.DropIndex(
                name: "IX_WorkFlowUserGroups_DepartmentId",
                table: "WorkFlowUserGroups");

            migrationBuilder.DropIndex(
                name: "IX_WorkFlowUserGroups_DocumentTypeId",
                table: "WorkFlowUserGroups");

            migrationBuilder.DropColumn(
                name: "DepartmentId",
                table: "WorkFlowUserGroups");

            migrationBuilder.DropColumn(
                name: "DocumentTypeId",
                table: "WorkFlowUserGroups");
        }
    }
}
