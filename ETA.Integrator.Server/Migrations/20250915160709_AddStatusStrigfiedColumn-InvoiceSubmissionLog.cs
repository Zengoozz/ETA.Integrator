using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ETA.Integrator.Server.Migrations
{
    /// <inheritdoc />
    public partial class AddStatusStrigfiedColumnInvoiceSubmissionLog : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "StatusStringfied",
                table: "InvoiceSubmissionLogs",
                type: "TEXT",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "StatusStringfied",
                table: "InvoiceSubmissionLogs");
        }
    }
}
