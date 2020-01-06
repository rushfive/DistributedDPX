using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace UserManagement.API.Migrations
{
    public partial class InitialMigration : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "AccessRole",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    Name = table.Column<string>(nullable: false),
                    ParticipantWorkflows = table.Column<string>(nullable: false),
                    Participants = table.Column<string>(nullable: false),
                    SubmissionData = table.Column<string>(nullable: false),
                    TestParticipantsAndData = table.Column<string>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AccessRole", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "User",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    UserName = table.Column<string>(nullable: false),
                    FirstName = table.Column<string>(nullable: false),
                    LastName = table.Column<string>(nullable: true),
                    MobilePhone = table.Column<string>(nullable: true),
                    Language = table.Column<string>(nullable: true),
                    Enabled = table.Column<bool>(nullable: false),
                    Locked = table.Column<bool>(nullable: false),
                    CreatedDate = table.Column<DateTime>(nullable: false),
                    Type = table.Column<string>(nullable: false),
                    IsAdmin = table.Column<bool>(nullable: false),
                    EmptySiteAccess = table.Column<bool>(nullable: false),
                    AllSitesAccess = table.Column<bool>(nullable: false),
                    AllCampaignsAccess = table.Column<bool>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_User", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "UserGroup",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    Name = table.Column<string>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserGroup", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "UserAccessRole",
                columns: table => new
                {
                    UserId = table.Column<Guid>(nullable: false),
                    AccessRoleId = table.Column<Guid>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserAccessRole", x => new { x.UserId, x.AccessRoleId });
                    table.ForeignKey(
                        name: "FK_UserAccessRole_AccessRole_AccessRoleId",
                        column: x => x.AccessRoleId,
                        principalTable: "AccessRole",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_UserAccessRole_User_UserId",
                        column: x => x.UserId,
                        principalTable: "User",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "UserCampaign",
                columns: table => new
                {
                    UserId = table.Column<Guid>(nullable: false),
                    CampaignId = table.Column<string>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserCampaign", x => new { x.UserId, x.CampaignId });
                    table.ForeignKey(
                        name: "FK_UserCampaign_User_UserId",
                        column: x => x.UserId,
                        principalTable: "User",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "UserUserGroup",
                columns: table => new
                {
                    UserId = table.Column<Guid>(nullable: false),
                    UserGroupId = table.Column<Guid>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserUserGroup", x => new { x.UserId, x.UserGroupId });
                    table.ForeignKey(
                        name: "FK_UserUserGroup_UserGroup_UserGroupId",
                        column: x => x.UserGroupId,
                        principalTable: "UserGroup",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_UserUserGroup_User_UserId",
                        column: x => x.UserId,
                        principalTable: "User",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_UserAccessRole_AccessRoleId",
                table: "UserAccessRole",
                column: "AccessRoleId");

            migrationBuilder.CreateIndex(
                name: "IX_UserUserGroup_UserGroupId",
                table: "UserUserGroup",
                column: "UserGroupId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "UserAccessRole");

            migrationBuilder.DropTable(
                name: "UserCampaign");

            migrationBuilder.DropTable(
                name: "UserUserGroup");

            migrationBuilder.DropTable(
                name: "AccessRole");

            migrationBuilder.DropTable(
                name: "UserGroup");

            migrationBuilder.DropTable(
                name: "User");
        }
    }
}
