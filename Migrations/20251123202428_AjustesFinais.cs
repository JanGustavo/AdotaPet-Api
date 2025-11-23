using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AdotaPet.Api.Migrations
{
    /// <inheritdoc />
    public partial class AjustesFinais : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_PetPhotos_Pets_PetID",
                table: "PetPhotos");

            migrationBuilder.RenameColumn(
                name: "PetID",
                table: "PetPhotos",
                newName: "PetId");

            migrationBuilder.RenameIndex(
                name: "IX_PetPhotos_PetID",
                table: "PetPhotos",
                newName: "IX_PetPhotos_PetId");

            migrationBuilder.AddForeignKey(
                name: "FK_PetPhotos_Pets_PetId",
                table: "PetPhotos",
                column: "PetId",
                principalTable: "Pets",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_PetPhotos_Pets_PetId",
                table: "PetPhotos");

            migrationBuilder.RenameColumn(
                name: "PetId",
                table: "PetPhotos",
                newName: "PetID");

            migrationBuilder.RenameIndex(
                name: "IX_PetPhotos_PetId",
                table: "PetPhotos",
                newName: "IX_PetPhotos_PetID");

            migrationBuilder.AddForeignKey(
                name: "FK_PetPhotos_Pets_PetID",
                table: "PetPhotos",
                column: "PetID",
                principalTable: "Pets",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
