using System;
using System.Linq;
using Microsoft.EntityFrameworkCore;

namespace Franquias.Api
{
    public class FranquiasDbContext : DbContext
    {
        public FranquiasDbContext(DbContextOptions<FranquiasDbContext> options)
            : base(options)
        {
        }

        public DbSet<Franqueadora> Franqueadoras { get; set; }
        public DbSet<Unidade> Unidades { get; set; }
        public DbSet<Usuario> Usuarios { get; set; }
        public DbSet<Categoria> Categorias { get; set; }
        public DbSet<Fornecedor> Fornecedores { get; set; }
        public DbSet<Produto> Produtos { get; set; }
        public DbSet<EstoqueProduto> Estoques { get; set; }
        public DbSet<MovimentoEstoque> MovimentosEstoque { get; set; }
        public DbSet<Venda> Vendas { get; set; }
        public DbSet<ItemVenda> ItensVenda { get; set; }
        public DbSet<Chamado> Chamados { get; set; }
        public DbSet<RoyaltyConfig> RoyaltyConfigs { get; set; }
        public DbSet<Responsavel> Responsaveis { get; set; }

        public void SeedData()
        {
            if (Franqueadoras.Any() || Usuarios.Any() || Categorias.Any())
            {
                return;
            }

            var franqueadora = new Franqueadora
            {
                NomeFantasia = "Rede Exemplo",
                RazaoSocial = "Rede Exemplo LTDA",
                Cnpj = "99999999000199",
                Email = "contato@redeexemplo.local",
                Ativa = true,
                DataCadastro = DateTime.UtcNow
            };

            Franqueadoras.Add(franqueadora);
            SaveChanges();

            var unidade = new Unidade
            {
                Nome = "Unidade Exemplo",
                Cnpj = "88888888000188",
                Endereco = "Rua de Exemplo, 100",
                Cidade = "Gravata",
                Estado = "PE",
                Telefone = "81999999999",
                PercentualRoyalty = 5.00m,
                Ativa = true,
                DataInicio = DateTime.UtcNow,
                FranqueadoraId = franqueadora.Id
            };

            Unidades.Add(unidade);
            SaveChanges();

            var categoria = new Categoria
            {
                Nome = "Bebidas",
                Ativa = true
            };

            Categorias.Add(categoria);
            SaveChanges();

            var fornecedor = new Fornecedor
            {
                Nome = "Fornecedor Exemplo LTDA",
                Cnpj = "77777777000177",
                Email = "fornecedor@exemplo.local",
                Telefone = "81988888888",
                Ativo = true
            };

            Fornecedores.Add(fornecedor);
            SaveChanges();

            var produto = new Produto
            {
                Nome = "Agua Mineral",
                Descricao = "Garrafa de agua mineral",
                Preco = 4.50m,
                EstoqueMinimo = 5,
                CategoriaId = categoria.Id,
                FornecedorId = fornecedor.Id,
                Ativo = true
            };

            Produtos.Add(produto);
            SaveChanges();

            var estoque = new EstoqueProduto
            {
                ProdutoId = produto.Id,
                UnidadeId = unidade.Id,
                Quantidade = 20
            };

            Estoques.Add(estoque);
            SaveChanges();

            var responsavel = new Responsavel
            {
                Nome = "Responsavel Exemplo",
                Cpf = "12345678901",
                Telefone = "81977777777",
                Email = "responsavel@exemplo.local",
                Ativo = true,
                UnidadeId = unidade.Id
            };

            Responsaveis.Add(responsavel);
            SaveChanges();

            var usuario = new Usuario
            {
                Nome = "Administrador Exemplo",
                Email = "admin.exemplo@franquias.local",
                SenhaHash = "3EB3FE66B31E3B4D10FA70B5CAD49C7112294AF6AE4E476A1C405155D45AA121",
                Perfil = PerfilUsuario.Administrador,
                UnidadeId = null,
                Ativo = true
            };

            Usuarios.Add(usuario);
            SaveChanges();
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Franqueadora>()
                .HasIndex(f => f.Cnpj)
                .IsUnique();

            modelBuilder.Entity<Unidade>()
                .HasIndex(u => u.Cnpj)
                .IsUnique();

            modelBuilder.Entity<Usuario>()
                .HasIndex(u => u.Email)
                .IsUnique();

            modelBuilder.Entity<Fornecedor>()
                .HasIndex(f => f.Cnpj)
                .IsUnique();

            modelBuilder.Entity<EstoqueProduto>()
                .HasIndex(e => new { e.ProdutoId, e.UnidadeId })
                .IsUnique();

            modelBuilder.Entity<Responsavel>()
                .HasIndex(r => new { r.UnidadeId, r.Cpf })
                .IsUnique();

            modelBuilder.Entity<Franqueadora>()
                .HasMany(f => f.Unidades)
                .WithOne(u => u.Franqueadora)
                .HasForeignKey(u => u.FranqueadoraId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<MovimentoEstoque>()
                .HasOne(m => m.EstoqueProduto)
                .WithMany()
                .HasForeignKey(m => m.EstoqueUnidadeId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Responsavel>()
                .HasOne(r => r.Unidade)
                .WithMany()
                .HasForeignKey(r => r.UnidadeId)
                .OnDelete(DeleteBehavior.Restrict);

            base.OnModelCreating(modelBuilder);
        }
    }
}