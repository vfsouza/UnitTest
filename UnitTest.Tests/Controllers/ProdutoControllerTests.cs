using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Moq;
using UnitTest.Controllers;
using UnitTest.DTO;
using UnitTest.Models;
using UnitTest.Services;

namespace UnitTest.Tests.Controllers {
	public class ProdutoControllerTests {
		private readonly Mock<IProdutoService> _mockProdutoService;
		private readonly ProdutoController _controller;

		public ProdutoControllerTests() {
			_mockProdutoService = new Mock<IProdutoService>();
			_controller = new ProdutoController(_mockProdutoService.Object);
		}

		[Fact]
		public async Task GetProduto_DeveRetornarOk_QuandoProdutoExiste() {
			// Arrange
			var produtoId = Guid.NewGuid();
			var produtoResponse = new Produto() {
				Id = produtoId,
				Nome = "Produto Teste"
			};

			_mockProdutoService
				.Setup(s => s.GetProduto(produtoId))
				.ReturnsAsync(produtoResponse);

			// Act
			var resultado = await _controller.GetProduto(produtoId);

			// Assert
			resultado.Result.Should().BeOfType<OkObjectResult>();
			var okResult = resultado.Result as OkObjectResult;
			okResult?.Value.Should().Be(produtoResponse);
		}

		[Fact]
		public async Task CreateProduto_DeveRetornarBadRequest_QuandoModelStateInvalido() {
			// Arrange
			_controller.ModelState.AddModelError("Nome", "Nome é obrigatório");
			var produtoDto = new ProdutoDTO();

			// Act
			var resultado = await _controller.CreateProduto(produtoDto);

			// Assert
			resultado.Result.Should().BeOfType<BadRequestObjectResult>();
		}
	}
}