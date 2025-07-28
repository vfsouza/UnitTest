using UnitTest.Models;

namespace UnitTest.DTO {
	public class MovimentacaoDTO {
		public Guid ProdutoId { get; set; }
		public TipoMovimentacao TipoMovimentacao { get; set; }
		public int Quantidade { get; set; }
	}
}
