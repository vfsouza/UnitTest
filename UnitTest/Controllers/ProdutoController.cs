using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using UnitTest.DTO;
using UnitTest.Models;
using UnitTest.Services;

namespace UnitTest.Controllers {
	[ApiController]
	[Route("api/[controller]")]
	public class ProdutoController : ControllerBase {
		private readonly IProdutoService _produtoService;

		public ProdutoController(IProdutoService service) {
			_produtoService = service;
		}

		[HttpGet]
		public async Task<ActionResult<IEnumerable<Produto>>> GetProduto([FromQuery] ProdutosFiltroDTO filtro) {
			if (!ModelState.IsValid) {
				return BadRequest(ModelState);
			}

			try {
				var resultado = await _produtoService.GetProdutos(filtro);
				return Ok(resultado);
			} catch (InvalidOperationException ex) {
				return BadRequest(ex.Message);
			} catch (ArgumentException ex) {
				return Conflict(ex.Message);
			}
		}

		[HttpGet("{id}")]
		public async Task<ActionResult<Produto>> GetProduto(Guid id) {
			try {
				var resultado = await _produtoService.GetProduto(id);
				return Ok(resultado);
			} catch (InvalidOperationException ex) {
				return BadRequest(ex.Message);
			} catch (ArgumentException ex) {
				return Conflict(ex.Message);
			}
		}

		[HttpPost]
		public async Task<ActionResult<Produto>> CreateProduto(ProdutoDTO produto) {
			if (!ModelState.IsValid) {
				return BadRequest(ModelState);
			}

			try {
				var resultado = await _produtoService.CreateProduto(produto);
				return CreatedAtAction(nameof(GetProduto), new { id = resultado.Id }, resultado);
			} catch (InvalidOperationException ex) {
				return BadRequest(ex.Message);
			} catch (ArgumentException ex) {
				return Conflict(ex.Message);
			}
		}

		[HttpPut("{id}")]
		public async Task<ActionResult<Produto>> UpdateProduto(Guid id, ProdutoDTO produto) {
			if (!ModelState.IsValid) {
				return BadRequest(ModelState);
			}

			try {
				var resultado = await _produtoService.UpdateProduto(id, produto);
				return Ok(resultado);
			} catch (InvalidOperationException ex) {
				return BadRequest(ex.Message);
			} catch (ArgumentException ex) {
				return Conflict(ex.Message);
			}
		}

		[HttpDelete("{id}")]
		public async Task<IActionResult> DeleteProduto(Guid id, [FromQuery] bool soft = false) {
			try {
				if (soft) {
					await _produtoService.DesativarProduto(id);
				} else {
					await _produtoService.DeleteProduto(id);
				}
				return NoContent();
			} catch (InvalidOperationException ex) {
				return BadRequest(ex.Message);
			} catch (ArgumentException ex) {
				return Conflict(ex.Message);
			}
		}
	}
}
