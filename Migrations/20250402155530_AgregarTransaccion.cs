using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HumanAid.Migrations
{
    /// <inheritdoc />
    public partial class AgregarTransaccion : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Transaccion",
                columns: table => new
                {
                    TransaccionId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Monto = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Metodo = table.Column<int>(type: "int", nullable: false),
                    Referencia = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    FechaPago = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Tipo = table.Column<int>(type: "int", nullable: false),
                    Descripcion = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    SocioId = table.Column<int>(type: "int", nullable: false),
                    TipoCuotaId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Transaccion", x => x.TransaccionId);
                    table.ForeignKey(
                        name: "FK_Transaccion_Socio_SocioId",
                        column: x => x.SocioId,
                        principalTable: "Socio",
                        principalColumn: "SocioId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Transaccion_TipoCuota_TipoCuotaId",
                        column: x => x.TipoCuotaId,
                        principalTable: "TipoCuota",
                        principalColumn: "TipoCuotaId");
                });

            migrationBuilder.CreateIndex(
                name: "IX_Transaccion_SocioId",
                table: "Transaccion",
                column: "SocioId");

            migrationBuilder.CreateIndex(
                name: "IX_Transaccion_TipoCuotaId",
                table: "Transaccion",
                column: "TipoCuotaId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Transaccion");
        }
    }
}
