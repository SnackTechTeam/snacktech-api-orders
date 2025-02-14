using System.Diagnostics.CodeAnalysis;

namespace SnackTech.Orders.Common.Dto.ApiSources.Payments
{
    [ExcludeFromCodeCoverage]
    public class PedidoItemPagamentoDto
    {
        public Guid PedidoItemId { get; set; }
        public decimal Valor { get; set; }
    }
}