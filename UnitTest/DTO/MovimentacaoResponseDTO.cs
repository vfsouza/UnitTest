using UnitTest.Models;

namespace UnitTest.DTO {
	public class MovimentacaoResponseDTO {
		public Guid MovimentacaoId { get; set; }
		public Guid ProdutoId { get; set; }
		public TipoMovimentacao TipoMovimentacao { get; set; }
		public int Quantidade { get; set; }
		public DateTime Data { get; set; }
	}
}
