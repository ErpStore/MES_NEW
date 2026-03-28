using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MES.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class UpdateFeedingPathSchema : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Name",
                table: "FeedingPaths");

            migrationBuilder.AddColumn<string>(
                name: "BinCode",
                table: "FeedingPaths",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "BinNumber",
                table: "FeedingPaths",
                type: "nvarchar(20)",
                maxLength: 20,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<double>(
                name: "CurrentStock",
                table: "FeedingPaths",
                type: "float",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<DateTime>(
                name: "ExpiryDate",
                table: "FeedingPaths",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "FilledDate",
                table: "FeedingPaths",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "MaterialId",
                table: "FeedingPaths",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<double>(
                name: "MaxCapacity",
                table: "FeedingPaths",
                type: "float",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.CreateIndex(
                name: "IX_FeedingPaths_MaterialId",
                table: "FeedingPaths",
                column: "MaterialId");

            migrationBuilder.AddForeignKey(
                name: "FK_FeedingPaths_Materials_MaterialId",
                table: "FeedingPaths",
                column: "MaterialId",
                principalTable: "Materials",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_FeedingPaths_Materials_MaterialId",
                table: "FeedingPaths");

            migrationBuilder.DropIndex(
                name: "IX_FeedingPaths_MaterialId",
                table: "FeedingPaths");

            migrationBuilder.DropColumn(
                name: "BinCode",
                table: "FeedingPaths");

            migrationBuilder.DropColumn(
                name: "BinNumber",
                table: "FeedingPaths");

            migrationBuilder.DropColumn(
                name: "CurrentStock",
                table: "FeedingPaths");

            migrationBuilder.DropColumn(
                name: "ExpiryDate",
                table: "FeedingPaths");

            migrationBuilder.DropColumn(
                name: "FilledDate",
                table: "FeedingPaths");

            migrationBuilder.DropColumn(
                name: "MaterialId",
                table: "FeedingPaths");

            migrationBuilder.DropColumn(
                name: "MaxCapacity",
                table: "FeedingPaths");

            migrationBuilder.AddColumn<string>(
                name: "Name",
                table: "FeedingPaths",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: false,
                defaultValue: "");
        }
    }
}
