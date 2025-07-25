﻿using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HumanAid.Migrations
{
    /// <inheritdoc />
    public partial class AgregarCodigoFecha : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "CodigoEnvio",
                table: "Envio",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<DateTime>(
                name: "FechaSalida",
                table: "Envio",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CodigoEnvio",
                table: "Envio");

            migrationBuilder.DropColumn(
                name: "FechaSalida",
                table: "Envio");
        }
    }
}
