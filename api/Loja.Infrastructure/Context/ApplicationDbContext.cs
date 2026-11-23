using Loja.Domain.Entities;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

namespace Loja.Infrastructure.Context;

public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }

    public DbSet<Produto> Produtos { get; set; }
    public DbSet<ProdutoImagens> ProdutoImagens { get; set; }
    public DbSet<Tags> Tags { get; set; }
    public DbSet<TagsProdutos> TagsProdutos { get; set; }
    public DbSet<TiposUsuarios> TiposUsuarios { get; set; }
    public DbSet<ClienteEnderecos> ClienteEnderecos { get; set; }
    public DbSet<Carrinho> Carrinhos { get; set; }
    public DbSet<CarrinhoProdutos> CarrinhoProdutos { get; set; }
    public DbSet<Vendas> Vendas { get; set; }
    public DbSet<VendasProdutos> VendasProdutos { get; set; }
    public DbSet<VendasProdutosTags> VendasProdutosTags { get; set; }
    public DbSet<VendasStatus> VendasStatus { get; set; }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        // Produto
        builder.Entity<Produto>(b =>
        {
            b.HasKey(p => p.Id);
            b.Property(p => p.Valor).HasPrecision(10, 2);

            b.Property(p => p.DataCadastro)
                .Metadata.SetAfterSaveBehavior(PropertySaveBehavior.Ignore);
            b.Property(p => p.UsuarioId)
                .Metadata.SetAfterSaveBehavior(PropertySaveBehavior.Ignore);

            b.HasOne(p => p.Usuario)
                .WithMany()
                .HasForeignKey(p => p.UsuarioId)
                .OnDelete(DeleteBehavior.Cascade);

            b.HasMany(p => p.Imagens)
                .WithOne(i => i.Produto)
                .HasForeignKey(i => i.ProdutoId)
                .OnDelete(DeleteBehavior.Cascade);

            b.HasMany(p => p.TagsProdutos)
                .WithOne(tp => tp.Produto)
                .HasForeignKey(tp => tp.ProdutoId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        // TagsProdutos
        builder.Entity<TagsProdutos>(b =>
        {
            b.HasOne(tp => tp.Tag)
                .WithMany(t => t.TagsProdutos)
                .HasForeignKey(tp => tp.TagId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        // TiposUsuarios (com Seed de Admin e Cliente)
        builder.Entity<TiposUsuarios>(b =>
        {
            b.HasKey(tu => tu.Id);
            b.HasData(
                new { Id = 1, Descricao = "Admin", Ativo = true },
                new { Id = 2, Descricao = "Cliente", Ativo = true }
            );
        });

        builder.Entity<ApplicationUser>(b =>
        {
            b.Property(u => u.TipoUsuarioId).HasDefaultValue(2);
            b.HasOne(u => u.TipoUsuario)
                .WithMany()
                .HasForeignKey(u => u.TipoUsuarioId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        // ClienteEnderecos
        builder.Entity<ClienteEnderecos>(b =>
        {
            b.HasKey(ce => ce.Id);
            b.HasOne(ce => ce.Cliente)
                .WithMany()
                .HasForeignKey(ce => ce.ClienteId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        // Carrinho
        builder.Entity<Carrinho>(b =>
        {
            b.HasKey(c => c.Id);
            b.HasOne(c => c.Usuario)
                .WithMany()
                .HasForeignKey(c => c.UsuarioId)
                .OnDelete(DeleteBehavior.Cascade);

            b.HasMany(c => c.Produtos)
                .WithOne(cp => cp.Carrinho)
                .HasForeignKey(cp => cp.CarrinhoId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        // CarrinhoProdutos (possui duas FKs apontando para a tabela Tags: TagId e TamanhoId)
        builder.Entity<CarrinhoProdutos>(b =>
        {
            b.HasKey(cp => cp.Id);

            b.HasOne(cp => cp.Produto)
                .WithMany()
                .HasForeignKey(cp => cp.ProdutoId)
                .OnDelete(DeleteBehavior.Cascade);

            b.HasOne(cp => cp.Genero)
                .WithMany()
                .HasForeignKey(cp => cp.GeneroId)
                .OnDelete(DeleteBehavior.Cascade);

            b.HasOne(cp => cp.Cor)
                .WithMany()
                .HasForeignKey(cp => cp.CorId)
                .OnDelete(DeleteBehavior.Cascade);

            b.HasOne(cp => cp.Tamanho)
                .WithMany()
                .HasForeignKey(cp => cp.TamanhoId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        // Vendas
        builder.Entity<Vendas>(b =>
        {
            b.HasKey(v => v.Id);
            b.Property(v => v.ValorFinal).HasPrecision(10, 2);

            b.HasOne(v => v.Usuario)
                .WithMany()
                .HasForeignKey(v => v.UsuarioId)
                .OnDelete(DeleteBehavior.Restrict);

            b.HasOne(v => v.Endereco)
                .WithMany()
                .HasForeignKey(v => v.EnderecoId)
                .OnDelete(DeleteBehavior.Restrict);

            b.HasMany(v => v.Produtos)
                .WithOne(vp => vp.Venda)
                .HasForeignKey(vp => vp.VendaId)
                .OnDelete(DeleteBehavior.Cascade);

            b.HasMany(v => v.StatusHistorico)
                .WithOne(status => status.Venda)
                .HasForeignKey(status => status.VendaId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        // VendasProdutos
        builder.Entity<VendasProdutos>(b =>
        {
            b.HasKey(vp => vp.Id);
            b.Property(vp => vp.ValorUnitario).HasPrecision(10, 2);

            b.HasOne(vp => vp.Produto)
                .WithMany()
                .HasForeignKey(vp => vp.ProdutoId)
                .OnDelete(DeleteBehavior.Restrict);

            b.HasMany(vp => vp.Tags)
                .WithOne(vpt => vpt.VendaProduto)
                .HasForeignKey(vpt => vpt.VendaProdutoId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        // VendasProdutosTags
        builder.Entity<VendasProdutosTags>(b =>
        {
            b.HasKey(vpt => vpt.Id);

            b.HasOne(vpt => vpt.Tag)
                .WithMany()
                .HasForeignKey(vpt => vpt.TagId)
                .OnDelete(DeleteBehavior.Restrict);
        });
    }
}