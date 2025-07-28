using Microsoft.EntityFrameworkCore;
using UnitTest.Database;
using UnitTest.DTO;
using UnitTest.Models;

namespace UnitTest.Services {
	public class ProdutoService : IProdutoService {
		private readonly EstoqueDB _context;
		public ProdutoService(EstoqueDB db) {
			_context = db;
		}

		public async Task<Produto> CreateProduto(ProdutoDTO novoProduto) {
			var categoria = await _context.Categorias.FirstOrDefaultAsync(c => c.Nome.ToLower() == novoProduto.CategoriaNome.ToLower());

			if (categoria == null) {
				throw new InvalidOperationException("Categoria não existe");
			}

			var produtoExiste = await _context.Produtos.AnyAsync(c => c.Nome.ToLower() == novoProduto.Nome.ToLower());

			if (produtoExiste) {
				throw new ArgumentException("Produto já existe");
			}


			Produto produto = new Produto {
				Nome = novoProduto.Nome,
				Descricao = novoProduto.Descricao,
				Preco = novoProduto.Preco,
				EstoqueMinimo = novoProduto.EstoqueMinimo,
				Categoria = categoria,
			};

			_context.Produtos.Add(produto);
			await _context.SaveChangesAsync();
			return produto;
		}

		public async Task DeleteProduto(Guid id) {
			var produto = await _context.Produtos.FindAsync(id);

			if (produto == null) {
				throw new InvalidOperationException($"Produto com Id {id} não encontrado");
			}

			var temMovimentacao = await _context.Movimentacoes.AnyAsync(m => m.Produto.Id == id);

			if (temMovimentacao) {
				throw new ArgumentException("Não é possível excluir produto que possui movimentação");
			}

			_context.Produtos.Remove(produto);
			await _context.SaveChangesAsync();
			return;
		}

		public async Task DesativarProduto(Guid id) {
			var produto = await _context.Produtos.FindAsync(id);

			if (produto == null) {
				throw new InvalidOperationException("Produto não encontrado");
			}

			produto.Ativo = false;
			await _context.SaveChangesAsync();
			return;
		}

		public async Task<Produto> GetProduto(Guid id) {
			var resultado = await _context.Produtos.Include(p => p.Categoria).FirstOrDefaultAsync(p => p.Id == id);

			if (resultado == null) {
				throw new InvalidOperationException($"Produto com Id {id} não encontrado");
			}

			return resultado;
		}

		public async Task<List<Produto>> GetProdutos(ProdutosFiltroDTO filtro) {
			var query = _context.Produtos.Include(p => p.Categoria).AsQueryable();

			if (filtro != null) {
				if (!string.IsNullOrEmpty(filtro.Nome)) {
					query = query.Where(p => p.Nome.Contains(filtro.Nome));
				}
				if (!string.IsNullOrEmpty(filtro.Descricao)) {
					query = query.Where(p => p.Descricao.Contains(filtro.Descricao));
				}
				if (filtro.MinValor.HasValue) {
					query = query.Where(p => p.Preco > filtro.MinValor);
				}
				if (filtro.MaxValor.HasValue) {
					query = query.Where(p => p.Preco < filtro.MaxValor);
				}
				if (filtro.MinQuantidade.HasValue) {
					query = query.Where(p => p.Quantidade > filtro.MinQuantidade);
				}
				if (filtro.MaxQuantidade.HasValue) {
					query = query.Where(p => p.Quantidade < filtro.MaxQuantidade);
				}
				if (filtro.CategoriaNome != null) {
					query = query.Where(p => p.Categoria.Nome.ToLower() == filtro.CategoriaNome.ToLower());
				}
				if (filtro.MinData != null) {
					query = query.Where(p => p.DataCadastro > filtro.MinData);
				}
				if (filtro.MaxData != null) {
					query = query.Where(p => p.DataCadastro < filtro.MaxData);
				}
				if (filtro.Ativo.HasValue) {
					query = query.Where(p => p.Ativo == filtro.Ativo);
				}
			}

			var resultado = await query.OrderBy(p => p.Nome).ToListAsync();
			return resultado;
		}

		public async Task<Produto> UpdateProduto(Guid id, ProdutoDTO produtoAtualizado) {
			var categoria = await _context.Categorias.FirstOrDefaultAsync(c => c.Nome == produtoAtualizado.CategoriaNome);

			if (categoria == null) {
				throw new InvalidOperationException("Categoria não existe");
			}

			var produto = await _context.Produtos.FindAsync(id);

			if (produto == null) {
				throw new InvalidOperationException("Produto não encontrado");
			}

			if (produto.Nome != produtoAtualizado.Nome) {
				var nomeExiste = await _context.Produtos.AnyAsync(p => p.Nome.ToLower() == produtoAtualizado.Nome.ToLower());

				if (nomeExiste) {
					throw new ArgumentException("Já existe um produto com esse nome");
				}
			}

			_context.Entry(produto).CurrentValues.SetValues(produtoAtualizado);
			produto.Id = id;
			produto.DataCadastro = produto.DataCadastro;
			produto.Categoria = categoria;

			await _context.SaveChangesAsync();
			return produto;
		}
	}

	public interface IProdutoService {
		public Task<Produto> GetProduto(Guid id);
		public Task<List<Produto>> GetProdutos(ProdutosFiltroDTO filtro);
		public Task<Produto> CreateProduto(ProdutoDTO novoProduto);
		public Task<Produto> UpdateProduto(Guid id, ProdutoDTO produtoAtualizado);
		public Task DeleteProduto(Guid id);
		public Task DesativarProduto(Guid id);
	}
}
