using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EmployeesManagment.Data.Migrations
{
    /// <inheritdoc />
    public partial class ApprovalUserMatrix : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ApprovalsUserMatrixes",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    DocumentTypeId = table.Column<int>(type: "int", nullable: false),
                    workFlowUserGroupId = table.Column<int>(type: "int", nullable: false),
                    Active = table.Column<bool>(type: "bit", nullable: false),
                    CreatedById = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedOn = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ModifiedById = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ModifiedOn = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ApprovalsUserMatrixes", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ApprovalsUserMatrixes_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ApprovalsUserMatrixes_SystemCodeDetails_DocumentTypeId",
                        column: x => x.DocumentTypeId,
                        principalTable: "SystemCodeDetails",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ApprovalsUserMatrixes_WorkFlowUserGroups_workFlowUserGroupId",
                        column: x => x.workFlowUserGroupId,
                        principalTable: "WorkFlowUserGroups",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ApprovalsUserMatrixes_DocumentTypeId",
                table: "ApprovalsUserMatrixes",
                column: "DocumentTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_ApprovalsUserMatrixes_UserId",
                table: "ApprovalsUserMatrixes",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_ApprovalsUserMatrixes_workFlowUserGroupId",
                table: "ApprovalsUserMatrixes",
                column: "workFlowUserGroupId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ApprovalsUserMatrixes");
        }
    }
}
