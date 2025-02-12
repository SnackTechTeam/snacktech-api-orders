using System.Diagnostics.CodeAnalysis;

namespace SnackTech.Orders.Common.Dto.Api;

[ExcludeFromCodeCoverage]
public class PedidoAtualizacaoDto
{
    public string IdentificacaoPedido { get; set; }
    public IEnumerable<PedidoItemAtualizacaoDto> PedidoItens { get; set; }
}
