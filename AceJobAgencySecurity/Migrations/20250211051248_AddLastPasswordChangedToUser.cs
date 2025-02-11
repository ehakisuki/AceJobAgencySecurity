using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AceJobAgencySecurity.Migrations
{
    /// <inheritdoc />
    public partial class AddLastPasswordChangedToUser : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "LastPasswordChanged",
                table: "AspNetUsers",
                type: "datetime2",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "LastPasswordChanged",
                table: "AspNetUsers");
        }
    }
}
