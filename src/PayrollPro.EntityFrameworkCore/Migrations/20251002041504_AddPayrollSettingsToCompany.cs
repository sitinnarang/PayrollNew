using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PayrollPro.Migrations
{
    /// <inheritdoc />
    public partial class AddPayrollSettingsToCompany : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "AutoProcessPayroll",
                table: "AppCompanies",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<decimal>(
                name: "OvertimeRate",
                table: "AppCompanies",
                type: "decimal(3,1)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<int>(
                name: "PayFrequency",
                table: "AppCompanies",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<DateTime>(
                name: "PayPeriodEnd",
                table: "AppCompanies",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "StandardWorkHours",
                table: "AppCompanies",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AutoProcessPayroll",
                table: "AppCompanies");

            migrationBuilder.DropColumn(
                name: "OvertimeRate",
                table: "AppCompanies");

            migrationBuilder.DropColumn(
                name: "PayFrequency",
                table: "AppCompanies");

            migrationBuilder.DropColumn(
                name: "PayPeriodEnd",
                table: "AppCompanies");

            migrationBuilder.DropColumn(
                name: "StandardWorkHours",
                table: "AppCompanies");
        }
    }
}
