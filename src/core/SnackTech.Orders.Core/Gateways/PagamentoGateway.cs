using SnackTech.Orders.Common.Dto.Api;
using SnackTech.Orders.Common.Interfaces.ApiSources;
using SnackTech.Orders.Core.Domain.Entities;

namespace SnackTech.Orders.Core.Gateways
{
    public class PagamentoGateway(IPagamentoApi _pagamentoApi)
    {
        internal async Task<ResultadoOperacao<string>> IntegrarPedidoAsync(Pedido pedido)
        {
            return await _pagamentoApi.IntegrarPedido(PedidoGateway.ConverterParaDto(pedido));
        }
    }
}
