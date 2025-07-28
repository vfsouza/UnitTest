using System.Data.Common;
using FluentAssertions;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using UnitTest.Database;
using UnitTest.Models;
using UnitTest.Services;

namespace UnitTest.Tests.Services {
	public class EstoqueServiceTests {
        private readonly DbConnection _connection;
        private readonly EstoqueDB _context;
        private readonly EstoqueService _estoqueService;

        public EstoqueServiceTests() {
            // Criar conexão persistente para o teste
            _connection = new SqliteConnection("Data Source=:memory:");
            _connection.Open();

            var options = new DbContextOptionsBuilder<EstoqueDB>()
                .UseSqlite(_connection)
                .Options;

            _context = new EstoqueDB(options);
            _context.Database.EnsureCreated();

            _estoqueService = new EstoqueService(_context);

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
        
		[Theory]
        [InlineData(10, 5, 15)]  // Estoque inicial 10, entrada 5, resultado 15
        [InlineData(0, 10, 10)]  // Estoque inicial 0, entrada 10, resultado 10
        [InlineData(50, 25, 75)] // Estoque inicial 50, entrada 25, resultado 75
        public void UpdateQuantidade_DeveIncrementarQuantidade_QuandoEntrada(
            int estoqueInicial, int quantidadeEntrada, int estoqueEsperado)
        {
            // Arrange
            var produto = new Produto 
            { 
                Nome = "Produto Teste",
                Quantidade = estoqueInicial 
            };

            // Act
            _estoqueService.UpdateQuantidade(ref produto, quantidadeEntrada, TipoMovimentacao.Entrada);

            // Assert
            produto.Quantidade.Should().Be(estoqueEsperado);
        }

        [Theory]
        [InlineData(10, 5, 5)]   // Estoque inicial 10, saída 5, resultado 5
        [InlineData(20, 10, 10)] // Estoque inicial 20, saída 10, resultado 10
        [InlineData(15, 15, 0)]  // Estoque inicial 15, saída 15, resultado 0
        public void UpdateQuantidade_DeveDecrementarQuantidade_QuandoSaidaValida(
            int estoqueInicial, int quantidadeSaida, int estoqueEsperado)
        {
            // Arrange
            var produto = new Produto 
            { 
                Nome = "Produto Teste",
                Quantidade = estoqueInicial 
            };

            // Act
            _estoqueService.UpdateQuantidade(ref produto, quantidadeSaida, TipoMovimentacao.Saida);

            // Assert
            produto.Quantidade.Should().Be(estoqueEsperado);
        }

        [Theory]
        [InlineData(5, 10)]  // Estoque 5, tentar saída 10
        [InlineData(0, 1)]   // Estoque 0, tentar saída 1
        [InlineData(10, 15)] // Estoque 10, tentar saída 15
        public void UpdateQuantidade_DeveLancarExcecao_QuandoSaidaInvalida(
            int estoqueInicial, int quantidadeSaida)
        {
            // Arrange
            var produto = new Produto 
            { 
                Nome = "Produto Teste",
                Quantidade = estoqueInicial 
            };

            // Act & Assert
            var exception = Assert.Throws<ArgumentException>(
                () => _estoqueService.UpdateQuantidade(ref produto, quantidadeSaida, TipoMovimentacao.Saida)
            );

            exception.Message.Should().Contain($"Estoque {estoqueInicial} não contém a quantidade solicitada {quantidadeSaida}");
            
            // Verificar que quantidade não foi alterada
            produto.Quantidade.Should().Be(estoqueInicial);
        }

        [Fact]
        public void UpdateQuantidade_DeveLancarExcecao_QuandoTipoMovimentacaoInvalido()
        {
            // Arrange
            var produto = new Produto 
            { 
                Nome = "Produto Teste",
                Quantidade = 10 
            };
            var tipoInvalido = (TipoMovimentacao)999; // Valor inválido

            // Act & Assert
            var exception = Assert.Throws<InvalidOperationException>(
                () => _estoqueService.UpdateQuantidade(ref produto, 5, tipoInvalido)
            );

            exception.Message.Should().Be("Tipo de movimentação não definido");
            
            // Verificar que quantidade não foi alterada
            produto.Quantidade.Should().Be(10);
        }
	}
}