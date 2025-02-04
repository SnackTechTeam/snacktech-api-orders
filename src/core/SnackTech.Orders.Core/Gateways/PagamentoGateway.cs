using SnackTech.Orders.Common.Dto.Api;
using SnackTech.Orders.Common.Dto.ApiSources.Payments;
using SnackTech.Orders.Common.Interfaces.ApiSources;
using SnackTech.Orders.Core.Domain.Entities;

namespace SnackTech.Orders.Core.Gateways
{
    public class PagamentoGateway(IPagamentoApi _pagamentoApi)
    {
        internal async Task<ResultadoOperacao<PagamentoDto>> CriarPagamentoAsync(Pedido pedido)
        {
            return await _pagamentoApi.CriarPagamento(ConverterParaDto(pedido));
        }

        internal static PedidoPagamentoDto ConverterParaDto(Pedido pedido)
        {
            return new PedidoPagamentoDto
            {
                PedidoId = pedido.Id,
                Cliente = ConverterClienteParaDto(pedido.Cliente),
                Itens = pedido.Itens.Select(ConverterItemParaDto)
            };
        }

        internal static ClientePagamentoDto ConverterClienteParaDto(Cliente cliente)
        {
            return new()
            {
                Id = cliente.Id,
                Nome = cliente.Nome,
                Email = cliente.Email
            };
        }

        internal static PedidoItemPagamentoDto ConverterItemParaDto(PedidoItem pedidoItem)
        {
            return new()
            {
                PedidoItemId = pedidoItem.Id,
                Valor = pedidoItem.Valor()
            };
        }
    }
}
