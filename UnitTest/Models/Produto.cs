using System.Text.Json.Serialization;

namespace UnitTest.Models {
	public class Produto {
		public Guid Id { get; set; } = Guid.NewGuid();
		public string Nome { get; set; } = string.Empty;
		public string Descricao { get; set; } = string.Empty;
		public float Preco { get; set; }
		public int Quantidade { get; set; }
		public int EstoqueMinimo { get; set; }
		public Categoria Categoria { get; set; } = null!;
		public DateTime DataCadastro { get; set; } = DateTime.Now;
		public bool Ativo { get; set; } = true;

		[JsonIgnore]
		public List<Movimentacao> Movimentacoes { get; set; } = new();
	}
}