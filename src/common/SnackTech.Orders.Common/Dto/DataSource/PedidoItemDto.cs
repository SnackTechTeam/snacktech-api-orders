namespace SnackTech.Orders.Common.Dto.DataSource;

public class PedidoItemDto
{
    public Guid Id { get; set; }
    public int Quantidade { get; set; }
    public decimal ValorTotal { get; set; }
    public decimal ValorUnitario { get; set; }
    public string Observacao { get; set; }
    public Guid ProdutoId { get; set; }
}
