using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Consultorio.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AdicionarDuracaoConsulta : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
         migrationBuilder.AddColumn<int>(
         name: "DuracaoMinutos",
         table: "Consultas",
         type: "int",
         nullable: false,
         defaultValue: 30);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
       name: "DuracaoMinutos",
       table: "Consultas");
        }
    }
}
