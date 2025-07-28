using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using UnitTest.Database;
using UnitTest.DTO;
using UnitTest.Models;
using UnitTest.Services;

namespace UnitTest.Controllers {
	[ApiController]
	[Route("api/[controller]")]
	public class CategoriaController : ControllerBase {
		private readonly ICategoriaService _categoriaService;

		public CategoriaController(ICategoriaService service) {
			_categoriaService = service;
		}

		[HttpGet]
		public async Task<ActionResult<List<Categoria>>> GetCategoria() {
			var categorias = await _categoriaService.GetCategoriasAsync();
			return Ok(categorias);
		}

		[HttpPost]
		public async Task<ActionResult<Categoria>> CreateCategoria(CategoriaDTO categoria) {
			if (!ModelState.IsValid) {
				return BadRequest(ModelState);
			}

			try {
				var novaCategoria = await _categoriaService.CreateCategoriaAsync(categoria);
				return CreatedAtAction(nameof(GetCategoria), new { id = novaCategoria.Id }, novaCategoria);
			} catch (InvalidOperationException ex) {
				return BadRequest(ex.Message);
			} catch (ArgumentException ex) {
				return Conflict(ex.Message);
			}
		}

		[HttpPut("{id}")]
		public async Task<IActionResult> UpdateCategoria(Guid id, CategoriaDTO categoria) {
			if (!ModelState.IsValid) {
				return BadRequest(ModelState);
			}

			try {
				var categoriaAtualizada = await _categoriaService.AtualizarCategoria(id, categoria);

				if (categoriaAtualizada == null)
					return NotFound($"Categoria com ID {id} não encontrada");

				return Ok(categoriaAtualizada);
			} catch (InvalidOperationException ex) {
				return BadRequest(ex.Message);
			} catch (ArgumentException ex) {
				return Conflict(ex.Message);
			}
		}
	}
}
