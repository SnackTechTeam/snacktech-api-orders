namespace SnackTech.Orders.Common.Dto.Api;

public class PedidoItemRetornoDto
{
    public Guid IdentificacaoItem { get; set; }
    public int Quantidade { get; set; }
    public decimal Valor { get; set; }
    public string? Observacao { get; set; }
    public Guid IdentificacaoProduto { get; set; }
}
