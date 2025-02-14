using System.Diagnostics.CodeAnalysis;

namespace SnackTech.Orders.Common.Dto.ApiSources.Payments
{
    public class PedidoItemPagamentoDto
    {
        public Guid PedidoItemId { get; set; }
        public decimal Valor { get; set; }
    }
}