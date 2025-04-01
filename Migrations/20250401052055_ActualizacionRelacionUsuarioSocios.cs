using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HumanAid.Migrations
{
    /// <inheritdoc />
    public partial class ActualizacionRelacionUsuarioSocios : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Socio_Usuario_UsuarioId",
                table: "Socio");

            migrationBuilder.AddForeignKey(
                name: "FK_Socio_Usuario_UsuarioId",
                table: "Socio",
                column: "UsuarioId",
                principalTable: "Usuario",
                principalColumn: "UsuarioId",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Socio_Usuario_UsuarioId",
                table: "Socio");

            migrationBuilder.AddForeignKey(
                name: "FK_Socio_Usuario_UsuarioId",
                table: "Socio",
                column: "UsuarioId",
                principalTable: "Usuario",
                principalColumn: "UsuarioId",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
