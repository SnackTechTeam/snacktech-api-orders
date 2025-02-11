using System.Diagnostics.CodeAnalysis;

namespace SnackTech.Orders.Common.Dto.Api
{
    [ExcludeFromCodeCoverage]
    public class ProdutoDto
    {
        public Guid IdentificacaoProduto { get; set; }
        public int Categoria { get; set; }
        public string Nome { get; set; } = string.Empty;
        public string Descricao { get; set; } = string.Empty;
        public decimal Valor { get; set; }
    }
}