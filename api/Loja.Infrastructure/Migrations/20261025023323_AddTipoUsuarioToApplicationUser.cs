using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Loja.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddTipoUsuarioToApplicationUser : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "TipoUsuarioId",
                table: "AspNetUsers",
                type: "int",
                nullable: false,
                defaultValue: 2);

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUsers_TipoUsuarioId",
                table: "AspNetUsers",
                column: "TipoUsuarioId");

            migrationBuilder.AddForeignKey(
                name: "FK_AspNetUsers_TiposUsuarios_TipoUsuarioId",
                table: "AspNetUsers",
                column: "TipoUsuarioId",
                principalTable: "TiposUsuarios",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AspNetUsers_TiposUsuarios_TipoUsuarioId",
                table: "AspNetUsers");

            migrationBuilder.DropIndex(
                name: "IX_AspNetUsers_TipoUsuarioId",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "TipoUsuarioId",
                table: "AspNetUsers");
        }
    }
}
