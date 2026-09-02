using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MySubs.Migrations
{
    /// <inheritdoc />
    public partial class AddFamilyGroups : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "FamilyGroups",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CreatedByUserId = table.Column<string>(type: "nvarchar(450)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FamilyGroups", x => x.Id);
                    table.ForeignKey(
                        name: "FK_FamilyGroups_AspNetUsers_CreatedByUserId",
                        column: x => x.CreatedByUserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "FamilyGroupMembers",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    FamilyGroupId = table.Column<int>(type: "int", nullable: false),
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Role = table.Column<int>(type: "int", nullable: false),
                    JoinedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FamilyGroupMembers", x => x.Id);
                    table.ForeignKey(
                        name: "FK_FamilyGroupMembers_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_FamilyGroupMembers_FamilyGroups_FamilyGroupId",
                        column: x => x.FamilyGroupId,
                        principalTable: "FamilyGroups",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_FamilyGroupMembers_FamilyGroupId_UserId",
                table: "FamilyGroupMembers",
                columns: new[] { "FamilyGroupId", "UserId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_FamilyGroupMembers_UserId",
                table: "FamilyGroupMembers",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_FamilyGroups_CreatedByUserId",
                table: "FamilyGroups",
                column: "CreatedByUserId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "FamilyGroupMembers");

            migrationBuilder.DropTable(
                name: "FamilyGroups");
        }
    }
}
