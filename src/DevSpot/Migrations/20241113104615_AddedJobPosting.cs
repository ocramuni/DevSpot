using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DevSpot.Migrations
{
    /// <inheritdoc />
    public partial class AddedJobPosting : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "JobPosting",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Title = table.Column<string>(type: "nvarchar(999)", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(999)", nullable: false),
                    Company = table.Column<string>(type: "nvarchar(999)", nullable: false),
                    Location = table.Column<string>(type: "nvarchar(999)", nullable: false),
                    PostedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    IsApproved = table.Column<bool>(type: "bit", nullable: false),
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_JobPosting", x => x.Id);
                    table.ForeignKey(
                        name: "FK_JobPosting_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_JobPosting_UserId",
                table: "JobPosting",
                column: "UserId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "JobPosting");
        }
    }
}
