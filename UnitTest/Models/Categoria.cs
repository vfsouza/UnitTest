using System.Text.Json.Serialization;

namespace UnitTest.Models {
	public class Categoria {
		public Guid Id { get; set; } = Guid.NewGuid();
		public string Nome { get; set; } = string.Empty;
		public string Descricao { get; set; } = string.Empty;

		[JsonIgnore]
		public List<Produto> Produtos { get; set; } = new();
	}
}