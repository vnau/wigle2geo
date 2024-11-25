using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Wigle2Geo.Migrations
{
    /// <inheritdoc />
    public partial class AddIndices : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_location_bssid",
                table: "location",
                column: "bssid");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_location_bssid",
                table: "location");
        }
    }
}
