using System.Diagnostics.CodeAnalysis;

namespace SnackTech.Orders.Common.Dto.ApiSources.Payments
{
    [ExcludeFromCodeCoverage]
    public class PedidoPagamentoDto
    {
        public required Guid PedidoId { get; set; }
        public required ClientePagamentoDto Cliente { get; set; }
        public required IEnumerable<PedidoItemPagamentoDto> Itens { get; set; }
    }
}
