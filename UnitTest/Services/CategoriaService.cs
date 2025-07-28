using Microsoft.EntityFrameworkCore;
using UnitTest.Database;
using UnitTest.DTO;
using UnitTest.Models;

namespace UnitTest.Services {
	public class CategoriaService : ICategoriaService {
		private readonly EstoqueDB _context;

		public CategoriaService(EstoqueDB db) {
			_context = db;
		}

		public async Task<Categoria> AtualizarCategoria(Guid id, CategoriaDTO categoriaAtualizada) {
			Categoria? categoria = await _context.Categorias.FindAsync(id);

			if (categoria == null) {
				throw new InvalidOperationException("Categoria não existe");
			}

			var existeProduto = await _context.Produtos.AnyAsync(p => p.Categoria == categoria);

			if (existeProduto) {
				throw new ArgumentException("Existe um produto com essa categoria");
			}

			bool existeCategoria = await _context.Categorias.AnyAsync(c => c.Nome.ToLower() == categoriaAtualizada.Nome.ToLower());

			if (existeCategoria) {
				throw new ArgumentException("Categoria já existe");
			}

			categoria.Nome = categoriaAtualizada.Nome;
			categoria.Descricao = categoriaAtualizada.Descricao;

			await _context.SaveChangesAsync();
			return categoria;
		}

		public async Task<Categoria> CreateCategoriaAsync(CategoriaDTO categoria) {
			bool existeCategoria = await _context.Categorias.AnyAsync(c => c.Nome.ToLower() == categoria.Nome.ToLower());

			if (existeCategoria) {
				throw new ArgumentException("Já existe esta categoria");
			}

			var newCategoria = new Categoria { Nome = categoria.Nome, Descricao = categoria.Descricao };
			_context.Categorias.Add(newCategoria);
			await _context.SaveChangesAsync();
			return newCategoria;
		}

		public async Task<IEnumerable<Categoria>> GetCategoriasAsync() {
			return await _context.Categorias.OrderBy(c => c.Nome).ToListAsync();
		}
	}

	public interface ICategoriaService {
		public Task<IEnumerable<Categoria>> GetCategoriasAsync();
		public Task<Categoria> CreateCategoriaAsync(CategoriaDTO categoria);
		public Task<Categoria> AtualizarCategoria(Guid id, CategoriaDTO categoriaAtualizada);
	}
}
