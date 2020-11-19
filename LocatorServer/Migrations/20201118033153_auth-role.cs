using Microsoft.EntityFrameworkCore.Migrations;

namespace LocatorServer.Migrations
{
    public partial class authrole : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[] { "2i3e7218-4232-420c-z4q2-1f9q65492768", "d4b9f87f-abff-4cfb-ba8b-4e547bd9b3bb", "authorized", "AUTHORIZED" });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "2i3e7218-4232-420c-z4q2-1f9q65492768");
        }
    }
}
