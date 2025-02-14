using SnackTech.Orders.Common.Dto.Api;
using SnackTech.Orders.Common.Dto.ApiSources.Payments;
using SnackTech.Orders.Common.Interfaces.ApiSources;
using SnackTech.Orders.Core.Domain.Entities;
using SnackTech.Orders.Core.Interfaces;

namespace SnackTech.Orders.Core.Gateways
{
    internal class PagamentoGateway(IPagamentoApi _pagamentoApi) : IPagamentoGateway
    {
        public async Task<ResultadoOperacao<PagamentoDto>> CriarPagamentoAsync(Pedido pedido)
        {
            var pedidoPagamentoDto = ConverterParaDto(pedido);
            return await _pagamentoApi.CriarPagamentoAsync(pedidoPagamentoDto);
        }

        public static PedidoPagamentoDto ConverterParaDto(Pedido pedido)
        {
            return new PedidoPagamentoDto
            {
                PedidoId = pedido.Id,
                Cliente = ConverterClienteParaDto(pedido.Cliente),
                Itens = pedido.Itens.Select(ConverterItemParaDto)
            };
        }

        public static ClientePagamentoDto ConverterClienteParaDto(Cliente cliente)
        {
            return new()
            {
                Id = cliente.Id,
                Nome = cliente.Nome,
                Email = cliente.Email
            };
        }

        public static PedidoItemPagamentoDto ConverterItemParaDto(PedidoItem pedidoItem)
        {
            return new()
            {
                PedidoItemId = pedidoItem.Id,
                Valor = pedidoItem.Valor()
            };
        }
    }
}
