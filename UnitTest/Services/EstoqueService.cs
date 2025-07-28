using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;
using UnitTest.Database;
using UnitTest.DTO;
using UnitTest.Models;

namespace UnitTest.Services {
	public class EstoqueService : IEstoqueService {
		private readonly EstoqueDB _context;

		public EstoqueService(EstoqueDB db) {
			_context = db;
		}

		public async Task<(EstoqueResponseDTO Estoque, Guid MovimentacaoId)> CreateMovimentacao(MovimentacaoDTO mov) {
			if (mov.Quantidade <= 0) {
				throw new InvalidOperationException("Quantidade deve ser > 0");
			}

			var produto = await _context.Produtos.FindAsync(mov.ProdutoId);

			if (produto == null) {
				throw new InvalidOperationException("Produto não encontrado");
			}

			UpdateQuantidade(ref produto, mov.Quantidade, mov.TipoMovimentacao);

			Movimentacao movimentacao = new Movimentacao() {
				Produto = produto,
				Data = DateTime.Now,
				Quantidade = mov.Quantidade,
				TipoMovimentacao = mov.TipoMovimentacao,
			};

			_context.Movimentacoes.Add(movimentacao);
			await _context.SaveChangesAsync();

			var response = new EstoqueResponseDTO() {
				ProdutoId = produto.Id,
				Nome = produto.Nome,
				Preco = produto.Preco,
				Quantidade = produto.Quantidade
			};

			return (response, movimentacao.Id);
		}

		public async Task<(EstoqueResponseDTO Estoque, Guid MovimentacaoId)> DeleteMovimentacao(Guid id) {
			var movimentacao = await _context.Movimentacoes.Include(m => m.Produto).FirstOrDefaultAsync(m => m.Id == id);

			if (movimentacao == null) {
				throw new InvalidOperationException("Movimentação não encontrada");
			}

			var produto = movimentacao.Produto;

			UpdateQuantidade(ref produto, -movimentacao.Quantidade, movimentacao.TipoMovimentacao);

			_context.Movimentacoes.Remove(movimentacao);
			await _context.SaveChangesAsync();

			var response = new EstoqueResponseDTO() {
				ProdutoId = produto.Id,
				Nome = produto.Nome,
				Preco = produto.Preco,
				Quantidade = produto.Quantidade
			};

			return (response, movimentacao.Id);
		}

		public async Task<(EstoqueResponseDTO Estoque, IEnumerable<MovimentacaoResponseDTO> Movimentacoes)> GetEstoque(Guid id) {
			var produto = await _context.Produtos.FirstOrDefaultAsync(p => p.Id == id);

			if (produto == null) {
				throw new InvalidOperationException("Produto não encontrado");
			}

			var response = new EstoqueResponseDTO() {
				ProdutoId = produto.Id,
				Nome = produto.Nome,
				Preco = produto.Preco,
				Quantidade = produto.Quantidade
			};

			var movimentacoes = _context.Movimentacoes.Where(m => m.Produto.Id == produto.Id);

			var movimentacoesProduto = await movimentacoes.Select(m => new MovimentacaoResponseDTO() {
				MovimentacaoId = m.Id,
				ProdutoId = m.Produto.Id,
				Data = DateTime.Now,
				Quantidade = m.Quantidade,
				TipoMovimentacao = m.TipoMovimentacao
			}).ToListAsync();

			return (response, movimentacoesProduto);
		}

		public async Task<IEnumerable<EstoqueResponseDTO>> GetEstoques() {
			var produtos = await _context.Produtos.ToListAsync();
			var response = produtos.Select(p => new EstoqueResponseDTO() {
				ProdutoId = p.Id,
				Nome = p.Nome,
				Preco = p.Preco,
				Quantidade = p.Quantidade
			});
			return response;
		}

		public async Task<MovimentacaoResponseDTO> GetMovimentacao(Guid id) {
			var movimentacao = await _context.Movimentacoes.Include(m => m.Produto).FirstOrDefaultAsync(m => m.Id == id);

			if (movimentacao == null) {
				throw new InvalidOperationException("Movimentação não encontrada");
			}

			return new MovimentacaoResponseDTO() {
				MovimentacaoId = movimentacao.Id,
				ProdutoId = movimentacao.Produto.Id,
				Quantidade = movimentacao.Quantidade,
				TipoMovimentacao = movimentacao.TipoMovimentacao,
				Data = movimentacao.Data,
			};
		}

		public async Task<IEnumerable<EstoqueResponseDTO>> GetEstoqueBaixo() {
			var produtos = await _context.Produtos.Where(p => p.Quantidade < p.EstoqueMinimo).ToListAsync();

			var response = produtos.Select(p => new EstoqueResponseDTO() {
				ProdutoId = p.Id,
				Nome = p.Nome,
				Preco = p.Preco,
				Quantidade = p.Quantidade,
			});
			
			return response;
		}

		public async Task<IEnumerable<MovimentacaoResponseDTO>> GetMovimentacoes(MovimentacoesFiltroDTO? filtro) {
			if (filtro != null) {
				var query = _context.Movimentacoes.Include(m => m.Produto).AsQueryable();

				if (filtro.ProdutoID != null) {
					query = query.Where(m => m.Produto.Id == filtro.ProdutoID);
				}
				if (filtro.CategoriaId != null) {
					query = query.Where(m => m.Produto.Categoria.Id == filtro.CategoriaId);
				}
				if (filtro.MaxQuantidade.HasValue) {
					query = query.Where(m => m.Produto.Quantidade < filtro.MaxQuantidade.Value);
				}
				if (filtro.MinQuantidade.HasValue) {
					query = query.Where(m => m.Produto.Quantidade > filtro.MinQuantidade.Value);
				}
				if (filtro.MaxData.HasValue) {
					query = query.Where(m => m.Data < filtro.MaxData.Value);
				}
				if (filtro.MinData.HasValue) {
					query = query.Where(m => m.Data > filtro.MinData.Value);
				}

				var movimentacoes = await query.OrderBy(m => m.Produto.Nome).ToListAsync();

				var response = movimentacoes.Select(m => new MovimentacaoResponseDTO {
					MovimentacaoId = m.Id,
					ProdutoId = m.Produto.Id,
					TipoMovimentacao = m.TipoMovimentacao,
					Quantidade = m.Quantidade,
					Data = m.Data
				}).ToList();

				return response;
			}
			throw new InvalidOperationException("Filtro não setado");
		}

		internal void UpdateQuantidade(ref Produto produto, int quantidade, TipoMovimentacao tipoMovimentacao) {
			switch (tipoMovimentacao) {
				case TipoMovimentacao.Saida:
					if (produto.Quantidade - quantidade >= 0) {
						produto.Quantidade -= quantidade;
					} else {
						throw new ArgumentException($"Estoque {produto.Quantidade} não contém a quantidade solicitada {quantidade}");
					}
					break;
				case TipoMovimentacao.Entrada:
					produto.Quantidade += quantidade;
					break;
				default:
					throw new InvalidOperationException("Tipo de movimentação não definido");
			}
		}
	}

	public interface IEstoqueService {
		public Task<IEnumerable<EstoqueResponseDTO>> GetEstoques();
		public Task<(EstoqueResponseDTO Estoque, IEnumerable<MovimentacaoResponseDTO> Movimentacoes)> GetEstoque(Guid id);
		public Task<MovimentacaoResponseDTO> GetMovimentacao(Guid id);
		public Task<IEnumerable<MovimentacaoResponseDTO>> GetMovimentacoes(MovimentacoesFiltroDTO filtro);
		public Task<IEnumerable<EstoqueResponseDTO>> GetEstoqueBaixo();
		public Task<(EstoqueResponseDTO Estoque, Guid MovimentacaoId)> CreateMovimentacao(MovimentacaoDTO mov);
		public Task<(EstoqueResponseDTO Estoque, Guid MovimentacaoId)> DeleteMovimentacao(Guid id);
	}
}