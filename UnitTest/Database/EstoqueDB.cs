using Microsoft.EntityFrameworkCore;
using UnitTest.Models;

namespace UnitTest.Database {
	public class EstoqueDB : DbContext {
		public EstoqueDB(DbContextOptions<EstoqueDB> options) : base(options) { }

		public DbSet<Produto> Produtos { get; set; }
		public DbSet<Categoria> Categorias { get; set; }
		public DbSet<Movimentacao> Movimentacoes { get; set; }

		protected override void OnModelCreating(ModelBuilder modelBuilder) {
			base.OnModelCreating(modelBuilder);

			modelBuilder.Entity<Produto>(entity => {
				entity.HasKey(p => p.Id);

				entity.Property<Guid>("CategoriaId");

				entity.HasOne(p => p.Categoria)
					.WithMany(c => c.Produtos)
					.HasForeignKey("CategoriaId")
					.OnDelete(DeleteBehavior.Restrict);
			});

			modelBuilder.Entity<Movimentacao>(entity => {
				entity.HasKey(m => m.Id);

				entity.Property(m => m.TipoMovimentacao)
					.HasConversion<string>();

				entity.Property<Guid>("ProdutoId");

				entity.HasOne(m => m.Produto)
					.WithMany(p => p.Movimentacoes)
					.HasForeignKey("ProdutoId")
					.OnDelete(DeleteBehavior.Cascade);
			});

			modelBuilder.Entity<Categoria>(entity => {
				entity.HasKey(c => c.Id);
				entity.Property(c => c.Nome).IsRequired().HasMaxLength(100);
			});
		}
	}
}
