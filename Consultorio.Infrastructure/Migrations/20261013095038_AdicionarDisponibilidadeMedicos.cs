using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Consultorio.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AdicionarDisponibilidadeMedicos : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "HorariosMedicos",
                columns: table => new
                {
                    HorarioMedicoId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    MedicoId = table.Column<int>(type: "int", nullable: false),
                    DiaSemana = table.Column<int>(type: "int", nullable: false),
                    HoraInicio = table.Column<TimeSpan>(type: "time", nullable: false),
                    HoraFim = table.Column<TimeSpan>(type: "time", nullable: false),
                    Ativo = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_HorariosMedicos", x => x.HorarioMedicoId);
                    table.ForeignKey(
                        name: "FK_HorariosMedicos_Medicos_MedicoId",
                        column: x => x.MedicoId,
                        principalTable: "Medicos",
                        principalColumn: "MedicoId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "IndisponibilidadesMedicos",
                columns: table => new
                {
                    IndisponibilidadeMedicoId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    MedicoId = table.Column<int>(type: "int", nullable: false),
                    DataHoraInicio = table.Column<DateTime>(type: "datetime2(0)", nullable: false),
                    DataHoraFim = table.Column<DateTime>(type: "datetime2(0)", nullable: false),
                    Motivo = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_IndisponibilidadesMedicos", x => x.IndisponibilidadeMedicoId);
                    table.ForeignKey(
                        name: "FK_IndisponibilidadesMedicos_Medicos_MedicoId",
                        column: x => x.MedicoId,
                        principalTable: "Medicos",
                        principalColumn: "MedicoId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_HorariosMedicos_MedicoId_DiaSemana_HoraInicio",
                table: "HorariosMedicos",
                columns: new[] { "MedicoId", "DiaSemana", "HoraInicio" });

            migrationBuilder.CreateIndex(
                name: "IX_IndisponibilidadesMedicos_MedicoId_DataHoraInicio",
                table: "IndisponibilidadesMedicos",
                columns: new[] { "MedicoId", "DataHoraInicio" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "HorariosMedicos");

            migrationBuilder.DropTable(
                name: "IndisponibilidadesMedicos");
        }
    }
}
