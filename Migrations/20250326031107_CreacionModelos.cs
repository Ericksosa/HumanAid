using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HumanAid.Migrations
{
    /// <inheritdoc />
    public partial class CreacionModelos : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Envio",
                columns: table => new
                {
                    EnvioId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Fecha = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Destino = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    TipoEnvio = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Estado = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Envio", x => x.EnvioId);
                });

            migrationBuilder.CreateTable(
                name: "Sede",
                columns: table => new
                {
                    SedeId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Nombre = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Ciudad = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Direccion = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    Director = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    FechaFundacion = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Sede", x => x.SedeId);
                });

            migrationBuilder.CreateTable(
                name: "TipoCuota",
                columns: table => new
                {
                    TipoCuotaId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Nombre = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Importe = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Descripcion = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TipoCuota", x => x.TipoCuotaId);
                });

            migrationBuilder.CreateTable(
                name: "Alimento",
                columns: table => new
                {
                    AlimentoId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    EnvioId = table.Column<int>(type: "int", nullable: false),
                    Tipo = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Peso = table.Column<decimal>(type: "decimal(18,2)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Alimento", x => x.AlimentoId);
                    table.ForeignKey(
                        name: "FK_Alimento_Envio_EnvioId",
                        column: x => x.EnvioId,
                        principalTable: "Envio",
                        principalColumn: "EnvioId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Medicamento",
                columns: table => new
                {
                    MedicamentoId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    EnvioId = table.Column<int>(type: "int", nullable: false),
                    Nombre = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Dosis = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    cantidad = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Medicamento", x => x.MedicamentoId);
                    table.ForeignKey(
                        name: "FK_Medicamento_Envio_EnvioId",
                        column: x => x.EnvioId,
                        principalTable: "Envio",
                        principalColumn: "EnvioId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "MisionHumanitaria",
                columns: table => new
                {
                    MisionId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    EnvioId = table.Column<int>(type: "int", nullable: false),
                    Fecha = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Descripcion = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Estado = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MisionHumanitaria", x => x.MisionId);
                    table.ForeignKey(
                        name: "FK_MisionHumanitaria_Envio_EnvioId",
                        column: x => x.EnvioId,
                        principalTable: "Envio",
                        principalColumn: "EnvioId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "EnvioSede",
                columns: table => new
                {
                    EnvioId = table.Column<int>(type: "int", nullable: false),
                    SedeId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EnvioSede", x => new { x.EnvioId, x.SedeId });
                    table.ForeignKey(
                        name: "FK_EnvioSede_Envio_EnvioId",
                        column: x => x.EnvioId,
                        principalTable: "Envio",
                        principalColumn: "EnvioId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_EnvioSede_Sede_SedeId",
                        column: x => x.SedeId,
                        principalTable: "Sede",
                        principalColumn: "SedeId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Voluntario",
                columns: table => new
                {
                    VoluntarioId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Nombre = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Direccion = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    FechaNacimiento = table.Column<DateTime>(type: "datetime2", nullable: false),
                    SedeId = table.Column<int>(type: "int", nullable: false),
                    FechaRegistro = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Email = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Telefono = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Voluntario", x => x.VoluntarioId);
                    table.ForeignKey(
                        name: "FK_Voluntario_Sede_SedeId",
                        column: x => x.SedeId,
                        principalTable: "Sede",
                        principalColumn: "SedeId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Socio",
                columns: table => new
                {
                    SocioId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Nombre = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    FechaNacimiento = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CuentaBancaria = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    FechaPago = table.Column<DateTime>(type: "datetime2", nullable: false),
                    TipoCuotaId = table.Column<int>(type: "int", nullable: false),
                    SedeId = table.Column<int>(type: "int", nullable: false),
                    FechaRegistro = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Socio", x => x.SocioId);
                    table.ForeignKey(
                        name: "FK_Socio_Sede_SedeId",
                        column: x => x.SedeId,
                        principalTable: "Sede",
                        principalColumn: "SedeId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Socio_TipoCuota_TipoCuotaId",
                        column: x => x.TipoCuotaId,
                        principalTable: "TipoCuota",
                        principalColumn: "TipoCuotaId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "VoluntarioAdministrativo",
                columns: table => new
                {
                    VoluntarioAdministrativoId = table.Column<int>(type: "int", nullable: false),
                    Profesion = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Departamento = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_VoluntarioAdministrativo", x => x.VoluntarioAdministrativoId);
                    table.ForeignKey(
                        name: "FK_VoluntarioAdministrativo_Voluntario_VoluntarioAdministrativoId",
                        column: x => x.VoluntarioAdministrativoId,
                        principalTable: "Voluntario",
                        principalColumn: "VoluntarioId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "VoluntarioSanitario",
                columns: table => new
                {
                    VoluntarioSanitarioId = table.Column<int>(type: "int", nullable: false),
                    Profesion = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Disponibilidad = table.Column<bool>(type: "bit", nullable: false),
                    NumeroTrabajosRealizados = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_VoluntarioSanitario", x => x.VoluntarioSanitarioId);
                    table.ForeignKey(
                        name: "FK_VoluntarioSanitario_Voluntario_VoluntarioSanitarioId",
                        column: x => x.VoluntarioSanitarioId,
                        principalTable: "Voluntario",
                        principalColumn: "VoluntarioId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "VoluntarioMision",
                columns: table => new
                {
                    VoluntarioSanitarioId = table.Column<int>(type: "int", nullable: false),
                    MisionId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_VoluntarioMision", x => new { x.VoluntarioSanitarioId, x.MisionId });
                    table.ForeignKey(
                        name: "FK_VoluntarioMision_MisionHumanitaria_MisionId",
                        column: x => x.MisionId,
                        principalTable: "MisionHumanitaria",
                        principalColumn: "MisionId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_VoluntarioMision_VoluntarioSanitario_VoluntarioSanitarioId",
                        column: x => x.VoluntarioSanitarioId,
                        principalTable: "VoluntarioSanitario",
                        principalColumn: "VoluntarioSanitarioId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Alimento_EnvioId",
                table: "Alimento",
                column: "EnvioId");

            migrationBuilder.CreateIndex(
                name: "IX_EnvioSede_SedeId",
                table: "EnvioSede",
                column: "SedeId");

            migrationBuilder.CreateIndex(
                name: "IX_Medicamento_EnvioId",
                table: "Medicamento",
                column: "EnvioId");

            migrationBuilder.CreateIndex(
                name: "IX_MisionHumanitaria_EnvioId",
                table: "MisionHumanitaria",
                column: "EnvioId");

            migrationBuilder.CreateIndex(
                name: "IX_Socio_SedeId",
                table: "Socio",
                column: "SedeId");

            migrationBuilder.CreateIndex(
                name: "IX_Socio_TipoCuotaId",
                table: "Socio",
                column: "TipoCuotaId");

            migrationBuilder.CreateIndex(
                name: "IX_Voluntario_SedeId",
                table: "Voluntario",
                column: "SedeId");

            migrationBuilder.CreateIndex(
                name: "IX_VoluntarioMision_MisionId",
                table: "VoluntarioMision",
                column: "MisionId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Alimento");

            migrationBuilder.DropTable(
                name: "EnvioSede");

            migrationBuilder.DropTable(
                name: "Medicamento");

            migrationBuilder.DropTable(
                name: "Socio");

            migrationBuilder.DropTable(
                name: "VoluntarioAdministrativo");

            migrationBuilder.DropTable(
                name: "VoluntarioMision");

            migrationBuilder.DropTable(
                name: "TipoCuota");

            migrationBuilder.DropTable(
                name: "MisionHumanitaria");

            migrationBuilder.DropTable(
                name: "VoluntarioSanitario");

            migrationBuilder.DropTable(
                name: "Envio");

            migrationBuilder.DropTable(
                name: "Voluntario");

            migrationBuilder.DropTable(
                name: "Sede");
        }
    }
}
