using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Loja.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class LimparDadosTeste : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Limpar dados de teste - removendo apenas tabelas que existem
            migrationBuilder.Sql("DELETE FROM `CarrinhoProdutos`;");
            migrationBuilder.Sql("DELETE FROM `VendasProdutosTags`;");
            migrationBuilder.Sql("DELETE FROM `VendasProdutos`;");
            migrationBuilder.Sql("DELETE FROM `Vendas`;");
            migrationBuilder.Sql("DELETE FROM `ProdutoImagens`;");
            migrationBuilder.Sql("DELETE FROM `TagsProdutos`;");
            migrationBuilder.Sql("DELETE FROM `Produtos`;");
            migrationBuilder.Sql("DELETE FROM `ClienteEnderecos`;");
            migrationBuilder.Sql("DELETE FROM `AspNetUsers`;");
            migrationBuilder.Sql("DELETE FROM `Tags`;");
            migrationBuilder.Sql("DELETE FROM `VendasStatus`;");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            // Nada a fazer no down
        }
    }
}
