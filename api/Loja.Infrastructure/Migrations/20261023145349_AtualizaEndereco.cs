using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Loja.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AtualizaEndereco : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Estado",
                table: "ClienteEnderecos");

            migrationBuilder.RenameColumn(
                name: "Localidade",
                table: "ClienteEnderecos",
                newName: "Cidade");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Cidade",
                table: "ClienteEnderecos",
                newName: "Localidade");

            migrationBuilder.AddColumn<string>(
                name: "Estado",
                table: "ClienteEnderecos",
                type: "longtext",
                nullable: false)
                .Annotation("MySql:CharSet", "utf8mb4");
        }
    }
}
