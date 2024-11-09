using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TaskMgtWebAPI.Migrations
{
    /// <inheritdoc />
    public partial class FirstMigration : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ProjectTb",
                columns: table => new
                {
                    ProjectId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ProjectName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    StartDate = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    EndDate = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProjectTb", x => x.ProjectId);
                });

            migrationBuilder.CreateTable(
                name: "RoleTb",
                columns: table => new
                {
                    RoleId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    RoleName = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RoleTb", x => x.RoleId);
                });

            migrationBuilder.CreateTable(
                name: "UserTb",
                columns: table => new
                {
                    UserId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    FirstName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    LastName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Email = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Password = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    RoleId = table.Column<int>(type: "int", nullable: false),
                    Role_ObjRoleId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserTb", x => x.UserId);
                    table.ForeignKey(
                        name: "FK_UserTb_RoleTb_Role_ObjRoleId",
                        column: x => x.Role_ObjRoleId,
                        principalTable: "RoleTb",
                        principalColumn: "RoleId");
                });

            migrationBuilder.CreateTable(
                name: "TaskTb",
                columns: table => new
                {
                    TaskId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Title = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    DueDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Priority = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Status = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    UserId = table.Column<int>(type: "int", nullable: false),
                    UserObjUserId = table.Column<int>(type: "int", nullable: true),
                    ProjectId = table.Column<int>(type: "int", nullable: false),
                    ProjectObjProjectId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TaskTb", x => x.TaskId);
                    table.ForeignKey(
                        name: "FK_TaskTb_ProjectTb_ProjectObjProjectId",
                        column: x => x.ProjectObjProjectId,
                        principalTable: "ProjectTb",
                        principalColumn: "ProjectId");
                    table.ForeignKey(
                        name: "FK_TaskTb_UserTb_UserObjUserId",
                        column: x => x.UserObjUserId,
                        principalTable: "UserTb",
                        principalColumn: "UserId");
                });

            migrationBuilder.CreateIndex(
                name: "IX_TaskTb_ProjectObjProjectId",
                table: "TaskTb",
                column: "ProjectObjProjectId");

            migrationBuilder.CreateIndex(
                name: "IX_TaskTb_UserObjUserId",
                table: "TaskTb",
                column: "UserObjUserId");

            migrationBuilder.CreateIndex(
                name: "IX_UserTb_Role_ObjRoleId",
                table: "UserTb",
                column: "Role_ObjRoleId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "TaskTb");

            migrationBuilder.DropTable(
                name: "ProjectTb");

            migrationBuilder.DropTable(
                name: "UserTb");

            migrationBuilder.DropTable(
                name: "RoleTb");
        }
    }
}
