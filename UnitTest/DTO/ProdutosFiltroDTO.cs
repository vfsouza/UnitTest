using UnitTest.Models;

namespace UnitTest.DTO {
	public class ProdutosFiltroDTO {
		public float? MaxValor { get; set; }
		public float? MinValor { get; set; }
		public string? Nome { get; set; }
		public string? Descricao { get; set; }
		public int? MaxQuantidade { get; set; }
		public int? MinQuantidade { get; set; }
		public string? CategoriaNome { get; set; }
		public DateTime? MaxData { get; set; }
		public DateTime? MinData { get; set; }
		public bool? Ativo { get; set; }
	}
}
