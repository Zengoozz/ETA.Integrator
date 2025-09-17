using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ETA.Integrator.Server.Migrations
{
    /// <inheritdoc />
    public partial class uuidColumnToInvoiceSubmissionLogTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Uuid",
                table: "InvoiceSubmissionLogs",
                type: "TEXT",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Uuid",
                table: "InvoiceSubmissionLogs");
        }
    }
}
