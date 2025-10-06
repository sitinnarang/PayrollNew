using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PayrollPro.Migrations
{
    /// <inheritdoc />
    public partial class AddExtendedEmployeeFields : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Address",
                table: "AppEmployees",
                type: "nvarchar(200)",
                maxLength: 200,
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "BillableByDefault",
                table: "AppEmployees",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<decimal>(
                name: "BillingRate",
                table: "AppEmployees",
                type: "decimal(18,2)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "City",
                table: "AppEmployees",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Country",
                table: "AppEmployees",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "DateOfBirth",
                table: "AppEmployees",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "DisplayName",
                table: "AppEmployees",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "EmergencyContactName",
                table: "AppEmployees",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "EmergencyContactPhone",
                table: "AppEmployees",
                type: "nvarchar(20)",
                maxLength: 20,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Gender",
                table: "AppEmployees",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Manager",
                table: "AppEmployees",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "MobilePhone",
                table: "AppEmployees",
                type: "nvarchar(20)",
                maxLength: 20,
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "ReleaseDate",
                table: "AppEmployees",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "SocialSecurityNumber",
                table: "AppEmployees",
                type: "nvarchar(20)",
                maxLength: 20,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "State",
                table: "AppEmployees",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ZipCode",
                table: "AppEmployees",
                type: "nvarchar(20)",
                maxLength: 20,
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Address",
                table: "AppEmployees");

            migrationBuilder.DropColumn(
                name: "BillableByDefault",
                table: "AppEmployees");

            migrationBuilder.DropColumn(
                name: "BillingRate",
                table: "AppEmployees");

            migrationBuilder.DropColumn(
                name: "City",
                table: "AppEmployees");

            migrationBuilder.DropColumn(
                name: "Country",
                table: "AppEmployees");

            migrationBuilder.DropColumn(
                name: "DateOfBirth",
                table: "AppEmployees");

            migrationBuilder.DropColumn(
                name: "DisplayName",
                table: "AppEmployees");

            migrationBuilder.DropColumn(
                name: "EmergencyContactName",
                table: "AppEmployees");

            migrationBuilder.DropColumn(
                name: "EmergencyContactPhone",
                table: "AppEmployees");

            migrationBuilder.DropColumn(
                name: "Gender",
                table: "AppEmployees");

            migrationBuilder.DropColumn(
                name: "Manager",
                table: "AppEmployees");

            migrationBuilder.DropColumn(
                name: "MobilePhone",
                table: "AppEmployees");

            migrationBuilder.DropColumn(
                name: "ReleaseDate",
                table: "AppEmployees");

            migrationBuilder.DropColumn(
                name: "SocialSecurityNumber",
                table: "AppEmployees");

            migrationBuilder.DropColumn(
                name: "State",
                table: "AppEmployees");

            migrationBuilder.DropColumn(
                name: "ZipCode",
                table: "AppEmployees");
        }
    }
}
