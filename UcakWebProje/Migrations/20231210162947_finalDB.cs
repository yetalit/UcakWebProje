using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace UcakWebProje.Migrations
{
    /// <inheritdoc />
    public partial class finalDB : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_Biletler",
                table: "Biletler");

            migrationBuilder.AddColumn<DateTime>(
                name: "orderTime",
                table: "Biletler",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddPrimaryKey(
                name: "PK_Biletler",
                table: "Biletler",
                columns: new[] { "departure", "destination", "date", "AirLine", "passengerUN", "orderTime" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_Biletler",
                table: "Biletler");

            migrationBuilder.DropColumn(
                name: "orderTime",
                table: "Biletler");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Biletler",
                table: "Biletler",
                columns: new[] { "departure", "destination", "date", "AirLine", "passengerUN" });
        }
    }
}
