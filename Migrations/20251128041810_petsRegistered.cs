using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AdotaPet.Api.Migrations
{
    /// <inheritdoc />
    public partial class petsRegistered : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "PetsRegistered",
                table: "Users",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "PetsRegistered",
                table: "Users");
        }
    }
}
