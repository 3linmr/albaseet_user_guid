using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Shared.Repository.Migrations
{
    /// <inheritdoc />
    public partial class UpdateCountryModel : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "CountryCode",
                table: "Countries",
                type: "varchar(10)",
                maxLength: 10,
                nullable: true)
                .Annotation("MySql:CharSet", "utf8mb4")
                .Annotation("Relational:ColumnOrder", 5);

            migrationBuilder.AddColumn<string>(
                name: "PhoneCode",
                table: "Countries",
                type: "varchar(10)",
                maxLength: 10,
                nullable: true)
                .Annotation("MySql:CharSet", "utf8mb4")
                .Annotation("Relational:ColumnOrder", 6);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CountryCode",
                table: "Countries");

            migrationBuilder.DropColumn(
                name: "PhoneCode",
                table: "Countries");
        }
    }
}
