using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Loja.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class SubstituirCarrinhoIdPorUsuarioId : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "UsuarioId",
                table: "Vendas",
                type: "varchar(255)",
                nullable: true)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.Sql(
                "UPDATE Vendas v INNER JOIN Carrinhos c ON c.Id = v.CarrinhoId SET v.UsuarioId = c.UsuarioId;");

            migrationBuilder.AlterColumn<string>(
                name: "UsuarioId",
                table: "Vendas",
                type: "varchar(255)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "varchar(255)",
                oldNullable: true)
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.DropForeignKey(
                name: "FK_Vendas_Carrinhos_CarrinhoId",
                table: "Vendas");

            migrationBuilder.DropIndex(
                name: "IX_Vendas_CarrinhoId",
                table: "Vendas");

            migrationBuilder.DropColumn(
                name: "CarrinhoId",
                table: "Vendas");

            migrationBuilder.CreateIndex(
                name: "IX_Vendas_UsuarioId",
                table: "Vendas",
                column: "UsuarioId");

            migrationBuilder.AddForeignKey(
                name: "FK_Vendas_AspNetUsers_UsuarioId",
                table: "Vendas",
                column: "UsuarioId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Vendas_AspNetUsers_UsuarioId",
                table: "Vendas");

            migrationBuilder.DropIndex(
                name: "IX_Vendas_UsuarioId",
                table: "Vendas");

            migrationBuilder.AddColumn<int>(
                name: "CarrinhoId",
                table: "Vendas",
                type: "int",
                nullable: true);

            migrationBuilder.Sql(
                "UPDATE Vendas v INNER JOIN Carrinhos c ON c.UsuarioId = v.UsuarioId SET v.CarrinhoId = c.Id;");

            migrationBuilder.AlterColumn<int>(
                name: "CarrinhoId",
                table: "Vendas",
                type: "int",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.DropColumn(
                name: "UsuarioId",
                table: "Vendas");

            migrationBuilder.CreateIndex(
                name: "IX_Vendas_CarrinhoId",
                table: "Vendas",
                column: "CarrinhoId");

            migrationBuilder.AddForeignKey(
                name: "FK_Vendas_Carrinhos_CarrinhoId",
                table: "Vendas",
                column: "CarrinhoId",
                principalTable: "Carrinhos",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
