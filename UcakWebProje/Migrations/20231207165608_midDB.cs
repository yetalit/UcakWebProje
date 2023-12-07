using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace UcakWebProje.Migrations
{
    /// <inheritdoc />
    public partial class midDB : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Price",
                table: "Biletler");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "Price",
                table: "Biletler",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }
    }
}
