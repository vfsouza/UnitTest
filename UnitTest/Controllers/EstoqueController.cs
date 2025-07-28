using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using UnitTest.DTO;
using UnitTest.Models;
using UnitTest.Services;

namespace UnitTest.Controllers {
	[ApiController]
	[Route("api/[controller]")]
	public class EstoqueController : ControllerBase {
		private readonly IEstoqueService _estoqueService;

		public EstoqueController(IEstoqueService service) {
			_estoqueService = service;
		}

		[HttpGet("{produtoId}")]
		public async Task<ActionResult> GetEstoque(Guid produtoId) {
			try {
				var resultado = await _estoqueService.GetEstoque(produtoId);
				var response = new {
					Estoque = resultado.Estoque,
					Movimentacoes = resultado.Movimentacoes,
				};
				return Ok(response);
			} catch (InvalidOperationException ex) {
				return BadRequest(ex.Message);
			}
		}

		[HttpGet]
		public async Task<ActionResult<IEnumerable<Produto>>> GetEstoque() {
			try {
				var resultado = await _estoqueService.GetEstoques();
				return Ok(resultado);
			} catch (InvalidOperationException ex) {
				return BadRequest(ex.Message);
			}
		}

		[HttpGet("movimentacao/{movimentacaoId}")]
		public async Task<ActionResult<MovimentacaoResponseDTO>> GetMovimentacao(Guid movimentacaoId) {
			try {
				var resultado = await _estoqueService.GetMovimentacao(movimentacaoId);
				return Ok(resultado);
			} catch (InvalidOperationException ex) {
				return BadRequest(ex.Message);
			}
		}

		[HttpGet("movimentacao")]
		public async Task<ActionResult<IEnumerable<MovimentacaoResponseDTO>>> GetMovimentacao([FromQuery] MovimentacoesFiltroDTO filtro) {
			if (!ModelState.IsValid) {
				return BadRequest(ModelState);
			}

			try {
				var resultado = await _estoqueService.GetMovimentacoes(filtro);
				return Ok(resultado);
			} catch (InvalidOperationException ex) {
				return BadRequest(ex.Message);
			}
		}

		[HttpGet("minimo")]
		public async Task<ActionResult<EstoqueResponseDTO>> GetMovimentacaoMinimo() {
			var resultado = await _estoqueService.GetEstoqueBaixo();
			return Ok(resultado);
		}

		[HttpPost]
		public async Task<ActionResult<EstoqueResponseDTO>> CreateMovimentacao(MovimentacaoDTO movimentacao) {
			try {
				var resultado = await _estoqueService.CreateMovimentacao(movimentacao);
				return CreatedAtAction(nameof(GetMovimentacao), new { id = resultado.MovimentacaoId }, resultado);
			} catch (InvalidOperationException ex) {
				return BadRequest(ex.Message);
			} catch (ArgumentException ex) {
				return Conflict(ex.Message);
			}
		}

		[HttpDelete("movimentacao/{movimentacaoId}")]
		public async Task<ActionResult> DeleteMovimentacao(Guid movimentacaoId) {
			try {
				var resultado = await _estoqueService.DeleteMovimentacao(movimentacaoId);
				var response = new {
					Estoque = resultado.Estoque,
					MovimentacaoId = resultado.MovimentacaoId
				};
				return Ok(response);
			} catch (InvalidOperationException ex) {
				return BadRequest(ex.Message);
			}
		}
	}
}