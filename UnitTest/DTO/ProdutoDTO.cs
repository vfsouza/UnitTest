using UnitTest.Models;

namespace UnitTest.DTO {
	public class ProdutoDTO {
		public string Nome { get; set; }
		public string Descricao { get; set; }
		public float Preco { get; set; }
		public int EstoqueMinimo { get; set; }
		public string CategoriaNome { get; set; }
	}
}
