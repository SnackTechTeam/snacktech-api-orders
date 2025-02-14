using SnackTech.Orders.Common.Dto.Api;
using SnackTech.Orders.Common.Dto.ApiSources.Payments;

namespace SnackTech.Orders.Common.Interfaces.ApiSources
{
    public interface IPagamentoApi
    {
        Task<ResultadoOperacao<PagamentoDto>> CriarPagamentoAsync(PedidoPagamentoDto pedido);
    }
}
