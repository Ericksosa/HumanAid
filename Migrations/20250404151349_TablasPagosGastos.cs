using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HumanAid.Migrations
{
    /// <inheritdoc />
    public partial class TablasPagosGastos : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Gastos",
                columns: table => new
                {
                    IdGastos = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Descripcion = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Importe = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    FechaGasto = table.Column<DateTime>(type: "datetime2", nullable: false),
                    SedeId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Gastos", x => x.IdGastos);
                    table.ForeignKey(
                        name: "FK_Gastos_Sede_SedeId",
                        column: x => x.SedeId,
                        principalTable: "Sede",
                        principalColumn: "SedeId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Pago",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    SocioId = table.Column<int>(type: "int", nullable: false),
                    TipoCuotaId = table.Column<int>(type: "int", nullable: false),
                    MontoCuota = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    CantidadCuotas = table.Column<int>(type: "int", nullable: false),
                    FechaPago = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Pago", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Pago_Socio_SocioId",
                        column: x => x.SocioId,
                        principalTable: "Socio",
                        principalColumn: "SocioId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Pago_TipoCuota_TipoCuotaId",
                        column: x => x.TipoCuotaId,
                        principalTable: "TipoCuota",
                        principalColumn: "TipoCuotaId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Gastos_SedeId",
                table: "Gastos",
                column: "SedeId");

            migrationBuilder.CreateIndex(
                name: "IX_Pago_SocioId",
                table: "Pago",
                column: "SocioId");

            migrationBuilder.CreateIndex(
                name: "IX_Pago_TipoCuotaId",
                table: "Pago",
                column: "TipoCuotaId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Gastos");

            migrationBuilder.DropTable(
                name: "Pago");
        }
    }
}
