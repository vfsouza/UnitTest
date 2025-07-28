using System.Data.Common;
using FluentAssertions;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using UnitTest.Database;
using UnitTest.DTO;
using UnitTest.Models;
using UnitTest.Services;

namespace UnitTest.Tests.Services {
	public class ProdutoServiceTests : IDisposable {
		private readonly DbConnection _connection;
		private readonly EstoqueDB _context;
		private readonly ProdutoService _produtoService;

		public ProdutoServiceTests() {
			// Criar conexão persistente para o teste
			_connection = new SqliteConnection("Data Source=:memory:");
			_connection.Open();

			var options = new DbContextOptionsBuilder<EstoqueDB>()
				.UseSqlite(_connection)
				.Options;

			_context = new EstoqueDB(options);
			_context.Database.EnsureCreated();

			_produtoService = new ProdutoService(_context);

			SeedTestData();
		}

		private void SeedTestData() {
			var categoria = new Categoria {
				Nome = "Eletrônicos",
				Descricao = "Categoria teste"
			};

			_context.Categorias.Add(categoria);
			_context.SaveChanges();
		}

		[Fact]
		public async Task CreateProduto_DeveRetornarProduto_QuandoDadosValidos() {
			// Arrange
			var produtoDto = new ProdutoDTO {
				Nome = "Notebook",
				Descricao = "Notebook Dell",
				Preco = 2500.99f,
				EstoqueMinimo = 5,
				CategoriaNome = "Eletrônicos"
			};

			// Act
			var resultado = await _produtoService.CreateProduto(produtoDto);

			// Assert
			resultado.Should().NotBeNull();
			resultado.Nome.Should().Be("Notebook");
		}

		[Fact]
		public async Task CreateProduto_DeveLancarExcecao_QuandoCategoriaNaoExiste() {
			// Arrange
			var produtoDto = new ProdutoDTO {
				Nome = "Produto Teste",
				CategoriaNome = "Categoria Inexistente"
			};

			// Act & Assert
			var exception = await Assert.ThrowsAsync<InvalidOperationException>(() => _produtoService.CreateProduto(produtoDto));

			exception.Message.Should().Be("Categoria não existe");
		}

		[Fact]
		public async Task GetProdutos_DeveRetornarListaVazia_QuandoNaoHaProdutos() {
			// Act
			var resultado = await _produtoService.GetProdutos(null!);

			// Assert
			resultado.Should().NotBeNull();
			resultado.Should().BeEmpty();
		}

		public void Dispose() {
			_context.Dispose();
			_connection.Dispose();
		}
	}
}