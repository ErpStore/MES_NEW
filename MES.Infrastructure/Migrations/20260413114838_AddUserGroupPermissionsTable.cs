using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MES.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddUserGroupPermissionsTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "UserGroupRights");

            migrationBuilder.DropTable(
                name: "UserRights");

            migrationBuilder.CreateTable(
                name: "UserGroupPermissions",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserGroupId = table.Column<int>(type: "int", nullable: false),
                    ScreenKey = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    CanAdd = table.Column<bool>(type: "bit", nullable: false),
                    CanEdit = table.Column<bool>(type: "bit", nullable: false),
                    CanDelete = table.Column<bool>(type: "bit", nullable: false),
                    IsVisibleForUser = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserGroupPermissions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_UserGroupPermissions_UserGroups_UserGroupId",
                        column: x => x.UserGroupId,
                        principalTable: "UserGroups",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_UserGroupPermissions_UserGroupId",
                table: "UserGroupPermissions",
                column: "UserGroupId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "UserGroupPermissions");

            migrationBuilder.CreateTable(
                name: "UserGroupRights",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserGroupId = table.Column<int>(type: "int", nullable: false),
                    CanAdd = table.Column<bool>(type: "bit", nullable: false),
                    CanDelete = table.Column<bool>(type: "bit", nullable: false),
                    CanEdit = table.Column<bool>(type: "bit", nullable: false),
                    IsVisibleForUser = table.Column<bool>(type: "bit", nullable: false),
                    ScreenKey = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserGroupRights", x => x.Id);
                    table.ForeignKey(
                        name: "FK_UserGroupRights_UserGroups_UserGroupId",
                        column: x => x.UserGroupId,
                        principalTable: "UserGroups",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "UserRights",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserId = table.Column<int>(type: "int", nullable: false),
                    CanAdd = table.Column<bool>(type: "bit", nullable: false),
                    CanDelete = table.Column<bool>(type: "bit", nullable: false),
                    CanEdit = table.Column<bool>(type: "bit", nullable: false),
                    ScreenKey = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserRights", x => x.Id);
                    table.ForeignKey(
                        name: "FK_UserRights_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_UserGroupRights_UserGroupId",
                table: "UserGroupRights",
                column: "UserGroupId");

            migrationBuilder.CreateIndex(
                name: "IX_UserRights_UserId",
                table: "UserRights",
                column: "UserId");
        }
    }
}
