using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace ETA.Integrator.Server.Migrations
{
    /// <inheritdoc />
    public partial class InsertingConstantsSettingsSteps : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "SettingsSteps",
                columns: new[] { "Order", "Data", "Name" },
                values: new object[,]
                {
                    { 1, null, "connection-settings" },
                    { 2, null, "issuer-settings" }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "SettingsSteps",
                keyColumn: "Order",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "SettingsSteps",
                keyColumn: "Order",
                keyValue: 2);
        }
    }
}
