using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Loja.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class Vendas : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CarrinhoProdutos_Tags_TagId",
                table: "CarrinhoProdutos");

            migrationBuilder.DropForeignKey(
                name: "FK_Vendas_AspNetUsers_ClienteId",
                table: "Vendas");

            migrationBuilder.DropIndex(
                name: "IX_Vendas_ClienteId",
                table: "Vendas");

            migrationBuilder.DropColumn(
                name: "ClienteId",
                table: "Vendas");

            migrationBuilder.RenameColumn(
                name: "TagId",
                table: "CarrinhoProdutos",
                newName: "GeneroId");

            migrationBuilder.RenameIndex(
                name: "IX_CarrinhoProdutos_TagId",
                table: "CarrinhoProdutos",
                newName: "IX_CarrinhoProdutos_GeneroId");

            migrationBuilder.AddColumn<string>(
                name: "Cor",
                table: "VendasProdutos",
                type: "longtext",
                nullable: false)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<string>(
                name: "Genero",
                table: "VendasProdutos",
                type: "longtext",
                nullable: false)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<string>(
                name: "Tamanho",
                table: "VendasProdutos",
                type: "longtext",
                nullable: false)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<int>(
                name: "CarrinhoId",
                table: "Vendas",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "CorId",
                table: "CarrinhoProdutos",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_Vendas_CarrinhoId",
                table: "Vendas",
                column: "CarrinhoId");

            migrationBuilder.CreateIndex(
                name: "IX_CarrinhoProdutos_CorId",
                table: "CarrinhoProdutos",
                column: "CorId");

            migrationBuilder.AddForeignKey(
                name: "FK_CarrinhoProdutos_Tags_CorId",
                table: "CarrinhoProdutos",
                column: "CorId",
                principalTable: "Tags",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_CarrinhoProdutos_Tags_GeneroId",
                table: "CarrinhoProdutos",
                column: "GeneroId",
                principalTable: "Tags",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Vendas_Carrinhos_CarrinhoId",
                table: "Vendas",
                column: "CarrinhoId",
                principalTable: "Carrinhos",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CarrinhoProdutos_Tags_CorId",
                table: "CarrinhoProdutos");

            migrationBuilder.DropForeignKey(
                name: "FK_CarrinhoProdutos_Tags_GeneroId",
                table: "CarrinhoProdutos");

            migrationBuilder.DropForeignKey(
                name: "FK_Vendas_Carrinhos_CarrinhoId",
                table: "Vendas");

            migrationBuilder.DropIndex(
                name: "IX_Vendas_CarrinhoId",
                table: "Vendas");

            migrationBuilder.DropIndex(
                name: "IX_CarrinhoProdutos_CorId",
                table: "CarrinhoProdutos");

            migrationBuilder.DropColumn(
                name: "Cor",
                table: "VendasProdutos");

            migrationBuilder.DropColumn(
                name: "Genero",
                table: "VendasProdutos");

            migrationBuilder.DropColumn(
                name: "Tamanho",
                table: "VendasProdutos");

            migrationBuilder.DropColumn(
                name: "CarrinhoId",
                table: "Vendas");

            migrationBuilder.DropColumn(
                name: "CorId",
                table: "CarrinhoProdutos");

            migrationBuilder.RenameColumn(
                name: "GeneroId",
                table: "CarrinhoProdutos",
                newName: "TagId");

            migrationBuilder.RenameIndex(
                name: "IX_CarrinhoProdutos_GeneroId",
                table: "CarrinhoProdutos",
                newName: "IX_CarrinhoProdutos_TagId");

            migrationBuilder.AddColumn<string>(
                name: "ClienteId",
                table: "Vendas",
                type: "varchar(255)",
                nullable: false,
                defaultValue: "")
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateIndex(
                name: "IX_Vendas_ClienteId",
                table: "Vendas",
                column: "ClienteId");

            migrationBuilder.AddForeignKey(
                name: "FK_CarrinhoProdutos_Tags_TagId",
                table: "CarrinhoProdutos",
                column: "TagId",
                principalTable: "Tags",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Vendas_AspNetUsers_ClienteId",
                table: "Vendas",
                column: "ClienteId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
