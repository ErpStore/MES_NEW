using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MES.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class UpdateMaterialFields : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<double>(
                name: "Density",
                table: "Materials",
                type: "float",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<string>(
                name: "HandlingInfo",
                table: "Materials",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsBlocked",
                table: "Materials",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "Manufacturer",
                table: "Materials",
                type: "nvarchar(200)",
                maxLength: 200,
                nullable: true);

            migrationBuilder.AddColumn<double>(
                name: "MinLevel",
                table: "Materials",
                type: "float",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<int>(
                name: "ShelfLifeDays",
                table: "Materials",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "Unit",
                table: "Materials",
                type: "nvarchar(20)",
                maxLength: 20,
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Density",
                table: "Materials");

            migrationBuilder.DropColumn(
                name: "HandlingInfo",
                table: "Materials");

            migrationBuilder.DropColumn(
                name: "IsBlocked",
                table: "Materials");

            migrationBuilder.DropColumn(
                name: "Manufacturer",
                table: "Materials");

            migrationBuilder.DropColumn(
                name: "MinLevel",
                table: "Materials");

            migrationBuilder.DropColumn(
                name: "ShelfLifeDays",
                table: "Materials");

            migrationBuilder.DropColumn(
                name: "Unit",
                table: "Materials");
        }
    }
}
