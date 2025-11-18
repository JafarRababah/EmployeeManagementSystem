using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EmployeesManagment.Data.Migrations
{
    /// <inheritdoc />
    public partial class PaymenRecodsUserActivity : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "UpdatedAt",
                table: "PaymentRecords");

            migrationBuilder.RenameColumn(
                name: "CreatedAt",
                table: "PaymentRecords",
                newName: "ModifiedOn");

            migrationBuilder.AddColumn<string>(
                name: "CreatedById",
                table: "PaymentRecords",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedOn",
                table: "PaymentRecords",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<string>(
                name: "ModifiedById",
                table: "PaymentRecords",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CreatedById",
                table: "PaymentRecords");

            migrationBuilder.DropColumn(
                name: "CreatedOn",
                table: "PaymentRecords");

            migrationBuilder.DropColumn(
                name: "ModifiedById",
                table: "PaymentRecords");

            migrationBuilder.RenameColumn(
                name: "ModifiedOn",
                table: "PaymentRecords",
                newName: "CreatedAt");

            migrationBuilder.AddColumn<DateTime>(
                name: "UpdatedAt",
                table: "PaymentRecords",
                type: "datetime2",
                nullable: true);
        }
    }
}
