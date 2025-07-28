namespace UnitTest.DTO {
	public class MovimentacoesFiltroDTO {
		public Guid? ProdutoID { get; set; }
		public Guid? CategoriaId { get; set; }
		public DateTime? MaxData { get; set; }
		public DateTime? MinData { get; set; }
		public int? MaxQuantidade { get; set; }
		public int? MinQuantidade { get; set; }
	}
}
