using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DotNetBrushUp.Migrations
{
    /// <inheritdoc />
    public partial class FromXenciadevice : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "EmployeeProofFileName",
                table: "EmployeeDetails",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "EmployeeProofFilePath",
                table: "EmployeeDetails",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "EmployeeProofFileName",
                table: "EmployeeDetails");

            migrationBuilder.DropColumn(
                name: "EmployeeProofFilePath",
                table: "EmployeeDetails");
        }
    }
}
