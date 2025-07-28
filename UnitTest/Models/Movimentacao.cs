
namespace UnitTest.Models {
	public class Movimentacao {
		public Guid Id { get; set; } = Guid.NewGuid();
		public Produto Produto { get; set; } = null!;
		public TipoMovimentacao TipoMovimentacao { get; set; }
		public int Quantidade { get; set; }
		public DateTime Data { get; set; } = DateTime.Now;
	}

	public enum TipoMovimentacao {
		Entrada,
		Saida
	}
}
