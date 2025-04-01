using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HumanAid.Migrations
{
    /// <inheritdoc />
    public partial class ActualizarRelacionVoluntarioUsuario : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Voluntario_Usuario_UsuarioId",
                table: "Voluntario");

            migrationBuilder.AddForeignKey(
                name: "FK_Voluntario_Usuario_UsuarioId",
                table: "Voluntario",
                column: "UsuarioId",
                principalTable: "Usuario",
                principalColumn: "UsuarioId",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Voluntario_Usuario_UsuarioId",
                table: "Voluntario");

            migrationBuilder.AddForeignKey(
                name: "FK_Voluntario_Usuario_UsuarioId",
                table: "Voluntario",
                column: "UsuarioId",
                principalTable: "Usuario",
                principalColumn: "UsuarioId",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
